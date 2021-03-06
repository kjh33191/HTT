﻿using System.Collections.Generic;
using System.Threading;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Com.Beardedhen.Androidbootstrap;
using Com.Densowave.Bhtsdk.Barcode;
using HHT.Common;
using HHT.Resources.Model;

namespace HHT
{
    public class IdouRegistWorkFragment : BaseFragment
    {
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        private View view;
        private BootstrapEditText etKaisyuLabel, etIdouTenpo, etIdouTokuisaki, etIdouTodokesaki;
        private BootstrapButton _ConfirmButton;

        private string syuka_date;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_idou_regist_work, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            syuka_date = prefs.GetString("syuka_date", "");

            // コンポーネント初期化
            InitComponent();

            return view;
        }

        // コンポーネント初期化
        private void InitComponent()
        {
            etKaisyuLabel = view.FindViewById<BootstrapEditText>(Resource.Id.et_kaisyuLabel);
            etIdouTenpo = view.FindViewById<BootstrapEditText>(Resource.Id.et_idouTenpo);
            etIdouTenpo.Focusable = false;

            etIdouTokuisaki = view.FindViewById<BootstrapEditText>(Resource.Id.et_idouTokuisaki);
            etIdouTodokesaki = view.FindViewById<BootstrapEditText>(Resource.Id.et_idouTodokesaki);

            _ConfirmButton = view.FindViewById<BootstrapButton>(Resource.Id.btn_confirm);
            _ConfirmButton.Enabled = false;
            _ConfirmButton.Click += delegate { Confirm(); };

            etKaisyuLabel.KeyPress += (sender, e) => {
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    e.Handled = true;
                    CommonUtils.HideKeyboard(Activity);
                    CheckKaisyuLabel();
                }
                else
                {
                    e.Handled = false;
                }
            };

