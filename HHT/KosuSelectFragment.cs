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
        private View view;

        private int kosuMenuflag;
        private string soukoCd, kitakuCd, vendorNm;

        private TextView txtConfirm;
        private EditText etSyukaDate, etTokuisaki, etTodokesaki, etVendorCode;
        private Button btnVendorSearch, confirmButton;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        TokuisakiHelper tokuisakiHelper;

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

            // GetTokuisakiMasterInfo();
            
            // ret = get_master()	// マスタ検索
            // ret = 2 then error todoke=> sagyou4 , vendor=> sagyou9

            return view;
        }

        private void GetTokuisakiMasterInfo()
        {
            // Handy:serialId.MTP
            var progress = ProgressDialog.Show(this.Activity, null, "得意先を確認しています。", true);

            new Thread(new ThreadStart(delegate
            {
                Activity.RunOnUiThread(() =>
                {
                    Thread.Sleep(5000);
                    Dictionary<string, string> param = new Dictionary<string, string>
                    {
                        { "souko_cd",  prefs.GetString("tokuisaki_cd", "103")},
                        { "kitaku_cd",  prefs.GetString("tokuisaki_cd", "103")},
                        { "syuka_date",  prefs.GetString("tokuisaki_cd", "103")},
                        { "bin_no",  prefs.GetString("tokuisaki_cd", "103")}
                    };

                    string resultJson = "";
                    if (kosuMenuflag == (int)Const.KOSU_MENU.TODOKE)
                    {
                        //resultJson = await CommonUtils.PostAsync(WebService.KOSU.KOSU060, param);
                    }
                    else if (kosuMenuflag == (int)Const.KOSU_MENU.VENDOR)
                    {
                        //string resultJson = await CommonUtils.PostAsync(WebService.KOSU.KOSU065, param);
                    }

                    //List<Tokuisaki> result = JsonConvert.DeserializeObject<List<Tokuisaki>>(resultJson);
                    List<Tokuisaki> resultList = new List<Tokuisaki>();

                    tokuisakiHelper = new TokuisakiHelper();

                    foreach (Tokuisaki tokuisaki in resultList)
                    {
                        tokuisakiHelper.InsertIntoTableTokuisakiInfo(tokuisaki);
                    }

                });
            }));
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
                etTokuisaki.RequestFocus();

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
                btnVendorSearch.Visibility = ViewStates.Gone;

                //初期フォーカス
                etVendorCode.RequestFocus();
            }

            txtConfirm = view.FindViewById<TextView>(Resource.Id.txt_kosuSelect_confirmMsg);

            etSyukaDate = view.FindViewById<EditText>(Resource.Id.todoke_et_deliveryDate);
            etSyukaDate.Text = DateTime.Now.ToString("yy/MM/dd");
            etSyukaDate.FocusChange += (sender, e) => { if (!e.HasFocus) CheckDate(); };

            confirmButton = view.FindViewById<Button>(Resource.Id.btn_todoke_confirm);
            confirmButton.Click += delegate { Confirm(); };
        }

        private void GetVendorInfo()
        {
            string syukaDate = etSyukaDate.Text;
            string errTitle = "";
            string errMsg = "";

            KOSU131 result = WebService.ExecuteKosu131(soukoCd, kitakuCd, syukaDate.Remove('/'), etVendorCode.Text);

            if (result == null)
            {
                errTitle = "Error";
                errMsg = "ベンダーコードがみつかりません。";
            }
            else if (!(result.state == "00" || result.state == "01" || result.state == "99"))
            {
                errTitle = "確認";
                errMsg = "全ての検品が完了しています。";
            }
            else if (result.state != "00")
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
            }
            else
            {
                vendorNm = result.vendor_nm;
            }

        }

        private void CheckTokuisaki()
        {
            var progress = ProgressDialog.Show(this.Activity, null, "得意先を確認しています。", true);

            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(() =>
                {
                    Thread.Sleep(1500);

                    Dictionary<string, string> param = new Dictionary<string, string>
                    {
                        { "tokuisaki_cd",  prefs.GetString("tokuisaki_cd", "103")}
                    };

                    //string resultJson = CommonUtils.Post(WebService.KOSU.KOSU010, param);
                    //Dictionary<string, string> result = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultJson);
                    Dictionary<string, string> result = new Dictionary<string, string>();
                    //string count = result["cnt"];

                }
                );
                Activity.RunOnUiThread(() => ((MainActivity)this.Activity).DismissDialog());
            }
            )).Start();
        }

        private async Task<bool> IsExistTokuisaki()
        {
            bool isExist = false;
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "tokuisaki_cd",  prefs.GetString("tokuisaki_cd", "103")}
            };

            //string resultJson = await CommonUtils.PostAsync(WebService.KOSU.KOSU010, param);
            //Dictionary<string, string> result = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultJson);
            Dictionary<string, string> result = new Dictionary<string, string>();
            //string count = result["cnt"];

            isExist = true;
            
            return isExist;
        }

        private async Task<bool> IsExistTodokesaki()
        {
            bool isExist = false;

            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "tokuisaki_cd",  prefs.GetString("tokuisaki_cd", "103")}
            };

            //string resultJson = await CommonUtils.PostAsync(WebService.KOSU.KOSU020, param);
            //Dictionary<string, string> result = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultJson);
            Dictionary<string, string> result = new Dictionary<string, string>();
            //string count = result["cnt"];
            isExist = true;

            return isExist;
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
                    var progress = ProgressDialog.Show(this.Activity, null, "届先情報確認しています。", true);

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

                    this.Activity.RunOnUiThread(async () => {
                        if (!await IsExistTokuisaki())
                        {
                            CommonUtils.ShowAlertDialog(view, "エラー", "得意先コードがみつかりません。");
                            etTokuisaki.Text = "";
                            etTokuisaki.RequestFocus();
                        }
                        else
                        {
                            editor.PutString("deliveryDate", etSyukaDate.Text);
                            editor.PutString("tokuisaki", etTokuisaki.Text);
                            editor.PutString("deliveryDate", etSyukaDate.Text);
                            editor.PutBoolean("isConfirm", false); // 届先検索フラグ設定
                            editor.Apply();

                            progress.Dismiss();
                            StartFragment(FragmentManager, typeof(KosuBinInputFragment));
                        }
                    });
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
                                editor.PutBoolean("isConfirm", true); // 届先検索フラグ設定
                                editor.PutString("tokuisaki", etTokuisaki.Text);
                                editor.PutString("todokesaki", etTodokesaki.Text);
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

                                    editor.PutString("deliveryDate", etSyukaDate.Text);
                                    editor.Apply();

                                    StartFragment(FragmentManager, typeof(TodokeTyingWorkFragment));
                                }
                            }
                            else if (kosuMenuflag == (int)Const.KOSU_MENU.BARA)
                            {
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
                    etTokuisaki.RequestFocus();
                    return false;
                }

                
                if (!await IsExistTodokesaki())
                {
                    CommonUtils.ShowAlertDialog(view, "エラー", "届先コードがみつかりません。");
                    etTodokesaki.Text = "";
                    etTodokesaki.RequestFocus();
                    return false;
                }
            }
            else if (kosuMenuflag == (int)Const.KOSU_MENU.VENDOR)
            {
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

                if (vendorNm == "")
                {
                    string syukaDate = etSyukaDate.Text;
                    string errTitle = "";
                    string errMsg = "";

                    KOSU131 result = WebService.ExecuteKosu131(soukoCd, kitakuCd, syukaDate.Remove('/'), etVendorCode.Text);

                    if (result == null)
                    {
                        errTitle = "Error";
                        errMsg = "ベンダーコードがみつかりません。";
                    }
                    else if (!(result.state == "00" || result.state == "01" || result.state == "99"))
                    {
                        errTitle = "確認";
                        errMsg = "全ての検品が完了しています。";
                    }
                    else if (result.state != "00")
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
                    else
                    {
                        vendorNm = result.vendor_nm;
                        return true;
                    }
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

            editor.PutBoolean("isConfirm", false); // 届先検索フラグ設定
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