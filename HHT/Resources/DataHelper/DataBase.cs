using Android.App;
using Android.Util;
using HHT.Resources.Model;
using SQLite;

namespace HHT.Resources.DataHelper
{
    public class DataBase
    {
        // Logcat tag
        private static readonly string LOG = "DatabaseHelper";
 
        // Database Version
        private static readonly int DATABASE_VERSION = 1;
        
        readonly string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        readonly string dbFileName = Application.Context.Resources.GetString(Resource.String.LocalDatabaseFileName);
        
        public bool CreateDataBase()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    connection.CreateTable<Tanto>();
                    connection.CreateTable<Login>();
                    connection.CreateTable<Tokuisaki>();

                    connection.CreateTable<MFile>();
                    connection.CreateTable<MbFile>();
                    connection.CreateTable<SoFile>();
                    connection.CreateTable<FtpFile>();
                    connection.CreateTable<PsFile>();
                    connection.CreateTable<MateFile>();
                    connection.CreateTable<TokuiFile>();

                    //　納品時、店舗到着情報
                    connection.CreateTable<TenpoArrive>();

                    //　納品用
                    connection.CreateTable<SndNohinWork>();
                    connection.CreateTable<SndNohinMate>();
                    connection.CreateTable<SndNohinMail>();
                    connection.CreateTable<SndNohinSyohinKaisyu>();
                    connection.CreateTable<SndNohinMailKaisyu>();
                    
                    return true;
                }
            }
            catch(SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool ClearAll()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    connection.DeleteAll<Tanto>();
                    connection.DeleteAll<Login>();
                    connection.DeleteAll<Tokuisaki>();
                    
                    connection.DeleteAll<MFile>();
                    connection.DeleteAll<MbFile>();
                    connection.DeleteAll<SoFile>();
                    connection.DeleteAll<FtpFile>();
                    connection.DeleteAll<PsFile>();
                    connection.DeleteAll<MateFile>();
                    connection.DeleteAll<TokuiFile>();
                    
                    //　納品用
                    connection.DeleteAll<SndNohinWork>();
                    connection.DeleteAll<SndNohinMate>();
                    connection.DeleteAll<SndNohinMail>();
                    connection.DeleteAll<SndNohinSyohinKaisyu>();
                    connection.DeleteAll<SndNohinMailKaisyuHelper>();
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }
        
    }
}