using System.Collections.Generic;
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
            ((MainActivity)this.Activity).ShowProgress("読み込み中...");

            List <TUMIKOMI020> todokeList = GetTokuisakiMasterInfo();

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
                        ShowDialog("エラー", "表示データがありません", () => { FragmentManager.PopBackStack(); });
                    }

                });
                Activity.RunOnUiThread(() => ((MainActivity)this.Activity).DismissDialog());
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
                    ShowDialog("エラー", "定番・増便データはありません。", () => { });
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
                ShowDialog("エラー", "届先指定に失敗しました。", () => { });
                return;
            }
        }
    }
}
 