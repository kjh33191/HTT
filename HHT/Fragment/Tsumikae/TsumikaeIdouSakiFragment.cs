using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Beardedhen.Androidbootstrap;
using Com.Densowave.Bhtsdk.Barcode;
using HHT.Resources.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace HHT
{
    public class TsumikaeIdouSakiFragment : BaseFragment
    {
        private readonly string TAG = "TsumikaeIdouSakiFragment";
        private View view;
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        private int menuFlag, btvQty, btvScnFlg;
        private string souko_cd, kitaku_cd;
        private TextView txtCase, txtOricon, txtIdosu, txtMail, txtSonota , txtFuteikei, txtHansoku, txtTc, txtKosu;
        private BootstrapButton btnConfirm;

        private List<string> motokamotuList;
        private IDOU033 kamotuInfo;

        private string sakiKamotsuNo;
        private string sakiMatehanCd;
        List<Ido> motoInfoList;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_tsumikae_Idou_saki, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetTitle("移動先マテハン");

            txtCase = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_case);
            txtOricon = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_oricon);
            txtIdosu = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_idosu);
            txtMail = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_mail);
            txtSonota = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_sonota);
            txtFuteikei = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_futeikei);
            txtHansoku = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_hansoku);
            txtTc = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_tc);
            txtKosu = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_kosu);

            btnConfirm = view.FindViewById<BootstrapButton>(Resource.Id.confirmButton);

            txtCase.Text = prefs.GetString("sk_case_su", "0");
            txtOricon.Text = prefs.GetString("sk_oricon_su", "0");
            txtIdosu.Text = prefs.GetString("sk_ido_su", "0");
            txtMail.Text = prefs.GetString("sk_mail_su", "0");
            txtSonota.Text = prefs.GetString("sk_sonota_su", "0");
            txtFuteikei.Text = prefs.GetString("sk_futeikei_su", "0");
            txtHansoku.Text = prefs.GetString("sk_hansoku_su", "0");
            txtTc.Text = prefs.GetString("sk_sonota_su", "0");
            txtKosu.Text = prefs.GetString("sk_ko_su", "0");
            
            souko_cd = prefs.GetString("souko_cd", "");
            kitaku_cd = prefs.GetString("kitaku_cd", "");

            menuFlag = prefs.GetInt("menuFlag", 1);
            btvQty = 0;
            btvScnFlg = 0;
            
            btnConfirm.Text = menuFlag != 3 ? "完了" : "確定";
            btnConfirm.Click += delegate{ if(btvScnFlg > 0) { CompleteIdou(); }};

            Bundle bundle = Arguments;
            motoInfoList = JsonConvert.DeserializeObject<List<Ido>>(bundle.GetString("motoInfo"));
            
            if (menuFlag == 1 || menuFlag == 2)
            {
                btnConfirm.Enabled = false;
            }

            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F1)
            {
                if (btvScnFlg > 0)
                {
                    CompleteIdou();
                }
            }
            else if (keycode == Keycode.Enter)
            {
                if (btvQty > 0)
                {
                    CompleteIdou();
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
                    string sakikamotu_no = barcodeData.Data;

                    if (btvScnFlg > 0)
                    {
                        ShowDialog("エラー", "既にスキャン済みです", () => { });
                        return;
                    }
                    
                    if (menuFlag == 1)
                    {
                        // 単品移動

                        // 移動先バーコードチェック
                        if (CheckScanNo(sakikamotu_no) == false) return;

                        // 移動先マテハンの貨物リストを取得する。
                        List<IDOU010> idou010List = WebService.RequestIdou010(souko_cd, kitaku_cd, sakikamotu_no);

                        if (idou010List.Count == 0)
                        {
                            ShowDialog("報告", "表示データがありません", () => { });
                            return;
                        }
                        
                        // 遷移先マテハン設定
                        foreach (IDOU010 idou010 in idou010List)
                        {
                            if (idou010.matehan.Substring(2) == idou010.matehan)
                            {
                                ShowDialog("エラー", "バラへの移動は出来ません", () => { });
                                return;
                            }

                            if (prefs.GetString("motomate_cd", "") == idou010.matehan)
                            {
                                ShowDialog("エラー", "同一のマテハンです", () => { });
                                return;
                            }

                            SetMatehan(idou010.bunrui, int.Parse(idou010.cnt));
                        }

                        sakiMatehanCd = idou010List[0].matehan;
                        sakiKamotsuNo = sakikamotu_no;
                        //sakikamotu_no
                        btnConfirm.Enabled = true;
                    }
                    else if (menuFlag == 2)
                    {
                        // 全品移動

                        // 移動先バーコードチェック
                        if (CheckScanNo(sakikamotu_no) == false) return;

                        // 移動先マテハンの貨物リストを取得する。
                        List<IDOU020> idou020List = WebService.RequestIdou020(souko_cd, kitaku_cd, sakikamotu_no);

                        if(idou020List.Count == 0)
                        {
                            ShowDialog("報告", "表示データがありません", () => { });
                            return;
                        }

                        // 遷移先マテハン設定
                        foreach (IDOU020 idou020 in idou020List)
                        {
                            if(idou020.matehan.Substring(0,2) == idou020.bara_matehan)
                            {
                                ShowDialog("エラー", "バラへの移動は出来ません", () => { });
                                return;
                            }

                            if (prefs.GetString("motomate_cd", "") == idou020.matehan)
                            {
                                ShowDialog("エラー", "同一のマテハンです", () => { });
                                return;
                            }
                            
                            SetMatehan(idou020.bunrui, int.Parse(idou020.cnt));
                        }

                        sakiKamotsuNo = sakikamotu_no;
                        btnConfirm.Enabled = true;
                    }

                    btnConfirm.Visibility = ViewStates.Visible;
                    btvScnFlg = 1;
                });
            }
        }

        private bool CheckScanNo(string kamotsu_no)
        {
            try { 
                // 移動元と移動先のマテハンが同じの場合
                motokamotuList = prefs.GetStringSet("kamotuList", new List<string>()).ToList();
                if (motokamotuList.FindIndex(x => x == kamotsu_no) != -1)
                {
                    ShowDialog("エラー", "同一のマテハンです。", () => { });
                    return false;
                }

                // 貨物番号に紐づく情報を取得する
                kamotuInfo = WebService.RequestIdou033(souko_cd, kitaku_cd, kamotsu_no);

                // 得意先、届先が一致するかを確認する
                if ((kamotuInfo.tokuisaki_cd == prefs.GetString("tmptokui_cd", "") && kamotuInfo.todokesaki_cd == prefs.GetString("tmptodoke_cd", ""))
                    || prefs.GetString("tmptokui_cd", "") == "")
                {
                    // Do nothing
                }
                else
                {
                    ShowDialog("エラー", "届先が異なります。", () => { });
                    return false;
                }

                // 便情報が一致するかを確認する
                if (txtKosu.Text != "0")
                {
                    // 便チェック
                    if (prefs.GetString("bin_no", "0") != kamotuInfo.torikomi_bin)
                    {
                        ShowDialog("エラー", "便が異なります。", () => { });
                        return false;
                    }
                }
            }
            catch
            {
                ShowDialog("エラー", "例外エラーが発生しました。", () => { });
                Log.Debug(TAG, "CheckScanNo　メソッドでエラー発生");
                return false;
            }

            return true;
        }
        
        private void SetMatehan(string bunrui, int addValue)
        {
            string addedValue = ""; //加算した値を保存

            switch (bunrui)
            {
                case "01":
                    addedValue = (int.Parse(txtCase.Text) + addValue).ToString();
                    editor.PutString("sk_case_su", addedValue);
                    txtCase.Text = addedValue.ToString();
                    break;
                case "02":
                    addedValue = (int.Parse(txtOricon.Text) + addValue).ToString();
                    editor.PutString("sk_oricon_su", addedValue);
                    txtOricon.Text = addedValue.ToString();
                    break; // case 03は存在しない
                case "04":
                    addedValue = (int.Parse(txtIdosu.Text) + addValue).ToString();
                    editor.PutString("sk_ido_su", addedValue);
                    txtIdosu.Text = addedValue.ToString();
                    break;
                case "05":
                    addedValue = (int.Parse(txtMail.Text) + addValue).ToString();
                    editor.PutString("sk_mail_su", addedValue);
                    txtMail.Text = addedValue.ToString();
                    break;
                case "06":
                    addedValue = (int.Parse(txtSonota.Text) + addValue).ToString();
                    editor.PutString("sk_sonota_su", addedValue);
                    txtSonota.Text = addedValue.ToString();
                    break;
                case "07":
                    addedValue = (int.Parse(txtFuteikei.Text) + addValue).ToString();
                    editor.PutString("sk_futeikei_su", addedValue);
                    txtFuteikei.Text = addedValue.ToString();
                    break;
                // case 08は存在しない
                case "09":
                    addedValue = (int.Parse(txtHansoku.Text) + addValue).ToString();
                    editor.PutString("sk_hansoku_su", addedValue);
                    txtHansoku.Text = addedValue.ToString();
                    break;
                default:
                    addedValue = (int.Parse(txtSonota.Text) + addValue).ToString();
                    editor.PutString("sk_sonota_su", addedValue);
                    txtSonota.Text = addedValue.ToString();
                    break;
            }

            txtKosu.Text = (int.Parse(txtKosu.Text) + addValue).ToString();
            editor.PutString("ko_su", txtKosu.Text);
            editor.Apply();
        }
        
        private void CompleteIdou()
        {
            List<string> motomateCdList = prefs.GetStringSet("motoMateCdList", new List<string>()).ToList();
            string tsumi_vendor_cd = prefs.GetString("tsumi_vendor_cd", "");

            if (menuFlag == 1)
            {
                foreach (Ido motoInfo in motoInfoList)
                {
                    Dictionary<string, string> param = new Dictionary<string, string>
                    {
                        {"pTerminalID", prefs.GetString("terminal_id","") },
                        {"pProgramID", "IDO" },
                        {"pSagyosyaCD", prefs.GetString("pSagyosyaCD", "") },
                        {"pSoukoCD", souko_cd },
                        {"pMotoKamotsuNo", motoInfo.kamotsuNo },
                        {"pSakiKamotsuNo", sakiKamotsuNo },
                        {"pGyomuKbn", "04" },
                        {"pVendorCd", tsumi_vendor_cd }
                    };
                    
                    IDOU050 idou050 = WebService.RequestIdou050(param);

                    if (idou050.poRet != "0")
                    {
                        ShowDialog("エラー", idou050.poMsg, () => { });
                        return;
                    }
                }

                ShowDialog("報告", "移動処理が\n完了しました。", () => { BackToMainMenu(); });

            }
            else if (menuFlag == 2)
            {
                Dictionary<string, string> param = new Dictionary<string, string>
                {
                    {"pTerminalID", prefs.GetString("terminal_id","") },
                    {"pProgramID", "IDO" },
                    {"pSagyosyaCD", prefs.GetString("pSagyosyaCD", "") },
                    {"pSoukoCD", souko_cd },
                    {"pMotoMatehan", motoInfoList[0].motoMateCode },
                    {"pSakiKamotsuNo", sakiKamotsuNo },
                    {"pGyomuKbn", "04" },
                    {"pVendorCd", tsumi_vendor_cd }
                };
                
                IDOU060 idou060 = WebService.RequestIdou060(param);
                if(idou060.poMsg != "")
                {
                    ShowDialog("エラー", idou060.poMsg, () => { });
                    return;
                }

                ShowDialog("報告", "移動処理が\n完了しました。", () => { BackToMainMenu(); });
            }
        }

        private void BackToMainMenu()
        {
            string menu_kbn = prefs.GetString("menu_kbn", "");
            string driver_nm = prefs.GetString("driver_nm", "");
            string souko_cd = prefs.GetString("souko_cd", "");
            string souko_nm = prefs.GetString("souko_nm", "");
            string driver_cd = prefs.GetString("driver_cd", "");
            string kitaku_cd = prefs.GetString("kitaku_cd", "");
            string def_tokuisaki_cd = prefs.GetString("def_tokuisaki_cd", "");
            string tsuhshin_kbn = prefs.GetString("tsuhshin_kbn", "");
            string souko_kbn = prefs.GetString("souko_kbn", "");

            editor.Clear();
            editor.Commit();

            editor.PutString("menu_kbn", menu_kbn);
            editor.PutString("driver_nm", driver_nm);
            editor.PutString("souko_cd", souko_cd);
            editor.PutString("souko_nm", souko_nm);
            editor.PutString("driver_cd", driver_cd);
            editor.PutString("kitaku_cd", kitaku_cd);
            editor.PutString("def_tokuisaki_cd", def_tokuisaki_cd);
            editor.PutString("tsuhshin_kbn", tsuhshin_kbn);
            editor.PutString("souko_kbn", souko_kbn);
            editor.Apply();

            FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(0).Id, 0);
        }


    }
}