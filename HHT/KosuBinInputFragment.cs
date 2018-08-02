using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using System;

namespace HHT
{
    public class KosuBinInputFragment : BaseFragment
    {
        private View view;
        private bool confirmFlag;
        private string deliveryDate, tokuisaki, todokesaki;
        private EditText etDeliveryDate, etBinNo;
        private Button btnConfirm;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            SetTitle("届先指定検品");
            SetFooterText("F4:確定");
            
            view = inflater.Inflate(Resource.Layout.fragment_kosu_bin_input, container, false);

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Application.Context);

            // parameter setting 
            confirmFlag = prefs.GetBoolean("isConfirm", false);
            deliveryDate = prefs.GetString("deliveryDate", "");
            tokuisaki = prefs.GetString("tokuisaki", "");
            todokesaki = prefs.GetString("todokesaki", "");
            etDeliveryDate = view.FindViewById<EditText>(Resource.Id.et_binInput_deliveryDate);
            etDeliveryDate.Text = deliveryDate;
            
            etBinNo = view.FindViewById<EditText>(Resource.Id.et_binInput_binNo);
            etBinNo.RequestFocus();

            btnConfirm = view.FindViewById<Button>(Resource.Id.btn_binInput_confirm);
            btnConfirm.Click += delegate {
                ISharedPreferencesEditor editor = prefs.Edit();
                editor.PutString("binNo", etBinNo.Text);
                editor.Apply();
                if (confirmFlag)
                {
                    StartFragment(FragmentManager, typeof(KosuConfirmFragment));
                }
                else
                {
                    StartFragment(FragmentManager, typeof(KosuSearchFragment));
                }
                
            };

            return view;
        }


        public override void OnResume()
        {

            base.OnResume();
        }
        

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F1)
            {
                
            }
            else if (keycode == Keycode.F3)
            {
               
            }
            else if (keycode == Keycode.Back)
            {
               
            }

            return true;
        }
    }
}
 