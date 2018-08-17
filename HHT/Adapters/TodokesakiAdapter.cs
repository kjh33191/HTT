﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HHT.Resources.Model;

namespace HHT
{
    class TodokesakiAdapter : BaseAdapter<Todokesaki>
    {
        private List<Todokesaki> items;

        public TodokesakiAdapter(List<Todokesaki> items)
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

            ProgressBar pgBar = view.FindViewById<ProgressBar>(Resource.Id.txt_adp_todoke_progressbar);
            
            pgBar.Max = int.Parse(item.kosu_kei);
            pgBar.Progress = int.Parse(item.kosu_sumi);

            return view;
        }

        public override int Count
        {
            get { return items.Count; }
        }

        public override Todokesaki this[int position] => items[position]; 
    }
}