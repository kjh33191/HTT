using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Util;
using Android.Views;
using Android.Widget;
using HHT.Resources.DataHelper;
using HHT.Resources.Model;

namespace HHT
{
    public class MainMenuFragment : BaseFragment
    {
        private readonly string TAG = "MainMenuFragment";
        private View view;

        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;
        
        SndNohinMailHelper mailHelper;
        SndNohinMailKaisyuHelper mailKaisyuHelper;
        SndNohinMateHelper mateHelper;
        SndNohinWorkHelper workHelper;
        SndNohinSyohinKaisyuHelper syohinKaisyuHelper;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_menu_main, container, false);
            LinearLayout layout = view.FindViewById<LinearLayout>(Resource.Id.linearLayout2);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            mailHelper = new SndNohinMailHelper();
            mailKaisyuHelper = new SndNohinMailKaisyuHelper();
            mateHelper = new SndNohinMateHelper();
            workHelper = new SndNohinWorkHelper();
            syohinKaisyuHelper = new SndNohinSyohinKaisyuHelper();

            string menu_kbn = prefs.GetString("menu_kbn", "");

            if (menu_kbn == "0")
            {
                // 構内の場合
                LinearLayout konaiLayout = view.FindViewById<LinearLayout>(Resource.Id.konaiLayout);
                konaiLayout.Visibility = ViewStates.Visible;

                Button btnNyuka = view.FindViewById<Button>(Resource.Id.konaiNyuka);
                btnNyuka.Click += delegate { StartFragment(FragmentManager, typeof(NyukaMenuFragment)); };

                Button btnTsumikae = view.FindViewById<Button>(Resource.Id.konaiIdou);
                btnTsumikae.Click += delegate { StartFragment(FragmentManager, typeof(TsumikaeMenuFragment)); };

                Button btnMatehanRegist = view.FindViewById<Button>(Resource.Id.konaiMate);
                btnMatehanRegist.Click += delegate { StartFragment(FragmentManager, typeof(MatehanMenuFragment)); };

                Button btnIdouRegist = view.FindViewById<Button>(Resource.Id.konaiIdouRegist);
                btnIdouRegist.Click += delegate { StartFragment(FragmentManager, typeof(IdouRegistSelectFragment)); };

            }
            else if(menu_kbn == "1")
            {
                // ドライバの場合
                LinearLayout driverLayout = view.FindViewById<LinearLayout>(Resource.Id.driverLayout);
                driverLayout.Visibility = ViewStates.Visible;
                
                Button btnNyuka = view.FindViewById<Button>(Resource.Id.driverNyuka);
                btnNyuka.Click += delegate { StartFragment(FragmentManager, typeof(NyukaMenuFragment)); };

                Button btnTsumikae = view.FindViewById<Button>(Resource.Id.driverIdou);
                btnTsumikae.Click += delegate { StartFragment(FragmentManager, typeof(TsumikaeMenuFragment)); };

                Button btnTsumikomi = view.FindViewById<Button>(Resource.Id.driverTsumikomi);
                btnTsumikomi.Click += delegate { GotoTusumikomi(); };

                Button btnNohin = view.FindViewById<Button>(Resource.Id.driverNohin);
                btnNohin.Click += delegate { StartFragment(FragmentManager, typeof(NohinSelectFragment)); };

                Button btnDataSend = view.FindViewById<Button>(Resource.Id.driverSend);
                btnDataSend.Click += delegate {　DataSend();　};

            }
            else if (menu_kbn == "2")
            {
                // 管理者の場合
                LinearLayout managerLayout = view.FindViewById<LinearLayout>(Resource.Id.managerLayout);
                managerLayout.Visibility = ViewStates.Visible;

                Button btnNyuka = view.FindViewById<Button>(Resource.Id.btn_main_manager_nyuka);
                btnNyuka.Click += delegate { StartFragment(FragmentManager, typeof(NyukaMenuFragment)); };

                Button btnTsumikae = view.FindViewById<Button>(Resource.Id.btn_main_manager_tsumikae);
                btnTsumikae.Click += delegate { StartFragment(FragmentManager, typeof(TsumikaeMenuFragment)); };

                Button btnTsumikomi = view.FindViewById<Button>(Resource.Id.btn_main_manager_tsumikomi);
                btnTsumikomi.Click += delegate { GotoTusumikomi(); };

                Button btnNohin = view.FindViewById<Button>(Resource.Id.btn_main_manager_nohin);
                btnNohin.Click += delegate { StartFragment(FragmentManager, typeof(NohinSelectFragment)); };

                Button btnDataSend = view.FindViewById<Button>(Resource.Id.btn_main_manager_dataSend);
                btnDataSend.Click += delegate { DataSend(); };

                Button btnMatehanRegist = view.FindViewById<Button>(Resource.Id.btn_main_manager_matehanRegist);
                btnMatehanRegist.Click += delegate { StartFragment(FragmentManager, typeof(MatehanMenuFragment)); };

                Button btnMailBag = view.FindViewById<Button>(Resource.Id.btn_main_manager_mailBag);
                btnMailBag.Click += delegate { StartFragment(FragmentManager, typeof(MailMenuFragment)); };

                Button btnIdouRegist = view.FindViewById<Button>(Resource.Id.btn_main_manager_idousakiRegist);
                btnIdouRegist.Click += delegate { StartFragment(FragmentManager, typeof(IdouRegistSelectFragment)); };

                Button btnIdouNohin = view.FindViewById<Button>(Resource.Id.btn_main_manager_idousakiNohin);
                btnIdouNohin.Click += delegate { StartFragment(FragmentManager, typeof(IdouNohinSelectFragment)); };
            }

