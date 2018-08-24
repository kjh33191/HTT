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

            SetTitle("納品検品");
            SetFooterText("F1:解除");

            TextView txtTokuisaki = view.FindViewById<TextView>(Resource.Id.txt_nohinwork_tokuisakiNm);
            txtTokuisaki.Text = "シーエスイー"; // prefs.GetString("def_tokuisaki_cd", "");
            TextView txtTodokesaki = view.FindViewById<TextView>(Resource.Id.txt_nohinwork_todokesakiNm);
            txtTodokesaki.Text = "水天宮店";

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

            //If JOB:ko_su == JOB:maxko_su Then
            //Return("sagyou5")

            ko_su = 0;



            // tumidai_su 積込台数를 구한다. 
            // m_handy_id 파일안에 5번째 인덱스가 마테한수
            // 토쿠이사키, 토도케사키로 검색한 후, 
            // 복수개의 레코드를 
            MFile tsumikomi;
            MFileHelper helper = new MFileHelper();

            int btvQty = 0;

            tsumikomiDataList = helper.SelectTsumikomiList("0000", "0374");
            if (tsumikomiDataList.Count == 0)
            {
                // エラー
                
                tsumikomi = new MFile
                {
                    kenpin_souko = "108",
                    kitaku_cd = "2",
                    syuka_date = "20180320", 
                    course = "101",
                    bin_no = "1",
                    tokuisaki_cd = "0000",
                    todokesaki_cd = "0374",
                    state="04",
                    tokuisaki_rk = "新白岡店",
                    bunrui = "03バラ",
                    driver_cd = "99999",
                    matehan = "1",
                    butsuryu_no = "1",
                    kamotsu_no = "9800000001940005404809700021",
                    nohin_yti_time = "20180321"
                    
                };
                helper.Insert(tsumikomi);

            }
            else
            {
                // 마테한 수 , 츠미다이수
                btvQty = tsumikomiDataList.Count;
            }
            
            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if(keycode == Keycode.F1)
            {
                CommonUtils.AlertConfirm(view, "", "納品業務を終了しますか？", (flag) => {
                    if (flag)
                    {
                        // SndMBN_+ 시리얼 +  .txt의 Empty File을 만든다. 
                        // 이미 존재한다면 패스
                        // const DEF_NOHIN    = "SndNOH_"	//納品業務
                        // 위에 파일을 지운다.
                        FragmentManager.PopBackStack();
                    }
                    else
                    {

                    }
                });
            }
            return true;
        }

        public override void OnBarcodeDataReceived(BarcodeDataReceivedEvent_ dataReceivedEvent)
        {
            IList<BarcodeDataReceivedEvent_.BarcodeData_> listBarcodeData = dataReceivedEvent.BarcodeData;

            foreach (BarcodeDataReceivedEvent_.BarcodeData_ barcodeData in listBarcodeData)
            {
                string kamotu_no = barcodeData.Data;

                //if (!checkData(kamotu_no))
                //{
                // "該当データがありません。"
                //    return;
                //}

                // JOB: mateCheck_inpData(JOB: noh_matehan) is false
                // then "登録済みです。"

                //JOB:check_inpData(JOB:kamotu_no) is false Then
                // then "登録済みです。"

                // Log.d("MAIN NOHIN", JOB:terminal_id, JOB:tokuisaki_cd, JOB:todokesaki_cd, "INPUT:" & JOB:kamotu_no)

                //JOB:get_matehan(JOB:kamotu_no)
                //JOB: get_bunrui(JOB: matehan_cd)
                //JOB: get_matecnt()
                // Return("sagyou8")

                // 마테한 카운트
                // btvTokuisaki, btvTodokesaki, 7(04) m 

                // 분류를 보고 카운트

                
                MFile tsumikomi = new MFile();
                tsumikomi.bunrui = "01";

                new Thread(new ThreadStart(delegate {
                    Activity.RunOnUiThread(() =>
                    {
                        switch (tsumikomi.bunrui)
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

                        //tvTsumidai.Text = (int.Parse(tvTsumidai.Text) + 1).ToString();
                        
                        ko_su = ko_su + 1;
                        maxko_su = 3;
                        
                        if (ko_su == maxko_su)
                        {
                            // レコード作成用　値取得
                            SndNohinWork sndNohinWork = new SndNohinWork
                            {
                                wPackage = "02",
                                wTerminalID = "", //Handy: serialId
                                wProgramID = "", //JOB: program_id
                                wSagyosyaCD = "99999",
                                wSoukoCD = "108", //JOB: noh_soukoCd
                                wHaisoDate = "20180320", // noh_syukaDate
                                wBinNo = "1", //JOB: noh_binNo
                                wCourse = "", //noh_course
                                wDriverCD = "99999", // noh_driverCd
                                wTokuisakiCD = "", // JOB: noh_tokuisakiCd
                                wTodokesakiCD = "", // JOB: noh_todokesakiCd
                                wKanriNo = "", // ""
                                wVendorCd = "", //JOB: vendor_cd
                                wMateVendorCd = "", // ""
                                wSyukaDate = "20180320", //JOB: haiso_date
                                wButsuryuNo = "", // ""
                                wKamotuNo = kamotu_no, //JOB: kamotu_no
                                wMatehan = "", // JOB: noh_matehan
                                wMatehanSu = "", // JOB: tumiko_su
                                wHHT_no = "",
                                wNohinKbn = "",
                                wKaisyuKbn = "", //FIX:setFixLength(1, "")
                                wTenkanState = "", //FIX:setFixLength(2, "00")
                                wSakiTokuisakiCD = "", //FIX:setFixLength(13, "")
                                wSakiTodokesakiCD = "", //FIX:setFixLength(13, "")
                                wNohinDate = "", //FIX:setFixLength(8, JOB: nohin_date)
                                wNohinTime = "" //FIX:setFixLength(4, JOB: nohin_time)
                            };

                            SndNohinWorkHelper nohinWorkHelper = new SndNohinWorkHelper();
                            nohinWorkHelper.Insert(sndNohinWork);
                            
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