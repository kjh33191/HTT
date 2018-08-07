using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Threading;

namespace HHT
{
    public class KosuBinInputFragment : BaseFragment
    {
        private View view;
        private bool confirmFlag;
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

            // parameter setting 
            confirmFlag = prefs.GetBoolean("isConfirm", false);
            deliveryDate = prefs.GetString("deliveryDate", "");
            tokuisaki = prefs.GetString("tokuisaki", "");
            todokesaki = prefs.GetString("todokesaki", "");
            etDeliveryDate = view.FindViewById<EditText>(Resource.Id.et_binInput_deliveryDate);
            etDeliveryDate.Text = deliveryDate;
            
            etBinNo = view.FindViewById<EditText>(Resource.Id.et_binInput_binNo);
            etBinNo.RequestFocus();

            btnConfirm = view.FindViewById<Button>(Resource.Id.btn_binInput_confirm);
            btnConfirm.Click += delegate {
                if(etBinNo.Text== "")
                {
                    CommonUtils.AlertDialog(view, "エラー", "便番号が入力されていません。", null);
                }
                else
                {
                    GetTokuisakiMasterInfo();
                }
                
            };

            return view;
        }



        private void GetTokuisakiMasterInfo()
        {
            // if(ret_menukbn == "2") sagyou5
            
            var progress = ProgressDialog.Show(this.Activity, null, "検品情報を確認しています。", true);

            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread( () =>
                {
                    Thread.Sleep(1000);
                    Dictionary<string, string> param = new Dictionary<string, string>
                    {
                        { "souko_cd",  prefs.GetString("tokuisaki_cd", "103")},
                        { "kitaku_cd",  prefs.GetString("tokuisaki_cd", "103")},
                        { "syuka_date",  prefs.GetString("tokuisaki_cd", "103")},
                        { "bin_no",  prefs.GetString("tokuisaki_cd", "103")}
                    };

                    string resultJson = "";
                    //resultJson = await CommonUtils.PostAsync(WebService.KOSU.KOSU040, param);
                    //Dictionary<string, string> result = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultJson);
                    int status = 0; // result["state"]
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
                        param = new Dictionary<string, string>
                        {
                            { "souko_cd",  prefs.GetString("tokuisaki_cd", "103")},
                            { "kitaku_cd",  prefs.GetString("tokuisaki_cd", "103")},
                            { "syuka_date",  prefs.GetString("tokuisaki_cd", "103")},
                            { "bin_no",  prefs.GetString("tokuisaki_cd", "103")}
                        };

                        //resultJson = await CommonUtils.PostAsync(WebService.KOSU.KOSU050, param);
                        //Dictionary<string, string> result = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultJson);

                        editor.PutString("tokuisaki_nm", etBinNo.Text); // result["tokuisaki_rk"]
                        editor.PutString("tsumi_vendor_cd", etBinNo.Text); // result["default_vendor"]
                        editor.PutString("tsumi_vendor_nm", etBinNo.Text); // result["vendor_nm"]
                        editor.PutString("binNo", etBinNo.Text);
                        editor.Apply();

                        if (confirmFlag)
                        {
                            StartFragment(FragmentManager, typeof(KosuConfirmFragment));
                        }
                        else
                        {
                            StartFragment(FragmentManager, typeof(KosuSearchFragment));
                        }
                    }
                }
                );
                Activity.RunOnUiThread(() => progress.Dismiss());
            }
            )).Start();
        }


        public override void OnResume()
        {
            base.OnResume();
        }
        

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F1)
            {
                
            }
            else if (keycode == Keycode.F3)
            {
               
            }
            else if (keycode == Keycode.Back)
            {
               
            }

            return true;
        }
    }
}
 