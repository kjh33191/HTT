using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Util;
using Android.Views;
using Android.Widget;
using HHT.Resources.DataHelper;
using HHT.Resources.Model;
using System.Collections.Generic;
using System.Threading;

namespace HHT
{
    public class NohinMenuFragment : BaseFragment
    {
        private string TAG = "NohinMenuFragment";
        private View view;
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        private bool hasMailBagData;

        SndNohinMailHelper mailHelper;
        SndNohinMailKaisyuHelper mailKaisyuHelper;
        SndNohinMateHelper mateHelper;
        SndNohinWorkHelper workHelper;
        SndNohinSyohinKaisyuHelper syohinKaisyuHelper;

        List<SndNohinMail> mailList;
        List<SndNohinMailKaisyu> mailKaisyuList;
        List<SndNohinMate> mateList;
        List<SndNohinWork> workList;
        List<SndNohinSyohinKaisyu> syohinKaisyuList;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_menu_nohin, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetTitle("納品検品");
            SetFooterText("");

            bool isDone = prefs.GetBoolean("mailBagFlag", false);

            MbFileHelper mbFileHelper = new MbFileHelper();
            hasMailBagData = mbFileHelper.HasExistMailBagData();

            SndNohinWorkHelper sndNohinWorkHelper = new SndNohinWorkHelper();
            int nohinCount = sndNohinWorkHelper.SelectAll().Count;

            Button button1 = view.FindViewById<Button>(Resource.Id.btn_nohinMenu_mailNohin);
            button1.Click += delegate {

                if (prefs.GetBoolean("mailBagFlag", false))
                {
                    CommonUtils.AlertDialog(view, "確認", "メールバッグ納品処理は終了しています。", null);
                }
                else
                {
                    StartFragment(FragmentManager, typeof(NohinMailBagNohinFragment));
                }
            };

            Button button2 = view.FindViewById<Button>(Resource.Id.btn_nohinMenu_nohin);
            button2.Click += delegate {
                bool errorFlag = false;
                
                if (!prefs.GetBoolean("mailBagFlag", false))
                {
                    Log.Debug(TAG, "メールバッグ納品処理が終了していません。");
                    errorFlag = true;
                    CommonUtils.AlertDialog(view, "確認", "メールバッグ納品処理が終了していません。", () => { return; });
                }
                else if (prefs.GetBoolean("nohinWorkEndFlag", false))
                {
                    Log.Debug(TAG, "納品処理は終了しています。");
                    errorFlag = true;
                    CommonUtils.AlertDialog(view, "確認", "納品処理は終了しています。", () => { return; });
                }
                
                if (errorFlag == false)
                    StartFragment(FragmentManager, typeof(NohinWorkFragment));

            };

            Button button3 = view.FindViewById<Button>(Resource.Id.btn_nohinMenu_kaisyu); // 回収業務
            button3.Click += delegate {
                bool errorFlag = false;

                /*
                if (!prefs.GetBoolean("mailBagFlag", false))
                {
                    Log.Debug(TAG, "メールバッグ納品処理が終了していません。");
                    errorFlag = true;
                    CommonUtils.AlertDialog(view, "確認", "メールバッグ納品処理が終了していません。", () => { return; });
                }
                else 
                if (!prefs.GetBoolean("nohinWorkEndFlag", false))
                {
                    Log.Debug(TAG, "納品処理が終了していません。");
                    errorFlag = true;
                    CommonUtils.AlertDialog(view, "確認", "納品処理が終了していません。", () => { return; });
                }
                else if (prefs.GetBoolean("kaisyuEndFlag", false))
                {
                    Log.Debug(TAG, "納品処理が終了していません。");
                    errorFlag = true;
                    CommonUtils.AlertDialog(view, "確認", "納品処理が終了していません。", () => { return; });
                }
                */
                if (errorFlag == false)
                    StartFragment(FragmentManager, typeof(NohinKaisyuMenuFragment));
            };

            Button button4 = view.FindViewById<Button>(Resource.Id.btn_nohinMenu_mailKaisyu);
            button4.Click += delegate {
                bool errorFlag = false;

                /*
                if (!prefs.GetBoolean("mailBagFlag", false))
                {
                    Log.Debug(TAG, "メールバッグ納品処理が終了していません。");
                    errorFlag = true;
                    CommonUtils.AlertDialog(view, "確認", "メールバッグ納品処理が終了していません。", () => { return; });
                } else 
                if (!prefs.GetBoolean("kaisyuEndFlag", false))
                {
                    Log.Debug(TAG, "回収処理が終了していません。");
                    errorFlag = true;
                    CommonUtils.AlertDialog(view, "確認", "回収処理が終了していません。", () => { return; });
                }
                else if (prefs.GetBoolean("mailKaisyuEndFlag", false))
                {
                    Log.Debug(TAG, "メールバッグ回収処理は終了しています。");
                    errorFlag = true;
                    CommonUtils.AlertDialog(view, "確認", "メールバッグ回収処理は終了しています。", () => { return; });
                }
                */
                if (errorFlag == false)
                    StartFragment(FragmentManager, typeof(NohinMailBagKaisyuFragment)); }; // sagyou2

            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Num1)
            {
                StartFragment(FragmentManager, typeof(MatehanSelectFragment));
            }
            else if (keycode == Keycode.Num2)
            {
                StartFragment(FragmentManager, typeof(MatehanSelectFragment));
            }
            else if (keycode == Keycode.Num3)
            {
                StartFragment(FragmentManager, typeof(MatehanSelectFragment));
            }
            else if (keycode == Keycode.Num4)
            {
                StartFragment(FragmentManager, typeof(MatehanSelectFragment));
            }

