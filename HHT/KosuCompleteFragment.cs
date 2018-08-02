using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;

namespace HHT
{
    public class KosuCompleteFragment : BaseFragment
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
            SetTitle("届先指定検品");
            SetFooterText("");

            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            Button confirmButton = view.FindViewById<Button>(Resource.Id.btn_vender_confirm2);
            confirmButton.Click += delegate {
                editor.Clear();
                editor.Commit();
                StartFragment(FragmentManager, typeof(KosuMenuFragment));
            };

            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F4)
            {
                editor.Clear();
                editor.Commit();
                StartFragment(FragmentManager, typeof(KosuMenuFragment));
            }

            return true;
        }
    }
}