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
using HHT.Common;
using HHT.Resources.Model;

namespace HHT
{
    public class TsumikomiSearchFragment : BaseFragment
    {
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        private View view;
        private Tsumikomi020Adapter todokesakiAdapter;
        
        private ListView listView;
        private string souko_cd = "";
        private string kitaku_cd = "";
        private string syuka_date = "";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_tsumikomi_search, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetTitle("積込検品");
            SetFooterText("");

            souko_cd = prefs.GetString("souko_cd", "");
            kitaku_cd = prefs.GetString("kitaku_cd", "");
            syuka_date = prefs.GetString("syuka_date", "");

            listView = view.FindViewById<ListView>(Resource.Id.listView1);
            
            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            GetTenpoList();
        }

        private void GetTenpoList()
        {
            var progress = ProgressDialog.Show(this.Activity, null, "届先検索中。。。", true);

            List<TUMIKOMI020> todokeList = GetTokuisakiMasterInfo();

            new Thread(new ThreadStart(delegate
            {
                Activity.RunOnUiThread(() => {
                    if (todokeList.Count > 0)
                    {
                        ListView listView = view.FindViewById<ListView>(Resource.Id.listView1);
                        listView.ItemClick += listView_ItemClick;

                        todokesakiAdapter = new Tsumikomi020Adapter(todokeList);
                        listView.Adapter = todokesakiAdapter;
                    }
                    else
                    {
                        CommonUtils.AlertDialog(view, "", "表示データがありません。", ()=> { FragmentManager.PopBackStack(); });
                    }

                });
                Activity.RunOnUiThread(() => progress.Dismiss());
            }
            )).Start();
        }

        private List<TUMIKOMI020> GetTokuisakiMasterInfo()
        {
            List<TUMIKOMI020> resultList = new List<TUMIKOMI020>(); ;
            string soukoCd = prefs.GetString("souko_cd", "");
            string kitakuCd = prefs.GetString("kitaku_cd", "");
            string syuka_date = prefs.GetString("syuka_date", "");
            string bin_no = prefs.GetString("bin_no", "");
            string course = prefs.GetString("course", "");

            resultList = WebService.RequestTumikomi020(soukoCd, kitakuCd, syuka_date,  bin_no, course);
            
            return resultList;
        }

        void listView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                var item = todokesakiAdapter[e.Position];
                int count = WebService.RequestTumikomi030(souko_cd, kitaku_cd, syuka_date, item.tokuisaki_cd, item.todokesaki_cd, item.bin_no);

                if (count == 0)
                {
                    CommonUtils.AlertDialog(view, "Error", "定番・増便データはありません。", null);
                    return;
                }
                else if (count == 1)
                {
                    editor.PutInt("zoubin_flg", count);
                    editor.PutString("todokesaki_cd", item.todokesaki_cd);
                    editor.PutString("tokuisaki_cd", item.tokuisaki_cd);
                    editor.PutString("tokuisaki_nm", item.tokuisaki_rk);
                    editor.PutString("tsumi_vendor_cd", "");
                    editor.PutString("tsumi_vendor_nm", "");
                    editor.PutString("vendor_cd", "");
                    editor.PutString("vendor_nm", "");
                    editor.PutString("start_vendor_cd", "");

                    editor.Apply();

                    StartFragment(FragmentManager, typeof(TsumikomiWorkFragment));
                }
            }
            catch
            {
                CommonUtils.AlertDialog(view, "Error", "届先指定に失敗しました。", null);
                return;
            }
        }
    }
}
 