            etKaisyuLabel.RequestFocus();

        }

        private bool CheckKaisyuLabel()
        {
            try
            {
                TIDOU001 tidou001 = WebService.RequestTidou001(etKaisyuLabel.Text, syuka_date);

                string kaisyu_ten_nm = tidou001.tokuisaki_rk;
                etIdouTenpo.Text = kaisyu_ten_nm; // 先に設定

                if (kaisyu_ten_nm == "")
                {
                    ShowDialog("エラー", "移動ラベルがみつかりません。", () => {
                        etKaisyuLabel.RequestFocus();
                        etIdouTenpo.Text = "";
                    });
                    return false;
                }
                else
                {
                    if (tidou001.tenkan_state == "01")
                    {
                        ShowDialog("エラー", "該当の移動ラベルは登録済です。", () => {
                            etKaisyuLabel.RequestFocus();
                            etIdouTenpo.Text = "";
                        });
                        return false;
                    }

                    editor.PutString("kaisyu_ten_nm", kaisyu_ten_nm);
                }
            }
            catch
            {
                ShowDialog("エラー", "移動ラベルがみつかりません。", () => {
                    etKaisyuLabel.RequestFocus();
                    etIdouTenpo.Text = "";
                });
                return false;
            }

            _ConfirmButton.Enabled = true;

            return true;
        }
        
        private void Confirm()
        {
            if(etKaisyuLabel.Text == "" || etIdouTenpo.Text == "")
            {
                ShowDialog("エラー", "回収ラベルを確認してください。", () => { etKaisyuLabel.RequestFocus(); });
                return;
            }

            //得意先チェック
            string tokuisaki_cd = etIdouTokuisaki.Text;
            string todokesaki_cd = etIdouTodokesaki.Text;

            int count = WebService.RequestKosu020(tokuisaki_cd, todokesaki_cd);
            if (count == 0)
            {
                ShowDialog("エラー", "届先コードがみつかりません。", () => { etIdouTodokesaki.RequestFocus(); });
                return;
            }

            TIDOU002 tidou002 = WebService.RequestTidou002(tokuisaki_cd, todokesaki_cd);
            editor.PutString("tokuisaki_nm", tidou002.tokuisaki_rk);
            editor.PutString("area_nm", tidou002.area_nm);

            string msg = "移動先店舗：" + etIdouTenpo.Text + "\n";
                 　msg += "移動先得意先：" + etIdouTokuisaki.Text + "\n";
                   msg += "移動先届先：" + etIdouTodokesaki.Text + "\n";
               　　msg += "" + tidou002.area_nm + "\n\n";
                   msg += "よろしいですか？";

            ShowDialog("確認", msg, () => {
                new Thread(new ThreadStart(delegate {
                    Activity.RunOnUiThread(async () =>
                    {
                        var result = await DialogAsync.Show(this.Activity, "", "入荷予定登録を行います。\n\nよろしいですか。");
                        if (result == false)
                        {
                            return;
                        }

                        string terminal_id = "432660068";
                        string sagyousya_cd = "99999";
                        string souko_cd = prefs.GetString("souko_cd", "");
                        string kitaku_cd = prefs.GetString("kitaku_cd", "");
                        string kaisyubi = prefs.GetString("kaisyu_date", "");
                        string haisoubi = prefs.GetString("haisou_date", "");
                        string kaisyu_label = etKaisyuLabel.Text;

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

                        TIDOU010 tidou010 = WebService.RequestTidou010(param);

                        if (tidou010.poRet == "0")
                        {
                            //	正常登録
                            ShowDialog("報告", "登録しました。", () => { FragmentManager.PopBackStack(); });
                        }
                        else if (tidou010.poRet == "1")
                        {
                            ShowDialog("報告", "登録済みです。", () => { });
                        }
                        else if (tidou010.poRet == "2")
                        {
                            ShowDialog("エラー", "回収対象の貨物がありません。", () => { });
                        }
                        else if (tidou010.poRet == "3")
                        {
                            ShowDialog("エラー", "店間移動ベンダーがセンターマスタに設定されていません。", () => { });
                        }
                        else if (tidou010.poRet == "4")
                        {
                            ShowDialog("エラー", "店間移動ベンダーがベンダーマスタに存在しません。", () => { });
                        }
                        else if (tidou010.poRet == "5")
                        {
                            ShowDialog("エラー", "店間移動先センターコードがセンターマスタに存在しません。", () => { });
                        }
                        else if (tidou010.poRet == "6")
                        {
                            ShowDialog("エラー", "店間移動先コースがコース割付マスタに存在しません。", () => { });
                        }
                        else if (tidou010.poRet == "7")
                        {
                            ShowDialog("エラー", "店間移動先コースがコースマスタに存在しません。", () => { });
                        }
                        else
                        {
                            ShowDialog("エラー", "入荷予定データ作成に失敗しました。", () => { });
                        }
                    });
                }
                )).Start();
            });
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F1 || keycode == Keycode.F4)
            {
                Confirm();
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

                    if (etKaisyuLabel.HasFocus)
                    {
                        etKaisyuLabel.Text = data;

                        bool result = CheckKaisyuLabel();
                        if (!result)
                        {
                            return;
                        }
                        else
                        {
                            etIdouTokuisaki.Enabled = true;
                            etIdouTodokesaki.Enabled = true;
                            etIdouTokuisaki.RequestFocus();
                        }
                    }
                    else if (etIdouTokuisaki.HasFocus)
                    {
                        if (etKaisyuLabel.Text == "" || etIdouTenpo.Text == "")
                        {
                            ShowDialog("エラー", "回収ラベルを先に入力してください。", () => { etKaisyuLabel.RequestFocus(); });
                            return;
                        }

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
                                ShowDialog("エラー", "届先コードがみつかりません。", () => {  });
                                return;
                            }

                            TIDOU002 tidou002 = WebService.RequestTidou002(tokuisaki_cd, todokesaki_cd);
                            
                            editor.PutString("tokuisaki_nm", tidou002.tokuisaki_rk);
                            editor.PutString("area_nm", tidou002.area_nm);
                            etIdouTokuisaki.Text = tokuisaki_cd;
                            etIdouTodokesaki.Text = todokesaki_cd;

                            string msg = "移動先店舗：" + etIdouTenpo.Text + "\n";
                            msg += "移動先得意先：" + etIdouTokuisaki.Text + "\n";
                            msg += "移動先届先：" + etIdouTokuisaki.Text + "\n";
                            msg += "" + tidou002.area_nm + "\n";

                            ShowDialog("確認", msg, () => {
                                new Thread(new ThreadStart(delegate {
                                    Activity.RunOnUiThread(async () =>
                                    {
                                        var result = await DialogAsync.Show(this.Activity, "", "入荷予定登録を行います。\n\nよろしいですか。");
                                        if (result == false)
                                        {
                                            return;
                                        }

                                        string terminal_id = "432660068";
                                        string sagyousya_cd = "99999";
                                        string souko_cd = prefs.GetString("souko_cd", "");
                                        string kitaku_cd = prefs.GetString("kitaku_cd", "");
                                        string kaisyubi = prefs.GetString("kaisyu_date", "");
                                        string haisoubi = prefs.GetString("haisou_date", "");
                                        string kaisyu_label = etKaisyuLabel.Text;

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

                                        TIDOU010 tidou010 = WebService.RequestTidou010(param);

                                        if (tidou010.poRet == "0")
                                        {
                                            //	正常登録
                                            ShowDialog("報告", "登録しました", () => { FragmentManager.PopBackStack(); });
                                        }
                                        else if (tidou010.poRet == "1")
                                        {
                                            ShowDialog("報告", "登録済みです。", () => { });
                                        }
                                        else if (tidou010.poRet == "2")
                                        {
                                            ShowDialog("エラー", "回収対象の貨物がありません。", () => { });
                                        }
                                        else if (tidou010.poRet == "3")
                                        {
                                            ShowDialog("エラー", "店間移動ベンダーがセンターマスタに設定されていません。", () => { });
                                        }
                                        else if (tidou010.poRet == "4")
                                        {
                                            ShowDialog("エラー", "店間移動ベンダーがベンダーマスタに存在しません。", () => { });
                                        }
                                        else if (tidou010.poRet == "5")
                                        {
                                            ShowDialog("エラー", "店間移動先センターコードがセンターマスタに存在しません。", () => { });
                                        }
                                        else if (tidou010.poRet == "6")
                                        {
                                            ShowDialog("エラー", "店間移動先コースがコース割付マスタに存在しません。", () => { });
                                        }
                                        else if (tidou010.poRet == "7")
                                        {
                                            ShowDialog("エラー", "店間移動先コースがコースマスタに存在しません。", () => { });
                                        }
                                        else
                                        {
                                            ShowDialog("エラー", "入荷予定データ作成に失敗しました。", () => { });
                                        }

                                    });
                                }
                                )).Start();
                            });
                        }
                    }
                });
            }
        }
    }
}