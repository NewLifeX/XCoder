using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using NewLife;
using NewLife.Log;
using XCode.DataAccessLayer;

namespace CrazyCoder.Data;

internal class DbHelper
{
    public static async Task AutoDetectDatabase(Action callback)
    {
        // 加上本机MSSQL
        var localName = "local_MSSQL";
        var localstr = "Data Source=.;Initial Catalog=master;Integrated Security=True;";
        if (!ContainConnStr(localstr)) DAL.AddConnStr(localName, localstr, null, "mssql");

        // 检测本地Access和SQLite
        //ThreadPoolX.QueueUserWorkItem(DetectFile);
        var t1 = Task.Run(() => DetectFile(callback));

        //!!! 必须另外实例化一个列表，否则作为数据源绑定时，会因为是同一个对象而被跳过
        //var list = new List<String>();

        // 探测连接中的其它库
        //ThreadPoolX.QueueUserWorkItem(DetectRemote);
        var t2 = Task.Run(() => DetectRemote(callback));

        await Task.WhenAll(t1, t2);
    }

    static Boolean ContainConnStr(String connstr)
    {
        foreach (var item in DAL.ConnStrs)
        {
            if (connstr.EqualIgnoreCase(item.Value)) return true;
        }
        return false;
    }

    static void DetectFile(Action callback)
    {
        var sw = Stopwatch.StartNew();

        var n = 0;
        var ss = Directory.GetFiles(".".GetFullPath(), "*.*", SearchOption.TopDirectoryOnly);
        foreach (var item in ss)
        {
            var ext = Path.GetExtension(item);
            if (ext.EqualIgnoreCase(".exe", ".dll", ".zip", ".rar", ".txt", ".config")) continue;

            try
            {
                if (DetectFileDb(item)) n++;
            }
            catch (Exception ex)
            {
                XTrace.WriteException(ex);
            }
        }
        ss = Directory.GetFiles(Setting.Current.DataPath.GetFullPath(), "*.*", SearchOption.TopDirectoryOnly);
        foreach (var item in ss)
        {
            var ext = Path.GetExtension(item);
            if (ext.EqualIgnoreCase(".exe", ".dll", ".zip", ".rar", ".txt", ".config")) continue;

            try
            {
                if (DetectFileDb(item)) n++;
            }
            catch (Exception ex)
            {
                XTrace.WriteException(ex);
            }
        }

        sw.Stop();
        XTrace.WriteLine("自动检测文件{0}个，发现数据库{1}个，耗时：{2}！", ss.Length, n, sw.Elapsed);

        var list = new List<String>();
        foreach (var item in DAL.ConnStrs)
        {
            if (!String.IsNullOrEmpty(item.Value)) list.Add(item.Key);
        }

        // 远程数据库耗时太长，这里先列出来
        //this.Invoke(SetDatabaseList, list);
        callback();
    }

