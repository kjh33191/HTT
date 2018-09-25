using Android.OS;
using Android.Views;
using Android.Widget;
using HHT.Resources.DataHelper;
using HHT.Resources.Model;
using Android.App;
using System;
using System.Collections.Generic;
using System.Threading;
using Android.Content;
using Android.Preferences;
using Android.Util;
using Android.Media;

namespace HHT
{
    public class LoginFragment : BaseFragment
    {
        private readonly string TAG = "LoginFragment";

        private View view;

        private LoginHelper loginHelper;
        private TantoHelper tantoHelper;

        private EditText etSoukoCode, etDriverCode;
        private TextView txtSoukoName;
        private Button btnLogin;

        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        LOGIN010 login010;

        private readonly string ERROR = "エラー";
        private readonly string ERR_NOT_FOUND_SOUKO = "センター情報が見つかりませんでした。\n再確認してください。";
        private readonly string ERR_NO_INPUT_SOUKO = "倉庫コードを\n入力して下さい。";
        private readonly string ERR_LOGIN_ERROR = "認証できませんでした。\n入力内容をご確認下さい。";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_login, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            loginHelper = new LoginHelper();

            SetTitle("ログイン");
            SetFooterText("");

            etSoukoCode = view.FindViewById<EditText>(Resource.Id.et_login_soukoCode);
            txtSoukoName = view.FindViewById<TextView>(Resource.Id.tv_login_soukoName);
            etDriverCode = view.FindViewById<EditText>(Resource.Id.et_login_driverCode);
            btnLogin = view.FindViewById<Button>(Resource.Id.loginButton);

            etDriverCode.KeyPress += (sender, e) =>{
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    e.Handled = true;
                    CommonUtils.HideKeyboard(Activity);
                    var editText = (EditText)sender;
                    Login();
                }
                else { 
                    e.Handled = false;
                }
            };

            etSoukoCode.FocusChange += delegate {
                if (!etSoukoCode.IsFocused)
                {
                    SetSoukoName(etSoukoCode.Text);
                }
            };

            btnLogin.Click += delegate {Login();};

            btnLogin.FocusChange += delegate {
                if (btnLogin.IsFocused)
                {
                    CommonUtils.HideKeyboard(this.Activity);
                }
            };

            // 以前ログイン情報を設定する。
            //SetLastLoginInfo();
            LoadLastLoginFromDB();

            // 担当者マスタ情報をロカールDBに保存する。
            //SaveTantoMaster();

