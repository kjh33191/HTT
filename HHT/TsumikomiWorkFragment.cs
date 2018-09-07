using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;
using HHT.Resources.DataHelper;
using HHT.Resources.Model;

namespace HHT
{
    public class TsumikomiWorkFragment : BaseFragment
    {
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        private View view;
        private EditText etKosu, etCarLabel, etCarry, etKargo, etCard, etBara, etSonata;
        private Button btnIdou;
        private int kansen_kbn;
        
        private bool carLabelInputMode;
        private string zoubin_flg;

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
            btnIdou.Click += delegate { GoToIdouMenu(); };
            
            kansen_kbn = 0;
            
            carLabelInputMode = false;
            zoubin_flg = prefs.GetString("zoubin_flg", "1");

            if (zoubin_flg == "1")
            {
                GetTenpoMatehanInfo();  // 作業5, 6 定番コース단골코스(zoubin_flg = 1)
            }
            else
            {
                GetCountSouko(); // 作業7, 8 増便コース증편코스(zoubin_flg >= 2)
            }
            
            return view;
        }
        private void GoToIdouMenu()
        {
            StartFragment(FragmentManager, typeof(TsumikomiIdouMenuFragment));
        }

        // 総個数取得 TUMIKOMI050
        private int GetCountSouko()
        {
            ((MainActivity)this.Activity).ShowProgress("");
            int count = 0;

            new Thread(new ThreadStart(delegate {

                Activity.RunOnUiThread(() =>
                {
                    Thread.Sleep(1500);

                    Dictionary<string, string> param = new Dictionary<string, string>
                    {
                        { "kenpin_souko",  prefs.GetString("souko_cd", "103")},
                        { "kitaku_cd", prefs.GetString("kitaku_cd", "2") },
                        { "syuka_date", prefs.GetString("shuka_date", "180310") },
                        { "nohin_date", prefs.GetString("nohin_date", "1") },
                        { "tokuisaki_cd", prefs.GetString("tokuisaki_cd", "1") },
                        { "todokesaki_cd", prefs.GetString("todokesaki_cd", "1") },
                        { "bin_no", prefs.GetString("bin_no", "310") },
                    };
                    
                    //string resultJson = CommonUtils.Post(WebService.TUMIKOMI.TUMIKOMI050, param);
                    //TUMIKOMI050 result = JsonConvert.DeserializeObject<TUMIKOMI050>(resultJson);
                    
                    Dictionary<string, string> result = new Dictionary<string, string>();
                    int.TryParse(result["kosu_kei"], out count);
                    
                 }
                    );
                    Activity.RunOnUiThread(() => ((MainActivity)this.Activity).DismissDialog());

                }
            )).Start();

            return count;
        }

