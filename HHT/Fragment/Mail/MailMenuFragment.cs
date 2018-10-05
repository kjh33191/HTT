using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;

namespace HHT
{
    public class MailMenuFragment : BaseFragment
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
            view = inflater.Inflate(Resource.Layout.fragment_menu_mailbag, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetTitle("メールバック");
            SetFooterText("");

            Button button1 = view.FindViewById<Button>(Resource.Id.btn_mailMenu_regist);
            button1.Click += delegate {
                editor.PutBoolean("registFlg", true);
                editor.Apply();
                StartFragment(FragmentManager, typeof(MailManageSelectFragment));  };

            Button button2 = view.FindViewById<Button>(Resource.Id.btn_mailMenu_delete);
            button2.Click += delegate {
                editor.PutBoolean("registFlg", false);
                editor.Apply();
                StartFragment(FragmentManager, typeof(MailManageSelectFragment));
            };

            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Num1)
            {
                editor.PutBoolean("registFlg", true);
                editor.Apply();
                StartFragment(FragmentManager, typeof(MatehanSelectFragment));
            }
            else if (keycode == Keycode.Num2)
            {
                editor.PutBoolean("registFlg", false);
                editor.Apply();
                StartFragment(FragmentManager, typeof(MailManageSelectFragment));
            }


            return true;
        }
    }
}