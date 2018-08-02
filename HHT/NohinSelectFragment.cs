using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;

namespace HHT
{
    public class NohinSelectFragment : BaseFragment
    {
        private View view;

        private int kosuMenuflag;
        private string soukoCd, kitakuCd, vendorNm;

        private TextView txtConfirm;
        private EditText etTokuisaki, etTodokesaki;
        private Button btnVendorSearch, confirmButton;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_nohin_select, container, false);

            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();
            
            return view;
        }
        

        public override void OnBarcodeDataReceived(BarcodeDataReceivedEvent_ dataReceivedEvent)
        {
            // When Scanner read some data
            IList<BarcodeDataReceivedEvent_.BarcodeData_> listBarcodeData = dataReceivedEvent.BarcodeData;

            foreach (BarcodeDataReceivedEvent_.BarcodeData_ barcodeData in listBarcodeData)
            {
                this.Activity.RunOnUiThread(() =>
                {
                    // Apply data to UI
                    string densoSymbology = barcodeData.SymbologyDenso;
                    string data = barcodeData.Data;
                    int barcodeDataLength = data.Length;

                    //_textBarData.Text = data;
                    //_textBarType.Text = GetCodeName(densoSymbology);
                    //_textBarDigit.Text = barcodeDataLength.ToString();
                });
            }
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Back)
            {
               
            }
            else if (keycode == Keycode.F1)
            {
                /*
                if (etTodokesaki.IsFocused)
                {
                    bool errFlag = false;
                    MainActivity.ShowProgressBar();

                    if (etSyukaDate.Text == "")
                    {
                        CommonUtils.ShowAlertDialog(view, "エラー", "配送日を入力してください。");
                        etSyukaDate.RequestFocus();
                        errFlag = true;
                        return false;
                    }

                    if (etTokuisaki.Text == "")
                    {
                        CommonUtils.ShowAlertDialog(view, "エラー", "得意先コードを入力してください。");
                        etTokuisaki.RequestFocus();
                        errFlag = true;
                        return false;
                    }

                    new Thread(new ThreadStart(delegate {
                        Activity.RunOnUiThread(() =>
                        {
                            if (!IsExistTokuisaki())
                            {
                                CommonUtils.ShowAlertDialog(view, "エラー", "得意先コードがみつかりません。");
                                etTokuisaki.Text = "";
                                etTokuisaki.RequestFocus();
                                errFlag = true;
                                return;
                            }
                        }
                        );
                        Activity.RunOnUiThread(() => MainActivity.HideProgressBar());
                    }
                    )).Start();

                    if (errFlag)
                    {
                        return false;
                    }

                    editor.PutString("deliveryDate", etSyukaDate.Text);
                    editor.PutString("tokuisaki", etTokuisaki.Text);
                    editor.PutString("deliveryDate", etSyukaDate.Text);
                    editor.PutBoolean("isConfirm", false); // 届先検索フラグ設定
                    editor.Apply();

                    StartFragment(FragmentManager, typeof(KosuBinInputFragment));
                }

                if (etVendorCode.IsFocused)
                {
                    GoVendorSearchPage();
                }
                */

            }
            else if (keycode == Keycode.F4)
            {
                Confirm();
            }

            return true;
        }

        private void Confirm()
        {
            MainActivity.ShowProgressBar();

            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(() =>
                    {
                        
                    }
                );
                Activity.RunOnUiThread(() => MainActivity.HideProgressBar());
               }
            )).Start();
        }
    }
}