        // 作業ステータス更新・積込処理 TUMIKOMI080,TUMIKOMI311
        private void UpdateSagyoStatus()
        {
            var progress = ProgressDialog.Show(this.Activity, "Please wait...", "Contacting server. Please wait...", true);
            int resultCode = 1;

            new Thread(new ThreadStart(delegate {

                Activity.RunOnUiThread(() =>
                {
                    Thread.Sleep(1500);

                    Dictionary<string, string> param = new Dictionary<string, string>
                    {
                        { "pTerminalID",  prefs.GetString("souko_cd", "103")},
                        { "pProgramID", prefs.GetString("kitaku_cd", "2") },
                        { "pSagyosyaCD", prefs.GetString("shuka_date", "180310") },
                        { "pSoukoCD",  prefs.GetString("souko_cd", "103")},
                        { "pSyukaDate", prefs.GetString("kitaku_cd", "2") },
                        { "pBinNo", prefs.GetString("shuka_date", "180310") },
                        { "pCourse", prefs.GetString("nohin_date", "1") },
                        { "pTokuisakiCD", prefs.GetString("tokuisaki_cd", "1") },
                        { "pTodokesakiCD", prefs.GetString("todokesaki_cd", "1") },
                        { "pHHT_No", prefs.GetString("todokesaki_cd", "1") },
                        { "pMatehan", prefs.GetString("todokesaki_cd", "1") },
                        { "pSyaryoNo", prefs.GetString("bin_no", "310") }
                    };

                    if (kansen_kbn != 0)
                    {
                        //string resultJson = CommonUtils.Post(WebService.TUMIKOMI.TUMIKOMI080, param);
                        //TUMIKOMI080 result = JsonConvert.DeserializeObject<TUMIKOMI080>(resultJson);

                        TUMIKOMI310 result = new TUMIKOMI310
                        {
                            poRet = "0" // todo
                        };
                        switch (result.poRet)
                        {
                            case "1": resultCode = 1; break;
                            case "2": resultCode = 2; break;
                            case "8": resultCode = 8; break;
                            case "0": resultCode = 0; break;
                            default: resultCode = 1; break;
                        }
                    }
                    else
                    {
                        //string resultJson = CommonUtils.Post(WebService.TUMIKOMI.TUMIKOMI311, param);
                        //TUMIKOMI311 result = JsonConvert.DeserializeObject<TUMIKOMI311>(resultJson);

                        TUMIKOMI310 result = new TUMIKOMI310();
                        result.poRet = "0"; // todo
                        switch (result.poRet)
                        {
                            case "1": resultCode = 1; break;
                            case "2": resultCode = 2; break;
                            case "8": resultCode = 8; break;
                            case "0": resultCode = 0; break;
                            default: resultCode = 1; break;
                        }
                    }

                    if (zoubin_flg == "1")
                    {

                        if (resultCode == 0 || resultCode == 2)
                        {
                            if (kansen_kbn == 0)
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
                                            // 積込みを完了
                                            CompleteTsumiKomi();
                                        }
                                    });
                                }
                                else
                                {
                                    // 積込みを完了
                                    CompleteTsumiKomi();
                                }
                            }

                            if (kansen_kbn == 0)
                            {
                                /*
                                Dictionary<string, string> param = new Dictionary<string, string>
                                    {
                                        { "pTerminalID",  prefs.GetString("souko_cd", "103")},
                                        { "pProgramID", prefs.GetString("kitaku_cd", "2") },
                                        { "pSagyosyaCD", prefs.GetString("shuka_date", "180310") },
                                        { "pSoukoCD",  prefs.GetString("souko_cd", "103")},
                                        { "pSyukaDate", prefs.GetString("kitaku_cd", "2") },
                                        { "pBinNo", prefs.GetString("shuka_date", "180310") },
                                        { "pCourse", prefs.GetString("nohin_date", "1") },
                                        { "pTokuisakiCD", prefs.GetString("tokuisaki_cd", "1") },
                                        { "pTodokesakiCD", prefs.GetString("todokesaki_cd", "1") },
                                        { "pHHT_No", prefs.GetString("todokesaki_cd", "1") },
                                        { "pMatehan", prefs.GetString("todokesaki_cd", "1") },
                                        { "pSyaryoNo", prefs.GetString("bin_no", "310") }
                                    };
                                    */
                                //string resultJson = CommonUtils.Post(WebService.TUMIKOMI.TUMIKOMI210, param);
                                //TUMIKOMI210 result = JsonConvert.DeserializeObject<TUMIKOMI210>(resultJson);

                                TUMIKOMI310 result = new TUMIKOMI310();
                                result.poRet = "0";
                                if (result.poRet == "99")
                                {
                                    // 残作業あり
                                    // tenpo_zan_flg = true
                                    StartFragment(FragmentManager, typeof(TsumikomiCompleteFragment));
                                }
                                else if (result.poRet == "0")
                                {
                                    // 正常終了
                                    // tenpo_zan_flg = false
                                    StartFragment(FragmentManager, typeof(TsumikomiCompleteFragment));
                                }
                                else
                                {
                                    // Error
                                    CommonUtils.AlertDialog(view, "エラー", "表示データがありません。", null);
                                    return;
                                }
                            }
                            else
                            {
                                /*
                                Dictionary<string, string> param = new Dictionary<string, string>
                                    {
                                        { "pTerminalID",  prefs.GetString("souko_cd", "103")},
                                        { "pProgramID", prefs.GetString("kitaku_cd", "2") },
                                        { "pSagyosyaCD", prefs.GetString("shuka_date", "180310") },
                                        { "pSoukoCD",  prefs.GetString("souko_cd", "103")},
                                        { "pSyukaDate", prefs.GetString("kitaku_cd", "2") },
                                        { "pBinNo", prefs.GetString("shuka_date", "180310") },
                                        { "pCourse", prefs.GetString("nohin_date", "1") },
                                        { "pTokuisakiCD", prefs.GetString("tokuisaki_cd", "1") },
                                        { "pTodokesakiCD", prefs.GetString("todokesaki_cd", "1") },
                                        { "pHHT_No", prefs.GetString("todokesaki_cd", "1") },
                                        { "pMatehan", prefs.GetString("todokesaki_cd", "1") },
                                        { "pSyaryoNo", prefs.GetString("bin_no", "310") }
                                    };
                                    */
                                //string resultJson = CommonUtils.Post(WebService.TUMIKOMI.TUMIKOMI314, param);
                                //TUMIKOMI314 result = JsonConvert.DeserializeObject<TUMIKOMI314>(resultJson);

                                TUMIKOMI310 result = new TUMIKOMI310();
                                if (result.poRet == "99")
                                {
                                    // 残作業あり
                                    // tenpo_zan_flg = true
                                    StartFragment(FragmentManager, typeof(TsumikomiCompleteFragment));
                                }
                                else if (result.poRet == "0")
                                {
                                    // 正常終了
                                    // tenpo_zan_flg = false
                                    StartFragment(FragmentManager, typeof(TsumikomiCompleteFragment));
                                }
                                else
                                {
                                    // Error
                                    CommonUtils.AlertDialog(view, "エラー", "表示データがありません。", null);
                                    return;
                                }
                            }
                        }
                        else if (resultCode == 1)
                        {
                            // scan_flg = true	//スキャン済みフラグ
                            // iniZero(4), Return("sagyou5")
                            carLabelInputMode = false;
                        }

                    }
                    else
                    {
                        if (resultCode == 0)
                        {
                            // ok
                            // JOB: kanryo_flg = true	//完了フラグを立てる
                            // JOB: scan_flg = true
                            // Return("sagyou9")

                        }
                        else if (resultCode == 8)
                        {
                            CommonUtils.AlertDialog(view, "エラー", "更新出来ませんでした。\n再度商品をスキャンして下さい。", null);
                        }
                        else
                        {
                            // sagyou7
                            carLabelInputMode = false;
                        }
                    }

                }
                );
                Activity.RunOnUiThread(() => progress.Dismiss());

            }
            )).Start();
        }

        // 出荷ラベル確認処理
        private void CheckSyukaLabel()
        {
            var progress = ProgressDialog.Show(this.Activity, null, "出荷ラベルを確認しています。", true);
            int resultCode = 1;

            new Thread(new ThreadStart(delegate {

                Activity.RunOnUiThread(() =>
                {
                    Thread.Sleep(1500);

                    Dictionary<string, string> param = new Dictionary<string, string>
                    {
                        { "pTerminalID",  prefs.GetString("souko_cd", "103")},
                        { "pProgramID", prefs.GetString("kitaku_cd", "2") },
                        { "pSagyosyaCD", prefs.GetString("shuka_date", "180310") },
                        { "pSoukoCD",  prefs.GetString("souko_cd", "103")},
                        { "pSyukaDate", prefs.GetString("kitaku_cd", "2") },
                        { "pBinNo", prefs.GetString("shuka_date", "180310") },
                        { "pCourse", prefs.GetString("nohin_date", "1") },
                        { "pTokuisakiCD", prefs.GetString("tokuisaki_cd", "1") },
                        { "pKamotsuNo", prefs.GetString("todokesaki_cd", "1") },
                        { "pHHT_No", prefs.GetString("bin_no", "310") }
                    };

                    string errorCode = "";

                    if (zoubin_flg == "1" && kansen_kbn != 0)
                    {
                        TUMIKOMI310 result = WebService.RequestTumikomi310(param);
                        errorCode = result.poRet;
                    }
                    else
                    {
                        TUMIKOMI060 result = WebService.RequestTumikomi060(param); //result.poMatehan
                         errorCode = result.poRet;
                    }

                    switch (errorCode)
                    {
                        case "1": CommonUtils.AlertDialog(view, "エラー", "貨物Noが見つかりません。", null); break;
                        case "2": CommonUtils.AlertDialog(view, "エラー", "個数検品が完了していません。", null); break;
                        case "3": CommonUtils.AlertDialog(view, "エラー", "他の作業者が作業中です。", null); break;
                        case "4": CommonUtils.AlertDialog(view, "エラー", "既に積込済です。", null); break;
                        case "5": CommonUtils.AlertDialog(view, "エラー", "他の便の貨物Noです。", null); break;
                        case "8": resultCode = 8; break;
                        case "0":
                            resultCode = 0; //matehan = arrData[3] 
                            break;
                    }

                    resultCode = 0;

                    if (resultCode == 0)
                    {
                        carLabelInputMode = true;
                        CommonUtils.AlertDialog(view, "確認", "出荷ラベルが確認できました。", null);
                    }
                    else if (resultCode == 8)
                    {
                        CommonUtils.AlertDialog(view, "エラー", "更新出来ませんでした。\n再度商品をスキャンして下さい。", null);
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

                    Dictionary<string, string> param = new Dictionary<string, string>
                    {
                        { "kenpin_souko",  prefs.GetString("souko_cd", "108")},
                        { "kitaku_cd", prefs.GetString("kitaku_cd", "2") },
                        { "syuka_date", prefs.GetString("shuka_date", "20180320") },
                        { "nohin_date", prefs.GetString("nohin_date", "20180321") },
                        { "tokuisaki_cd", prefs.GetString("tokuisaki_cd", "0000") },
                        { "todokesaki_cd", prefs.GetString("todokesaki_cd", "0194") },
                        { "bin_no", prefs.GetString("bin_no", "1") },
                    };

                    List<TUMIKOMI040> resultList;
                    
                    if (kansen_kbn == 0)
                    {
                        // 該当店舗の各マテハン数を取得(定番コース)
                        resultList = WebService.RequestTumikomi040(param);
                    }
                    else
                    {
                        // 該当店舗の各マテハン数を取得(定番コース)
                        resultList = WebService.RequestTumikomi300(param);
                    }

                    foreach (TUMIKOMI040 result in resultList)
                    {
                        string btvCategory = result.name_cd;
                        string btvCategoryNm = result.category_nm;
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

        public override void OnBarcodeDataReceived(BarcodeDataReceivedEvent_ dataReceivedEvent)
        {
            IList<BarcodeDataReceivedEvent_.BarcodeData_> listBarcodeData = dataReceivedEvent.BarcodeData;

            foreach (BarcodeDataReceivedEvent_.BarcodeData_ barcodeData in listBarcodeData)
            {
                string densoSymbology = barcodeData.SymbologyDenso;
                string data = barcodeData.Data;
                int barcodeDataLength = data.Length;
                
                this.Activity.RunOnUiThread(() =>
                {
                    if (carLabelInputMode == false) // 出荷ラベル
                    {
                        // 出荷ラベル確認処理
                        CheckSyukaLabel();

                        if (zoubin_flg == "1")
                        {
                            // 貨物Noスキャン時、各分類のカウントを取得
                            CountKamotsu();
                        }

                    }
                    else　// 車両ラベル
                    {
                        // 作業ステータス更新・積込処理
                        UpdateSagyoStatus();
                    }
                });
            }
        }

        // 貨物Noスキャン時、各分類のカウントを取得 TUMIKOMI070
        private int CountKamotsu()
        {
            var progress = ProgressDialog.Show(this.Activity, "Please wait...", "Contacting server. Please wait...", true);
            int resultCode = 1;
            
            new Thread(new ThreadStart(delegate {

                Activity.RunOnUiThread(() =>
                {
                    Thread.Sleep(1500);

                    Dictionary<string, string> param = new Dictionary<string, string>
                    {
                        { "kenpin_souko",  prefs.GetString("souko_cd", "103")},
                        { "kitaku_cd", prefs.GetString("kitaku_cd", "2") },
                        { "syuka_date", prefs.GetString("shuka_date", "180310") },
                        { "nohin_date",  prefs.GetString("souko_cd", "103")},
                        { "tokuisaki_cd", prefs.GetString("kitaku_cd", "2") },
                        { "todokesaki_cd", prefs.GetString("shuka_date", "180310") },
                        { "matehan", prefs.GetString("nohin_date", "1") },
                        { "bin_no", prefs.GetString("tokuisaki_cd", "1") }
                    };

                    //string resultJson = CommonUtils.Post(WebService.TUMIKOMI.TUMIKOMI070, param);
                    //TUMIKOMI070 result = JsonConvert.DeserializeObject<TUMIKOMI070>(resultJson);

                    Dictionary<string, string> result = new Dictionary<string, string>();
                    if(result == null)
                    {
                        CommonUtils.AlertDialog(view, "エラー", "表示データがありません。", null);
                        return;
                    }

                    //string btvBunrui = result["bunrui"];
                    //string btvKosu = result["cnt"];
                    string btvBunrui = "01";
                    string btvKosu = "1";

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
                if (carLabelInputMode)
                {
                    CancelTsumiKomi();
                    return false;
                }
            }
            else if (keycode == Keycode.F1)
            {
                GoToIdouMenu();
            }
            else if (keycode == Keycode.F3)
            {
                

                if (!carLabelInputMode)
                {
                    if (zoubin_flg == "1")
                    {
                        // 移動メッセージ画面
                        //StartFragment(FragmentManager, typeof());
                    }
                    else
                    {
                        // zeroQty()
                        //JOB: menu_flg = 0

                        // Return("sagyou11")
                    }
                }
            }

            return true;
        }

        private void CancelTsumiKomi()
        {
            
            var progress = ProgressDialog.Show(this.Activity, null, "積込作業をキャンセルしています。", true);
            int resultCode = 1;

            new Thread(new ThreadStart(delegate {

                Activity.RunOnUiThread(() =>
                {
                    Thread.Sleep(1500);

                    Dictionary<string, string> param = new Dictionary<string, string>
                    {
                        { "pTerminalID",  prefs.GetString("souko_cd", "103")},
                        { "pProgramID", prefs.GetString("kitaku_cd", "2") },
                        { "pSagyosyaCD", prefs.GetString("shuka_date", "180310") },
                        { "pSoukoCD",  prefs.GetString("souko_cd", "103")},
                        { "pSyukaDate", prefs.GetString("kitaku_cd", "2") },
                        { "pBinNo", prefs.GetString("shuka_date", "180310") },
                        { "pCourse", prefs.GetString("nohin_date", "1") },
                        { "pTokuisakiCD", prefs.GetString("tokuisaki_cd", "1") },
                        { "pKamotsuNo", prefs.GetString("todokesaki_cd", "1") },
                        { "pHHT_No", prefs.GetString("bin_no", "310") }
                    };

                    if (zoubin_flg == "1" && kansen_kbn != 0)
                    {
                        //string resultJson = CommonUtils.Post(WebService.TUMIKOMI.TUMIKOMI090, param);
                        //TUMIKOMI090 result = JsonConvert.DeserializeObject<TUMIKOMI090>(resultJson);

                        TUMIKOMI310 result = new TUMIKOMI310();
                        switch (result.poRet)
                        {
                            case "1": CommonUtils.AlertDialog(view, "エラー", "貨物Noが見つかりません。", null); break;
                            case "2": CommonUtils.AlertDialog(view, "エラー", "個数検品が完了していません。", null); break;
                            case "3": CommonUtils.AlertDialog(view, "エラー", "他の作業者が作業中です。", null); break;
                            case "4": CommonUtils.AlertDialog(view, "エラー", "既に積込済です。", null); break;
                            case "5": CommonUtils.AlertDialog(view, "エラー", "他の便の貨物Noです。", null); break;
                            case "8": resultCode = 8; break;
                            case "0":
                                resultCode = 0; //matehan = arrData[3] 
                                break;
                        }
                    }
                    else
                    {
                        //string resultJson = CommonUtils.Post(WebService.TUMIKOMI.TUMIKOMI312, param);
                        //TUMIKOMI312 result = JsonConvert.DeserializeObject<TUMIKOMI312>(resultJson);

                        TUMIKOMI060 result = new TUMIKOMI060();
                        switch (result.poRet)
                        {
                            case "1": CommonUtils.AlertDialog(view, "エラー", "貨物Noが見つかりません。", null); break;
                            case "2": CommonUtils.AlertDialog(view, "エラー", "個数検品が完了していません。", null); break;
                            case "3": CommonUtils.AlertDialog(view, "エラー", "他の作業者が作業中です。", null); break;
                            case "4": CommonUtils.AlertDialog(view, "エラー", "既に積込済です。", null); break;
                            case "5": CommonUtils.AlertDialog(view, "エラー", "他の便の貨物Noです。", null); break;
                            case "8": resultCode = 8; break;
                            case "0":
                                resultCode = 0; //matehan = arrData[3] 
                                break;
                        }
                    }

                    resultCode = 0;

                    if (resultCode == 0)
                    {
                        carLabelInputMode = false;
                    }
                    else if (resultCode == 8)
                    {
                        CommonUtils.AlertDialog(view, "エラー", ERR_UPDATE_001, null);
                    }
                }
                );
                Activity.RunOnUiThread(() => progress.Dismiss());
            }
            )).Start();
        }

        private void CompleteTsumiKomi()
        {

            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(() =>
                {
                    // TUMIKOMI100 -> main file
                    //MFile mFile = WebService.RequestTumikomi100();
                    MFile mFile = new MFile
                    {
                        kenpin_souko = "108",
                        kitaku_cd = "2",
                        syuka_date = "20180320",
                        bin_no = "1",
                        course = "101",
                        driver_cd = "832",
                        butsuryu_no = "1",
                        nohin_yti_time = "1605",
                        tokuisaki_cd = "0000",
                        todokesaki_cd = "0374",
                        tokuisaki_rk = "新白岡店",
                        vendor_cd = "10041",
                        vendor_nm = "【ＤＣ】㈱ＰＡＬＴＡＣ",
                        default_vendor = "999999",
                        default_vendor_nm = "※旭丘　クスリのアオキ",
                        bunrui = "1",
                        kamotsu_no = "9800000001940005404809700021",
                        matehan = "99111010000037400228816",
                        category = "3",
                        category_nm = "バラ",
                        state = "4"
                    };

                    new MFileHelper().Insert(mFile);

                    // TUMIKOMI180 -> ps file
                    //PsFile psFile = WebService.RequestTumikomi180();
                    PsFile psFile = new PsFile{ pass = "" };
                    new PsFileHelper().Insert(psFile);

                    // TUMIKOMI140 -> mail bag file
                    //MbFile mbFile = WebService.RequestTumikomi140();
                    //MbFile mbFile = new MbFile();
                    //new MbFileHelper().Insert(mbFile);

                    //SoFile soFile = WebService.RequestTumikomi160();
                    SoFile soFile = new SoFile { def_tokuisaki_cd = "0000", ido_vendor_cd = "999999" };
                    new SoFileHelper().Insert(soFile);

                    // TUMIKOMI190 -> ftp file ? 
                    //FtpFile ftpFile = WebService.RequestTumikomi190();
                    //FtpFile ftpFile = new FtpFile {  };
                    //new FtpFileHelper().Insert(ftpFile);

                    // TUMIKOMI260 -> vendor file
                    List<MateFile> mateFile = WebService.RequestTumikomi260();
                    new MateFileHelper().InsertAll(mateFile);

                    // TUMIKOMI270 -> tokuisaki file
                    List<TokuiFile> tokuiFile = WebService.RequestTumikomi270();
                    new TokuiFileHelper().InsertAll(tokuiFile);


                    // 9. 配車テーブルの該当コースの各数量を実績数で更新する
                    /*
                    int result = 0;
                    if (kansen_kbn == 0)
                    {
                        // ret =  proc_tumikomikenpin TUMIKOMI210
                        result = WebService.RequestTumikomi210();
                    }
                    else
                    {
                        // ret =proc_tumikomikenpin TUMIKOMI314
                        result = WebService.RequestTumikomi314();
                    }

                    if(result == "0" || result == "99"){
                        if(result == "99"){
                            JOB:tenpo_zan_flg = true
                        }else{
                            JOB:tenpo_zan_flg = false
                        }
                        Return("msg1")
                    }else{
                        // error
                        // btvOkFlg = 1 <-- 파일 만드는 처리 스킵

                    }


                       ret = 0 or 99 -> comp_OK(),iniZero(4),
                       99 -> tenpo_zan_flg = true or false;
                       alert msg1;

                        not 0, 99 -> btvOkFlg = 1
                     */

                }
                );
                Activity.RunOnUiThread(() =>
                {

                }
                );

            }
            )).Start();


            
        }
    }
}
 