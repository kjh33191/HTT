using System;

using Android.App;
using Android.OS;
using Android.Views;

namespace HHT
{
    public class CustomDialogFragment : DialogFragment
    {
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
            //((TextView)view.FindViewById(Resource.Id.dialog_confirm_msg)).setText(mMainMsg);
            //view.findViewById(R.id.dialog_confirm_btn).setOnClickListener(this);
            builder.SetView(view);
            return builder.Create();
        }
    }
}