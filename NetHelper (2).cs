﻿using System;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
namespace DotNet.Utilities
{
    /// <summary>网络操作相关的类</summary>
    public class NetHelper
    {
        #region 检查设置的端口号是否正确，返回正确的端口号
        /// <summary>检查设置的端口号是否正确，并返回正确的端口号,无效端口号返回-1。</summary>
        /// <param name="port">设置的端口号</param>
        public static int GetValidPort(string port)
        {
            //声明返回的正确端口号
            int validPort = -1;
            //最小有效端口号
            const int MINPORT = 0;
            //最大有效端口号
            const int MAXPORT = 65535;
            //检测端口号
            try
            {
                //传入的端口号为空则抛出异常
                if (port == "")
                {
                    throw new Exception("端口号不能为空！");
                }
                //检测端口范围
                if ((Convert.ToInt32(port) < MINPORT) || (Convert.ToInt32(port) > MAXPORT))
                {
                    throw new Exception("端口号范围无效！");
                }
                //为端口号赋值
                validPort = Convert.ToInt32(port);
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message;
            }
            return validPort;
        }
        #endregion 检查设置的端口号是否正确，返回正确的端口号
        #region 将字符串形式的IP地址转换成IPAddress对象
        /// <summary>将字符串形式的IP地址转换成IPAddress对象</summary>
        /// <param name="ip">字符串形式的IP地址</param>
        public static IPAddress StringToIPAddress(string ip)
        {
            return IPAddress.Parse(ip);
        }
        #endregion 将字符串形式的IP地址转换成IPAddress对象
        #region 获取本机的计算机名
        /// <summary>获取本机的计算机名</summary>
        public static string LocalHostName
        {
            get
            {
                return Dns.GetHostName();
            }
        }
        #endregion 获取本机的计算机名
        #region 获取本机的局域网IP
        /// <summary>获取本机的局域网IP</summary>
        public static string LANIP
        {
            get
            {
                //获取本机的IP列表,IP列表中的第一项是局域网IP，第二项是广域网IP
                IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                //如果本机IP列表为空，则返回空字符串
                if (addressList.Length < 1)
                {
                    return "";
                }
                //返回本机的局域网IP
                return addressList[0].ToString();
            }
        }
        #endregion 获取本机的局域网IP
        #region 获取本机在Internet网络的广域网IP
        /// <summary>获取本机在Internet网络的广域网IP</summary>
        public static string WANIP
        {
            get
            {
                //获取本机的IP列表,IP列表中的第一项是局域网IP，第二项是广域网IP
                IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                //如果本机IP列表小于2，则返回空字符串
                if (addressList.Length < 2)
                {
                    return "";
                }
                //返回本机的广域网IP
                return addressList[1].ToString();
            }
        }
        #endregion 获取本机在Internet网络的广域网IP
        #region 获取远程客户机的IP地址
        /// <summary>获取远程客户机的IP地址</summary>
        /// <param name="clientSocket">客户端的socket对象</param>
        public static string GetClientIP(Socket clientSocket)
        {
            IPEndPoint client = (IPEndPoint)clientSocket.RemoteEndPoint;
            return client.Address.ToString();
        }
        #endregion 获取远程客户机的IP地址
        #region 创建一个IPEndPoint对象
        /// <summary>创建一个IPEndPoint对象</summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口号</param>
        public static IPEndPoint CreateIPEndPoint(string ip, int port)
        {
            IPAddress ipAddress = StringToIPAddress(ip);
            return new IPEndPoint(ipAddress, port);
        }
        #endregion 创建一个IPEndPoint对象
        #region 创建一个TcpListener对象
        /// <summary>创建一个自动分配IP和端口的TcpListener对象</summary>
        public static TcpListener CreateTcpListener()
        {
            //创建一个自动分配的网络节点
            IPAddress ipAddress = IPAddress.Any;
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 0);
            return new TcpListener(localEndPoint);
        }
        /// <summary>创建一个TcpListener对象</summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口</param>
        public static TcpListener CreateTcpListener(string ip, int port)
        {
            //创建一个网络节点
            IPAddress ipAddress = StringToIPAddress(ip);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            return new TcpListener(localEndPoint);
        }
        #endregion 创建一个TcpListener对象
        #region 创建一个基于TCP协议的Socket对象
        /// <summary>创建一个基于TCP协议的Socket对象</summary>
        public static Socket CreateTcpSocket()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        #endregion 创建一个基于TCP协议的Socket对象
        #region 创建一个基于UDP协议的Socket对象
        /// <summary>创建一个基于UDP协议的Socket对象</summary>
        public static Socket CreateUdpSocket()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }
        #endregion 创建一个基于UDP协议的Socket对象
        #region 获取本地终结点
        #region 获取TcpListener对象的本地终结点
        /// <summary>获取TcpListener对象的本地终结点</summary>
        /// <param name="tcpListener">TcpListener对象</param>
        public static IPEndPoint GetLocalPoint(TcpListener tcpListener)
        {
            return (IPEndPoint)tcpListener.LocalEndpoint;
        }
        /// <summary>获取TcpListener对象的本地终结点的IP地址</summary>
        /// <param name="tcpListener">TcpListener对象</param>
        public static string GetLocalPoint_IP(TcpListener tcpListener)
        {
            IPEndPoint localEndPoint = (IPEndPoint)tcpListener.LocalEndpoint;
            return localEndPoint.Address.ToString();
        }
        /// <summary>获取TcpListener对象的本地终结点的端口号</summary>
        /// <param name="tcpListener">TcpListener对象</param>
        public static int GetLocalPoint_Port(TcpListener tcpListener)
        {
            IPEndPoint localEndPoint = (IPEndPoint)tcpListener.LocalEndpoint;
            return localEndPoint.Port;
        }
        #endregion 获取TcpListener对象的本地终结点
        #region 获取Socket对象的本地终结点
        /// <summary>获取Socket对象的本地终结点</summary>
        /// <param name="socket">Socket对象</param>
        public static IPEndPoint GetLocalPoint(Socket socket)
        {
            return (IPEndPoint)socket.LocalEndPoint;
        }
        /// <summary>获取Socket对象的本地终结点的IP地址</summary>
        /// <param name="socket">Socket对象</param>
        public static string GetLocalPoint_IP(Socket socket)
        {
            IPEndPoint localEndPoint = (IPEndPoint)socket.LocalEndPoint;
            return localEndPoint.Address.ToString();
        }
        /// <summary>获取Socket对象的本地终结点的端口号</summary>
        /// <param name="socket">Socket对象</param>
        public static int GetLocalPoint_Port(Socket socket)
        {
            IPEndPoint localEndPoint = (IPEndPoint)socket.LocalEndPoint;
            return localEndPoint.Port;
        }
        #endregion 获取Socket对象的本地终结点
        #endregion 获取本地终结点
        #region 绑定终结点
        /// <summary>绑定终结点</summary>
        /// <param name="socket">Socket对象</param>
        /// <param name="endPoint">要绑定的终结点</param>
        public static void BindEndPoint(Socket socket, IPEndPoint endPoint)
        {
            if (!socket.IsBound)
            {
                socket.Bind(endPoint);
            }
        }
        /// <summary>绑定终结点</summary>
        /// <param name="socket">Socket对象</param>
        /// <param name="ip">服务器IP地址</param>
        /// <param name="port">服务器端口</param>
        public static void BindEndPoint(Socket socket, string ip, int port)
        {
            //创建终结点
            IPEndPoint endPoint = CreateIPEndPoint(ip, port);
            //绑定终结点
            if (!socket.IsBound)
            {
                socket.Bind(endPoint);
            }
        }
        #endregion 绑定终结点
        #region 指定Socket对象执行监听
        /// <summary>指定Socket对象执行监听，默认允许的最大挂起连接数为100</summary>
        /// <param name="socket">执行监听的Socket对象</param>
        /// <param name="port">监听的端口号</param>
        public static void StartListen(Socket socket, int port)
        {
            //创建本地终结点
            IPEndPoint localPoint = CreateIPEndPoint(NetHelper.LocalHostName, port);
            //绑定到本地终结点
            BindEndPoint(socket, localPoint);
            //开始监听
            socket.Listen(100);
        }
        /// <summary>指定Socket对象执行监听</summary>
        /// <param name="socket">执行监听的Socket对象</param>
        /// <param name="port">监听的端口号</param>
        /// <param name="maxConnection">允许的最大挂起连接数</param>
        public static void StartListen(Socket socket, int port, int maxConnection)
        {
            //创建本地终结点
            IPEndPoint localPoint = CreateIPEndPoint(NetHelper.LocalHostName, port);
            //绑定到本地终结点
            BindEndPoint(socket, localPoint);
            //开始监听
            socket.Listen(maxConnection);
        }
        /// <summary>指定Socket对象执行监听</summary>
        /// <param name="socket">执行监听的Socket对象</param>
        /// <param name="ip">监听的IP地址</param>
        /// <param name="port">监听的端口号</param>
        /// <param name="maxConnection">允许的最大挂起连接数</param>
        public static void StartListen(Socket socket, string ip, int port, int maxConnection)
        {
            //绑定到本地终结点
            BindEndPoint(socket, ip, port);
            //开始监听
            socket.Listen(maxConnection);
        }
        #endregion 指定Socket对象执行监听
        #region 连接到基于TCP协议的服务器
        /// <summary>连接到基于TCP协议的服务器,连接成功返回true，否则返回false</summary>
        /// <param name="socket">Socket对象</param>
        /// <param name="ip">服务器IP地址</param>
        /// <param name="port">服务器端口号</param>
        public static bool Connect(Socket socket, string ip, int port)
        {
            try
            {
                //连接服务器
                socket.Connect(ip, port);
                //检测连接状态
                return socket.Poll(-1, SelectMode.SelectWrite);
            }
            catch (SocketException ex)
            {
                throw new Exception(ex.Message);
                //LogHelper.WriteTraceLog(TraceLogLevel.Error, ex.Message);
            }
        }
        #endregion 连接到基于TCP协议的服务器
        #region 以同步方式发送消息
        /// <summary>以同步方式向指定的Socket对象发送消息</summary>
        /// <param name="socket">socket对象</param>
        /// <param name="msg">发送的消息</param>
        public static void SendMsg(Socket socket, byte[] msg)
        {
            //发送消息
            socket.Send(msg, msg.Length, SocketFlags.None);
        }
        /// <summary>使用UTF8编码格式以同步方式向指定的Socket对象发送消息</summary>
        /// <param name="socket">socket对象</param>
        /// <param name="msg">发送的消息</param>
        public static void SendMsg(Socket socket, string msg)
        {
            //将字符串消息转换成字符数组
            byte[] buffer = ConvertHelper.StringToBytes(msg, Encoding.Default);
            //发送消息
            socket.Send(buffer, buffer.Length, SocketFlags.None);
        }
        #endregion 以同步方式发送消息
        #region 以同步方式接收消息
        /// <summary>以同步方式接收消息</summary>
        /// <param name="socket">socket对象</param>
        /// <param name="buffer">接收消息的缓冲区</param>
        public static void ReceiveMsg(Socket socket, byte[] buffer)
        {
            socket.Receive(buffer);
        }
        /// <summary>以同步方式接收消息，并转换为UTF8编码格式的字符串,使用5000字节的默认缓冲区接收。</summary>
        /// <param name="socket">socket对象</param>
        public static string ReceiveMsg(Socket socket)
        {
            //定义接收缓冲区
            byte[] buffer = new byte[5000];
            //接收数据，获取接收到的字节数
            int receiveCount = socket.Receive(buffer);
            //定义临时缓冲区
            byte[] tempBuffer = new byte[receiveCount];
            //将接收到的数据写入临时缓冲区
            Buffer.BlockCopy(buffer, 0, tempBuffer, 0, receiveCount);
            //转换成字符串，并将其返回
            return ConvertHelper.BytesToString(tempBuffer, Encoding.Default);
        }
        #endregion 以同步方式接收消息
        #region 关闭基于Tcp协议的Socket对象
        /// <summary>关闭基于Tcp协议的Socket对象</summary>
        /// <param name="socket">要关闭的Socket对象</param>
        public static void Close(Socket socket)
        {
            try
            {
                //禁止Socket对象接收和发送数据
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException ex)
            {
                throw ex;
            }
            finally
            {
                //关闭Socket对象
                socket.Close();
            }
        }
        #endregion 关闭基于Tcp协议的Socket对象
        #region 获得用户IP
        /// <summary>获得用户IP</summary>
        public static string GetUserIp()
        {
            string ip;
            string[] temp;
            bool isErr = false;
            if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_ForWARDED_For"] == null)
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            else
                ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_ForWARDED_For"].ToString();
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
        /*
   * C#完整的通信代码（点对点，点对多，同步，异步，UDP，TCP）
   * http://topic.csdn.net/u/20080619/08/dcef3fe2-f95b-4918-8edb-36d48a3d0528_2.html
   *
   */
        /// <summary>
        /// 获取IP地址 返回第一个
        /// </summary>
        /// <returns></returns>
        public static string getIP_This()
        {
            IPHostEntry hostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] address = hostInfo.AddressList;
            if (address.Length == 0)
                return "";
            else
                return address[0].ToString();
        }
        /// <summary>
        /// ping IP地址 timeout 局域网用200,广域网用2000
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="timeout">超时 毫秒</param>
        /// <returns></returns>
        public static bool ping(string ip, int timeout)
        {
            IPAddress ipadd;
            if (!IPAddress.TryParse(ip, out ipadd))
            {
                return false;
            }
            Ping pingSender = new Ping();
            PingReply reply = pingSender.Send(ip, timeout, new Byte[] { Convert.ToByte(1) });
            if (reply.Status == IPStatus.Success)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 判读是否是IP地址
        /// </summary>
        /// <param name="in_str"></param>
        /// <returns></returns>
        public static bool IsIPStr(string in_str)
        {
            if (in_str.Replace(".", "").Length != in_str.Length - 3)
                return false;
            try
            {
                IPAddress ip = IPAddress.Parse(in_str);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}