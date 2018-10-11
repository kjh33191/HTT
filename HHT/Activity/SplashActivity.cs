using Android.App;
using Android.Content;
using Android.Preferences;
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

            //prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            //editor = prefs.Edit();
            //editor.Clear();
            //editor.Commit();

            Task.Run(() =>
            {
                db = new DataBase();
                db.CreateDataBase();
                string hostIp = db.GetHostIpAddress();

                if(hostIp != null && hostIp != "")
                {
                    WebService.SetHostIpAddress(hostIp);
                }
                
                //db.ClearAll();

                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                
            });
        }

    }
}