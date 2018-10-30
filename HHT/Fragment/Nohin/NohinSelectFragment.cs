using System;
using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Com.Beardedhen.Androidbootstrap;
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
        private BootstrapEditText etTokuisaki, etTodokesaki, etReceipt;

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

            etTokuisaki = view.FindViewById<BootstrapEditText>(Resource.Id.et_nohinSelect_tokuisaki);
            etTokuisaki.Text = prefs.GetString("def_tokuisaki_cd", "");
            etTodokesaki = view.FindViewById<BootstrapEditText>(Resource.Id.et_nohinSelect_todokesaki);
            etReceipt = view.FindViewById<BootstrapEditText>(Resource.Id.et_nohinSelect_receipt);
            
            BootstrapButton confirm = view.FindViewById<BootstrapButton>(Resource.Id.btn_nohinSelect_confirm);
            confirm.FocusChange += delegate { if (confirm.IsFocused) CommonUtils.HideKeyboard(this.Activity); };
            confirm.Click += delegate {

                // 得意先チェック
                string tokui = etTokuisaki.Text;
                if(tokui == "")
                {
                    ShowDialog("エラー", "得意先コードを入力してください。", () => { etTokuisaki.RequestFocus(); });
                    return;
                }

                // 届先チェック
                string todoke = etTodokesaki.Text;
                if (todoke == "")
                {
                    ShowDialog("エラー", "届先コードを入力してください。", () => { etTodokesaki.RequestFocus(); });
                    return;
                }

                // 受領書チェック
                string jyuryo = etReceipt.Text;
                if (jyuryo == "")
                {
                    ShowDialog("エラー", "受領書をスキャンしてください。", () => { etReceipt.RequestFocus(); });
                    return;
                }

                if (jyuryo[0] != 'J' || jyuryo.Length != 9)
                {
                    ShowDialog("エラー", "受領書ではありません。", () => { etReceipt.RequestFocus(); });
                    return;
                }

                if (jyuryo.Substring(1, 4) != etTokuisaki.Text || jyuryo.Substring(5, 4) != etTodokesaki.Text)
                {
                    ShowDialog("エラー", "納入先店舗が違います。", () => { etReceipt.RequestFocus(); });
                    return;
                }

                Confirm();
                
            };

            etReceipt.KeyPress += (sender, e) => {
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    e.Handled = true;
                    CommonUtils.HideKeyboard(Activity);
                    // 得意先チェック
                    string tokui = etTokuisaki.Text;
                    if (tokui == "")
                    {
                        ShowDialog("エラー", "得意先コードを入力してください。", () => { etTokuisaki.RequestFocus(); });
                        return;
                    }

                    // 届先チェック
                    string todoke = etTodokesaki.Text;
                    if (todoke == "")
                    {
                        ShowDialog("エラー", "届先コードを入力してください。", () => { etTodokesaki.RequestFocus(); });
                        return;
                    }

                    // 受領書チェック
                    string jyuryo = etReceipt.Text;
                    if (jyuryo == "")
                    {
                        ShowDialog("エラー", "受領書をスキャンしてください。", () => { etReceipt.RequestFocus(); });
                        return;
                    }

                    if (jyuryo[0] != 'J' || jyuryo.Length != 9)
                    {
                        ShowDialog("エラー", "受領書ではありません。", () => { etReceipt.RequestFocus(); });
                        return;
                    }

                    
                    if (jyuryo.Substring(1,4) != etTokuisaki.Text || jyuryo.Substring(5, 4) != etTodokesaki.Text)
                    {
                        ShowDialog("エラー", "納入先店舗が違います。", () => { etReceipt.RequestFocus(); });
                        return;
                    }

                    Confirm();

                }
                else
                {
                    e.Handled = false;
                }
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

                    if (etReceipt.HasFocus)
                    {
                        string barcode_toku_todo = data.Substring(1, 8);
                        if (barcode_toku_todo[0] != 'J')
                        {
                            ShowDialog("エラー", "受領書ではありません。", () => { etTodokesaki.RequestFocus(); });
                            return;
                        }

                        if (barcode_toku_todo != etTokuisaki.Text + etTodokesaki.Text)
                        {
                            ShowDialog("エラー", "納入先店舗が違います。", () => { });
                            return;
                        }

                        etReceipt.Text = barcode_toku_todo;
                        Confirm();
                    }
                });
            }
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F4)
            {
                Confirm();
            }
            return true;
        }

        private void Confirm()
        {
            bool hasError = false;
            
            ((MainActivity)this.Activity).ShowProgress("納品情報を確認しています。");

            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(() =>
                    {
                        // Main file
                        MFileHelper mFlieHelper = new MFileHelper();
                        List<MFile> tsumikomiList = mFlieHelper.SelectTsumikomiList(etTokuisaki.Text, etTodokesaki.Text);
                        if (tsumikomiList.Count == 0)
                        {
                            hasError = true;
                            ShowDialog("エラー", "得意先コードが見つかりません。", () => { etTodokesaki.RequestFocus();  });
                            return;
                        }
                        

                        // 店舗到着情報を登録する。
                        TenpoArrive tenpoArrive = new TenpoArrive
                        {
                            pBinNo = tsumikomiList[0].bin_no,
                            pCourse = tsumikomiList[0].course,
                            pSoukoCD = tsumikomiList[0].kenpin_souko,
                            pKitakuCD = tsumikomiList[0].kitaku_cd,
                            pNohinDate = DateTime.Now.ToString("yyyyMMdd"),
                            pNohinTime = DateTime.Now.ToString("HHmm"),
                            pProgramID = "NOH",
                            pSyukaDate = tsumikomiList[0].syuka_date,
                            pTerminalID = prefs.GetString("terminal_id", ""),
                            pTodokesakiCD = tsumikomiList[0].todokesaki_cd,
                            pTokuisakiCD = tsumikomiList[0].tokuisaki_cd
                        };

                        TenpoArriveHelper tenpoArriveHelper = new TenpoArriveHelper();
                        tenpoArriveHelper.Insert(tenpoArrive);

                        editor.PutString("souko_cd", tsumikomiList[0].kenpin_souko);
                        editor.PutString("kitaku_cd", tsumikomiList[0].kitaku_cd);
                        editor.PutString("haiso_date", tsumikomiList[0].syuka_date);
                        editor.PutString("bin_no", tsumikomiList[0].bin_no);
                        editor.PutString("course", tsumikomiList[0].course);
                        editor.PutString("driver_cd", tsumikomiList[0].driver_cd);
                        
                        editor.PutString("tokuisaki_cd", tsumikomiList[0].tokuisaki_cd);
                        editor.PutString("tokuisaki_nm", tsumikomiList[0].tokuisaki_rk);
                        editor.PutString("todokesaki_cd", tsumikomiList[0].todokesaki_cd);
                        editor.PutString("vendor_cd", tsumikomiList[0].vendor_cd);
                        editor.PutString("vendor_nm", tsumikomiList[0].vendor_nm);
                        editor.PutString("mate_vendor_cd", tsumikomiList[0].default_vendor);
                        editor.PutString("mate_vendor_nm", tsumikomiList[0].default_vendor_nm);
                        editor.PutString("mailbag_flg", "1");
                        editor.PutString("nohin_date", DateTime.Now.ToString("yyyyMMdd"));
                        editor.PutString("nohin_time", DateTime.Now.ToString("HHmm"));
                        
                        editor.PutString("bunrui", tsumikomiList[0].bunrui);
                        editor.PutString("matehan_cd", tsumikomiList[0].matehan);
                        
                        editor.PutString("jyuryo", etReceipt.Text);
                        editor.PutBoolean("mailBagFlag", false);
                        
                        editor.Apply();
                        
                    }
                );
                Activity.RunOnUiThread(() => {
                    ((MainActivity)this.Activity).DismissDialog();

                    if (!hasError)
                    {
                        StartFragment(FragmentManager, typeof(NohinMenuFragment));
                    }
                    
                });
               }
            )).Start();
        }
    }
}