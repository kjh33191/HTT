using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;

namespace HHT
{
    public class BaseFragment : Fragment
    {
        TextView toolbarTitle;
        TextView txtFooterBody;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            toolbarTitle = this.Activity.FindViewById<TextView>(Resource.Id.toolbar_title);
            txtFooterBody = this.Activity.FindViewById<TextView>(Resource.Id.tv_foot_body);

        }

        protected void StartFragment(FragmentManager fm, Type fragmentClass)
        {
            BaseFragment fragment = null;
            try
            {
                fragment = (BaseFragment)Activator.CreateInstance(fragmentClass);
            } catch (Exception e)
            {

            }

            this.Activity.FragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.fragmentContainer, fragment)
                //.Add(Resource.Id.fragmentContainer, fragment)
                .AddToBackStack(null)
                .Commit();
        }

        protected void StartFragment(FragmentManager fm, Type fragmentClass,Bundle bundle)
        {
            BaseFragment fragment = null;
            try
            {
                fragment = (BaseFragment)Activator.CreateInstance(fragmentClass);
                fragment.Arguments = bundle;
            }
            catch (Exception e)
            {

            }

            this.Activity.FragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.fragmentContainer, fragment)
                //.Add(Resource.Id.fragmentContainer, fragment)
                .AddToBackStack(null)
                .Commit();
        }

        public virtual bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            return true;
        }

        public virtual void OnBarcodeDataReceived(BarcodeDataReceivedEvent_ dataReceivedEvent)
        {
            return;
        }

        public virtual bool OnBackPressed()
        {
            return true;
        }

        public void SetTitle(string title)
        {
            toolbarTitle.Text = title;
        }

        public void SetFooterText(string content)
        {
            txtFooterBody.Text = content;
        }
    }
}