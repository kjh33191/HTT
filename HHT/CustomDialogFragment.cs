using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Beardedhen.Androidbootstrap;
using Com.Beardedhen.Androidbootstrap.Api.Defaults;

namespace HHT
{
    public class CustomDialogFragment : DialogFragment
    {
        public event DialogEventHandler Dismissed;

        private static readonly string ARG_DIALOG_MAIN_MSG = "dialog_main_msg";
        private String mMainMsg;

        public static CustomDialogFragment newInstance(String mainMsg)
        {
            Bundle bundle = new Bundle();
            bundle.PutString(ARG_DIALOG_MAIN_MSG, mainMsg);
            CustomDialogFragment fragment = new CustomDialogFragment
            {
                Arguments = bundle
            };

            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (Arguments != null) mMainMsg = Arguments.GetString("ARG_DIALOG_MAIN_MSG");
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
            View view = Activity.LayoutInflater.Inflate(Resource.Layout.dialog_custom2, null);

            string title = Arguments.GetString("title");
            string body = Arguments.GetString("body");

            ((TextView)view.FindViewById(Resource.Id.title)).Text = title;
            ((TextView)view.FindViewById(Resource.Id.message)).Text = body;

            BootstrapButton button = view.FindViewById<BootstrapButton>(Resource.Id.okButton);
            button.Click += delegate {
                if (null != Dismissed)
                    Dismissed(this, new DialogEventArgs {
                        Text = "Test"
                    });
                Dismiss();
            };

            BootstrapButton cancelButton = view.FindViewById<BootstrapButton>(Resource.Id.cancelButton);
            cancelButton.BootstrapBrand = DefaultBootstrapBrand.Regular;
            cancelButton.Click += delegate {
                Dismiss();
            };

            if (title == "エラー")
            {
                ((TextView)view.FindViewById(Resource.Id.title)).SetBackgroundColor(Color.Red);
                button.BootstrapBrand = DefaultBootstrapBrand.Danger;
                cancelButton.Visibility = ViewStates.Gone;
            }

            builder.SetView(view);
            return builder.Create();
        }
        
        public class DialogEventArgs : EventArgs
        {
            public string Text { get; set; }
        }

        public delegate void DialogEventHandler(object sender, DialogEventArgs args);

    }
}