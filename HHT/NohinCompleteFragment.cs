using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;

namespace HHT
{
    public class NohinCompleteFragment : BaseFragment
    {
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_nohin_complete, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetTitle("納品検品");
            SetFooterText("");

            string menuFlag = prefs.GetString("menu_flg", "");

            TextView message = view.FindViewById<TextView>(Resource.Id.txt_nohinComplete_message);
            if (menuFlag == "1")
            {
                message.Text = "メールバッグの\n納品が完了しました。";
            }
            else
            {
                message.Text = "納品検品が\n完了しました。\n\nお疲れ様でした！";
            }
            
            
            Button confirmButton = view.FindViewById<Button>(Resource.Id.btn_nohinComplete_confirm);
            confirmButton.Click += delegate { FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(1).Id, 0); };

            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Num4)
            {
                FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(1).Id, 0);
            }
            return true;
        }
    }
}