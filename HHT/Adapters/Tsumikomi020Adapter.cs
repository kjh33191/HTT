using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using Com.Beardedhen.Androidbootstrap;
using HHT.Resources.Model;

namespace HHT
{
    class Tsumikomi020Adapter : BaseAdapter<TUMIKOMI020>
    {
        private List<TUMIKOMI020> items;

        public Tsumikomi020Adapter(List<TUMIKOMI020> items)
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
            view.FindViewById<TextView>(Resource.Id.txt_adp_todoke_time).Text = item.nohin_yti_time;
            view.FindViewById<TextView>(Resource.Id.txt_adp_todoke_tenpoName).Text = item.tokuisaki_rk;
            view.FindViewById<TextView>(Resource.Id.txt_adp_todoke_berth).Text = item.kansen_kbn;
            
            string sumi = item.Qty.Substring(0, 7);
            string kei = item.Qty.Substring(7, 7);
            string mate = item.Qty.Substring(14, 7);

            view.FindViewById<TextView>(Resource.Id.txt_adp_todoke_per).Text = int.Parse(sumi).ToString() + "/" + int.Parse(kei).ToString();
            
            BootstrapProgressBar pgBar = view.FindViewById<BootstrapProgressBar>(Resource.Id.txt_adp_todoke_progressbar);
            pgBar.Progress = ((int.Parse(sumi) / int.Parse(kei)) * 100);

            return view;
        }

        public override int Count
        {
            get { return items.Count; }
        }

        public override TUMIKOMI020 this[int position] => items[position]; 
    }
}