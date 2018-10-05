using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace HHT
{
    public class TsumikomiCompleteFragment : BaseFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_tsumikomi_complete, container, false);
            
            SetTitle("積込検品");

            Button confirm = view.FindViewById<Button>(Resource.Id.btn_tsumikomiComplete_confirm);
            confirm.Click += delegate { Confirm(); };

            return view;
        }

        private void Confirm()
        {
            // tenpo_zan_flg > 残り作業が存在留守場合、
            // scan_flg = false <-= 必要？
            // sagyou4
            
            FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(1).Id, 0);

            // 残り作業ない
            // System: Load("02_MainMenu", JOB: topmenu_flg, 0, 0, 0)
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F4)
            {
                StartFragment(FragmentManager, typeof(KosuMenuFragment));
            }
            
            return true;
        }

    }
}