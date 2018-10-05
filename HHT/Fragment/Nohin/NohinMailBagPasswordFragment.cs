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
    public class NohinMailBagPasswordFragment : BaseFragment
    {
        private View view;
        private EditText etPassword;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_nohin_mailBag_password, container, false);

            SetTitle("メールバック");

            etPassword = view.FindViewById<EditText>(Resource.Id.et_nohinMailbagPwd_password);

            Button button = view.FindViewById<Button>(Resource.Id.btn_nohinMailbagPwd_confirm);
            button.Click += delegate { StartFragment(FragmentManager, typeof(NohinCompleteFragment)); };
            
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