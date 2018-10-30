using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using Com.Beardedhen.Androidbootstrap;
using Com.Densowave.Bhtsdk.Barcode;
using HHT.Resources.Model;
using Newtonsoft.Json;

namespace HHT
{
    public class TsumikaeIdouMotoFragment : BaseFragment
    {
        private View view;
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        private int menuFlag;
        private string souko_cd, kitaku_cd;
        private TextView txtCase, txtOricon, txtIdosu, txtMail, txtSonota
            , txtFuteikei, txtHansoku, txtTc, txtKosu;

        private BootstrapButton btnConfirm, btnMate;
        private List<string> kamotuList;
        private List<string> motomateCdList;

        private List<Ido> motoMateInfo;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_tsumikae_Idou_moto, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            txtCase = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_case);
            txtOricon = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_oricon);
            txtIdosu = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_idosu);
            txtMail = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_mail);
            txtSonota = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_sonota);
            txtFuteikei = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_futeikei);
            txtHansoku = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_hansoku);
            txtTc = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_tc);
            txtKosu = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_kosu);

            // 連続バーコード読み取り時に使う変数初期化
            editor.PutString("tmptokui_cd", "");
            editor.PutString("tmptodoke_cd", "");
            editor.Apply();

            // 確定ボタン
            btnConfirm = view.FindViewById<BootstrapButton>(Resource.Id.confirmButton);
            btnConfirm.Click += delegate {
                if (menuFlag == 1)
                {
                    Bundle bundle = new Bundle();
                    bundle.PutString("motoInfo", JsonConvert.SerializeObject(motoMateInfo));
                    StartFragment(FragmentManager, typeof(TsumikaeIdouSakiFragment), bundle);
                }
                else if (menuFlag == 3)
                {
                    editor.PutString("from_gyomu", "3");
                    editor.Apply();
                    Bundle bundle = new Bundle();
                    bundle.PutString("motoInfo", JsonConvert.SerializeObject(motoMateInfo));
                    StartFragment(FragmentManager, typeof(TsumikaeIdouMatehanFragment), bundle);
                }
            };

            // マテハンボタン
            btnMate = view.FindViewById<BootstrapButton>(Resource.Id.mateButton);
            btnMate.Click += delegate
            {
                Bundle bundle = new Bundle();
                bundle.PutString("motoInfo", JsonConvert.SerializeObject(motoMateInfo));

                editor.PutString("from_gyomu", "1");
                editor.Apply();
                StartFragment(FragmentManager, typeof(TsumikaeIdouMatehanFragment), bundle);
            };

            // 初期値設定
            SetTitle("移動元マテハン");

            souko_cd = prefs.GetString("souko_cd", "");
            kitaku_cd = prefs.GetString("kitaku_cd", "");

            menuFlag = prefs.GetInt("menuFlag", 1); // メニュー区分

            txtCase.Text = "0";
            txtOricon.Text = "0";
            txtIdosu.Text = "0";
            txtMail.Text = "0";
            txtSonota.Text = "0";
            txtFuteikei.Text = "0";
            txtHansoku.Text = "0";
            txtTc.Text = "0";
            txtKosu.Text = "0";

            kamotuList = new List<string>();
            motomateCdList = prefs.GetStringSet("motoMateCdList", new List<string>()).ToList();
            motoMateInfo = new List<Ido>();

            btnConfirm.Enabled = false;
            btnMate.Enabled = false;

            if (menuFlag != 1) btnMate.Visibility = ViewStates.Gone;

            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Enter || keycode == Keycode.F4)
            {
                if (txtKosu.Text != "0")
                {
                    if (menuFlag == 1)
                    {
                        StartFragment(FragmentManager, typeof(TsumikaeIdouSakiFragment));
                    }
                    else if (menuFlag == 3)
                    {
                        editor.PutString("from_gyomu", "3");
                        editor.Apply();
                        StartFragment(FragmentManager, typeof(TsumikaeIdouMatehanFragment));
                    }
                }
            }
            else if (keycode == Keycode.F3)
            {
                if (txtKosu.Text != "0" && menuFlag == 1)
                {
                    editor.PutString("from_gyomu", "1");
                    editor.Apply();
                    StartFragment(FragmentManager, typeof(TsumikaeIdouMatehanFragment));
                }
            }
            else if (keycode == Keycode.Back)
            {
                editor.PutString("case_su", "0");
                editor.PutString("sk_case_su", "0");
                editor.PutString("oricon_su", "0");
                editor.PutString("sk_oricon_su", "0");
                editor.PutString("ido_su", "0");
                editor.PutString("sk_ido_su", "0");
                editor.PutString("mail_su", "0");
                editor.PutString("sk_mail_su", "0");
                editor.PutString("sonota_su", "0");
                editor.PutString("sk_sonota_su", "0");
                editor.PutString("futeikei_su", "0");
                editor.PutString("sk_futeikei_su", "0");
                editor.PutString("hansoku_su", "0");
                editor.PutString("sk_hansoku_su", "0");
                editor.PutString("sonota_su", "0");
                editor.PutString("sk_sonota_su", "0");

                editor.PutString("ko_su", "0");
                editor.PutString("sk_ko_su", "0");

                editor.PutString("motok_su", "0");

                editor.PutStringSet("kamotuList", new List<string>());
                editor.Apply();

            }
            
            return true;
        }

        public override void OnBarcodeDataReceived(BarcodeDataReceivedEvent_ dataReceivedEvent)
        {
            IList<BarcodeDataReceivedEvent_.BarcodeData_> listBarcodeData = dataReceivedEvent.BarcodeData;

            foreach (BarcodeDataReceivedEvent_.BarcodeData_ barcodeData in listBarcodeData)
            {
                string kamotsu_no = barcodeData.Data;

                // スキャン重複チェック
                if (kamotuList.FindIndex(x => x == kamotsu_no) != -1)
                {
                    Activity.RunOnUiThread(() => {
                        ShowDialog("報告", "同一の商品です。", () => { });
                    });
                    return;
                }

                if (menuFlag == 1) // 単品移動
                {
                    SettingTanpinMotoInfo(kamotsu_no);
                }
                else if (menuFlag == 2) // 全品移動
                {
                    SettingZenpinMotoInfo(kamotsu_no);
                }
                else if (menuFlag == 3) // マテハン移動
                {
                    SettingMateInfo(kamotsu_no);
                }
            }
        }

        private void SettingTanpinMotoInfo(string kamotsu_no)
        {
            try
            {
                // 貨物番号に紐づく情報を取得する
                IDOU033 idou033 = WebService.RequestIdou033(souko_cd, kitaku_cd, kamotsu_no);

                // 得意先、届先が一致するかを確認する
                if ((idou033.tokuisaki_cd == prefs.GetString("tmptokui_cd", "")
                    && idou033.todokesaki_cd == prefs.GetString("tmptodoke_cd", ""))
                    || prefs.GetString("tmptokui_cd", "") == "")
                {
                    // Do nothing
                }
                else
                {
                    ShowDialog("エラー", "届先が異なります。", () => { });
                    return;
                }

                // 便情報が一致するかを確認する
                if (txtKosu.Text != "0")
                {
                    // 便チェック
                    if (prefs.GetString("bin_no", "0") != idou033.torikomi_bin)
                    {
                        ShowDialog("エラー", "便が異なります。", () => { });
                        return;
                    }
                }

                Dictionary<string, string> param = new Dictionary<string, string>
                {
                    {"pTerminalID", prefs.GetString("terminal_id","") },
                    {"pProgramID", "IDO" },
                    {"pSagyosyaCD", prefs.GetString("sagyousya_cd","") },
                    {"pSoukoCD", souko_cd },
                    {"pMotoKamotsuNo", kamotsu_no },
                    {"pGyomuKbn", "04" }
                };

                IDOU040 idou040 = WebService.RequestIdou040(param);
                if (idou040.poRet == "1")
                {
                    ShowDialog("エラー", "移動元の貨物Noが見つかりません。", () => { });
                    return;
                }

                SetMatehan(idou033.bunrui, 1);
                motomateCdList.Add(idou033.matehan);
                kamotuList.Add(kamotsu_no);
                motoMateInfo.Add(new Ido { kamotsuNo = kamotsu_no, motoMateCode = idou033.matehan });

                editor.PutString("tmptokui_cd", idou033.tokuisaki_cd);
                editor.PutString("tmptodoke_cd", idou033.todokesaki_cd);
                editor.PutString("vendor_cd", idou033.default_vendor);
                editor.PutString("vendor_nm", idou033.vendor_nm);
                editor.PutString("tsumi_vendor_cd", idou033.default_vendor);
                editor.PutString("tsumi_vendor_nm", idou033.vendor_nm);
                editor.PutString("btvBunrui", idou033.bunrui);
                editor.PutString("motomate_cd", idou033.matehan);
                editor.PutString("bin_no", idou033.torikomi_bin);
                editor.PutString("motok_su", txtKosu.Text);
                editor.PutString("ko_su", txtKosu.Text);
                editor.PutString("sk_ko_su", txtKosu.Text);
                editor.PutStringSet("motomateCdList", motomateCdList);
                editor.PutStringSet("kamotuList", kamotuList);
                editor.Apply();

                Activity.RunOnUiThread(() => {
                    btnConfirm.Enabled = true;
                    btnMate.Enabled = true;
                });
            }
            catch
            {
                ShowDialog("エラー", "移動元の貨物Noが見つかりません。", () => { });
                return;
            }
            
        }

        
        private void SettingZenpinMotoInfo(string kamotsu_no)
        {
            try
            {
                // 貨物番号に紐づく情報を取得する
                List<IDOU020> idou020List = WebService.RequestIdou020(souko_cd, kitaku_cd, kamotsu_no);

                if (idou020List.Count == 0)
                {
                    ShowDialog("エラー", "移動元の貨物Noが見つかりません。", () => { });
                    return;
                }
                else
                {
                    kamotuList.Add(kamotsu_no);
                }

                foreach (IDOU020 idou020 in idou020List)
                {
                    string btvBunrui = idou020.bunrui;
                    string btvBunruiNm = idou020.bunrui_nm;
                    string btvMatehan = idou020.matehan;
                    int btvMateSu = int.Parse(idou020.cnt);
                    string btvBaraMatehan = idou020.bara_matehan;

                    SetMatehan(btvBunrui, btvMateSu);
                    motoMateInfo.Add(new Ido { kamotsuNo = kamotsu_no, motoMateCode = idou020.matehan });

                }

                IDOU033 idou033 = WebService.RequestIdou033(souko_cd, kitaku_cd, kamotsu_no);

                if (idou033.todokesaki_cd == "")
                {
                    ShowDialog("エラー", "貨物Noが見つかりません。", () => { });
                    return;
                }

                editor.PutString("tmptokui_cd", idou033.tokuisaki_cd);
                editor.PutString("tmptodoke_cd", idou033.todokesaki_cd);
                editor.PutString("tsumi_vendor_cd", idou033.default_vendor);
                editor.PutString("tsumi_vendor_nm", idou033.vendor_nm);
                editor.PutString("vendor_cd", idou033.default_vendor);
                editor.PutString("vendor_nm", idou033.vendor_nm);
                editor.PutString("btvBunrui", idou033.bunrui);
                editor.PutString("motomate_cd", idou033.matehan);
                editor.PutString("bin_no", idou033.torikomi_bin);

                editor.PutStringSet("kamotuList", kamotuList);
                editor.Apply();

                Bundle bundle = new Bundle();
                bundle.PutString("motoInfo", JsonConvert.SerializeObject(motoMateInfo));
                StartFragment(FragmentManager, typeof(TsumikaeIdouSakiFragment), bundle);
            }
            catch
            {
                ShowDialog("エラー", "貨物Noが見つかりません。", () => { });
                return;
            }
        }


        private void SettingMateInfo(string kamotsu_no)
        {
            try
            {
                // 貨物番号に紐づく情報を取得する
                List<IDOU030> idou030List = WebService.RequestIdou030(souko_cd, kitaku_cd, kamotsu_no);

                if (idou030List.Count == 0)
                {
                    ShowDialog("エラー", "移動元の貨物Noが見つかりません。", () => { });
                    return;
                }
                else
                {
                    kamotuList.Add(kamotsu_no);
                }

                foreach (IDOU030 idou030 in idou030List)
                {
                    SetMatehan(idou030.bunrui, int.Parse(idou030.cnt));
                    motoMateInfo.Add(new Ido { kamotsuNo = kamotsu_no, motoMateCode = idou030.matehan });
                }

                IDOU033 idou033 = WebService.RequestIdou033(souko_cd, kitaku_cd, kamotsu_no);

                if (idou033.todokesaki_cd == "")
                {
                    ShowDialog("エラー", "貨物Noが見つかりません。", () => { });
                    return;
                }

                editor.PutString("tmptokui_cd", idou033.tokuisaki_cd);
                editor.PutString("tmptodoke_cd", idou033.todokesaki_cd);
                editor.PutString("tsumi_vendor_cd", idou033.default_vendor);
                editor.PutString("tsumi_vendor_nm", idou033.vendor_nm);
                editor.PutString("vendor_cd", idou033.default_vendor);
                editor.PutString("vendor_nm", idou033.vendor_nm);
                editor.PutString("btvBunrui", idou033.bunrui);
                editor.PutString("motomate_cd", idou033.matehan);
                editor.PutString("bin_no", idou033.torikomi_bin);
                editor.Apply();

                Activity.RunOnUiThread(() => {
                    btnConfirm.Enabled = true;
                });
            }
            catch
            {
                ShowDialog("エラー", "貨物Noが見つかりません。", () => { });
                return;
            }
        }

        private void SetMatehan(string bunrui, int addValue)
        {
            string addedValue = ""; //加算した値を保存

            // テキスト修正はUiThreadで動かないとエラーになる
            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(() =>
                {
                    switch (bunrui)
                    {
                        case "01":
                            addedValue = (int.Parse(txtCase.Text) + addValue).ToString();
                            editor.PutString("case_su", addedValue);
                            editor.PutString("sk_case_su", addedValue);
                            txtCase.Text = addedValue.ToString();
                            break;
                        case "02":
                            addedValue = (int.Parse(txtOricon.Text) + addValue).ToString();
                            editor.PutString("oricon_su", addedValue);
                            editor.PutString("sk_oricon_su", addedValue);
                            txtOricon.Text = addedValue.ToString();
                            break; // case 03は存在しない
                        case "04":
                            addedValue = (int.Parse(txtIdosu.Text) + addValue).ToString();
                            editor.PutString("ido_su", addedValue);
                            editor.PutString("sk_ido_su", addedValue);
                            txtIdosu.Text = addedValue.ToString();
                            break;
                        case "05":
                            addedValue = (int.Parse(txtMail.Text) + addValue).ToString();
                            editor.PutString("mail_su", addedValue);
                            editor.PutString("sk_mail_su", addedValue);
                            txtMail.Text = addedValue.ToString();
                            break;
                        case "06":
                            addedValue = (int.Parse(txtSonota.Text) + addValue).ToString();
                            editor.PutString("sonota_su", addedValue);
                            editor.PutString("sk_sonota_su", addedValue);
                            txtSonota.Text = addedValue.ToString();
                            break;
                        case "07":
                            addedValue = (int.Parse(txtFuteikei.Text) + addValue).ToString();
                            editor.PutString("futeikei_su", addedValue);
                            editor.PutString("sk_futeikei_su", addedValue);
                            txtFuteikei.Text = addedValue.ToString();
                            break;
                        // case 08は存在しない
                        case "09":
                            addedValue = (int.Parse(txtHansoku.Text) + addValue).ToString();
                            editor.PutString("hansoku_su", addedValue);
                            editor.PutString("sk_hansoku_su", addedValue);
                            txtHansoku.Text = addedValue.ToString();
                            break;
                        default:
                            addedValue = (int.Parse(txtSonota.Text) + addValue).ToString();
                            editor.PutString("sonota_su", addedValue);
                            editor.PutString("sk_sonota_su", addedValue);
                            txtSonota.Text = addedValue.ToString();
                            break;

                    }

                    txtKosu.Text = (int.Parse(txtKosu.Text) + addValue).ToString();
                    editor.PutString("ko_su", txtKosu.Text);
                    editor.PutString("sk_ko_su", txtKosu.Text);
                    editor.Apply();

                }
                );

            }
            )).Start();
        }
    }
}