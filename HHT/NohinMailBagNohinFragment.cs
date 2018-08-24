using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;
using System.Collections.Generic;

namespace HHT
{
    public class NohinMailBagNohinFragment : BaseFragment
    {
        private View view;
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        EditText etMailBagNumber;
        private string mail_bag_su;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_nohin_mailBag_nohin, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetTitle("メールバック納品");
            SetFooterText(" F1:解除");

            TextView txtTokusakiName = view.FindViewById<TextView>(Resource.Id.txt_nohinMailBagNohin_tokuisakiNm);
            txtTokusakiName.Text = prefs.GetString("souko_nm", "");

            etMailBagNumber = view.FindViewById<EditText>(Resource.Id.et_nohinMailBagNohin_mailbagNumber);
            etMailBagNumber.Text = "0";
            etMailBagNumber.RequestFocus();

            Button kaizoButton = view.FindViewById<Button>(Resource.Id.btn_nohinMailBagNohin_kaizo);
            kaizoButton.Click += delegate { StartFragment(FragmentManager, typeof(NohinMailBagPasswordFragment)); };

            mail_bag_su = "3";

            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {

            if (keycode == Keycode.F1)
            {
                StartFragment(FragmentManager, typeof(NohinMailBagPasswordFragment));
            }
            if (keycode == Keycode.F3)
            {
                CommonUtils.AlertConfirm(view, "", "メールバッグ納品業務を終了しますか？", (flag) => {
                    if (flag)
                    {
                        // SndMBN_+ 시리얼 +  .txt의 Empty File을 만든다. 
                        // 이미 존재한다면 패스
                        FragmentManager.PopBackStack();
                    }
                    else
                    {

                    }
                });

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
                    string densoSymbology = barcodeData.SymbologyDenso;
                    string data = barcodeData.Data;
                    int barcodeDataLength = data.Length;

                    //data = 
                    //string mailBag = GetMailBag();
                    // 바코드로 읽어온 값이랑 동일한 mailbag 데이터를 찾는다 그래서 존재할시, 
                    data = "J00000374";
                    if (IsValidMailBagData(data))
                    {
                        // 중복된 놈인지 체크 // メールバッグ重複チェック
                        // "既にスキャン済みです。"


                        /*
                    Lenkey1 = mail_bag.Length
                    btvTokuisaki = mail_bag.Mid(1, 4)
                    btvTodokesaki = mail_bag.Mid(5, 4)
                    btvKey1 = btvTokuisaki & btvTodokesaki
                    btvKey2 = JOB:tokuisaki_cd & JOB:todokesaki_cd
                    
                        // JOB:mail_bag_su = JOB:mail_bag_su + 1
                        // appendFile(mail_bag,0)

                        */

                        etMailBagNumber.Text = (int.Parse(etMailBagNumber.Text) + 1).ToString();
                        // appendFile(mail_bag,0) DB 업뎃　<--- 송신용 파일 작성

                        if (mail_bag_su == etMailBagNumber.Text)
                        {
                            // DB 삭제후 
                            StartFragment(FragmentManager, typeof(NohinCompleteFragment));
                        }
                    }

                    // mb_ + serialId + .txt 파일이 있어야 함.
                    // 만약 사이즈가 0이라면 2를 반환

                    // mbcont = :GetCount(false)
                    // If mbcont <= 0 Then	// レコード無し del_File(nil,mb_file,nil) 파일 삭제 그리고 2 반환
                    // 아니면 mail_bag  = :GetData(4) 그리고 파일 삭제, 0 리턴

                });
            }
        }

        private bool IsValidMailBagData(string data)
        {

            return true;
        }

        private void appendFile()
        {

            // SndMBN_ file 

            /*
            // wPackage = "01"
            wTerminalID = FIX:setFixLength(10, Handy: serialId)
        wProgramID = FIX:setFixLength(10, JOB: program_id)
        wSagyosyaCD = FIX:setFixLength(13, JOB: sagyousya_cd)
        wSoukoCD = FIX:setFixLength(10, JOB: souko_cd)
        wHaisoDate = FIX:setFixLength(8, JOB: haiso_date)
        wBinNo = FIX:setFixLength(10, JOB: bin_no)
        wCourse = FIX:setFixLength(10, JOB: course)
        wDriverCD = FIX:setFixLength(13, JOB: driver_cd)
        wTokuisakiCD = FIX:setFixLength(13, JOB: tokuisaki_cd)
        wTodokesakiCD = FIX:setFixLength(13, JOB: todokesaki_cd)
        wKanriNo = FIX:setFixLength(20, mailback)
        wVendorCd = FIX:setFixLength(13, JOB: vendor_cd)
        wMateVendorCd = FIX:setFixLength(13, "")
        wSyukaDate = FIX:setFixLength(8, "0")
        wButsuryuNo = FIX:setFixLength(10, "")
        wKamotuNo = FIX:setFixLength(28, "")
        wMatehan = FIX:setFixLength(50, "")
        wMatehanSu = FIX:setFixLength(5, "0")
        wHHT_no = FIX:setFixLength(15, "")
        wNohinKbn = FIX:setFixLength(1, "")
        wKaisyuKbn = FIX:setFixLength(1, "")
        wTenkanState = FIX:setFixLength(2, "00")
        wSakiTokuisakiCD = FIX:setFixLength(13, "")
        wSakiTodokesakiCD = FIX:setFixLength(13, "")
        wNohinDate = FIX:setFixLength(8, JOB: nohin_date)
        wNohinTime = FIX:setFixLength(4, JOB: nohin_time)
        
             
             
             */
        }

    }
}