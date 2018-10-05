using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        private Button btnConfirm, btnMate;
        private List<string> kamotuList;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_tsumikae_Idou_moto, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetTitle("移動元マテハン");

            InitComponent();
            
            btnConfirm.Click += delegate {
                if(menuFlag == 1)
                {
                    StartFragment(FragmentManager, typeof(TsumikaeIdouSakiFragment));
                }
                else if (menuFlag == 3)
                {
                    editor.PutString("from_gyomu", "3");
                    editor.Apply();
                    StartFragment(FragmentManager, typeof(TsumikaeIdouMatehanFragment));
                }
            };
            btnMate.Click += delegate
            {
                editor.PutString("from_gyomu", "1");
                editor.Apply();
                StartFragment(FragmentManager, typeof(TsumikaeIdouMatehanFragment));
            };
            
            
            return view;
        }

        private void InitComponent()
        {
            txtCase = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_case);
            txtOricon = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_oricon);
            txtIdosu = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_idosu);
            txtMail = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_mail);
            txtSonota = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_sonota);
            txtFuteikei = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_futeikei);
            txtHansoku = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_hansoku);
            txtTc = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_tc);
            txtKosu = view.FindViewById<TextView>(Resource.Id.txt_tsumikae_kosu);

            btnConfirm = view.FindViewById<Button>(Resource.Id.confirmButton);
            btnMate = view.FindViewById<Button>(Resource.Id.mateButton);
            
            txtCase.Text = prefs.GetString("case_su", "0");
            txtOricon.Text = prefs.GetString("oricon_su", "0");
            txtIdosu.Text = prefs.GetString("ido_su", "0");
            txtMail.Text = prefs.GetString("mail_su", "0");
            txtSonota.Text = prefs.GetString("sonota_su", "0");
            txtFuteikei.Text = prefs.GetString("futeikei_su", "0");
            txtHansoku.Text = prefs.GetString("hansoku_su", "0");
            txtTc.Text = prefs.GetString("sonota_su", "0");
            txtKosu.Text = prefs.GetString("ko_su", "0");

            souko_cd = prefs.GetString("souko_cd", "");
            kitaku_cd = prefs.GetString("kitaku_cd", "");

            menuFlag = prefs.GetInt("menuFlag", 1);
            kamotuList = new List<string>();

            btnConfirm.Visibility = ViewStates.Invisible;
            btnMate.Visibility = ViewStates.Invisible;
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
                this.Activity.RunOnUiThread(() =>
                {
                    string kamotsu_no = barcodeData.Data;
                    List<string> motomateCdList = prefs.GetStringSet("motoMateCdList", new List<string>()).ToList();

                    // 同じ貨物かを確認
                    if (kamotuList.FindIndex(x => x == kamotsu_no) != -1)
                    {
                        CommonUtils.AlertDialog(view, "", "同一の商品です。", null);
                        return;
                    }

                    if (menuFlag == 1)
                    {
                        // 貨物番号に紐づく情報を取得する
                        IDOU033 idou033 = WebService.RequestIdou033(souko_cd, kitaku_cd, kamotsu_no);

                        // 得意先、届先が一致するかを確認する
                        if ((idou033.tokuisaki_cd == prefs.GetString("tmptokui_cd", "") && idou033.todokesaki_cd == prefs.GetString("tmptodoke_cd", ""))
                            || prefs.GetString("tmptokui_cd", "") == "")
                        {
                            // Do nothing
                        }
                        else
                        {
                            CommonUtils.AlertDialog(view, "エラー", "届先が異なります。", null);
                            return;
                        }

                        // 便情報が一致するかを確認する
                        if (txtKosu.Text != "0")
                        {
                            // 便チェック
                            if (prefs.GetString("bin_no", "0") != idou033.torikomi_bin)
                            {
                                CommonUtils.AlertDialog(view, "エラー", "便が異なります。", null);
                                return;
                            }
                        }

                        //IDOU040 idou040 = WebService.RequestIdou040(pTerminalID, pProgramID, pSagyosyaCD, pSoukoCD, pMotoKamotsuNo, pGyomuKbn);
                        IDOU040 idou040 = new IDOU040();
                        idou040.poRet = "0";
                        if (idou040.poRet == "1")
                        {
                            CommonUtils.AlertDialog(view, "Error", "移動元の貨物Noが見つかりません。", null);
                            return;
                        }


                        SetMatehan(idou033.bunrui, 1);
                        motomateCdList.Add(idou033.matehan);
                        kamotuList.Add(kamotsu_no);

                        editor.PutString("tmptokui_cd", idou033.tokuisaki_cd);
                        editor.PutString("tmptodoke_cd", idou033.todokesaki_cd);
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

                        btnConfirm.Visibility = ViewStates.Visible;
                        btnMate.Visibility = ViewStates.Visible;
                    }
                    else if (menuFlag == 2)
                    {

                        // 貨物番号に紐づく情報を取得する
                        List<IDOU020> idou020List = WebService.RequestIdou020(souko_cd, kitaku_cd, kamotsu_no);

                        if (idou020List.Count == 0)
                        {
                            CommonUtils.AlertDialog(view, "", "移動元の貨物Noが見つかりません。", null);
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

                        }

                        IDOU033 idou033 = WebService.RequestIdou033(souko_cd, kitaku_cd, kamotsu_no);

                        if (idou033.todokesaki_cd == "")
                        {
                            CommonUtils.AlertDialog(view, "", "貨物Noが見つかりません。", null);
                            return;
                        }

                        editor.PutString("tmptokui_cd", idou033.tokuisaki_cd);
                        editor.PutString("tmptodoke_cd", idou033.todokesaki_cd);
                        editor.PutString("tsumi_vendor_cd", idou033.default_vendor);
                        editor.PutString("tsumi_vendor_nm", idou033.vendor_nm);
                        editor.PutString("btvBunrui", idou033.bunrui);
                        editor.PutString("motomate_cd", idou033.matehan);
                        editor.PutString("bin_no", idou033.torikomi_bin);

                        editor.PutStringSet("kamotuList", kamotuList);
                        editor.Apply();

                        StartFragment(FragmentManager, typeof(TsumikaeIdouSakiFragment));
                    }
                    else if (menuFlag == 3)
                    {
                        // 貨物番号に紐づく情報を取得する
                        List<IDOU030> idou030List = WebService.RequestIdou030(souko_cd, kitaku_cd, kamotsu_no);

                        if (idou030List.Count == 0)
                        {
                            CommonUtils.AlertDialog(view, "", "移動元の貨物Noが見つかりません。", null);
                            return;
                        }
                        else
                        {
                            kamotuList.Add(kamotsu_no);
                        }

                        SetMatehan(idou030List[0].bunrui, int.Parse(idou030List[0].cnt));

                        IDOU033 idou033 = WebService.RequestIdou033(souko_cd, kitaku_cd, kamotsu_no);

                        if (idou033.todokesaki_cd == "")
                        {
                            CommonUtils.AlertDialog(view, "", "貨物Noが見つかりません。", null);
                            return;
                        }

                        editor.PutString("tmptokui_cd", idou033.tokuisaki_cd);
                        editor.PutString("tmptodoke_cd", idou033.todokesaki_cd);
                        editor.PutString("tsumi_vendor_cd", idou033.default_vendor);
                        editor.PutString("tsumi_vendor_nm", idou033.vendor_nm);
                        editor.PutString("btvBunrui", idou033.bunrui);
                        editor.PutString("motomate_cd", idou033.matehan);
                        editor.PutString("bin_no", idou033.torikomi_bin);
                        editor.Apply();

                        btnConfirm.Visibility = ViewStates.Visible;
                    }
                });
            }
        }
        
        private void SetMatehan(string bunrui, int addValue)
        {
            string addedValue = ""; //加算した値を保存

            switch (bunrui) {
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
                    addedValue = (int.Parse(txtMail.Text) + addValue).ToString();
                    editor.PutString("sonota_su", addedValue);
                    editor.PutString("sk_sonota_su", addedValue);
                    txtSonota.Text = addedValue.ToString();
                    break;
                case "07":
                    addedValue = (int.Parse(txtMail.Text) + addValue).ToString();
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
        }
        
    }
}