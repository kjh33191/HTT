using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;
using HHT.Resources.DataHelper;
using HHT.Resources.Model;

namespace HHT
{
    public class IdouNohinSelectFragment : BaseFragment
    {
        private readonly string TAG = "";
        View view;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        EditText etTokuisakiCd, etTodokesakiCd;
        TextView txtTodokesakiNm;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_idou_nohin_select, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            // コンポーネント初期化
            InitComponent();

            return view;
        }

        // コンポーネント初期化
        private void InitComponent()
        {
            etTokuisakiCd = view.FindViewById<EditText>(Resource.Id.et_tokuisakiCd);
            etTodokesakiCd = view.FindViewById<EditText>(Resource.Id.et_todokesakiCd);
            txtTodokesakiNm = view.FindViewById<TextView>(Resource.Id.txt_todokesakiName);
            Button btnConfirm = view.FindViewById<Button>(Resource.Id.btn_confirm);
            btnConfirm.Click += delegate { Confirm(); };
        }

        private void Confirm()
        {
            TokuiFileHelper tokuiFileHelper = new TokuiFileHelper();
            List<TokuiFile> result = tokuiFileHelper.SelectByTokuisakiWithTodokesaki(etTokuisakiCd.Text, etTodokesakiCd.Text);

            if (result.Count > 0)
            {
                // よろしいですか？ 表示
                string confirmMsg = @"
移動先得意先 : @temp1
移動先届先 : @temp2
@temp3

よろしいですか？
                                        ";

                confirmMsg = confirmMsg.Replace("@temp1", result[0].tokuisaki_cd);
                confirmMsg = confirmMsg.Replace("@temp2", result[0].todokesaki_cd);
                confirmMsg = confirmMsg.Replace("@temp3", result[0].tokuisaki_nm);

                CommonUtils.AlertConfirm(view, "確認", confirmMsg, (flag) =>
                {
                    if (flag)
                    {
                        StartFragment(FragmentManager, typeof(IdouNohinWorkFragment));
                    }
                });
            }
            else
            {
                CommonUtils.AlertDialog(view, "", "届先コードがみつかりません。", null);
            }

            //StartFragment(FragmentManager, typeof(IdouNohinWorkFragment));
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F4)
            {
                Confirm();
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