﻿using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;
using System;
using System.Collections.Generic;
using HHT.Resources.Model;
using Android.Content;
using Android.Preferences;
using System.Threading;
using Android.Util;
using HHT.Common;

namespace HHT
{
    public class TodokeTyingWorkFragment : BaseFragment
    {
        private readonly string TAG = "KosuWorkFragment";

        // UI Component
        private View view;
        private TextView txtVendorName, txtMiseName, txtTenpoLocation, txtCase, txtHuteikei
            , txtMiseidou, txtHansoku, txtTotal
            , txtOricon, txtHazai, txtHenpin, txtKaisyu, txtDaisu;
        private Button btnStop, btnCancel, btnMantan, btnComplete;
        private GridLayout gdTyingCanman;

        // For Handling Parameters
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        // Local Parameters
        private int kosuMenuflag;
        private string venderCd;
        private int kosuMax;
        private bool hasKamotsuScanned;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Log.Debug(TAG, "Start OnCreateView ");

            view = inflater.Inflate(Resource.Layout.fragment_kosu_tying, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();
            kosuMenuflag = prefs.GetInt(Const.KOSU_MENU_FLAG, 0); // 画面区分

            // コンポーネント初期化
            InitComponents();

            // 遷移メニューより画面初期処理
            InitScreenByMenu(kosuMenuflag);

            return view;
        }

        private void InitScreenByMenu(int kosuMenuflag)
        {
            kosuMenuflag = prefs.GetInt(Const.KOSU_MENU_FLAG, (int)Const.KOSU_MENU.TODOKE); // 画面区分

            if (kosuMenuflag == (int)Const.KOSU_MENU.TODOKE)
            {
                SetTitle("届先指定検品");
                SetFooterText("F1:中断");
            }
            else if (kosuMenuflag == (int)Const.KOSU_MENU.VENDOR)
            {
                SetTitle("ベンダー指定検品");
                SetFooterText("F1:中断");
                venderCd = prefs.GetString("vendor_cd", "");
                txtVendorName.Text = prefs.GetString("vendor_nm", "");
            }
            else if (kosuMenuflag == (int)Const.KOSU_MENU.BARA)
            {
                SetTitle("バラ検品");
                txtVendorName.Text = prefs.GetString("vendor_nm", "");
                SetFooterText("");
                btnStop.Visibility = ViewStates.Gone;
            }
            else
            {
                FragmentManager.PopBackStack();
            }
        }

        private void InitComponents()
        {
            txtVendorName = view.FindViewById<TextView>(Resource.Id.vendorName);
            txtMiseName = view.FindViewById<TextView>(Resource.Id.txtMiseName);
            txtTenpoLocation = view.FindViewById<TextView>(Resource.Id.txt_todoke_tenpoLocation);
            txtCase = view.FindViewById<TextView>(Resource.Id.txt_todoke_case);
            txtHuteikei = view.FindViewById<TextView>(Resource.Id.txt_todoke_huteikei);
            txtMiseidou = view.FindViewById<TextView>(Resource.Id.txt_todoke_miseidou);
            txtHansoku = view.FindViewById<TextView>(Resource.Id.txt_todoke_hansoku);
            txtTotal = view.FindViewById<TextView>(Resource.Id.txt_todoke_total);

            txtOricon = view.FindViewById<TextView>(Resource.Id.txt_todoke_oricon);
            txtHazai = view.FindViewById<TextView>(Resource.Id.txt_todoke_hazai);
            txtHenpin = view.FindViewById<TextView>(Resource.Id.txt_todoke_henpin);
            txtKaisyu = view.FindViewById<TextView>(Resource.Id.txt_todoke_kaisyu);
            txtDaisu = view.FindViewById<TextView>(Resource.Id.txt_todoke_daisu);

            btnStop = view.FindViewById<Button>(Resource.Id.btn_todoke_stop);
            btnStop.Click += delegate { StartFragment(FragmentManager, typeof(KosuTyingConfrimFragment)); }; // 中断

            btnMantan = view.FindViewById<Button>(Resource.Id.btn_todoke_mantan);　// 届先検品の満タンボタン
            btnMantan.Click += delegate { GoMantanPage(); }; // 満タン

            btnCancel = view.FindViewById<Button>(Resource.Id.btn_todoke_cancel);
            btnCancel.Click += delegate { CancelKamotsuScan(false); };　// 取消

            btnComplete = view.FindViewById<Button>(Resource.Id.completeButton); // バラ検品の完了ボタン
            btnComplete.Click += delegate {
                try
                {
                    KOSU070 kosu070 = WebService.RequestKosu180(GetProcedureParam(""));

                    if (kosu070.poRet == "0")
                    {
                        if (int.Parse(kosu070.poMsg) > 0)
                        {
                            // 完了OK
                            txtCase.Text = "0";
                            txtHuteikei.Text = "0";
                            txtMiseidou.Text = "0";
                            txtHansoku.Text = "0";
                            txtOricon.Text = "0";
                            txtHazai.Text = "0";
                            txtHenpin.Text = "0";
                            txtKaisyu.Text = "0";
                            txtDaisu.Text = (int.Parse(txtDaisu.Text) + 1).ToString();
                            btnComplete.Visibility = ViewStates.Invisible;
                            hasKamotsuScanned = false;
                        }
                        else
                        {
                            // 完了OK
                            editor.PutString("dai_su", (int.Parse(txtDaisu.Text) + 1).ToString());
                            editor.Apply();
                            FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(2).Id, 0);
                        }
                    }
                    else
                    {
                        CommonUtils.AlertDialog(view, "", "更新出来ませんでした。\n管理者に連絡してください。", null);
                    }
                }
                catch
                {
                    CommonUtils.AlertDialog(view, "エラー", "満タン処理完了に失敗しました。", null);
                    Log.Error(Tag, "");
                    return;
                }
            };

