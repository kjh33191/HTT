using Android.App;
using Android.Util;
using HHT.Resources.Model;
using SQLite;
using System.Collections.Generic;
using System.Linq;

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
                    connection.CreateTable<Model.Config>();

                    connection.CreateTable<Tanto>();
                    connection.CreateTable<Login>();

                    connection.CreateTable<MFile>();
                    connection.CreateTable<MbFile>();
                    connection.CreateTable<SoFile>();
                    //connection.CreateTable<PsFile>();
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

        public Model.Config GetHostIpAddress()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    List<Model.Config> config = connection.Table<Model.Config>().ToList();

                    if (config.Count > 0)
                    {
                        return config[0];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }

        public bool SetHostIpAddress(string hostIp, string port)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    List<Model.Config> configList = connection.Table<Model.Config>().ToList();
                    Model.Config config = new Model.Config();
                    config.hostIp = hostIp;
                    config.port = port;

                    if (configList.Count > 0)
                    {
                        connection.DeleteAll<Model.Config>();
                    }

                    connection.Insert(config);
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }

            return true;
        }
    }
}