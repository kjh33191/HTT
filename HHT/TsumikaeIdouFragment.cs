using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;
using HHT.Resources.Model;

namespace HHT
{
    public class TsumikaeIdouFragment : BaseFragment
    {
        private View view;
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        private int menuFlag, btvQty, btvScnFlg;
        private TextView txtCase, txtOricon, txtIdosu, txtMail, txtSonota
            , txtFuteikei, txtHansoku, txtTc, txtKosu;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_tsumikae_Idou, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetTitle("移動元マテハン");
            
            txtCase = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_case);
            txtOricon = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_oricon);
            txtIdosu = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_idosu);
            txtMail = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_mail);
            txtSonota = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_sonota);
            txtFuteikei = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_futeikei);
            txtHansoku = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_hansoku);
            txtTc = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_tc);
            txtKosu = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_kosu);

            menuFlag = prefs.GetInt("menuFlag", 1);
            btvQty = 0;
            /*
            If btvQty > 0 And JOB:menu_flg == 1 Then
                OutputText(" ENT:確 F3:マテ L:戻")
            ElseIf btvQty > 0 Then
                OutputText("  ENT:確定  L:戻る  ")
            Else
                OutputText("            L:戻る  ")
            EndIf
            */

            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F1)
            {
                if (btvScnFlg > 0)
                {
                    if (menuFlag == 1)
                    {
                        // get_kamotuno()

                        // ido file안에서 레코드 갯수를 세아린다.
                        // 모든 카모츠컬럼을 이용해 proc_tumikomi(btvPram,"02") 실행하는데
                        // 0이 아닌 값이 돌아오면 중단하고 2를 리턴
                        // 모두 정상적으로 종료되면 1을 리턴

                        //if (get_kamotuno() == 1)
                        {
                            //btvScnFlg = 0
                            //Return("msg1")

                            editor.PutString("completeTitle", "メッセージ");
                            editor.PutString("completeMsg", "移動処理が\n完了しました。");
                            editor.Apply();

                            StartFragment(FragmentManager, typeof(CompleteFragment));

                        }
                    }
                    else if (menuFlag == 2)
                    {
                        // IDOU060
                        //btvScnFlg = 0
                        //Return("msg1")

                        /*
                        string pTerminalID = "";
                        string pProgramID = "";
                        string pSagyosyaCD = "";
                        string pSoukoCD = "";
                        string pMotoKamotsuNo = "";
                        string pSakiKamotsuNo = "";
                        string pGyomuKbn = "";
                        string pVendorCd = "";

                        IDOU060 idou060 = WebService.RequestIdou060(pTerminalID, pProgramID, pSagyosyaCD, pSoukoCD, pMotoKamotsuNo, pSakiKamotsuNo, pGyomuKbn, pVendorCd);
                        */

                        editor.PutString("completeTitle", "メッセージ");
                        editor.PutString("completeMsg", "移動処理が\n完了しました。");
                        editor.Apply();

                        StartFragment(FragmentManager, typeof(CompleteFragment));

                    }
                }

            }
            else if (keycode == Keycode.Enter)
            {
                if (btvQty > 0)
                {
                    if (menuFlag == 1)
                    {
                        // 単品
                        // sagyou2
                    }
                    else if (menuFlag == 2)
                    {
                        // sagyou2   
                    }
                    else if (menuFlag == 3)
                    {
                        // sagyou3
                    }

                    StartFragment(FragmentManager, typeof(KosuMenuFragment));
                }
                
            }


            return true;
        }

        public override void OnBarcodeDataReceived(BarcodeDataReceivedEvent_ dataReceivedEvent)
        {
            IList<BarcodeDataReceivedEvent_.BarcodeData_> listBarcodeData = dataReceivedEvent.BarcodeData;


            foreach (BarcodeDataReceivedEvent_.BarcodeData_ barcodeData in listBarcodeData)
            {
                this.Activity.RunOnUiThread(() =>
                {
                    string kamotsu_no = barcodeData.Data;

                    
                    if (menuFlag == 1)
                    {
                        
                        ProcessTanpin(kamotsu_no);
                    }
                    else if (menuFlag == 2)
                    {

                    }
                    else if (menuFlag == 3)
                    {

                    }
                    
                });
            }

        }
        /*
        private int check_scnno(string kamotsu_no, int menuFlag)
        {
            // 単品移動の場合、ido.txt 
            // 全品移動の場合、idox.txt 
            // マテハン移動の場合、idosk.txt 
            // 上記以外はidoxsk.txtに記載するらしい

            // 上記のファイルでおなじ貨物番号が存在する場合、2を戻り値で設定。
            if (menuFlag == 1)
            {
                
            }
        }
        */

        private void ProcessTanpin(string kamotsu_no) {

            // ret = check_scnno(kamotsu_no, 1);
            int ret = 0; // 正常0

            if (ret == 2)
            {
                CommonUtils.AlertDialog(view, "", "同一の商品です。", null);
                return;
            }

            // check_todoke(kamotsu_no, 0, tokuiArr);
            // if false then retry
            string soukouCd = "108";
            string kitakuCd = "2";

            IDOU033 idou033 = WebService.RequestIdou033(soukouCd, kitakuCd, kamotsu_no);
            //idou033 == arrTokui

            // If (btvTodoke1 eq JOB:tmptokui_cd And btvTodoke2 eq JOB:tmptodoke_cd) Or JOB:tmptodoke_cd eq "" Then
            // then 届先が異なります。

            if (btvQty > 0)
            {
                // 便チェック
                // check_bin_no(JOB:tokuiArr[8]) is false Then Return("retry")
                return;
            }

            string pTerminalID = "";
            string pProgramID = "";
            string pSagyosyaCD = "";
            string pSoukoCD = "";
            string pMotoKamotsuNo = "";
            string pGyomuKbn = "";

            IDOU040 idou040 = WebService.RequestIdou040(pTerminalID, pProgramID, pSagyosyaCD, pSoukoCD, pMotoKamotsuNo, pGyomuKbn);
            if (idou040.poRet == "1") {
                // "移動元の貨物№が見つかりません。"
                return;
            }

            editor.PutString("tmptokui_cd", idou033.tokuisaki_cd);
            editor.PutString("tmptodoke_cd", idou033.todokesaki_cd);
            editor.PutString("tsumi_vendor_cd", idou033.default_vendor);
            editor.PutString("tsumi_vendor_nm", idou033.vendor_nm);
            editor.PutString("btvBunrui", idou033.bunrui);
            editor.PutString("motomate_cd", idou033.matehan);
            editor.PutString("bin_no", idou033.torikomi_bin);
            
            appendFile(kamotsu_no, "1");
            SetMatehan(idou033.bunrui, 0);
            
            editor.PutInt("motok_su", prefs.GetInt("motok_su", 0) + 1);
            editor.PutInt("ko_su", prefs.GetInt("motok_su", 0) + 1);

            editor.Apply();
            
        }

        private int Check_scnno(int flag)
        {
            if (flag == 1) // 単
            {
                // filename ido.txt
            }
            else if (flag == 2) // 単
            {
                // filename idosk.txt
            }
            else if (flag == 3) // 単
            {
                // filename idoxsk.txt
            }

            // 만약 파일 크기가 0면 retrun 0;
            // 아니면 
            // file name + .MTP, .ADD삭제 그리고 위의 파일을 열어서 첫번째 항목을 검색
            // GetCnt > 2 return 2; else 0 2는 같은 바코드정보가 존재하기 때문에 에러

            return 0;
        }

        private void Check_todoke()
        {
            string resultData = ""; // IDOU033

        }

        private int Proc_tumikomi(string btvPram, string pno)
        {
            int ret = 1;
            int resultCode = 0;

            switch (pno) {
                case "00":
                    resultCode = 0; //SearchProc("IDOU060", 2, btvPram);
                    if (resultCode == 2)
                    {
                        CommonUtils.AlertDialog(View, "エラー", "移動先の貨物No.が見つかりません。", null);
                    }
                    else if (resultCode == 3)
                    {
                        CommonUtils.AlertDialog(View, "エラー", "移動先の届先が違います。", null);
                    }
                    else if (resultCode == 4)
                    {
                        CommonUtils.AlertDialog(View, "エラー", "移動元と移動先のマテハンが同じです。", null);
                    }
                    else if (resultCode == 5)
                    {
                        CommonUtils.AlertDialog(View, "エラー", "ベンダー別マテハンマスタに該当ベンダーが存在しません。", null);
                    }
                    else if (resultCode == 6)
                    {
                        CommonUtils.AlertDialog(View, "エラー", "バラへの移動は出来ません。", null);
                    }
                    else if (resultCode == 0)
                    {
                        // okCode
                        ret = 0;
                    }
                    break;

                case "01":
                    resultCode = 0; //SearchProc("IDOU040", 2, btvPram);
                    if (resultCode == 0)
                    {
                        ret = 0;
                    }
                    else if (resultCode == 1)
                    {
                        CommonUtils.AlertDialog(View, "エラー", "移動元の貨物No.が見つかりません。", null);
                    }
                    else
                    {
                        CommonUtils.AlertDialog(View, "エラー", "もう一度スキャンしてください。", null);
                    }
                    break;

                case "02":
                    resultCode = 0; //SearchProc("IDOU050", 2, btvPram);
                    if (resultCode == 2)
                    {
                        CommonUtils.AlertDialog(View, "エラー", "移動先の貨物No.が見つかりません。", null);
                    }
                    else if (resultCode == 3)
                    {
                        CommonUtils.AlertDialog(View, "エラー", "移動先の届先が違います。", null);
                    }
                    else if (resultCode == 4)
                    {
                        CommonUtils.AlertDialog(View, "エラー", "移動元と移動先のマテハンが同じです。", null);
                    }
                    else if (resultCode == 6)
                    {
                        CommonUtils.AlertDialog(View, "エラー", "バラへの移動は出来ません。", null);
                    }
                    else if (resultCode == 0)
                    {
                        ret = 0;
                    }
                    else
                    {
                        CommonUtils.AlertDialog(View, "エラー", "移動先の貨物No.が見つかりません。", null);
                    }
                    break;

                case "03":
                    resultCode = 0; //SearchProc("IDOU070", 2, btvPram);
                    if (resultCode == 0)
                    {
                        ret = 0;
                    }
                    else if (resultCode == 5)
                    {
                        CommonUtils.AlertDialog(View, "エラー", "該当ベンダーはマスタに存在しません。", null);
                    }
                    else
                    {
                        CommonUtils.AlertDialog(View, "エラー", "マテハン番号取得に失敗しました。", null);
                    }
                    break;

                case "05":  // マテハン連番取得
                    //resultCode = 0; //SearchProc("IDOU080", 2, btvPram);
                    string result = "0,1";
                    string[] resultArray = result.Split(',');
                    if(resultArray.Length == 2 && resultArray[0] == "0" && resultArray[1].Length > 0)
                    {
                        ret = int.Parse(resultArray[1]);
                    }
                    else
                    {
                        CommonUtils.AlertDialog(View, "エラー", "マテハン連番の取得に失敗しました。", null);
                    }

                    break;

                case "06":  // マテハン登録
                    resultCode = 0; //SearchProc("IDOU090", 2, btvPram);
                    if (resultCode == 2)
                    {
                        CommonUtils.AlertDialog(View, "エラー", "移動先の貨物№が見つかりません。", null);
                    }
                    else if (resultCode == 3)
                    {
                        CommonUtils.AlertDialog(View, "エラー", "移動先の届先が違います。", null);
                    }
                    else if (resultCode == 4)
                    {
                        CommonUtils.AlertDialog(View, "エラー", "移動元と移動先のマテハンが同じです。", null);
                    }
                    else if (resultCode == 0)
                    {
                        ret = 0;
                    }
                    else
                    {
                        CommonUtils.AlertDialog(View, "エラー", "移動先の貨物№が見つかりません。", null);
                    }
                    break;

                default: break;


            }

            return ret;

        }

        private void SetMatehan(string bunrui, int flag)
        {
            
            switch(bunrui){
                case "01":
                    if(flag == 0)
                    {
                        txtCase.Text = (int.Parse(txtCase.Text) + 1).ToString();
                        // case ++
                        // sk_case_se ++
                    }
                    else
                    {
                        // sk_case_se ++
                    }
                    break;
                case "02":
                    if (flag == 0)
                    {
                        // oricon_su ++
                        // sk_oricon_su ++
                    }
                    else
                    {
                        // sk_oricon_su ++
                    }
                    break; // // case 03は存在しない
                case "04":
                    if (flag == 0)
                    {
                        // ido_su ++
                        // sk_ido_su ++
                    }
                    else
                    {
                        // sk_ido_su ++
                    }
                    break;
                case "05":
                    if (flag == 0)
                    {
                        // mail_su ++
                        // sk_mail_su ++
                    }
                    else
                    {
                        // sk_mail_su ++
                    }
                    break;
                case "06":
                    if (flag == 0)
                    {
                        // sonota_su ++
                        // sk_sonota_su ++
                    }
                    else
                    {
                        // sk_sonota_su ++
                    }
                    break;
                case "07":
                    if (flag == 0)
                    {
                        // futeikei_su ++
                        // sk_futeikei_su ++
                    }
                    else
                    {
                        // sk_futeikei_su ++
                    }
                    break;
                    // case 08は存在しない
                case "09":
                    if (flag == 0)
                    {
                        // hansoku_su ++
                        // sk_hansoku_su ++
                    }
                    else
                    {
                        // sk_hansoku_su ++
                    }
                    break;
                default:
                    if (flag == 0)
                    {
                        // sonota_su ++
                        // sk_sonota_su ++
                    }
                    else
                    {
                        // sk_sonota_su ++
                    }
                    break;

            }
        }

        private void appendFile(string kamotu_no , string flag)
        {
            if(flag == "1")
            {
                //単品
                // filenm = "ido"
                // 固定長
                // strStroRec = wMotoKamotuNo & "," & motomate_cd & "\n"
                

            }
            else if (flag == "2")
            {
                //全品
            }
            else if (flag == "3")
            {
                //マテハン
            }
        }
    }
}