            gdTyingCanman = view.FindViewById<GridLayout>(Resource.Id.gd_tying_canman);

            txtMiseName.Text = prefs.GetString("tokuisaki_nm", "");
            txtCase.Text = prefs.GetString("case_su", "0");
            txtOricon.Text = prefs.GetString("oricon_su", "0");
            txtHuteikei.Text = prefs.GetString("futeikei_su", "0");
            txtMiseidou.Text = prefs.GetString("ido_su", "0");
            txtHazai.Text = prefs.GetString("hazai_su", "0");
            txtHenpin.Text = prefs.GetString("henpin_su", "0");
            txtHansoku.Text = prefs.GetString("hansoku_su", "0");
            txtKaisyu.Text = prefs.GetString("kaisyu_su", "0");
            txtTotal.Text = prefs.GetString("ko_su", "0");
            txtDaisu.Text = prefs.GetString("dai_su", "0");

            try
            {
                kosuMax = WebService.RequestKosu210();
            }
            catch
            {
                CommonUtils.ShowAlertDialog(View, "エラー", "個数上限値がみつかりません。");
                kosuMax = 10;
            }

            hasKamotsuScanned = false;
        }

        public override void OnBarcodeDataReceived(BarcodeDataReceivedEvent_ dataReceivedEvent)
        {
            IList<BarcodeDataReceivedEvent_.BarcodeData_> listBarcodeData = dataReceivedEvent.BarcodeData;

            Activity.RunOnUiThread(async () => {
                // 最大個数チェック
                bool isMax = int.Parse(txtTotal.Text) + 1 > kosuMax;

                if (isMax)
                {
                    // 最大検品数を超えた場合
                    var result = await DialogAsync.Show(Activity, "確認", "検品数が" + kosuMax + "を超えています。続けますか？");
                    if (result.Value)
                    {
                        CountItem(listBarcodeData);
                    }
                    else
                    {
                        GoMantanPage();
                    }
                }
                else
                {
                    CountItem(listBarcodeData);
                }
            });
        }


