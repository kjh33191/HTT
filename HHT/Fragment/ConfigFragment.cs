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
        private EditText _HostIpEditText, _PortEditText;
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

            _HostIpEditText = view.FindViewById<EditText>(Resource.Id.hostIp);
            _HostIpEditText.Text = WebService.GetHostIpAddress();

            _PortEditText = view.FindViewById<EditText>(Resource.Id.port);
            _PortEditText.Text = WebService.GetPort();
            

            btnConfirm = view.FindViewById<Button>(Resource.Id.confirm);
            btnConfirm.Click += delegate {
                if (_HostIpEditText.Text == ""){
                    ShowDialog("報告", "ホストIPを入力してください。", () => { });
                    return;
                }

                ShowDialog("警告", "この設定でよろしいでしょうか？", () => {
                    new DataBase().SetHostIpAddress(_HostIpEditText.Text, _PortEditText.Text);
                    WebService.SetHostIpAddress(_HostIpEditText.Text, _PortEditText.Text);

                    FragmentManager.PopBackStack();
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
 