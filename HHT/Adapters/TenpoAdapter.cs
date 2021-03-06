﻿using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using HHT.Resources.Model;

namespace HHT
{
    class TenpoAdapter : BaseAdapter
    {
        private List<TUMIKOMI020> tenpoList;

        public TenpoAdapter(List<TUMIKOMI020> tenpoList)
        {
            this.tenpoList = tenpoList;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return tenpoList[position].todokesaki_cd;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            TenpoAdapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as TenpoAdapterViewHolder;

            if (holder == null)
            {
                holder = new TenpoAdapterViewHolder();
                //var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                
                //LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);

                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.adapter_list_todoke, parent, false);
                //replace with your item and your holder items
                //comment back in
                //view = inflater.Inflate(Resource.Layout.item, parent, false);
                //holder.Title = view.FindViewById<TextView>(Resource.Id.text);
                view.Tag = holder;
            }


            //fill in your items
            //holder.Title.Text = "new text here";

            return view;
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return tenpoList.Count;
            }
        }

    }

    class TenpoAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}