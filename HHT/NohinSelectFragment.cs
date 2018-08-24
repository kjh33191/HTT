using System;
using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;
using HHT.Resources.DataHelper;
using HHT.Resources.Model;

namespace HHT
{
    public class NohinSelectFragment : BaseFragment
    {
        private View view;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;
        private EditText etTokuisaki, etTodokesaki, etReceipt;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_nohin_select, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetTitle("納品検品");
            SetFooterText("");

            etTokuisaki = view.FindViewById<EditText>(Resource.Id.et_nohinSelect_tokuisaki);
            etTokuisaki.Text = prefs.GetString("def_tokuisaki_cd", "");
            etTodokesaki = view.FindViewById<EditText>(Resource.Id.et_nohinSelect_todokesaki);
            etReceipt = view.FindViewById<EditText>(Resource.Id.et_nohinSelect_receipt);
            etReceipt.Text = "J00000374";

            Button confirm = view.FindViewById<Button>(Resource.Id.btn_nohinSelect_confirm);
            confirm.Click += delegate {
                
                // Main file
                MFileHelper mFlieHelper = new MFileHelper();
                List<MFile> tsumikomiList = mFlieHelper.SelectTsumikomiList(etTokuisaki.Text, etTodokesaki.Text);

                if (tsumikomiList.Count == 0)
                {
                    CommonUtils.AlertDialog(View, "エラー", "得意先コードが見つかりません。", () => {
                        etTodokesaki.RequestFocus();
                    });
                    return;
                }

                string jyuryo = etReceipt.Text;
                if(jyuryo[0] != 'J')
                {
                    CommonUtils.AlertDialog(View, "エラー", "受領書ではありません。", () => {
                        etTodokesaki.RequestFocus();
                    });
                    return;
                }

                string yyyyMMdd = DateTime.Now.ToString("yyyyMMdd");
                string hhmm = DateTime.Now.ToString("HHmm");

                editor.PutString("tokuisaki_nm", tsumikomiList[0].tokuisaki_rk);
                editor.PutString("vendor_cd", tsumikomiList[0].vendor_cd);
                editor.PutString("vendor_nm", tsumikomiList[0].vendor_nm);
                editor.PutString("mailbag_flg", "1");
                editor.PutString("nohin_date", DateTime.Now.ToString("yyyyMMdd"));
                editor.PutString("nohin_time", DateTime.Now.ToString("HHmm"));
                StartFragment(FragmentManager, typeof(NohinMenuFragment));
                
            };

            etTodokesaki.RequestFocus();

            return view;
        }
        

        public override void OnBarcodeDataReceived(BarcodeDataReceivedEvent_ dataReceivedEvent)
        {
            IList<BarcodeDataReceivedEvent_.BarcodeData_> listBarcodeData = dataReceivedEvent.BarcodeData;

            foreach (BarcodeDataReceivedEvent_.BarcodeData_ barcodeData in listBarcodeData)
            {
                this.Activity.RunOnUiThread(() =>
                {
                    string data = barcodeData.Data;
                    
                });
            }
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Back)
            {
               
            }
            else if (keycode == Keycode.F1)
            {
                /*
                if (etTodokesaki.IsFocused)
                {
                    bool errFlag = false;
                    MainActivity.ShowProgressBar();

                    if (etSyukaDate.Text == "")
                    {
                        CommonUtils.ShowAlertDialog(view, "エラー", "配送日を入力してください。");
                        etSyukaDate.RequestFocus();
                        errFlag = true;
                        return false;
                    }

                    if (etTokuisaki.Text == "")
                    {
                        CommonUtils.ShowAlertDialog(view, "エラー", "得意先コードを入力してください。");
                        etTokuisaki.RequestFocus();
                        errFlag = true;
                        return false;
                    }

                    new Thread(new ThreadStart(delegate {
                        Activity.RunOnUiThread(() =>
                        {
                            if (!IsExistTokuisaki())
                            {
                                CommonUtils.ShowAlertDialog(view, "エラー", "得意先コードがみつかりません。");
                                etTokuisaki.Text = "";
                                etTokuisaki.RequestFocus();
                                errFlag = true;
                                return;
                            }
                        }
                        );
                        Activity.RunOnUiThread(() => MainActivity.HideProgressBar());
                    }
                    )).Start();

                    if (errFlag)
                    {
                        return false;
                    }

                    editor.PutString("deliveryDate", etSyukaDate.Text);
                    editor.PutString("tokuisaki", etTokuisaki.Text);
                    editor.PutString("deliveryDate", etSyukaDate.Text);
                    editor.PutBoolean("isConfirm", false); // 届先検索フラグ設定
                    editor.Apply();

                    StartFragment(FragmentManager, typeof(KosuBinInputFragment));
                }

                if (etVendorCode.IsFocused)
                {
                    GoVendorSearchPage();
                }
                */

            }
            else if (keycode == Keycode.F4)
            {
                Confirm();
            }

            return true;
        }

        private void Confirm()
        {
            var progress = ProgressDialog.Show(this.Activity, null, "納品情報を確認しています。", true);

            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(() =>
                    {
                        
                    }
                );
                Activity.RunOnUiThread(() => progress.Dismiss());
               }
            )).Start();
        }
    }
}