using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Util;
using HHT.Resources.Model;
using SQLite;

namespace HHT.Resources.DataHelper
{
    public class TantoHelper : DataBase
    {
        readonly string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        readonly string dbFileName = Application.Context.Resources.GetString(Resource.String.LocalDatabaseFileName);

        public bool InsertTantoList(List<Tanto> tantoList)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    connection.Execute("DELETE FROM Tanto");

                    connection.InsertAll(tantoList);
                    
                    return true;
                }

            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool InsertIntoTableTantoInfo(Tanto tanto)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    connection.Insert(tanto);
                    return true;
                }

            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public Tanto SelectTantoInfo(string driver_cd)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    List<Tanto> tantoList = connection.Query<Tanto>("select * from Tanto where tantohsya_cd = ?", driver_cd);
                    
                    if (tantoList.Count > 0)
                    {
                        return tantoList[0];
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


        public List<Tanto> SelectALLTokuisakiInfo()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    List<Tanto> tokuisakiListInfo = connection.Table<Tanto>().ToList();
                    return tokuisakiListInfo;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }

        public void DeleteAllTantoInfo()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, dbFileName)))
                {
                    List<Tanto> tokuisakiListInfo = connection.Table<Tanto>().ToList();
                    connection.Execute("Delete From Tanto");
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return;
            }
        }
    }
}