using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Util;
using HHT.Resources.Model;
using SQLite;

namespace HHT.Resources.DataHelper
{
    public class MFileHelper : DataBase
    {
        readonly string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        readonly string dbFileName = Application.Context.Resources.GetString(Resource.String.LocalDatabaseFileName);
        
        public bool Insert(MFile mFile)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    connection.DeleteAll<MFile>();
                    connection.Insert(mFile);
                    return true;
                }

            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool InsertALL(List<MFile> tsumikomiList)
        {
            try
            {
                if(tsumikomiList.Count > 0)
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                    {
                        connection.InsertAll(tsumikomiList);
                        return true;
                    }
                }
                else
                {
                    return false;
                }
                
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }
        

        public List<MFile> SelectALLTokuisakiInfo()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    List<MFile> tokuisakiListInfo = connection.Table<MFile>().ToList();
                    return tokuisakiListInfo;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }

        public List<MFile> SelectTsumikomiList(string tokuisaki , string todokesaki)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    //SQLiteCommand cmd = connection.CreateCommand();
                    string sql = @"select * from MFile where tokuisaki_cd = ? and todokesaki_cd= ?";
                    List<MFile> tokuisakiList = connection.Query<MFile>(sql, tokuisaki, todokesaki).ToList();
                       
                    return tokuisakiList;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }


        public List<MFile> SelectByMatehanCd(string tokuisaki, string todokesaki, string matehanCd)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    //SQLiteCommand cmd = connection.CreateCommand();
                    string sql = @"select * from MFile where tokuisaki_cd = ? and todokesaki_cd= ? and matehan = ?";
                    List<MFile> tokuisakiList = connection.Query<MFile>(sql, tokuisaki, todokesaki, matehanCd).ToList();

                    return tokuisakiList;
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
                    connection.DeleteAll<MFile>();
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