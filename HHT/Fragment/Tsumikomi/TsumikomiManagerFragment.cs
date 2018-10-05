using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using HHT.Resources.Model;

namespace HHT
{
    public class TsumikomiManagerFragment : BaseFragment
    {
        private View view;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_tsumikomi_manager, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetTitle("積込検品");
            SetFooterText("");


            Button managerConfirmBtn = view.FindViewById<Button>(Resource.Id.btn_tsumikomiManger_managerConfirm);
            managerConfirmBtn.Click += delegate {
                view.FindViewById<LinearLayout>(Resource.Id.lo_tsumikomiManager_msg).Visibility = ViewStates.Gone;
                view.FindViewById<LinearLayout>(Resource.Id.lo_tsumikomiManager_pwd).Visibility = ViewStates.Visible;
            };

            Button pwdConfirmBtn = view.FindViewById<Button>(Resource.Id.btn_tsumikomiManger_pwdConfirm);
            pwdConfirmBtn.Click += delegate { CheckPassword(); };

            return view;
        }

        private void CheckPassword()
        {

        }

    }
}
 