﻿using System;
using System.Net;
using System.Web;
namespace Core.Net
{
    /// <summary>
    /// 共用工具类
    /// </summary>
    public static class IpHelper
    {
        #region 获得用户IP
        /// <summary>
        /// 获得用户IP
        /// </summary>
        public static string GetUserIp()
        {
            string ip;
            string[] temp;
            bool isErr = false;
            if (HttpContext.Current.Request.ServerVariables["HTTP_X_ForWARDED_For"] == null)
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            else
                ip = HttpContext.Current.Request.ServerVariables["HTTP_X_ForWARDED_For"].ToString();
            if (ip.Length > 15)
                isErr = true;
            else
            {
                temp = ip.Split('.');
                if (temp.Length == 4)
                {
                    for (int i = 0; i < temp.Length; i++)
                    {
                        if (temp[i].Length > 3) isErr = true;
                    }
                }
                else
                    isErr = true;
            }
            if (isErr)
                return "1.1.1.1";
            else
                return ip;
        }
        #endregion 获得用户IP
        #region 获得当前页面客户端的IP
        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetIP()
        {
            string result = String.Empty;
            result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }
            if (null == result || result == String.Empty || !IsIP(result))
            {
                return "0.0.0.0";
            }
            return result;
        }
        #endregion 获得当前页面客户端的IP
        #region 把IP地址数字格式转换为string
        /// <summary>
        /// 把IP地址数字格式转换为string
        /// </summary>
        /// <param name="a">ip地址</param>
        /// <returns>字符串</returns>
        private static String intToString(int a)
        {
            string sb = "";
            int b = (a >> 0) & 0xff;
            sb = "." + b;
            b = (a >> 8) & 0xff;
            sb = "." + b + sb;
            b = (a >> 16) & 0xff;
            sb = "." + b + sb;
            b = (a >> 24) & 0xff;
            sb = b + sb;
            return sb;
        } 
        #endregion
        #region 把IP地址转换为数字格式
        /// <summary>
        /// 把IP地址转换为数字格式
        /// </summary>
        /// <param name="strIp">IP地址</param>
        /// <returns>数字</returns>
        public static int IPtoNum(string strIp)
        {
            string[] temp = strIp.Split('.');
            return (int.Parse(temp[0])) * 256 * 256 * 256 + (int.Parse(temp[1])) * 256 * 256 * 256 + (int.Parse(temp[2])) * 256 * 256 * 256;
        }
        #endregion 把IP地址转换为数字格式
        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static bool IsIP(string ip)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(ip,
                @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        /// <summary>
        /// 获取本地机器IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIP()
        {
            string strHostIP = string.Empty;
            IPHostEntry oIPHost = Dns.GetHostEntry(Environment.MachineName);
            if (oIPHost.AddressList.Length > 0)
            {
                strHostIP = oIPHost.AddressList[0].ToString();
            }
            return strHostIP;
        }
        #region 把IP地址转换为数字格式
        /// <summary>
        /// 把IP地址转换为数字格式
        /// </summary>
        /// <param name="strIp">IP地址</param>
        /// <returns>数字</returns>
        public static int IPtoNum(string strIp)
        {
            string[] temp = strIp.Split('.');
            return (int.Parse(temp[0])) * 256 * 256 * 256 + (int.Parse(temp[1])) * 256 * 256 * 256 + (int.Parse(temp[2])) * 256 * 256 * 256;
        }
        #endregion 把IP地址转换为数字格式
    }
}