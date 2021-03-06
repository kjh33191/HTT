﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Telephony;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;
using Java.Net;

namespace HHT
{
    public class CommonUtils
    {

        private static readonly HttpClient client = new HttpClient();

        // サーバからデータ取得（同期）
        public static string GetJsonData(string url)
        {
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new System.Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response:
            using (WebResponse response = request.GetResponse())
            {
                // Get a stream representation of the HTTP web response:
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        Console.Out.WriteLine("Response contained empty body...");
                    }
                    else
                    {
                        return content;
                    }
                }
            }

            return null;
        }

        // サーバからデータ取得（非同期）
        public static async Task<string> GetJsonDataAsync(string url)
        {
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new System.Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response:
            using (WebResponse response = await request.GetResponseAsync())
            {
                // Get a stream representation of the HTTP web response:
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        Console.Out.WriteLine("Response contained empty body...");
                    }
                    else
                    {
                        return content;
                    }
                }
            }

            return null;
        }

        // サーバからデータ取得（非同期）
        public static string Post(string url, Dictionary<string, string> values)
        {
            var content = new FormUrlEncodedContent(values);
            
            var response = client.PostAsync(url, content).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            
            return responseString;
        }

        // サーバからデータ取得（非同期）
        public static async Task<string> PostAsync(string url, Dictionary<string, string> values)
        {
            var content = new FormUrlEncodedContent(values);
            
            var response = await client.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();
            
            return responseString;
        }

        // ヘッダータイトル設定
        public static void SetTitle(Activity activity, string title)
        {
            //TextView toolbarTitle = activity.FindViewById<TextView>(Resource.Id.toolbar_title);
            //toolbarTitle.Text = title;
        }

        // ソフトキーボードを隠す
        public static void HideKeyboard(Activity activity)
        {
            InputMethodManager inputManager = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);

            var currentFocus = activity.CurrentFocus;
            if (currentFocus != null)
            {
                inputManager.HideSoftInputFromWindow(currentFocus.WindowToken, HideSoftInputFlags.None);
            }
        }

        public static bool IsOnline(Activity activity)
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)activity.GetSystemService(Context.ConnectivityService);
            NetworkInfo networkInfo = connectivityManager.ActiveNetworkInfo;

            return networkInfo != null && networkInfo.IsConnectedOrConnecting;
        }

        public static async Task<bool> IsHostReachable(string host)
        {
            if (string.IsNullOrEmpty(WebService.GetHostIpAddress()))
                return false;

            bool isReachable = false;

            Ping x = new Ping();
            PingReply reply = x.Send(IPAddress.Parse(host)); //enter ip of the machine
            if (reply.Status == IPStatus.Success) // here we check for the reply status if it is success it means the host is reachable
            {
                isReachable = true;
            }
            
            return await Task.FromResult(isReachable);
        }


        /*
        public static void AlertDialog(View view, string title, string message, Action callback)
        {
            AlertDialog alertDialog = new AlertDialog.Builder(view.Context).Create();
            alertDialog.SetTitle(title);
            alertDialog.SetMessage(message);
            alertDialog.SetButton("確認", (senderAlert, args) => { callback?.Invoke(); });
            alertDialog.SetCancelable(false);
            alertDialog.Show();

            //alertDialog.SetOnShowListener(new IDialogInterfaceOnShowListener() {  })
        }

        public static void ShowAlertDialog(View view, string title, string message)
        {
            AlertDialog alertDialog = new AlertDialog.Builder(view.Context).Create();
            alertDialog.SetTitle(title);
            alertDialog.SetMessage(message);
            alertDialog.SetButton("OK", (senderAlert, args) => { });
            alertDialog.Show();
        }

        public static bool AlertConfirm(View view, string title, string content, Action<bool> callback)
        {
            var alert = new AlertDialog.Builder(view.Context).Create();
            alert.SetTitle(title);
            alert.SetMessage(content);
            alert.SetButton2("はい", (sender, e) => { callback(true); });
            alert.SetButton("いいえ", (sender, e) => { callback(false); });
            alert.SetCancelable(false);
            alert.Show();

            return true;
        }
        */

        public static string GetDateYYMMDDwithSlash(string dateString)
        {
            string ymd = Regex.Replace(Convert.ToString(dateString), @"[^\u0000-\u007F]|/", string.Empty);
            DateTime dt = DateTime.ParseExact(ymd.ToString(), "yyMMdd", CultureInfo.InvariantCulture);
            return dt.ToString("yy/MM/dd", CultureInfo.InvariantCulture);
        }

        public static string GetDateYYYYMMDDwithSlash(string dateString)
        {
            string ymd = Regex.Replace(Convert.ToString(dateString), @"[^\u0000-\u007F]|/", string.Empty);
            DateTime dt = DateTime.ParseExact(ymd.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
            return dt.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
        }

    }
}