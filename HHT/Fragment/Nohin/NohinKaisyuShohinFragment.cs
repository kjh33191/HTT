using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Beardedhen.Androidbootstrap;
using Com.Densowave.Bhtsdk.Barcode;
using HHT.Resources.DataHelper;
using HHT.Resources.Model;
using System.Collections.Generic;

namespace HHT
{
    public class NohinKaisyuShohinFragment : BaseFragment
    {
        private string TAG = "NohinKaisyuShohinFragment";
        private View view;
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;
        private List<string> arrKamotu;

        private TextView txtIdouSu, txtMailSu, txtHazaiSu, txtSonotaSu, txtHenpinSu, txtSougoSu;
        private BootstrapButton btnOsamu;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_nohin_kaisyu_shohin, container, false);

            // パラメータ管理
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            // 項目初期化
            InitComponent();

            return view;
        }

        private void InitComponent()
        {
            SetTitle("商品回収");

            TextView txtTokusakiNm = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuShohin_tokuisakiNm);
            TextView txtTodokesakiNm = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuShohin_todokisakiNm);

            txtIdouSu = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuShohin_idousu);
            txtMailSu = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuShohin_mailsu);
            txtHazaiSu = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuShohin_hazaisu);
            txtSonotaSu = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuShohin_sonotasu);
            txtHenpinSu = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuShohin_henpinsu);
            txtSougoSu = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuShohin_sougosu);

            txtTokusakiNm.Text = prefs.GetString("tokuisaki_nm", "");
            txtTodokesakiNm.Text = prefs.GetString("todokesaki_nm", "");
            arrKamotu = new List<string>();

            BootstrapButton button1 = view.FindViewById<BootstrapButton>(Resource.Id.btn_nohinKaisyuShohin_confirm);
            button1.Click += delegate { Confirm(); };

            btnOsamu = view.FindViewById<BootstrapButton>(Resource.Id.btn_nohinKaisyuShohin_osamu);
            btnOsamu.Click += delegate
            {
                if (btnOsamu.Text == "修")
                {
                    btnOsamu.Text = "入";
                }
                else
                {
                    btnOsamu.Text = "修";
                }
            };
        }

        private void Confirm()
        {
            string confirmMsg = @"
シーエスイー
水天宮店
移動[  2] 返品[  2]
破材[  1] ﾒｰﾙ [  1]
他　[  1]

総個数(  7)

よろしいですか？
                                        ";

            CommonUtils.AlertConfirm(view, "確認", "よろしいですか？", (flag) =>
            {
                if (flag)
                {
                    Log.Debug(TAG, "COMMIT");
                    //送信ファイル作成
                    //repairFile();

                    foreach (string kamotu_no in arrKamotu) {
                        appendFile(kamotu_no);
                        // ==> DB insert 
                    }

                    editor.PutString("label_flg", "0");
                    editor.Apply();
                    // arrSyo_su = 0 必要？

                    FragmentManager.PopBackStack();
                }
            });
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Enter || keycode == Keycode.F4)
            {
                Confirm();
            }
            else if (keycode == Keycode.Back)
            {
                editor.PutString("label_flg", "0");
                editor.Apply();

                // DEF_SYO_KAI fileのもとに戻す処理が必要

                //arrKamotu
                // 
                //string tokusaki_cd = prefs.GetString("tokuisaki_cd", "");
                //string todokesaki_cd = prefs.GetString("todokesaki_cd", "");

                // file search

                Log.Debug(TAG, "L return");

            }
            else if (keycode == Keycode.F2)
            {
                string label_flg = prefs.GetString("label_flg", "0");
                if(label_flg == "0")
                {
                    editor.PutString("label_flg", "1");
                }
                else
                {
                    editor.PutString("label_flg", "0");
                }

                editor.Apply();
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
                    string kamotu_no = barcodeData.Data;

                    if (kamotu_no.Length > 0)
                    {
                        string label_flg = prefs.GetString("label_flg", "0");
                        int btvLen = int.Parse(kamotu_no[0].ToString());

                        //*** バーコードが納品する届先のものかを確認
                        string btvTokuisaki = "";
                        string btvTodokesaki = "";

                        if (btvLen == 7)   // 店間移動
                        {
                            btvTokuisaki = kamotu_no.Substring(1, 4);
                            btvTodokesaki = kamotu_no.Substring(5, 4);
                        }
                        else
                        {
                            btvTokuisaki = kamotu_no.Substring(9, 4);
                            btvTodokesaki = kamotu_no.Substring(13, 4);
                        }

                        if (btvTokuisaki != prefs.GetString("tokuisaki_cd", "")
                            || btvTodokesaki != prefs.GetString("todokesaki_cd", ""))
                        {
                            CommonUtils.AlertDialog(view, "", "納入先店舗が違います。", null);
                            return;
                        }

                        //*** スキャンデータチェック
                        if (ItemCheck(kamotu_no) == -1)
                        {
                            if (label_flg == "0")
                            {
                                CommonUtils.AlertDialog(view, "", "登録済みです。", null);
                                return;
                            }
                            else
                            {
                                CommonUtils.AlertDialog(view, "", "未登録です。", null);
                                return;
                            }
                        }

                        switch (btvLen)
                        {
                            case 5: // mail
                                if (label_flg == "0")
                                {
                                    txtMailSu.Text = (int.Parse(txtMailSu.Text) + 1).ToString();
                                }
                                else
                                {
                                    txtMailSu.Text = (int.Parse(txtMailSu.Text) - 1).ToString();
                                }

                                break;
                            case 6: // mail
                                if (label_flg == "0")
                                {
                                    txtHazaiSu.Text = (int.Parse(txtHazaiSu.Text) + 1).ToString();
                                }
                                else
                                {
                                    txtHazaiSu.Text = (int.Parse(txtHazaiSu.Text) - 1).ToString();
                                }

                                break;
                            case 7: // idou
                                if (label_flg == "0")
                                {
                                    txtIdouSu.Text = (int.Parse(txtIdouSu.Text) + 1).ToString();
                                }
                                else
                                {
                                    txtIdouSu.Text = (int.Parse(txtIdouSu.Text) - 1).ToString();
                                }

                                break;
                            case 8: // 返品数
                                if (label_flg == "0")
                                {
                                    txtHenpinSu.Text = (int.Parse(txtHenpinSu.Text) + 1).ToString();
                                }
                                else
                                {
                                    txtHenpinSu.Text = (int.Parse(txtHenpinSu.Text) - 1).ToString();
                                }

                                break;
                            default:
                                if (label_flg == "0")
                                {
                                    txtSonotaSu.Text = (int.Parse(txtSonotaSu.Text) + 1).ToString();
                                }
                                else
                                {
                                    txtSonotaSu.Text = (int.Parse(txtSonotaSu.Text) - 1).ToString();
                                }
                                break;
                        
                        }

                        txtSougoSu.Text = (int.Parse(txtMailSu.Text) + (int.Parse(txtHazaiSu.Text) + int.Parse(txtIdouSu.Text)
                            + int.Parse(txtHenpinSu.Text) + int.Parse(txtSonotaSu.Text))).ToString();
                    }

                });
            }
        }
        
        private int ItemCheck(string kamotu_no)
        {
            if (!prefs.GetBoolean("label_flg", false))
            {
                // 登録済みです。
                int idx = arrKamotu.FindIndex(x => x == kamotu_no);
                if (idx == -1)
                {
                    arrKamotu.Add(kamotu_no);
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                // キャンセル
                int idx = arrKamotu.FindIndex(x => x == kamotu_no);
                if (idx != -1)
                {
                    arrKamotu.RemoveAt(idx);
                    return 0;
                }

                return -1;
            }

            return 0;
        }

        private void appendFile(string kamotu_no)
        {
            // レコード作成用　値取得
            SndNohinSyohinKaisyu sndNohinSyohinKaisyu = new SndNohinSyohinKaisyu
            {
                wPackage = "02",
                wTerminalID = "11101",
                wProgramID = prefs.GetString("program_id", "NOH"),
                wSagyosyaCD = prefs.GetString("sagyousya_cd", "99999"),
                wSoukoCD = prefs.GetString("souko_cd", "108"),
                wHaisoDate = "0",
                wBinNo = prefs.GetString("bin_no", ""),
                wCourse = prefs.GetString("course", ""),
                wDriverCD = "",
                wTokuisakiCD = prefs.GetString("tokuisaki_cd", ""),
                wTodokesakiCD = prefs.GetString("todokesaki_cd", ""),
                wKanriNo = "",
                wVendorCd = prefs.GetString("vendor_cd", ""),
                wMateVendorCd = "",
                wSyukaDate = prefs.GetString("haiso_date", ""),
                wButsuryuNo = "",
                wKamotuNo = kamotu_no,
                wMatehan = "",
                wMatehanSu = "0",
                wHHT_no = "11101",
                wNohinKbn = "1", // 商品回収
                wKaisyuKbn = "0",
                wTenkanState = "00", 
                wSakiTokuisakiCD = "",
                wSakiTodokesakiCD = "",
                wNohinDate = prefs.GetString("nohin_date", ""), 
                wNohinTime = prefs.GetString("nohin_time", "")
            };

            SndNohinSyohinKaisyuHelper nohinWorkHelper = new SndNohinSyohinKaisyuHelper();
            nohinWorkHelper.Insert(sndNohinSyohinKaisyu);
        }
    }
}