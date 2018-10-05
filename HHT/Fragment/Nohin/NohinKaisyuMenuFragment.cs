using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;

namespace HHT
{
    public class NohinKaisyuMenuFragment : BaseFragment
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

            SetTitle("回収業務");
            SetFooterText("");

            var view = inflater.Inflate(Resource.Layout.fragment_menu_nohin_kaisyu, container, false);
            Button button1 = view.FindViewById<Button>(Resource.Id.btn_kaisyuMenu_sohin);
            button1.Click += delegate { StartFragment(FragmentManager, typeof(NohinKaisyuShohinFragment)); }; 

            Button button2 = view.FindViewById<Button>(Resource.Id.btn_kaisyuMenu_matehan);
            button2.Click += delegate { StartFragment(FragmentManager, typeof(NohinKaisyuMatehanFragment)); };

            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Num1)
            {
                StartFragment(FragmentManager, typeof(NohinKaisyuShohinFragment));
            }
            else if (keycode == Keycode.Num2)
            {
                StartFragment(FragmentManager, typeof(NohinKaisyuMatehanFragment));
            }

            return true;
        }
    }
}