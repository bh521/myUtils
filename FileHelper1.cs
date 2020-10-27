using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace Utils
{
    public class FileHelper
    {
        #region 检测指定路径是否存在
        /// <summary>
        /// 检测指定路径是否存在
        /// </summary>
        /// <param name="Path">目录的绝对路径</param> 
        public static bool IsExistDirectory(string Path)
        {
            return Directory.Exists(Path);
        }
        #endregion
        #region 检测指定文件是否存在,如果存在则返回true
        /// <summary>
        /// 检测指定文件是否存在,如果存在则返回true
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>  
        public static bool IsExistFile(string filePath)
        {
            return File.Exists(filePath);
        }
        #endregion
        #region 创建文件夹
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="FolderPath">文件夹的绝对路径</param>
        public static void CreateFolder(string FolderPath)
        {
            if (!IsExistDirectory(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
        }
        #endregion 
        #region 判断上传文件后缀名
        /// <summary>
        /// 判断上传文件后缀名
        /// </summary>
        /// <param name="strExtension">后缀名</param>
        public static bool IsCanEdit(string strExtension)
        {
            strExtension = strExtension.ToLower();
            if (strExtension.LastIndexOf(".") >= 0)
            {
                strExtension = strExtension.Substring(strExtension.LastIndexOf("."));
            }
            else
            {
                strExtension = ".txt";
            }
            string[] strArray = new string[] { ".htm", ".html", ".txt", ".js", ".css", ".xml", ".sitemap" };
            for (int i = 0; i < strArray.Length; i++)
            {
                if (strExtension.Equals(strArray[i]))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsSafeName(string strExtension)
        {
            strExtension = strExtension.ToLower();
            if (strExtension.LastIndexOf(".") >= 0)
            {
                strExtension = strExtension.Substring(strExtension.LastIndexOf("."));
            }
            else
            {
                strExtension = ".txt";
            }
            string[] strArray = new string[] { ".jpg", ".gif", ".png" };
            for (int i = 0; i < strArray.Length; i++)
            {
                if (strExtension.Equals(strArray[i]))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsZipName(string strExtension)
        {
            strExtension = strExtension.ToLower();
            if (strExtension.LastIndexOf(".") >= 0)
            {
                strExtension = strExtension.Substring(strExtension.LastIndexOf("."));
            }
            else
            {
                strExtension = ".txt";
            }
            string[] strArray = new string[] { ".zip", ".rar" };
            for (int i = 0; i < strArray.Length; i++)
            {
                if (strExtension.Equals(strArray[i]))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
        #region 创建文件夹
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="fileName">文件的绝对路径</param>
        public static void CreateSuffic(string filename)
        {
            try
            {
                if (!Directory.Exists(filename))
                {
                    Directory.CreateDirectory(filename);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="fileName">文件的绝对路径</param>
        public static void CreateFiles(string fileName)
        {
            try
            {
                //判断文件是否存在，不存在创建该文件
                if (!IsExistFile(fileName))
                {
                    FileInfo file = new FileInfo(fileName);
                    FileStream fs = file.Create();
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex; 
            }
        }
        /// <summary>
        /// 创建一个文件,并将字节流写入文件。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="buffer">二进制流数据</param>
        public static void CreateFile(string filePath, byte[] buffer)
        {
            try
            {
                //判断文件是否存在，不存在创建该文件
                if (!IsExistFile(filePath))
                {
                    FileInfo file = new FileInfo(filePath);
                    FileStream fs = file.Create();
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Close();
                }
                else
                {
                    File.WriteAllBytes(filePath, buffer);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region 将文件移动到指定目录
        /// <summary>
        /// 将文件移动到指定目录
        /// </summary>
        /// <param name="sourceFilePath">需要移动的源文件的绝对路径</param>
        /// <param name="descDirectoryPath">移动到的目录的绝对路径</param>
        public static void Move(string sourceFilePath, string descDirectoryPath)
        {
            string sourceName = GetFileName(sourceFilePath);
            if (IsExistDirectory(descDirectoryPath))
            {
                //如果目标中存在同名文件,则删除
                if (IsExistFile(descDirectoryPath + "\\" + sourceFilePath))
                {
                    DeleteFile(descDirectoryPath + "\\" + sourceFilePath);
                }
                else
                {
                    //将文件移动到指定目录
                    File.Move(sourceFilePath, descDirectoryPath + "\\" + sourceFilePath);
                }
            }
        }
        #endregion 
        /// <summary>
        /// 将源文件的内容复制到目标文件中
        /// </summary>
        /// <param name="sourceFilePath">源文件的绝对路径</param>
        /// <param name="descDirectoryPath">目标文件的绝对路径</param>
        public static void Copy(string sourceFilePath, string descDirectoryPath)
        {
            File.Copy(sourceFilePath, descDirectoryPath, true);
        }
        /// <summary>
        /// 从文件的绝对路径中获取文件名( 不包含扩展名 )
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param> 
        public static string GetFileName(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            return file.Name;
        }
        /// <summary>
        /// 获取文件的后缀名
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static string GetExtension(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            return file.Extension;
        }
        /// <summary>
        /// 删除指定文件
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static void DeleteFile(string filePath)
        {
            if (IsExistFile(filePath))
            {
                File.Delete(filePath);
            }
        }
        /// <summary>
        /// 删除指定目录及其所有子目录
        /// </summary>
        /// <param name="directoryPath">文件的绝对路径</param>
        public static void DeleteDirectory(string directoryPath)
        {
            if (IsExistDirectory(directoryPath))
            {
                Directory.Delete(directoryPath);
            }
        }
        /// <summary>
        /// 清空指定目录下所有文件及子目录,但该目录依然保存.
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        public static void ClearDirectory(string directoryPath)
        {
            if (IsExistDirectory(directoryPath))
            {
                //删除目录中所有的文件
                string[] fileNames = GetFileNames(directoryPath);
                for (int i = 0; i < fileNames.Length; i++)
                {
                    DeleteFile(fileNames[i]);
                }
                //删除目录中所有的子目录
                string[] directoryNames = GetDirectories(directoryPath);
                for (int i = 0; i < directoryNames.Length; i++)
                {
                    DeleteDirectory(directoryNames[i]);
                }
            }
        }
        #region  剪切  粘贴
        /// <summary>
        /// 剪切文件
        /// </summary>
        /// <param name="source">原路径</param> 
        /// <param name="destination">新路径</param> 
        public bool FileMove(string source, string destination)
        {
            bool ret = false;
            FileInfo file_s = new FileInfo(source);
            FileInfo file_d = new FileInfo(destination);
            if (file_s.Exists)
            {
                if (!file_d.Exists)
                {
                    file_s.MoveTo(destination);
                    ret = true;
                }
            }
            if (ret == true)
            {
                //Response.Write("<script>alert('剪切文件成功！');</script>");
            }
            else
            {
                //Response.Write("<script>alert('剪切文件失败！');</script>");
            }
            return ret;
        }
        #endregion
        /// <summary>
        /// 检测指定目录是否为空
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>  
        public static bool IsEmptyDirectory(string directoryPath)
        {
            try
            {
                //判断文件是否存在
                string[] fileNames = GetFileNames(directoryPath);
                if (fileNames.Length > 0)
                {
                    return false;
                }
                //判断是否存在文件夹
                string[] directoryNames = GetDirectories(directoryPath);
                if (directoryNames.Length > 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return true;
            }
        }
        #region 获取指定目录中所有文件列表
        /// <summary>
        /// 获取指定目录中所有文件列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>  
        public static string[] GetFileNames(string directoryPath)
        {
            if (!IsExistDirectory(directoryPath))
            {
                throw new FileNotFoundException();
            }
            return Directory.GetFiles(directoryPath);
        }
        #endregion
        #region 获取指定目录中的子目录列表
        /// <summary>
        /// 获取指定目录中所有子目录列表,若要搜索嵌套的子目录列表,请使用重载方法
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        public static string[] GetDirectories(string directoryPath)
        {
            try
            {
                return Directory.GetDirectories(directoryPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 获取指定目录及子目录中所有子目录列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>
        /// <param name="isSearchChild">是否搜索子目录</param>
        public static string[] GetDirectories(string directoryPath, string searchPattern, bool isSearchChild)
        {
            try
            {
                if (isSearchChild)
                {
                    return Directory.GetDirectories(directoryPath, searchPattern, SearchOption.AllDirectories);
                }
                else
                {
                    return Directory.GetDirectories(directoryPath,searchPattern, SearchOption.TopDirectoryOnly);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region 获取一个文件的长度
        /// <summary> 
        /// 获取一个文件的长度,单位为Byte 
        /// </summary> 
        /// <param name="filePath">文件的绝对路径</param>         
        public static int GetFileSize(string filePath)
        {
            //创建一个文件对象 
            FileInfo fi = new FileInfo(filePath);
            //获取文件的大小 
            return (int)fi.Length;
        }
        /// <summary> 
        /// 获取一个文件的长度,单位为KB 
        /// </summary> 
        /// <param name="filePath">文件的路径</param>         
        public static double GetFileSizeByKB(string filePath)
        {
            //创建一个文件对象 
            FileInfo fi = new FileInfo(filePath);
            //获取文件的大小 
            return Convert.ToDouble(filePath.Length)/1024;// ConvertHelper.ToDouble(ConvertHelper.ToDouble(fi.Length) / 1024, 1);
        }
        /// <summary> 
        /// 获取一个文件的长度,单位为MB 
        /// </summary> 
        /// <param name="filePath">文件的路径</param>         
        //public static double GetFileSizeByMB(string filePath)
        //{
        //    //创建一个文件对象 
        //    FileInfo fi = new FileInfo(filePath);
        //    //获取文件的大小 
        //    return ConvertHelper.ToDouble(ConvertHelper.ToDouble(fi.Length) / 1024 / 1024, 1);
        //}
        #endregion
    }
}
