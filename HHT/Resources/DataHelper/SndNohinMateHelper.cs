using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Util;
using HHT.Resources.Model;
using SQLite;

namespace HHT.Resources.DataHelper
{
    public class SndNohinMateHelper : DataBase
    {
        readonly string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        readonly string dbFileName = Application.Context.Resources.GetString(Resource.String.LocalDatabaseFileName);
        
        public bool Insert(SndNohinMate nohinMate)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    connection.Insert(nohinMate);
                    return true;
                }

            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }
        public List<SndNohinMate> SelectAll()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    return connection.Table<SndNohinMate>().ToList();
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }
        public bool DeleteAll()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    connection.DeleteAll<SndNohinMate>();
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