        public void CountItem(IList<BarcodeDataReceivedEvent_.BarcodeData_> listBarcodeData)
        {
            foreach (BarcodeDataReceivedEvent_.BarcodeData_ barcodeData in listBarcodeData)
            {
                string kamotsu_no = barcodeData.Data;

                Dictionary<string, string> param = GetProcedureParam(kamotsu_no);

                KOSU070 kosuKenpin = new KOSU070();
                try
                {
                    if (kosuMenuflag == (int)Const.KOSU_MENU.TODOKE)
                    {
                        kosuKenpin = WebService.RequestKosu070(param);
                    }
                    else if (kosuMenuflag == (int)Const.KOSU_MENU.VENDOR)
                    {
                        kosuKenpin = WebService.RequestKosu150(param);
                    }
                    else if (kosuMenuflag == (int)Const.KOSU_MENU.BARA)
                    {
                        kosuKenpin = WebService.RequestKosu170(param);
                    }

                    if (kosuKenpin.poMsg != "")
                    {
                        CommonUtils.AlertDialog(view, "", kosuKenpin.poMsg, null);
                        return;
                    }

                    switch (kosuKenpin.poLabelType)
                    {
                        case "0":
                            txtCase.Text = (Int32.Parse(txtCase.Text) + 1).ToString();
                            break;
                        case "1":
                            txtOricon.Text = (Int32.Parse(txtOricon.Text) + 1).ToString();
                            break;
                        case "2":
                            txtHuteikei.Text = (Int32.Parse(txtHuteikei.Text) + 1).ToString();
                            break;
                        case "3":
                            txtMiseidou.Text = (Int32.Parse(txtMiseidou.Text) + 1).ToString();
                            break;
                        case "4":
                            txtHazai.Text = (Int32.Parse(txtHazai.Text) + 1).ToString();
                            break;
                        case "5":
                            txtHenpin.Text = (Int32.Parse(txtHenpin.Text) + 1).ToString();
                            break;
                        case "6":
                            txtHansoku.Text = (Int32.Parse(txtHansoku.Text) + 1).ToString();
                            break;
                        case "7":
                            txtKaisyu.Text = (Int32.Parse(txtKaisyu.Text) + 1).ToString();
                            break;
                    }

                    txtMiseName.Text = kosuKenpin.poTokuisakiNm;
                    txtTenpoLocation.Text = kosuKenpin.poTenpoLocaCD;
                    txtTotal.Text = (Int32.Parse(txtTotal.Text) + 1).ToString();
                    editor.PutString("todokesaki_cd", kosuKenpin.poTodokesakiCD);
                    editor.Apply();
                    hasKamotsuScanned = true;

                    if (kosuMenuflag == (int)Const.KOSU_MENU.TODOKE)
                    {
                        SetFooterText("  F2 :取消                    F3:満タン");
                        btnStop.Visibility = ViewStates.Gone;
                        gdTyingCanman.Visibility = ViewStates.Visible;
                    }
                    else if (kosuMenuflag == (int)Const.KOSU_MENU.VENDOR)
                    {
                        SetFooterText("  F3:満タン");
                        btnStop.Visibility = ViewStates.Gone;
                        gdTyingCanman.Visibility = ViewStates.Visible;
                    }
                    else if (kosuMenuflag == (int)Const.KOSU_MENU.BARA)
                    {
                        SetFooterText("");
                        btnComplete.Visibility = ViewStates.Visible;
                    }
                }
                catch
                {
                    CommonUtils.AlertDialog(view, "エラー", "更新出来ませんでした。\n管理者に連絡してください。", null);
                    Log.Error(Tag, "");
                    return;
                }
            }
        }

