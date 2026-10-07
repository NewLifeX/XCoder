using NewLife;
using NewLife.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALib
{
    public class _StructuralLog<T> : DisposeBase where T : _StructuralLog<T> 
    {

        #region 属性

        /// <summary>目录</summary>
        public String BasePath { get; set; } = "StrtLog/";

        /// <summary>名称，日志目录名称</summary>
        public String Name { get; set; } = "123";

        /// <summary>日志路径</summary>
        public String LogPath { get; private set; }

        /// <summary>根日志</summary>
        public CompositeLog RootLog { get; private set; } = new CompositeLog();

        /// <summary>子项日志</summary>
        private Dictionary<String, TextFileLog> _Dic = new();

        #endregion

        /// <summary>构造</summary>
        /// <param name="sn">S/N号</param>
        /// <param name="basepath">日志路径</param>
        /// <param name="time">目录时间戳</param>
        public _StructuralLog(string sn, string basepath = "StrtLog/", bool addtime = true)
        {
            if (sn == null) sn = "123";
            if (sn.Length < 1) sn = "123";
            Name = sn;

            if (basepath == null) basepath = "StrtLog/";
            if (basepath.Length < 1) basepath = "StrtLog/";
            if (!basepath.EndsWith("/")) basepath += '/';
            BasePath = basepath;

            var path = PathHelper.GetFullPath(BasePath);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var time = DateTime.Now;
            // 生成适合文件目录的时间戳格式: _yyMMdd_HHmmss
            var tstr = time.ToString("_yyMMdd_HHmmss");

            var snpath = path + Name + tstr;
            if (!addtime) snpath = path + Name;

            if (!snpath.EndsWith('/')) snpath = snpath + "/";
            if (!Directory.Exists(snpath)) Directory.CreateDirectory(snpath);

            LogPath = snpath;

            // 根日志的文件部分
            var zroot = LogPath + "z.root.txt";
            var tlg = TextFileLog.CreateFile(zroot);
            // 装进去
            RootLog.Add(tlg);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            RootLogClear();
        }

        /// <summary>清空日志输出途径</summary>
        public void RootLogClear()
        {
            RootLog.Logs.Clear();
        }

        #region 写日志

        /// <summary>写根日志</summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WriteRootLog(String format, params Object[] args) => RootLog?.Info(format, args);

        /// <summary>写日志</summary>
        /// <param name="name">日志，root为根日志，其余为子项</param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WriteLog(String name, String format, params Object[] args)
        {
            if (name == null) name = "root";
            if (name.Length < 1) name = "root";
            if (name == "root")
            {
                WriteRootLog(format, args);
                return;
            }

            ILog log = SubItemLog(name);
            log?.Info(format, args);
        }

        /// <summary>当前日志</summary>
        public String CurrWork { get; set; } = "root";

        /// <summary>按照CurrWork写日志</summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WorkLog(String format, params Object[] args)
        {
            var work = SubItemLog(CurrWork);
            work?.Info(format, args);
        }

        #endregion

        /// <summary>子项的日志路径</summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public String SubItemPath(string name)
        {
            if (name == null) return "";
            return LogPath + name + ".txt";
        }

        /// <summary>子项的文本日志对象</summary>
        /// <param name="name">子项名称</param>
        /// <returns></returns>
        public TextFileLog SubItemLog(string name)
        {
            if (name == null) return null;

            if (_Dic.ContainsKey(name)) return _Dic[name];

            var file = LogPath + name + ".txt";
            var log = TextFileLog.CreateFile(file);
            _Dic.Add(name, log);
            return log;
        }

        private static T _curr;
        /// <summary>当前项</summary>
        public static T Current
        {
            get { return _curr; }
            set
            {
                var old = _curr;
                _curr = value;

                if (old != null)
                {
                    old.TryDispose();
                    // var flog = old.RootLog.Get<TextFileLog>();
                    // flog?.Dispose();
                }
            }
        }
    }

    public class StructuralLog : _StructuralLog<StructuralLog>
    {
        public StructuralLog(string sn, string basepath = "StrtLog/", bool addtime = true) : base(sn, basepath, addtime) { }
    }
}
