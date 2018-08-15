using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;

namespace HHT
{
    public class KosuBinInputFragment : BaseFragment
    {
        private View view;
        private int menuKbn;
        private string deliveryDate, tokuisaki, todokesaki;
        private EditText etDeliveryDate, etBinNo;
        private Button btnConfirm;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            SetTitle("届先指定検品");
            SetFooterText("F4:確定");
            
            view = inflater.Inflate(Resource.Layout.fragment_kosu_bin_input, container, false);

            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            deliveryDate = prefs.GetString("deliveryDate", "");
            tokuisaki = prefs.GetString("tokuisaki", "");
            todokesaki = prefs.GetString("todokesaki", "");
            menuKbn = prefs.GetInt("menuKbn", 1);

            etDeliveryDate = view.FindViewById<EditText>(Resource.Id.et_binInput_deliveryDate);
            etDeliveryDate.Text = deliveryDate;
            
            etBinNo = view.FindViewById<EditText>(Resource.Id.et_binInput_binNo);
            etBinNo.RequestFocus();

            btnConfirm = view.FindViewById<Button>(Resource.Id.btn_binInput_confirm);
            btnConfirm.Click += delegate { Confirm(); };

            return view;
        }

        private void Confirm()
        {
            if (etBinNo.Text == "")
            {
                CommonUtils.AlertDialog(view, "エラー", "便番号が入力されていません。", null);
            }
            else
            {
                if (menuKbn == 2) // 届先検索の場合
                {
                    editor.PutString("syuka_date", "20" + etDeliveryDate.Text.Replace("/", ""));
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
            var progress = ProgressDialog.Show(this.Activity, null, "検品情報を確認しています。", true);

            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(() =>
                {
                    Thread.Sleep(1000);

                    Dictionary<string, string> param = new Dictionary<string, string>
                    {
                        { "kenpin_souko",  prefs.GetString("souko_cd", "108")},
                        { "kitaku_cd",  prefs.GetString("kitaku_cd", "2")},
                        { "syuka_date",  "20" + etDeliveryDate.Text.Replace("/","")},
                        { "bin_no",  etBinNo.Text},
                        { "tokuisaki_cd",  prefs.GetString("tokuisaki_cd", "0000")},
                        { "todokesaki_cd",  prefs.GetString("todokesaki_cd", "0374")}
                    };

                    string resultJson = "";
                    resultJson = CommonUtils.Post(WebService.KOSU.KOSU040, param);
                    Dictionary<string, string> result = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultJson);

                    int status = int.Parse(result["state"]);

                    if (status == 99)
                    {
                        CommonUtils.AlertDialog(view, "エラー", "検品可能なデータがありません。", null);
                    }
                    else if (status >= 1)
                    {
                        CommonUtils.AlertDialog(view, "確認", "全ての検品が完了しています。", null);
                    }
                    else
                    {
                        resultJson = CommonUtils.Post(WebService.KOSU.KOSU050, param);
                        result = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultJson);

                        editor.PutString("tokuisaki_nm", result["tokuisaki_rk"]);
                        editor.PutString("default_vendor", result["default_vendor"]);
                        editor.PutString("vendor_nm", result["vendor_nm"]);

                        editor.PutString("syuka_date", "20" + etDeliveryDate.Text.Replace("/", ""));
                        editor.PutString("bin_no", etBinNo.Text);

                        editor.Apply();

                        StartFragment(FragmentManager, typeof(KosuConfirmFragment));
                    }
                }
                );
            Activity.RunOnUiThread(() =>
            {
                progress.Dismiss();
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
 