using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using HHT.Resources.DataHelper;
using HHT.Resources.Model;

namespace HHT
{
    public class KosuVendorAllSearchFragment : BaseFragment
    {
        private View view;
        private VendorAllAdapter vendorAdapter;
        private int kosuMenuflag;

        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;
        private List<KOSU190> vendorList;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_search_vendor_all, container, false);

            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            kosuMenuflag = prefs.GetInt(Const.KOSU_MENU_FLAG, (int)Const.KOSU_MENU.TODOKE); // 画面区分

            SetTodokesakiAsync();

            return view;
        }

        private void SetTodokesakiAsync()
        {
            string soukoCd = prefs.GetString("souko_cd", "");
            string kitakuCd = prefs.GetString("kitaku_cd", "");
            string syuka_date = prefs.GetString("syuka_date", "");

            if (prefs.GetBoolean("kounaiFlag", true))
            {
                vendorList = WebService.RequestKosu190(); ;
            }
            else
            {
                vendorList = new List<KOSU190>();
                List<MateFile> mateList = new MateFileHelper().SelectAll();

                string tempVendorCd = "";

                foreach (MateFile mfile in mateList)
                {
                   // ベンダーに紐づくマテハン情報も複数持つので、ベンダーコードで分ける
                    if (tempVendorCd != mfile.vendor_cd)
                    {
                        tempVendorCd = mfile.vendor_cd;

                        KOSU190 kosu190 = new KOSU190();
                        kosu190.vendor_cd = mfile.vendor_cd;
                        kosu190.vendor_nm = mfile.vendor_nm;
                        
                        vendorList.Add(kosu190);
                    }
                }
            }

            if (vendorList.Count > 0)
            {
                ListView listView = view.FindViewById<ListView>(Resource.Id.listView1);
                listView.ItemClick += listView_ItemClick;

                vendorAdapter = new VendorAllAdapter(vendorList);
                listView.Adapter = vendorAdapter;
            }
            else
            {
                ShowDialog("報告", "表示データがありません。", () =>
                {
                    FragmentManager.PopBackStack();
                });
            }
        }
        
        void listView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = vendorList[e.Position];

            editor.PutBoolean("searchFlag", true);
            editor.PutString("vendor_cd", item.vendor_cd);
            editor.PutString("vendor_nm", item.vendor_nm);
            editor.Apply();
            FragmentManager.PopBackStack();

        }
    }
}