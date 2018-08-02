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
    public class NyukaMenuFragment : BaseFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_menu_nyuka, container, false);

            SetTitle("入荷検品");

            Button button2 = view.FindViewById<Button>(Resource.Id.singleInspButton);
            button2.Click += delegate { StartFragment(FragmentManager, typeof(KosuMenuFragment));};
            
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