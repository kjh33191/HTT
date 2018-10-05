using Android.OS;
using Android.Views;
using Android.Widget;
using HHT.Resources.DataHelper;
using Android.App;
using Android.Content;
using Android.Preferences;

namespace HHT
{
    public class ConfigFragment : BaseFragment
    {
        private readonly string TAG = "ConfigFragment";

        private View view;
        private EditText etHostIp;
        private Button btnConfirm;

        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_config, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();
            
            SetTitle("ログイン");
            SetFooterText("");

            etHostIp = view.FindViewById<EditText>(Resource.Id.hostIp);
            etHostIp.Text = WebService.GetHostIpAddress();

            btnConfirm = view.FindViewById<Button>(Resource.Id.confirm);
            btnConfirm.Click += delegate {
                if (etHostIp.Text == ""){
                    CommonUtils.AlertDialog(view, "", "ホストIPを入力してください。", null);
                    return;
                }

                CommonUtils.AlertConfirm(view, "", "この設定でよろしいでしょうか？", (flag)=>
                {
                    if (flag)
                    {
                        new DataBase().SetHostIpAddress(etHostIp.Text);
                        WebService.SetHostIpAddress(etHostIp.Text);

                        FragmentManager.PopBackStack();

                    }
                });
            };

            return view;
        }
        
        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            return true;
        }
    }
}
 