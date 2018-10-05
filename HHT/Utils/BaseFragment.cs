using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;

namespace HHT
{
    public class BaseFragment : Fragment
    {

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        protected void StartFragment(FragmentManager fm, Type fragmentClass)
        {
            BaseFragment fragment = null;
            try
            {
                fragment = (BaseFragment)Activator.CreateInstance(fragmentClass);
            }
            catch (Exception e)
            {

            }

            this.Activity.FragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.fragmentContainer, fragment)
                //.Add(Resource.Id.fragmentContainer, fragment)
                .SetTransition(FragmentTransit.FragmentFade)
                .AddToBackStack(null)
                .Commit();
        }

        protected void StartFragment(FragmentManager fm, Type fragmentClass, Bundle bundle)
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
            ((MainActivity)this.Activity).SupportActionBar.Title = title;
        }

        public void SetFooterText(string content)
        {
            this.Activity.FindViewById<TextView>(Resource.Id.tv_foot_body).Text = content;
        }

        public void ShowFooter()
        {
            this.Activity.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.footerLayout).Visibility = ViewStates.Visible;
        }

        public void HideFooter()
        {
            this.Activity.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.footerLayout).Visibility = ViewStates.Gone;
        }

        public void Vibrate()
        {
            ((Vibrator)Activity.GetSystemService(Android.Content.Context.VibratorService)).Vibrate(1000);
        }
    }
}