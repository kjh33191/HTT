using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;
using Newtonsoft.Json;
using HHT.Resources.Model;
using static Android.App.FragmentManager;
using Android.Media;
using Android.Net;
using Android.Content;
using Android.Preferences;
using System.Threading;

namespace HHT
{
    public class KosuTyingConfrimFragment : BaseFragment
    {
        private View view;
        private TextView txtCase, txtHuteikei
            , txtMiseidou, txtHansoku, txtTotal
            , txtOricon, txtHazai, txtHenpin, txtKaisyu, txtDaisu;
        private ToneGenerator toneGenerator;
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        private int kosuMenuflag;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_kosu_tyingConfirm, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            kosuMenuflag = prefs.GetInt(Const.KOSU_MENU_FLAG, (int)Const.KOSU_MENU.TODOKE); // 画面区分
            
            string kenpin_souko = prefs.GetString("souko_cd", "");
            string kitaku_cd = prefs.GetString("kitaku_cd", "");
            string syuka_date = prefs.GetString("syuka_date", "");
            string bin_no = prefs.GetString("bin_no", "");
            string tokuisaki_cd = prefs.GetString("tokuisaki_cd", "");
            string todokesaki_cd = prefs.GetString("todokesaki_cd", "");
            string vendor_cd = prefs.GetString("vendor_cd", "");

            txtCase = view.FindViewById<TextView>(Resource.Id.txt_tyConfirm_case);
            txtHuteikei = view.FindViewById<TextView>(Resource.Id.txt_tyConfirm_huteikei);
            txtMiseidou = view.FindViewById<TextView>(Resource.Id.txt_tyConfirm_miseidou);
            txtHansoku = view.FindViewById<TextView>(Resource.Id.txt_tyConfirm_hansoku);
            txtTotal = view.FindViewById<TextView>(Resource.Id.txt_tyConfirm_total);

            txtOricon = view.FindViewById<TextView>(Resource.Id.txt_tyConfirm_oricon);
            txtHazai = view.FindViewById<TextView>(Resource.Id.txt_tyConfirm_hazai);
            txtHenpin = view.FindViewById<TextView>(Resource.Id.txt_tyConfirm_henpin);
            txtKaisyu = view.FindViewById<TextView>(Resource.Id.txt_tyConfirm_kaisyu);
            txtDaisu = view.FindViewById<TextView>(Resource.Id.txt_tyConfirm_daisu);
            

            try
            {
                if (kosuMenuflag == (int)Const.KOSU_MENU.TODOKE)
                {

                    SetTitle("届先指定検品");
                    SetFooterText("F1：中断");

                    KOSU110 kosu110 = WebService.RequestKosu110(kenpin_souko, kitaku_cd, syuka_date, bin_no, tokuisaki_cd, todokesaki_cd);
                    
                    txtCase.Text = kosu110.sum_case_sumi + "/" + kosu110.sum_case;
                    txtOricon.Text = kosu110.sum_oricon_sumi + "/" + kosu110.sum_oricon;
                    txtHuteikei.Text = kosu110.sum_futeikei_sumi + "/" + kosu110.sum_futeikei;
                    txtHazai.Text = kosu110.sum_hazai_sumi + "/" + kosu110.sum_hazai;
                    txtMiseidou.Text = kosu110.sum_ido_sumi + "/" + kosu110.sum_ido;
                    txtHenpin.Text = kosu110.sum_henpin_sumi + "/" + kosu110.sum_henpin;
                    txtHansoku.Text = kosu110.sum_hansoku_sumi + "/" + kosu110.sum_hansoku;
                    txtKaisyu.Text = "0" + "/" + "0".PadLeft(3, ' ');
                    txtTotal.Text = kosu110.sum_tc_sumi + "/" + kosu110.sum_tc;
                    txtDaisu.Text = kosu110.sum_mate_cnt.PadLeft(3, ' ');

                }
                else if (kosuMenuflag == (int)Const.KOSU_MENU.VENDOR)
                {
                    SetTitle("ベンダー指定検品");
                    SetFooterText("F1：中断");

                    KOSU115 kosu115 = WebService.RequestKosu115(kenpin_souko, kitaku_cd, syuka_date, vendor_cd);

                    txtCase.Text = kosu115.sum_case_sumi + "/" + kosu115.sum_case;
                    txtOricon.Text = kosu115.sum_oricon_sumi + "/" + kosu115.sum_oricon;
                    txtHuteikei.Text = kosu115.sum_futeikei_sumi + "/" + kosu115.sum_futeikei;
                    txtHazai.Text = kosu115.sum_hazai_sumi + "/" + kosu115.sum_hazai;
                    txtMiseidou.Text = kosu115.sum_ido_sumi + "/" + kosu115.sum_ido;
                    txtHenpin.Text = kosu115.sum_henpin_sumi + "/" + kosu115.sum_henpin;
                    txtHansoku.Text = kosu115.sum_hansoku_sumi + "/" + kosu115.sum_hansoku;
                    txtKaisyu.Text = "0" + "/" + "0".PadLeft(3, ' ');
                    txtTotal.Text = kosu115.sum_tc_sumi + "/" + kosu115.sum_tc;
                    txtDaisu.Text = kosu115.sum_mate_cnt.PadLeft(3, ' ');

                }
            }
            catch
            {
                new Thread(new ThreadStart(delegate {
                    Activity.RunOnUiThread(() =>
                    {
                        CommonUtils.AlertDialog(view, "", "表示データがありません。", 
                            ()=> FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(3).Id, 0)
                        );
                    }
                    );
                })
                ).Start();
            }

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();
            
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F1 || keycode == Keycode.F4)
            {
                IBackStackEntry entry = FragmentManager.GetBackStackEntryAt(2);
                int id = entry.Id;
                FragmentManager.PopBackStack(id, 0);
            }

            return true;
        }
    }
}
 