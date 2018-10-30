using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using Com.Beardedhen.Androidbootstrap;
using Com.Densowave.Bhtsdk.Barcode;

namespace HHT
{
    public class IdouRegistSelectFragment : BaseFragment
    {
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        private View view;
        private BootstrapEditText etKaisyuDate, etHaisoDate;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_idou_regist_select, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetTitle("移動先店舗登録");

            // コンポーネント初期化
            InitComponent();

            return view;
        }

        // コンポーネント初期化
        private void InitComponent()
        {
            etKaisyuDate = view.FindViewById<BootstrapEditText>(Resource.Id.et_idouRegistSelect_kaisyuDate);
            etKaisyuDate.FocusChange += (sender, e) => {
                if (e.HasFocus)
                {
                    etKaisyuDate.Text = etKaisyuDate.Text.Replace("/", "");
                    etKaisyuDate.SetSelection(etKaisyuDate.Text.Length);
                }
                else
                {
                    try
                    {
                        etKaisyuDate.Text = CommonUtils.GetDateYYYYMMDDwithSlash(etKaisyuDate.Text);
                    }
                    catch
                    {
                        ShowDialog("エラー", "正しい日付を入力してください。", () => {
                            etKaisyuDate.Text = "";
                            etKaisyuDate.RequestFocus();
                        });
                    }
                }
            };

            etHaisoDate = view.FindViewById<BootstrapEditText>(Resource.Id.et_idouRegistSelect_haisoDate);
            etHaisoDate.FocusChange += (sender, e) => {
                if (e.HasFocus)
                {
                    etHaisoDate.Text = etHaisoDate.Text.Replace("/", "");
                    etHaisoDate.SetSelection(etHaisoDate.Text.Length);
                }
                else
                {
                    try
                    {
                        etHaisoDate.Text = CommonUtils.GetDateYYYYMMDDwithSlash(etKaisyuDate.Text);
                    }
                    catch
                    {
                        ShowDialog("エラー", "正しい日付を入力してください。", () => {
                            etHaisoDate.Text = "";
                            etHaisoDate.RequestFocus();
                        });
                    }
                }
            };

            BootstrapButton btnConfirm = view.FindViewById<BootstrapButton>(Resource.Id.btn_idouRegistSelect_confirm);
            btnConfirm.Click += delegate { Confirm(); };
            
            etKaisyuDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
            etHaisoDate.Text = DateTime.Now.AddDays(1).ToString("yyyy/MM/dd");
            
        }
        
        private void Confirm()
        {
            editor.PutString("syuka_date", etKaisyuDate.Text.Replace("/", ""));
            editor.PutString("kaisyu_date", etKaisyuDate.Text.Replace("/", ""));
            editor.PutString("haisou_date", etHaisoDate.Text.Replace("/", ""));
            editor.Apply();
            StartFragment(FragmentManager, typeof(IdouRegistWorkFragment));
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
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