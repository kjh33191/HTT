using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Beardedhen.Androidbootstrap;

namespace HHT
{
    public class NohinMailBagPasswordFragment : BaseFragment
    {
        private View view;
        private EditText etPassword;
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_nohin_mailBag_password, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetTitle("メールバック");

            etPassword = view.FindViewById<EditText>(Resource.Id.et_nohinMailbagPwd_password);

            string menuFlag = prefs.GetString("menu_flg", "1");
            
            BootstrapButton button = view.FindViewById<BootstrapButton>(Resource.Id.btn_nohinMailbagPwd_confirm);
            button.Click += delegate {
                //StartFragment(FragmentManager, typeof(NohinCompleteFragment));
                if (menuFlag == "1")
                {
                    editor.PutBoolean("mailBagFlag", true);
                    editor.Apply();

                    ShowDialog("報告", "メールバッグの\n納品が完了しました。", () => { FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(2).Id, 0); });
                }
                else
                {
                    ShowDialog("報告", "納品検品が\n完了しました。\n\nお疲れ様でした！", () => { FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(2).Id, 0); });
                }

                
            };
            
            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Num1)
            {
                StartFragment(FragmentManager, typeof(KosuMenuFragment));
            }
            
            return true;
        }

    }
}