using System;
using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;
using HHT.Resources.DataHelper;
using HHT.Resources.Model;

namespace HHT
{
    public class TsumikomiWorkFragment : BaseFragment
    {
        private readonly string TAG = "TsumikomiWorkFragment";
        
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        private View view;
        private EditText etKosu, etCarLabel, etCarry, etKargo, etCard, etBara, etSonata;
        private Button btnIdou;
        private string kansen_kbn;

        private string souko_cd, kitaku_cd, syuka_date, tokuisaki_cd, todokesaki_cd, bin_no, course;
        private int zoubin_flg;
        private string matehan;
        private bool carLabelInputMode = false;
        private static readonly string ERR_UPDATE_001 = "更新出来ませんでした。\n再度商品をスキャンして下さい。";

        // Proc TRG - >060(kansen_kbn == 0), 310 ;  
        // syaryou TRG - > 080(kansen_kbn == 0); 311 ; => 210, 314  cancel -> 090, 312

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_tsumikomi_work, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetTitle("積込検品");
            SetFooterText("F3：移動");
            
            view.FindViewById<TextView>(Resource.Id.txt_tsumikomiWork_tokuisakiNm).Text = prefs.GetString("tokuisaki_nm", "");
            etKosu = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_kosu);
            etCarLabel = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_carLabel);
            etCarry = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_carry);
            etKargo = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_kargoCar);
            etCard = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_card);
            etBara = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_bara);
            etSonata = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_sonota);

            btnIdou = view.FindViewById<Button>(Resource.Id.et_tsumikomiWork_idou);
            btnIdou.Click += delegate { StartFragment(FragmentManager, typeof(TsumikomiIdouMenuFragment)); };
            
            souko_cd = prefs.GetString("souko_cd", "");
            kitaku_cd = prefs.GetString("kitaku_cd", "");
            syuka_date = prefs.GetString("syuka_date", "");
            tokuisaki_cd = prefs.GetString("tokuisaki_cd", "");
            todokesaki_cd = prefs.GetString("todokesaki_cd", "");
            bin_no = prefs.GetString("bin_no", "");
            course = prefs.GetString("course", "");

            zoubin_flg = prefs.GetInt("zoubin_flg", 1);
            kansen_kbn = prefs.GetString("kansen_kbn", "1");
            
            if (zoubin_flg == 1)
            {
                GetTenpoMatehanInfo();  // 作業5, 6 定番コース단골코스(zoubin_flg = 1)
            }
            else
            {
                GetCountSouko(); // 作業7, 8 増便コース증편코스(zoubin_flg >= 2)
            }
            
            return view;
        }

        // TRG ボタン押した時
        public override void OnBarcodeDataReceived(BarcodeDataReceivedEvent_ dataReceivedEvent)
        {
            IList<BarcodeDataReceivedEvent_.BarcodeData_> listBarcodeData = dataReceivedEvent.BarcodeData;

            foreach (BarcodeDataReceivedEvent_.BarcodeData_ barcodeData in listBarcodeData)
            {
                this.Activity.RunOnUiThread(() =>
                {
                    if (carLabelInputMode == false) // 出荷ラベル
                    {
                        // 出荷ラベル確認処理
                        Thread.Sleep(1500);

                        Dictionary<string, string> param = GetProcParam(barcodeData.Data);
                        //MTumikomiProc result = WebService.CallTumiKomiProc(kansen_kbn == "0" ? "060" : "310", param); // IT HAS ERROR

                        MTumikomiProc result = new MTumikomiProc();
                        result.poMsg = "";
                        if (result.poMsg != "")
                        {
                            CommonUtils.AlertDialog(view, "エラー", result.poMsg, null);
                            return;
                        }

                        matehan = result.poMatehan;
                        CommonUtils.AlertDialog(view, "確認", "出荷ラベルが確認できました。", null);

                        // result.poMatehan

                        // sagyou8
                        /*
                        if (zoubin_flg != 1)
                        {
                            // 貨物Noスキャン時、各分類のカウントを取得
                            CountKamotsu(result.poMatehan);
                        }
                        */

                        carLabelInputMode = true;

                    }
                    else　// 車両ラベル
                    {
                        // 作業ステータス更新・積込処理
                        UpdateSagyoStatus(barcodeData.Data);
                    }
                });
            }
        }

        // 総個数取得 TUMIKOMI050
        private int GetCountSouko()
        {
            ((MainActivity)this.Activity).ShowProgress("マテハン情報取得しています。");
            int count = 0;

            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(() =>
                {
                    Thread.Sleep(1500);
                    count = WebService.RequestTumikomi050(souko_cd, kitaku_cd, syuka_date, tokuisaki_cd, todokesaki_cd, bin_no);
                 });
                Activity.RunOnUiThread(() => ((MainActivity)this.Activity).DismissDialog());
                }
            )).Start();

            return count;
        }

        // 作業ステータス更新・積込処理 TUMIKOMI080,TUMIKOMI311
        private void UpdateSagyoStatus(string saryouData)
        {
            var progress = ProgressDialog.Show(this.Activity, "Please wait...", "Contacting server. Please wait...", true);
            int resultCode = 1;

            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(() =>
                {
                    Thread.Sleep(1500);

                    Dictionary<string, string> param = GetProcParam(saryouData);
                    //MTumikomiProc result = WebService.CallTumiKomiProc( kansen_kbn != "0" ? "080": "311", param);

                    // ********* delete
                    MTumikomiProc result = new MTumikomiProc();
                    result.poMsg = "";
                    result.poRet = "0";
                    // ********* delete

                    // プロシージャ内エラーの場合
                    if (result.poMsg != "")
                    {
                        CommonUtils.AlertDialog(view, "エラー", result.poMsg, null);
                        return;
                    }

                    resultCode = int.Parse(result.poRet);
                   
                    if (resultCode == 0 || resultCode == 2)
                    {
                        if (kansen_kbn == "0")
                        {
                            if (resultCode == 2)
                            {
                                CommonUtils.AlertConfirm(view, "確認", "積込可能な商品があります。\n積込みを完了\nしますか？", (flag) =>
                                {
                                    if (!flag)
                                    {
                                        carLabelInputMode = false;
                                        return;
                                    }
                                    else
                                    {
                                        Log.Debug(TAG, "CreateTsumiFiles Start");
                                   
                                        CreateTsumiFiles();

                                        Log.Debug(TAG, "CreateTsumiFiles End");
                                    }
                                });
                            }
                            else
                            {
                                CreateTsumiFiles();
                            }
                        }

                        //配車テーブルの該当コースの各数量を実績数で更新する
                        var updateResult = WebService.CallTumiKomiProc(kansen_kbn == "0" ? "210" : "314", param);
                                
                        if (updateResult.poRet == "0" || updateResult.poRet == "99")
                        {
                            editor.PutBoolean("tenpo_zan_flg", updateResult.poRet == "99" ? true : false);
                            editor.Apply();
                            StartFragment(FragmentManager, typeof(TsumikomiCompleteFragment));
                        }
                        else
                        {
                            CommonUtils.AlertDialog(view, "エラー", "表示データがありません。", null);
                            return;
                        }

                    }
                    else if (resultCode == 1)
                    {
                        // scan_flg = true	//スキャン済みフラグ
                        // iniZero(4), Return("sagyou5")
                        carLabelInputMode = false;
                    }
                        
                }
                );
                Activity.RunOnUiThread(() => progress.Dismiss());
            }
            )).Start();
        }

        // マテハン情報取得 TUMIKOMI040,TUMIKOMI300
        private void GetTenpoMatehanInfo()
        {
            ((MainActivity)this.Activity).ShowProgress("マテハン情報取得しています。");

            new Thread(new ThreadStart(delegate {

                Activity.RunOnUiThread(() =>
                {
                    Thread.Sleep(1500);

                    List<TUMIKOMI040> resultList;
                    
                    if (kansen_kbn == "0")
                    {
                        // 該当店舗の各マテハン数を取得(定番コース)
                        resultList = WebService.RequestTumikomi040(souko_cd, kitaku_cd, syuka_date, tokuisaki_cd, todokesaki_cd, bin_no);
                    }
                    else
                    {
                        // 該当店舗の各マテハン数を取得(定番コース)
                        resultList = WebService.RequestTumikomi300(souko_cd, kitaku_cd, syuka_date, tokuisaki_cd, todokesaki_cd, bin_no);
                    }

                    foreach (TUMIKOMI040 result in resultList)
                    {
                        string btvCategory = result.name_cd;
                        string btvKosu = result.cnt;

                        if (btvCategory == "00")
                        {
                            etCarry.Text = btvKosu;
                        }
                        else if (btvCategory == "01") {
                            etKargo.Text = btvKosu;
                        }
                        else if (btvCategory == "02") {
                            etCard.Text = btvKosu;
                        }
                        else if (btvCategory == "03") {
                            etBara.Text = btvKosu;
                        }
                        else if (btvCategory == "04") {
                            etSonata.Text = btvKosu;
                        }
                    }
                }
                );
                Activity.RunOnUiThread(() => ((MainActivity)this.Activity).DismissDialog());
            }
        )).Start();

        }
        
        // 貨物Noスキャン時、各分類のカウントを取得 TUMIKOMI070 sagyou8 
        private int CountKamotsu(string matehan)
        {
            var progress = ProgressDialog.Show(this.Activity, "Please wait...", "Contacting server. Please wait...", true);
            int resultCode = 1;
            
            new Thread(new ThreadStart(delegate {

                Activity.RunOnUiThread(() =>
                {
                    Thread.Sleep(1500);

                    Dictionary<string, string> param = new Dictionary<string, string>
                    {
                        { "kenpin_souko",  souko_cd},
                        { "kitaku_cd", kitaku_cd},
                        { "syuka_date", syuka_date },
                        { "tokuisaki_cd", tokuisaki_cd },
                        { "todokesaki_cd", todokesaki_cd },
                        { "matehan", matehan },
                        { "bin_no", bin_no }
                    };

                    TUMIKOMI070 result = WebService.RequestTumikomi070(param);
                    
                    if (result == null)
                    {
                        CommonUtils.AlertDialog(view, "エラー", "表示データがありません。", null);
                        return;
                    }

                    string btvBunrui = result.bunrui;
                    string btvKosu = result.cnt;

                    // JOB:ko_su = 0
                    // JOB: b_ko_su = JOB:b_ko_su.Remove(" ")
                    // JOB: dai_su = JOB:dai_su + 1

                    switch (btvBunrui)
                    {
                        case "01" :
                            
                            // JOB: case_su = btvKosu
                            break;
                        case "02":
                            // JOB: oricon_su = btvKosu
                            break;
                        case "03":
                            // JOB: sonota_su = JOB:sonota_su + btvKosu
                            break;
                        case "04":
                            // JOB: ido_su = btvKosu
                            break;
                        case "05":
                            // JOB: mail_su = btvKosu
                            break;
                        case "06":
                            // JOB: sonota_su = JOB:sonota_su + btvKosu
                            break;
                        case "07":
                            // JOB: futeikei_su = btvKosu
                            break;
                        case "08":
                            // JOB: sonota_su = JOB:sonota_su + btvKosu
                            break;
                        case "09":
                            // JOB: hansoku_su = btvKosu
                            break;
                        default:
                            // JOB: sonota_su = JOB:sonota_su + btvKosu
                            break;
                    }

                    // JOB: b_ko_su = JOB:b_ko_su + btvKosu
                    // JOB: ko_su = JOB:ko_su + btvKosu

                }
                );
                Activity.RunOnUiThread(() => progress.Dismiss());

            }
            )).Start();

            return resultCode;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Back)
            {
                if (carLabelInputMode == false) {
                    FragmentManager.PopBackStack();
                }
                else
                {
                     CancelTsumiKomi();
                }
            }
            else if (keycode == Keycode.F1)
            {
                if(kansen_kbn == "0" && prefs.GetBoolean("scan_flg", false)) StartFragment(FragmentManager, typeof(TsumikomiIdouMenuFragment));
            }
            else if (keycode == Keycode.F3)
            {
                // 移動メッセージ画面
                if (!carLabelInputMode)
                {
                    // JOB: menu_flg = 0 Return("sagyou11")
                    // StartFragment(FragmentManager, typeof()); 
                }
            }

            return true;
        }

        public override bool OnBackPressed()
        {
            return false;
        }

        private void CancelTsumiKomi()
        {
            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(() =>
                {
                    Thread.Sleep(1500);
                    
                    Dictionary<string, string> param = GetProcParam("");
                    MTumikomiProc result = WebService.CallTumiKomiProc(kansen_kbn == "0" ? "090" : "312", param);

                    if(result.poMsg != "")
                    {
                        CommonUtils.AlertDialog(view, "エラー", result.poMsg, null);
                        return;
                    }
                  
                    if (result.poRet == "0")
                    {
                        carLabelInputMode = false;
                    }
                    else if (result.poRet == "8")
                    {
                        CommonUtils.AlertDialog(view, "エラー", ERR_UPDATE_001, null);
                        return;
                    }
                }
                );
            }
            )).Start();
        }

        // 積込完了時に生成されるファイル（納品で使います。）
        private void CreateTsumiFiles()
        {
            // CRATE TUMIKOMI FILE
            // MAIN FILE
            List<MFile> mFiles = WebService.RequestTumikomi100(souko_cd, kitaku_cd, syuka_date, bin_no, course, tokuisaki_cd, todokesaki_cd);
            new MFileHelper().InsertALL(mFiles);

            // It would be useless..
            //PsFile psFile = WebService.RequestTumikomi180();
            PsFile psFile = new PsFile { pass = "" };
            new PsFileHelper().Insert(psFile);

            // MAILBACK FILE 
            List<MbFile> mbFiles = WebService.RequestTumikomi140(souko_cd, kitaku_cd, syuka_date, bin_no, course);
            new MbFileHelper().InsertAll(mbFiles);

            // SOUKO FILE
            SoFile soFile = WebService.RequestTumikomi160(souko_cd);
            new SoFileHelper().Insert(soFile);

            // // It would be useless..
            // TUMIKOMI190 -> ftp file ? 
            //FtpFile ftpFile = WebService.RequestTumikomi190();
            //FtpFile ftpFile = new FtpFile {  };
            //new FtpFileHelper().Insert(ftpFile);

            // VENDOR FILE
            string nohin_date = DateTime.Now.ToString("yyyyMMdd");
            List<MateFile> mateFile = WebService.RequestTumikomi260(souko_cd, kitaku_cd, syuka_date, nohin_date, bin_no, course);
            new MateFileHelper().InsertAll(mateFile);

            // TOKUISAKI FILE
            List<TokuiFile> tokuiFile = WebService.RequestTumikomi270();
            new TokuiFileHelper().InsertAll(tokuiFile);

            Log.Debug(TAG, "CreateTsumiFiles end");
            
        }

        // PROC専用のパラメータ設定
        private Dictionary<string, string> GetProcParam(string barcodeData)
        {
            return new Dictionary<string, string>
                        {
                            { "pTerminalID",  "432660068"},
                            { "pProgramID", "TUM" },
                            { "pSagyosyaCD", "99999" },
                            { "pSoukoCD",  souko_cd},
                            { "pSyukaDate", syuka_date},
                            { "pBinNo", bin_no},
                            { "pCourse", course },
                            { "matehan", "" },
                            { "pTokuisakiCD", tokuisaki_cd },
                            { carLabelInputMode == false ? "pKamotsuNo" : "pSyaryoNo", barcodeData },
                            { "pHHT_No", "11101" }
                        };
        }

    }
}
 