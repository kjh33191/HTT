using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using Com.Beardedhen.Androidbootstrap;
using Com.Densowave.Bhtsdk.Barcode;

namespace HHT
{
    public class IdouNohinWorkFragment : BaseFragment
    {
        View view;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_idou_nohin_work, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            // コンポーネント初期化
            InitComponent();
            
            // SndIDO_ 파일이 있어야 이 화면은 처리를 진행할수 있음
            

            // SNDIDOU파일을 열어서 0건 이상이라면 
            // 해당 행의 4번째 인덱스값이 00이라면 
            // 店間移動荷卸し区分 = "01", tokuisaki_cd, todokesaki_cd를 설정
            // JOB:task_cnt가 0이라면 JOB:kamotu_no에 temp를 설정
            // task_cnt를 1증가

            // 00이 아니라면 "該当の移動ラベルは登録済です"

            // SNDIDOU파일을 열어서 0건이라면
            // "移動ラベルがみつかりません"


            return view;
        }

        // コンポーネント初期化
        private void InitComponent()
        {
            BootstrapButton completeButton = view.FindViewById<BootstrapButton>(Resource.Id.btn_confirm);
            completeButton.Click += delegate { Confirm(); };

            BootstrapEditText etKaisyuLabel = view.FindViewById<BootstrapEditText>(Resource.Id.et_kaisyuLabel);

            TextView task_cnt = view.FindViewById<TextView>(Resource.Id.kaisyuLabelSu);

            etKaisyuLabel.RequestFocus();
        }

        private void Confirm()
        {
            ShowDialog("エラー", "移動ラベルがみつかりません。", () => { });
            return;
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