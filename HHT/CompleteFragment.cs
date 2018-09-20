using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;

namespace HHT
{
    public class CompleteFragment : BaseFragment
    {
        private View view;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_kosu_complete, container, false);

            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetTitle(prefs.GetString("completeTitle", ""));
            SetFooterText("");

            TextView completeMsg = view.FindViewById<TextView>(Resource.Id.textView10);
            completeMsg.Text = prefs.GetString("completeMsg", "");

            Button confirmButton = view.FindViewById<Button>(Resource.Id.btn_vender_confirm2);
            confirmButton.Click += delegate { BackToMainMenu(); };

            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F4)
            {
                BackToMainMenu();
            }

            return true;
        }

        public override bool OnBackPressed()
        {
            return false;
        }

        private void BackToMainMenu()
        {
            string menu_kbn = prefs.GetString("menu_kbn", "");
            string driver_nm = prefs.GetString("driver_nm", "");
            string souko_cd = prefs.GetString("souko_cd", "");
            string souko_nm = prefs.GetString("souko_nm", "");
            string driver_cd = prefs.GetString("driver_cd", "");
            string kitaku_cd = prefs.GetString("kitaku_cd", "");
            string def_tokuisaki_cd = prefs.GetString("def_tokuisaki_cd", "");
            string tsuhshin_kbn = prefs.GetString("tsuhshin_kbn", "");
            string souko_kbn = prefs.GetString("souko_kbn", "");

            editor.Clear();
            editor.Commit();

            editor.PutString("menu_kbn", menu_kbn);
            editor.PutString("driver_nm", driver_nm);
            editor.PutString("souko_cd", souko_cd);
            editor.PutString("souko_nm", souko_nm);
            editor.PutString("driver_cd", driver_cd);
            editor.PutString("kitaku_cd", kitaku_cd);
            editor.PutString("def_tokuisaki_cd", def_tokuisaki_cd);
            editor.PutString("tsuhshin_kbn", tsuhshin_kbn);
            editor.PutString("souko_kbn", souko_kbn);
            editor.Apply();

            FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(0).Id, 0);
        }
    }
}