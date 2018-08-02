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

namespace HHT
{
    public class TsumikaeIdouFragment : BaseFragment
    {
        private int menuFlag;
        private TextView txtCase, txtOricon, txtIdosu, txtMail, txtSonota
            , txtFuteikei, txtHansoku, txtTc, txtKosu;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_tsumikae_Idou, container, false);

            SetTitle("移動元マテハン");
            menuFlag = savedInstanceState.GetInt("menuFlag");

            txtCase = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_case);
            txtOricon = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_oricon);
            txtIdosu = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_idosu);
            txtMail = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_mail);
            txtSonota = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_sonota);
            txtFuteikei = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_futeikei);
            txtHansoku = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_hansoku);
            txtTc = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_tc);
            txtKosu = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_kosu);


            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Num1)
            {
                StartFragment(FragmentManager, typeof(KosuMenuFragment));
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

                    // Apply data to UI
                    string densoSymbology = barcodeData.SymbologyDenso;
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

        private void ProcessTanpin(string kamotsu_no) {
            // ret = check_scnno(kamotsu_no, 1);
            // ret == 2 then err "同一の商品です。"
            // else 
            // check_todoke(kamotsu_no, 0, tokuiArr);
            // if false then retry

            // if btvQty > 0 Then 便チェック
            // check_bin_no(JOB:tokuiArr[8]) is false Then Return("retry")


            // btvPram = JOB:ht_serial & "," & JOB:ht_program & "," & JOB:sagyousya_cd & "," & JOB:souko_cd & "," & JOB:kamotu_no & "," & JOB:DEF_GYOMU_IDO
            // proc_tumikomi(btvPram,"01") == 0

            /*
            arrTokui[0] = btvTodoke1
	    	arrTokui[1] = btvTodoke2
		    arrTokui[2] = btvDefVendorCd
		    arrTokui[3] = btvVendorCd
		    arrTokui[4] = btvVendorNm
		    arrTokui[5] = btvMtVendorNm
		    arrTokui[6] = btvBunrui
		    arrTokui[7] = btvMatehanCd
		    arrTokui[8] = btvBinNo    
            
            JOB:tmptokui_cd		= JOB:tokuiArr[0]
			JOB:tmptodoke_cd	= JOB:tokuiArr[1]
    		JOB:tsumi_vendor_cd	= JOB:tokuiArr[2]	// デフォルトベンダー
            //JOB:tsumi_vendor_cd	= JOB:tokuiArr[3]	// マテハンベンダー
		    JOB:tsumi_vendor_nm	= JOB:tokuiArr[4]	// デフォルトベンダー名
            //JOB:tsumi_vendor_nm	= JOB:tokuiArr[5]	// マテハンベンダー名
		    btvBunrui			= JOB:tokuiArr[6]
		    JOB:motomate_cd		= JOB:tokuiArr[7]
		    JOB:bin_no			= JOB:tokuiArr[8]

		    JOB:appendFile(JOB:kamotu_no,1)
		    JOB:set_matehan(btvBunrui,0)
		    JOB:motok_su = JOB:motok_su + 1
		    JOB:ko_su = JOB:motok_su

            */
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
                    break;

            }
           
        }
    }
}