using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
namespace SiteClass
{
    /// <summary>
    /// .INI文件 操作。
    /// </summary>
    public class ftpini
    {
        /// <summary>
        /// 创建一个如下的INI对象
        /// INI ini = new INI(@"C:\test.ini");
        /// </summary>
        public string path;
        //引用动态连接库方法
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// 节点名
        /// <PARAM name="Key"></PARAM>
        /// 键名
        /// <PARAM name="Value"></PARAM>
        /// 值名
        public void Write(string Section, string Key, string value, string path)
        {
            WritePrivateProfileString(Section, Key, value, path);
        }
        /// <summary>
        /// 读取INI数据
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// <PARAM name="Key"></PARAM>
        /// <PARAM name="Path"></PARAM>
        /// <returns></returns>
        public string Read(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            string path = System.Configuration.ConfigurationManager.AppSettings["FTPinstallpath"] + "ServUDaemon.ini";
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, path);
            return temp.ToString();
        }
        public void EditFtppassword(string ftpaccount, string ftppassword)
        {
            string FTPinstallpath = System.Configuration.ConfigurationManager.AppSettings["FTPinstallpath"];
            this.Write("USER=" + ftpaccount + "|1", "Password", "dv" + System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile("dv" + ftppassword, "md5"), FTPinstallpath + "ServUDaemon.ini");
            this.Write("GLOBAL", "ReloadSettings", "True", FTPinstallpath + "ServUDaemon.ini");
        }
        /// <summary>
        /// 开通FTP帐号
        /// </summary>
        /// <param name="ftpaccount"></param>
        /// <param name="ftppassword"></param>
        /// <param name="Homedir"></param>
        /// <param name="spaceM"></param>
        /// <param name="maxupload"></param>
        /// <param name="maxdownload"></param>
        /// <param name="sessiontimeout"></param>
        /// <param name="maxusercon"></param>
        /// <returns></returns>
        public void OpenFtp(string ftpaccount, string ftppassword, string Homedir, string spaceM, string proid)
        {
            string MaxUpload = System.Configuration.ConfigurationManager.AppSettings["MaxUpload"];
            string MaxDownload = System.Configuration.ConfigurationManager.AppSettings["MaxDownload"];
            string SessionTimeOut = System.Configuration.ConfigurationManager.AppSettings["SessionTimeOut"];
            string MaxUserConnection = System.Configuration.ConfigurationManager.AppSettings["MaxUserConnection"];
            string FTPinstallpath = System.Configuration.ConfigurationManager.AppSettings["FTPinstallpath"];
            //向FTP安装目录ServUDaemon.ini写入用户记录
            this.Write("Domain1", "User" + proid.Trim(), ftpaccount + "|1|0", FTPinstallpath + "ServUDaemon.ini");
            this.Write("USER=" + ftpaccount + "|1", "Password", "dv" + System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile("dv" + ftppassword, "md5"), FTPinstallpath + "ServUDaemon.ini");
            this.Write("USER=" + ftpaccount + "|1", "HomeDir", Homedir, FTPinstallpath + "ServUDaemon.ini");
            this.Write("USER=" + ftpaccount + "|1", "RelPaths", "1", FTPinstallpath + "ServUDaemon.ini");
            this.Write("USER=" + ftpaccount + "|1", "HideHidden", "1", FTPinstallpath + "ServUDaemon.ini");
            this.Write("USER=" + ftpaccount + "|1", "PasswordLastChange", "1183865700", FTPinstallpath + "ServUDaemon.ini");
            this.Write("USER=" + ftpaccount + "|1", "SpeedLimitUp", (Convert.ToInt32(MaxUpload) * 1024).ToString(), FTPinstallpath + "ServUDaemon.ini");
            this.Write("USER=" + ftpaccount + "|1", "SpeedLimitDown", (Convert.ToInt32(MaxDownload) * 1024).ToString(), FTPinstallpath + "ServUDaemon.ini");
            this.Write("USER=" + ftpaccount + "|1", "TimeOut", "600", FTPinstallpath + "ServUDaemon.ini");
            this.Write("USER=" + ftpaccount + "|1", "SessionTimeOut", (Convert.ToInt32(SessionTimeOut) * 60).ToString(), FTPinstallpath + "ServUDaemon.ini");
            this.Write("USER=" + ftpaccount + "|1", "MaxNrUsers", MaxUserConnection, FTPinstallpath + "ServUDaemon.ini");
            this.Write("USER=" + ftpaccount + "|1", "Access1", Homedir + "|RWAMLCDP", FTPinstallpath + "ServUDaemon.ini");
            this.Write("USER=" + ftpaccount + "|1", "DiskQuota", "1|" + (Convert.ToInt32(spaceM) * 1024 * 1024).ToString() + "|0", FTPinstallpath + "ServUDaemon.ini");
            this.Write("USER=" + ftpaccount + "|1", "Enable", "1", FTPinstallpath + "ServUDaemon.ini");
            this.Write("GLOBAL", "ReloadSettings", "True", FTPinstallpath + "ServUDaemon.ini");
            //创建这个目录
            if (!Directory.Exists(Homedir))
            {
                Directory.CreateDirectory(Homedir);
            }
            if (!Directory.Exists(Homedir + "/wwwroots"))
            {
                Directory.CreateDirectory(Homedir + "/wwwroots");
            }
            if (!Directory.Exists(Homedir + "/Logfiles"))
            {
                Directory.CreateDirectory(Homedir + "/Logfiles");
            }
            if (!Directory.Exists(Homedir + "/Database"))
            {
                Directory.CreateDirectory(Homedir + "/Database");
            }
        }
        ///   <summary>
        /// 递归读取所有目录大小
        ///   </summary>
        ///   <param   name="FolderPath">文件目录</param>
        ///   <param   name="size">初始大小</param>
        ///   <returns>返回字节</returns>
        public long FolderSize(string FolderPath, long size)
        {
            long Csize = size;
            string[] Folder = Directory.GetDirectories(FolderPath);
            string[] files = Directory.GetFiles(FolderPath);
            int i = 0;
            for (i = 0; i < files.Length; i++)
            {
                try
                {
                    FileAttributes fa = File.GetAttributes(files[i]);
                    FileInfo f = new FileInfo(files[i]);
                    Console.WriteLine(files[i] + "大小:" + f.Length);
                    Csize += f.Length;
                }
                catch
                {
                    //("读取文件失败");
                }
            }
            for (i = 0; i < Folder.Length; i++)
            {
                FolderSize(Folder[i], Csize);
            }
            return Csize;
        }
        public string unzip(string rarpath, string savepath, bool rewrite)
        {
            //解压缩
            string retstr = string.Empty;
            String the_rar;
            RegistryKey the_Reg;
            Object the_Obj;
            String the_Info;
            ProcessStartInfo the_StartInfo;
            Process the_Process;
            try
            {
                the_Reg = Registry.ClassesRoot.OpenSubKey(@"Applications\WinRar.exe\Shell\Open\Command");
                the_Obj = the_Reg.GetValue("");
                the_rar = the_Obj.ToString();
                the_Reg.Close();
                the_rar = the_rar.Substring(1, the_rar.Length - 7);
                if (rewrite)
                {
                    //判断是否要覆盖旧文件
                    the_Info = " X " + rarpath + " " + savepath + "  " + "-o+";
                }
                else
                {
                    the_Info = " X " + rarpath + " " + savepath + "  ";
                }
                the_StartInfo = new ProcessStartInfo();
                the_StartInfo.FileName = the_rar;
                the_StartInfo.Arguments = the_Info;
                the_StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                the_Process = new Process();
                the_Process.StartInfo = the_StartInfo;
                the_Process.Start();
                retstr = "恭喜您，已经解压缩成功，请使用FTP软件登录FTP服务器移动文件或整理";
            }
            catch (Exception ex)
            {
                retstr = ex.Message;
            }
            return retstr;
        }
        public string zip(string path, string savepath, bool childfolder)
        {
            //压缩
            string retstr = string.Empty;
            String the_rar;
            RegistryKey the_Reg;
            Object the_Obj;
            String the_Info;
            ProcessStartInfo the_StartInfo;
            Process the_Process;
            Random rdn = new Random();
            string newrarfile = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Millisecond.ToString() + rdn.Next(99999999).ToString() + ".rar";
            //try
            //{
            the_Reg = Registry.ClassesRoot.OpenSubKey(@"Applications\WinRAR.exe\Shell\Open\Command");
            the_Obj = the_Reg.GetValue("");
            the_rar = the_Obj.ToString();
            the_Reg.Close();
            the_rar = the_rar.Substring(1, the_rar.Length - 7);
            //判断是否将子文件夹也一起打包
            if (childfolder)
            {
                the_Info = " a    " + newrarfile + "  " + path + "  " + "-r";
            }
            else
            {
                the_Info = " a    " + newrarfile + "  " + path;
            }
            the_StartInfo = new ProcessStartInfo();
            the_StartInfo.FileName = the_rar;
            the_StartInfo.Arguments = the_Info;
            the_StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            the_StartInfo.WorkingDirectory = savepath;//获取或设置要启动的进程的初始目录。
            the_Process = new Process();
            the_Process.StartInfo = the_StartInfo;
            the_Process.Start();
            retstr = "恭喜您，已经压缩成功，请使用FTP软件登录FTP服务器下载,文件名为:" + newrarfile;
            //}
            //catch (Exception ex)
            //{
            //    retstr = ex.Message;
            //}
            return retstr;
        }
    }
}