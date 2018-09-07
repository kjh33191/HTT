﻿using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;

namespace HHT
{
    public class IdouRegistSelectFragment : BaseFragment
    {
        View view;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        EditText etKaisyuDate, etHaisoDate;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_idou_regist_select, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            // コンポーネント初期化
            InitComponent();

            return view;
        }

        // コンポーネント初期化
        private void InitComponent()
        {
            etKaisyuDate = view.FindViewById<EditText>(Resource.Id.et_idouRegistSelect_kaisyuDate);
            etKaisyuDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
            etKaisyuDate.FocusChange += (sender, e) => {
                if (e.HasFocus)
                {
                    etKaisyuDate.Text = etKaisyuDate.Text.Replace("/", "");
                }
                else
                {
                    try
                    {
                        etKaisyuDate.Text = CommonUtils.GetDateYYYYMMDDwithSlash(etKaisyuDate.Text);
                    }
                    catch
                    {
                        CommonUtils.ShowAlertDialog(view, "日付形式ではありません", "正しい日付を入力してください");
                        etKaisyuDate.Text = "";
                        etKaisyuDate.RequestFocus();
                    }
                }
            };


            etHaisoDate = view.FindViewById<EditText>(Resource.Id.et_idouRegistSelect_haisoDate);
            etHaisoDate.Text = DateTime.Now.AddDays(1).ToString("yyyy/MM/dd");

            etHaisoDate.Click += (sender, e) => {
                DateTime today = DateTime.Today;
                DatePickerDialog dialog = new DatePickerDialog(this.Activity, OnDateSet, today.Year, today.Month, today.Day);
                dialog.DatePicker.MinDate = today.Millisecond;
                dialog.Show();
            };

            Button btnConfirm = view.FindViewById<Button>(Resource.Id.btn_idouRegistSelect_confirm);
            btnConfirm.Click += delegate { Confirm(); };
            
        }

        void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            etHaisoDate.Text = e.Date.ToLongDateString();
        }
        
        private void Confirm()
        {    
           StartFragment(FragmentManager, typeof(IdouRegistWorkFragment));
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {

            if (keycode == Keycode.F4)
            {
                
            }

            return true;
        }

        public override void OnBarcodeDataReceived(BarcodeDataReceivedEvent_ dataReceivedEvent)
        {
            IList<BarcodeDataReceivedEvent_.BarcodeData_> listBarcodeData = dataReceivedEvent.BarcodeData;

            foreach (BarcodeDataReceivedEvent_.BarcodeData_ barcodeData in listBarcodeData)
            {
                this.Activity.RunOnUiThread(() =>
                {
                    string data = barcodeData.Data;
                 
                    
                });
            }
        }
    }
}