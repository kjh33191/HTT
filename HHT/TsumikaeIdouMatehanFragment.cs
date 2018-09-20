using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using HHT.Resources.Model;
using static Android.Widget.AdapterView;
using Android.Content;
using Android.Preferences;

namespace HHT
{
    public class TsumikaeIdouMatehanFragment : BaseFragment
    {
        private View view;
        private List<KOSU200> matehanList;
        private EditText etMantanVendor;
        private ListView listView;

        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

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

            etMantanVendor = view.FindViewById<EditText>(Resource.Id.et_mantan_vendor);
            etMantanVendor.FocusChange += delegate {
                if (!etMantanVendor.IsFocused)
                {
                    // find vendor name

                    // find vendor matehan list
                }
            };

            // ベンダー別マテハンコード取得
            string vendorCode = prefs.GetString("tsumi_vendor_cd", "");
            etMantanVendor.Text = vendorCode;

            matehanList = WebService.ExecuteKosu200(vendorCode);
            listView = view.FindViewById<ListView>(Resource.Id.lv_matehanList);

            List<string> temp = new List<string>();
            int count = 1;
            foreach (KOSU200 matehan in matehanList)
            {
                temp.Add(count+ ".  "+ matehan.matehan_nm);
                count++;
            }

            ArrayAdapter<String> adapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, temp);
            listView.Adapter = adapter;

            listView.ItemClick += (object sender, ItemClickEventArgs e) =>
            {
                SelectListViewItem(e.Position);
            };
            
            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            listView.ChoiceMode = ChoiceMode.Single;
            listView.SetItemChecked(0, true);
            listView.RequestFocus();
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

        // マテハンコード取得
        public string GetMaster()
        {
            // File読み取り
            // マテハンコードの種類は最大でも4件
            // int resultCode = GetCount();
            // aryMatehancd, aryMatehannm
            // GetData(1), GetData(2)

            return null;
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


                        if (menuFlag == 3)
                        {
                            //WebService.RequestIdou070()
                            IDOU070 idou070 = new IDOU070();
                            idou070.poRet = "0";
                            if (idou070.poRet == "0")
                            {
                                CommonUtils.AlertDialog(view, "メッセージ", "移動処理が\n完了しました。", () => {
                                    BackToMainMenu();
                                });
                            }
                            else if (idou070.poRet == "5")
                            {
                                CommonUtils.AlertDialog(View, "エラー", "該当ベンダーはマスタに存在しません。", null);
                            }
                            else
                            {
                                CommonUtils.AlertDialog(View, "エラー", "マテハン番号取得に失敗しました。", null);
                            }

                        }
                        else
                        {
                            // IDOU090
                            // int_matehan() // マテハン登録
                            // ido파일을 열어서 행을 카운트
                            // mate_renban = get_matehan()

                            string mate_renban = "";

                            if (mate_renban.Length > 0)
                            {
                                // getTodokeCd() 매개변수 ido파일안의 첫번째 값 = 카모츠번호, 토쿠이사키 값 배열

                                // ido파일안의 행 수 만큼 반복하면서 proc_tumikomi(btvPram,"06")을 반복 실행(IDOU090)
                                /*
                                If stRet.Length > 0 Then
                                If stRet == 2 Then
                                    DEVICE:syougou_NG()

                                    Handy: ShowMessageBox("移動先の貨物№が見つかりません。", "confirm")

                                ElseIf stRet == 3 Then
                                    DEVICE:syougou_NG()

                                    Handy: ShowMessageBox("移動先の届先が違います。", "confirm")

                                ElseIf stRet == 4 Then
                                    DEVICE:syougou_NG()

                                    Handy: ShowMessageBox("移動元と移動先のマテハンが同じです。", "confirm")

                                ElseIf stRet == 0 Then
                                    ret = 0

                                    EndIf
                                EndIf

                            ElseIf ret == 10 Then
                                    DEVICE:syougou_NG()
                                    Handy:ShowMessageBox("データの取得に失敗しました。","confirm")
                                    Return("retry")
                                ElseIf ret == 11 Then
                                    DEVICE:syougou_NG()
                                    Handy:ShowMessageBox("更新できませんでした。\n管理者に連絡してください。","confirm")
                                    Return("retry")
                                Else
                                    DEVICE:read_NG()
                                    Handy:ShowMessageBox("更新出来ませんでした。\n管理者に連絡してください。","confirm")
                                    Return("sagyou1")
                                EndIf


                                If ret == 0 Then
                                TOOL:del_File(nil,"ido",1)
                                Return("msg1")

                                */
                            }
                        }
                    }
                }
                );
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
 