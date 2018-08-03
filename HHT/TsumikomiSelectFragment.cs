using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Newtonsoft.Json;

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
            etCourse.FocusChange += (sender, e) => { if (!e.HasFocus && etCourse.Text != "") SearchBinNo(); };
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

        private void SearchBinNo()
        {
            ProgressDialog progress = new ProgressDialog(this.Activity)
            {
                Indeterminate = true
            };
            progress.SetMessage("Contacting server. Please wait...");
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetCancelable(false);
            progress.Show();
            
            new Thread(new ThreadStart(delegate {
                Activity.RunOnUiThread(() =>
                {
                    Thread.Sleep(500);

                    Dictionary<string, string> param = new Dictionary<string, string>
                    {
                        { "kenpin_souko",  prefs.GetString("souko_cd", "103")},
                        { "kitaku_cd", prefs.GetString("kitaku_cd", "2") },
                        { "syuka_date", prefs.GetString("shuka_date", "180310") },
                        { "nohin_date", prefs.GetString("bin_no", "1") },
                        { "course", prefs.GetString("course", "310") },
                    };

                    //string resultJson = CommonUtils.Post(WebService.TUMIKOMI.TUMIKOMI010, param);
                    //Dictionary<string, string> result = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultJson);
                    Dictionary<string, string> result = new Dictionary<string, string>();

                    //string state = result["state"];
                    //string binNo = result["bin_no"];
                    //string kansenKbn = result["kansen_kbn"];

                    if (txtConfirmMsg.Visibility != ViewStates.Visible)
                    {
                        txtConfirmBin.Visibility = ViewStates.Visible;
                        txtConfirmMsg.Visibility = ViewStates.Visible;
                        txtConfirmBin.Text = 2 + " 便";

                        etSyukaDate.Focusable = false;
                        etCourse.Focusable = false;
                        etBinNo.Focusable = false;
                    }

                }
                );
                Activity.RunOnUiThread(() => progress.Hide());
            }
            )).Start();

        }

        private void Confirm()
        {
            if (txtConfirmMsg.Visibility != ViewStates.Visible)
            {
                SearchBinNo();
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