using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;
using Java.Nio.Charset;

namespace HHT
{
    [Activity(Theme = "@style/AppTheme",
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, BarcodeManager_.IBarcodeManagerListener_, BarcodeScanner_.IBarcodeDataListener_
    {
        private const string TAG = "MainActivity";

        static ProgressDialog progress;
        static LinearLayout footerLayout;
        static TextView txtFooterBody;

        private BarcodeManager_ mBarcodeManager = null;
        private BarcodeScanner_ mBarcodeScanner = null;
        private BarcodeScannerSettings_ mSettings = null;
        private BarcodeScannerInfo_.BarcodeScannerType_ mScannerType = null;

        private bool mResumed = false;

        readonly ReadOnlyCollection<ScanSettings_.TriggerMode_> TRIGGER_MODE = Array.AsReadOnly(new ScanSettings_.TriggerMode_[] {
                ScanSettings_.TriggerMode_.AutoOff,
                ScanSettings_.TriggerMode_.Momentary,
                ScanSettings_.TriggerMode_.Alternate,
                ScanSettings_.TriggerMode_.Continuous,
                ScanSettings_.TriggerMode_.TriggerRelease
        });

        readonly ReadOnlyCollection<BarcodeScannerInfo_.BarcodeScannerType_> SCANNER_TYPE = Array.AsReadOnly(new BarcodeScannerInfo_.BarcodeScannerType_[] {
                BarcodeScannerInfo_.BarcodeScannerType_.Type1d,
                BarcodeScannerInfo_.BarcodeScannerType_.Type2d,
                BarcodeScannerInfo_.BarcodeScannerType_.Type2dLong
        });

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            footerLayout = FindViewById<LinearLayout>(Resource.Id.footerLayout);
            txtFooterBody = FindViewById<TextView>(Resource.Id.tv_foot_body);

            try
            {
                MainActivity mainActivity = this;
                BarcodeManager_.Create(this, this);
            }
            catch (Java.Lang.NullPointerException ex)
            {
                mScannerType = null;
                mSettings = null;
            }

            FragmentManager.BeginTransaction().Replace(Resource.Id.fragmentContainer, new LoginFragment()).Commit();
            //FragmentManager.BeginTransaction().Replace(Resource.Id.fragmentContainer, new TsumikomiWorkFragment()).Commit();

        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            Fragment localFragment = FragmentManager.FindFragmentById(Resource.Id.fragmentContainer);

            if ((localFragment is BaseFragment baseFragment) && (((BaseFragment)localFragment).OnKeyDown(keyCode, e)))
            {
                return base.OnKeyDown(keyCode, e);
            }
            return base.OnKeyDown(keyCode, e);
        }

        public static void ShowFooter()
        {
            footerLayout.Visibility = ViewStates.Visible;
        }

        public static void HideFooter()
        {
            footerLayout.Visibility = ViewStates.Gone;
        }

        public static void SetTextFooter(string content)
        {
            txtFooterBody.Text = content;
        }

        public void ShowProgress(string message)
        {
            if (progress == null)
            {
                progress = new ProgressDialog(this);
            }
            progress.Indeterminate = true;
            progress.SetMessage(message);
            progress.SetCancelable(false);
            progress.Show();
        }

        public void DismissDialog()
        {
            if (progress != null && progress.IsShowing)
                progress.Dismiss();
        }

        protected override void OnResume()
        {
            base.OnResume();

            try
            {
                if (mBarcodeScanner != null)
                {
                    mBarcodeScanner.AddDataListener(this);

                    if (mSettings == null)
                    {
                        mSettings = mBarcodeScanner.Settings;
                        SetScanner();
                    }
                    mBarcodeScanner.Settings = mSettings;
                    mBarcodeScanner.Claim();
                }

            }
            catch (BarcodeException_ e)
            {
                Log.Error(TAG, "ErrorCode is " + e.ErrorCode, e);
            }
            catch (Java.Lang.IllegalArgumentException e)
            {
                Toast toast = Toast.MakeText(this, Resource.String.error_message_symbol_settings, ToastLength.Long);
                toast.Show();
            }
            mResumed = true;
        }

        public void OnBarcodeManagerCreated(BarcodeManager_ barcodeManager)
        {
            // When barcode scanner manager created.
            mBarcodeManager = barcodeManager;
            try
            {
                IList<BarcodeScanner_> listScanner = barcodeManager.BarcodeScanners;
                if (listScanner.Count > 0)
                {
                    // Get BarcodeScanner instance
                    mBarcodeScanner = listScanner[0];   // 0 is default scanner

                    if (mResumed)
                    {
                        // Register Data Received event
                        mBarcodeScanner.AddDataListener(this);

                        // Setting for Scanner
                        if (mScannerType == null)
                        {
                            mScannerType = mBarcodeScanner.Info.Type;
                        }
                        if (mSettings == null)
                        {
                            mSettings = mBarcodeScanner.Settings;
                            this.SetScanner();
                        }

                        mBarcodeScanner.Settings = mSettings;

                        // Enable Scanner
                        mBarcodeScanner.Claim();
                    }
                }
            }
            catch (BarcodeException_ e)
            {
                Log.Error(TAG, "ErrorCode is " + e.ErrorCode, e);
            }
        }

        public void OnBarcodeDataReceived(BarcodeDataReceivedEvent_ dataReceivedEvent)
        {
            Fragment localFragment = FragmentManager.FindFragmentById(Resource.Id.fragmentContainer);
            BaseFragment baseFragment = localFragment as BaseFragment;

            ((BaseFragment)localFragment).OnBarcodeDataReceived(dataReceivedEvent);
        }

        private void SetScanner()
        {
            // Scanner default settings
            BarcodeScannerSettings_ settings = mBarcodeScanner.Settings;

            // Trigger Mode
            settings.Scan.TriggerMode = ScanSettings_.TriggerMode_.AutoOff;
            //settings.Scan.TriggerMode = ScanSettings_.TriggerMode_.Momentary;
            //settings.Scan.TriggerMode = ScanSettings_.TriggerMode_.Alternate;
            //settings.Scan.TriggerMode = ScanSettings_.TriggerMode_.Continuous;
            //settings.Scan.TriggerMode = ScanSettings_.TriggerMode_.TriggerRelease;

            // For 2D Module Settings
            if (mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2d)
            {
                // Light Mode
                settings.Scan.LightMode = ScanSettings_.LightMode_.Auto;
                //settings.Scan.LightMode = ScanSettings_.LightMode_.AlwaysOn;
                //settings.Scan.LightMode = ScanSettings_.LightMode_.Off;

                //Marker Mode
                settings.Scan.MarkerMode = ScanSettings_.MarkerMode_.Normal;
                //settings.Scan.MarkerMode = ScanSettings_.MarkerMode_.Ahead;
                //settings.Scan.MarkerMode = ScanSettings_.MarkerMode_.Off;
            }

            // For 2D LONG Module Settings
            if (mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2dLong)
            {
                settings.Scan.SideLightMode = ScanSettings_.SideLightMode_.Off;
                //settings.Scan.SideLightMode = ScanSettings_.SideLightMode_.On;
            }

            // Notification Sound Settings
            settings.Notification.Sound.Enabled = true;
            //settings.Notification.Sound.Enabled = false;

            if (settings.Notification.Sound.Enabled)
            {
                settings.Notification.Sound.UsageType = NotificationSettings_.UsageType_.Ringtone;
                //settings.Notification.Sound.UsageType = NotificationSettings_.UsageType_.Media;
                //settings.Notification.Sound.UsageType = NotificationSettings_.UsageType_.Alarm;
                
                if (settings.Notification.Sound.UsageType == NotificationSettings_.UsageType_.Media)
                {
                    //TO BE Implement
                    settings.Notification.Sound.GoodDecodeFilePath = "";
                }
            }

            //Notification Vibrator
            //settings.Notification.Vibrate.Enabled = false;
            settings.Notification.Vibrate.Enabled = true;


            // Decode Settings

            // Decode interval
            settings.Decode.SameBarcodeIntervalTime = 10; // 1,000msec

            // For 1D & 2D Module Settings
            if ((mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type1d) ||
                    (mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2d))
            {
                // Decode Level
                settings.Decode.DecodeLevel = 4;              // Decode Level
            }

            // Invert Mode
            settings.Decode.InvertMode = DecodeSettings_.InvertMode_.Disabled;
            //settings.Decode.InvertMode = DecodeSettings_.InvertMode_.InversionOnly;
            //settings.Decode.InvertMode = DecodeSettings_.InvertMode_.Auto;

            // For 2D & 2D LONG Module Settings
            if ((mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2d) ||
                    (mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2dLong))
            {
                // Point Scan Mode
                settings.Decode.PointScanMode = DecodeSettings_.PointScanMode_.Disabled;
                //settings.Decode.PointScanMode = DecodeSettings_.PointScanMode_.Enabled;
            }

            // For 2D Module Settings
            if (mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2d)
            {
                // Reverse Mode
                settings.Decode.ReverseMode = DecodeSettings_.ReverseMode_.Disabled;
                //settings.Decode.ReverseMode = DecodeSettings_.ReverseMode_.Enabled;
            }

            // Encode Charset
            settings.Decode.Charset = Charset.ForName("Shift-JIS");
            //settings.Decode.Charset = Charset.ForName("UTF-8");


            // Symbology Settings

            // For 2D Module Settings
            if (mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2d)
            {
                // Multi Line
                settings.Decode.MultiLineMode.Enabled = false;
            }

            //JAN-13(EAN-13), UPC-A
            settings.Decode.Symbologies.Ean13UpcA.Enabled = true;
            if ((mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type1d) ||
                    (mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2d))
            {
                settings.Decode.Symbologies.Ean13UpcA.FirstCharacter = "";
                settings.Decode.Symbologies.Ean13UpcA.SecondCharacter = "";
            }
            settings.Editing.Ean13.ReportCheckDigit = true;
            settings.Editing.UpcA.ReportCheckDigit = true;
            settings.Editing.UpcA.AddLeadingZero = true;

            // EAN-13 add on
            settings.Decode.Symbologies.Ean13UpcA.AddOn.Enabled = false;
            settings.Decode.Symbologies.Ean13UpcA.AddOn.OnlyWithAddOn = false;
            if ((mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type1d) ||
                    (mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2d))
            {
                settings.Decode.Symbologies.Ean13UpcA.AddOn.AddOn2Digit = false;
                settings.Decode.Symbologies.Ean13UpcA.AddOn.AddOn5Digit = false;
            }

            // JAN-8(EAN-8)
            settings.Decode.Symbologies.Ean8.Enabled = true;
            if ((mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type1d) ||
                    (mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2d))
            {
                settings.Decode.Symbologies.Ean8.FirstCharacter = "";
                settings.Decode.Symbologies.Ean8.SecondCharacter = "";
            }
            settings.Editing.Ean8.ReportCheckDigit = true;

            // EAN-8 add on
            settings.Decode.Symbologies.Ean8.AddOn.Enabled = false;
            settings.Decode.Symbologies.Ean8.AddOn.OnlyWithAddOn = false;
            if ((mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type1d) ||
                    (mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2d))
            {
                settings.Decode.Symbologies.Ean8.AddOn.AddOn2Digit = false;
                settings.Decode.Symbologies.Ean8.AddOn.AddOn5Digit = false;
            }

            // UPC-E
            settings.Decode.Symbologies.UpcE.Enabled = true;
            if ((mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type1d) ||
                    (mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2d))
            {
                settings.Decode.Symbologies.UpcE.FirstCharacter = "";
                settings.Decode.Symbologies.UpcE.SecondCharacter = "";
            }
            settings.Editing.UpcE.ReportCheckDigit = true;
            settings.Editing.UpcE.AddLeadingZero = false;
            settings.Editing.UpcE.ConvertToUpcA = false;
            settings.Editing.UpcE.ReportNumberSystemCharacterOfConvertedUpcA = true;

            // UPC-E add on
            settings.Decode.Symbologies.UpcE.AddOn.Enabled = false;
            settings.Decode.Symbologies.UpcE.AddOn.OnlyWithAddOn = false;
            if ((mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type1d) ||
                    (mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2d))
            {
                settings.Decode.Symbologies.UpcE.AddOn.AddOn2Digit = false;
                settings.Decode.Symbologies.UpcE.AddOn.AddOn5Digit = false;
            }

            // ITF
            settings.Decode.Symbologies.Itf.Enabled = true;
            settings.Decode.Symbologies.Itf.LengthMin = 4;
            settings.Decode.Symbologies.Itf.LengthMax = 99;
            settings.Decode.Symbologies.Itf.VerifyCheckDigit = false;
            //settings.Decode.Symbologies.itf.verifyCheckDigit = true;
            settings.Editing.Itf.ReportCheckDigit = true;

            // STF
            settings.Decode.Symbologies.Stf.Enabled = true;
            settings.Decode.Symbologies.Stf.LengthMin = 4;
            settings.Decode.Symbologies.Stf.LengthMax = 99;
            settings.Decode.Symbologies.Stf.VerifyCheckDigit = false;
            //settings.Decode.Symbologies.stf.verifyCheckDigit = true;
            if ((mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type1d) ||
                    (mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2dLong))
            {
                settings.Decode.Symbologies.Stf.StartStopCharacter = "";
                //settings.Decode.Symbologies.Stf.StartStopCharacter = "S";
                //settings.Decode.Symbologies.Stf.StartStopCharacter = "N";
            }
            settings.Editing.Stf.ReportCheckDigit = true;

            // Codabar
            settings.Decode.Symbologies.Codabar.Enabled = true;
            settings.Decode.Symbologies.Codabar.LengthMin = 4;
            settings.Decode.Symbologies.Codabar.LengthMax = 99;
            settings.Decode.Symbologies.Codabar.VerifyCheckDigit = false;
            //settings.Decode.Symbologies.Codabar.VerifyCheckDigit = true;
            if ((mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type1d) ||
                    (mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2d))
            {
                settings.Decode.Symbologies.Codabar.StartStopCharacter = "";
            }
            settings.Editing.Codabar.ReportCheckDigit = true;
            settings.Editing.Codabar.ReportStartStopCharacter = true;
            settings.Editing.Codabar.ConvertToUpperCase = false;

            // Code39
            settings.Decode.Symbologies.Code39.Enabled = true;
            settings.Decode.Symbologies.Code39.LengthMin = 1;
            settings.Decode.Symbologies.Code39.LengthMax = 99;
            settings.Decode.Symbologies.Code39.VerifyCheckDigit = false;
            //settings.Decode.Symbologies.Code39.VerifyCheckDigit = true;
            settings.Editing.Code39.ReportCheckDigit = true;
            settings.Editing.Code39.ReportStartStopCharacter = false;

            // Code93
            settings.Decode.Symbologies.Code93.Enabled = true;
            settings.Decode.Symbologies.Code93.LengthMin = 1;
            settings.Decode.Symbologies.Code93.LengthMax = 99;

            // Code128
            settings.Decode.Symbologies.Code128.Enabled = true;
            settings.Decode.Symbologies.Code128.LengthMin = 1;
            settings.Decode.Symbologies.Code128.LengthMax = 99;

            // MSI
            if (mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type1d)
            {
                settings.Decode.Symbologies.Msi.Enabled = true;
                settings.Decode.Symbologies.Msi.LengthMin = 1;
                settings.Decode.Symbologies.Msi.LengthMax = 99;
                settings.Decode.Symbologies.Msi.NumberOfCheckDigitVerification = 1;
                //settings.Decode.Symbologies.Msi.NumberOfCheckDigitVerification = 2;
            }

            // GS1 Databar
            settings.Decode.Symbologies.Gs1DataBar.Enabled = true;
            if ((mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type1d) ||
                    (mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2d))
            {
                settings.Decode.Symbologies.Gs1DataBar.Stacked = false;
            }

            // Gs1 Databar Limited
            settings.Decode.Symbologies.Gs1DataBarLimited.Enabled = false;

            // Gs1 Databar Expanded
            settings.Decode.Symbologies.Gs1DataBarExpanded.Enabled = false;
            settings.Decode.Symbologies.Gs1DataBarExpanded.LengthMin = 1;
            settings.Decode.Symbologies.Gs1DataBarExpanded.LengthMax = 99;
            if ((mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type1d) ||
                    (mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2d))
            {
                settings.Decode.Symbologies.Gs1DataBarExpanded.Stacked = false;
            }

            // Gs1 Composite
            if ((mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2d) ||
                    (mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2dLong))
            {
                settings.Decode.Symbologies.Gs1Composite.Enabled = false;
            }

            if ((mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2d) ||
                    (mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2dLong))
            {

                // QR Code
                settings.Decode.Symbologies.QrCode.Enabled = false;

                if (mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2d)
                {
                    settings.Decode.Symbologies.QrCode.SplitMode = Symbologies_.SplitModeQr_.Disabled;
                    //settings.Decode.Symbologies.QrCode.SplitMode = Symbologies_.SplitModeQr_.Edit;
                    //settings.Decode.Symbologies.QrCode.SplitMode = Symbologies_.SplitModeQr_.BatchEdit;
                    //settings.Decode.Symbologies.QrCode.SplitMode = Symbologies_.SplitModeQr_.NonEdit;

                    // QR Code Model1
                    settings.Decode.Symbologies.QrCode.Model1.Enabled = true;
                    settings.Decode.Symbologies.QrCode.Model1.VersionMin = 1;
                    settings.Decode.Symbologies.QrCode.Model1.VersionMax = 22;

                    // QR Code Model2
                    settings.Decode.Symbologies.QrCode.Model2.Enabled = true;
                    settings.Decode.Symbologies.QrCode.Model2.VersionMin = 1;
                    settings.Decode.Symbologies.QrCode.Model2.VersionMax = 40;

                    // Micro QR Code
                    settings.Decode.Symbologies.MicroQr.Enabled = true;
                    settings.Decode.Symbologies.MicroQr.VersionMin = 1;
                    settings.Decode.Symbologies.MicroQr.VersionMax = 4;

                    // iQR Code
                    settings.Decode.Symbologies.IqrCode.Enabled = true;
                    settings.Decode.Symbologies.IqrCode.SplitMode = Symbologies_.SplitModeIqr_.Disabled;
                    //settings.Decode.Symbologies.IqrCode.SplitMode = Symbologies_.SplitModeIqr_.Edit;
                    //settings.Decode.Symbologies.IqrCode.SplitMode = Symbologies_.SplitModeIqr_.NonEdit;

                    // Square iQR Code
                    settings.Decode.Symbologies.IqrCode.Square.Enabled = true;
                    settings.Decode.Symbologies.IqrCode.Square.VersionMin = 1;
                    settings.Decode.Symbologies.IqrCode.Square.VersionMax = 61;

                    // Rectangle iQR Code
                    settings.Decode.Symbologies.IqrCode.Rectangle.Enabled = true;
                    settings.Decode.Symbologies.IqrCode.Rectangle.VersionMin = 1;
                    settings.Decode.Symbologies.IqrCode.Rectangle.VersionMax = 15;
                }
                else
                {
                    //For 2D Long model
                    settings.Decode.Symbologies.MicroQr.Enabled = false;
                    settings.Decode.Symbologies.IqrCode.Enabled = false;
                }

                // Data Matrix
                settings.Decode.Symbologies.DataMatrix.Enabled = true;

                if (mScannerType == BarcodeScannerInfo_.BarcodeScannerType_.Type2d)
                {
                    // DataMatrix Square
                    settings.Decode.Symbologies.DataMatrix.Square.Enabled = true;
                    settings.Decode.Symbologies.DataMatrix.Square.CodeNumberMin = 1;
                    settings.Decode.Symbologies.DataMatrix.Square.CodeNumberMax = 24;

                    // DataMatrix ReactAngle
                    settings.Decode.Symbologies.DataMatrix.Rectangle.Enabled = true;
                    settings.Decode.Symbologies.DataMatrix.Rectangle.CodeNumberMin = 1;
                    settings.Decode.Symbologies.DataMatrix.Rectangle.CodeNumberMax = 6;
                }

                // PDF417
                settings.Decode.Symbologies.Pdf417.Enabled = true;

                // Micro PDF 417
                settings.Decode.Symbologies.MicroPdf417.Enabled = true;

                // Maxi
                settings.Decode.Symbologies.MaxiCode.Enabled = true;
            }
            else
            {
                //For 1D model
                settings.Decode.Symbologies.QrCode.Enabled = false;
                settings.Decode.Symbologies.MicroQr.Enabled = false;
                settings.Decode.Symbologies.IqrCode.Enabled = false;
                settings.Decode.Symbologies.Pdf417.Enabled = false;
                settings.Decode.Symbologies.MicroPdf417.Enabled = false;
                settings.Decode.Symbologies.MaxiCode.Enabled = false;
                settings.Decode.Symbologies.DataMatrix.Enabled = false;
            }

            mBarcodeScanner.Settings = settings;
        }

        // Get Barcode Type 
        private String GetCodeName(String denso)
        {
            // Get Code Name from DENSO Code Pattern
            String ret = "";

            switch (denso)
            {
                case "A":
                    ret = GetString(Resource.String.symbol_label_ean13);
                    break;
                case "B":
                    ret = GetString(Resource.String.symbol_label_ean8);
                    break;
                case "C":
                    ret = GetString(Resource.String.symbol_label_upce);
                    break;
                case "I":
                    ret = GetString(Resource.String.symbol_label_itf);
                    break;
                case "H":
                    ret = GetString(Resource.String.symbol_label_stf);
                    break;
                case "N":
                    ret = GetString(Resource.String.symbol_label_codabar);
                    break;
                case "M":
                    ret = GetString(Resource.String.symbol_label_code39);
                    break;
                case "L":
                    ret = GetString(Resource.String.symbol_label_code93);
                    break;
                case "K":
                    ret = GetString(Resource.String.symbol_label_code128);
                    break;
                case "W":
                    ret = GetString(Resource.String.symbol_label_gs1_128);
                    break;
                case "P":
                    ret = GetString(Resource.String.symbol_label_msi);
                    break;
                case "R":
                    ret = GetString(Resource.String.symbol_label_gs1_databar);
                    break;
                case "Q":
                    ret = GetString(Resource.String.symbol_label_qr);
                    break;
                case "G":
                    ret = GetString(Resource.String.symbol_label_iqr);
                    break;
                case "Y":
                    ret = GetString(Resource.String.symbol_label_pdf417);
                    break;
                case "X":
                    ret = GetString(Resource.String.symbol_label_maxi);
                    break;
                case "Z":
                    ret = GetString(Resource.String.symbol_label_data_matrix);
                    break;
                default:
                    break;
            }

            return ret;
        }

        public override void OnBackPressed()
        {
            Fragment localFragment = FragmentManager.FindFragmentById(Resource.Id.fragmentContainer);
            BaseFragment baseFragment = localFragment as BaseFragment;

            if (((BaseFragment)localFragment).OnBackPressed())
            {
                base.OnBackPressed();
            }
        }
    }
}