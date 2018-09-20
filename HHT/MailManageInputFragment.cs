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
    public class MailManageInputFragment : BaseFragment
    {
        View view;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        EditText etMailBag;
        TextView mailBagSu;
        private bool registFlg;
        private int mail_back;

        private readonly string ERR1 = "メールバッグバーコードではありません。";
        private readonly string ERR2 = "該当のメールバッグは登録済です。";
        private readonly string ERR3 = "該当コースは積込済のため登録できません。";
        private readonly string ERR4 = "該当のメールバッグは出発点呼済です。";
        private readonly string ERR5 = "マスタに存在しない店舗のメールバッグをスキャンしました。";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_mail_manage_input, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            // コンポーネント初期化
            registFlg = prefs.GetBoolean("registFlg", true);

            if (registFlg)
            {
                SetTitle("メールバッグ登録");
            }
            else
            {
                SetTitle("メールバッグ削除");
            }

            Button btnComplete = view.FindViewById<Button>(Resource.Id.btn_mailRegistInput_complete);
            btnComplete.Click += delegate { Complete(); };

            etMailBag = view.FindViewById<EditText>(Resource.Id.et_mailRegistInput_mail);
            mailBagSu = view.FindViewById<TextView>(Resource.Id.txt_mailRegistInput_mailbagSu);
            mail_back = 0;
            
            etMailBag.RequestFocus();
            

            return view;
        }

        private void Complete()
        {
            if (registFlg)
            {
                editor.PutString("completeTitle", "メールバッグ登録");
                editor.PutString("completeMsg", "メールバッグの登録が\n完了しました。");
                editor.Apply();
                StartFragment(FragmentManager, typeof(CompleteFragment));
            }
            else
            {
                FragmentManager.PopBackStack();
            }
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {

            if (keycode == Keycode.F1)
            {
                FragmentManager.PopBackStack();
            }
            else if (keycode == Keycode.Back)
            {

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

                    // メールバッグ専用バーコード確認
                    if (data[0].ToString() != "M") {
                        CommonUtils.AlertDialog(view, "", ERR1, null);
                        return;
                    }
                   
                    if (registFlg)　
                    {
                        // メールバック登録処理
                        Dictionary<string, string> param = new Dictionary<string, string>
                        {
                            {"pTerminalID", prefs.GetString("terminal_id","") },
                            {"pProgramID", prefs.GetString("program_id","") },
                            {"pSagyosyaCD", prefs.GetString("sagyousya_cd","") },
                            {"pSoukoCD", prefs.GetString("souko_cd","") },
                            {"pHaisoDate", prefs.GetString("haiso_date","") },
                            {"pTokuisakiCD", data.Substring(1,4) },
                            {"pTodokesakiCD", data.Substring(5,4) },
                            {"pKanriNo", data },
                            {"pBinNo", prefs.GetString("bin_no","") },
                        };

                        string pSagyosyaCD = "";
                        string pSoukoCD = "";
                        string pHaisoDate = "";
                        string pBinNo = "";
                        string pTokuisakiCD = "";
                        string pTodokesakiCD = "";
                        string pKanriNo = "";

                        Dictionary<string, string> result = WebService.RequestMAIL010(pSagyosyaCD, pSoukoCD, pHaisoDate, pBinNo, pTokuisakiCD, pTodokesakiCD, pKanriNo);
                        
                        switch (result["poRet"].ToString())
                        {
                            case "0":
                                //	登録解除
                                mail_back++;
                                mailBagSu.Text = "(" + mail_back + ")";
                                break;
                            case "1":
                                //	該当コースが既に積込中以上のステータスのためエラー
                                // DEVICE:syougou_NG()
                                CommonUtils.AlertDialog(view, "", ERR3, null);
                                break;
                            case "2":
                                //	該当コースが既に出発受付以上のステータスのためエラー
                                // DEVICE:syougou_NG()
                                CommonUtils.AlertDialog(view, "", ERR4, null);
                                break;
                            case "3":
                                //	コース割付マスタに存在しないためエラー
                                // DEVICE:syougou_NG()
                                CommonUtils.AlertDialog(view, "", ERR5, null);
                                break;
                            case "9":
                                //	登録済
                                // DEVICE:syougou_NG()
                                CommonUtils.AlertDialog(view, "", ERR2, null);
                                break;
                            default:
                                break;

                        }
                    }
                    else
                    {
                        // メールバック削除処理
                        Dictionary<string, string> param = new Dictionary<string, string>
                        {
                            {"pTerminalID", prefs.GetString("terminal_id","") },
                            {"pProgramID", prefs.GetString("program_id","") },
                            {"pSagyosyaCD", prefs.GetString("sagyousya_cd","") },
                            {"pSoukoCD", prefs.GetString("souko_cd","") },
                            {"pHaisoDate", prefs.GetString("haiso_date","") },
                            {"pTokuisakiCD", data.Substring(1,4) },
                            {"pTodokesakiCD", data.Substring(5,4) },
                            {"pKanriNo", data },
                            {"pBinNo", prefs.GetString("bin_no","") },
                        };

                        //int ret = WebService.requestMail030();
                        int ret = 0;

                        switch (ret)
                        {
                            case 0:
                                //	登録解除
                                mail_back++;
                                mailBagSu.Text = "(" + mail_back + ")";
                                CommonUtils.AlertDialog(view, "", "登録されているメールバッグを取消しました。", null);
                                break;
                            case 1:
                                // 未登録
                                // DEVICE:syougou_NG()
                                CommonUtils.AlertDialog(view, "", "該当のメールバッグは未登録です。", null);
                                break;
                            case 2:
                                //	該当コースが既に出発受付以上のステータスのためエラー
                                // DEVICE:syougou_NG()
                                CommonUtils.AlertDialog(view, "", "該当のメールバッグは出発点呼済です。", null);
                                break;
                            case 3:
                                //	コース割付マスタに存在しないためエラー
                                // DEVICE:syougou_NG()
                                CommonUtils.AlertDialog(view, "", "マスタに存在しない店舗のメールバッグをスキャンしました。", null);
                                break;
                            default:
                                break;

                        }
                    }
                });
            }
        }
    }
}