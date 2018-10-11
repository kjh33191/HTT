using Android.OS;
using Android.Views;
using Com.Beardedhen.Androidbootstrap;

namespace HHT
{
    public class TsumikomiManagerFragment : BaseFragment
    {
        private View view;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_tsumikomi_manager, container, false);
            SetTitle("積込検品");
            
            BootstrapButton managerConfirmBtn = view.FindViewById<BootstrapButton>(Resource.Id.btn_tsumikomiManger_managerConfirm);
            managerConfirmBtn.Click += delegate {
                StartFragment(FragmentManager, typeof(TsumikomiPassFragment));
            };
            
            return view;
        }
    }
}
 