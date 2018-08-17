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
        private string shuka_date;
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

            etSyukaDate.Text = "18/03/20";
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
        }

        // パラメータ設定
        private void InitParamter()
        {
            souko_cd = prefs.GetString("souko_cd", "");
            kitaku_cd = prefs.GetString("kitaku_cd", "");
            shuka_date = prefs.GetString("shuka_date", "");
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
                    Thread.Sleep(500);

                    string kenpin_souko = "";
                    string kitaku_cd = "";
                    string syuka_date = "";
                    string nohin_date = "";
                    string course = "";
                    
                    TUMIKOMI010 result = WebService.RequestTumikomi010(kenpin_souko, kitaku_cd, syuka_date, nohin_date, course);

                    if (result.state == "03")
                    {
                        CommonUtils.AlertDialog(View, "エラー", "該当コースの積込みは完了しています。", () => { return; });
                    }

                    editor.PutString("bin_no", result.bin_no);
                    editor.PutString("kansen_kbn", result.kansen_kbn);
                    editor.Apply();

                    ShowConfirmMessage();

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
                /*
                // FTP店着実績ファイルの残骸がいるかもしれないので削除
				TOOL:del_File(nil, "FTPSend_" & Handy:serialId, 1)

				// 幹線便で且つメールバッグが存在した場合は「メールバッグ積込画面」へ遷移する。
				btvPram = JOB:souko_cd & "," & JOB:kitaku_cd & "," & JOB:shuka_date & "," & JOB:nohin_date & "," & JOB:bin_no & "," & JOB:course
				iRet = REMOTE:SearchCnt("TUMIKOMI230",3,btvPram)
				If iRet is nil Then
					If JOB:kansen_kbn eq "1" Then
						// 幹線便
						JOB:zoubin_flg = 1
						Return("sagyou5")
					Else
						Return("sagyou4")
					EndIf
				ElseIf iRet > 0 Then
					// メールバッグ積込画面へ
					JOB:mail_bag = 0
					JOB:zoubin_flg = 1
					DEVICE:read_OK()
					Return("sagyou14")
				Else
					If JOB:kansen_kbn eq "1" Then
						// 幹線便コース
						JOB:zoubin_flg = 1
						Return("sagyou5")
					Else
						Return("sagyou4")
					EndIf
				EndIf
                 */

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
                HideConfirmMessage();
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
                            //에러 コース№がみつかりません。
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
                                string kenpin_souko = "";
                                string kitaku_cd = "";
                                string syuka_date = "";
                                string nohin_date = "";
                                string course = "";

                                string haiso_date = CommonUtils.GetDateYYMMDDwithSlash(btvHaisohDate);

                                TUMIKOMI010 result = WebService.RequestTumikomi010(kenpin_souko, kitaku_cd, syuka_date, nohin_date, course);

                                if (result.state == "03")
                                {
                                    CommonUtils.AlertDialog(View, "エラー", "該当コースの積込みは完了しています。", () => { return; });
                                }

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