using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Beardedhen.Androidbootstrap;
using Com.Densowave.Bhtsdk.Barcode;
using HHT.Resources.DataHelper;
using HHT.Resources.Model;
using System.Collections.Generic;
using System.Threading;

namespace HHT
{
    public class NohinWorkFragment : BaseFragment
    {
        private readonly string TAG = "NohinWorkFragment";

        private View view;
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        private TextView tvCase, tvOricon, tvSonota, tvIdo, tvMail, tvFuteikei, tvHansoku, tvTc, tvTsumidai, tvAll, tvmatehanNm;
        private BootstrapButton nohinWorkButton, kaizoButton;
        private int ko_su, maxko_su;

        MFileHelper mFilehelper;
        private List<MFile> tsumikomiDataList;
        private int matehanCnt;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_nohin_work, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();
            mFilehelper = new MFileHelper();

            InitComponent(); // 初期化
            GetTumisu(); // 積込台数取得
            
            ko_su = int.Parse(prefs.GetString("ko_su", "0"));
            
            return view;
        }

        private void InitComponent()
        {
            SetTitle("納品検品");

            TextView txtTokuisaki = view.FindViewById<TextView>(Resource.Id.txt_nohinwork_tokuisakiNm);
            txtTokuisaki.Text = prefs.GetString("tokuisaki_nm", "");
            TextView txtTodokesaki = view.FindViewById<TextView>(Resource.Id.txt_nohinwork_todokesakiNm);
            txtTodokesaki.Text = prefs.GetString("todokesaki_nm", ""); ;

            tvCase = view.FindViewById<TextView>(Resource.Id.txt_nohinWork_case);
            tvOricon = view.FindViewById<TextView>(Resource.Id.txt_nohinWork_oricon);
            tvSonota = view.FindViewById<TextView>(Resource.Id.txt_nohinWork_sonota);
            tvIdo = view.FindViewById<TextView>(Resource.Id.txt_nohinWork_ido);
            tvMail = view.FindViewById<TextView>(Resource.Id.txt_nohinWork_mail);
            tvFuteikei = view.FindViewById<TextView>(Resource.Id.txt_nohinWork_futeikei);
            tvHansoku = view.FindViewById<TextView>(Resource.Id.txt_nohinWork_hansoku);
            tvTc = view.FindViewById<TextView>(Resource.Id.txt_nohinWork_tc);
            tvTsumidai = view.FindViewById<TextView>(Resource.Id.txt_nohinWork_tsumidaisu);
            tvAll = view.FindViewById<TextView>(Resource.Id.txt_nohinWork_all);
            tvmatehanNm = view.FindViewById<TextView>(Resource.Id.matehanNm);
            nohinWorkButton = view.FindViewById<BootstrapButton>(Resource.Id.nohinButton);
            nohinWorkButton.Click += delegate {
                if (tsumikomiDataList.Count == ko_su)
                {
                    Log.Debug(TAG, "MAIN NOHIN COMPLETE");
                    editor.PutString("menu_flg", "2");
                    editor.PutBoolean("nohinWorkEndFlag", true);
                    editor.Apply();
                    
                    ShowDialog("報告", "納品検品が\n完了しました。\n\nお疲れ様でした！", () => { FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(2).Id, 0); });

                }
                else
                {
                    nohinWorkButton.Visibility = ViewStates.Gone;
                    kaizoButton.Visibility = ViewStates.Visible;

                    tvCase.Text = "0";
                    tvOricon.Text = "0";
                    tvFuteikei.Text = "0";
                    tvTc.Text = "0";
                    tvIdo.Text = "0";
                    tvMail.Text = "0";
                    tvHansoku.Text = "0";
                    tvSonota.Text = "0";
                }
            };

            kaizoButton = view.FindViewById<BootstrapButton>(Resource.Id.kaizoButton);
            kaizoButton.Click += delegate {
                editor.PutString("menu_flg", "2");
                editor.Apply();
                StartFragment(FragmentManager, typeof(NohinMailBagPasswordFragment));
            };

