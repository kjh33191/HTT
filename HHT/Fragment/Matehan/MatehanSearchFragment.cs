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
    public class MatehanSearchFragment : BaseFragment
    {
        private View view;
        private VendorAllAdapter todokesakiAdapter;
        private int kosuMenuflag;
        string souko_cd;

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
            souko_cd = prefs.GetString("souko_cd", "");

            SetTitle("貸出先検索");
            SetTodokesakiAsync();

            return view;
        }

        private void SetTodokesakiAsync()
        {
            ((MainActivity)this.Activity).ShowProgress("読み込み中");

            List<KOSU190> todokeList = WebService.RequestMate020(souko_cd);

            if (todokeList.Count > 0)
            {
                ListView listView = view.FindViewById<ListView>(Resource.Id.listView1);
                listView.ItemClick += listView_ItemClick;

                todokesakiAdapter = new VendorAllAdapter(todokeList);
                listView.Adapter = todokesakiAdapter;
            }
            else
            {
                ShowDialog("報告", "表示データがありません。", () => {
                    FragmentManager.PopBackStack();
                });
            }

            new Thread(new ThreadStart(delegate
            {
                Activity.RunOnUiThread(() => ((MainActivity)this.Activity).DismissDialog());
            }
            )).Start();
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