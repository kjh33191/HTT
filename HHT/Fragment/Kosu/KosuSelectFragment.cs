using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;
using HHT.Resources.Model;
using Com.Beardedhen.Androidbootstrap;

namespace HHT
{
    public class KosuSelectFragment : BaseFragment
    {
        private readonly string TAG = "KosuSelectFragment";

        private View view;
        private TextView txtConfirm;
        private BootstrapEditText etSyukaDate, etTokuisaki, etTodokesaki, etVendorCode;
        private BootstrapButton btnVendorSearch, confirmButton;

        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        private int kosuMenuflag;
        private string soukoCd, kitakuCd, vendorNm;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_kosu_select, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();
            //ShowFooter();

            kosuMenuflag = prefs.GetInt(Const.KOSU_MENU_FLAG, (int)Const.KOSU_MENU.TODOKE); // 画面区分
            soukoCd = prefs.GetString("souko_cd", "");
            kitakuCd = prefs.GetString("kitaku_cd", "");

            
            editor.PutString("ko_su", "0");
            editor.PutString("dai_su", "0");
            editor.Apply();
            
            InitComponent();

            return view;
        }

        private void InitComponent()
        {
            txtConfirm = view.FindViewById<TextView>(Resource.Id.txt_kosuSelect_confirmMsg);
            etSyukaDate = view.FindViewById<BootstrapEditText>(Resource.Id.todoke_et_deliveryDate);

            //etSyukaDate.Text = "2018/03/20"; // テスト用
            etSyukaDate.Text = DateTime.Now.ToString("yyyy/MM/dd");

            etSyukaDate.FocusChange += (sender, e) => {
                if (e.HasFocus)
                {
                    etSyukaDate.Text = etSyukaDate.Text.Replace("/", "");
                    etSyukaDate.SetSelection(etSyukaDate.Text.Length);
                }
                else
                {
                    CheckDate();
                }
            };
            
            confirmButton = view.FindViewById<BootstrapButton>(Resource.Id.btn_todoke_confirm);
            confirmButton.Click += delegate { Confirm(); };
            
            if (kosuMenuflag == (int)Const.KOSU_MENU.TODOKE)
            {
                SetTitle("届先指定検品");
                SetFooterText("");

                GridLayout gl = view.FindViewById<GridLayout>(Resource.Id.gl_kosuSelect_Todoke);
                gl.Visibility = ViewStates.Visible;

                BootstrapButton searchButton = view.FindViewById<BootstrapButton>(Resource.Id.todokeSearch);
                searchButton.Click += delegate { SearchTodokesaki(); };
                searchButton.Enabled = false;

                etTokuisaki = view.FindViewById<BootstrapEditText>(Resource.Id.tokuiCode);
                etTodokesaki = view.FindViewById<BootstrapEditText>(Resource.Id.todokeCode);

                etTodokesaki.FocusChange += (sender, e) =>
                {
                    if (e.HasFocus)
                    {
                        SetFooterText("F1：検索");
                        searchButton.Enabled = true;
                    }
                    else
                    {
                        SetFooterText("");
                        searchButton.Enabled = false;
                    }
                };
                etTodokesaki.KeyPress += (sender, e) => {
                    if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                    {
                        e.Handled = true;
                        Confirm();
                    }
                    else
                    {
                        e.Handled = false;
                    }
                };

                //初期フォーカス
                if (etTokuisaki.Text == "")
                {
                    etTokuisaki.Text = prefs.GetString("def_tokuisaki_cd", "");
                    etTodokesaki.RequestFocus();
                }
            }
            else if (kosuMenuflag == (int)Const.KOSU_MENU.VENDOR)
            {
                SetTitle("ベンダー指定検品");
                SetFooterText("");

                GridLayout gl = view.FindViewById<GridLayout>(Resource.Id.gl_kosuSelect_Vendor);
                gl.Visibility = ViewStates.Visible;


                btnVendorSearch = view.FindViewById<BootstrapButton>(Resource.Id.btn_kosuSelect_vendorSearch);
                btnVendorSearch.Click += delegate { GoVendorSearchPage(); };
                btnVendorSearch.Visibility = ViewStates.Visible;
                btnVendorSearch.Enabled = false;

                etVendorCode = view.FindViewById<BootstrapEditText>(Resource.Id.vendorCode);
                etVendorCode.FocusChange += (sender, e) => {
                    if (e.HasFocus)
                    {
                        SetFooterText("F1：検索");
                        btnVendorSearch.Enabled = true;
                    }
                    else
                    {
                        SetFooterText("");
                        btnVendorSearch.Enabled = false;
                    }
                };

                etVendorCode.KeyPress += (sender, e) => {
                    if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                    {
                        e.Handled = true;
                        Confirm();
                    }
                    else
                    {
                        e.Handled = false;
                    }
                };
                
                //初期フォーカス
                etVendorCode.RequestFocus();
            }
            else if (kosuMenuflag == (int)Const.KOSU_MENU.BARA)
            {
                SetTitle("バラ検品");
                etSyukaDate.KeyPress += (sender, e) => {
                    if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                    {
                        e.Handled = true;
                        Confirm();
                    }
                    else
                    {
                        e.Handled = false;
                    }
                };
                etSyukaDate.SetSelection(etSyukaDate.Text.Length);
                etSyukaDate.RequestFocus();
            }
        }
    
