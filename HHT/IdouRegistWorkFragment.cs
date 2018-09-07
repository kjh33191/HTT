using System.Collections.Generic;
using System.Threading;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;
using HHT.Common;
using HHT.Resources.Model;

namespace HHT
{
    public class IdouRegistWorkFragment : BaseFragment
    {
        View view;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        EditText etKaisyuLabel, etIdouTenpo, etIdouTokuisaki, etIdouTodokesaki;
        TextView txtConfirmMsg, txtAreaName;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_idou_regist_work, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            // コンポーネント初期化
            InitComponent();

            return view;
        }

        // コンポーネント初期化
        private void InitComponent()
        {
            etKaisyuLabel = view.FindViewById<EditText>(Resource.Id.et_kaisyuLabel);
            etIdouTenpo = view.FindViewById<EditText>(Resource.Id.et_idouTenpo);
            etIdouTokuisaki = view.FindViewById<EditText>(Resource.Id.et_idouTokuisaki);
            etIdouTodokesaki = view.FindViewById<EditText>(Resource.Id.et_idouTodokesaki);
            Button btnConfirm = view.FindViewById<Button>(Resource.Id.btn_confirm);
            txtConfirmMsg = view.FindViewById<TextView>(Resource.Id.txt_ConfirmMessage);
            txtAreaName = view.FindViewById<TextView>(Resource.Id.txt_areaName);

            etKaisyuLabel.FocusChange += delegate
            {
                if (!etKaisyuLabel.HasFocus)
                {
                    //etKaisyuLabel.Text = "0132456811154";

                    //TIDOU001 tidou001 = WebService.RequestTidou001(etKaisyuLabel.Text, "20180906");
                    TIDOU001 tidou001 = new TIDOU001
                    {
                        tokuisaki_rk = "テスト店舗名",
                        tenkan_state = "00"
                    };

                    string kaisyu_ten_nm = tidou001.tokuisaki_rk;
                    etIdouTenpo.Text = kaisyu_ten_nm;

                    if (kaisyu_ten_nm == "")
                    {
                        CommonUtils.AlertDialog(view, "", "移動ラベルがみつかりません。", null);
                        return;
                    }
                    else
                    {
                        if (tidou001.tenkan_state == "01")
                        {
                            CommonUtils.AlertDialog(view, "", "該当の移動ラベルは登録済です。", null);
                            return;
                        }

                        editor.PutString("kaisyu_ten_nm", kaisyu_ten_nm);
                        etIdouTokuisaki.RequestFocus();
                    }
                }
            };

            etKaisyuLabel.RequestFocus();
            btnConfirm.Click += delegate { Confirm(); };
        }
        
