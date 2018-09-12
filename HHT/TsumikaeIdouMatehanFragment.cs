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

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            SetTitle("届先指定検品");
            SetFooterText("");

            view = inflater.Inflate(Resource.Layout.fragment_tsumikae_idou_matehan, container, false);

            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

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
                        editor.PutString("mateno", matehanList[index].matehan);
                        editor.Apply();

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
                        
                        /* TODO まだプロシージャが動かない
                            if (kosuMenuflag == (int)Const.KOSU_MENU.TODOKE)
                            {
                                kosuKenpin = WebService.RequestKosu080(param);
                            }
                            else if (kosuMenuflag == (int)Const.KOSU_MENU.VENDOR)
                            {
                                kosuKenpin = WebService.RequestKosu160(param);
                            }

                        */

                        int resultCode = 0;
                        // 全完了OK

                        string dai_su = prefs.GetString("dai_su", "0");
                        int dai_su_intValue = int.TryParse(dai_su, out dai_su_intValue) ? dai_su_intValue : 0;

                        if (resultCode == 0 || resultCode == 1)
                        {
                            dai_su_intValue = dai_su_intValue + 1; //台数加算

                            editor.PutString("dai_su", dai_su_intValue.ToString());
                            editor.Apply();

                            if (resultCode == 0)
                            {
                                StartFragment(FragmentManager, typeof(KosuCompleteFragment));
                            }
                            else if (resultCode == 1)
                            {
                                /*
                                If JOB:menu_flg == JOB:MENU_VENDOR THEN
                                JOB: tokuisaki_cd = ""
                                JOB: todokesaki_cd = ""
                                JOB: tokuisaki_nm = ""
                                Return("sagyou15")
                                */
                                // 前の画面に遷移するけど、変更された値を考える必要がある
                                // Return("sagyou15") 紐付画面
                                this.Activity.FragmentManager.PopBackStack();
                            }
                        }
                        else
                        {
                            CommonUtils.AlertDialog(view, "エラー", "更新出来ませんでした。\n管理者に連絡してください。", null);
                            return;
                        }
                    }
                }
                );
            }
        }

        public void iniZeroData2()
        {
            /*
            JOB: case_su = 0		//ケース
            JOB: oricon_su = 0		//オリコン
            JOB: futeikei_su = 0		//不定形
            JOB: ido_su = 0		//店移動
            JOB: hansoku_su = 0		//販促物
            JOB: hazai_su = 0		//破材
            JOB: henpin_su = 0		//返品数
            JOB: kaisyu_su = 0		//回収
            */
        }
    }
}
 