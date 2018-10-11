using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using HHT.Resources.Model;

namespace HHT
{
    class VendorAllAdapter : BaseAdapter<KOSU190>
    {
        
        private List<KOSU190> vendorList;

        public VendorAllAdapter(List<KOSU190> vendorList)
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

                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.adapter_list_all_vendor, parent, false);
                view.FindViewById<TextView>(Resource.Id.vendorCode).Text = item.vendor_cd;
                view.FindViewById<TextView>(Resource.Id.vendorName).Text = item.vendor_nm;
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

        public override KOSU190 this[int position] => vendorList[position];

    }
}