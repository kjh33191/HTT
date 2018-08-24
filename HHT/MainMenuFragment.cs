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
using Java.Interop;

namespace HHT
{
    public class MainMenuFragment : BaseFragment
    {
        private View view;
        private string mUserKbn = "0";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_menu_main, container, false);
            LinearLayout layout = view.FindViewById<LinearLayout>(Resource.Id.linearLayout2);
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            
            // 管理者の場合
            
            Button btnNyuka = view.FindViewById<Button>(Resource.Id.btn_main_manager_nyuka);
            btnNyuka.Click += delegate { StartFragment(FragmentManager, typeof(NyukaMenuFragment)); };

            Button btnTsumikae = view.FindViewById<Button>(Resource.Id.btn_main_manager_tsumikae);
            btnTsumikae.Click += delegate { StartFragment(FragmentManager, typeof(TsumikaeMenuFragment)); };

            Button btnTsumikomi = view.FindViewById<Button>(Resource.Id.btn_main_manager_tsumikomi);
            btnTsumikomi.Click += delegate { StartFragment(FragmentManager, typeof(TsumikomiSelectFragment)); };

            Button btnNohin = view.FindViewById<Button>(Resource.Id.btn_main_manager_nohin);
            btnNohin.Click += delegate { StartFragment(FragmentManager, typeof(NohinSelectFragment)); };

            Button btnDataSend = view.FindViewById<Button>(Resource.Id.btn_main_manager_dataSend);
            btnDataSend.Click += delegate { CommonUtils.AlertDialog(view, "Error", "送信するデータが存在しません。", null); };


            Button btnMailBag = view.FindViewById<Button>(Resource.Id.btn_main_manager_mailBag);
            Button btnIdouRegist = view.FindViewById<Button>(Resource.Id.btn_main_manager_idousakiRegist);
            Button btnIdouNohin = view.FindViewById<Button>(Resource.Id.btn_main_manager_idousakiNohin);
            Button btnTc2 = view.FindViewById<Button>(Resource.Id.btn_main_manager_tc2);
            Button btnZaikou = view.FindViewById<Button>(Resource.Id.btn_main_manager_zaikou);

            SetTitle("業務メニュー");
            SetFooterText("");
            
            return view;
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            //管理者基準
            if (keycode == Keycode.Num1)
            {
                StartFragment(FragmentManager, typeof(NyukaMenuFragment));
            }
            else if (keycode == Keycode.Num2)
            {
                StartFragment(FragmentManager, typeof(TsumikaeMenuFragment));
            }
            else if (keycode == Keycode.Num3)
            {
                StartFragment(FragmentManager, typeof(TsumikomiSelectFragment));
            }
            else if (keycode == Keycode.Num4)
            {
                StartFragment(FragmentManager, typeof(NohinSelectFragment));
            }
            else if (keycode == Keycode.Num5)
            {
                // データ送信
                //DataSend();
                CommonUtils.AlertDialog(view, "Error", "送信するデータが存在しません。", null);
            }
            else if (keycode == Keycode.Num6)
            {
                // マテハン登録
                StartFragment(FragmentManager, typeof(MatehanMenuFragment));
            }
            else if (keycode == Keycode.Num7)
            {
                // メールバッグ
            }
            else if (keycode == Keycode.Num8)
            {
                // 移動先店舗登録
            }
            else if (keycode == Keycode.Num9)
            {
                // 移動先店舗納品
            }
            else if (keycode == Keycode.Num0)
            {
                // TC2
            }
            else if (keycode == Keycode.Period)
            {
                // 在庫集約
            }
            return true;
        }

        private void DataSend()
        {
            // 送信データが存在しない場合、
            //CommonUtils.AlertDialog(view, "Error", "送信するデータが存在しません。", null);

            CommonUtils.AlertConfirm(view, "Error", "送信するデータが存在しません。", (flag)=> {
                if (flag)
                {
                    // データ送信する。（サーバーを呼出す。）　　
                    //　　②－１．ファイルの拡張子が「.csv」の場合　：　FTP010
　　                //    ②－２．ファイルの拡張子が「.txt」の場合：　SEND010

                    // 파일이 아니라 로컬DB이므로, 어떻게 보내야할까..


                }
                else
                {

                }
            });
        }
    }
}