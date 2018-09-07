using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;
using HHT.Resources.Model;
using Newtonsoft.Json;

namespace HHT
{
    public class MailManageSelectFragment : BaseFragment
    {
        View view;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        EditText etHaisoDate, etBin;
        TextView txtConfirmMsg, txtTargetName;
        
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
            

            Button btnConfirm = view.FindViewById<Button>(Resource.Id.btn_mailRegistSelect_confirm);
            etHaisoDate = view.FindViewById<EditText>(Resource.Id.et_mailRegistSelect_haiso);
            etBin = view.FindViewById<EditText>(Resource.Id.et_mailRegistSelect_bin);
            

            btnConfirm.Click += delegate { Confirm(); };
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
                        CommonUtils.ShowAlertDialog(view, "日付形式ではありません", "正しい日付を入力してください");
                        etHaisoDate.Text = "";
                        etHaisoDate.RequestFocus();
                    }
                }
            };
            

            etHaisoDate.Text = "18/03/20";
            etHaisoDate.RequestFocus();

        }
        
        private void SearchBinNo()
        {
            ((MainActivity)this.Activity).ShowProgress("便情報を確認しています。");

            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(() =>
                {
                    // get_vendornm(JOB:vendor_cd) == 0 ok
                    // Return("sagyou4") 貸出先検索

                    // get_vendornm(JOB: vendor_cd) == 2 error
                    //"貸出先コードが見つかりません。
                }
                );
                Activity.RunOnUiThread(() => ((MainActivity)this.Activity).DismissDialog());
            }
            )).Start();

        }
        
        private void Confirm()
        {    
           StartFragment(FragmentManager, typeof(MailManageInputFragment));
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {

            if (keycode == Keycode.F4)
            {
                
            }
            else if (keycode == Keycode.Back)
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