using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Beardedhen.Androidbootstrap;
using Com.Densowave.Bhtsdk.Barcode;
using HHT.Common;
using HHT.Resources.DataHelper;
using HHT.Resources.Model;
using System.Collections.Generic;
using System.Threading;

namespace HHT
{
    public class NohinMailBagKaisyuFragment : BaseFragment
    {
        private readonly string TAG = "NohinMailBagKaisyuFragment";
        private View view;
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;
        private List<string> arrMailBag;
        private int mail_bag;
        private EditText etKaisyuMail;

        private SndNohinMailKaisyuHelper sndNohinMailKaisyuHelper;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_nohin_mailBag_kaisyu, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            InitComponent();

            return view;
        }

        private void InitComponent()
        {
            SetTitle("メールバック回収");
            SetFooterText("");

            TextView tvTokuisaki = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuMail_tokuisakiNm);
            tvTokuisaki.Text = prefs.GetString("tokuisaki_nm", "");

            TextView tvTodokesaki = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuMail_todokisakiNm);
            tvTodokesaki.Text = prefs.GetString("todokesaki_nm", "");

            etKaisyuMail = view.FindViewById<EditText>(Resource.Id.et_nohinKaisyuMail_kaisyuMailbag);

            BootstrapButton btnConfirm = view.FindViewById<BootstrapButton>(Resource.Id.et_nohinKaisyuMail_Confirm);
            btnConfirm.Click += delegate { ConfirmMailBagKaisyu(); };
            
            sndNohinMailKaisyuHelper = new SndNohinMailKaisyuHelper();
            List<SndNohinMailKaisyu> temp = sndNohinMailKaisyuHelper.SelectAll();
            if (temp.Count > 0)
            {
                etKaisyuMail.Text = temp.Count.ToString();
                mail_bag = 0;
            }
            else
            {
                etKaisyuMail.Text = "0";
                mail_bag = 0;
            }
            
            arrMailBag = new List<string>();
            
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Enter)
            {
                ConfirmMailBagKaisyu();
            }
            else if (keycode == Keycode.Back)
            {
                LinearLayout layoutConfirm = view.FindViewById<LinearLayout>(Resource.Id.ly_nohinKaisyuMail_confirm);
                if (layoutConfirm.Visibility == ViewStates.Visible)
                {
                    layoutConfirm.Visibility = ViewStates.Invisible;
                }
                else
                {
                    FragmentManager.PopBackStack();
                }
            }
            return true;
        }

        public override bool OnBackPressed()
        {
            return false;
        }

        private void ConfirmMailBagKaisyu()
        {

            LinearLayout layoutConfirm = view.FindViewById<LinearLayout>(Resource.Id.ly_nohinKaisyuMail_confirm);
            if (layoutConfirm.Visibility == ViewStates.Invisible)
            {
                layoutConfirm.Visibility = ViewStates.Visible;
            }
            else
            {
                // 送信ファイル作成
                foreach (string mailbag in arrMailBag)
                {
                    appendFile(mailbag);
                }

                Log.Debug(TAG, "COMMIT");
                FragmentManager.PopBackStack();
            }
        }

        public override void OnBarcodeDataReceived(BarcodeDataReceivedEvent_ dataReceivedEvent)
        {
            foreach (BarcodeDataReceivedEvent_.BarcodeData_ barcodeData in dataReceivedEvent.BarcodeData)
            {
                string data = barcodeData.Data;

                new Thread(new ThreadStart(delegate {
                    Activity.RunOnUiThread(async () =>
                    {
                        if (data[0].ToString() != "M")
                        {
                            CommonUtils.AlertDialog(view, "", "メールバッグではありません。", null);
                            Log.Debug(TAG, "MAIN BAG KAISYU INPUT_ERR1:" + data);
                            return;
                        }

                        string btvTokuisaki = data.Substring(1, 4);
                        string btvTodokesaki = data.Substring(5, 4);

                        // スキャン済みチェック
                        SndNohinMailHelper sendMailHelper = new SndNohinMailHelper();
                        List<SndNohinMail> result = sendMailHelper.Select(btvTokuisaki, btvTodokesaki, data);
                        if (result.Count != 0)
                        {
                            return;
                        }

                        // 納品メールバッグ重複チェック
                        MbFileHelper mbFileHelper = new MbFileHelper();
                        bool hasData = mbFileHelper.HasExistMailBagData(data);
                        if (true)
                        {
                            var resultYn = await DialogAsync.Show(Activity, "", "回収メールバッグと納入メールバッグが同じですよろしいですか？");

                            if (!resultYn.Value)
                            {
                                return;
                            }
                        }

                        string btvKey1 = btvTokuisaki + btvTodokesaki;
                        string btvKey2 = prefs.GetString("tokuisaki_cd", "") + prefs.GetString("todokesaki_cd", "");

                        if (btvKey1 != btvKey2)
                        {
                            CommonUtils.AlertDialog(view, "", "納入先店舗が違います。", null);
                            Log.Debug(TAG, "納入先店舗が違います  btvKey1 :" + btvKey1 + "  btvKey2 :" + btvKey2);
                            return;
                        }

                        int idx = arrMailBag.FindIndex(x => x == data);
                        if (idx == -1)
                        {
                            arrMailBag.Add(data);
                            mail_bag++;

                            etKaisyuMail.Text = mail_bag.ToString();
                        }
                        else
                        {
                            CommonUtils.AlertDialog(view, "", "既にスキャン済みです。", null);
                            Log.Debug(TAG, "既にスキャン済みです。 data :" + data);
                            return;
                        }


                    });
                }
                )).Start();
            }
        }

        private void appendFile(string mailbag)
        {
            // レコード作成用　値取得
            SndNohinMailKaisyu sndNohinMailKaisyu = new SndNohinMailKaisyu
            {
                wPackage = "00",
                wTerminalID = "11101",
                wProgramID = prefs.GetString("program_id", "NOH"),
                wSagyosyaCD = prefs.GetString("sagyousya_cd", "99999"),
                wSoukoCD = prefs.GetString("souko_cd", "108"),
                wHaisoDate = prefs.GetString("haiso_date", ""),
                wBinNo = prefs.GetString("bin_no", ""),
                wCourse = prefs.GetString("course", ""),
                wDriverCD = prefs.GetString("driver_cd", ""),
                wTokuisakiCD = prefs.GetString("tokuisaki_cd", ""),
                wTodokesakiCD = prefs.GetString("todokesaki_cd", ""),
                wKanriNo = mailbag,
                wVendorCd = prefs.GetString("vendor_cd", ""),
                wMateVendorCd = "",
                wSyukaDate = "0",
                wButsuryuNo = "",
                wKamotuNo = "",
                wMatehan = "",
                wMatehanSu = "0",
                wHHT_no = "11101",
                wNohinKbn = "1", // 商品回収
                wKaisyuKbn = "0",
                wTenkanState = "00",
                wSakiTokuisakiCD = "",
                wSakiTodokesakiCD = "",
                wNohinDate = prefs.GetString("nohin_date", ""),
                wNohinTime = prefs.GetString("nohin_time", "")
            };
            
            //DEF_MAIL_KAI
            sndNohinMailKaisyuHelper.Insert(sndNohinMailKaisyu);

        }
    }
}