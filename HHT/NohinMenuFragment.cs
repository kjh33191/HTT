using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;

namespace HHT
{
    public class NohinMenuFragment : BaseFragment
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

            SetTitle("納品検品");
            SetFooterText("");

            var view = inflater.Inflate(Resource.Layout.fragment_menu_nohin, container, false);
            Button button1 = view.FindViewById<Button>(Resource.Id.btn_nohinMenu_mailNohin);
            button1.Click += delegate { StartFragment(FragmentManager, typeof(NohinMailBagNohinFragment)); }; // sagyou2

            Button button2 = view.FindViewById<Button>(Resource.Id.btn_nohinMenu_nohin);
            button2.Click += delegate { StartFragment(FragmentManager, typeof(NohinWorkFragment)); }; // sagyou2

            Button button3 = view.FindViewById<Button>(Resource.Id.btn_nohinMenu_kaisyu);
            button3.Click += delegate { StartFragment(FragmentManager, typeof(NohinKaisyuMenuFragment)); }; // sagyou2

            Button button4 = view.FindViewById<Button>(Resource.Id.btn_nohinMenu_mailKaisyu);
            button4.Click += delegate { StartFragment(FragmentManager, typeof(NohinMailBagKaisyuFragment)); }; // sagyou2

            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Num1)
            {
                StartFragment(FragmentManager, typeof(MatehanSelectFragment));
            }
            else if (keycode == Keycode.Num2)
            {
                StartFragment(FragmentManager, typeof(MatehanSelectFragment));
            }
            else if (keycode == Keycode.Num3)
            {
                StartFragment(FragmentManager, typeof(MatehanSelectFragment));
            }
            else if (keycode == Keycode.Num4)
            {
                StartFragment(FragmentManager, typeof(MatehanSelectFragment));
            }

            return true;
        }
    }
}