using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;

namespace HHT
{
    public class NohinKaisyuMatehanFragment : BaseFragment
    {
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetTitle("マテハン回収");
            SetFooterText("");

            var view = inflater.Inflate(Resource.Layout.fragment_nohin_kaisyu_matehan, container, false);
            /*
            Button button1 = view.FindViewById<Button>(Resource.Id.btn_nohinMenu_mailNohin);
            button1.Click += delegate { StartFragment(FragmentManager, typeof(MatehanSelectFragment)); }; // sagyou2
            */
            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Num1)
            {
                StartFragment(FragmentManager, typeof(MatehanSelectFragment));
            }

            return true;
        }
    }
}