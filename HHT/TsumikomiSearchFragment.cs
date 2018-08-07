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
using HHT.Resources.Model;

namespace HHT
{
    public class TsumikomiSearchFragment : BaseFragment
    {
        private View view;
        private TenpoAdapter tenpoAdapter;
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
            view = inflater.Inflate(Resource.Layout.fragment_kosu_search, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetTitle("積込検品");
            SetFooterText("");
            
            listView = view.FindViewById<ListView>(Resource.Id.listView1);
            listView.ItemClick += listView_ItemClick;
            
            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            GetTenpoList();
        }

        private void GetTenpoList()
        {
            //var progress = ProgressDialog.Show(this.Activity, "Please wait...", "Contacting server. Please wait...", true);
            ((MainActivity)this.Activity).ShowProgress("Contacting server. Please wait...");

            new Thread(new ThreadStart(delegate {

            Activity.RunOnUiThread(() =>
            {
                Thread.Sleep(3000);

                Dictionary<string, string> param = new Dictionary<string, string>
                {
                    { "kenpin_souko",  prefs.GetString("souko_cd", "103")},
                    { "kitaku_cd", prefs.GetString("kitaku_cd", "2") },
                    { "syuka_date", prefs.GetString("shuka_date", "180310") },
                    { "nohin_date", prefs.GetString("nohin_date", "1") },
                    { "bin_no", prefs.GetString("bin_no", "310") },
                    { "course", prefs.GetString("course", "310") },
                };
                
                //string resultJson = CommonUtils.Post(WebService.TUMIKOMI.TUMIKOMI010, param);
                //List<TUMIKOMI010> result = JsonConvert.DeserializeObject<List<TUMIKOMI010>>(resultJson);
                result = new List<TUMIKOMI020>();
                result.Add(new TUMIKOMI020());
                result.Add(new TUMIKOMI020());
                result.Add(new TUMIKOMI020());
                result.Add(new TUMIKOMI020());
                result.Add(new TUMIKOMI020());
                result.Add(new TUMIKOMI020());
                result.Add(new TUMIKOMI020());
                result.Add(new TUMIKOMI020());
                result.Add(new TUMIKOMI020());
                result.Add(new TUMIKOMI020());

                tenpoAdapter = new TenpoAdapter(result);
                //listView.Adapter = tenpoAdapter;
                
            }
            );
            Activity.RunOnUiThread(() => ((MainActivity)this.Activity).DismissDialog());

        }
        )).Start();

        }

        void listView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {

            var item = this.tenpoAdapter.GetItem(e.Position);

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            ISharedPreferencesEditor editor = prefs.Edit();

            //editor.PutString("todokesaki", item.ToString());

            editor.Apply();

            CheckTenpo(e.Position);
            
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
 