            SetTitle("業務メニュー");
            SetFooterText("");
            
            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            //管理者基準
            if (keycode == Keycode.Num1)
            {
                StartFragment(FragmentManager, typeof(NyukaMenuFragment));
            }
            else if (keycode == Keycode.Num2)
            {
                StartFragment(FragmentManager, typeof(TsumikaeMenuFragment));
            }
            else if (keycode == Keycode.Num3)
            {
                GotoTusumikomi();
            }
            else if (keycode == Keycode.Num4)
            {
                StartFragment(FragmentManager, typeof(NohinSelectFragment));
            }
            else if (keycode == Keycode.Num5)
            {
                // データ送信
                DataSend();
            }
            else if (keycode == Keycode.Num6)
            {
                // マテハン登録
                StartFragment(FragmentManager, typeof(MatehanMenuFragment));
            }
            else if (keycode == Keycode.Num7)
            {
                // メールバッグ
            }
            else if (keycode == Keycode.Num8)
            {
                // 移動先店舗登録
            }
            else if (keycode == Keycode.Num9)
            {
                // 移動先店舗納品
            }
            else if (keycode == Keycode.Num0)
            {
                // TC2
            }
            else if (keycode == Keycode.Period)
            {
                // 在庫集約
            }
            return true;
        }

        private void GotoTusumikomi()
        {
            List<SndNohinMail> mailList = mailHelper.SelectAll();
            List<SndNohinMailKaisyu> mailKaisyuList = mailKaisyuHelper.SelectAll();
            List<SndNohinMate> mateList = mateHelper.SelectAll();
            List<SndNohinWork> workList = workHelper.SelectAll();
            List<SndNohinSyohinKaisyu> syohinKaisyuList = syohinKaisyuHelper.SelectAll();

            int count = mailList.Count + mailKaisyuList.Count + mateList.Count + workList.Count + syohinKaisyuList.Count;

            if (count > 0)
            {
                ShowDialog("警告", "未送信データが存在します。\n削除して業務を続行しますか？", () => {
                    mailHelper.DeleteAll();
                    mailKaisyuHelper.DeleteAll();
                    mateHelper.DeleteAll();
                    workHelper.DeleteAll();
                    syohinKaisyuHelper.DeleteAll();

                    editor.PutBoolean("mailBagFlag", false);
                    editor.PutBoolean("nohinWorkEndFlag", false);
                    editor.PutBoolean("mailKaisyuEndFlag", false);
                    editor.Apply();

                    StartFragment(FragmentManager, typeof(TsumikomiSelectFragment));
                });
            }
            else
            {
                StartFragment(FragmentManager, typeof(TsumikomiSelectFragment));
            }
        }

        private void DataSend()
        {
            List<SndNohinMail> mailList = mailHelper.SelectAll();
            List<SndNohinMailKaisyu> mailKaisyuList = mailKaisyuHelper.SelectAll();
            List<SndNohinMate> mateList = mateHelper.SelectAll();
            List<SndNohinWork> workList = workHelper.SelectAll();
            List<SndNohinSyohinKaisyu> syohinKaisyuList = syohinKaisyuHelper.SelectAll();

            int count = mailList.Count + mailKaisyuList.Count + mateList.Count + workList.Count + syohinKaisyuList.Count;
            
            if (count == 0)
            {
                ShowDialog("エラー", "送信するデータが存在しません。", () => { });
                return;
            }
            else
            {
                ShowDialog("警告", "データ送信します\nよろしいですか？", () => {
                    try
                    {
                        ((MainActivity)this.Activity).ShowProgress("データ送信中");

                        foreach (SndNohinMail temp in mailList)
                        {
                            Dictionary<string, string> param = SetSendParam(temp);
                            var result = WebService.RequestSend010(param);
                        }

                        Log.Debug(TAG, "メールバックデータ送信完了");

                        foreach (SndNohinMailKaisyu temp in mailKaisyuList)
                        {
                            Dictionary<string, string> param = SetSendParam(temp);
                            var result = WebService.RequestSend010(param);
                        }

                        Log.Debug(TAG, "メールバック回収データ送信完了");

                        foreach (SndNohinMate temp in mateList)
                        {
                            Dictionary<string, string> param = SetSendParam(temp);
                            var result = WebService.RequestSend010(param);

                        }

                        Log.Debug(TAG, "マテハンデータ送信完了");

                        foreach (SndNohinWork temp in workList)
                        {
                            Dictionary<string, string> param = SetSendParam(temp);
                            var result = WebService.RequestSend010(param);

                        }

                        Log.Debug(TAG, "納品作業データ送信完了");

                        foreach (SndNohinSyohinKaisyu temp in syohinKaisyuList)
                        {
                            Dictionary<string, string> param = SetSendParam(temp);
                            var result = WebService.RequestSend010(param);

                        }

                        Log.Debug(TAG, "商品回収データ送信完了");

                        // 削除処理
                        mailHelper.DeleteAll();
                        mailKaisyuHelper.DeleteAll();
                        mateHelper.DeleteAll();
                        workHelper.DeleteAll();
                        syohinKaisyuHelper.DeleteAll();

                        new MFileHelper().DeleteAll();
                        new MbFileHelper().DeleteAll();

                        editor.PutBoolean("mailBagFlag", false);
                        editor.PutBoolean("nohinWorkEndFlag", false);
                        editor.PutBoolean("mailKaisyuEndFlag", false);
                        editor.Apply();
                        
                        ShowDialog("報告", "データ送信完了しました。", () => {
                            ((MainActivity)this.Activity).DismissDialog();
                            FragmentManager.PopBackStack();
                            FragmentManager.PopBackStack();
                        });
                    }
                    catch
                    {
                        ShowDialog("エラー", "例外エラーが発生しました。", () => {
                            ((MainActivity)this.Activity).DismissDialog();
                        });
                    }

                });
            }

        }

        private Dictionary<string, string> SetSendParam(object param)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (param.GetType() == typeof(SndNohinMail))
            {
                SndNohinMail temp = ((SndNohinMail)param);

                result = new Dictionary<string, string>
                {
                    {"pPackage", temp.wPackage },
                    {"pTerminalID", temp.wTerminalID },
                    {"pProgramID",  temp.wProgramID },
                    {"pSagyosyaCD", temp.wSagyosyaCD },
                    {"pSoukoCD", temp.wSoukoCD },
                    {"pHaisoDate", temp.wHaisoDate },
                    {"pBinNo", temp.wBinNo },
                    {"pCourse", temp.wCourse },
                    {"pDriverCD", temp.wDriverCD },
                    {"pTokuisakiCD", temp.wTokuisakiCD },
                    {"pTodokesakiCD", temp.wTodokesakiCD },
                    {"pKanriNo", temp.wKanriNo },
                    {"pVendorCD", temp.wVendorCd },
                    {"pMatehanVendor",temp.wMateVendorCd },
                    {"pSyukaDate", temp.wSyukaDate },
                    {"pButsuryuNo", temp.wButsuryuNo },
                    {"pKamotsuNo", temp.wKamotuNo },
                    {"pMatehan", temp.wMatehan },
                    {"pMatehan_Su", temp.wMatehanSu },
                    {"pHHT_No", temp.wHHT_no }
                };
            }
            else if (param.GetType() == typeof(SndNohinMailKaisyu))
            {
                SndNohinMailKaisyu temp = ((SndNohinMailKaisyu)param);

                result = new Dictionary<string, string>
                {
                    {"pPackage", temp.wPackage },
                    {"pTerminalID", temp.wTerminalID },
                    {"pProgramID",  temp.wProgramID },
                    {"pSagyosyaCD", temp.wSagyosyaCD },
                    {"pSoukoCD", temp.wSoukoCD },
                    {"pHaisoDate", temp.wHaisoDate },
                    {"pBinNo", temp.wBinNo },
                    {"pCourse", temp.wCourse },
                    {"pDriverCD", temp.wDriverCD },
                    {"pTokuisakiCD", temp.wTokuisakiCD },
                    {"pTodokesakiCD", temp.wTodokesakiCD },
                    {"pKanriNo", temp.wKanriNo },
                    {"pVendorCD", temp.wVendorCd },
                    {"pMatehanVendor",temp.wMateVendorCd },
                    {"pSyukaDate", temp.wSyukaDate },
                    {"pButsuryuNo", temp.wButsuryuNo },
                    {"pKamotsuNo", temp.wKamotuNo },
                    {"pMatehan", temp.wMatehan },
                    {"pMatehan_Su", temp.wMatehanSu },
                    {"pHHT_No", temp.wHHT_no },
                };
            }
            else if (param.GetType() == typeof(SndNohinMate))
            {
                SndNohinMate temp = ((SndNohinMate)param);

                result = new Dictionary<string, string>
                {
                    {"pPackage", temp.wPackage },
                    {"pTerminalID", temp.wTerminalID },
                    {"pProgramID",  temp.wProgramID },
                    {"pSagyosyaCD", temp.wSagyosyaCD },
                    {"pSoukoCD", temp.wSoukoCD },
                    {"pHaisoDate", temp.wHaisoDate },
                    {"pBinNo", temp.wBinNo },
                    {"pCourse", temp.wCourse },
                    {"pDriverCD", temp.wDriverCD },
                    {"pTokuisakiCD", temp.wTokuisakiCD },
                    {"pTodokesakiCD", temp.wTodokesakiCD },
                    {"pKanriNo", temp.wKanriNo },
                    {"pVendorCD", temp.wVendorCd },
                    {"pMatehanVendor",temp.wMateVendorCd },
                    {"pSyukaDate", temp.wSyukaDate },
                    {"pButsuryuNo", temp.wButsuryuNo },
                    {"pKamotsuNo", temp.wKamotuNo },
                    {"pMatehan", temp.wMatehan },
                    {"pMatehan_Su", temp.wMatehanSu },
                    {"pHHT_No", temp.wHHT_no },
                };
            }
            else if (param.GetType() == typeof(SndNohinWork))
            {
                SndNohinWork temp = ((SndNohinWork)param);

                result = new Dictionary<string, string>
                {
                    {"pPackage", temp.wPackage },
                    {"pTerminalID", temp.wTerminalID },
                    {"pProgramID",  temp.wProgramID },
                    {"pSagyosyaCD", temp.wSagyosyaCD },
                    {"pSoukoCD", temp.wSoukoCD },
                    {"pHaisoDate", temp.wHaisoDate },
                    {"pBinNo", temp.wBinNo },
                    {"pCourse", temp.wCourse },
                    {"pDriverCD", temp.wDriverCD },
                    {"pTokuisakiCD", temp.wTokuisakiCD },
                    {"pTodokesakiCD", temp.wTodokesakiCD },
                    {"pKanriNo", temp.wKanriNo },
                    {"pVendorCD", temp.wVendorCd },
                    {"pMatehanVendor",temp.wMateVendorCd },
                    {"pSyukaDate", temp.wSyukaDate },
                    {"pButsuryuNo", temp.wButsuryuNo },
                    {"pKamotsuNo", temp.wKamotuNo },
                    {"pMatehan", temp.wMatehan },
                    {"pMatehan_Su", temp.wMatehanSu },
                    {"pHHT_No", temp.wHHT_no },
                    {"pNohinKbn", temp.wNohinKbn },
                    {"pTenkanState", temp.wTenkanState },
                    {"pKaisyuKbn", temp.wKaisyuKbn },
                    {"pSakiTokuisakiCD", temp.wSakiTokuisakiCD },
                    {"pSakiTodokesakiCD", temp.wSakiTodokesakiCD },
                    {"pNohinDate", temp.wNohinDate },
                    {"pNohinTime", temp.wNohinTime }
                };
            }
            else if (param.GetType() == typeof(SndNohinSyohinKaisyu))
            {
                SndNohinSyohinKaisyu temp = ((SndNohinSyohinKaisyu)param);

                result = new Dictionary<string, string>
                {
                    {"pPackage", temp.wPackage },
                    {"pTerminalID", temp.wTerminalID },
                    {"pProgramID",  temp.wProgramID },
                    {"pSagyosyaCD", temp.wSagyosyaCD },
                    {"pSoukoCD", temp.wSoukoCD },
                    {"pHaisoDate", temp.wHaisoDate },
                    {"pBinNo", temp.wBinNo },
                    {"pCourse", temp.wCourse },
                    {"pDriverCD", temp.wDriverCD },
                    {"pTokuisakiCD", temp.wTokuisakiCD },
                    {"pTodokesakiCD", temp.wTodokesakiCD },
                    {"pKanriNo", temp.wKanriNo },
                    {"pVendorCD", temp.wVendorCd },
                    {"pMatehanVendor",temp.wMateVendorCd },
                    {"pSyukaDate", temp.wSyukaDate },
                    {"pButsuryuNo", temp.wButsuryuNo },
                    {"pKamotsuNo", temp.wKamotuNo },
                    {"pMatehan", temp.wMatehan },
                    {"pMatehan_Su", temp.wMatehanSu },
                    {"pHHT_No", temp.wHHT_no },
                };
            }

            return result;
        }
        
    }
}