        private void Confirm()
        {
            if (txtConfirmMsg.Visibility == ViewStates.Invisible)
            {
                // 回収ラベルチェック
                //TIDOU001 tidou001 = WebService.RequestTidou001(etKaisyuLabel.Text, "20180906");
                TIDOU001 tidou001 = new TIDOU001
                {
                    tokuisaki_rk = "test",
                    tenkan_state = "01"
                };

                string kaisyu_ten_nm = tidou001.tokuisaki_rk;
                etIdouTenpo.Text = kaisyu_ten_nm;

                if (kaisyu_ten_nm == "")
                {
                    CommonUtils.AlertDialog(view, "", "移動ラベルがみつかりません。", null);
                    return;
                }
                else
                {
                    if (tidou001.tenkan_state == "01")
                    {
                        CommonUtils.AlertDialog(view, "", "該当の移動ラベルは登録済です。", null);
                        return;
                    }

                    editor.PutString("kaisyu_ten_nm", kaisyu_ten_nm);
                }

                //得意先チェック
                string tokuisaki_cd = etKaisyuLabel.Text.Substring(0, 4);
                string todokesaki_cd = etKaisyuLabel.Text.Substring(4, 4);

                int count = WebService.RequestKosu020(tokuisaki_cd, todokesaki_cd);
                if (count == 0)
                {
                    CommonUtils.AlertDialog(view, "", "届先コードがみつかりません。", null);
                    return;
                }

                TIDOU002 tidou002 = WebService.RequestTidou002(tokuisaki_cd, todokesaki_cd);
                editor.PutString("tokuisaki_nm", tidou002.tokuisaki_rk);
                editor.PutString("area_nm", tidou002.area_nm);

                // == > よろしいですか？
                txtConfirmMsg.Visibility = ViewStates.Visible;
            }
            else
            {
                new Thread(new ThreadStart(delegate {
                    Activity.RunOnUiThread(async () =>
                    {

                        var result = await DialogAsync.Show(this.Activity, "", "入荷予定登録を行います。\n\nよろしいですか。");
                        if (result == false)
                        {
                            return;
                        }
                       
                        string terminal_id = "";
                        string sagyousya_cd = "";
                        string souko_cd = "";
                        string kitaku_cd = "";
                        string kaisyubi = ""; // remove'/'
                        string haisoubi = ""; // remove'/'
                        string kaisyu_label = "";
                        string tokuisaki_cd = "";
                        string todokesaki_cd = "";

                        Dictionary<string, string> param = new Dictionary<string, string>
                        {
                            {"pPackage", "01" },
                            {"pTerminalId", terminal_id },
                            {"pProgramId", "TIDO" },
                            {"pSagyosyaCd", sagyousya_cd },
                            {"pSoukoCd", souko_cd },
                            {"pKitakuCd", kitaku_cd },
                            {"pKaisyuDate", kaisyubi },
                            {"pHaisohDate", haisoubi },
                            {"pKamotuNo", kaisyu_label },
                            {"pTokuisakiCd", tokuisaki_cd },
                            {"pTodokesakiCd", todokesaki_cd }
                        };

                        //TIDOU010 tidou010 = WebService.RequestTidou010(param);
                        TIDOU010 tidou010 = new TIDOU010
                        {
                            poRet = "0"
                        };

                        if (tidou010.poRet == "0")
                        {
                            //	正常登録
                            CommonUtils.AlertDialog(view, "", "登録しました。", () => {
                                FragmentManager.PopBackStack();
                            }
                            );
                            //JOB: compleate_yn = "1"
                            // 完了ボタンの表示
                        }
                        else if (tidou010.poRet == "1")
                        {
                            CommonUtils.AlertDialog(view, "", "登録済みです。", null);
                        }
                        else if (tidou010.poRet == "2")
                        {
                            CommonUtils.AlertDialog(view, "", "回収対象の貨物がありません。", null);
                            txtConfirmMsg.Visibility = ViewStates.Invisible;
                        }
                        else if (tidou010.poRet == "3")
                        {
                            CommonUtils.AlertDialog(view, "", "店間移動ベンダーがセンターマスタに設定されていません。", null);
                            txtConfirmMsg.Visibility = ViewStates.Invisible;
                        }
                        else if (tidou010.poRet == "4")
                        {
                            CommonUtils.AlertDialog(view, "", "店間移動ベンダーがベンダーマスタに存在しません。", null);
                            txtConfirmMsg.Visibility = ViewStates.Invisible;
                        }
                        else if (tidou010.poRet == "5")
                        {
                            CommonUtils.AlertDialog(view, "", "店間移動先センターコードがセンターマスタに存在しません。", null);
                            txtConfirmMsg.Visibility = ViewStates.Invisible;
                        }
                        else if (tidou010.poRet == "6")
                        {
                            CommonUtils.AlertDialog(view, "", "店間移動先コースがコース割付マスタに存在しません。", null);
                            txtConfirmMsg.Visibility = ViewStates.Invisible;
                        }
                        else if (tidou010.poRet == "7")
                        {
                            CommonUtils.AlertDialog(view, "", "店間移動先コースがコースマスタに存在しません。", null);
                            txtConfirmMsg.Visibility = ViewStates.Invisible;
                        }
                        else
                        {
                            CommonUtils.AlertDialog(view, "", "入荷予定データ作成に失敗しました。", null);
                            txtConfirmMsg.Visibility = ViewStates.Invisible;
                        }

                    });
                    //Activity.RunOnUiThread(() => ((MainActivity)this.Activity).DismissDialog());
                }
                )).Start();
            }
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {

            if (keycode == Keycode.F1 || keycode == Keycode.F4)
            {
                Confirm();
            }
            else if (keycode == Keycode.Back)
            {
                if(txtConfirmMsg.Visibility == ViewStates.Visible)
                {
                    txtConfirmMsg.Visibility = ViewStates.Invisible;
                }
                else
                {
                    FragmentManager.PopBackStack();
                }
            }

            return true;
        }

        public override bool OnBackPressed()
        {
            return false;
        }


        public override void OnBarcodeDataReceived(BarcodeDataReceivedEvent_ dataReceivedEvent)
        {
            IList<BarcodeDataReceivedEvent_.BarcodeData_> listBarcodeData = dataReceivedEvent.BarcodeData;

            foreach (BarcodeDataReceivedEvent_.BarcodeData_ barcodeData in listBarcodeData)
            {
                this.Activity.RunOnUiThread(() =>
                {
                    string data = barcodeData.Data;

                    if (etKaisyuLabel.HasFocus)
                    {
                        data = "0132456811154";
                        etKaisyuLabel.Text = data;

                        //TIDOU001 tidou001 = WebService.RequestTidou001(etKaisyuLabel.Text, "20180906");
                        TIDOU001 tidou001 = new TIDOU001
                        {
                            tokuisaki_rk = "テスト店舗名",
                            tenkan_state = "00"
                        };

                        string kaisyu_ten_nm = tidou001.tokuisaki_rk;
                        etIdouTenpo.Text = kaisyu_ten_nm;

                        if (kaisyu_ten_nm == "")
                        {
                            CommonUtils.AlertDialog(view, "", "移動ラベルがみつかりません。", null);
                            return;
                        }
                        else
                        {
                            if (tidou001.tenkan_state == "01")
                            {
                                CommonUtils.AlertDialog(view, "", "該当の移動ラベルは登録済です。", null);
                                return;
                            }

                            editor.PutString("kaisyu_ten_nm", kaisyu_ten_nm);
                            etIdouTokuisaki.RequestFocus();
                        }
                    }
                    else if (etIdouTokuisaki.HasFocus)
                    {
                        if (etKaisyuLabel.Text == "" || etIdouTenpo.Text == "")
                        {
                            CommonUtils.AlertDialog(view, "", "回収ラベルを先に入力してください。", null);
                            return;
                        }

                        data = "00000374";
                        if (data.Length == 4)
                        {
                            etIdouTokuisaki.Text = data;
                            return;
                        }
                        else if (data.Length == 8)
                        {
                            //得意先チェック
                            string tokuisaki_cd = data.Substring(0, 4);
                            string todokesaki_cd = data.Substring(4, 4);

                            int count = WebService.RequestKosu020(tokuisaki_cd, todokesaki_cd);
                            if (count == 0)
                            {
                                CommonUtils.AlertDialog(view, "", "届先コードがみつかりません。", null);
                                return;
                            }

                            TIDOU002 tidou002 = WebService.RequestTidou002(tokuisaki_cd, todokesaki_cd);
                            
                            editor.PutString("tokuisaki_nm", tidou002.tokuisaki_rk);
                            editor.PutString("area_nm", tidou002.area_nm);
                            etIdouTokuisaki.Text = tokuisaki_cd;
                            etIdouTodokesaki.Text = todokesaki_cd;
                            txtAreaName.Text = tidou002.area_nm;

                            // == > よろしいですか？
                            txtConfirmMsg.Visibility = ViewStates.Visible;
                        }
                        else
                        {

                        }
                        
                    }
                });
            }
        }
    }
}