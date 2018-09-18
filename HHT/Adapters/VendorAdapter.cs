using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using HHT.Resources.Model;

namespace HHT
{
    class VendorAdapter : BaseAdapter
    {
        
        Context context;
        private List<KOSU095> vendorList;

        public VendorAdapter(List<KOSU095> vendorList)
        {
            this.vendorList = vendorList;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return vendorList[position].vendor_cd;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            VendorAdapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as VendorAdapterViewHolder;

            if (holder == null)
            {
                holder = new VendorAdapterViewHolder();
                var item = vendorList[position];

                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.adapter_list_vendor, parent, false);
                view.FindViewById<TextView>(Resource.Id.nyukaTime).Text = item.ven_yti_time;
                view.FindViewById<TextView>(Resource.Id.vendorName).Text = item.vendor_nm;
                
                string sumi = item.kosu_sumi;
                string kei = item.kosu_kei;

                view.FindViewById<TextView>(Resource.Id.txt_adp_todoke_per).Text = int.Parse(sumi).ToString() + "/" + int.Parse(kei).ToString();

                ProgressBar pgBar = view.FindViewById<ProgressBar>(Resource.Id.txt_adp_todoke_progressbar);

                pgBar.Max = int.Parse(kei);
                pgBar.Progress = int.Parse(sumi);
                
            }

            return view;
        }

        public override int Count
        {
            get
            {
                return vendorList.Count;
            }
        }

    }

    class VendorAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}