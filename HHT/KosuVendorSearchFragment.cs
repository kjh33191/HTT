
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
    public class KosuVendorSearchFragment : BaseFragment
    {
        private View view;
        private VendorAdapter vendorAdapter;
        private int kosuMenuflag;

        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;
        private List<KOSU095> vendorList;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_kosu_vendor_search, container, false);

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
            string soukoCd = prefs.GetString("souko_cd", "");
            string kitakuCd = prefs.GetString("kitaku_cd", "");
            string syuka_date = prefs.GetString("syuka_date", "");
            
            vendorList = WebService.RequestKosu095(soukoCd, kitakuCd, syuka_date);

            if (vendorList.Count > 0)
            {
                ListView listView = view.FindViewById<ListView>(Resource.Id.listView1);
                listView.ItemClick += listView_ItemClick;

                vendorAdapter = new VendorAdapter(vendorList);
                listView.Adapter = vendorAdapter;
            }
            else
            {
                CommonUtils.AlertDialog(view, "確認", "表示データがありません。", () =>
                {
                    FragmentManager.PopBackStack();
                });
            }
        }

        private List<Todokesaki> GetTokuisakiMasterInfo()
        {
            List<Todokesaki> resultList = new List<Todokesaki>(); ;
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
            var item = vendorList[e.Position];

            var message =
                @"配送日：" + prefs.GetString("syuka_date", "20180320")
            + "ベンダー：" + item.vendor_cd
            + item.vendor_nm
            + "よろしいですか？";


            CommonUtils.AlertConfirm(view, "", "message", (okFlag) =>
            {
                if (okFlag)
                {
                    editor.PutString("vendor_cd", item.vendor_cd);
                    editor.PutString("vendor_nm", item.vendor_nm);
                    editor.Apply();
                    StartFragment(FragmentManager, typeof(TodokeTyingWorkFragment));
                }
            });
            /*
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

            StartFragment(FragmentManager, typeof(TodokeTyingWorkFragment));
            */
        }
        
    }
}