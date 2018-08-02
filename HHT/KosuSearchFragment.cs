using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public class KosuSearchFragment : BaseFragment
    {
        private View view;
        private Adapter1 adapter1;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_kosu_search, container, false);

            SetTitle("届先指定検品");
            SetFooterText("");

            ListView listView = view.FindViewById<ListView>(Resource.Id.listView1);
            List<string> temp = new List<string>
            {
                "0111",
                "0222",
                "0333",
                "0444",
                "0555",
                "0666",
                "0777",
                "0888",
                "0999",
                "0123",
                "0213"
            };

            listView.ItemClick += listView_ItemClick;

            adapter1 = new Adapter1(temp);
            listView.Adapter = adapter1;
            
            return view;
        }

        void listView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //Get our item from the list adapter
            var item = this.adapter1.GetItem(e.Position);

            //Make a toast with the item name just to show it was clicked
            // CommonUtils.ShowAlertDialog(view, "Alert", (e.Position + 1).ToString() + "번째 아이템을 눌렀습니다." );
            // CommonUtils.AlertConfirm(view, "Alert", (e.Position + 1).ToString() + "번째 아이템을 눌렀습니다.", confirm());

            // TOOD
            // parameter setting 

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            ISharedPreferencesEditor editor = prefs.Edit();

            //editor.PutString("deliveryDate", etDeliveryDate.Text);
            //editor.PutString("tokuisaki", etTokuisaki.Text);
            editor.PutString("todokesaki", item.ToString());

            editor.Apply();


            StartFragment(FragmentManager, typeof(KosuConfirmFragment));
            
        }

        void confirm()
        {
            CommonUtils.ShowAlertDialog(view, "Alert", "Alert");
        }



    }
}