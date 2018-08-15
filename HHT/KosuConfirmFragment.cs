using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;
using System.Collections.Generic;

namespace HHT
{
    public class KosuConfirmFragment : BaseFragment
    {
        private View view;
        private EditText txtDeliveryDate, txtTodokesaki, txtTokuisaki, txtBinNo;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_kosu_confirm, container, false);
            SetTitle("届先指定検品");
            SetFooterText("");

            Button confirmButton = view.FindViewById<Button>(Resource.Id.btn_confirm_confirmBtn);
            confirmButton.Click += delegate { StartFragment(FragmentManager, typeof(TodokeTyingWorkFragment)); };

            txtDeliveryDate = view.FindViewById<EditText>(Resource.Id.et_confirm_deliveryDate);
            txtTodokesaki = view.FindViewById<EditText>(Resource.Id.et_confirm_todokesaki);
            txtTokuisaki = view.FindViewById<EditText>(Resource.Id.et_confirm_tokuisaki);
            txtBinNo = view.FindViewById<EditText>(Resource.Id.et_confirm_binNo);

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            txtDeliveryDate.Text = prefs.GetString("deliveryDate", "");
            txtTodokesaki.Text = prefs.GetString("todokesaki_cd", "");
            txtTokuisaki.Text = prefs.GetString("tokuisaki_cd", "");
            txtBinNo.Text = prefs.GetString("bin_no", "");

            return view;
        }

    }
}