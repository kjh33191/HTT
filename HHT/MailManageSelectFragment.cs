﻿using System;
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

namespace HHT
{
    public class MailManageSelectFragment : BaseFragment
    {
        private readonly string TAG = "MailManageSelectFragment";
        View view;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        EditText etHaisoDate, etBin;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_mail_manage_select, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            // コンポーネント初期化
            InitComponent();
            
            return view;
        }

        // コンポーネント初期化
        private void InitComponent()
        {
            bool registFlg = prefs.GetBoolean("registFlg", true);

            if (registFlg)
            {
                SetTitle("メールバッグ登録");
            }
            else
            {
                SetTitle("メールバッグ削除");
            }
            
            etBin = view.FindViewById<EditText>(Resource.Id.et_mailRegistSelect_bin);

            Button btnConfirm = view.FindViewById<Button>(Resource.Id.btn_mailRegistSelect_confirm);
            btnConfirm.Click += delegate { Confirm(); };

            etHaisoDate = view.FindViewById<EditText>(Resource.Id.et_mailRegistSelect_haiso);
            etHaisoDate.FocusChange += (sender, e) => {
                if (e.HasFocus)
                {
                    etHaisoDate.Text = etHaisoDate.Text.Replace("/", "");
                }
                else
                {
                    try
                    {
                        etHaisoDate.Text = CommonUtils.GetDateYYMMDDwithSlash(etHaisoDate.Text);
                    }
                    catch
                    {
                        CommonUtils.ShowAlertDialog(view, "", "日付を正しく入力してください。");
                        etHaisoDate.Text = "";
                        etHaisoDate.RequestFocus();
                    }
                }
            };

            etHaisoDate.RequestFocus();
            etHaisoDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
            
        }
        
        private void Confirm()
        {   
            // Input Check
            if(etHaisoDate.Text == "")
            {
                CommonUtils.ShowAlertDialog(view, "", "配送日を入力してください");
                etHaisoDate.RequestFocus();
                return;
            }

            if (etBin.Text == "")
            {
                CommonUtils.ShowAlertDialog(view, "", "便を入力してください");
                etBin.RequestFocus();
                return;
            }

            // 登録済みメールバッグ数を取得
            string souko_cd = prefs.GetString("souko_cd", "");
            string haisoDate = "20" + etHaisoDate.Text.Replace("/", "");

            try
            {
                int cnt = WebService.RequestMAIL020(souko_cd, haisoDate, etBin.Text);
                editor.PutInt("mail_back", cnt);
                editor.Apply();
                
                StartFragment(FragmentManager, typeof(MailManageInputFragment));
            }
            catch
            {
                CommonUtils.AlertDialog(view, "", "", null);
                Log.Error(TAG, "登録済みメールバッグ数取得に失敗しました。");
            }
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F4)
            {
                Confirm();
            }

            return true;
        }
    }
}