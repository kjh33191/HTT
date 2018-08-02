using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using HHT.Resources.DataHelper;
using HHT.Resources.Model;
using Android.App;
using System;
using System.Collections.Generic;

namespace HHT
{
    public class LoginFragment : BaseFragment
    {
        View view;
        
        private LoginHelper loginHelper;
        //private bool btvIniflg = false;
        private EditText etSoukoCode, etDriverCode;
        private TextView txtSoukoName;
        private Button btnLogin;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_login, container, false);
            
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

            if (CommonUtils.IsOnline(this.Activity))
            {
                //ログイン情報をサーバから取得ーーーでも必要？
                //LOGIN020 login020 = WebService.ExecuteLogin020(Handyid);
            }
            else
            {
                //btvIniflg = false;
            }

            SetLastLoginInfo();
            
            
            //db.DeleteAllTantoshya();
            // 担当者マスタ取得
            // LOGIN050
            /*
            Tantohsya tantohsya = new Tantohsya()
            {
                sagyogroup_cd = "104010",
                tantohsya_cd = "10007",
                tantohsya_nm = "熊谷　悟司",
                menu_kbn = "1",
                default_souko = "104"

            };

            db.InsertIntoTablePerson(tantohsya);
            */
            /*
             * if(IsOnline()){
             * 
             * // ログイン情報の存在有無によって、処理が分岐される
             * 最初ログインの場合、LOGIN001
             * 最初ログインではない場合、ローカルから取得
             * LOGIN050
             * 
             * }else{
             * ローカルから前回のログイン情報を取得する。
             * 取得OKの場合、センターCD、センター名を設定する。
             * 取得NGの場合、空白でする
             * OFFLINEフラグ設定（必要あるのか？）
             * }
             * 
             */

            // ローカルからログイン情報取得
            // その後、最初の場合、サーバからデータを取得し、ロカールDBに保存する。

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();
            MainActivity.SetTextFooter("");
        }

        public void Login()
        {
            if(etSoukoCode.Text == "" || txtSoukoName.Text == "")
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

            // 無線管理テーブルへ情報を登録する。
            // LOGIN040

            // 担当者名取得

            if (CommonUtils.IsOnline(this.Activity))
            {
                // btvData = LOGIN030(driver_cd)
                // if btvData is null => "認証できませんでした。\n入力内容をご確認下さい。"
                // not null => JOB:menu_kbn = JOB:arrData[1], JOB: driver_nm = JOB:arrData[0]
            }
            else
            {
                // local dbから担当者情報を取得して判断する
                // if btvData is null => "認証できませんでした。\n入力内容をご確認下さい。"
            }

            // 正常の場合、前回ログイン情報を保存する
            Login loginInfo = new Login
            {
                souko_cd = etSoukoCode.Text,
                souko_nm = txtSoukoName.Text,
                tantousha_cd = etDriverCode.Text
            };

            loginHelper.InsertIntoTableLoginInfo(loginInfo);
            StartFragment(FragmentManager, typeof(MainMenuFragment));
        }

        // センター名取得（倉庫名）LOGIN010
        public async void SetSoukoName(string soukoCd)
        {
            if (soukoCd == "")
            {
                txtSoukoName.Text = "";
                return;
            }

            MainActivity.ShowProgressBar();

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

            MainActivity.HideProgressBar();
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

        private void SetLastLoginInfo()
        {
            // ログイン情報の存在有無によって、処理が分岐される
            Login lastLoginInfo = loginHelper.SelectLastLoginInfo();

            if (lastLoginInfo == null)
            {
                // 最初ログインではない場合、LOGIN050
            }
            else
            {
                etSoukoCode.Text = lastLoginInfo.souko_cd;
                txtSoukoName.Text = lastLoginInfo.souko_nm;
                etDriverCode.RequestFocus();
                // 最初ログインの場合、LOGIN001
            }
        }
    }
}