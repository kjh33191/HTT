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
            confirmButton.Click += delegate {
                editor.Clear();
                editor.Commit();
                FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(0).Id, 0);
            };

            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F4)
            {
                editor.Clear();
                editor.Commit();
                FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(0).Id, 0);
            }

            return true;
        }
    }
}