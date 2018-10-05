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
    public class KosuSearchFragment : BaseFragment
    {
        private View view;
        private TodokesakiAdapter todokesakiAdapter;
        
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        ListView listView;
        private int kosuMenuflag;

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
            SetFooterText("F1：未検");

            SetTodokesakiAsync();

            return view;
        }

        private void SetTodokesakiAsync()
        {
            var progress = ProgressDialog.Show(this.Activity, null, "届先検索中。。。", true);

            List<KOSU060> todokeList = GetTokuisakiMasterInfo();

            if(todokeList.Count > 0)
            {
                listView = view.FindViewById<ListView>(Resource.Id.listView1);
                listView.ItemClick += listView_ItemClick;

                todokesakiAdapter = new TodokesakiAdapter(todokeList);
                listView.Adapter = todokesakiAdapter;
            }
            else
            {
                CommonUtils.AlertDialog(view, "確認", "表示データがありません。", () =>
                {
                  FragmentManager.PopBackStack();  
                });
                Vibrate();
            }

            new Thread(new ThreadStart(delegate
            {
                Activity.RunOnUiThread(() => progress.Dismiss());
            }
            )).Start();
        }

        private List<KOSU060> GetTokuisakiMasterInfo()
        {
            List<KOSU060> resultList = new List<KOSU060>(); ;
            string soukoCd = prefs.GetString("souko_cd", "108");
            string kitakuCd = prefs.GetString("kitaku_cd", "2");
            string syuka_date = prefs.GetString("syuka_date", "20180320");
            string bin_no = prefs.GetString("bin_no", "1");
            
            if (kosuMenuflag == (int)Const.KOSU_MENU.TODOKE)
            {
                resultList = WebService.RequestKosu060(soukoCd, kitakuCd, syuka_date, bin_no);
            }
            else if (kosuMenuflag == (int)Const.KOSU_MENU.VENDOR)
            {
                resultList = WebService.RequestKosu065(soukoCd, kitakuCd, syuka_date, bin_no);
            }
            
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

            StartFragment(FragmentManager, typeof(KosuWorkFragment));
            
        }


        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F1)
            {
                // 未検画面へ遷移(sagyou19)
                var item = todokesakiAdapter[listView.SelectedItemPosition];
                editor.PutString("tokuisaki_cd", item.tokuisaki_cd);
                editor.PutString("todokesaki_cd", item.todokesaki_cd);
                editor.Apply();

                StartFragment(FragmentManager, typeof(KosuMikenFragment));
            }

            return true;
        }

    }
}