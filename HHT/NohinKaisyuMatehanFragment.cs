﻿using Android.Content;
using Android.OS;
using Android.Preferences;
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

        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        private View view;
        private List<MateFile> matehanList;

        private SndNohinMateHelper nohinMateHelper;
        private MateFileHelper mateFileHelper;
        
        TextView matehan1Nm, matehan2Nm, matehan3Nm, matehan4Nm;
        TextView matehan1Su, matehan2Su, matehan3Su, matehan4Su;

        private string motoVendorCd;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_nohin_kaisyu_matehan, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            // DB helper
            nohinMateHelper = new SndNohinMateHelper();
            mateFileHelper = new MateFileHelper();

            // コンポーネント初期化
            SetTitle("マテハン回収");
            SetFooterText("");

            TextView vendorNm = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuMatehan_vendorName);
            matehan1Nm = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuMatehan_matehan1);
            matehan2Nm = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuMatehan_matehan2);
            matehan3Nm = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuMatehan_matehan3);
            matehan4Nm = view.FindViewById<TextView>(Resource.Id.txt_nohinKaisyuMatehan_matehan4);

            EditText vendorCd = view.FindViewById<EditText>(Resource.Id.et_nohinKaisyuMatehan_vendorCode);
            matehan1Su = view.FindViewById<EditText>(Resource.Id.et_nohinKaisyuMatehan_matehan1);
            matehan2Su = view.FindViewById<EditText>(Resource.Id.et_nohinKaisyuMatehan_matehan2);
            matehan3Su = view.FindViewById<EditText>(Resource.Id.et_nohinKaisyuMatehan_matehan3);
            matehan4Su = view.FindViewById<EditText>(Resource.Id.et_nohinKaisyuMatehan_matehan4);

            Button button1 = view.FindViewById<Button>(Resource.Id.btn_nohinKaisyuMatehan_confirm);
            button1.Click += delegate { ConfirmMatehanKaisyu(); };

            vendorCd.Text = prefs.GetString("mate_vendor_cd", "");
            vendorNm.Text = prefs.GetString("mate_vendor_nm", "");

            vendorCd.FocusChange += delegate {
                if (!vendorCd.IsFocused)
                {
                    if (vendorCd.Text != "")
                    {
                        matehanList = mateFileHelper.SelectByVendorCd(vendorCd.Text);

                        if (matehanList.Count == 0)
                        {
                            CommonUtils.AlertDialog(view, "", "ベンダーコードが存在しません。", () => { vendorCd.Text = motoVendorCd; vendorCd.RequestFocus(); });
                            return;
                        }
                        else
                        {
                            SetMateVendorInfo(vendorCd.Text);
                            motoVendorCd = vendorCd.Text;
                            vendorNm.Text = matehanList[0].vendor_nm;
                            
                            editor.PutString("mate_vendor_cd", vendorCd.Text);
                            editor.PutString("mate_vendor_nm", matehanList[0].vendor_nm);
                            editor.Apply();
                        }
                    }
                    
                }
            };

            matehanList = mateFileHelper.SelectByVendorCd(prefs.GetString("mate_vendor_cd", ""));
            SetMateVendorInfo(prefs.GetString("mate_vendor_cd", ""));
            motoVendorCd = prefs.GetString("mate_vendor_cd", "");

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
                        break;
                    case 1:
                        matehan2Su.Text = "0";
                        matehan2Nm.Text = matehanList[index].matehan_nm;
                        matehan2Su.Visibility = ViewStates.Visible;
                        matehan2Nm.Visibility = ViewStates.Visible;
                        break;
                    case 2:
                        matehan3Su.Text = "0";
                        matehan3Nm.Text = matehanList[index].matehan_nm;
                        matehan3Su.Visibility = ViewStates.Visible;
                        matehan3Nm.Visibility = ViewStates.Visible;
                        break;
                    case 3:
                        matehan4Su.Text = "0";
                        matehan4Nm.Text = matehanList[index].matehan_nm;
                        matehan4Su.Visibility = ViewStates.Visible;
                        matehan4Nm.Visibility = ViewStates.Visible;
                        break;
                    default: break;
                }

                index++;
            }
            
            List<SndNohinMate> items = nohinMateHelper.SelectByMateVendorCd(mateVendorCd);
            foreach (SndNohinMate item in items)
            {
                if (item.wMatehan == "01") matehan1Su.Text = item.wMatehanSu;
                else if (item.wMatehan == "02") matehan2Su.Text = item.wMatehanSu;
                else if (item.wMatehan == "03") matehan3Su.Text = item.wMatehanSu;
                else if (item.wMatehan == "04") matehan4Su.Text = item.wMatehanSu;
            }
        }

        private void ConfirmMatehanKaisyu()
        {
            CommonUtils.AlertConfirm(view, "", "よろしいですか？", (flag) => {
                if (flag)
                {
                    
                    CreateKaisyuData();

                    string bodyMsg = "マテハン回収情報が保存されました。";
                    
                    Toast.MakeText(this.Activity, bodyMsg, ToastLength.Long).Show();
                    
                    FragmentManager.PopBackStack();
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