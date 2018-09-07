using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Util;
using HHT.Resources.Model;
using SQLite;

namespace HHT.Resources.DataHelper
{
    public class TokuiFileHelper : DataBase
    {
        readonly string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        readonly string dbFileName = Application.Context.Resources.GetString(Resource.String.LocalDatabaseFileName);
        
        public bool Insert(TokuiFile file)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    connection.Insert(file);
                    return true;
                }

            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool InsertAll(List<TokuiFile> file)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    connection.DeleteAll<TokuiFile>();
                    connection.InsertAll(file);
                    return true;
                }

            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }


        public List<TokuiFile> SelectByTokuisakiWithTodokesaki(string tokuisaki, string todokesaki)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    string sql = "select * from TOkuiFile where tokuisaki_cd =? and todokesaki_cd=? ";

                    List<TokuiFile> tokuisakiListInfo = connection.Query<TokuiFile>(sql, tokuisaki, todokesaki);
                    return tokuisakiListInfo;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }

    }
}