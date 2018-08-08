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
                    return true;
                }
            }
            catch(SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }
    }
}