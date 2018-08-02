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
    public class TsumikomiSelectFragment : BaseFragment
    {
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        EditText etSyukaDate, etCourse, etBinNo;
        TextView txtConfirmMsg, txtConfirmBin;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_tsumikomi_select, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();
            
            SetTitle("積込検品");
            
            Button btnConfirm = view.FindViewById<Button>(Resource.Id.btn_tsumikomiSelect_confirm);
            etSyukaDate = view.FindViewById<EditText>(Resource.Id.et_tsumikomiSelect_syukaDate);
            etCourse = view.FindViewById<EditText>(Resource.Id.et_tsumikomiSelect_course);
            etBinNo = view.FindViewById<EditText>(Resource.Id.et_tsumikomiSelect_binNo);
            txtConfirmBin = view.FindViewById<TextView>(Resource.Id.tv_tsumikomiSelect_confirmBin);
            txtConfirmMsg = view.FindViewById<TextView>(Resource.Id.tv_tsumikomiSelect_confirmMsg);

            btnConfirm.Click += delegate { Confirm(); };

            // 初期処理順
            // 1. 幹線便とメールバックを確認する。
            // ret = TUMIKOMI230

            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "souko_cd",  prefs.GetString("souko_cd", "103")},
                { "kitaku_cd", prefs.GetString("kitaku_cd", "2") },
                { "shuka_date", prefs.GetString("shuka_date", "180310") },
                { "bin_no", prefs.GetString("bin_no", "1") },
                { "course", prefs.GetString("course", "310") },
            };

            WebService.ReqeustTUMIKOMI230(param);


            return view;
        }

        private void Confirm()
        {
            if (txtConfirmMsg.Visibility != ViewStates.Visible)
            {
                txtConfirmBin.Visibility = ViewStates.Visible;
                txtConfirmMsg.Visibility = ViewStates.Visible;
                txtConfirmBin.Text = "9 便";

                etSyukaDate.Focusable = false;
                etCourse.Focusable = false;
                etBinNo.Focusable = false;

            }
            else
            {
                StartFragment(FragmentManager, typeof(TsumikomiSearchFragment));
            }
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {

            if (keycode == Keycode.F4)
            {
                Confirm();
                return false;
            }
            else if (keycode == Keycode.Back)
            {
                if (txtConfirmMsg.Visibility == ViewStates.Visible)
                {
                    txtConfirmBin.Visibility = ViewStates.Gone;
                    txtConfirmMsg.Visibility = ViewStates.Gone;

                    etSyukaDate.Focusable = true;
                    etCourse.Focusable = true;
                    etBinNo.Focusable = true;
                    return false;
                }
            }

            return true;
        }

    }
}