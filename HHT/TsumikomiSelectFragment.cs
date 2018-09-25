using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;
using HHT.Resources.Model;

namespace HHT
{
    public class TsumikomiSelectFragment : BaseFragment
    {
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        View view;
        EditText etSyukaDate, etCourse;
        Button btnConfirm;

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
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            // TITLE SETTING
            SetTitle("積込検品");

            // PARAMETER SETTING
            souko_cd = prefs.GetString("souko_cd", "");
            kitaku_cd = prefs.GetString("kitaku_cd", "");
            syuka_date = prefs.GetString("syuka_date", "");
            nohin_date = prefs.GetString("nohin_date", "");
            course = prefs.GetString("course", "");
            bin_no = prefs.GetString("bin_no", "");
            kansen_kbn = prefs.GetString("kansen_kbn", "");

            // ITEM EVENT SETTING 
            etSyukaDate = view.FindViewById<EditText>(Resource.Id.et_tsumikomiSelect_syukaDate);
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

            etCourse = view.FindViewById<EditText>(Resource.Id.et_tsumikomiSelect_course);

            btnConfirm = view.FindViewById<Button>(Resource.Id.btn_tsumikomiSelect_confirm);
            btnConfirm.Click += delegate { Confirm(); };

            // FIRST FOCUS
            etCourse.RequestFocus();
            
            // DUMMY DATA
            etSyukaDate.Text = "18/03/20";

            return view;
        }

        // CHECK INPUT AND MOVE TO NEXT FRAGMENT
        private void Confirm()
        {
            ((MainActivity)this.Activity).ShowProgress("便情報を確認しています。");

            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        syuka_date = "20" + etSyukaDate.Text.Replace("/", "");
                        TUMIKOMI010 result = WebService.RequestTumikomi010(souko_cd, kitaku_cd, syuka_date, etCourse.Text);

                        if (result == null)
                        {
                            CommonUtils.AlertDialog(View, "エラー", "コースNoがみつかりません。", null);
                            return;
                        }
                        else if (result.state == "03")
                        {
                            CommonUtils.AlertDialog(View, "エラー", "該当コースの積込みは完了しています。", null);
                            return;
                        }

                        bin_no = result.bin_no;
                        kansen_kbn = result.kansen_kbn;

                        editor.PutString("syuka_date", syuka_date);
                        editor.PutString("course", etCourse.Text);
                        editor.PutString("bin_no", bin_no);
                        editor.PutString("kansen_kbn", kansen_kbn);
                        editor.Apply();

                        ShowConfirmMessage();
                        CommonUtils.HideKeyboard(this.Activity);
                    }
                    catch
                    {
                        CommonUtils.AlertDialog(View, "エラー", "コースNoがみつかりません。", () => { return; });
                    }
                }
                );
                Activity.RunOnUiThread(() => ((MainActivity)this.Activity).DismissDialog());
            }
            )).Start();
        }

        private void ShowConfirmMessage()
        {
            string message = "配送日".PadLeft(5) + " : " + etSyukaDate.Text + "\n";
            message += "コース".PadLeft(5) + " : " + etCourse.Text + "\n";
            message += "便No".PadLeft(5) + " : " + bin_no + "\n\n";
            message += "よろしいですか？".PadLeft(5) + " : " + bin_no + "\n";

            CommonUtils.AlertConfirm(view, "確認", message, (flag)=> {
                if (flag)
                {
                    int count = WebService.RequestTumikomi230(souko_cd,kitaku_cd, syuka_date, nohin_date, bin_no, etCourse.Text);

                    // TODO
                    if (count > 0)
                    {
                        // メールバッグ積込画面へ 
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
            });
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F4)
            {
                ShowConfirmMessage();
                return false;
            }

            return true;
        }

        public override void OnBarcodeDataReceived(BarcodeDataReceivedEvent_ dataReceivedEvent)
        {
            IList<BarcodeDataReceivedEvent_.BarcodeData_> listBarcodeData = dataReceivedEvent.BarcodeData;

            foreach (BarcodeDataReceivedEvent_.BarcodeData_ barcodeData in listBarcodeData)
            {
                Activity.RunOnUiThread(() =>
                {
                    string data = barcodeData.Data;

                    if (etCourse.HasFocus)
                    {
                        if (data.Length < 12)
                        {
                            CommonUtils.AlertDialog(View, "エラー", "コースNoがみつかりません。", null);
                            return;
                        }
                       
                        string btvTmp = data.Substring(0, 11);              // 配送日(8桁) + センター(3桁)
                        string btvHaisohDate = btvTmp.Substring(2, 4);      // 配送日(YYMMDD)
                        string btvCenterCd = btvTmp.Substring(6, 3);        // センターコード(3桁)
                        string btvCourse = data.Substring(11, data.Length); // コース(桁可変) 

                        try
                        {
                            string haiso_date = CommonUtils.GetDateYYMMDDwithSlash(btvHaisohDate);

                            TUMIKOMI010 result = WebService.RequestTumikomi010(souko_cd, kitaku_cd, syuka_date, btvCourse);
                            
                            if (result.state == "03")
                            {
                                CommonUtils.AlertDialog(View, "エラー", "該当コースの積込みは完了しています。", null);
                                return;
                            }
                            
                            editor.PutString("syuka_date", syuka_date);
                            editor.PutString("course", btvCourse);
                            editor.PutString("bin_no", result.bin_no);
                            editor.PutString("kansen_kbn", result.kansen_kbn);
                            editor.Apply();

                            ShowConfirmMessage();
                        }
                        catch
                        {
                            CommonUtils.AlertDialog(View, "エラー", "コースNoがみつかりません。", null);
                            return;
                        }
                    }
                });
            }
        }
    }
}