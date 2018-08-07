using System.Collections.Generic;
using System.Threading;
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

            etKosu = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_kosu);
            etCarLabel = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_carLabel);
            etCarry = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_carry);
            etKargo = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_kargoCar);
            etCard = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_card);
            etBara = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_bara);
            etSonata = view.FindViewById<EditText>(Resource.Id.et_tsumikomiWork_sonota);

            btnIdou = view.FindViewById<Button>(Resource.Id.et_tsumikomiWork_idou);
            btnIdou.Click += delegate { };
            
            kansen_kbn = 0;
            
            carLabelInputMode = false;
            zoubin_flg = prefs.GetString("zoubin_flg", "1");

            // 참고. TODO 삭제할것.
            // 사교5,6,7은 같은 화면에 처리방식이 다름. 
            // 사교5는 定番コース단골코스(zoubin_flg = 1)
            // 사교5은 사교5의車両ラベル入力화면 (sagyou5에서 납품바코드가 한번 읽히면 이동)
            // 사교7은 増便コース증편코스(zoubin_flg >= 2)
            // 그렇다면 zoubin_flg로 일단 로직을 갈라내는게 좋을거같다. 

            if (zoubin_flg == "1")
            {
                GetTenpoMatehanInfo();  // 作業5
            }
            else
            {
                GetCountSouko(); // 作業7
            }
            
            return view;
        }
        
        // 総個数取得 TUMIKOMI050
        private int GetCountSouko()
        {
            var progress = ProgressDialog.Show(this.Activity, "Please wait...", "Contacting server. Please wait...", true);
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
                    Activity.RunOnUiThread(() => progress.Dismiss());

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

                    if (zoubin_flg == "1" && kansen_kbn != 0)
                    {
                        //string resultJson = CommonUtils.Post(WebService.TUMIKOMI.TUMIKOMI310, param);
                        //TUMIKOMI310 result = JsonConvert.DeserializeObject<TUMIKOMI310>(resultJson);

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
                        //string resultJson = CommonUtils.Post(WebService.TUMIKOMI.TUMIKOMI060, param);
                        //TUMIKOMI060 result = JsonConvert.DeserializeObject<TUMIKOMI060>(resultJson);

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
            var progress = ProgressDialog.Show(this.Activity, "", "マテハン情報取得しています。", true);

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

                    if (kansen_kbn == 0)
                    {
                        //string resultJson = CommonUtils.Post(WebService.TUMIKOMI.TUMIKOMI040, param);
                        //List<TUMIKOMI040> result = JsonConvert.DeserializeObject<List<TUMIKOMI010>>(resultJson);
                    }
                    else
                    {
                        //string resultJson = CommonUtils.Post(WebService.TUMIKOMI.TUMIKOMI300, param);
                        //List<TUMIKOMI300> result = JsonConvert.DeserializeObject<List<TUMIKOMI300>>(resultJson);
                    }

                    List<Dictionary<string, string>> resultList = new List<Dictionary<string, string>>();

                    foreach (Dictionary<string, string> result in resultList)
                    {
                        string btvCategory = result["name_cd"];
                        string btvCategoryNm = result["category_nm"];
                        string btvKosu = result["cnt"];

                        if (btvCategory == "00")
                        {
                            // キャリーラベル
                            // category1_nm = btvCategoryNm
                            // category1_su = btvKosu
                        }
                        else if (btvCategory == "01") {
                            // category2_nm = btvCategoryNm
                            // category2_su = btvKosu
                        }
                        else if (btvCategory == "02") {
                            // category3_nm = btvCategoryNm
                            // category3_su = btvKosu
                        }
                        else if (btvCategory == "03") {
                            // category4_nm = btvCategoryNm
                            // category4_su = btvKosu
                        }
                        else if (btvCategory == "04") {
                            // category5_nm = btvCategoryNm
                            // category5_su = btvKosu
                        }
                    }
                }
                );
                Activity.RunOnUiThread(() => progress.Dismiss());
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
            //scan_flg = true;
            //btvOkFlag == 0 then
            // 
            

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
        }
    }
}
 