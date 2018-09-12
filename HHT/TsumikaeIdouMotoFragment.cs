﻿using System;
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
    public class TsumikaeIdouMotoFragment : BaseFragment
    {
        private View view;
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        private int menuFlag, btvQty, btvScnFlg;
        private TextView txtCase, txtOricon, txtIdosu, txtMail, txtSonota
            , txtFuteikei, txtHansoku, txtTc, txtKosu;

        private int case_su, oricon_su, ido_su, mail_su, sonota_su, futeikei_su, hansoku_su, tc_su, kosu;
        private int sk_case_su, sk_oricon_su, sk_ido_su, sk_mail_su, sk_sonota_su, sk_futeikei_su, sk_hansoku_su, sk_tc_su, sk_kosu;
        private List<string> kamotuList;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_tsumikae_Idou_moto, container, false);
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

            Button btnConfirm = view.FindViewById<Button>(Resource.Id.confirmButton);
            Button btnMate = view.FindViewById<Button>(Resource.Id.mateButton);

            txtCase.Text = prefs.GetInt("case_su", 0).ToString();
            txtOricon.Text = prefs.GetInt("oricon_su", 0).ToString();
            txtIdosu.Text = prefs.GetInt("ido_su", 0).ToString();
            txtMail.Text = prefs.GetInt("mail_su", 0).ToString();
            txtSonota.Text = prefs.GetInt("sonota_su", 0).ToString();
            txtFuteikei.Text = prefs.GetInt("futeikei_su", 0).ToString();
            txtHansoku.Text = prefs.GetInt("hansoku_su", 0).ToString();
            txtTc.Text = prefs.GetInt("sonota_su", 0).ToString();
            txtKosu.Text = prefs.GetInt("ko_su", 0).ToString();

            menuFlag = prefs.GetInt("menuFlag", 1);
            kamotuList = new List<string>();

            btnConfirm.Click += delegate { if (prefs.GetInt("ko_su", 0) > 0) StartFragment(FragmentManager, typeof(TsumikaeIdouSakiFragment)); };
            btnMate.Click += delegate
            {
                if (prefs.GetInt("ko_su", 0) > 0 && menuFlag == 1)
                {
                    //JOB: from_gyomu = 1
                    //Return("sagyou3")
                    StartFragment(FragmentManager, typeof(TsumikaeIdouMatehanFragment));
                }
            };

            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Enter)
            {
                if (prefs.GetInt("ko_su", 0) > 0)
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

                    StartFragment(FragmentManager, typeof(TsumikaeIdouSakiFragment));
                }

            }
            else if (keycode == Keycode.Back)
            {
                editor.PutInt("case_su", 0);
                editor.PutInt("sk_case_su", 0);
                editor.PutInt("oricon_su", 0);
                editor.PutInt("sk_oricon_su", 0);
                editor.PutInt("ido_su", 0);
                editor.PutInt("sk_ido_su", 0);
                editor.PutInt("mail_su", 0);
                editor.PutInt("sk_mail_su", 0);
                editor.PutInt("sonota_su", 0);
                editor.PutInt("sk_sonota_su", 0);
                editor.PutInt("futeikei_su", 0);
                editor.PutInt("sk_futeikei_su", 0);
                editor.PutInt("hansoku_su", 0);
                editor.PutInt("sk_hansoku_su", 0);
                editor.PutInt("sonota_su", 0);
                editor.PutInt("sk_sonota_su", 0);

                editor.PutInt("ko_su", 0);
                editor.PutInt("sk_ko_su", 0);

                editor.PutInt("motok_su", 0);

                editor.PutStringSet("kamotuList", new List<string>());
                editor.Apply();

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
                    string soukouCd = "108";
                    string kitakuCd = "2";

                    if (menuFlag == 1)
                    {
                        ProcessTanpin(kamotsu_no);
                    }
                    else if (menuFlag == 2)
                    {
                        // 同じ貨物かを確認
                        if (kamotuList.FindIndex(x => x == kamotsu_no) != -1)
                        {
                            CommonUtils.AlertDialog(view, "", "同一の商品です。", null);
                            return;
                        }

                        // 貨物番号に紐づく情報を取得する
                        List<IDOU020> idou020List = WebService.RequestIdou020(soukouCd, kitakuCd, kamotsu_no);

                        if (idou020List.Count == 0)
                        {
                            CommonUtils.AlertDialog(view, "", "移動元の貨物Noが見つかりません。", null);
                            return;
                        }
                        else
                        {
                            kamotuList.Add(kamotsu_no);
                        }

                        foreach(IDOU020 idou020 in idou020List)
                        {
                            string btvBunrui = idou020.bunrui;
                            string btvBunruiNm = idou020.bunrui_nm;
                            string btvMatehan = idou020.matehan;
                            int btvMateSu = int.Parse(idou020.cnt);
                            string btvBaraMatehan = idou020.bara_matehan;

                            switch (btvBunrui)
                            {
                                case "01":
                                    case_su = case_su + btvMateSu;
                                    sk_case_su = case_su;
                                    txtCase.Text = case_su.ToString();
                                    break;
                                case "02":
                                    oricon_su = oricon_su + btvMateSu;
                                    sk_oricon_su = oricon_su;
                                    txtOricon.Text = oricon_su.ToString();
                                    break; // // case 03は存在しない
                                case "04":
                                    ido_su = ido_su + btvMateSu;
                                    sk_ido_su = ido_su;
                                    txtIdosu.Text = ido_su.ToString();
                                    break;
                                case "05":
                                    mail_su = mail_su + btvMateSu;
                                    sk_mail_su = mail_su;
                                    txtMail.Text = mail_su.ToString();
                                    break;
                                case "06":
                                    sonota_su = sonota_su + btvMateSu;
                                    sk_sonota_su = sonota_su;
                                    txtSonota.Text = sonota_su.ToString();
                                    break;
                                case "07":
                                    futeikei_su = futeikei_su + btvMateSu;
                                    sk_futeikei_su = futeikei_su;
                                    txtFuteikei.Text = futeikei_su.ToString();
                                    break;
                                // case 08は存在しない
                                case "09":
                                    hansoku_su = hansoku_su + btvMateSu;
                                    sk_hansoku_su = hansoku_su;
                                    txtHansoku.Text = hansoku_su.ToString();
                                    break;
                                default:
                                    sonota_su = sonota_su + btvMateSu;
                                    sk_sonota_su = sonota_su;
                                    txtSonota.Text = sonota_su.ToString();
                                    break;
                            }

                            kosu = kosu + btvMateSu;
                            //JOB: motok_su = JOB:motok_su + btvMateSu
                        }

                        editor.PutInt("case_su", case_su);
                        editor.PutInt("sk_case_su", sk_case_su);
                        editor.PutInt("oricon_su", oricon_su);
                        editor.PutInt("sk_oricon_su", sk_oricon_su);
                        editor.PutInt("ido_su", ido_su);
                        editor.PutInt("sk_ido_su", sk_ido_su);
                        editor.PutInt("mail_su", mail_su);
                        editor.PutInt("sk_mail_su", sk_mail_su);
                        editor.PutInt("sonota_su", sonota_su);
                        editor.PutInt("sk_sonota_su", sk_sonota_su);
                        editor.PutInt("futeikei_su", futeikei_su);
                        editor.PutInt("sk_futeikei_su", sk_futeikei_su);
                        editor.PutInt("hansoku_su", hansoku_su);
                        editor.PutInt("sk_hansoku_su", sk_hansoku_su);
                        editor.PutInt("sonota_su", sonota_su);
                        editor.PutInt("sk_sonota_su", sk_sonota_su);
                        editor.Apply();

                        StartFragment(FragmentManager, typeof(TsumikaeIdouSakiFragment));
                    }
                    else if (menuFlag == 3)
                    {

                    }

                });
            }
        }

        private void ProcessTanpin(string kamotsu_no) {

            string soukouCd = "108";
            string kitakuCd = "2";

            // 同じ貨物かを確認
            List<string> kamotuList = prefs.GetStringSet("kamotuList", new List<string>()).ToList();
            List<string> motomateCdList = prefs.GetStringSet("motoMateCdList", new List<string>()).ToList();

            if (kamotuList.FindIndex(x => x == kamotsu_no) != -1)
            {
                CommonUtils.AlertDialog(view, "", "同一の商品です。", null);
                return;
            }

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

            string pTerminalID = "";
            string pProgramID = "";
            string pSagyosyaCD = "";
            string pSoukoCD = "";
            string pMotoKamotsuNo = "";
            string pGyomuKbn = "";

            //IDOU040 idou040 = WebService.RequestIdou040(pTerminalID, pProgramID, pSagyosyaCD, pSoukoCD, pMotoKamotsuNo, pGyomuKbn);
            IDOU040 idou040 = new IDOU040();
            idou040.poRet = "0";
            if (idou040.poRet == "1")
            {
                CommonUtils.AlertDialog(view, "Error", "移動元の貨物Noが見つかりません。", null);
                return;
            }

            SetMatehan(idou033.bunrui, 0);
            motomateCdList.Add(idou033.matehan);
            kamotuList.Add(kamotsu_no);

            editor.PutString("tmptokui_cd", idou033.tokuisaki_cd);
            editor.PutString("tmptodoke_cd", idou033.todokesaki_cd);
            editor.PutString("tsumi_vendor_cd", idou033.default_vendor);
            editor.PutString("tsumi_vendor_nm", idou033.vendor_nm);
            editor.PutString("btvBunrui", idou033.bunrui);
            editor.PutString("motomate_cd", idou033.matehan);
            editor.PutString("bin_no", idou033.torikomi_bin);
            editor.PutInt("motok_su", prefs.GetInt("motok_su", 0) + 1);
            editor.PutInt("ko_su", prefs.GetInt("motok_su", 0) + 1);
            editor.PutInt("sk_ko_su", prefs.GetInt("motok_su", 0) + 1);
            editor.PutStringSet("motomateCdList", motomateCdList);
            editor.PutStringSet("kamotuList", kamotuList);
            editor.Apply();

            txtKosu.Text = (prefs.GetInt("ko_su", 0)).ToString();

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
                    if (resultArray.Length == 2 && resultArray[0] == "0" && resultArray[1].Length > 0)
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

            switch (bunrui) {
                case "01":
                    if (flag == 0)
                    {
                        case_su++;
                        sk_case_su++;

                        editor.PutInt("case_su", case_su);
                        editor.PutInt("sk_case_su", sk_case_su);

                        txtCase.Text = case_su.ToString();
                    }
                    else
                    {
                        sk_case_su++;
                        editor.PutInt("sk_case_su", sk_case_su);
                        txtCase.Text = case_su.ToString();
                    }
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