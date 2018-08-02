using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public class TsumikomiWorkFragment : BaseFragment
    {
        private View view;
        private EditText etKosu, etCarLabel, etCarry, etKargo, etCard, etBara, etSonata;
        private Button btnConfirm;
        private int kansen_kbn;
        private bool isSagyo5;

        private static string ERR_UPDATE_001 = "更新出来ませんでした。\n再度商品をスキャンして下さい。";


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_tsumikomi_work, container, false);

            SetTitle("積込検品");
            SetFooterText("");

            etKosu = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_kosu);
            etCarLabel = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_carLabel);
            etCarry = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_carry);
            etKargo = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_kargoCar);
            etCard = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_card);
            etBara = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_bara);
            etSonata = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_sonota);

            btnConfirm = view.FindViewById<Button>(Resource.Id.et_tsumikomiWork_confirm);

            // temp
            kansen_kbn = 0;
            isSagyo5 = true;

            // kansenFlag == 0 then TUMIKOMI040
            // TUMIKOMI300

            /*
            If JOB:kansen_kbn eq "0" Then
                ret = JOB:proc_tumikomikenpin(btvPram, "00")	//TUMIKOMI060
            Else
                ret = JOB:proc_tumikomikenpin(btvPram, "10")	//TUMIKOMI310
            EndIf
            If ret == 0 Then
                Return("sagyou6")
            ElseIf ret == 8 Then
                Handy: ShowMessageBox("更新出来ませんでした。\n再度商品をスキャンして下さい。", "confirm")
            */


            return view;
        }


        public override void OnBarcodeDataReceived(BarcodeDataReceivedEvent_ dataReceivedEvent)
        {
            // When Scanner read some data
            IList<BarcodeDataReceivedEvent_.BarcodeData_> listBarcodeData = dataReceivedEvent.BarcodeData;

            foreach (BarcodeDataReceivedEvent_.BarcodeData_ barcodeData in listBarcodeData)
            {
                string densoSymbology = barcodeData.SymbologyDenso;
                string data = barcodeData.Data;
                int barcodeDataLength = data.Length;

                
                this.Activity.RunOnUiThread(() =>
                {

                    if (IsQRCode(densoSymbology))
                    {
                        CommonUtils.AlertDialog(view, "エラー", "QRコードはスキャン出来ません。。", null);
                        return;
                    }

                    if (isSagyo5 == true)
                    {
                        int ret = 0;

                        if (kansen_kbn == 0)
                        {
                            // proc_tumikomikenpin TUMIKOMI060
                        }
                        else
                        {
                            // proc_tumikomikenpin TUMIKOMI310
                        }

                        if (ret == 0)
                        {
                            isSagyo5 = false;
                            CommonUtils.AlertDialog(view, "確認", "出荷ラベルを確認しました。", null);
                        }
                        else if (ret == 8)
                        {
                            CommonUtils.AlertDialog(view, "エラー", "更新出来ませんでした。\n再度商品をスキャンして下さい。", null);
                        }
                    }
                    else
                    {
                        int ret = 0;
                        // data is syaryono

                        // １．積込検品処理を行う
                        if (kansen_kbn == 0)
                        {
                            // 積込検品用Proc
                            // proc_tumikomikenpin TUMIKOMI080
                        }
                        else
                        {
                            // proc_tumikomikenpin TUMIKOMI311
                        }

                        if (ret == 0 || ret == 2)
                        {
                            if (kansen_kbn == 0 && ret == 2)
                            {
                                // alert "積込可能な商品があります。\n積込みを完了\nしますか？"
                                // false => sagyou5
                                // true 

                            }
                        }
                        else
                        {

                        }


                        // 2．メインファイル取得
                        // 3．パスワード取得
                        // 4．メールバック取得
                        // 5．倉庫マスタ取得
                        // 6. FTP接続情報＆BDアドレス取得
                        // 7. ベンダーマテハンマスタ取得
                        // 8. 得意先マスタ取得
                        // 9. 配車テーブルの該当コースの各数量を実績数で更新する
                        /*
                        if (kansen_kbn == 0)
                        {
                            // ret =  proc_tumikomikenpin TUMIKOMI210
                        }
                        else
                        {
                            // ret =proc_tumikomikenpin TUMIKOMI314
                        }

                           ret = 0 or 99 -> comp_OK(),iniZero(4),
                           99 -> tenpo_zan_flg = true or false;
                           alert msg1;

                            not 0, 99 -> btvOkFlg = 1
                         */
                        StartFragment(FragmentManager, typeof(TsumikomiCompleteFragment));
                    }


                });
                
                
            }
        }

        private bool IsQRCode(string densoSymbology)
        {
            return densoSymbology == "Q" || densoSymbology == "G";
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Back)
            {
                if (!isSagyo5)
                {
                    int ret = 0;
                    if (kansen_kbn == 0)
                    {
                        // proc_tumikomikenpin TUMIKOMI090
                    }
                    else
                    {
                        // proc_tumikomikenpin TUMIKOMI312
                    }


                    if (ret == 0)
                    {
                        isSagyo5 = true;
                    }
                    else if (ret == 8)
                    {
                        CommonUtils.AlertDialog(view, "エラー", ERR_UPDATE_001, null);
                    }

                    return false;
                }

            }
            else if (keycode == Keycode.F3)
            {
                if (isSagyo5)
                {
                    //StartFragment(FragmentManager, typeof());
                }
            }

            return true;
        }

    }
}
 