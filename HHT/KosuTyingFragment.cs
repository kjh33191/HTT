using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using HHT.Resources.Model;
using Android.Content;
using Android.Preferences;

namespace HHT
{
    public class TodokeTyingWorkFragment : BaseFragment
    {
        private View view;
        private int kosuMenuflag;
        private int totalCount;
        private TextView txtMiseName, txtTenpoLocation, txtCase, txtHuteikei
            , txtMiseidou, txtHansoku, txtTotal
            , txtOricon, txtHazai, txtHenpin, txtKaisyu, txtDaisu;
        private Button btnStop, btnCancel, btnMantan;
        private GridLayout gdTyingCanman;
        private int kosuMax;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
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
            }
            else if (kosuMenuflag == (int)Const.KOSU_MENU.BARA)
            {
                SetTitle("バラ検品");
                SetFooterText("");
            }
            else
            {
                FragmentManager.PopBackStack();
            }
        }

        private void InitComponents()
        {
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
            btnCancel = view.FindViewById<Button>(Resource.Id.btn_todoke_cancel);
            btnMantan = view.FindViewById<Button>(Resource.Id.btn_todoke_mantan);
            gdTyingCanman = view.FindViewById<GridLayout>(Resource.Id.gd_tying_canman);

            // 中断
            btnStop.Click += delegate { StartFragment(FragmentManager, typeof(KosuTyingConfrimFragment)); };
            btnCancel.Click += delegate { Cancel(); };　// 取消
            btnMantan.Click += delegate { GoMantanPage(); }; // 満タン

            string miseName = "株）ＰＡＬＴＡＣ";
            txtMiseName.Text = "■　" + miseName + "店";

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

            if (txtTotal.Text != "0")
            {
                SetFooterText("    F2 :取消          F3:満タン");
                gdTyingCanman.Visibility = ViewStates.Visible;
                btnStop.Visibility = ViewStates.Gone;
            }

        }

        public void SetBarcodeSetting()
        {
            /*
             If JOB:menu_flg == JOB:MENU_TODOKE Then
                OutputText("")
            Else
                OutputText(JOB:vendor_nm)
            End If

            OutputText(tokuisaki_nm)
            "店舗ロケ：" + loca_cd

            If btvQtyDisp == 0 Then
                OutputText(" F1:中断            ")
            Else
                OutputText(" F2 :取消  F3:満タン ")
            End If

            ///// barcode setting 
            // CODE128･ITFを有効
            JAN:enable = False
            CODE39:enable = False
            With EAN128
            :enable = True :useCD = True :includeCD = False :separator = 32//' '
            If ( :max > 1) Then  :min = 1 :max = 100  Else  :max = 100:min = 1  EndIf
            EndWith
            CODE128:enable = True
            ITF:enable = True
            NW7:enable = False
            CODE93:enable = False
            TOF:enable = False
            COOP:enable = False
            QR:enable = True
            DataMatrix:enable = False
            Maxi:enable = False
            PDF417:enable = False
            RSS:enable = False
            Composite:enable = False
             */
        }

        public override void OnResume()
        {
            base.OnResume();
            SetKosuMax();
        }

        public void CountItem(IList<BarcodeDataReceivedEvent_.BarcodeData_> listBarcodeData)
        {
            foreach (BarcodeDataReceivedEvent_.BarcodeData_ barcodeData in listBarcodeData)
            {
                this.Activity.RunOnUiThread(() =>
                {

                    // Apply data to UI
                    string densoSymbology = barcodeData.SymbologyDenso;
                    string kamotsu_no = barcodeData.Data;

                    string result = "";

                    // カテゴリ・定特混在チェック
                    if (kosuMenuflag == (int)Const.KOSU_MENU.TODOKE)
                    {
                        result = ""; //proc_kosukenpin("070")
                    }
                    else if(kosuMenuflag == (int)Const.KOSU_MENU.VENDOR)
                    {
                        result = ""; //proc_kosukenpin("150")
                    }
                    else if (kosuMenuflag == (int)Const.KOSU_MENU.BARA)
                    {
                        //何もしない？
                    }
                    
                    

                    string resultData = "{" +
                        "labelType:'0'" +
                        "}";

                    // if(erRet == 0)
                    KosuKenpin kosuKenpin = JsonConvert.DeserializeObject<KosuKenpin>(resultData);
                    if (kosuKenpin != null)
                    {
                        switch (kosuKenpin.labelType)
                        {
                            case 0:
                                txtCase.Text = (Int32.Parse(txtCase.Text) + 1).ToString();
                                break;
                            case 1:
                                txtOricon.Text = (Int32.Parse(txtOricon.Text) + 1).ToString();
                                break;
                            case 2:
                                txtHuteikei.Text = (Int32.Parse(txtHuteikei.Text) + 1).ToString();
                                break;
                            case 3:
                                txtMiseidou.Text = (Int32.Parse(txtMiseidou.Text) + 1).ToString();
                                break;
                            case 4:
                                txtHazai.Text = (Int32.Parse(txtHazai.Text) + 1).ToString();
                                break;
                            case 5:
                                txtHenpin.Text = (Int32.Parse(txtHenpin.Text) + 1).ToString();
                                break;
                            case 6:
                                txtHansoku.Text = (Int32.Parse(txtHansoku.Text) + 1).ToString();
                                break;
                            case 7:
                                txtKaisyu.Text = (Int32.Parse(txtKaisyu.Text) + 1).ToString();
                                break;
                        }

                        txtTotal.Text = (Int32.Parse(txtTotal.Text) + 1).ToString();
                        //JOB: scan_ko_su = JOB:scan_ko_su + 1	// スキャンした個数
                        gdTyingCanman.Visibility = ViewStates.Visible;
                        btnStop.Visibility = ViewStates.Gone;
                        SetFooterText("  F2 :取消                    F3:満タン");
                        // if(erRet != 0)
                        // 
                    }

                });
            }
        }

        public override void OnBarcodeDataReceived(BarcodeDataReceivedEvent_ dataReceivedEvent)
        {
            IList<BarcodeDataReceivedEvent_.BarcodeData_> listBarcodeData = dataReceivedEvent.BarcodeData;

            // 最大個数チェック
            kosuMax = 1;
            Int32.TryParse(txtTotal.Text, out totalCount);

            if (totalCount + 1 > kosuMax)
            {
                CommonUtils.AlertConfirm(View, "注意", "検品数が" + kosuMax + "を超えています。続けますか？", (flag) =>
                {
                    if (flag)
                    {
                        CountItem(listBarcodeData);
                    }
                    else
                    {
                        // if menu_flg = vender 
                        // JOB:tsumi_vendor_cd = JOB:vendor_cd
                        // JOB: tsumi_vendor_nm = JOB:vendor_nm
                        // return ("sagyou16")	//満タン処理
                        SetSumQty();
                        StartFragment(FragmentManager, typeof(KosuMantanFragment));
                        return;
                    }
                });
            }
            else
            {
                CountItem(listBarcodeData);
            }
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Back)
            {
                if(txtTotal.Text != "0")
                {
                    CommonUtils.AlertConfirm(view, "確認", "処理を中断して前画面に戻りますか？", (flag) =>
                    {
                        if (flag)
                        {
                            this.Activity.FragmentManager.PopBackStack();
                            editor.Clear();
                            editor.Commit();
                        }
                    });
                }
                
                return true;
            }
            else if (keycode == Keycode.F1)
            {
                StartFragment(FragmentManager, typeof(KosuTyingConfrimFragment));
            }
            else if (keycode == Keycode.F2)
            {
                Cancel(); // 取消
            }
            else if (keycode == Keycode.F3)
            {
                GoMantanPage(); // 満タン
            }

            return true;
        }

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

        private void SetKosuMax()
        {
            //TODO
            //SearchLine("KOSU210", 4, "")
            string result = "result";
            
            if (result.Length <= 0)
            {
                CommonUtils.ShowAlertDialog(View, "エラー", "個数上限値がみつかりません。");
            }
            else
            {
                kosuMax = 10;
            }
        }

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

        private void Cancel()
        {
            if (txtTotal.Text == "0")
            {
                return;
            }

            CommonUtils.AlertConfirm(view, "確認", "スキャンデータを取り消します。\nよろしいですか？", (flag) =>
            {
                // TODOKE(proc_kosukenpin(085))
                // vendor(proc_kosukenpin(165))

                // erRet == 0 Then
                ScanCntReset();
                // loca_cd= "";
                // If JOB:menu_flg == JOB:MENU_VENDOR THEN
                // JOB: tokuisaki_cd = ""
                // JOB: todokesaki_cd = ""
                // JOB: tokuisaki_nm = ""

                // erRet == 7 Then
                // デッドロック発生
                // i = i + 1
                // If i > 2 Then
                // DEVICE:read_NG()
                // Handy: ShowMessageBox("更新出来ませんでした。\n管理者に連絡してください。", "confirm")

                // else (0,7 x)
                // ShowMessageBox("取消処理が失敗しました。","confirm")

            });
        }

        private void GoMantanPage()
        {
            if (txtTotal.Text != "0")
            {
                //DEVICE:read_OK()
                SetSumQty();
                //IF JOB:menu_flg == JOB:MENU_VENDOR Then
                //JOB: tsumi_vendor_cd = JOB:vendor_cd
                //JOB:tsumi_vendor_nm = JOB:vendor_nm

                // 満タン(sagyou16)
                StartFragment(FragmentManager, typeof(KosuMantanFragment));
            }
        }
    }
}
 