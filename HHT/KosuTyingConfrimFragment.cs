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

namespace HHT
{
    public class KosuTyingConfrimFragment : BaseFragment
    {
        private View view;
        private TextView txtCase, txtHuteikei
            , txtMiseidou, txtHansoku, txtTotal
            , txtOricon, txtHazai, txtHenpin, txtKaisyu, txtDaisu;
        private ToneGenerator toneGenerator;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            SetTitle("届先指定検品");
            SetFooterText("F1：中断");
            
            view = inflater.Inflate(Resource.Layout.fragment_kosu_tyingConfirm, container, false);

            // todoke KOSU110
            // vendor KOSU115
            // input parameter 
            // kenpin_souko, kitaku_cd, syuka_date, bin_no, todokesaki_cd, todokesaki_cd

            KOSU110 kosu110 = new KOSU110();

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
 