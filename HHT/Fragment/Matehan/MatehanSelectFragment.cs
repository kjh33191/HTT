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
    public class MatehanSelectFragment : BaseFragment
    {
        View view;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        EditText etKasidatuDate, etKasidatuTarget, etBinNo;
        TextView txtConfirmMsg, txtTargetName;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_matehan_lending, container, false);

            // コンポーネント初期化
            InitComponent();

            return view;
        }

        // コンポーネント初期化
        private void InitComponent()
        {
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetTitle("貸出登録");

            Button btnConfirm = view.FindViewById<Button>(Resource.Id.btn_matehan_confirm);
            etKasidatuDate = view.FindViewById<EditText>(Resource.Id.et_matehan_kasidatuDate);
            etKasidatuTarget = view.FindViewById<EditText>(Resource.Id.et_matehan_kasidatuTarget);
            etKasidatuTarget.FocusChange += (sender, e) => { if (!e.HasFocus && etKasidatuTarget.Text != "") SearchBinNo(); };
            
            txtTargetName = view.FindViewById<TextView>(Resource.Id.tv_matehan_targetName);
            txtConfirmMsg = view.FindViewById<TextView>(Resource.Id.tv_matehan_confirmMsg);

            btnConfirm.Click += delegate { Confirm(); };
            etKasidatuDate.FocusChange += (sender, e) => {
                if (e.HasFocus)
                {
                    etKasidatuDate.Text = etKasidatuDate.Text.Replace("/", "");
                }
                else
                {
                    try
                    {
                        etKasidatuDate.Text = CommonUtils.GetDateYYMMDDwithSlash(etKasidatuDate.Text);
                    }
                    catch
                    {
                        CommonUtils.ShowAlertDialog(view, "日付形式ではありません", "正しい日付を入力してください");
                        etKasidatuDate.Text = "";
                        etKasidatuDate.RequestFocus();
                    }
                }
            };

            Button btnSearch = view.FindViewById<Button>(Resource.Id.btn_matehan_kasidasiSakiSearch);
            btnSearch.Click += delegate { SearchKasidasiSaki(); };

            etKasidatuDate.Text = "18/03/20";
            etKasidatuTarget.RequestFocus();

        }

        private void SearchKasidasiSaki()
        {
            /*
            if (etSyukaDate.Text == "")
            {
                CommonUtils.ShowAlertDialog(view, "エラー", "配送日を入力してください。");
                etSyukaDate.RequestFocus();
                return;
            }

            */


            //editor.PutString("deliveryDate", etSyukaDate.Text);

            //editor.PutInt("menuKbn", 2); // 届先検索フラグ設定
            //editor.Apply();

            StartFragment(FragmentManager, typeof(MatehanSearchFragment));
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

        private void ShowConfirmMessage()
        {
            if (txtConfirmMsg.Visibility != ViewStates.Visible)
            {
                txtTargetName.Visibility = ViewStates.Visible;
                txtConfirmMsg.Visibility = ViewStates.Visible;
                txtTargetName.Text = "???????";

                etKasidatuDate.Focusable = false;
                etKasidatuTarget.Focusable = false;
                etBinNo.Focusable = false;
            }
        }

        private void HideConfirmMessage()
        {
            if (txtConfirmMsg.Visibility == ViewStates.Visible)
            {
                txtTargetName.Visibility = ViewStates.Gone;
                txtConfirmMsg.Visibility = ViewStates.Gone;

                etKasidatuDate.Focusable = true;
                etKasidatuTarget.Focusable = true;
                etBinNo.Focusable = true;
            }
        }


        private void Confirm()
        {
            if (txtConfirmMsg.Visibility != ViewStates.Visible)
            {
                SearchBinNo();
            }
            else
            {
                
                StartFragment(FragmentManager, typeof(TsumikomiSearchFragment));
            }
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {

            if (keycode == Keycode.F4)
            {
                Confirm();
                return false;
            }
            else if (keycode == Keycode.Back)
            {
                if (txtConfirmMsg.Visibility == ViewStates.Visible)
                {
                    HideConfirmMessage();
                    return false;
                }
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

                    if (etKasidatuTarget.HasFocus)
                    {
                        if (data.Length < 12)
                        {
                            CommonUtils.AlertDialog(View, "エラー", "コースNoがみつかりません。", () => { return; });
                        }
                        else
                        {
                            
                            try
                            {
                                ShowConfirmMessage();
                            }
                            catch
                            {
                                CommonUtils.AlertDialog(View, "エラー", "コースNoがみつかりません。", () => { return; });
                            }

                        }
                    }
                    
                });
            }
        }
    }
}