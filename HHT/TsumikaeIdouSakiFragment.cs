using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;
using HHT.Resources.Model;

namespace HHT
{
    public class TsumikaeIdouSakiFragment : BaseFragment
    {
        private View view;
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        private int menuFlag, btvQty, btvScnFlg;
        private TextView txtCase, txtOricon, txtIdosu, txtMail, txtSonota
            , txtFuteikei, txtHansoku, txtTc, txtKosu;

        private int case_su, oricon_su, ido_su, mail_su, sonota_su, futeikei_su, hansoku_su, tc_su, kosu;
        private int sk_case_su, sk_oricon_su, sk_ido_su, sk_mail_su, sk_sonota_su, sk_futeikei_su, sk_hansoku_su, sk_tc_su, sk_kosu;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_tsumikae_Idou_moto, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetTitle("移動先マテハン");

            txtCase = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_case);
            txtOricon = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_oricon);
            txtIdosu = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_idosu);
            txtMail = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_mail);
            txtSonota = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_sonota);
            txtFuteikei = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_futeikei);
            txtHansoku = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_hansoku);
            txtTc = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_tc);
            txtKosu = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_kosu);

            Button btnConfirm = view.FindViewById<Button>(Resource.Id.confirmButton);


            txtCase.Text = prefs.GetInt("sk_case_su", 0).ToString();
            txtOricon.Text = prefs.GetInt("sk_oricon_su", 0).ToString();
            txtIdosu.Text = prefs.GetInt("sk_ido_su", 0).ToString();
            txtMail.Text = prefs.GetInt("sk_mail_su", 0).ToString();
            txtSonota.Text = prefs.GetInt("sk_sonota_su", 0).ToString();
            txtFuteikei.Text = prefs.GetInt("sk_futeikei_su", 0).ToString();
            txtHansoku.Text = prefs.GetInt("sk_hansoku_su", 0).ToString();
            txtTc.Text = prefs.GetInt("sk_sonota_su", 0).ToString();
            txtKosu.Text = prefs.GetInt("sk_ko_su", 0).ToString();


            menuFlag = prefs.GetInt("menuFlag", 1);
            btvQty = 0;
            btvScnFlg = 0;


            // 마테한등록일 경우 확정버튼. 
            // 이외에는 완료버튼으로 표시 

            btnConfirm.Click += delegate
            {
                // F1이랑 같은 동작
                if(btvScnFlg > 0)
                {
                    string pTerminalID = "";
                    string pProgramID = "";
                    string pSagyosyaCD = "";
                    string pSoukoCD = "";
                    string pMotoKamotsuNo = "";
                    string pSakiKamotsuNo = "";
                    string pGyomuKbn = "";
                    string pVendorCd = "";

                    if (menuFlag == 1)
                    {
                        //IDOU050 idou050 = WebService.RequestIdou050(pTerminalID, pProgramID, pSagyosyaCD, pSoukoCD, pMotoKamotsuNo, pSakiKamotsuNo, pGyomuKbn, pVendorCd);
                        IDOU050 idou050 = new IDOU050();
                        idou050.poRet = "0";
                        switch (idou050.poRet)
                        {
                            case "2":
                                CommonUtils.AlertDialog(view, "", "移動先の貨物№が見つかりません。", null);
                                break;
                            case "3":
                                CommonUtils.AlertDialog(view, "", "移動先の届先が違います。", null);
                                break;
                            case "4":
                                CommonUtils.AlertDialog(view, "", "移動元と移動先のマテハンが同じです。", null);
                                break;
                            case "6":
                                CommonUtils.AlertDialog(view, "", "バラへの移動は出来ません。", null);
                                break;
                            case "0":
                                CommonUtils.AlertDialog(view, "メッセージ", "移動処理が\n完了しました。", ()=> {
                                    editor.Clear();
                                    editor.Commit();
                                    FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(0).Id, 0);
                                });
                                /*
                                editor.PutString("completeTitle", "メッセージ");
                                editor.PutString("completeMsg", "移動処理が\n完了しました。");
                                editor.Apply();
                                StartFragment(FragmentManager, typeof(CompleteFragment));
                                */
                                break;
                            default:
                                break;
                        }

                    }
                    else if(menuFlag == 2)
                    {
                        IDOU060 idou060 = WebService.RequestIdou060(pTerminalID, pProgramID, pSagyosyaCD, pSoukoCD, pMotoKamotsuNo, pSakiKamotsuNo, pGyomuKbn, pVendorCd);
                        switch (idou060.poRet)
                        {
                            case "2":
                                CommonUtils.AlertDialog(view, "", "移動先の貨物№が見つかりません。", null);
                                break;
                            case "3":
                                CommonUtils.AlertDialog(view, "", "移動先の届先が違います。", null);
                                break;
                            case "4":
                                CommonUtils.AlertDialog(view, "", "移動元と移動先のマテハンが同じです。", null);
                                break;
                            case "6":
                                CommonUtils.AlertDialog(view, "", "バラへの移動は出来ません。", null);
                                break;
                            case "0":
                                StartFragment(FragmentManager, typeof(CompleteFragment));
                                break;
                            default:
                                break;
                        }
                    }
                }
            };

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

                    if (btvScnFlg > 0)
                    {
                        CommonUtils.AlertDialog(view, "エラー", "既にスキャン済みです。", null);
                        return;
                    }
                    
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

        private void ProcessTanpin(string kamotsu_no) {
            
            
            // check_todoke(kamotsu_no, 0, tokuiArr);
            // if false then retry
            string soukouCd = "108";
            string kitakuCd = "2";

            // 貨物番号に紐づく情報を取得する
            IDOU033 idou033 = WebService.RequestIdou033(soukouCd, kitakuCd, kamotsu_no);

            // 得意先、届先が一致するかを確認する
            if ((idou033.tokuisaki_cd == prefs.GetString("tmptokui_cd", "") && idou033.todokesaki_cd == prefs.GetString("tmptodoke_cd", ""))
                || prefs.GetString("tmptokui_cd", "") == "")
            {
                // Do nothing
            }
            else
            {
                CommonUtils.AlertDialog(view, "エラー", "届先が異なります。", null);
                return;
            }

            // 便情報が一致するかを確認する
            if (prefs.GetInt("ko_su", 0) > 0)
            {
                // 便チェック
                if (prefs.GetString("bin_no", "0") != idou033.torikomi_bin)
                {
                    CommonUtils.AlertDialog(view, "エラー", "便が異なります。", null);
                    return;
                }
            }

            List<IDOU010> idou010 = WebService.RequestIdou010(soukouCd, kitakuCd, kamotsu_no);
            
            List<string> kamotuList = prefs.GetStringSet("kamotuList", new List<string>()).ToList();
            if (kamotuList.FindIndex(x => x == idou010[0].matehan) != -1)
            {
                CommonUtils.AlertDialog(view, "エラー", "同一のマテハンです。", null);
                return;
            }

            SetMatehan(idou010[0], 1);
            
            editor.Apply();

            txtKosu.Text = (prefs.GetInt("ko_su", 0) + int.Parse(idou010[0].cnt)).ToString();

            btvScnFlg = 1;

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

        private void SetMatehan(IDOU010 idou010, int flag)
        {
            switch (idou010.bunrui)
            {
                case "01":
                    editor.PutInt("sk_case_su", (prefs.GetInt("sk_case_su", 0) + int.Parse(idou010.cnt)));
                    txtCase.Text = (prefs.GetInt("sk_case_su", 0) + int.Parse(idou010.cnt)).ToString();
                    editor.Apply();
                    break;
                case "02":
                    if (flag == 0)
                    {
                        oricon_su++;
                        sk_oricon_su++;

                        editor.PutInt("oricon_su", oricon_su);
                        editor.PutInt("sk_oricon_su", sk_oricon_su);

                        txtOricon.Text = oricon_su.ToString();

                    }
                    else
                    {
                        sk_oricon_su++;
                        editor.PutInt("sk_oricon_su", sk_oricon_su);
                        txtCase.Text = oricon_su.ToString();
                    }
                    break; // // case 03は存在しない
                case "04":
                    if (flag == 0)
                    {
                        ido_su++;
                        sk_ido_su++;

                        editor.PutInt("ido_su", ido_su);
                        editor.PutInt("sk_ido_su", sk_ido_su);

                        txtIdosu.Text = ido_su.ToString();

                    }
                    else
                    {
                        sk_ido_su++;
                        editor.PutInt("sk_ido_su", sk_ido_su);
                        txtIdosu.Text = sk_ido_su.ToString();
                    }
                    break;
                case "05":
                    if (flag == 0)
                    {
                        mail_su++;
                        sk_mail_su++;

                        editor.PutInt("mail_su", mail_su);
                        editor.PutInt("sk_mail_su", sk_mail_su);

                        txtMail.Text = mail_su.ToString();
                    }
                    else
                    {
                        sk_mail_su++;
                        editor.PutInt("sk_mail_su", sk_mail_su);

                        txtMail.Text = sk_mail_su.ToString();
                    }
                    break;
                case "06":
                    if (flag == 0)
                    {
                        sonota_su++;
                        sk_sonota_su++;

                        editor.PutInt("sonota_su", sonota_su);
                        editor.PutInt("sk_sonota_su", sk_sonota_su);

                        txtSonota.Text = sonota_su.ToString();
                    }
                    else
                    {
                        sk_sonota_su++;
                        editor.PutInt("sk_sonota_su", sk_sonota_su);

                        txtSonota.Text = sk_sonota_su.ToString();
                    }
                    break;
                case "07":
                    if (flag == 0)
                    {
                        futeikei_su++;
                        sk_futeikei_su++;

                        editor.PutInt("futeikei_su", futeikei_su);
                        editor.PutInt("sk_futeikei_su", sk_futeikei_su);

                        txtFuteikei.Text = futeikei_su.ToString();
                    }
                    else
                    {
                        sk_futeikei_su++;

                        editor.PutInt("sk_futeikei_su", sk_futeikei_su);

                        txtFuteikei.Text = sk_futeikei_su.ToString();
                    }
                    break;
                // case 08は存在しない
                case "09":
                    if (flag == 0)
                    {
                        hansoku_su++;
                        sk_hansoku_su++;

                        editor.PutInt("hansoku_su", hansoku_su);
                        editor.PutInt("sk_hansoku_su", sk_hansoku_su);

                        txtHansoku.Text = hansoku_su.ToString();
                    }
                    else
                    {
                        sk_hansoku_su++;

                        editor.PutInt("sk_hansoku_su", sk_hansoku_su);

                        txtHansoku.Text = hansoku_su.ToString();
                    }
                    break;
                default:
                    if (flag == 0)
                    {
                        sonota_su++;
                        sk_sonota_su++;

                        editor.PutInt("sonota_su", sonota_su);
                        editor.PutInt("sk_sonota_su", sk_sonota_su);
                        txtSonota.Text = sonota_su.ToString();
                    }
                    else
                    {
                        sk_sonota_su++;
                        editor.PutInt("sk_sonota_su", sk_sonota_su);
                        txtSonota.Text = sonota_su.ToString();
                    }
                    break;

            }
        }
    }
}