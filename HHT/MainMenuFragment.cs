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
using Com.Densowave.Bhtsdk.Barcode;
using Java.Interop;

namespace HHT
{
    public class MainMenuFragment : BaseFragment
    {
        private string mUserKbn = "0";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_menu_main, container, false);
            LinearLayout layout = view.FindViewById<LinearLayout>(Resource.Id.linearLayout2);
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            /*
            String mString = prefs.GetString("souko_cd", "");
            String mString = prefs.GetString("sagyousya_cd", "");
            String mString = prefs.GetString("kitaku_cd", "");
            String mString = prefs.GetString("menu_kbn", "");
            String mString = prefs.GetString("souko_kbn", "");
            */

            // 管理者の場合
            
            Button btnNyuka = view.FindViewById<Button>(Resource.Id.btn_main_manager_nyuka);
            btnNyuka.Click += delegate { StartFragment(FragmentManager, typeof(NyukaMenuFragment)); };

            Button btnTsumikae = view.FindViewById<Button>(Resource.Id.btn_main_manager_tsumikae);
            btnTsumikae.Click += delegate { StartFragment(FragmentManager, typeof(TsumikaeMenuFragment)); };

            Button btnTsumikomi = view.FindViewById<Button>(Resource.Id.btn_main_manager_tsumikomi);
            btnTsumikomi.Click += delegate { StartFragment(FragmentManager, typeof(TsumikomiSelectFragment)); };

            Button btnNohin = view.FindViewById<Button>(Resource.Id.btn_main_manager_nohin);
            btnNohin.Click += delegate { StartFragment(FragmentManager, typeof(NohinSelectFragment)); };

            Button btnDataSend = view.FindViewById<Button>(Resource.Id.btn_main_manager_dataSend);
            Button btnMailBag = view.FindViewById<Button>(Resource.Id.btn_main_manager_mailBag);
            Button btnIdouRegist = view.FindViewById<Button>(Resource.Id.btn_main_manager_idousakiRegist);
            Button btnIdouNohin = view.FindViewById<Button>(Resource.Id.btn_main_manager_idousakiNohin);
            Button btnTc2 = view.FindViewById<Button>(Resource.Id.btn_main_manager_tc2);
            Button btnZaikou = view.FindViewById<Button>(Resource.Id.btn_main_manager_zaikou);

            SetTitle("業務メニュー");
            SetFooterText("");
            
            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Num1)
            {
                StartFragment(FragmentManager, typeof(NyukaMenuFragment));
            }else if (keycode == Keycode.Num2)
            {
                StartFragment(FragmentManager, typeof(NyukaMenuFragment));
            }

            return true;
        }
    }
}