        // キーハンドリング
        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F1)
            {
                StartFragment(FragmentManager, typeof(KosuTyingConfrimFragment));
            }
            else if (keycode == Keycode.F2)
            {
                CancelKamotsuScan(false); // 取消
            }
            else if (keycode == Keycode.F3)
            {
                GoMantanPage(); // 満タン
            }

            return true;
        }

        // バックボタン押した時
        public override bool OnBackPressed()
        {
            if (hasKamotsuScanned)
            {
                CancelKamotsuScan(true);
            }
            else
            {
                if (kosuMenuflag == (int)Const.KOSU_MENU.BARA)
                {
                    return true;
                }
            }

            return false;
        }

        // 紐づけ情報リセット
        private void ScanCntReset()
        {
            txtCase.Text = "0";
            txtHuteikei.Text = "0";
            txtMiseidou.Text = "0";
            txtHansoku.Text = "0";
            txtTotal.Text = "0";

            txtOricon.Text = "0";
            txtHazai.Text = "0";
            txtHenpin.Text = "0";
            txtKaisyu.Text = "0";
            txtDaisu.Text = "0";

            gdTyingCanman.Visibility = ViewStates.Gone;
            btnStop.Visibility = ViewStates.Visible;
            SetFooterText("F1:中断");
        }

        // 紐づけ情報保存
        public void SetSumQty()
        {
            editor.PutString("case_su", txtCase.Text);
            editor.PutString("oricon_su", txtOricon.Text);
            editor.PutString("futeikei_su", txtHuteikei.Text);
            editor.PutString("ido_su", txtMiseidou.Text);
            editor.PutString("hazai_su", txtHazai.Text);
            editor.PutString("henpin_su", txtHenpin.Text);
            editor.PutString("hansoku_su", txtHansoku.Text);
            editor.PutString("kaisyu_su", txtKaisyu.Text);
            editor.PutString("ko_su", txtTotal.Text);
            editor.PutString("dai_su", txtDaisu.Text);
            editor.Apply();
        }

        // プロシージャパラメータ設定
        private Dictionary<string, string> GetProcedureParam(string kamotsu_no)
        {
            string pSagyosyaCD = prefs.GetString("driver_cd", "");
            string pSoukoCD = prefs.GetString("souko_cd", "");
            string pSyukaDate = prefs.GetString("syuka_date", "");
            string pTokuisakiCD = prefs.GetString("tokuisaki_cd", "0000");
            string pTodokesakiCD = kosuMenuflag == (int)Const.KOSU_MENU.TODOKE ? prefs.GetString("todokesaki_cd", "") : "";
            string pVendorCD = prefs.GetString("vendor_cd", "");
            string pTsumiVendorCD = "";
            string pKamotsuNo = kamotsu_no;
            string pBinNo = prefs.GetString("bin_no", "0");
            string pHHT_No = "11101";

            string pMatehan = "";
            string pJskCaseSu = txtCase.Text;
            string pJskOriconSu = txtOricon.Text;
            string pJskFuteikeiSu = txtHuteikei.Text;
            string pJskHazaiSu = txtHazai.Text;
            string pJskIdoSu = txtMiseidou.Text;
            string pJskHenpinSu = txtHenpin.Text;
            string pJskHansokuSu = txtHansoku.Text;

            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "pTerminalID",  "432660068"},
                { "pProgramID",  "KOS"},
                { "pSagyosyaCD",  pSagyosyaCD},
                { "pSoukoCD",  pSoukoCD},
                { "pSyukaDate",  pSyukaDate},
                { "pTokuisakiCD" ,  pTokuisakiCD},
                { "pTodokesakiCD" ,  pTodokesakiCD},
                { "pVendorCD",  pVendorCD},
                { "pTsumiVendorCD",  pTsumiVendorCD},
                { "pKamotsuNo",  pKamotsuNo},
                { "pBinNo",  pBinNo},
                { "pHHT_No",  "11101"},
                { "pMatehan",  pMatehan},
                { "pJskCaseSu",  pJskCaseSu},
                { "pJskOriconSu",  pJskOriconSu},
                { "pJskFuteikeiSu",  pJskFuteikeiSu},
                { "pJskTcSu",  "0"},
                { "pJskMailbinSu",  "0"},
                { "pJskHazaiSu",  pJskHazaiSu},
                { "pJskIdoSu",  pJskIdoSu},
                { "pJskHenpinSu",  pJskHenpinSu},
                { "pJskHansokuSu",  pJskHansokuSu}
            };

            return param;
        }

        // キャンセル処理
        private void CancelKamotsuScan(bool backFlag)
        {
            string message = backFlag == false ? "スキャンデータを取り消します。\nよろしいですか？" : "スキャンした内容を破棄し、メニューに戻りますか？";

            CommonUtils.AlertConfirm(view, "確認", message, (flag) =>
            {
                if (flag)
                {
                    string errMsg = RequestCancelKamotuScan(GetProcedureParam(""));
                    if (errMsg != "")
                    {
                        CommonUtils.AlertDialog(view, "エラー", errMsg, null);
                        return;
                    }

                    if (backFlag == true)
                    {
                        editor.Remove("case_su");
                        editor.Remove("oricon_su");
                        editor.Remove("futeikei_su");
                        editor.Remove("ido_su");
                        editor.Remove("hazai_su");
                        editor.Remove("henpin_su");
                        editor.Remove("hansoku_su");
                        editor.Remove("kaisyu_su");
                        editor.Remove("ko_su");
                        editor.Remove("dai_su");
                        editor.Commit();

                        this.Activity.FragmentManager.PopBackStack();
                    }
                    else
                    {
                        if (kosuMenuflag != (int)Const.KOSU_MENU.TODOKE)
                        {
                            // 店舗名削除
                            txtMiseName.Text = "";
                        }

                        ScanCntReset();
                        txtTenpoLocation.Text = "";
                    }
                }
            });
        }

        private string RequestCancelKamotuScan(Dictionary<string, string>　param)
        {
            KOSU070 result;
            if (kosuMenuflag == (int)Const.KOSU_MENU.TODOKE)
            {
                result = WebService.RequestKosu085(param);
            }
            else if (kosuMenuflag == (int)Const.KOSU_MENU.VENDOR)
            {
                result = WebService.RequestKosu165(param);
            }
            else
            {
                result = WebService.RequestKosu185(param);
            }

            return result.poMsg;
        }

        private void GoMantanPage()
        {
            if (hasKamotsuScanned)
            {
                SetSumQty();

                if(kosuMenuflag == (int)Const.KOSU_MENU.VENDOR)
                {
                    editor.PutString("tsumi_vendor_cd", prefs.GetString("vendor_cd", ""));
                    editor.PutString("tsumi_vendor_nm", prefs.GetString("vendor_nm", ""));
                    editor.Apply();
                }

                StartFragment(FragmentManager, typeof(KosuMantanFragment));
            }
        }
        
    }
}
 