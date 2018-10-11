using System.Collections.Generic;
using System.Threading;
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
    public class MatehanSelectFragment : BaseFragment
    {
        View view;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        EditText etKasidatuDate, etKasidatuTarget;
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

            BootstrapButton btnConfirm = view.FindViewById<BootstrapButton>(Resource.Id.btn_matehan_confirm);
            etKasidatuDate = view.FindViewById<BootstrapEditText>(Resource.Id.et_matehan_kasidatuDate);
            etKasidatuTarget = view.FindViewById<BootstrapEditText>(Resource.Id.et_matehan_kasidatuTarget);
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

            BootstrapButton btnSearch = view.FindViewById<BootstrapButton>(Resource.Id.btn_matehan_kasidasiSakiSearch);
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
                    try
                    {
                        string vendorNm = WebService.RequestMate010(etKasidatuTarget.Text);
                        if(vendorNm == "")
                        {
                            CommonUtils.AlertDialog(view, "", "貸出先コードが見つかりません", () => {
                                etKasidatuTarget.Text = "";
                                etKasidatuTarget.RequestFocus();
                            });
                        }
                        else
                        {
                            txtTargetName.Text = vendorNm;
                            txtConfirmMsg.Visibility = ViewStates.Visible;

                            etKasidatuDate.Focusable = false;
                            etKasidatuTarget.Focusable = false;

                        }

                    }
                    catch
                    {
                        CommonUtils.AlertDialog(view, "", "貸出先コードが見つかりません", () =>{
                            etKasidatuTarget.Text = "";
                            etKasidatuTarget.RequestFocus();
                        });
                    }
                }
                );
                Activity.RunOnUiThread(() => ((MainActivity)this.Activity).DismissDialog());
            }
            )).Start();

        }

        private void HideConfirmMessage()
        {
            if (txtConfirmMsg.Visibility == ViewStates.Visible)
            {
                txtTargetName.Visibility = ViewStates.Gone;
                txtConfirmMsg.Visibility = ViewStates.Gone;

                etKasidatuDate.Focusable = true;
                etKasidatuTarget.Focusable = true;
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
                editor.PutString("vendor_cd", etKasidatuTarget.Text);
                editor.PutString("vendor_nm", txtTargetName.Text);
                editor.PutString("kasidasi_date", "20" + etKasidatuDate.Text.Replace("/", ""));
                
                editor.Apply();

                StartFragment(FragmentManager, typeof(MatehanWorkFragment));
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
                                // ShowConfirmMessage();
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