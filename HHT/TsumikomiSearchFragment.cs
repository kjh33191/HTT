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
        private View view;
        private TenpoAdapter tenpoAdapter;
        private Tsumikomi020Adapter todokesakiAdapter;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;
        
        ListView listView;
        List<TUMIKOMI020> result = new List<TUMIKOMI020>();

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
            
            listView = view.FindViewById<ListView>(Resource.Id.listView1);
            //listView.ItemClick += listView_ItemClick;
            
            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            //GetTenpoList();
            SetTodokesakiAsync();
        }

        private void SetTodokesakiAsync()
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
            string soukoCd = prefs.GetString("souko_cd", "108");
            string kitakuCd = prefs.GetString("kitaku_cd", "2");
            string syuka_date = prefs.GetString("syuka_date", "20180320");
            string nohin_date = prefs.GetString("nohin_date", "20180320");
            string bin_no = prefs.GetString("bin_no", "1");
            string course = prefs.GetString("course", "101");

            resultList = WebService.RequestTumikomi020(soukoCd, kitakuCd, syuka_date, nohin_date,  bin_no, course);
            
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

            //int count = WebService.RequestTumikomi030();
            int count = 1;

            if(count == 0)
            {
                CommonUtils.AlertDialog(view, "Error", "定番・増便データはありません。", ()=> { return; });
            }else if(count == 1)
            {
                //Return("sagyou5") //積込作業の場合(積込検品)定番コース
                StartFragment(FragmentManager, typeof(TsumikomiWorkFragment));
            }
            else
            {
                //Return("sagyou7") //積込作業(増便コース)入力の場合(積込検品) 増便コース
                StartFragment(FragmentManager, typeof(TsumikomiWorkFragment));
            }
        }

        private void CheckTenpo(int pos)
        {
            var progress = ProgressDialog.Show(this.Activity, "", "Contacting server. Please wait...", true);
            
            TUMIKOMI020 selectedTenpo = result[pos];
            
            new Thread(new ThreadStart(delegate {

                Activity.RunOnUiThread(() =>
                {
                    Thread.Sleep(1500);

                    Dictionary<string, string> param = new Dictionary<string, string>
                    {
                        { "kenpin_souko",  prefs.GetString("souko_cd", "103")},
                        { "kitaku_cd", prefs.GetString("kitaku_cd", "2") },
                        { "syuka_date", prefs.GetString("shuka_date", "180310") },
                        { "nohin_date", prefs.GetString("nohin_date", "1") },
                        { "bin_no", prefs.GetString("bin_no", "310") },
                        { "course", prefs.GetString("course", "310") },
                    };


                    //string resultJson = CommonUtils.Post(WebService.TUMIKOMI.TUMIKOMI030, param);
                    //int result = JsonConvert.DeserializeObject<List<TUMIKOMI010>>(resultJson);
                    int zoubin_flg = 1;
                    if (zoubin_flg == 0)
                    {
                        CommonUtils.AlertDialog(view, "エラー", "定番・増便データはありません。", null);
                    }
                    else if (zoubin_flg == 1)
                    {
                        //JOB: scan_flg = false	//スキャン済みフラグ
                        // Return("sagyou5")積込作業
                        StartFragment(FragmentManager, typeof(TsumikomiWorkFragment));
                    }
                    else if (zoubin_flg >= 2)
                    {
                        // Return("sagyou7") 積込作業画面だけど。。。
                        StartFragment(FragmentManager, typeof(TsumikomiWorkFragment));
                    }

                }
                );
                Activity.RunOnUiThread(() => progress.Dismiss());

            }
            )).Start();

        }

        void confirm()
        {
            CommonUtils.ShowAlertDialog(view, "Alert", "Alert");
        }



    }
}
 