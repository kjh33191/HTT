using Android.App;
using Android.Content;
using HHT.Resources.DataHelper;
using System.Threading.Tasks;

namespace HHT
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        DataBase db;

        protected override void OnResume()
        {
            base.OnResume();
            
            Task.Run(() =>
            {
                db = new DataBase();
                db.CreateDataBase();
                Resources.Model.Config config = db.GetHostIpAddress();
                
                if (config != null)
                {
                    string hostIp = config.hostIp;
                    string port = config.port;

                    WebService.SetHostIpAddress(hostIp, port);
                }

                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                
            });
        }
    }
}