            return true;
        }

        private bool HasSendData()
        {
            SndNohinMailHelper mailHelper = new SndNohinMailHelper();
            SndNohinMailKaisyuHelper mailKaisyuHelper = new SndNohinMailKaisyuHelper();
            SndNohinMateHelper mateHelper = new SndNohinMateHelper();
            SndNohinWorkHelper workHelper = new SndNohinWorkHelper();
            SndNohinSyohinKaisyuHelper syohinKaisyuHelper = new SndNohinSyohinKaisyuHelper();

            mailList = mailHelper.SelectAll();
            mailKaisyuList = mailKaisyuHelper.SelectAll();
            mateList = mateHelper.SelectAll();
            workList = workHelper.SelectAll();
            syohinKaisyuList = syohinKaisyuHelper.SelectAll();

            int count = mailList.Count + mailKaisyuList.Count + mateList.Count + workList.Count + syohinKaisyuList.Count;

            return count != 0;
        }

        public override bool OnBackPressed()
        {
            //bool isReached = await CommonUtils.IsHostReachable(WebService.GetHostIpAddress());

            if (HasSendData())
            {
                CommonUtils.AlertConfirm(view, "", "納品情報を送信して業務メニューに戻ってよろしいですか？", (flag) =>
                {
                    if (flag)
                    {
                        ((MainActivity)this.Activity).ShowProgress("データ送信中");

                        /*
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
                        */

                        Activity.RunOnUiThread(() =>
                        {
                            /*
                            // 削除処理
                            mailHelper.DeleteAll();
                            mailKaisyuHelper.DeleteAll();
                            mateHelper.DeleteAll();
                            workHelper.DeleteAll();
                            syohinKaisyuHelper.DeleteAll();

                            //new MFileHelper().DeleteAll();
                            //new MbFileHelper().DeleteAll();
                            //new PsFileHelper().DeleteAll();

                            */
                            ((MainActivity)this.Activity).DismissDialog();

                            CommonUtils.AlertDialog(view, "", "データ送信完了しました。", () =>
                            {
                                FragmentManager.PopBackStack();
                                FragmentManager.PopBackStack();
                            });
                        });
                    }
                });
            }
            else
            {
                CommonUtils.AlertConfirm(view, "", "業務メニューに戻ってよろしいですか？", (flag) => {
                    if (flag)
                    {
                        Log.Debug(TAG, "NOHIN_END");
                        FragmentManager.PopBackStack();
                    }
                });
            }

            return false;
        }

        private void DataSend()
        {
            SndNohinMailHelper mailHelper = new SndNohinMailHelper();
            SndNohinMailKaisyuHelper mailKaisyuHelper = new SndNohinMailKaisyuHelper();
            SndNohinMateHelper mateHelper = new SndNohinMateHelper();
            SndNohinWorkHelper workHelper = new SndNohinWorkHelper();
            SndNohinSyohinKaisyuHelper syohinKaisyuHelper = new SndNohinSyohinKaisyuHelper();

            List<SndNohinMail> mailList = mailHelper.SelectAll();
            List<SndNohinMailKaisyu> mailKaisyuList = mailKaisyuHelper.SelectAll();
            List<SndNohinMate> mateList = mateHelper.SelectAll();
            List<SndNohinWork> workList = workHelper.SelectAll();
            List<SndNohinSyohinKaisyu> syohinKaisyuList = syohinKaisyuHelper.SelectAll();

            int count = mailList.Count + mailKaisyuList.Count + mateList.Count + workList.Count + syohinKaisyuList.Count;

            if (count > 0)
            {
                new Thread(new ThreadStart(delegate
                {
                    Activity.RunOnUiThread(() =>
                    {
                        // 業務メニューに戻ってよろしいですか？ 
                        CommonUtils.AlertConfirm(view, "", "納品情報を送信して業務メニューに戻ってよろしいですか？", (flag) =>
                            {
                                if (flag)
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

                                }
                            });
                    }
                    );
                    Activity.RunOnUiThread(() =>
                    {
                        // 削除処理
                        mailHelper.DeleteAll();
                        mailKaisyuHelper.DeleteAll();
                        mateHelper.DeleteAll();
                        workHelper.DeleteAll();
                        syohinKaisyuHelper.DeleteAll();

                        //new MFileHelper().DeleteAll();
                        //new MbFileHelper().DeleteAll();
                        
                    });
                    Activity.RunOnUiThread(() =>
                    {
                        CommonUtils.AlertDialog(view, "", "データ送信完了しました。", () =>
                        {
                            ((MainActivity)this.Activity).DismissDialog();
                            FragmentManager.PopBackStack();
                        });
                        
                    });
                }

            )).Start();
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
                    {"pHHT_No", temp.wHHT_no },
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
 