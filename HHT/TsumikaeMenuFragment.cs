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

namespace HHT
{
    public class TsumikaeMenuFragment : BaseFragment
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
            view = inflater.Inflate(Resource.Layout.fragment_menu_tsumikae, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();
            
            SetTitle("積替移動");

            
            Button button1 = view.FindViewById<Button>(Resource.Id.btn_tsumikaeMenu_tanpin);
            button1.Click += delegate {
                editor.PutInt("menuFlag", 1);
                editor.Apply();
                StartFragment(FragmentManager, typeof(TsumikaeIdouFragment));
            };

            Button button2 = view.FindViewById<Button>(Resource.Id.btn_tsumikaeMenu_zenpin);
            button2.Click += delegate {
                editor.PutInt("menuFlag", 2);
                editor.Apply();
                StartFragment(FragmentManager, typeof(TsumikaeIdouFragment));
            };

            Button button3 = view.FindViewById<Button>(Resource.Id.btn_tsumikaeMenu_matehan);
            button3.Click += delegate {
                editor.PutInt("menuFlag", 3);
                editor.Apply();
                StartFragment(FragmentManager, typeof(TsumikaeIdouFragment));
            };

            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Num1)
            {
                editor.PutInt("menuFlag", 1);
                editor.Apply();
                StartFragment(FragmentManager, typeof(TsumikaeIdouFragment));
            }
            else if (keycode == Keycode.Num2)
            {
                editor.PutInt("menuFlag", 2);
                editor.Apply();
                StartFragment(FragmentManager, typeof(TsumikaeIdouFragment));
            }
            else if (keycode == Keycode.Num3)
            {
                editor.PutInt("menuFlag", 3);
                editor.Apply();
                StartFragment(FragmentManager, typeof(TsumikaeIdouFragment));
            }

            return true;
        }

    }
}