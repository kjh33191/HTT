using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Beardedhen.Androidbootstrap;
using Com.Densowave.Bhtsdk.Barcode;
using HHT.Resources.DataHelper;
using HHT.Resources.Model;
using System.Collections.Generic;

namespace HHT
{
    public class NohinMailBagNohinFragment : BaseFragment
    {
        private readonly string TAG = "NohinMailBagNohinFragment";
        private View view;
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;
        private SndNohinMailHelper sndNohinMailHelper;
        private MbFileHelper mbFileHelper;

        private BootstrapEditText etMailBagNumber;
        private string tokuisakiCd;
        private string todokesakiCd;
        private int mailbackCnt;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_nohin_mailBag_nohin, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();
            sndNohinMailHelper = new SndNohinMailHelper();
            mbFileHelper = new MbFileHelper();

            tokuisakiCd = prefs.GetString("tokuisaki_cd", "");
            todokesakiCd = prefs.GetString("todokesaki_cd", "");

            SetTitle("メールバック納品");
            SetFooterText(" F1:解除");

            TextView txtTokusakiName = view.FindViewById<TextView>(Resource.Id.txt_nohinMailBagNohin_tokuisakiNm);
            txtTokusakiName.Text = prefs.GetString("tokuisaki_nm", "");

            etMailBagNumber = view.FindViewById<BootstrapEditText>(Resource.Id.et_nohinMailBagNohin_mailbagNumber);
            etMailBagNumber.Text = "0";

            List<SndNohinMail> mailList = sndNohinMailHelper.SelectAll();
            if (mailList.Count > 0) etMailBagNumber.Text = mailList.Count.ToString();

            // メールバッグ件数取得
            mailbackCnt = mbFileHelper.GetMailbackCount();

            BootstrapButton kaizoButton = view.FindViewById<BootstrapButton>(Resource.Id.btn_nohinMailBagNohin_kaizo);
            kaizoButton.Click += delegate {
                //menu_flg
                editor.PutString("menu_flg", "1");
                editor.Apply();
                StartFragment(FragmentManager, typeof(NohinMailBagPasswordFragment)); };

            BootstrapButton muButton = view.FindViewById<BootstrapButton>(Resource.Id.btn_nohinMailBagNohin_mu);
            muButton.Click += delegate { CompleteMailNohin(); };

            if (mailbackCnt != 0)
            {
                muButton.Enabled = false;
            }

            return view;
        }

        private void CompleteMailNohin()
        {
            CommonUtils.AlertConfirm(view, "", "メールバッグ納品業務を終了しますか？", (flag) => {
                if (flag)
                {
                    Log.Debug(TAG, "F3 return pushed : " + prefs.GetString("tokuisaki_cd", "") + ", " + prefs.GetString("tokuisaki_nm", ""));
                    editor.PutBoolean("mailBagFlag", true);
                    editor.Apply();
                    FragmentManager.PopBackStack();
                }
                else
                {

                }
            });
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F1)
            {
                StartFragment(FragmentManager, typeof(NohinMailBagPasswordFragment));
            }
            if (keycode == Keycode.F3)
            {
                if (mailbackCnt == 0)
                {
                    CompleteMailNohin();
                }
            }

            return true;
        }

        public override bool OnBackPressed()
        {
            return false;
        }

        public override void OnBarcodeDataReceived(BarcodeDataReceivedEvent_ dataReceivedEvent)
        {
            IList<BarcodeDataReceivedEvent_.BarcodeData_> listBarcodeData = dataReceivedEvent.BarcodeData;

            foreach (BarcodeDataReceivedEvent_.BarcodeData_ barcodeData in listBarcodeData)
            {
                this.Activity.RunOnUiThread(() =>
                {
                    string data = barcodeData.Data;   
                    MbFileHelper mbFileHelper = new MbFileHelper();

                    if (mbFileHelper.HasExistMailBagData(data))
                    {
                        // 이미 스캔되었는지 확인
                        if (CheckMailBagData(data))
                        {
                            CommonUtils.AlertDialog(view, "", "既にスキャン済みです。", null);
                            return;
                        }

                        string btvTokuisaki = data.Substring(1, 4);
                        string btvTodokesaki = data.Substring(5, 4);

                        // 해당 바코드가 토쿠이,토도케 값이 맞는지 확인.
                        if (tokuisakiCd != btvTokuisaki || todokesakiCd != btvTodokesaki)
                        {
                            CommonUtils.AlertDialog(view, "", "納入先店舗が違います。", null);
                            return;
                        }

                        // 전송용DB에 해당 메일백에 대한 레코드 등록
                        InsertSndMailInfo(data);

                        // 메일백수 +1
                        int mail_bag_su = prefs.GetInt("mail_bag_su", 0) + 1;
                        editor.PutInt("mail_bag_su", mail_bag_su);
                        editor.Apply();

                        etMailBagNumber.Text = mail_bag_su.ToString();
                        
                        // 최대 메일백수에 도달하면 메일백 납품완료
                        if (mail_bag_su == mailbackCnt)
                        {
                            editor.PutBoolean("mailBagFlag", true);
                            // ("   =メールバッグ=   ")
                            // ("メールバッグの")
                            // ("納品が完了しました。")
                            editor.PutString("menu_flg", "1");
                            editor.Apply();
                            StartFragment(FragmentManager, typeof(NohinCompleteFragment));
                        }
                    }
                    else
                    {
                        CommonUtils.AlertDialog(view, "", "該当メールバッグはありません。", null);
                        return;
                    }
                    
                });
            }
        }
        
        private bool CheckMailBagData(string data)
        {
            List<SndNohinMail> sndNohinMail = sndNohinMailHelper.Select(tokuisakiCd, todokesakiCd, data);
            return sndNohinMail.Count != 0;
        }

        private void InsertSndMailInfo(string mailback)
        {
            // レコード作成用　値取得
            SndNohinMail sndNohinMail = new SndNohinMail
            {
                wPackage = "01",
                wTerminalID = "11101", //Handy: serialId
                wProgramID = prefs.GetString("program_id", "NOH"), //JOB: program_id
                wSagyosyaCD = prefs.GetString("sagyousya_cd", "99999"),
                wSoukoCD = prefs.GetString("souko_cd", ""), //JOB: noh_soukoCd
                wHaisoDate = prefs.GetString("haiso_date", ""), // noh_syukaDate
                wBinNo = prefs.GetString("bin_no", ""), //JOB: noh_binNo
                wCourse = prefs.GetString("course", ""),
                wDriverCD = prefs.GetString("driver_cd", ""),
                wTokuisakiCD = prefs.GetString("tokuisaki_cd", ""),
                wTodokesakiCD = prefs.GetString("todokesaki_cd", ""),
                wKanriNo = mailback,
                wVendorCd = prefs.GetString("vendor_cd", ""),
                wMateVendorCd = "",
                wSyukaDate = "0",
                wButsuryuNo = "",
                wKamotuNo = "",
                wMatehan = "",
                wMatehanSu = "",
                wHHT_no = "",
                wNohinKbn = "",
                wKaisyuKbn = "",
                wTenkanState = "00",
                wSakiTokuisakiCD = "",
                wSakiTodokesakiCD = "",
                wNohinDate = prefs.GetString("nohin_date", ""),
                wNohinTime = prefs.GetString("nohin_time", "")
            };
            sndNohinMailHelper.Insert(sndNohinMail);
        }
    }
}