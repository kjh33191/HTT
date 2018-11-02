using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using HHT.Resources.Model;
using Com.Beardedhen.Androidbootstrap;
using System;

namespace HHT
{
    class TodokesakiAdapter : BaseAdapter<KOSU060>
    {
        private List<KOSU060> items;

        public TodokesakiAdapter(List<KOSU060> items)
        {
            this.items = items;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return items[position].todokesaki_cd;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            var item = items[position];

            if (view == null)
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.adapter_list_todoke, parent, false);

            view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.adapter_list_todoke, parent, false);
            view.FindViewById<TextView>(Resource.Id.txt_adp_todoke_time).Text = item.tsumikomi_yti_time;
            view.FindViewById<TextView>(Resource.Id.txt_adp_todoke_tenpoName).Text = item.tokuisaki_rk;
            view.FindViewById<TextView>(Resource.Id.txt_adp_todoke_berth).Text = item.berth;
            view.FindViewById<TextView>(Resource.Id.txt_adp_todoke_per).Text = item.kosu_sumi + "/" + item.kosu_kei;
            
            BootstrapProgressBar pgBar = view.FindViewById<BootstrapProgressBar>(Resource.Id.txt_adp_todoke_progressbar);
            pgBar.Progress = Convert.ToInt32((double.Parse(item.kosu_sumi) / double.Parse(item.kosu_kei)) * 100);

            return view;
        }

        public override int Count
        {
            get { return items.Count; }
        }

        public override KOSU060 this[int position] => items[position]; 
    }
}