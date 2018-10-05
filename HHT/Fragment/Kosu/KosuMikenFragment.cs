using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using HHT.Resources.Model;

namespace HHT
{
    public class KosuMikenFragment : BaseFragment
    {
        private View view;

        private int kosuMenuflag;

        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;
        
        List<KOSU120> mikenList;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_kosu_miken, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            kosuMenuflag = prefs.GetInt(Const.KOSU_MENU_FLAG, (int)Const.KOSU_MENU.TODOKE); // 画面区分

            SetTitle("入荷検品");
            SetFooterText("");

            ListView listView = view.FindViewById<ListView>(Resource.Id.mikenListView);
            
            string souko_cd = prefs.GetString("souko_cd", "");
            string kitaku_cd = prefs.GetString("kitaku_cd", "");
            string syuka_date = prefs.GetString("syuka_date", "");
            string tokuisaki_cd = prefs.GetString("tokuisaki_cd", "");
            string todokesaki_cd = prefs.GetString("todokesaki_cd", "");
            
            ((MainActivity)this.Activity).ShowProgress("未検一覧を取得中");

            mikenList = WebService.RequestKosu120(souko_cd, kitaku_cd, syuka_date, tokuisaki_cd, todokesaki_cd);
            
            Activity.RunOnUiThread(() =>
            {
                MikenAdapter mikenAdapter = new MikenAdapter(mikenList);
                listView.Adapter = mikenAdapter;

                ((MainActivity)this.Activity).DismissDialog();
            });

            return view;
        }
    }
}