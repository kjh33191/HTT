using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using HHT.Resources.Model;
using System.Threading;
using Com.Beardedhen.Androidbootstrap;

namespace HHT
{
    public class KosuBinInputFragment : BaseFragment
    {
        private View view;
        private int menuKbn;
        private string deliveryDate, tokuisaki, todokesaki;
        private BootstrapEditText etDeliveryDate, etBinNo;
        private BootstrapButton btnConfirm;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_kosu_bin_input, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetTitle("届先指定検品");
            SetFooterText("");
            
            deliveryDate = prefs.GetString("deliveryDate", "");
            tokuisaki = prefs.GetString("tokuisaki", "");
            todokesaki = prefs.GetString("todokesaki", "");
            menuKbn = prefs.GetInt("menuKbn", 1);

            etDeliveryDate = view.FindViewById<BootstrapEditText>(Resource.Id.syukaDate);
            etDeliveryDate.Text = deliveryDate;
            
            etBinNo = view.FindViewById<BootstrapEditText>(Resource.Id.binNo);
            etBinNo.KeyPress += (sender, e) => {
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    e.Handled = true;
                    Confirm();
                }
                else
                {
                    e.Handled = false;
                }
            };
            etBinNo.RequestFocus();

            btnConfirm = view.FindViewById<BootstrapButton>(Resource.Id.btn_binInput_confirm);
            btnConfirm.Click += delegate { Confirm(); };

            return view;
        }

        private void Confirm()
        {
            if (etBinNo.Text == "")
            {
                ShowDialog("エラー", "便番号が入力されていません。", null);
                Vibrate();
            }
            else
            {
                if (menuKbn == 2) // 届先検索の場合
                {
                    editor.PutString("syuka_date", etDeliveryDate.Text.Replace("/", ""));
                    editor.PutString("bin_no", etBinNo.Text);

                    editor.Apply();

                    StartFragment(FragmentManager, typeof(KosuSearchFragment));
                }
                else
                {
                    GetTokuisakiMasterInfo();
                }
            }
        }

        private void GetTokuisakiMasterInfo()
        {
            ((MainActivity)this.Activity).ShowProgress("検品情報を確認しています。");
            
            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(() =>
                {
                    Thread.Sleep(1000);

                    string souko_cd = prefs.GetString("souko_cd", "");
                    string kitaku_cd = prefs.GetString("kitaku_cd", "");
                    string syuka_date = etDeliveryDate.Text.Replace("/", "");
                    string bin_no = etBinNo.Text;
                    string tokuisaki_cd = prefs.GetString("tokuisaki_cd", "");
                    string todokesaki_cd = prefs.GetString("todokesaki_cd", "");

                    try
                    {
                        int status = WebService.RequestKosu040(souko_cd, kitaku_cd, syuka_date, bin_no, tokuisaki_cd, todokesaki_cd);
                        
                        if (status == 99)
                        {
                            ShowDialog("エラー", "検品可能なデータがありません。", null);
                        }
                        else if (status >= 1)
                        {
                            ShowDialog("報告", "全ての検品が完了しています。", null);
                        }
                        else
                        {
                            KOSU050 kosu050 = WebService.RequestKosu050(tokuisaki_cd, todokesaki_cd);
                            editor.PutString("tokuisaki_nm", kosu050.tokuisaki_rk);
                            editor.PutString("default_vendor", kosu050.default_vendor);
                            editor.PutString("vendor_cd", kosu050.default_vendor);
                            editor.PutString("vendor_nm", kosu050.vendor_nm);
                            editor.PutString("syuka_date", syuka_date);
                            editor.PutString("bin_no", bin_no);
                            
                            editor.Apply();
                            
                            StartFragment(FragmentManager, typeof(KosuWorkFragment));
                        }
                    }
                    catch
                    {
                        ShowDialog("エラー", "検品可能なデータがありません。", null);
                    }
                }
                );
            Activity.RunOnUiThread(() =>
            {
                ((MainActivity)this.Activity).DismissDialog();
            });
            }
            )).Start();
        }

        public override void OnResume()
        {
            base.OnResume();
        }
        

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F4)
            {
                Confirm();
            }
            return true;
        }
    }
}
 