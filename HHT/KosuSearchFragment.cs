using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using HHT.Resources.DataHelper;
using HHT.Resources.Model;
using Newtonsoft.Json;

namespace HHT
{
    public class KosuSearchFragment : BaseFragment
    {
        private View view;
        private TodokesakiAdapter todokesakiAdapter;

        private int kosuMenuflag;

        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        TokuisakiHelper tokuisakiHelper;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_kosu_search, container, false);

            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            kosuMenuflag = prefs.GetInt(Const.KOSU_MENU_FLAG, (int)Const.KOSU_MENU.TODOKE); // 画面区分

            SetTitle("届先指定検品");
            SetFooterText("");

            SetTodokesakiAsync();

            return view;
        }

        private async void SetTodokesakiAsync()
        {
            var progress = ProgressDialog.Show(this.Activity, null, "届先検索中。。。", true);

            List<Todokesaki> todokeList = await GetTokuisakiMasterInfo();

            if(todokeList.Count > 0)
            {
                ListView listView = view.FindViewById<ListView>(Resource.Id.listView1);
                listView.ItemClick += listView_ItemClick;

                todokesakiAdapter = new TodokesakiAdapter(todokeList);
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

        private async Task<List<Todokesaki>> GetTokuisakiMasterInfo()
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "kenpin_souko",  prefs.GetString("souko_cd", "108")},
                { "kitaku_cd",  prefs.GetString("kitaku_cd", "2")},
                { "syuka_date",  prefs.GetString("syuka_date", "20180320")},
                { "bin_no",  prefs.GetString("bin_no", "1")}
            };

            string resultJson = "";
            if (kosuMenuflag == (int)Const.KOSU_MENU.TODOKE)
            {
                resultJson = await CommonUtils.PostAsync(WebService.KOSU.KOSU060, param);
            }
            else if (kosuMenuflag == (int)Const.KOSU_MENU.VENDOR)
            {
                resultJson = await CommonUtils.PostAsync(WebService.KOSU.KOSU065, param);
            }

            List<Todokesaki> resultList = JsonConvert.DeserializeObject<List<Todokesaki>>(resultJson);

            return resultList;
        }

        void listView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = todokesakiAdapter[e.Position];
            
            editor.PutString("todokesaki_cd", item.todokesaki_cd);
            editor.PutString("tokuisaki_cd", item.tokuisaki_cd);
            editor.PutString("tokuisaki_nm", item.tokuisaki_rk);
            editor.PutString("tsumi_vendor_cd", "");
            editor.PutString("tsumi_vendor_nm", "");
            editor.PutString("vendor_cd", "");
            editor.PutString("vendor_nm", "");
            editor.PutString("start_vendor_cd", "");

            editor.Apply();

            StartFragment(FragmentManager, typeof(KosuConfirmFragment));
            
        }
        
    }
}