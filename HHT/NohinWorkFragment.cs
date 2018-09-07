using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;
using HHT.Resources.DataHelper;
using HHT.Resources.Model;
using System.Collections.Generic;
using System.Threading;

namespace HHT
{
    public class NohinWorkFragment : BaseFragment
    {
        private View view;
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        private TextView tvCase, tvOricon, tvSonota, tvIdo, tvMail, tvFuteikei, tvHansoku, tvTc, tvTsumidai, tvDai, tvAll;
        private int ko_su, maxko_su;

        MFileHelper mFilehelper;
        private List<MFile> tsumikomiDataList;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_nohin_work, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();


            InitComponent(); // 初期化
            GetTumisu(); // 積込台数取得
            
            ko_su = prefs.GetInt("ko_su", 0);

            // ?
            if (maxko_su <= ko_su)
            {
                //Return("sagyou5")
            }

            return view;
        }

        private void InitComponent()
        {
            SetTitle("納品検品");
            SetFooterText("F1:解除");

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
            tvDai = view.FindViewById<TextView>(Resource.Id.txt_nohinWork_daisu);
            tvAll = view.FindViewById<TextView>(Resource.Id.txt_nohinWork_all);

            mFilehelper = new MFileHelper();

        }

        private void GetTumisu()
        {
            string tokuisaki_cd = prefs.GetString("tokuisaki_cd", "0000");
            string todokesaki_cd = prefs.GetString("todokesaki_cd", "0374");
            
            MFile tsumikomi;

            int btvQty = 0;

            tsumikomiDataList = mFilehelper.SelectTsumikomiList(tokuisaki_cd, todokesaki_cd);
            // 원래 소스에서는 0건일 경우를 상정하지 않음.
            if (tsumikomiDataList.Count == 0)
            {
                // temp
                tsumikomi = new MFile
                {
                    kenpin_souko = "108",
                    kitaku_cd = "2",
                    syuka_date = "20180320",
                    course = "101",
                    bin_no = "1",
                    tokuisaki_cd = "0000",
                    todokesaki_cd = "0374",
                    state = "04",
                    tokuisaki_rk = "新白岡店",
                    bunrui = "03",
                    driver_cd = "99999",
                    matehan = "1",
                    butsuryu_no = "1",
                    kamotsu_no = "9800000001940005404809700021",
                    nohin_yti_time = "20180321"

                };
                mFilehelper.Insert(tsumikomi);
                btvQty = 1;
            }
            else
            {
                btvQty = tsumikomiDataList.Count;

                // 積込台数
                int idx = tvTsumidai.Text.IndexOf('/');
                string tsumidaiFull = btvQty.ToString();
                int tsumidai = (int.Parse(tvTsumidai.Text.Substring(0, idx)));
                tvTsumidai.Text = tsumidai + "/" + tsumidaiFull;

                // 総個数
                idx = tvAll.Text.IndexOf('/');
                tvAll.Text = (int.Parse(tvAll.Text.Substring(0, idx))) + "/" + btvQty.ToString();
                maxko_su = btvQty;
            }
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if(keycode == Keycode.F1)
            {
                EditText et = new EditText(this.Activity);
                
                AlertDialog.Builder ad = new AlertDialog.Builder(this.Activity);
                ad.SetTitle("Password");
                ad.SetView(et);
                ad.SetPositiveButton("Submit", delegate
                 {
                     // password テーブルからパスワード情報を取得する。

                     Toast.MakeText(this.Activity, "Submit Input: "+ et.Text, ToastLength.Short).Show();

                     if(et.Text == "")
                     {
                         SndNohinWorkHelper sndNohinWorkHelper = new SndNohinWorkHelper();
                         sndNohinWorkHelper.DeleteAll();

                         editor.PutBoolean("nohinWorkEndFlag", true);
                         editor.Apply();
                         StartFragment(FragmentManager, typeof(NohinCompleteFragment));
                     }
                     else
                     {
                         CommonUtils.AlertDialog(view, "エラー", "パスワードが違います。", null);
                         return;
                     }
                     
                 });
                ad.Show();
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
                kamotu_no = "9800000001940005404809700021";
                
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
                            CommonUtils.AlertDialog(view, "", "該当データがありません。", null);
                            return;
                        }

                        // JOB: mateCheck_inpData(JOB: noh_matehan) is false
                        // then "登録済みです。"
                        
                        SndNohinWorkHelper sndNohinWorkHelper = new SndNohinWorkHelper();
                        if (sndNohinWorkHelper.SelectNohinWorkWithKamotu(kamotu_no).Count > 0)
                        {
                            CommonUtils.AlertDialog(view, "", "登録済みです。", null);
                            return;
                        }
                        
                        switch ("0" + tsumikomi.bunrui)
                        {
                            case "01": tvCase.Text = (int.Parse(tvCase.Text) + 1).ToString(); break;
                            case "02": tvOricon.Text = (int.Parse(tvOricon.Text) + 1).ToString(); break;
                            case "03": tvSonota.Text = (int.Parse(tvSonota.Text) + 1).ToString(); break;
                            case "04": tvIdo.Text = (int.Parse(tvIdo.Text) + 1).ToString(); break;
                            case "05": tvMail.Text = (int.Parse(tvMail.Text) + 1).ToString(); break;
                            case "06": tvSonota.Text = (int.Parse(tvSonota.Text) + 1).ToString(); break;
                            case "07": tvFuteikei.Text = (int.Parse(tvFuteikei.Text) + 1).ToString(); break;
                            case "08": tvSonota.Text = (int.Parse(tvSonota.Text) + 1).ToString(); break;
                            case "09": tvHansoku.Text = (int.Parse(tvHansoku.Text) + 1).ToString(); break;
                            case "T": tvTc.Text = (int.Parse(tvTc.Text) + 1).ToString(); break;
                            default: tvSonota.Text = (int.Parse(tvSonota.Text) + 1).ToString(); break;
                        }

                        int idx = tvTsumidai.Text.IndexOf('/');
                        string tsumidaiFull = tvTsumidai.Text.Substring(idx + 1, tvTsumidai.Text.Length - (idx + 1));
                        int tsumidai = (int.Parse(tvTsumidai.Text.Substring(0, idx)) + 1);
                        tvTsumidai.Text = tsumidai + "/" + tsumidaiFull;


                        string tokuisaki_cd = prefs.GetString("tokuisaki_cd", "0000");
                        string todokesaki_cd = prefs.GetString("todokesaki_cd", "0374");
                        string matehanCd = tsumikomi.matehan;
                        List<MFile> mfileList = mFilehelper.SelectByMatehanCd(tokuisaki_cd, todokesaki_cd, matehanCd);

                        // 総個数
                        idx = tvAll.Text.IndexOf('/');
                        string full = tvTsumidai.Text.Substring(idx + 1, tvTsumidai.Text.Length - (idx + 1));
                        ko_su = int.Parse(tvAll.Text.Substring(0, idx)) + mfileList.Count;
                        tvAll.Text = ko_su + "/" + full;
                        

                        if (ko_su == maxko_su)
                        {
                            // レコード作成用　値取得
                            SndNohinWork sndNohinWork = new SndNohinWork
                            {
                                wPackage = "02",
                                wTerminalID = "", //Handy: serialId
                                wProgramID = prefs.GetString("program_id", "NOH"), //JOB: program_id
                                wSagyosyaCD = prefs.GetString("sagyosya", "99999"),
                                wSoukoCD = prefs.GetString("noh_soukoCd", "108"), //JOB: noh_soukoCd
                                wHaisoDate = prefs.GetString("noh_syukaDate", "20180320"), // noh_syukaDate
                                wBinNo = prefs.GetString("noh_binNo", "1"), //JOB: noh_binNo
                                wCourse = prefs.GetString("noh_course", "101"), //noh_course
                                wDriverCD = prefs.GetString("noh_tokuisakiCd", "99999"), // noh_driverCd
                                wTokuisakiCD = prefs.GetString("noh_tokuisakiCd", ""), // JOB: noh_tokuisakiCd
                                wTodokesakiCD = prefs.GetString("noh_todokesakiCd", ""), // JOB: noh_todokesakiCd
                                wKanriNo = "", // ""
                                wVendorCd = prefs.GetString("vendor_cd", ""), //JOB: vendor_cd
                                wMateVendorCd = "", // ""
                                wSyukaDate = "20180320", //JOB: haiso_date
                                wButsuryuNo = "", // ""
                                wKamotuNo = kamotu_no, //JOB: kamotu_no
                                wMatehan = prefs.GetString("noh_matehan", ""), // JOB: noh_matehan
                                wMatehanSu = prefs.GetString("tumiko_su", ""), // JOB: tumiko_su
                                wHHT_no = "",
                                wNohinKbn = "",
                                wKaisyuKbn = "", //FIX:setFixLength(1, "")
                                wTenkanState = "00", //FIX:setFixLength(2, "00")
                                wSakiTokuisakiCD = "", //FIX:setFixLength(13, "")
                                wSakiTodokesakiCD = "", //FIX:setFixLength(13, "")
                                wNohinDate = prefs.GetString("nohin_date", ""), //FIX:setFixLength(8, JOB: nohin_date)
                                wNohinTime = prefs.GetString("nohin_time", "") //FIX:setFixLength(4, JOB: nohin_time)
                            };

                            SndNohinWorkHelper nohinWorkHelper = new SndNohinWorkHelper();
                            nohinWorkHelper.Insert(sndNohinWork);

                            editor.PutBoolean("nohinWorkEndFlag", true);
                            editor.Apply();
                            
                            StartFragment(FragmentManager, typeof(NohinCompleteFragment));
                            return;
                        }
                    } 
                    );
                }
            )).Start();
                
            }
        }
    }
}