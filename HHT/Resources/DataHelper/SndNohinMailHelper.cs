using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Util;
using HHT.Resources.Model;
using SQLite;

namespace HHT.Resources.DataHelper
{
    public class SndNohinMailHelper : DataBase
    {
        readonly string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        readonly string dbFileName = Application.Context.Resources.GetString(Resource.String.LocalDatabaseFileName);
        
        public bool Insert(SndNohinMail sndNohinMail)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    connection.Insert(sndNohinMail);
                    return true;
                }

            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }
        public List<SndNohinMail> SelectAll()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    List<SndNohinMail> nohinMailList = connection.Table<SndNohinMail>().ToList();
                    return nohinMailList;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }
        public List<SndNohinMail> Select(string tokuisaki_cd, string todoke_cd, string mailback_cd)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    string sql = "select * from SndNohinMail where wTokuisakiCD = ? and wTodokesakiCD =? and wKanriNo = ?";
                    List<SndNohinMail> result = connection.Query<SndNohinMail>(sql, tokuisaki_cd, todoke_cd, mailback_cd);
                    
                    return result;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }
        public void DeleteAll()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    connection.DeleteAll<SndNohinMail>();
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
            }
        }
    }
}