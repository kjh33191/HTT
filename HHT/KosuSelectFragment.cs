using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;
using HHT.Resources.DataHelper;
using HHT.Resources.Model;
using Newtonsoft.Json;

namespace HHT
{
    public class KosuSelectFragment : BaseFragment
    {
        private readonly string TAG = "KosuSelectFragment";
        private View view;

        private int kosuMenuflag;
        private string soukoCd, kitakuCd, vendorNm;

        private TextView txtConfirm;
        private EditText etSyukaDate, etTokuisaki, etTodokesaki, etVendorCode;
        private Button btnVendorSearch, confirmButton;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_kosu_select, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            kosuMenuflag = prefs.GetInt(Const.KOSU_MENU_FLAG, (int)Const.KOSU_MENU.TODOKE); // 画面区分
            soukoCd = prefs.GetString("soukoCd", "");
            kitakuCd = prefs.GetString("kitakuCd", "");

            InitComponent();
            
            return view;
        }

        private void InitComponent()
        {
            if (kosuMenuflag == (int)Const.KOSU_MENU.TODOKE)
            {
                SetTitle("届先指定検品");
                SetFooterText("F4：確定");

                GridLayout gl = view.FindViewById<GridLayout>(Resource.Id.gl_kosuSelect_Todoke);
                gl.Visibility = ViewStates.Visible;

                Button searchButton = view.FindViewById<Button>(Resource.Id.btn_search_todoke);
                searchButton.Click += delegate { SearchTodokesaki(); };
                searchButton.Visibility = ViewStates.Gone;

                etTokuisaki = view.FindViewById<EditText>(Resource.Id.et_todoke_tokuisaki);
                etTodokesaki = view.FindViewById<EditText>(Resource.Id.et_todoke_todokesaki);

                etTodokesaki.FocusChange += (sender, e) =>
                {
                    if (e.HasFocus)
                    {
                        SetFooterText("F1：届先検索　　　F4：確定");
                        searchButton.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        SetFooterText("F4：確定");
                        searchButton.Visibility = ViewStates.Gone;
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
                SetFooterText("F4：確定");
                
                GridLayout gl = view.FindViewById<GridLayout>(Resource.Id.gl_kosuSelect_Vendor);
                gl.Visibility = ViewStates.Visible;

                etVendorCode = view.FindViewById<EditText>(Resource.Id.et_kosuSelect_vendorCode);

                btnVendorSearch = view.FindViewById<Button>(Resource.Id.btn_kosuSelect_vendorSearch);
                btnVendorSearch.Click += delegate { GoVendorSearchPage(); };
                btnVendorSearch.Visibility = ViewStates.Visible;

                //初期フォーカス
                etVendorCode.RequestFocus();
            }

            txtConfirm = view.FindViewById<TextView>(Resource.Id.txt_kosuSelect_confirmMsg);

            etSyukaDate = view.FindViewById<EditText>(Resource.Id.todoke_et_deliveryDate);

            //etSyukaDate.Text = DateTime.Now.ToString("yy/MM/dd");
            etSyukaDate.Text = "18/03/20"; // テスト用

            etSyukaDate.FocusChange += (sender, e) => {
                if (e.HasFocus)
                {
                    etSyukaDate.Text = etSyukaDate.Text.Replace("/", "");
                }
                else
                {
                    CheckDate();
                }

            };

            confirmButton = view.FindViewById<Button>(Resource.Id.btn_todoke_confirm);
            confirmButton.Click += delegate { Confirm(); };
        }
    
        private void CheckTokuisaki()
        {
            var progress = ProgressDialog.Show(this.Activity, null, "得意先を確認しています。", true);

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
                string syukaDate = "20" + etSyukaDate.Text.Replace("/", "");
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
                if (etTodokesaki.IsFocused)
                {
                    SearchTodokesaki();
                }

                if (kosuMenuflag == (int)Const.KOSU_MENU.VENDOR)
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
            var progress = ProgressDialog.Show(this.Activity, null, "指定先を確認しています。", true);

            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(async () =>
                    {
                        if (await CheckValidation())
                        {
                            if (kosuMenuflag == (int)Const.KOSU_MENU.TODOKE)
                            {
                                editor.PutString("deliveryDate", etSyukaDate.Text);
                                editor.PutString("tokuisaki_cd", etTokuisaki.Text);
                                editor.PutString("tokuisaki_nm", etTokuisaki.Text);
                                editor.PutString("todokesaki_cd", etTodokesaki.Text);
                                editor.PutInt("menuKbn", 1); // 届先検索フラグ設定
                                editor.Apply();

                                StartFragment(FragmentManager, typeof(KosuBinInputFragment));
                            }
                            else if (kosuMenuflag == (int)Const.KOSU_MENU.VENDOR)
                            {
                                if (txtConfirm.Visibility == ViewStates.Gone)
                                {
                                    txtConfirm.Visibility = ViewStates.Visible;
                                }
                                else
                                {
                                    editor.PutString("vendor_cd", etVendorCode.Text);
                                    editor.PutString("vendor_nm", vendorNm);
                                    editor.PutString("tokuisaki_nm", vendorNm);

                                    editor.PutString("deliveryDate", etSyukaDate.Text);
                                    editor.Apply();

                                    StartFragment(FragmentManager, typeof(TodokeTyingWorkFragment));
                                }
                            }
                            else if (kosuMenuflag == (int)Const.KOSU_MENU.BARA)
                            {
                                editor.PutString("tokuisaki_nm", "");
                                StartFragment(FragmentManager, typeof(TodokeTyingWorkFragment));
                            }
                        }
                    }
                );
                Activity.RunOnUiThread(() => progress.Dismiss());
               }
            )).Start();
        }

        private async Task<bool> CheckValidation()
        {
            if (kosuMenuflag == (int)Const.KOSU_MENU.TODOKE)
            {
                if (etSyukaDate.Text == "")
                {
                    CommonUtils.ShowAlertDialog(view, "エラー", "配送日を入力してください。");
                    etSyukaDate.RequestFocus();
                    return false;
                }

                if (etTokuisaki.Text == "")
                {
                    CommonUtils.ShowAlertDialog(view, "エラー", "得意先コードを入力してください。");
                    etTokuisaki.RequestFocus();
                    return false;
                }

                if (etTodokesaki.Text == "")
                {
                    CommonUtils.ShowAlertDialog(view, "エラー", "届先コードを入力してください。");
                    etTodokesaki.RequestFocus();
                    return false;
                }

                
                if (!await IsExistTokuisaki())
                {
                    CommonUtils.ShowAlertDialog(view, "エラー", "得意先コードがみつかりません。");
                    etTokuisaki.Text = "";
                    etTodokesaki.Text = "";
                    etTokuisaki.RequestFocus();
                    return false;
                }

                
                if (!await IsExistTodokesaki())
                {
                    CommonUtils.ShowAlertDialog(view, "エラー", "得意先コードがみつかりません。");
                    etTodokesaki.Text = "";
                    etTodokesaki.RequestFocus();
                    return false;
                }
            }
            else if (kosuMenuflag == (int)Const.KOSU_MENU.VENDOR)
            {
                // 必須チェック
                if (etSyukaDate.Text == "")
                {
                    CommonUtils.ShowAlertDialog(view, "エラー", "配送日を入力してください。");
                    etSyukaDate.RequestFocus();
                    return false;
                }

                if (etVendorCode.Text == "")
                {
                    CommonUtils.ShowAlertDialog(view, "エラー", "ベンダーコードを入力してください。");
                    etVendorCode.RequestFocus();
                    return false;
                }

                // ベンダー情報検索
                string syukaDate = etSyukaDate.Text;
                string errTitle = "";
                string errMsg = "";

                string resultState = await GetVendorState();

                if (resultState == null)
                {
                    errTitle = "Error";
                    errMsg = "ベンダーコードがみつかりません。";
                }
                else if (!(resultState == "00" || resultState == "01" || resultState == "99"))
                {
                    errTitle = "確認";
                    errMsg = "全ての検品が完了しています。";
                }
                else if (resultState != "00")
                {
                    errTitle = "確認";
                    errMsg = "検品可能なベンダーではありません。";
                }

                if (errMsg.Length > 0)
                {
                    CommonUtils.AlertDialog(view, errTitle, errMsg, () => {
                        etVendorCode.Text = "";
                        etVendorCode.RequestFocus();
                        return;
                    });
                    return false;
                }
                
            }
            else if (kosuMenuflag == (int)Const.KOSU_MENU.BARA)
            {
                if (etSyukaDate.Text == "")
                {
                    CommonUtils.ShowAlertDialog(view, "エラー", "配送日を入力してください。");
                    etSyukaDate.RequestFocus();
                    return false;
                }
            }

            return true;
        }

        private void SearchTodokesaki()
        {

            if (etSyukaDate.Text == "")
            {
                CommonUtils.ShowAlertDialog(view, "エラー", "配送日を入力してください。");
                etSyukaDate.RequestFocus();
                return;
            }

            if (etTokuisaki.Text == "")
            {
                CommonUtils.ShowAlertDialog(view, "エラー", "得意先コードを入力してください。");
                etTokuisaki.RequestFocus();
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
                etSyukaDate.Text = CommonUtils.GetDateYYMMDDwithSlash(etSyukaDate.Text);
            }
            catch
            {
                CommonUtils.ShowAlertDialog(view, "日付形式ではありません", "正しい日付を入力してください");
                etSyukaDate.Text = "";
                etSyukaDate.RequestFocus();
            }
        }

        private void GoVendorSearchPage()
        {
            if (etSyukaDate.Text == "")
            {
                CommonUtils.ShowAlertDialog(view, "エラー", "配送日を入力してください。");
                etSyukaDate.RequestFocus();
                return;
            }

            // TODO ベンダー検索画面
            StartFragment(FragmentManager, typeof(KosuBinInputFragment));
        }
    }
}