        private void CheckTokuisaki()
        {
            ((MainActivity)this.Activity).ShowProgress("読み込み中...");

            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(() =>
                {
                    Thread.Sleep(1500);
                    int count = WebService.RequestKosu010(etTokuisaki.Text);
                    if (count == 0)
                    {
                        // 得意先コードがみつかりません。
                    }
                }
                );
                Activity.RunOnUiThread(() => ((MainActivity)this.Activity).DismissDialog());
            }
            )).Start();
        }

        private async Task<bool> IsExistTokuisaki()
        {
            bool isExist = false;
            try
            {
                int count = WebService.RequestKosu010(etTokuisaki.Text);
                if (count >= 1)
                {
                    isExist = true;
                }
            }
            catch
            {

            }
            
            return await Task.FromResult(isExist);
        }

        private async Task<bool> IsExistTodokesaki()
        {
            bool isExist = false;

            try
            {
                int count = WebService.RequestKosu020(etTokuisaki.Text, etTodokesaki.Text);
                if(count >= 1)
                {
                    isExist = true;
                }
                
            }
            catch(Exception e)
            {
                Log.Debug(TAG, e.StackTrace.ToString());
            }

            return await Task.FromResult(isExist);
        }

        private async Task<string> GetVendorState()
        {
            string venderState = "";
            
            try
            {
                string syukaDate = etSyukaDate.Text.Replace("/", "");
                KOSU131 kosu131 = WebService.RequestKosu131(soukoCd, kitakuCd, syukaDate, etVendorCode.Text);

                venderState = kosu131.state;
                vendorNm = kosu131.vendor_nm;
            }
            catch
            {

            }

            return await Task.FromResult(venderState);
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

                    if (etSyukaDate.HasFocus)
                    {
                        // todo?
                    }
                    else if (etTokuisaki.HasFocus)
                    {
                        etTokuisaki.Text = data;
                        etTodokesaki.RequestFocus();
                    }
                    else if (etTodokesaki.HasFocus)
                    {
                        etTodokesaki.Text = data;   
                        confirmButton.CallOnClick();
                    }
                });
            }
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Back)
            {
                if (txtConfirm.Visibility == ViewStates.Visible)
                {
                    txtConfirm.Visibility = ViewStates.Gone;
                    return false;
                }
            }
            else if (keycode == Keycode.F1)
            {
                if (kosuMenuflag == (int)Const.KOSU_MENU.TODOKE)
                {
                    if (etTodokesaki.IsFocused)
                    {
                        SearchTodokesaki();
                    }
                }
                else if (kosuMenuflag == (int)Const.KOSU_MENU.VENDOR)
                {
                    if (etVendorCode.IsFocused)
                    {
                        GoVendorSearchPage();
                    }
                }
            }
            else if (keycode == Keycode.F4)
            {
                Confirm();
            }

            return true;
        }

        private void Confirm()
        {
            ((MainActivity)this.Activity).ShowProgress("指定先を確認しています。");

            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(async () =>
                    {
                        if (await CheckValidation())
                        {
                            if (kosuMenuflag == (int)Const.KOSU_MENU.TODOKE)
                            {
                                editor.PutString("syuka_date", etSyukaDate.Text.Replace("/", ""));
                                editor.PutString("deliveryDate", etSyukaDate.Text);
                                editor.PutString("tokuisaki_cd", etTokuisaki.Text);
                                editor.PutString("todokesaki_cd", etTodokesaki.Text);
                                editor.PutInt("menuKbn", 1); // 届先検索フラグ設定
                                editor.Apply();

                                StartFragment(FragmentManager, typeof(KosuBinInputFragment));
                            }
                            else if (kosuMenuflag == (int)Const.KOSU_MENU.VENDOR)
                            {
                                var message =
                                        @"配送日：" + etSyukaDate.Text
                                    + "\nベンダー：" + etVendorCode.Text
                                    + "\n" + vendorNm
                                    + "\n\n" + "よろしいですか？";

                                ShowDialog("確認", message, () =>
                                {
                                    editor.PutString("vendor_cd", etVendorCode.Text);
                                    editor.PutString("vendor_nm", vendorNm);
                                    editor.PutString("syuka_date", etSyukaDate.Text.Replace("/", ""));
                                    editor.PutString("tokuisaki_nm", "");
                                    editor.Apply();
                                    StartFragment(FragmentManager, typeof(KosuWorkFragment));
                                });
                            }
                            else if (kosuMenuflag == (int)Const.KOSU_MENU.BARA)
                            {
                                editor.PutString("tokuisaki_nm", "");
                                editor.PutString("vendor_cd", "");
                                editor.PutString("vendor_nm", "");
                                editor.PutString("syuka_date", etSyukaDate.Text.Replace("/", ""));
                                editor.Apply();
                                StartFragment(FragmentManager, typeof(KosuWorkFragment));
                            }
                        }
                    }
                );
                Activity.RunOnUiThread(() => ((MainActivity)this.Activity).DismissDialog());
               }
            )).Start();
        }

        private async Task<bool> CheckValidation()
        {
            if (kosuMenuflag == (int)Const.KOSU_MENU.TODOKE)
            {
                if (etSyukaDate.Text == "")
                {
                    ShowDialog("エラー", "配送日を確認してください。", () =>
                    {
                        etSyukaDate.RequestFocus();
                    });
                    return false;
                }

                if (etTokuisaki.Text == "")
                {
                    ShowDialog("エラー", "得意先コードを入力してください。", () =>
                    {
                        etTokuisaki.RequestFocus();
                    });
                    return false;
                }

                if (etTodokesaki.Text == "")
                {
                    ShowDialog("エラー", "届先コードを入力してください。", () =>
                    {
                        etTodokesaki.RequestFocus();
                    });
                    return false;
                }

                
                if (!await IsExistTokuisaki())
                {
                    ShowDialog("エラー", "得意先コードがみつかりません。", () =>
                    {
                        etTokuisaki.Text = "";
                        etTodokesaki.Text = "";
                        etTokuisaki.RequestFocus();
                    });
                    return false;
                }

                
                if (!await IsExistTodokesaki())
                {
                    ShowDialog("エラー", "得意先コードがみつかりません。", () =>
                    {
                        etTodokesaki.Text = "";
                        etTodokesaki.RequestFocus();
                    });
                    return false;
                }
            }
            else if (kosuMenuflag == (int)Const.KOSU_MENU.VENDOR)
            {
                // 必須チェック
                if (etSyukaDate.Text == "")
                {
                    ShowDialog("エラー", "配送日を確認してください。", () =>
                    {
                        etSyukaDate.RequestFocus();
                    });
                    return false;
                }

                if (etVendorCode.Text == "")
                {
                    ShowDialog("エラー", "ベンダーコードを入力してください。", () =>
                    {
                        etVendorCode.RequestFocus();
                    });
                    return false;
                }

                // ベンダー情報検索
                string syukaDate = etSyukaDate.Text;
                string errTitle = "";
                string errMsg = "";

                string resultState = await GetVendorState();

                if (vendorNm == "")
                {
                    errTitle = "エラー";
                    errMsg = "ベンダーコードがみつかりません。";
                }
                else if (!(resultState == "00" || resultState == "01" || resultState == "99"))
                {
                    errTitle = "報告";
                    errMsg = "全ての検品が完了しています。";
                }
                else if (resultState != "00")
                {
                    errTitle = "エラー";
                    errMsg = "検品可能なベンダーではありません。";
                }

                if (errMsg.Length > 0)
                {
                    ShowDialog(errTitle, errMsg, () =>
                    {
                        etVendorCode.Text = "";
                        etVendorCode.RequestFocus();
                    });
                    return false;
                }
            }
            else if (kosuMenuflag == (int)Const.KOSU_MENU.BARA)
            {
                if (etSyukaDate.Text == "")
                {
                    ShowDialog("エラー", "配送日を確認してください。", () =>
                    {
                        etSyukaDate.RequestFocus();
                    });
                    return false;
                }
            }

            return true;
        }

        private void SearchTodokesaki()
        {
            if (etSyukaDate.Text == "")
            {
                ShowDialog("エラー", "配送日を入力してください。", () =>
                {
                    etSyukaDate.RequestFocus();
                });
                return;
            }

            if (etTokuisaki.Text == "")
            {
                ShowDialog("エラー", "得意先コードを入力してください。", () =>
                {
                    etTokuisaki.RequestFocus();
                });
                return;
            }
            
            editor.PutString("deliveryDate", etSyukaDate.Text);
            editor.PutString("tokuisaki", etTokuisaki.Text);
            editor.PutString("deliveryDate", etSyukaDate.Text);
            
            editor.PutInt("menuKbn", 2); // 届先検索フラグ設定
            editor.Apply();

            StartFragment(FragmentManager, typeof(KosuBinInputFragment));
        }

        private void CheckDate()
        {
            try
            {
                etSyukaDate.Text = CommonUtils.GetDateYYYYMMDDwithSlash(etSyukaDate.Text);
            }
            catch
            {
                ShowDialog("エラー", "正しい日付を入力してください。", () =>
                {
                    etSyukaDate.Text = "";
                    etSyukaDate.RequestFocus();
                });
            }
        }

        private void GoVendorSearchPage()
        {
            string syukaDate = etSyukaDate.Text;

            if (etSyukaDate.Text == "")
            {
                ShowDialog("エラー", "配送日を確認してください。", () =>
                {
                    etSyukaDate.RequestFocus();
                });
                return;
            }
            
            editor.PutString("syuka_date", syukaDate.Replace("/", ""));
            editor.Apply();

            StartFragment(FragmentManager, typeof(KosuVendorSearchFragment));
        }
    }
}