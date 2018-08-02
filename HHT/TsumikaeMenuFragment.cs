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
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_menu_tsumikae, container, false);
            
            SetTitle("積替移動");

            Bundle bundle = new Bundle();

            Button button1 = view.FindViewById<Button>(Resource.Id.btn_tsumikaeMenu_tanpin);
            button1.Click += delegate {
                bundle.PutInt("menuFlag", 1);
                StartFragment(FragmentManager, typeof(TsumikaeIdouFragment), bundle);
            };

            Button button2 = view.FindViewById<Button>(Resource.Id.btn_tsumikaeMenu_zenpin);
            button2.Click += delegate {
                bundle.PutInt("menuFlag", 2);
                StartFragment(FragmentManager, typeof(TsumikaeIdouFragment), bundle);
            };

            Button button3 = view.FindViewById<Button>(Resource.Id.btn_tsumikaeMenu_matehan);
            button3.Click += delegate {
                bundle.PutInt("menuFlag", 3);
                StartFragment(FragmentManager, typeof(TsumikaeIdouFragment), bundle);
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