    static Boolean DetectFileDb(String item)
    {
        var sqlite = "SQLite";

        using (var fs = new FileStream(item, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            if (fs.Length <= 0) return false;

            var reader = new BinaryReader(fs);
            var bts = reader.ReadBytes(sqlite.Length);
            if (bts != null && bts.Length > 0)
            {
                if (bts[0] == 'S' && bts[1] == 'Q' && Encoding.ASCII.GetString(bts) == sqlite)
                {
                    var localstr = String.Format("Data Source={0};", item);
                    if (!ContainConnStr(localstr)) DAL.AddConnStr("SQLite_" + Path.GetFileNameWithoutExtension(item), localstr, null, "SQLite");
                    return true;
                }
            }
        }

        return false;
    }

    static async Task DetectRemote(Action callback)
    {
        var list = new List<String>();
        foreach (var item in DAL.ConnStrs)
        {
            if (!String.IsNullOrEmpty(item.Value)) list.Add(item.Key);
        }

        var ts = new List<Task>();
        foreach (var item in list)
        {
            var t1 = Task.Run(() => DetectSqlServer(item, callback));
            var t2 = Task.Run(() => DetectMySql(item, callback));
            ts.Add(t1);
            ts.Add(t2);
        }

        var localName = "local_MSSQL";
        if (DAL.ConnStrs.ContainsKey(localName)) DAL.ConnStrs.Remove(localName);
        //if (list.Contains(localName)) list.Remove(localName);

        await Task.WhenAll(ts);
    }

    static void DetectSqlServer(String item, Action callback)
    {
        try
        {
            var dal = DAL.Create(item);
            if (dal.DbType != DatabaseType.SqlServer) return;

            var sw = Stopwatch.StartNew();

            DataTable dt = null;

            // 列出所有数据库
            //Boolean old = DAL.ShowSQL;
            //DAL.ShowSQL = false;
            //try
            //{
            if (dal.Db.CreateMetaData().MetaDataCollections.Contains("Databases"))
            {
                dt = dal.Db.CreateSession().GetSchema(null, "Databases", null);
            }
            //}
            //finally { DAL.ShowSQL = old; }

            if (dt == null) return;

            var dbprovider = dal.DbType.ToString();
            var builder = new DbConnectionStringBuilder
            {
                ConnectionString = dal.ConnStr
            };

            // 统计库名
            var n = 0;
            var names = new List<String>();
            var sysdbnames = new String[] { "master", "tempdb", "model", "msdb" };
            foreach (DataRow dr in dt.Rows)
            {
                var dbname = dr[0].ToString();
                if (Array.IndexOf(sysdbnames, dbname) >= 0) continue;

                var connName = String.Format("{0}_{1}", item, dbname);

                builder["Database"] = dbname;
                DAL.AddConnStr(connName, builder.ToString(), null, dbprovider);
                n++;

                try
                {
                    var ver = dal.Db.ServerVersion;
                    names.Add(connName);
                }
                catch
                {
                    if (DAL.ConnStrs.ContainsKey(connName)) DAL.ConnStrs.Remove(connName);
                }
            }


            sw.Stop();
            XTrace.WriteLine("发现远程数据库{0}个，耗时：{1}！", n, sw.Elapsed);

            if (names != null && names.Count > 0)
            {
                var list = new List<String>();
                foreach (var elm in DAL.ConnStrs)
                {
                    if (!String.IsNullOrEmpty(elm.Value)) list.Add(elm.Key);
                }
                list.AddRange(names);

                //this.Invoke(SetDatabaseList, list);
            }
        }
        catch
        {
            //if (item == localName) DAL.ConnStrs.Remove(localName);
        }
    }

    static void DetectMySql(String item, Action callback)
    {
        try
        {
            var dal = DAL.Create(item);
            if (dal.DbType != DatabaseType.MySql) return;

            var sw = Stopwatch.StartNew();

            // 列出所有数据库
            DataTable dt = null;
            if (dal.Db.CreateMetaData().MetaDataCollections.Contains("Databases"))
            {
                dt = dal.Db.CreateSession().GetSchema(null, "Databases", null);
            }
            if (dt == null) return;

            var dbprovider = dal.DbType.ToString();
            var builder = new DbConnectionStringBuilder
            {
                ConnectionString = dal.ConnStr
            };

            // 统计库名
            var n = 0;
            var names = new List<String>();
            var sysdbnames = new String[] { "mysql" };
            foreach (DataRow dr in dt.Rows)
            {
                var dbname = dr["database_name"].ToString();
                if (Array.IndexOf(sysdbnames, dbname) >= 0) continue;

                var connName = String.Format("{0}_{1}", item, dbname);

                builder["Database"] = dbname;
                DAL.AddConnStr(connName, builder.ToString(), null, dbprovider);
                n++;

                try
                {
                    var ver = dal.Db.ServerVersion;
                    names.Add(connName);
                }
                catch
                {
                    if (DAL.ConnStrs.ContainsKey(connName)) DAL.ConnStrs.Remove(connName);
                }
            }

            sw.Stop();
            XTrace.WriteLine("发现远程数据库{0}个，耗时：{1}！", n, sw.Elapsed);

            if (names != null && names.Count > 0)
            {
                //var list = new List<String>();
                //foreach (var elm in DAL.ConnStrs)
                //{
                //    if (!String.IsNullOrEmpty(elm.Value)) list.Add(elm.Key);
                //}
                //list.AddRange(names);

                //this.Invoke(SetDatabaseList, list);
                callback();
            }
        }
        catch
        {
            //if (item == localName) DAL.ConnStrs.Remove(localName);
        }
    }
}