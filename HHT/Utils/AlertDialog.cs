using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace HHT.Common
{
    class CustomDialog
    {
        public CustomDialog(View view, string title, string body , Action callback)
        {
            Android.App.AlertDialog alertDialog = new Android.App.AlertDialog.Builder(view.Context).Create();
            alertDialog.SetTitle(title);
            alertDialog.SetMessage(body);
            alertDialog.SetButton("確認", (senderAlert, args) => { callback?.Invoke(); });
            alertDialog.Show();
        }

        public CustomDialog(View view, string title, string body, Action<bool> callback)
        {
            Android.App.AlertDialog alertDialog = new Android.App.AlertDialog.Builder(view.Context).Create();
            alertDialog.SetTitle(title);
            alertDialog.SetMessage(body);
            alertDialog.SetButton2("はい", (sender, e) => { callback(true); });
            alertDialog.SetButton("いいえ", (sender, e) => { callback(false); });
            alertDialog.Show();
        }
    }
}