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
    public class TsumikomiSelectFragment : BaseFragment
    {
        View view;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        EditText etSyukaDate, etCourse, etBinNo;
        TextView txtConfirmMsg, txtConfirmBin;
        private string souko_cd;
        private string kitaku_cd;
        private string syuka_date;
        private string nohin_date;
        private string course;
        private string bin_no;
        private string kansen_kbn;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_tsumikomi_select, container, false);

            // コンポーネント初期化
            InitComponent();

            // パラメータ設定
            InitParamter();

            return view;
        }

        // コンポーネント初期化
        private void InitComponent()
        {
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetTitle("積込検品");

            Button btnConfirm = view.FindViewById<Button>(Resource.Id.btn_tsumikomiSelect_confirm);
            etSyukaDate = view.FindViewById<EditText>(Resource.Id.et_tsumikomiSelect_syukaDate);
            etCourse = view.FindViewById<EditText>(Resource.Id.et_tsumikomiSelect_course);
            etCourse.FocusChange += (sender, e) => { if (!e.HasFocus && etCourse.Text != "") SearchBinNo(); };
            etBinNo = view.FindViewById<EditText>(Resource.Id.et_tsumikomiSelect_binNo);
            txtConfirmBin = view.FindViewById<TextView>(Resource.Id.tv_tsumikomiSelect_confirmBin);
            txtConfirmMsg = view.FindViewById<TextView>(Resource.Id.tv_tsumikomiSelect_confirmMsg);

            btnConfirm.Click += delegate { Confirm(); };
            etSyukaDate.FocusChange += (sender, e) => {
                if (e.HasFocus)
                {
                    etSyukaDate.Text = etSyukaDate.Text.Replace("/", "");
                }
                else
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
            };

            etSyukaDate.Text = "18/03/20";
            etCourse.RequestFocus();

        }

        // パラメータ設定
        private void InitParamter()
        {
            souko_cd = prefs.GetString("souko_cd", "");
            kitaku_cd = prefs.GetString("kitaku_cd", "");
            syuka_date = prefs.GetString("syuka_date", "");
            nohin_date = prefs.GetString("nohin_date", "");
            course = prefs.GetString("course", "");
            bin_no = prefs.GetString("bin_no", "");
            kansen_kbn = prefs.GetString("kansen_kbn", "");

        }

        private void SearchBinNo()
        {
            ((MainActivity)this.Activity).ShowProgress("便情報を確認しています。");

            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(() =>
                {
                    //TUMIKOMI010 result = WebService.RequestTumikomi010(souko_cd, kitaku_cd, syuka_date, nohin_date, course);
                    
                    TUMIKOMI010 result = new TUMIKOMI010
                    {
                        state = "00",
                        bin_no = "1",
                        kansen_kbn = "0"
                    };
                    

                    if (result.state == "03")
                    {
                        CommonUtils.AlertDialog(View, "エラー", "該当コースの積込みは完了しています。", () => { return; });
                    }

                    bin_no = result.bin_no;
                    kansen_kbn = result.kansen_kbn;

                    etBinNo.Text = bin_no;
                    editor.PutString("bin_no", bin_no);
                    editor.PutString("kansen_kbn", kansen_kbn);
                    editor.Apply();

                    ShowConfirmMessage();
                    CommonUtils.HideKeyboard(this.Activity);
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
                txtConfirmBin.Visibility = ViewStates.Visible;
                txtConfirmMsg.Visibility = ViewStates.Visible;
                txtConfirmBin.Text = 2 + " 便";

                etSyukaDate.Focusable = false;
                etCourse.Focusable = false;
                etBinNo.Focusable = false;
            }
        }

        private void HideConfirmMessage()
        {
            if (txtConfirmMsg.Visibility == ViewStates.Visible)
            {
                txtConfirmBin.Visibility = ViewStates.Gone;
                txtConfirmMsg.Visibility = ViewStates.Gone;

                etSyukaDate.Focusable = true;
                etCourse.Focusable = true;
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

                //int count = WebService.RequestTumikomi230();
                int count = 0;

                if(count > 0)
                {
                    // Return("sagyou14")
                    return;
                }

                if (kansen_kbn == "1")
                {
                    StartFragment(FragmentManager, typeof(TsumikomiWorkFragment));
                }
                else
                {
                    StartFragment(FragmentManager, typeof(TsumikomiSearchFragment));
                }
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

                    if (etCourse.HasFocus)
                    {
                        if (data.Length < 12)
                        {
                            CommonUtils.AlertDialog(View, "エラー", "コースNoがみつかりません。", () => { return; });
                        }
                        else
                        {
                            string btvTmp = data.Substring(0, 11);              // 配送日(8桁) + センター(3桁)
                            string btvHaisohDate = btvTmp.Substring(2, 4);      // 配送日(YYMMDD)
                            string btvCenterCd = btvTmp.Substring(6, 3);        // センターコード(3桁)
                            string btvCourse = data.Substring(11, data.Length); // コース(桁可変) 

                            try
                            {

                                string haiso_date = CommonUtils.GetDateYYMMDDwithSlash(btvHaisohDate);

                                //TUMIKOMI010 result = WebService.RequestTumikomi010(souko_cd, kitaku_cd, syuka_date, nohin_date, btvCourse);
                                
                                TUMIKOMI010 result = new TUMIKOMI010
                                {
                                    state = "00",
                                    bin_no = "1",
                                    kansen_kbn = "0"
                                };
                                

                                if (result.state == "03")
                                {
                                    CommonUtils.AlertDialog(View, "エラー", "該当コースの積込みは完了しています。", () => { return; });
                                }

                                etBinNo.Text = bin_no;
                                editor.PutString("course", btvCourse);
                                editor.PutString("bin_no", result.bin_no);
                                editor.PutString("kansen_kbn", result.kansen_kbn);
                                editor.Apply();

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