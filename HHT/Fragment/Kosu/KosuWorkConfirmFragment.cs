using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using HHT.Resources.Model;
using Android.Media;
using Android.Content;
using Android.Preferences;
using Com.Beardedhen.Androidbootstrap;

namespace HHT
{
    public class KosuWorkConfirmFragment : BaseFragment
    {
        private View view;
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;
       
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_kosu_tyingConfirm, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();
            HideFooter();
            
            string kenpin_souko = prefs.GetString("souko_cd", "");
            string kitaku_cd = prefs.GetString("kitaku_cd", "");
            string syuka_date = prefs.GetString("syuka_date", "");
            string bin_no = prefs.GetString("bin_no", "");
            string tokuisaki_cd = prefs.GetString("tokuisaki_cd", "");
            string todokesaki_cd = prefs.GetString("todokesaki_cd", "");
            string vendor_cd = prefs.GetString("vendor_cd", "");

            view.FindViewById<BootstrapButton>(Resource.Id.stopButton).Click += delegate {
                FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(2).Id, 0);
            };
            
            try
            {
                KOSU110 nyukaStatus;

                if (prefs.GetInt(Const.KOSU_MENU_FLAG, (int)Const.KOSU_MENU.TODOKE) == (int)Const.KOSU_MENU.TODOKE)
                {
                    SetTitle("届先指定検品");

                    nyukaStatus = WebService.RequestKosu110(kenpin_souko, kitaku_cd, syuka_date, bin_no, tokuisaki_cd, todokesaki_cd);                    
                }
                else
                {
                    SetTitle("ベンダー指定検品");

                    nyukaStatus = WebService.RequestKosu115(kenpin_souko, kitaku_cd, syuka_date, vendor_cd);
                }

                view.FindViewById<TextView>(Resource.Id.txt_tyConfirm_case).Text = nyukaStatus.sum_case_sumi + "/" + nyukaStatus.sum_case;
                view.FindViewById<TextView>(Resource.Id.txt_tyConfirm_oricon).Text = nyukaStatus.sum_oricon_sumi + "/" + nyukaStatus.sum_oricon;
                view.FindViewById<TextView>(Resource.Id.txt_tyConfirm_huteikei).Text = nyukaStatus.sum_futeikei_sumi + "/" + nyukaStatus.sum_futeikei;
                view.FindViewById<TextView>(Resource.Id.txt_tyConfirm_hazai).Text = nyukaStatus.sum_hazai_sumi + "/" + nyukaStatus.sum_hazai;
                view.FindViewById<TextView>(Resource.Id.txt_tyConfirm_miseidou).Text = nyukaStatus.sum_ido_sumi + "/" + nyukaStatus.sum_ido;
                view.FindViewById<TextView>(Resource.Id.txt_tyConfirm_henpin).Text = nyukaStatus.sum_henpin_sumi + "/" + nyukaStatus.sum_henpin;
                view.FindViewById<TextView>(Resource.Id.txt_tyConfirm_hansoku).Text = nyukaStatus.sum_hansoku_sumi + "/" + nyukaStatus.sum_hansoku;
                view.FindViewById<TextView>(Resource.Id.txt_tyConfirm_kaisyu).Text = "0" + "/" + "0";
                view.FindViewById<TextView>(Resource.Id.txt_tyConfirm_total).Text = nyukaStatus.total_sumi + "/" + nyukaStatus.total;
                view.FindViewById<TextView>(Resource.Id.txt_tyConfirm_daisu).Text = nyukaStatus.sum_mate_cnt;

            }
            catch
            {
                Activity.RunOnUiThread(() =>
                {
                    ShowDialog("報告", "表示データがありません。", () => {
                        FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(3).Id, 0);
                     });
                });
            }

            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F1 || keycode == Keycode.F4)
            {
                FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(2).Id, 0);
            }

            return true;
        }
    }
}
 