using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;

namespace HHT
{
    public class NohinKaisyuShohinFragment : BaseFragment
    {
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetTitle("商品回収");
            SetFooterText("");

            var view = inflater.Inflate(Resource.Layout.fragment_nohin_kaisyu_shohin, container, false);
            
            Button button1 = view.FindViewById<Button>(Resource.Id.btn_nohinKaisyuShohin_confirm);
            button1.Click += delegate {
                string confirmMsg = @"
シーエスイー
水天宮店
移動[  2] 返品[  2]
破材[  1] ﾒｰﾙ [  1]
他　[  1]

総個数(  7)

よろしいですか？
                                        ";

                CommonUtils.AlertConfirm(view, "確認", confirmMsg, (flag) =>
                 {
                     if (flag)
                     {

                     }
                     else
                     {

                     }
                 });
            }; 
            
            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.Num1)
            {
                StartFragment(FragmentManager, typeof(MatehanSelectFragment));
            }

            return true;
        }
    }
}