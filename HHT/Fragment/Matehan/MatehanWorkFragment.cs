using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using Com.Beardedhen.Androidbootstrap;
using HHT.Resources.DataHelper;
using HHT.Resources.Model;
using System.Collections.Generic;

namespace HHT
{
    public class MatehanWorkFragment : BaseFragment
    {
        private readonly string TAG = "MatehanWorkFragment";

        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        private View view;
        private List<MateFile> matehanList;

        private SndNohinMateHelper nohinMateHelper;
        private MateFileHelper mateFileHelper;

        TextView matehan1Nm, matehan2Nm, matehan3Nm, matehan4Nm
            , mateRental1, mateRental2, mateRental3, mateRental4;
        BootstrapEditText matehan1Su, matehan2Su, matehan3Su, matehan4Su;

        List<MATE030> mate030List;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_matehan_work, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            // DB helper
            nohinMateHelper = new SndNohinMateHelper();
            mateFileHelper = new MateFileHelper();

            // コンポーネント初期化
            SetTitle("貸出登録");

            TextView vendorNm = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuMatehan_vendorName);
            matehan1Nm = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuMatehan_matehan1);
            matehan2Nm = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuMatehan_matehan2);
            matehan3Nm = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuMatehan_matehan3);
            matehan4Nm = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuMatehan_matehan4);

            matehan1Su = view.FindViewById<BootstrapEditText>(Resource.Id.et_nohinKaisyuMatehan_matehan1);
            matehan2Su = view.FindViewById<BootstrapEditText>(Resource.Id.et_nohinKaisyuMatehan_matehan2);
            matehan3Su = view.FindViewById<BootstrapEditText>(Resource.Id.et_nohinKaisyuMatehan_matehan3);
            matehan4Su = view.FindViewById<BootstrapEditText>(Resource.Id.et_nohinKaisyuMatehan_matehan4);

            mateRental1 = view.FindViewById<TextView>(Resource.Id.matehanRental1);
            mateRental2 = view.FindViewById<TextView>(Resource.Id.matehanRental2);
            mateRental3 = view.FindViewById<TextView>(Resource.Id.matehanRental3);
            mateRental4 = view.FindViewById<TextView>(Resource.Id.matehanRental4);

            BootstrapButton button1 = view.FindViewById<BootstrapButton>(Resource.Id.btn_nohinKaisyuMatehan_confirm);
            button1.Click += delegate { ConfirmMatehanKaisyu(); };

            vendorNm.Text = prefs.GetString("vendor_nm", "");
            
            mate030List = WebService.RequestMate030(prefs.GetString("vendor_cd", ""));
            matehanList = new List<MateFile>();

            foreach (MATE030 mate030 in mate030List)
            {
                string code_name = WebService.RequestMate040(mate030.matehan_cd);
                MateFile mateFile = new MateFile();
                mateFile.matehan_cd = mate030.matehan_cd;
                mateFile.matehan_nm = code_name;
                matehanList.Add(mateFile);
            }

            SetMateVendorInfo(prefs.GetString("vendor_cd", ""));
            matehan1Su.RequestFocus();
            
            return view;
        }

        private void SetMateVendorInfo(string mateVendorCd)
        {
            matehan1Su.Visibility = ViewStates.Invisible;
            matehan1Nm.Visibility = ViewStates.Invisible;
            matehan2Su.Visibility = ViewStates.Invisible;
            matehan2Nm.Visibility = ViewStates.Invisible;
            matehan3Su.Visibility = ViewStates.Invisible;
            matehan3Nm.Visibility = ViewStates.Invisible;
            matehan4Su.Visibility = ViewStates.Invisible;
            matehan4Nm.Visibility = ViewStates.Invisible;
            mateRental1.Visibility = ViewStates.Invisible;
            mateRental2.Visibility = ViewStates.Invisible;
            mateRental3.Visibility = ViewStates.Invisible;
            mateRental4.Visibility = ViewStates.Invisible;

            int index = 0;
            foreach (MateFile matehan in matehanList)
            {
                switch (index)
                {
                    case 0:
                        matehan1Su.Text = "0";
                        matehan1Nm.Text = matehanList[index].matehan_nm;
                        matehan1Su.Visibility = ViewStates.Visible;
                        matehan1Nm.Visibility = ViewStates.Visible;
                        mateRental1.Visibility = ViewStates.Visible;
                        mateRental1.Text = "/" + mate030List[index].matehan_rental;
                        break;
                    case 1:
                        matehan2Su.Text = "0";
                        matehan2Nm.Text = matehanList[index].matehan_nm;
                        matehan2Su.Visibility = ViewStates.Visible;
                        matehan2Nm.Visibility = ViewStates.Visible;
                        mateRental2.Visibility = ViewStates.Visible;
                        mateRental2.Text = "/" + mate030List[index].matehan_rental;
                        break;
                    case 2:
                        matehan3Su.Text = "0";
                        matehan3Nm.Text = matehanList[index].matehan_nm;
                        matehan3Su.Visibility = ViewStates.Visible;
                        matehan3Nm.Visibility = ViewStates.Visible;
                        mateRental3.Visibility = ViewStates.Visible;
                        mateRental2.Text = "/" + mate030List[index].matehan_rental;
                        break;
                    case 3:
                        matehan4Su.Text = "0";
                        matehan4Nm.Text = matehanList[index].matehan_nm;
                        matehan4Su.Visibility = ViewStates.Visible;
                        matehan4Nm.Visibility = ViewStates.Visible;
                        mateRental4.Visibility = ViewStates.Visible;
                        mateRental2.Text = "/" + mate030List[index].matehan_rental;
                        break;
                    default: break;
                }

                index++;
            }
            
        }

        private void ConfirmMatehanKaisyu()
        {
            if (matehan1Su.Text != "" && matehan1Su.Text != "0")
            {
                int btvVal1 = int.Parse(matehan1Su.Text);
                int btvVal2 = int.Parse(mate030List[0].matehan_rental);

                if(btvVal1 > btvVal2)
                {
                    CommonUtils.AlertDialog(view, "", "貸出数量が許容数を超えました。", null);
                    return;
                }
            }

            if (matehan2Su.Text != "" && matehan2Su.Text != "0")
            {
                int btvVal1 = int.Parse(matehan2Su.Text);
                int btvVal2 = int.Parse(mate030List[1].matehan_rental);

                if (btvVal1 > btvVal2)
                {
                    CommonUtils.AlertDialog(view, "", "貸出数量が許容数を超えました。", null);
                    return;
                }
            }

            if (matehan3Su.Text != "" && matehan3Su.Text != "0")
            {
                int btvVal1 = int.Parse(matehan3Su.Text);
                int btvVal2 = int.Parse(mate030List[2].matehan_rental);

                if (btvVal1 > btvVal2)
                {
                    CommonUtils.AlertDialog(view, "", "貸出数量が許容数を超えました。", null);
                    return;
                }
            }

            if (matehan4Su.Text != "" && matehan4Su.Text != "0")
            {
                int btvVal1 = int.Parse(matehan4Su.Text);
                int btvVal2 = int.Parse(mate030List[3].matehan_rental);

                if (btvVal1 > btvVal2)
                {
                    CommonUtils.AlertDialog(view, "", "貸出数量が許容数を超えました。", null);
                    return;
                }
            }

            CommonUtils.AlertConfirm(view, "", "よろしいですか？", (flag) => {
                if (flag)
                {
                    // proc_hht_kasidasi
                    Dictionary<string, string> param = new Dictionary<string, string>
                    {
                        { "pTerminalID", prefs.GetString("terminal_id","")},
                        { "pProgramID", "HHT"},
                        { "pSagyosyaCD", prefs.GetString("sagyousya_cd", "")},
                        { "pSoukoCD", prefs.GetString("souko_cd", "")},
                        { "pKitakuCD", prefs.GetString("kitaku_cd", "")},
                        { "pNyusyukoDate", prefs.GetString("kasidasi_date", "")},
                        { "pNyusyukoSu1", matehan1Su.Text == "" ? "0" : matehan1Su.Text},
                        { "pNyusyukoSu2", matehan2Su.Text == "" ? "0" : matehan2Su.Text},
                        { "pNyusyukoSu3", matehan3Su.Text == "" ? "0" : matehan3Su.Text},
                        { "pNyusyukoSu4", matehan4Su.Text == "" ? "0" : matehan4Su.Text},
                        { "pMatehanVendor", prefs.GetString("vendor_cd", "")},
                        { "pMatehanCd1", matehanList.Count > 0 ? matehanList[0].matehan_cd : ""},
                        { "pMatehanCd2", matehanList.Count > 1 ? matehanList[1].matehan_cd : ""},
                        { "pMatehanCd3", matehanList.Count > 2 ? matehanList[2].matehan_cd : ""},
                        { "pMatehanCd4", matehanList.Count > 3 ? matehanList[3].matehan_cd : ""}
                    };

                    MATE060 mate060 = WebService.RequestMate060(param);
                    if(mate060.poRet == "0")
                    {
                        CommonUtils.AlertDialog(view, "", "貸出登録が完了しました。",()=> {
                            FragmentManager.PopBackStack();
                        });
                    }
                }
            });
        }

        private void CreateKaisyuData()
        {
            // 以前保存された情報は削除
            nohinMateHelper.DeleteAll();

            int idx = 0;
            foreach (MateFile mate in matehanList)
            {
                string matesu = "";

                switch (idx)
                {
                    case 0:
                        matesu = view.FindViewById<EditText>(Resource.Id.et_nohinKaisyuMatehan_matehan1).Text;
                        break;
                    case 1:
                        matesu = view.FindViewById<EditText>(Resource.Id.et_nohinKaisyuMatehan_matehan2).Text;
                        break;
                    case 2:
                        matesu = view.FindViewById<EditText>(Resource.Id.et_nohinKaisyuMatehan_matehan3).Text;
                        break;
                    case 3:
                        matesu = view.FindViewById<EditText>(Resource.Id.et_nohinKaisyuMatehan_matehan4).Text;
                        break;
                    default:
                        break;
                }

                if(matesu != "0") InsertSndMat(mate.matehan_cd, matesu);

                idx++;
            };
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F1)
            {
                ConfirmMatehanKaisyu();
            }
            
            return true;
        }

        private void InsertSndMat(string matehan_cd, string matesu)
        {
            // レコード作成用　値取得
            SndNohinMate sndNohinMate = new SndNohinMate
            {
                wPackage = "02",
                wTerminalID = "432660068",
                wProgramID = "NOH",
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
                wMateVendorCd = prefs.GetString("mate_vendor_cd", ""),
                wSyukaDate = prefs.GetString("haiso_date", ""),
                wButsuryuNo = "",
                wKamotuNo = "",
                wMatehan = matehan_cd,
                wMatehanSu = matesu,
                wHHT_no = "11101",
                wNohinKbn = "1", // 商品回収
                wKaisyuKbn = "1",
                wTenkanState = "00",
                wSakiTokuisakiCD = "",
                wSakiTodokesakiCD = "",
                wNohinDate = prefs.GetString("nohin_date", ""),
                wNohinTime = prefs.GetString("nohin_time", "")
            };

            SndNohinMateHelper nohinMateHelper = new SndNohinMateHelper();
            nohinMateHelper.Insert(sndNohinMate);
        }
    }
}