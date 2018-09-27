using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using HHT.Resources.Model;

namespace HHT
{
    class MikenAdapter : BaseAdapter
    {
        private List<KOSU120> mikenList;

        public MikenAdapter(List<KOSU120> mikenList)
        {
            this.mikenList = mikenList;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return mikenList[position].syuka_home;
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
                var item = mikenList[position];

                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.adapter_list_miken, parent, false);

                view.FindViewById<TextView>(Resource.Id.bunrui).Text = item.bunrui_nm;
                view.FindViewById<TextView>(Resource.Id.case_loca).Text = item.case_loca;
                view.FindViewById<TextView>(Resource.Id.kamotsu_no).Text = item.kamotsu_no;
                view.FindViewById<TextView>(Resource.Id.syuka_home).Text = item.syuka_home;

            }

            return view;
        }

        public override int Count
        {
            get
            {
                return mikenList.Count;
            }
        }

    }

    class MikenAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}