            nohinWorkButton.Visibility = ViewStates.Gone;
            kaizoButton.Visibility = ViewStates.Visible;

        }

        private void GetTumisu()
        {
            string tokuisaki_cd = prefs.GetString("tokuisaki_cd", "");
            string todokesaki_cd = prefs.GetString("todokesaki_cd", "");
            
            tsumikomiDataList = mFilehelper.SelectTsumikomiList(tokuisaki_cd, todokesaki_cd);
            int btvQty = tsumikomiDataList.Count;

            // 積込台数
            string tempMatehan = "";
            matehanCnt = 0;
            foreach (MFile data in tsumikomiDataList) {
                if (data.matehan != tempMatehan)
                {
                    tempMatehan = data.matehan;
                    matehanCnt++;
                }
            }

            int idx = tvTsumidai.Text.IndexOf('/');
            string tsumidaiFull = btvQty.ToString();
            int tsumidai = (int.Parse(tvTsumidai.Text.Substring(0, idx)));
            tvTsumidai.Text = tsumidai + "/" + matehanCnt;

            // 総個数
            idx = tvAll.Text.IndexOf('/');
            tvAll.Text = (int.Parse(tvAll.Text.Substring(0, idx))) + "/" + btvQty.ToString();
            maxko_su = btvQty;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F1)
            {
                EditText et = new EditText(this.Activity);

                AlertDialog.Builder ad = new AlertDialog.Builder(this.Activity);
                ad.SetTitle("Password");
                ad.SetView(et);
                ad.SetPositiveButton("Submit", delegate
                 {
                     // password テーブルからパスワード情報を取得する。

                     Toast.MakeText(this.Activity, "Submit Input: " + et.Text, ToastLength.Short).Show();

                     if (et.Text == "")
                     {
                         SndNohinWorkHelper sndNohinWorkHelper = new SndNohinWorkHelper();
                         sndNohinWorkHelper.DeleteAll();

                         editor.PutBoolean("nohinWorkEndFlag", true);
                         editor.Apply();
                         StartFragment(FragmentManager, typeof(NohinCompleteFragment));
                     }
                     else
                     {
                         ShowDialog("エラー", "パスワードが違います。", () => { });
                         return;
                     }

                 });
                ad.Show();
            }
            else if (keycode == Keycode.Enter)
            {
                if (nohinWorkButton.Visibility == ViewStates.Visible)
                {
                    if (tsumikomiDataList.Count == ko_su)
                    {
                        Log.Debug(TAG, "MAIN NOHIN COMPLETE ");
                        editor.PutBoolean("nohinWorkEndFlag", true);
                        editor.Apply();
                        StartFragment(FragmentManager, typeof(NohinCompleteFragment));
                    }
                    else
                    {
                        nohinWorkButton.Visibility = ViewStates.Gone;
                        kaizoButton.Visibility = ViewStates.Visible;

                        tvCase.Text = "0";
                        tvOricon.Text = "0";
                        tvFuteikei.Text = "0";
                        tvTc.Text = "0";
                        tvIdo.Text = "0";
                        tvMail.Text = "0";
                        tvHansoku.Text = "0";
                        tvSonota.Text = "0";
                    }
                }
            }

            return true;
        }

        public override bool OnBackPressed()
        {
            return false;
        }

        public override void OnBarcodeDataReceived(BarcodeDataReceivedEvent_ dataReceivedEvent)
        {
            IList<BarcodeDataReceivedEvent_.BarcodeData_> listBarcodeData = dataReceivedEvent.BarcodeData;

            foreach (BarcodeDataReceivedEvent_.BarcodeData_ barcodeData in listBarcodeData)
            {
                string kamotu_no = barcodeData.Data;
                
                bool isExist = false;
                MFile tsumikomi = null;
                foreach (MFile temp in tsumikomiDataList)
                {
                    if (temp.kamotsu_no == kamotu_no)
                    {
                        isExist = true;
                        tsumikomi = temp; 
                        break;
                    }
                }

                new Thread(new ThreadStart(delegate {
                    Activity.RunOnUiThread(() =>
                    {
                        if (!isExist)
                        {
                            ShowDialog("エラー", "該当データがありません。", () => { });
                            return;
                        }
                        
                        SndNohinWorkHelper sndNohinWorkHelper = new SndNohinWorkHelper();
                        if (sndNohinWorkHelper.SelectNohinWorkWithKamotu(kamotu_no).Count > 0)
                        {
                            ShowDialog("エラー", "登録済みです。", () => { });
                            return;
                        }
                        
                        string tokuisaki_cd = prefs.GetString("tokuisaki_cd", "");
                        string todokesaki_cd = prefs.GetString("todokesaki_cd", "");
                        string matehanCd = tsumikomi.matehan;

                        List<MFile> mfileList = mFilehelper.SelectByMatehanCd(tokuisaki_cd, todokesaki_cd, matehanCd);

                        if(mfileList.Count > 0)
                        {
                            foreach(SndNohinWork temp in sndNohinWorkHelper.SelectAll())
                            {
                                if (mfileList[0].matehan == temp.wMatehan)
                                {
                                    ShowDialog("エラー", "登録済みです。", () => { });
                                    return;
                                }
                            }
                        }
                        else
                        {
                            // err?
                        }
                        
                        tvmatehanNm.Text = mfileList[0].category_nm;
                        
                        int idx = tvTsumidai.Text.IndexOf('/');
                        int tsumidai = (int.Parse(tvTsumidai.Text.Substring(0, idx)) + 1);

                        // 総個数
                        idx = tvAll.Text.IndexOf('/');
                        ko_su = int.Parse(tvAll.Text.Substring(0, idx)) + mfileList.Count;

                        if (ko_su <= tsumikomiDataList.Count)
                        {
                            // 画面の数字をカウントアップする
                            tvTsumidai.Text = tsumidai + "/" + matehanCnt;
                            tvAll.Text = ko_su + "/" + tsumikomiDataList.Count;

                            switch (tsumikomi.bunrui)
                            {
                                case "01": tvCase.Text = (int.Parse(tvCase.Text) + mfileList.Count).ToString(); break;
                                case "02": tvOricon.Text = (int.Parse(tvOricon.Text) + mfileList.Count).ToString(); break;
                                case "03": tvSonota.Text = (int.Parse(tvSonota.Text) + mfileList.Count).ToString(); break;
                                case "04": tvIdo.Text = (int.Parse(tvIdo.Text) + mfileList.Count).ToString(); break;
                                case "05": tvMail.Text = (int.Parse(tvMail.Text) + mfileList.Count).ToString(); break;
                                case "06": tvSonota.Text = (int.Parse(tvSonota.Text) + mfileList.Count).ToString(); break;
                                case "07": tvFuteikei.Text = (int.Parse(tvFuteikei.Text) + mfileList.Count).ToString(); break;
                                case "08": tvSonota.Text = (int.Parse(tvSonota.Text) + mfileList.Count).ToString(); break;
                                case "09": tvHansoku.Text = (int.Parse(tvHansoku.Text) + mfileList.Count).ToString(); break;
                                case "T": tvTc.Text = (int.Parse(tvTc.Text) + mfileList.Count).ToString(); break;
                                default: tvSonota.Text = (int.Parse(tvSonota.Text) + mfileList.Count).ToString(); break;
                            }
                            
                            if (ko_su == tsumikomiDataList.Count)
                            {
                                // レコード作成用　値取得
                                SndNohinWork sndNohinWork = new SndNohinWork
                                {
                                    wPackage = "02",
                                    wTerminalID = "432660068", //Handy: serialId
                                    wProgramID = prefs.GetString("program_id", "NOH"), //JOB: program_id
                                    wSagyosyaCD = prefs.GetString("sagyosya", ""),
                                    wSoukoCD = mfileList[0].kenpin_souko,
                                    wHaisoDate = mfileList[0].syuka_date, // noh_syukaDate
                                    wBinNo = mfileList[0].bin_no, //JOB: noh_binNo
                                    wCourse = mfileList[0].course, //noh_course
                                    wDriverCD = mfileList[0].driver_cd, // noh_driverCd
                                    wTokuisakiCD = mfileList[0].tokuisaki_cd, // JOB: noh_tokuisakiCd
                                    wTodokesakiCD = mfileList[0].todokesaki_cd, // JOB: noh_todokesakiCd
                                    wKanriNo = "", // ""
                                    wVendorCd = mfileList[0].vendor_cd, //JOB: vendor_cd
                                    wMateVendorCd = "", // ""
                                    wSyukaDate = mfileList[0].syuka_date, //JOB: haiso_date
                                    wButsuryuNo = "", // ""
                                    wKamotuNo = kamotu_no, //JOB: kamotu_no
                                    wMatehan = mfileList[0].matehan, // JOB: noh_matehan
                                    wMatehanSu = matehanCnt.ToString(), // JOB: tumiko_su
                                    wHHT_no = "11101",
                                    wNohinKbn = "0",
                                    wKaisyuKbn = "",
                                    wTenkanState = "00",
                                    wSakiTokuisakiCD = "",
                                    wSakiTodokesakiCD = "",
                                    wNohinDate = prefs.GetString("nohin_date", ""), //FIX:setFixLength(8, JOB: nohin_date)
                                    wNohinTime = prefs.GetString("nohin_time", "") //FIX:setFixLength(4, JOB: nohin_time)
                                };

                                SndNohinWorkHelper nohinWorkHelper = new SndNohinWorkHelper();
                                nohinWorkHelper.Insert(sndNohinWork);

                                editor.PutBoolean("nohinWorkEndFlag", true);
                                editor.Apply();

                                kaizoButton.Visibility = ViewStates.Gone;
                                nohinWorkButton.Visibility = ViewStates.Visible;

                                return;
                            }
                        }
                    } 
                    );
                }
            )).Start();
                
            }
        }
    }
}