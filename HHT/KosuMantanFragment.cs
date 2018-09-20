using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using HHT.Resources.Model;
using Android.Content;
using Android.Preferences;
using static Android.Widget.AdapterView;

namespace HHT
{
    public class KosuMantanFragment : BaseFragment
    {
        private View view;
        private List<KOSU200> matehanList;
        private EditText etMantanVendor;
        private ListView listView;

        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        private int kosuMenuflag;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_kosu_mantan, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetFooterText("");

            kosuMenuflag = prefs.GetInt(Const.KOSU_MENU_FLAG, (int)Const.KOSU_MENU.TODOKE); // 画面区分

            etMantanVendor = view.FindViewById<EditText>(Resource.Id.et_mantan_vendor);
            etMantanVendor.FocusChange += delegate {
                if (!etMantanVendor.IsFocused)
                {
                    // find vendor name

                    // find vendor matehan list
                }
            };

            TextView txtVenderName = view.FindViewById<TextView>(Resource.Id.vendorName);
            txtVenderName.Text = prefs.GetString("vendor_nm", "");

            string vendorCode = prefs.GetString("vendor_cd", "");
            etMantanVendor.Text = vendorCode;

            // ベンダー別マテハンコード取得
            matehanList = WebService.RequestKosu200(vendorCode);
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

        public void SelectListViewItem(int index)
        {
            if (matehanList.Count > index)
            {
                string msg = matehanList[index].matehan_nm + "でよろしいですか？";
                CommonUtils.AlertConfirm(view, "確認", msg, (flag) =>
                {
                    if (flag)
                    {
                        try
                        {
                            KOSU070 result = new KOSU070();
                            if (kosuMenuflag == (int)Const.KOSU_MENU.TODOKE)
                            {
                                result = WebService.RequestKosu080(GetProcedureParam(matehanList[index].matehan_cd));
                            }
                            else if (kosuMenuflag == (int)Const.KOSU_MENU.VENDOR)
                            {
                                result = WebService.RequestKosu160(GetProcedureParam(matehanList[index].matehan_cd));
                            }
                            
                            string dai_su = prefs.GetString("dai_su", "0");
                            int dai_su_intValue = int.TryParse(dai_su, out dai_su_intValue) ? dai_su_intValue : 0;

                            if (result.poRet == "0")
                            {
                                dai_su_intValue = dai_su_intValue + 1; //台数加算

                                editor.PutString("dai_su", dai_su_intValue.ToString());
                                editor.Apply();

                                if (int.Parse(result.poMsg) == 0)
                                {
                                    StartFragment(FragmentManager, typeof(KosuCompleteFragment));
                                }
                                else if (int.Parse(result.poMsg) > 0)
                                {
                                    // 正常に処理されたが、残り作業がある場合、
                                    // 紐づけ画面に遷移する。
                                    
                                    editor.PutString("case_su", "0");
                                    editor.PutString("oricon_su", "0");
                                    editor.PutString("futeikei_su", "0");
                                    editor.PutString("ido_su", "0");
                                    editor.PutString("hazai_su", "0");
                                    editor.PutString("henpin_su", "0");
                                    editor.PutString("hansoku_su", "0");
                                    editor.PutString("kaisyu_su", "0");
                                    editor.Apply();

                                    this.Activity.FragmentManager.PopBackStack();
                                }
                            }
                            else
                            {
                                CommonUtils.AlertDialog(view, "エラー", "更新出来ませんでした。\n管理者に連絡してください。", null);
                                return;
                            }
                        }
                        catch
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


        private Dictionary<string, string> GetProcedureParam(string pMatehan)
        {
            string pSagyosyaCD = prefs.GetString("driver_cd", "");
            string pSoukoCD = prefs.GetString("souko_cd", "");
            string pSyukaDate = prefs.GetString("syuka_date", "");
            string pTokuisakiCD = prefs.GetString("tokuisaki_cd", "0000");
            string pTodokesakiCD = prefs.GetString("todokesaki_cd", "");
            string pVendorCD = prefs.GetString("vendor_cd", "");
            string pTsumiVendorCD = prefs.GetString("vendor_cd", "");
            string pKamotsuNo = "";
            string pBinNo = prefs.GetString("bin_no", "0");
            string pHHT_No = "11101";
            
            string pJskCaseSu = prefs.GetString("case_su", "0");
            string pJskOriconSu = prefs.GetString("oricon_su", "0");
            string pJskFuteikeiSu = prefs.GetString("futeikei_su", "1");
            string pJskHazaiSu = prefs.GetString("hazai_su", "0");
            string pJskIdoSu = prefs.GetString("ido_su", "0");
            string pJskHenpinSu = prefs.GetString("henpin_su", "0");
            string pJskHansokuSu = prefs.GetString("hansoku_su", "0");

            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "pTerminalID",  "432660068"},
                { "pProgramID",  "KOS"},
                { "pSagyosyaCD",  pSagyosyaCD},
                { "pSoukoCD",  pSoukoCD},
                { "pSyukaDate",  pSyukaDate},
                { "pTokuisakiCD" ,  pTokuisakiCD},
                { "pTodokesakiCD" ,  pTodokesakiCD},
                { "pVendorCD",  pVendorCD},
                { "pTsumiVendorCD",  pTsumiVendorCD},
                { "pKamotsuNo",  pKamotsuNo},
                { "pBinNo",  pBinNo},
                { "pHHT_No",  "11101"},
                { "pMatehan",  pMatehan},
                { "pJskCaseSu",  pJskCaseSu},
                { "pJskOriconSu",  pJskOriconSu},
                { "pJskFuteikeiSu",  pJskFuteikeiSu},
                { "pJskTcSu",  "0"},
                { "pJskMailbinSu",  "0"},
                { "pJskHazaiSu",  pJskHazaiSu},
                { "pJskIdoSu",  pJskIdoSu},
                { "pJskHenpinSu",  pJskHenpinSu},
                { "pJskHansokuSu",  pJskHansokuSu}
            };

            return param;
        }

    }
}
 