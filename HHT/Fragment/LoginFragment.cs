﻿using Android.OS;
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
using Com.Beardedhen.Androidbootstrap;
using Android.Content.Res;

namespace HHT
{
    public class LoginFragment : BaseFragment
    {
        private readonly string TAG = "LoginFragment";

        private View view;

        private LoginHelper loginHelper;
        private TantoHelper tantoHelper;

        private BootstrapEditText etSoukoCode, etDriverCode;
        private TextView txtSoukoName;
        private BootstrapButton btnLogin;

        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        LOGIN010 login010;
        string def_tokuisaki_cd, kitaku_cd, tsuhshin_kbn, souko_kbn;
        
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
            HideFooter();

            loginHelper = new LoginHelper();

            ((MainActivity)this.Activity).SupportActionBar.Title = "ログイン";
            
            etSoukoCode = view.FindViewById<BootstrapEditText>(Resource.Id.soukoCode);
            etSoukoCode.FocusChange += delegate {
                if (!etSoukoCode.IsFocused)
                {
                    ((MainActivity)this.Activity).ShowProgress("読み込み中");

                    new Thread(new ThreadStart(delegate
                    {
                        Activity.RunOnUiThread(() =>
                        {
                            SetSoukoName(etSoukoCode.Text);
                        });
                        Activity.RunOnUiThread(() => ((MainActivity)this.Activity).DismissDialog());
                    }
                    )).Start();
                    
                }
            };

            txtSoukoName = view.FindViewById<TextView>(Resource.Id.tv_login_soukoName);
            
            etDriverCode = view.FindViewById<BootstrapEditText>(Resource.Id.tantoCode);
            etDriverCode.KeyPress += (sender, e) => {
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
            
            btnLogin = view.FindViewById<BootstrapButton>(Resource.Id.loginButton);
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

                ShowDialog(alertTitle, alertBody, () => {
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

                ShowDialog(alertTitle, "担当者コードを\n入力して下さい。", ()=> { });
                
                Log.Warn(TAG, "DriverCode is Empty");
                return;
            }

            ((MainActivity)this.Activity).ShowProgress("ログインしています");

            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(async () =>
                {

                    if (await CommonUtils.IsHostReachable(WebService.GetHostIpAddress()))
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
                            
                            // 正常の場合、前回ログイン情報を保存する
                            loginHelper.Insert(new Login
                            {
                                souko_cd = etSoukoCode.Text,
                                souko_nm = txtSoukoName.Text,
                                tantousha_cd = etDriverCode.Text,
                                menu_flg = login030.menu_kbn,
                                tantohsya_nm = login030.tantohsya_nm,
                                def_tokuisaki_cd = this.def_tokuisaki_cd,
                                kitaku_cd = this.kitaku_cd,
                                souko_kbn = this.souko_kbn,
                                tsuhshin_kbn = this.tsuhshin_kbn
                            });

                        }
                        catch (Exception e)
                        {
                            ShowDialog("エラー", "認証できませんでした。\n入力内容をご確認下さい。", () => {
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
                        ShowDialog("エラー", "サーバから答えがありません。", () => { });
                        hasError = true;
                        return;

                        /*
                        tantoHelper = new TantoHelper();
                        Tanto tanto = tantoHelper.SelectTantoInfo(etDriverCode.Text);
                        editor.PutString("menu_kbn", tanto.menu_kbn);
                        editor.PutString("driver_nm", tanto.tantohsya_nm);

                        SetSoukoName(etSoukoCode.Text);

                        // 正常の場合、前回ログイン情報を保存する
                        loginHelper.InsertIntoTableLoginInfo(new Login
                        {
                            souko_cd = etSoukoCode.Text,
                            souko_nm = txtSoukoName.Text,
                            tantousha_cd = etDriverCode.Text,
                            menu_flg = tanto.menu_kbn,
                            tantohsya_nm = tanto.tantohsya_nm,
                            def_tokuisaki_cd = this.def_tokuisaki_cd,
                            kitaku_cd = this.kitaku_cd
                        });

                        */
                    }

                    // TEMP 
                    // ***********************;
                    editor.PutString("terminal_id", "432660068");
                    editor.PutString("hht_no", "11101");
                    // ***********************

                    editor.PutString("souko_cd", etSoukoCode.Text);
                    editor.PutString("souko_nm", txtSoukoName.Text);
                    editor.PutString("driver_cd", etDriverCode.Text);
                    editor.PutString("sagyousya_cd", etDriverCode.Text);
                    editor.PutString("kitaku_cd", kitaku_cd);
                    editor.PutString("def_tokuisaki_cd", def_tokuisaki_cd);
                    editor.PutString("tsuhshin_kbn", tsuhshin_kbn);
                    editor.PutString("souko_kbn", souko_kbn);
                    
                    editor.Apply();

                    Log.Verbose(TAG, "Login Succeeded");

                }
                );
                Activity.RunOnUiThread(() => {
                    ((MainActivity)this.Activity).DismissDialog();
                    if (!hasError) {
                        string menu_kbn = prefs.GetString("menu_kbn", "");

                        AudioAttributes attributes = new AudioAttributes.Builder()
                                        .SetUsage(AudioUsageKind.Game)
                                        .SetContentType(AudioContentType.Sonification)
                                        .Build();

                        SoundPool soundPool = new SoundPool.Builder()
                                                            .SetAudioAttributes(attributes)
                                                            // ストリーム数に応じて
                                                            .SetMaxStreams(2)
                                                            .Build();


                        //AssetManager assets = this.Activity.Assets;

                        //soundPool = new SoundPool(5, Stream.Ring, 0);
                        soundPool.Play(soundPool.Load(this.Activity, Resource.Raw.beep, 0), 1.0f, 1.0f, 0, 0, 1);

                        //MediaPlayer mp = MediaPlayer.Create(this.Activity, Resource.Raw.beep);
                        //mp.Start();
                        //ToneGenerator toneGen1 = new ToneGenerator(Stream.System, 100);
                        //toneGen1.StartTone(Tone.PropBeep, 1000);

                        try
                        {
                            StartFragment(FragmentManager, typeof(MainMenuFragment));
                        }
                        catch (Exception e)
                        {
                            Log.Debug(TAG, e.StackTrace.ToString());
                        }
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
                def_tokuisaki_cd = login010.def_tokuisaki_cd;
                kitaku_cd = login010.kitaku_cd;
                tsuhshin_kbn = login010.tsuhshin_kbn;
                souko_kbn = login010.souko_kbn;
            }
            catch (Exception e)
            {
                Log.Debug(TAG, e.StackTrace.ToString());
                
                ShowDialog(ERROR, ERR_NOT_FOUND_SOUKO, () => {
                    etSoukoCode.Text = "";
                    txtSoukoName.Text = "";
                    etSoukoCode.RequestFocus();
                });
            }
        }

        // 以前ログイン情報を設定する。
        private void SetLastLoginInfo()
        {
            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(async () =>
                {
                    if (await CommonUtils.IsHostReachable(WebService.GetHostIpAddress()))
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
                );
            }
            )).Start();
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
                def_tokuisaki_cd = lastLoginInfo.def_tokuisaki_cd;
                kitaku_cd = lastLoginInfo.kitaku_cd;
                
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
            if (keycode == Keycode.F3)
            {
                StartFragment(FragmentManager, typeof(ConfigFragment));
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
 