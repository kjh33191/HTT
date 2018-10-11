using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;

namespace HHT
{
    public class KosuMenuFragment : BaseFragment
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

            SetTitle("入荷検品");
            SetFooterText("");
            HideFooter();

            var view = inflater.Inflate(Resource.Layout.fragment_menu_kosu, container, false);
            Button button1 = view.FindViewById<Button>(Resource.Id.todokeInspButton);
            button1.Click += delegate { GoTodokeSelect(); }; // sagyou2

            Button button2 = view.FindViewById<Button>(Resource.Id.vendaInspButton);
            button2.Click += delegate { GoVendorSelect(); }; // sagyou7

            Button button3 = view.FindViewById<Button>(Resource.Id.baraInspButton);
            button3.Click += delegate { GoBaraSelect(); }; // sagyou20_1

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
            else if (keycode == Keycode.Num3)
            {
                GoBaraSelect();
            }

            return true;
        }

        private void GoTodokeSelect()
        {
            editor.PutString("case_su", "0");
            editor.PutString("oricon_su", "0");
            editor.PutString("futeikei_su", "0");
            editor.PutString("ido_su", "0");
            editor.PutString("hazai_su", "0");
            editor.PutString("henpin_su", "0");
            editor.PutString("hansoku_su", "0");
            editor.PutString("kaisyu_su", "0");
            editor.PutString("ko_su", "0");
            editor.PutString("dai_su", "0");
            editor.PutInt(Const.KOSU_MENU_FLAG, (int)Const.KOSU_MENU.TODOKE);
            editor.Apply();
            StartFragment(FragmentManager, typeof(KosuSelectFragment));
        }

        private void GoVendorSelect()
        {
            editor.PutString("case_su", "0");
            editor.PutString("oricon_su", "0");
            editor.PutString("futeikei_su", "0");
            editor.PutString("ido_su", "0");
            editor.PutString("hazai_su", "0");
            editor.PutString("henpin_su", "0");
            editor.PutString("hansoku_su", "0");
            editor.PutString("kaisyu_su", "0");
            editor.PutString("ko_su", "0");
            editor.PutString("dai_su", "0");
            editor.PutInt(Const.KOSU_MENU_FLAG, (int)Const.KOSU_MENU.VENDOR);
            editor.Apply();
            StartFragment(FragmentManager, typeof(KosuSelectFragment));
        }

        private void GoBaraSelect()
        {
            editor.PutString("case_su", "0");
            editor.PutString("oricon_su", "0");
            editor.PutString("futeikei_su", "0");
            editor.PutString("ido_su", "0");
            editor.PutString("hazai_su", "0");
            editor.PutString("henpin_su", "0");
            editor.PutString("hansoku_su", "0");
            editor.PutString("kaisyu_su", "0");
            editor.PutString("ko_su", "0");
            editor.PutString("dai_su", "0");
            editor.PutInt(Const.KOSU_MENU_FLAG, (int)Const.KOSU_MENU.BARA);
            editor.Apply();
            StartFragment(FragmentManager, typeof(KosuSelectFragment));
        }
    }
}