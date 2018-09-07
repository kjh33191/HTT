using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Util;
using Android.Views;
using Android.Widget;
using HHT.Resources.DataHelper;
using HHT.Resources.Model;
using System.Collections.Generic;

namespace HHT
{
    public class NohinKaisyuMatehanFragment : BaseFragment
    {
        private readonly string TAG = "NohinKaisyuMatehanFragment";

        private View view;
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        private List<MateFile> mateList;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_nohin_kaisyu_matehan, container, false);
            // パラメータ設定
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            // コンポーネント初期化
            InitComponent();


            return view;
        }

        private void InitComponent()
        {
            SetTitle("マテハン回収");
            SetFooterText("");

            //et_nohinKaisyuMatehan_matehan1
            TextView vendorNm = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuMatehan_vendorName);
            TextView matehan1Nm = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuMatehan_matehan1);
            TextView matehan2Nm = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuMatehan_matehan2);
            TextView matehan3Nm = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuMatehan_matehan3);
            TextView matehan4Nm = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuMatehan_matehan4);

            EditText vendorCd = view.FindViewById<EditText>(Resource.Id.et_nohinKaisyuMatehan_vendorCode);
            EditText matehan1Su = view.FindViewById<EditText>(Resource.Id.et_nohinKaisyuMatehan_matehan1);
            EditText matehan2Su = view.FindViewById<EditText>(Resource.Id.et_nohinKaisyuMatehan_matehan2);
            EditText matehan3Su = view.FindViewById<EditText>(Resource.Id.et_nohinKaisyuMatehan_matehan3);
            EditText matehan4Su = view.FindViewById<EditText>(Resource.Id.et_nohinKaisyuMatehan_matehan4);

            List<MateFile> matehanList = GetMatehanData();

            vendorCd.Text = prefs.GetString("mate_vendor_cd", "");
            vendorNm.Text = prefs.GetString("mate_vendor_nm", "");

            matehan1Su.Text = "0";
            matehan1Nm.Text = matehanList[0].matehan_nm;

            matehan2Su.Text = "0";
            matehan2Nm.Text = matehanList[1].matehan_nm;

            matehan3Su.Text = "0";
            matehan3Nm.Text = matehanList[2].matehan_nm;

            matehan4Su.Text = "0";
            matehan4Nm.Text = matehanList[3].matehan_nm;

            matehan1Su.RequestFocus();

            Button button1 = view.FindViewById<Button>(Resource.Id.btn_nohinKaisyuMatehan_confirm);
            button1.Click += delegate {
                int idx = 0;
                foreach (MateFile mate in mateList)
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

                    appendFile(mate.matehan_cd, matesu);
                    idx++;
                };

                FragmentManager.PopBackStack();
            }; 

        }

        private List<MateFile> GetMatehanData()
        {
            Log.Debug(TAG, "GetMatehanData start");
            MateFileHelper mateFileHelper = new MateFileHelper();
            mateList = mateFileHelper.SelectByVendorCd(prefs.GetString("mate_vendor_cd", ""));
            return mateList;
        }


        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F1)
            {
                int idx = 0;
                foreach (MateFile mate in mateList)
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

                    appendFile(mate.matehan_cd, matesu);
                    idx++;
                };

                FragmentManager.PopBackStack();
            }

            else if (keycode == Keycode.Num1)
            {
                StartFragment(FragmentManager, typeof(MatehanSelectFragment));
            }

            return true;
        }

        private void appendFile(string matehan_cd, string matesu)
        {
            // レコード作成用　値取得
            SndNohinMate sndNohinMate = new SndNohinMate
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