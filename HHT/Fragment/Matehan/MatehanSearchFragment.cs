﻿using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using HHT.Resources.Model;

namespace HHT
{
    public class MatehanSearchFragment : BaseFragment
    {
        private View view;
        private VendorAllAdapter todokesakiAdapter;

        private int kosuMenuflag;

        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_matehan_search, container, false);

            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            kosuMenuflag = prefs.GetInt(Const.KOSU_MENU_FLAG, (int)Const.KOSU_MENU.TODOKE); // 画面区分

            SetTitle("届先指定検品");
            SetFooterText("");

            SetTodokesakiAsync();

            return view;
        }

        private void SetTodokesakiAsync()
        {
            var progress = ProgressDialog.Show(this.Activity, null, "届先検索中。。。", true);

            List<KOSU190> todokeList = GetTokuisakiMasterInfo();

            if(todokeList.Count > 0)
            {
                ListView listView = view.FindViewById<ListView>(Resource.Id.listView1);
                listView.ItemClick += listView_ItemClick;

                todokesakiAdapter = new VendorAllAdapter(todokeList);
                listView.Adapter = todokesakiAdapter;
            }
            else
            {
                CommonUtils.AlertDialog(view, "確認", "表示データがありません。", () =>
                {
                    if(kosuMenuflag == (int)Const.KOSU_MENU.TODOKE)
                    {
                        // sagyou4
                        FragmentManager.PopBackStack();
                    }
                    else if (kosuMenuflag == (int)Const.KOSU_MENU.VENDOR)
                    {
                        // sagyou9
                        // ベンダー検索画面へ
                    }
                });
            }

            new Thread(new ThreadStart(delegate
            {
                Activity.RunOnUiThread(() => progress.Dismiss());
            }
            )).Start();
        }

        private List<KOSU190> GetTokuisakiMasterInfo()
        {
            string nyuka_souko = prefs.GetString("souko_cd", "108");
            return WebService.RequestMate020(nyuka_souko);
        }

        void listView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = todokesakiAdapter[e.Position];
            
            editor.PutString("vendor_cd", item.vendor_cd);
            editor.PutString("vendor_nm", item.vendor_nm);
            editor.Apply();

            StartFragment(FragmentManager, typeof(MatehanWorkFragment));
        }
        
    }
}