            return view;
        }

        public void Login()
        {
            bool hasError = false;

            if (etSoukoCode.Text == "" || txtSoukoName.Text == "")
            {
                string alertTitle = Resources.GetString(Resource.String.error);
                string alertBody = Resources.GetString(Resource.String.errorMsg002);

                CommonUtils.AlertDialog(view, alertTitle, alertBody, () => {
                    etSoukoCode.Text = "";
                    txtSoukoName.Text = "";
                    etSoukoCode.RequestFocus();
                });

                Log.Warn(TAG, "SoukoCode is Empty");
                return;
            }

            if (etDriverCode.Text == "")
            {
                string alertTitle = Resources.GetString(Resource.String.error);
                string alertBody = Resources.GetString(Resource.String.errorMsg002);

                CommonUtils.AlertDialog(view, alertTitle, "担当者コードを\n入力して下さい。", () => {
                    etDriverCode.Text = "";
                    etDriverCode.RequestFocus();
                });

                Log.Warn(TAG, "DriverCode is Empty");
                return;
            }

            ((MainActivity)this.Activity).ShowProgress("ログイン情報を確認しています。");

            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(() =>
                {

                    if (CommonUtils.IsHostReachable(WebService.HOST_ADDRESS))
                    {
                        Log.Verbose(TAG, "Status : Host Reached");

                        try
                        {
                            // 無線管理テーブルへ情報を登録する。
                            WebService.RequestLogin040(etDriverCode.Text, etSoukoCode.Text, "11101");

                            // 担当者名取得
                            LOGIN030 login030 = WebService.RequestLogin030(etDriverCode.Text);
                            editor.PutString("menu_kbn", login030.menu_kbn);
                            editor.PutString("driver_nm", login030.tantohsya_nm); 
                        }
                        catch (Exception e)
                        {
                            CommonUtils.AlertDialog(view, "ERROR", "認証できませんでした。\n入力内容をご確認下さい。", () => {
                                etDriverCode.Text = "";
                                etDriverCode.RequestFocus();
                            });

                            hasError = true;
                            Log.Error(TAG, "Login Failed ：" + e.StackTrace.ToString());
                            return;
                        }
                    }
                    else
                    {
                        Tanto tanto = tantoHelper.SelectTantoInfo(etDriverCode.Text);
                        editor.PutString("menu_kbn", tanto.menu_kbn);
                        editor.PutString("driver_nm", tanto.tantohsya_nm);

                    }

                    SetSoukoName(etSoukoCode.Text);

                    // 正常の場合、前回ログイン情報を保存する
                    loginHelper.InsertIntoTableLoginInfo(new Login
                    {
                        souko_cd = etSoukoCode.Text,
                        souko_nm = txtSoukoName.Text,
                        tantousha_cd = etDriverCode.Text
                    });

                    editor.PutString("souko_cd", etSoukoCode.Text);
                    editor.PutString("souko_nm", login010.souko_nm);
                    editor.PutString("driver_cd", etDriverCode.Text);
                    editor.PutString("kitaku_cd", login010.kitaku_cd);
                    editor.PutString("def_tokuisaki_cd", login010.def_tokuisaki_cd);
                    editor.PutString("tsuhshin_kbn", login010.tsuhshin_kbn);
                    editor.PutString("souko_kbn", login010.souko_kbn);
                    
                    editor.Apply();

                    Log.Verbose(TAG, "Login Succeeded");

                }
                );
                Activity.RunOnUiThread(() => {
                    ((MainActivity)this.Activity).DismissDialog();
                    if (!hasError) {
                        //StartFragment(FragmentManager, typeof(MainMenuFragment));
                        string bodyMsg = "";
                        string menu_kbn = prefs.GetString("menu_kbn", "");

                        if (menu_kbn == "0")
                        {
                            bodyMsg = "構内でログインしました。";
                        }
                        else if (menu_kbn == "1")
                        {
                            bodyMsg = "ドライバーでログインしました。";
                        }else if(menu_kbn == "2")
                        {
                            bodyMsg = "管理者でログインしました。";
                        }
                        else if (menu_kbn == "3")
                        {
                            bodyMsg = "ＴＣ２型（花王）でログインしました。";
                        }

                        Toast.MakeText(this.Activity, bodyMsg, ToastLength.Long).Show();
                        
                        ToneGenerator toneGen1 = new ToneGenerator(Stream.Notification, 100);
                        toneGen1.StartTone(Tone.CdmaAlertCallGuard, 1000);
                        
                        StartFragment(FragmentManager, typeof(MainMenuFragment));
                    }

                });
            }
            )).Start();
        }

        // センター名取得（倉庫名）
        public void SetSoukoName(string soukoCd)
        {
            if (soukoCd == "")
            {
                txtSoukoName.Text = "";
                return;
            }
            
            try
            {
                login010 = WebService.RequestLogin010(soukoCd);
                txtSoukoName.Text = login010.souko_nm;
            }
            catch
            {
                CommonUtils.AlertDialog(view, ERROR, ERR_NOT_FOUND_SOUKO, () => {
                    etSoukoCode.Text = "";
                    txtSoukoName.Text = "";
                    etSoukoCode.RequestFocus();
                });
            }
        }

        // 以前ログイン情報を設定する。
        private void SetLastLoginInfo()
        {
            if (CommonUtils.IsHostReachable(WebService.HOST_ADDRESS))
            {
                try
                {
                    LOGIN020 loginInfo = WebService.RequestLogin020("11101");
                    etSoukoCode.Text = loginInfo.souko_cd;
                    SetSoukoName(loginInfo.souko_cd);
                    etDriverCode.RequestFocus();
                }
                catch
                {
                    LoadLastLoginFromDB();
                }
            }
            else
            {
                LoadLastLoginFromDB();
            }

        }

        // ホストと連携されていない場合、ローカルから最終ログイン情報を設定する。
        private void LoadLastLoginFromDB()
        {
            // ログイン情報の存在有無によって、処理が分岐される
            Login lastLoginInfo = loginHelper.SelectLastLoginInfo();

            if (lastLoginInfo != null)
            {
                etSoukoCode.Text = lastLoginInfo.souko_cd;
                txtSoukoName.Text = lastLoginInfo.souko_nm;
                etDriverCode.RequestFocus();
            }
        }
        
        // 担当者マスタ情報をロカールDBに保存する。
        private void SaveTantoMaster()
        {
            try
            {
                List<Tanto> tantoshaList = WebService.RequestLogin050();

                if(tantoHelper == null) tantoHelper = new TantoHelper();
                tantoHelper.InsertTantoList(tantoshaList);

            }
            catch
            {
                // ?
            }
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Back)
            {
                return false;
            }
            else if (keycode == Keycode.F4)
            {
                Login();
            }
            
            return true;
        }

        public override bool OnBackPressed()
        {
            return false;
        }
    }
}
 