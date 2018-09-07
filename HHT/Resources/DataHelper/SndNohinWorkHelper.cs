using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Util;
using HHT.Resources.Model;
using SQLite;

namespace HHT.Resources.DataHelper
{
    public class SndNohinWorkHelper : DataBase
    {
        readonly string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        readonly string dbFileName = Application.Context.Resources.GetString(Resource.String.LocalDatabaseFileName);
        
        public bool Insert(SndNohinWork nohinWork)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    connection.Insert(nohinWork);
                    return true;
                }

            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }
        public List<SndNohinWork> SelectAll()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    List<SndNohinWork> nohinWorkList = connection.Table<SndNohinWork>().ToList();
                    return nohinWorkList;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }
        public List<SndNohinWork> SelectNohinWorkWithKamotu(string kamotu)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    List<SndNohinWork> nohinWorkList = connection.Query<SndNohinWork>("select * from SndNohinWork where wKamotuNo = ?", kamotu);
                    return nohinWorkList;
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
                    connection.DeleteAll<SndNohinWork>();
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