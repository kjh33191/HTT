using System;
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
            etKasidatuTarget.KeyPress += (sender, e) => {
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    e.Handled = true;
                    CommonUtils.HideKeyboard(Activity);

                    if(etKasidatuTarget.Text != "") SearchBinNo();
                }
                else
                {
                    e.Handled = false;
                }
            };

            btnConfirm.Click += delegate { SearchBinNo(); };
            etKasidatuDate.FocusChange += (sender, e) => {
                if (e.HasFocus)
                {
                    etKasidatuDate.Text = etKasidatuDate.Text.Replace("/", "");
                    etKasidatuDate.SetSelection(etKasidatuDate.Text.Length);
                }
                else
                {
                    try
                    {
                        etKasidatuDate.Text = CommonUtils.GetDateYYYYMMDDwithSlash(etKasidatuDate.Text);
                    }
                    catch
                    {
                        ShowDialog("エラー", "正しい日付を入力してください", ()=>{
                            etKasidatuDate.Text = "";
                            etKasidatuDate.RequestFocus();
                        });
                    }
                }
            };

            BootstrapButton btnSearch = view.FindViewById<BootstrapButton>(Resource.Id.btn_matehan_kasidasiSakiSearch);
            btnSearch.Click += delegate { SearchKasidasiSaki(); };

            //etKasidatuDate.Text = "18/03/20";
            etKasidatuDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
            etKasidatuTarget.RequestFocus();

        }

        private void SearchKasidasiSaki()
        {
            
            if (etKasidatuDate.Text == "")
            {
                ShowDialog("エラー", "貸出日を入力してください。", () => { });
                etKasidatuDate.RequestFocus();
                return;
            }
            
            editor.PutString("kasidasi_date", etKasidatuDate.Text.Replace("/", ""));
            editor.Apply();
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
                            ShowDialog("報告", "貸出先コードが見つかりません。", () => {
                                etKasidatuTarget.Text = "";
                                etKasidatuTarget.RequestFocus();
                            });
                        }
                        else
                        {
                            string message = "貸出日：" + etKasidatuDate.Text;
                            message += "\n" + "貸出先：" + etKasidatuTarget.Text;
                            message += "\n" + "貸出名：\n" + vendorNm;
                            message += "\n\n" + "よろしいですか？";

                            ShowDialog("確認", message, () => {
                                editor.PutString("vendor_cd", etKasidatuTarget.Text);
                                editor.PutString("vendor_nm", vendorNm);
                                editor.PutString("kasidasi_date", etKasidatuDate.Text.Replace("/", ""));

                                editor.Apply();

                                StartFragment(FragmentManager, typeof(MatehanWorkFragment));

                            });
                        }

                    }
                    catch
                    {
                        ShowDialog("エラー", "貸出先コードが見つかりません。", () => {
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

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {

            if (keycode == Keycode.F4)
            {
                SearchBinNo();
                return false;
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
                            ShowDialog("エラー", "コースNoがみつかりません。", () => {});
                        }
                        else
                        {
                            
                            try
                            {
                                // ShowConfirmMessage();
                            }
                            catch
                            {
                                ShowDialog("エラー", "コースNoがみつかりません。", () => { });
                            }

                        }
                    }
                    
                });
            }
        }
    }
}