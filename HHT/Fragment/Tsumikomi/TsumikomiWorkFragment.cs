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
using Com.Beardedhen.Androidbootstrap;
using Com.Densowave.Bhtsdk.Barcode;
using HHT.Common;
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
        private BootstrapButton _CompleteButton,  _IdouButton;
        private string kansen_kbn;

        private string souko_cd, kitaku_cd, syuka_date, tokuisaki_cd, todokesaki_cd, bin_no, course;
        private int zoubin_flg;
        private string matehan;
        private bool carLabelInputMode = false;
        private static readonly string ERR_UPDATE_001 = "更新出来ませんでした。\n再度商品をスキャンして下さい。";
        
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
            
            view.FindViewById<TextView>(Resource.Id.txt_tsumikomiWork_tokuisakiNm).Text = prefs.GetString("tokuisaki_nm", "");
            etKosu = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_kosu);
            etCarLabel = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_carLabel);
            etCarry = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_carry);
            etKargo = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_kargoCar);
            etCard = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_card);
            etBara = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_bara);
            etSonata = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_sonota);

            _CompleteButton = view.FindViewById<BootstrapButton>(Resource.Id.completeButton);
            _CompleteButton.Click += delegate { StartFragment(FragmentManager, typeof(TsumikomiPassFragment)); };
            _CompleteButton.Enabled = false;

            _IdouButton = view.FindViewById<BootstrapButton>(Resource.Id.idouButton);
            _IdouButton.Click += delegate { StartFragment(FragmentManager, typeof(TsumikomiIdouMenuFragment)); };
            
            souko_cd = prefs.GetString("souko_cd", "");
            kitaku_cd = prefs.GetString("kitaku_cd", "");
            syuka_date = prefs.GetString("syuka_date", "");
            tokuisaki_cd = prefs.GetString("tokuisaki_cd", "");
            todokesaki_cd = prefs.GetString("todokesaki_cd", "");
            bin_no = prefs.GetString("bin_no", "");
            course = prefs.GetString("course", "");

            zoubin_flg = prefs.GetInt("zoubin_flg", 1);
            kansen_kbn = prefs.GetString("kansen_kbn", "1");

            etKosu.SetBackgroundColor(Android.Graphics.Color.Yellow);

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
                        try
                        {
                            MTumikomiProc result = WebService.CallTumiKomiProc(kansen_kbn == "0" ? "060" : "310", param);

                            if (result.poMsg != "")
                            {
                                ShowDialog("エラー", result.poMsg, () => { });
                                return;
                            }

                            matehan = result.poMatehan;
                            etKosu.Text = result.poKosuCnt;

                            //	正常登録
                            carLabelInputMode = true;
                            _IdouButton.Enabled = false;
                            etCarLabel.SetBackgroundColor(Android.Graphics.Color.Yellow);
                            etKosu.SetBackgroundColor(Android.Graphics.Color.White);
                        }
                        catch
                        {
                            ShowDialog("エラー", "更新出来ませんでした。\n再度商品をスキャンして下さい。", () => { });
                            return;
                        }
                    }
                    else　// 車両ラベル
                    {
                        // 作業ステータス更新・積込処理
                        etCarLabel.Text = barcodeData.Data;
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
            ((MainActivity)this.Activity).ShowProgress("作業ステータス更新中...");

            int resultCode = 1;

            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(async () =>
                {
                    Thread.Sleep(1500);
                    try
                    {
                        Dictionary<string, string> param = GetProcParam(saryouData);
                        MTumikomiProc result = WebService.CallTumiKomiProc(kansen_kbn == "0" ? "080" : "311", param);
                        resultCode = int.Parse(result.poMsg);

                        if (resultCode == 0 || resultCode == 2)
                        {
                            if (kansen_kbn == "0")
                            {
                                if (resultCode == 2)
                                {
                                    carLabelInputMode = false;

                                    ShowDialog("確認", "積込可能な商品があります。\n積込みを完了\nしますか？", (flag) => {
                                        if (flag)
                                        {
                                            carLabelInputMode = true;

                                            Log.Debug(TAG, "CreateTsumiFiles Start");

                                            CreateTsumiFiles();

                                            Log.Debug(TAG, "CreateTsumiFiles End");

                                            //配車テーブルの該当コースの各数量を実績数で更新する
                                            var updateResult = WebService.CallTumiKomiProc(kansen_kbn == "0" ? "210" : "314", param);

                                            if (updateResult.poRet == "0" || updateResult.poRet == "99")
                                            {
                                                //editor.PutBoolean("tenpo_zan_flg", updateResult.poRet == "99" ? true : false);
                                                //editor.Apply();
                                                //StartFragment(FragmentManager, typeof(TsumikomiCompleteFragment));
                                                Activity.RunOnUiThread(() =>
                                                {
                                                    //	正常登録
                                                    ShowDialog("報告", "積込検品が\n完了しました。", () => { FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(0).Id, 0); });
                                                });

                                            }
                                            else
                                            {
                                                ShowDialog("エラー", "表示データがありません", () => { FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(0).Id, 0); });
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            GetTenpoMatehanInfo();

                                            etKosu.SetBackgroundColor(Android.Graphics.Color.Yellow);
                                            etCarLabel.SetBackgroundColor(Android.Graphics.Color.White);

                                            etKosu.Text = "";
                                            etCarLabel.Text = "";

                                            carLabelInputMode = false;
                                        }
                                        
                                    });
                                }
                                else
                                {
                                    Log.Debug(TAG, "CreateTsumiFiles Start");

                                    CreateTsumiFiles();

                                    Log.Debug(TAG, "CreateTsumiFiles End");

                                    //配車テーブルの該当コースの各数量を実績数で更新する
                                    var updateResult = WebService.CallTumiKomiProc(kansen_kbn == "0" ? "210" : "314", param);

                                    if (updateResult.poRet == "0" || updateResult.poRet == "99")
                                    {
                                        //editor.PutBoolean("tenpo_zan_flg", updateResult.poRet == "99" ? true : false);
                                        //editor.Apply();
                                        //StartFragment(FragmentManager, typeof(TsumikomiCompleteFragment));
                                        Activity.RunOnUiThread(() =>
                                        {
                                            //	正常登録22
                                            ShowDialog("報告", "積込検品が\n完了しました。", () => { FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(0).Id, 0); });
                                        });
                                    }
                                    else
                                    {
                                        ShowDialog("エラー", "表示データがありません", () => { FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(0).Id, 0); });
                                        return;
                                    }
                                }
                            }
                        }
                        else if (resultCode == 1)
                        {
                            // scan_flg = true	//スキャン済みフラグ
                            // iniZero(4), Return("sagyou5")
                            
                            GetTenpoMatehanInfo();

                            Activity.RunOnUiThread(() =>
                            {
                                etKosu.SetBackgroundColor(Android.Graphics.Color.Yellow);
                                etCarLabel.SetBackgroundColor(Android.Graphics.Color.White);

                                etKosu.Text = "";
                                etCarLabel.Text = "";

                                carLabelInputMode = false;

                                _CompleteButton.Enabled = true;

                            });
                        }
                    }
                    catch
                    {
                        ShowDialog("エラー", "例外エラーが発生しました。", () => { });
                        return;
                    }
                }
                );
                Activity.RunOnUiThread(() => ((MainActivity)this.Activity).DismissDialog());
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
        
        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F1)
            {
                if(kansen_kbn == "0" && prefs.GetBoolean("scan_flg", false)) StartFragment(FragmentManager, typeof(TsumikomiIdouMenuFragment));
            }
            else if (keycode == Keycode.F3)
            {
                // 移動メッセージ画面
                if (!carLabelInputMode) StartFragment(FragmentManager, typeof(TsumikomiIdouMenuFragment));
            }

            return true;
        }

        public override bool OnBackPressed()
        {
            if (carLabelInputMode)
            {
                CancelTsumiKomi();
                _IdouButton.Enabled = true;
                etKosu.Text = "0";
                etKosu.SetBackgroundColor(Android.Graphics.Color.Yellow);
                etCarLabel.SetBackgroundColor(Android.Graphics.Color.White);
            }
            else
            {
                return true;
            }

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
                        ShowDialog("エラー", result.poMsg, () => { });
                        return;
                    }
                  
                    if (result.poRet == "0")
                    {
                        carLabelInputMode = false;
                    }
                    else if (result.poRet == "8")
                    {
                        ShowDialog("エラー", ERR_UPDATE_001, () => { });
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
            MFileHelper mFileHelper = new MFileHelper();
            mFileHelper.DeleteAll();
            mFileHelper.InsertALL(mFiles);

            //PsFile psFile = WebService.RequestTumikomi180(souko_cd, syuka_date);
            /*
            PsFileHelper psFileHelper = new PsFileHelper();
            PsFile psFile = new PsFile();
            psFileHelper.DeleteAll();
            psFileHelper.Insert(psFile);
            */

            // MAILBACK FILE 
            List<MbFile> mbFiles = WebService.RequestTumikomi140(souko_cd, kitaku_cd, syuka_date, bin_no, course);
            MbFileHelper mbFileHelper = new MbFileHelper();
            mbFileHelper.DeleteAll();
            mbFileHelper.InsertAll(mbFiles);

            // SOUKO FILE
            SoFile soFile = WebService.RequestTumikomi160(souko_cd);
            SoFileHelper soFileHelper = new SoFileHelper();
            soFileHelper.DeleteAll();
            soFileHelper.Insert(soFile);
            
            // VENDOR FILE
            string nohin_date = DateTime.Now.ToString("yyyyMMdd");
            List<MateFile> mateFile = WebService.RequestTumikomi260();
            MateFileHelper mateFileHelper = new MateFileHelper();
            mateFileHelper.DeleteAll();
            mateFileHelper.InsertAll(mateFile);

            // TOKUISAKI FILE
            List<TokuiFile> tokuiFile = WebService.RequestTumikomi270();
            TokuiFileHelper tokuiFileHelper = new TokuiFileHelper();
            tokuiFileHelper.DeleteAll();
            tokuiFileHelper.InsertAll(tokuiFile);

            Log.Debug(TAG, "CreateTsumiFiles end");
            
        }

        // PROC専用のパラメータ設定
        private Dictionary<string, string> GetProcParam(string barcodeData)
        {
            return new Dictionary<string, string>
                        {
                            { "pTerminalID",  prefs.GetString("terminal_id","")},
                            { "pProgramID", "TUM" },
                            { "pSagyosyaCD", prefs.GetString("sagyousya_cd","") },
                            { "pSoukoCD",  souko_cd},
                            { "pSyukaDate", syuka_date},
                            { "pBinNo", bin_no},
                            { "pCourse", course },
                            { "pMatehan", matehan },
                            { "pTokuisakiCD", tokuisaki_cd },
                            { "pTodokesakiCD", todokesaki_cd },
                            { carLabelInputMode == false ? "pKamotsuNo" : "pSyaryoNo", barcodeData },
                            { "pHHT_No", prefs.GetString("hht_no","") }
                        };
        }

    }
}
 