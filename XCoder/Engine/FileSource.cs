using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using NewLife;
using NewLife.Log;

namespace XCoder
{
    /// <summary>文件资源</summary>
    public static class Source
    {
        public static Icon GetIcon()
        {
            try
            {
                return new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(Source), "leaf.ico"));
            }
            catch (Exception ex)
            {
                XTrace.WriteException(ex);

                return null;
            }
        }

        /// <summary>获取文件资源</summary>
        /// <param name="asm"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Stream GetFileResource(this Assembly asm, String filename)
        {
            if (String.IsNullOrEmpty(filename)) return null;

            var name = String.Empty;
            if (asm == null) asm = Assembly.GetCallingAssembly();
            var ss = asm.GetManifestResourceNames();
            if (ss != null && ss.Length > 0)
            {
                //找到资源名
                name = ss.FirstOrDefault(e => e == filename);
                if (String.IsNullOrEmpty(name)) name = ss.FirstOrDefault(e => e.EqualIgnoreCase(filename));
                if (String.IsNullOrEmpty(name)) name = ss.FirstOrDefault(e => e.EndsWith(filename));

                if (!String.IsNullOrEmpty(name)) return asm.GetManifestResourceStream(name);
            }
            return null;
        }

        public static String GetText(String name)
        {
            if (Path.GetExtension(name).IsNullOrWhiteSpace()) name += ".txt";
            var ms = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(Source), name);
            return ms.ToStr();
        }

        /// <summary>释放模版文件</summary>
        public static Dictionary<String, String> GetTemplates()
        {
            var ss = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            if (ss == null || ss.Length <= 0) return null;

            var dic = new Dictionary<String, String>();

            //找到资源名
            foreach (var item in ss)
            {
                if (item.StartsWith("XCoder.App."))
                {
                    ReleaseFile(item, "XCoder.exe.config");
                }
                else if (item.StartsWith("XCoder.Template."))
                {
                    var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(item);
                    var tempName = item.Substring("XCoder.Template.".Length);
                    var buffer = new Byte[stream.Length];
                    var count = stream.Read(buffer, 0, buffer.Length);

                    var content = Encoding.UTF8.GetString(buffer, 0, count);
                    dic.Add(tempName, content);
                }
            }

            return dic.OrderBy(e => e.Key).ToDictionary(e => e.Key, e => e.Value);
        }

        /// <summary>读取资源，并写入到文件</summary>
        /// <param name="name">名称</param>
        /// <param name="fileName"></param>
        public static void ReleaseFile(String name, String fileName)
        {
            fileName = fileName.GetFullPath();
            if (String.IsNullOrEmpty(fileName) || File.Exists(fileName)) return;

            try
            {
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
                if (stream == null) return;

                var buffer = new Byte[stream.Length];
                var count = stream.Read(buffer, 0, buffer.Length);

                fileName = fileName.GetFullPath();
                var p = Path.GetDirectoryName(fileName);
                if (!String.IsNullOrEmpty(p) && !Directory.Exists(p)) Directory.CreateDirectory(p);

                File.WriteAllBytes(fileName, buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}