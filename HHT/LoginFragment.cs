using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using HHT.Resources.DataHelper;
using HHT.Resources.Model;
using Android.App;
using System;
using System.Collections.Generic;
using System.Threading;
using Android.Content;
using Android.Preferences;
using System.Threading.Tasks;

namespace HHT
{
    public class LoginFragment : BaseFragment
    {
        View view;

        private LoginHelper loginHelper;
        private TantoHelper tantoHelper;

        private EditText etSoukoCode, etDriverCode;
        private TextView txtSoukoName;
        private Button btnLogin;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;
        
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

            etSoukoCode.FocusChange += delegate {
                if (!etSoukoCode.IsFocused)
                {
                    SetSoukoName(etSoukoCode.Text);
                }
            };

            btnLogin.Click += delegate { Login(); };

            btnLogin.FocusChange += delegate {
                if (btnLogin.IsFocused)
                {
                    CommonUtils.HideKeyboard(this.Activity);
                }
            };

            // 以前ログイン情報を設定する。
            SetLastLoginInfo();

            // 担当者マスタ情報をロカールDBに保存する。
            SaveTantoMaster();

            return view;
        }

        private void RegistMusenKanri()
        {
            //ログイン情報をサーバから取得ーーーでも必要？
            Dictionary<string, string> param = new Dictionary<string, string>
                        {
                            { "driver_cd",  etDriverCode.Text},
                            { "souko_cd",  etSoukoCode.Text},
                            { "htt_id",  "11101"}
                        };

            CommonUtils.Post(WebService.LOGIN.LOGIN040, param);
        }

        private async Task<LOGIN030> GetLOGIN030(string driver_cd)
        {
            LOGIN030 login030 = new LOGIN030();
            //ログイン情報をサーバから取得ーーーでも必要？
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "tantohsya_cd",  driver_cd}
            };

            string resultJson030 = await CommonUtils.PostAsync(WebService.LOGIN.LOGIN030, param);
            login030 = JsonConvert.DeserializeObject<LOGIN030>(resultJson030);

            return login030;
        }

        public void Login()
        {
            ((MainActivity)this.Activity).ShowProgress("ログイン情報を確認しています。");
            
            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(async () =>
                {
                    Thread.Sleep(2000);

                    if (etSoukoCode.Text == "" || txtSoukoName.Text == "")
                    {
                        string alertTitle = Resources.GetString(Resource.String.error);
                        string alertBody = Resources.GetString(Resource.String.errorMsg002);

                        CommonUtils.AlertDialog(view, alertTitle, alertBody, () => {
                            etSoukoCode.Text = "";
                            txtSoukoName.Text = "";
                            etSoukoCode.RequestFocus();
                        });

                        return;
                    }
                    
                    if (CommonUtils.IsOnline(this.Activity))
                    {
                        // 無線管理テーブルへ情報を登録する。
                        RegistMusenKanri();

                        // 担当者名取得
                        LOGIN030 login030 = await GetLOGIN030(etDriverCode.Text);
                        editor.PutString("menu_kbn", login030.menu_kbn);
                        editor.PutString("driver_nm", login030.tantohsya_nm);
                    }
                    else
                    {
                        Tanto tanto = tantoHelper.SelectTantoInfo(etDriverCode.Text);
                        editor.PutString("menu_kbn", tanto.menu_kbn);
                        editor.PutString("driver_nm", tanto.tantohsya_nm);
                    }

                    // 正常の場合、前回ログイン情報を保存する
                    Login loginInfo = new Login
                    {
                        souko_cd = etSoukoCode.Text,
                        souko_nm = txtSoukoName.Text,
                        tantousha_cd = etDriverCode.Text
                    };

                    loginHelper.InsertIntoTableLoginInfo(loginInfo);
                }
                );
                Activity.RunOnUiThread(() => {
                    ((MainActivity)this.Activity).DismissDialog();
                    StartFragment(FragmentManager, typeof(MainMenuFragment));
                });
                //Activity.RunOnUiThread(() => ((MainActivity)this.Activity).DismissDialog());
                //Activity.RunOnUiThread(() => StartFragment(FragmentManager, typeof(MainMenuFragment)));
            }
            )).Start();
        }

        // センター名取得（倉庫名）LOGIN010
        public async void SetSoukoName(string soukoCd)
        {
            if (soukoCd == "")
            {
                txtSoukoName.Text = "";
                return;
            }
            
            view.ClearFocus();
            view.Focusable = false;
            view.FocusableInTouchMode = false;
            
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>
                {
                    { "souko_cd", soukoCd }
                };

                LOGIN010 login010 = await WebService.ExecuteLogin010(param);
                txtSoukoName.Text = login010.souko_nm;

                editor.PutString("kitaku_cd", login010.kitaku_cd);
                editor.PutString("def_tokuisaki_cd", login010.def_tokuisaki_cd);
                editor.PutString("souko_nm", login010.souko_nm);
                editor.PutString("tsuhshin_kbn", login010.tsuhshin_kbn);
                editor.PutString("souko_kbn", login010.souko_kbn);
                editor.Apply();

            }
            catch
            {
                string alertTitle = Resources.GetString(Resource.String.error);
                string alertBody = Resources.GetString(Resource.String.errorMsg001);
                
                CommonUtils.AlertDialog(view, alertTitle, alertBody, () => {
                    etSoukoCode.Text = "";
                    txtSoukoName.Text = "";
                    etSoukoCode.RequestFocus();
                });
            }

            view.Focusable = true;
            view.FocusableInTouchMode = true;
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

        // 以前ログイン情報を設定する。
        private void SetLastLoginInfo()
        {
            if (CommonUtils.IsOnline(this.Activity))
            {
                //ログイン情報をサーバから取得ーーーでも必要？
                Dictionary<string, string> param = new Dictionary<string, string>
                        {
                            { "hht_id",  "11101"}
                        };

                string resultJson020 = CommonUtils.Post(WebService.LOGIN.LOGIN020, param);
                LOGIN020 loginInfo = JsonConvert.DeserializeObject<LOGIN020>(resultJson020);

                editor.PutString("souko_cd", loginInfo.souko_cd);
                editor.Apply();
                etSoukoCode.Text = loginInfo.souko_cd;
                SetSoukoName(loginInfo.souko_cd);
                etDriverCode.RequestFocus();

                //btvIniflg = true;
            }
            else
            {
                //btvIniflg = false;

                // ログイン情報の存在有無によって、処理が分岐される
                Login lastLoginInfo = loginHelper.SelectLastLoginInfo();

                if (lastLoginInfo != null)
                {
                    etSoukoCode.Text = lastLoginInfo.souko_cd;
                    txtSoukoName.Text = lastLoginInfo.souko_nm;
                    etDriverCode.RequestFocus();
                }

            }

        }

        // 担当者マスタ情報をロカールDBに保存する。
        private void SaveTantoMaster()
        {
            string resultJson050 = CommonUtils.Post(WebService.LOGIN.LOGIN050, new Dictionary<string, string>());
            List<Tanto> result = JsonConvert.DeserializeObject<List<Tanto>>(resultJson050);

            tantoHelper = new TantoHelper();
            tantoHelper.InsertTantoList(result);
        }
    }
}