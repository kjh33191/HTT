using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using Com.Beardedhen.Androidbootstrap;
using Com.Densowave.Bhtsdk.Barcode;
using HHT.Resources.DataHelper;
using HHT.Resources.Model;

namespace HHT
{
    public class IdouNohinSelectFragment : BaseFragment
    {
        private readonly string TAG = "IdouNohinSelectFragment";
        View view;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        BootstrapEditText etTokuisakiCd, etTodokesakiCd;
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

            // 初期フォーカス
            etTokuisakiCd.RequestFocus();

            return view;
        }

        // コンポーネント初期化
        private void InitComponent()
        {
            etTokuisakiCd = view.FindViewById<BootstrapEditText>(Resource.Id.et_tokuisakiCd);
            etTodokesakiCd = view.FindViewById<BootstrapEditText>(Resource.Id.et_todokesakiCd);
            etTodokesakiCd.KeyPress += (sender, e) => {
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    e.Handled = true;
                    CommonUtils.HideKeyboard(Activity);
                    Confirm();
                }
                else
                {
                    e.Handled = false;
                }
            };
            txtTodokesakiNm = view.FindViewById<TextView>(Resource.Id.txt_todokesakiName);
            BootstrapButton btnConfirm = view.FindViewById<BootstrapButton>(Resource.Id.btn_confirm);
            btnConfirm.Click += delegate { Confirm(); };
        }

        private void Confirm()
        {
            if(etTokuisakiCd.Text == "")
            {
                CommonUtils.AlertDialog(view, "", "得意先コードを入力してください。", null);
                Vibrate();
                etTokuisakiCd.RequestFocus();
                return;
            }

            if (etTodokesakiCd.Text == "")
            {
                CommonUtils.AlertDialog(view, "", "届先コードを入力してください。", null);
                etTodokesakiCd.RequestFocus();
                Vibrate();
                return;
            }


            TokuiFileHelper tokuiFileHelper = new TokuiFileHelper();
            TokuiFile result = tokuiFileHelper.SelectByPk(etTokuisakiCd.Text, etTodokesakiCd.Text);

            if (result != null)
            {
                // よろしいですか？ 表示
                string confirmMsg = @"
移動先得意先 : @temp1
移動先届先 : @temp2
@temp3

よろしいですか？
                                        ";

                confirmMsg = confirmMsg.Replace("@temp1", result.tokuisaki_cd);
                confirmMsg = confirmMsg.Replace("@temp2", result.todokesaki_cd);
                confirmMsg = confirmMsg.Replace("@temp3", result.tokuisaki_nm);

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
                etTokuisakiCd.Text = "";
                etTodokesakiCd.Text = "";
                etTokuisakiCd.RequestFocus();
            }
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