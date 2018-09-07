using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Util;
using HHT.Resources.DataHelper;
using System.Threading.Tasks;

namespace HHT
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        DataBase db;
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        protected override void OnResume()
        {
            base.OnResume();

            prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            editor = prefs.Edit();
            editor.Clear();
            editor.Commit();

            Task.Run(() =>
            {
                db = new DataBase();
                db.CreateDataBase();

                //db.ClearAll();
                
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                
            });
        }

    }
}