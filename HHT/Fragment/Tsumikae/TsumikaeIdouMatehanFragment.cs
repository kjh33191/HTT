using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using HHT.Resources.Model;
using static Android.Widget.AdapterView;
using Android.Content;
using Android.Preferences;
using Com.Beardedhen.Androidbootstrap;

namespace HHT
{
    public class TsumikaeIdouMatehanFragment : BaseFragment
    {
        private View view;
        private List<KOSU200> matehanList;
        private EditText etMantanVendor;
        private TextView txtVenderName;
        private ListView listView;

        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        private List<Ido> motoInfoList;
        private int menuFlag;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            SetTitle("入荷検品");
            SetFooterText("");

            view = inflater.Inflate(Resource.Layout.fragment_tsumikae_idou_matehan, container, false);

            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();
            menuFlag = prefs.GetInt("menuFlag", 1);

            motoInfoList = JsonConvert.DeserializeObject<List<Ido>>(Arguments.GetString("motoInfo"));

            etMantanVendor = view.FindViewById<EditText>(Resource.Id.et_mantan_vendor);
            etMantanVendor.KeyPress += (sender, e) => {
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    e.Handled = true;
                    CommonUtils.HideKeyboard(Activity);
                    SetMatehanList();
                }
                else
                {
                    e.Handled = false;
                }
            };

            txtVenderName = view.FindViewById<TextView>(Resource.Id.vendorName);
            txtVenderName.Text = prefs.GetString("vendor_nm", "");

            BootstrapButton vendorSearchButton = view.FindViewById<BootstrapButton>(Resource.Id.vendorSearch);
            vendorSearchButton.Click += delegate { StartFragment(FragmentManager, typeof(KosuVendorAllSearchFragment)); };

            // ベンダー別マテハンコード取得
            string vendorCode = prefs.GetString("vendor_cd", ""); // prefs.GetString("vendor_cd", "");
            etMantanVendor.Text = vendorCode;
            
            listView = view.FindViewById<ListView>(Resource.Id.lv_matehanList);
            listView.ItemClick += (object sender, ItemClickEventArgs e) =>
            {
                SelectListViewItem(e.Position);
            };

            SetMatehanList();


            return view;
        }

        private void SetMatehanList()
        {
            string venderName = WebService.RequestKosu220(etMantanVendor.Text);
            txtVenderName.Text = venderName;
            // ベンダーコードがみつかりません。

            // ベンダー別マテハンコード取得
            matehanList = WebService.RequestKosu200(etMantanVendor.Text);

            List<string> temp = new List<string>();
            int count = 1;
            foreach (KOSU200 matehan in matehanList)
            {
                temp.Add(count + ".  " + matehan.matehan_nm);
                count++;
            }

            ArrayAdapter<String> adapter = new ArrayAdapter<string>(Activity, Resource.Layout.adapter_list_matehan, temp);
            listView.Adapter = adapter;
        }

        public override void OnResume()
        {
            base.OnResume();
            etMantanVendor.Text = prefs.GetString("vendor_cd", "");
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Num1)
            {
                SelectListViewItem(0);
            }
            else if (keycode == Keycode.Num2)
            {
                SelectListViewItem(1);
            }
            else if (keycode == Keycode.Num3)
            {
                SelectListViewItem(2);
            }
            else if (keycode == Keycode.Num4)
            {
                SelectListViewItem(3);
            }

            return true;
        }

        public void SelectListViewItem(int index)
        {
            if (matehanList.Count > index)
            {
                string msg = matehanList[index].matehan_nm + "でよろしいですか？";
                CommonUtils.AlertConfirm(view, "確認", msg, (flag) =>
                {
                    if (flag)
                    {
                        editor.PutString("mateno", matehanList[index].matehan_cd);
                        editor.Apply();

                        string sin_matehan = "";
                        string mateno = matehanList[index].matehan_cd;
                        string hht_no = "99";
                        string tokuiCd = prefs.GetString("tmptokui_cd", "");
                        string todokeCd = prefs.GetString("tmptodoke_cd", "");
                        string mate_renban = GetMateRandomNo();

                        sin_matehan = mateno + hht_no + tokuiCd + todokeCd + mate_renban;

                        if (menuFlag == 3)
                        {
                            Dictionary<string, string> param = new Dictionary<string, string>
                            {
                                {"pTerminalID", "432660068" },
                                {"pProgramID", "IDO" },
                                {"pSagyosyaCD", prefs.GetString("pSagyosyaCD", "") },
                                {"pSoukoCD", prefs.GetString("souko_cd", "") },
                                {"pKitakuCD", prefs.GetString("kitaku_cd", "") },
                                {"pMotoMatehan", motoInfoList[0].motoMateCode },
                                {"pSakiMatehan", sin_matehan },
                                {"pGyomuKbn", "04" },
                                {"pVendorCd", prefs.GetString("tsumi_vendor_cd", "") }
                            };
                            
                            IDOU070 idou070 = WebService.RequestIdou070(param);

                            if (idou070.poRet != "0")
                            {
                                CommonUtils.AlertDialog(view, "", idou070.poMsg, null);
                                return;
                            }

                            // 積替処理完了
                            CommonUtils.AlertDialog(view, "    =メッセージ=    ", "移動処理が\n完了しました。", () =>
                                FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(0).Id, 0)
                            );
                        }
                        else
                        {
                            // 単品マテハン登録
                            
                            if (mate_renban.Length > 0)
                            {
                                foreach(Ido motoInfo in motoInfoList)
                                {
                                    Dictionary<string, string> param = new Dictionary<string, string>
                                    {
                                        {"pTerminalID", "432660068" },
                                        {"pProgramID", "IDO" },
                                        {"pSagyosyaCD", prefs.GetString("pSagyosyaCD", "") },
                                        {"pSoukoCD", prefs.GetString("souko_cd", "") },
                                        {"pMotoKamotsuNo", motoInfo.kamotsuNo },
                                        {"pSakiMatehan", sin_matehan },
                                        {"pGyomuKbn", "04" },
                                        {"pVendorCd", prefs.GetString("tsumi_vendor_cd", "") }
                                    };

                                    IDOU090 idou090 = WebService.RequestIdou090(param);
                                    if(idou090.poMsg != "")
                                    {
                                        CommonUtils.AlertDialog(view, "", idou090.poMsg, null);
                                        return;
                                    }
                                }
                                
                                // 積替処理完了
                                CommonUtils.AlertDialog(view, "    =メッセージ=    ", "移動処理が\n完了しました。", ()=>
                                    FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(0).Id, 0)
                                );
                            }
                        }
                    }
                }
                );
            }
        }

        private string GetMateRandomNo()
        {
            Dictionary<string, string> param = new Dictionary<string, string>
                            {
                                {"pTerminalID", "432660068" },
                                {"pProgramID", "IDO" },
                                {"pSagyosyaCD", prefs.GetString("pSagyosyaCD", "") },
                                {"pSoukoCD", prefs.GetString("souko_cd", "") },
                                {"pGyomuKbn", "04" }
                            };

            IDOU080 iDOU080 = WebService.RequestIdou080(param);
            return iDOU080.poMsg;
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
 