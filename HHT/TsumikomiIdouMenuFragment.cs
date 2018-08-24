using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;

namespace HHT
{
    public class TsumikomiIdouMenuFragment : BaseFragment
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

            SetTitle("積込移動");
            SetFooterText("");

            var view = inflater.Inflate(Resource.Layout.fragment_menu_tsumikomi_idou, container, false);
            Button button1 = view.FindViewById<Button>(Resource.Id.btn_tsumikomiMenu_course);
            button1.Click += delegate { GoTodokeSelect(); }; // sagyou2

            Button button2 = view.FindViewById<Button>(Resource.Id.btn_tsumikomiMenu_tsumikae);
            button2.Click += delegate { GoVendorSelect(); }; // sagyou7

            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Num1)
            {
                GoTodokeSelect();
            }
            else if (keycode == Keycode.Num2)
            {
                GoVendorSelect();
            }

            return true;
        }

        private void GoTodokeSelect()
        {
            StartFragment(FragmentManager, typeof(TsumikomiManagerFragment));
        }

        private void GoVendorSelect()
        {
            editor.PutInt(Const.KOSU_MENU_FLAG, (int)Const.KOSU_MENU.VENDOR);
            editor.Apply();
            StartFragment(FragmentManager, typeof(TsumikaeIdouFragment));
        }
    }
}