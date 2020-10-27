using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace System.NJCT.TYY.MyWindows.MyNet
{
    public partial class TcpClient : Component
    {
        #region 控件属性
        private bool _Connected = false;//是否连接
        /// <summary>
        /// 客户端是否处于连接中
        /// </summary>
        [Browsable(false)]
        public bool Connected
        {
            get { return _Connected; }
            set { _Connected = value; }
        }
        private string _ConnectIp = "127.0.0.1";//连接的IP地址
        /// <summary>
        /// 远程连接的IP地址
        /// </summary>
        [Browsable(true), Category("ConnectIp"), Description("连接的远程IP地址")]
        public string ConnectIp
        {
            get { return _ConnectIp; }
            set { _ConnectIp = value; }
        }
        private int _Prot = 5555;//远程端口号
        /// <summary>
        /// 远程端口号
        /// </summary>
        [Browsable(true), Category("Prot"), Description("远程端口号")]
        public int Prot
        {
            get { return _Prot; }
            set { _Prot = value; }
        }
        private int _BuffCount = 100;//缓冲区大小以M为单位
        /// <summary>
        /// 缓冲区大小以M为单位
        /// </summary>
        [Browsable(true), Category("BuffCount"), Description("缓冲区大小以M为单位")]
        public int BuffCount
        {
            get { return _BuffCount; }
            set
            {
                if (value < 500)
                {
                    _BuffCount = value;
                }
                else
                {
                    _BuffCount = 500;
                }
            }
        }
        #endregion 控件属性
        #region 控件事件
        /// <summary>
        /// 文本到达托管委托
        /// </summary>
        public delegate void TextArrivedEventHandler(string msg);
        /// <summary>
        /// 文本到达时触发
        /// </summary>
        [Browsable(true), Category("TextArrived"), Description("文本到达时触发")]
        public event TextArrivedEventHandler TextArrived;//文本到达时触发
        /// <summary>
        ///   命令到达时触发委托
        /// </summary>
        /// <param name="cmd">到达的命令</param>
        /// <param name="data">与命令相关联的数据</param>
        public delegate void CommandArrivalEventHandler(MsgCommand cmd, byte[] data);//命令到达时触发委托
        /// <summary>
        /// 命令到达时触发
        /// </summary>
        [Browsable(true), Category("CommandArrival"), Description("命令到达时触发")]
        public event CommandArrivalEventHandler CommandArrival;
        /// <summary>
        ///   命令到达时触发委托
        /// </summary>
        /// <param name="obj">到达的对象</param>
        public delegate void ObjectArrivalEventHandler(object obj);
        /// <summary>
        /// 对象到达时触发
        /// </summary>
        [Browsable(true), Category("ObjectArrival"), Description("对象到达时触发")]
        public event ObjectArrivalEventHandler ObjectArrival;
        /// <summary>
        /// 连接到服务器托管委托
        /// </summary>
        /// <param name="ip">服务器IP地址</param>
        /// <param name="prot">服务器端口号</param>
        public delegate void ConnectedEventHandler(IPAddress ip, int prot);
        /// <summary>
        /// 连接到远程服务器时触发
        /// </summary>
        [Browsable(true), Category("Connected"), Description("连接到远程服务器时触发")]
        public event ConnectedEventHandler ServerConnected;//连接到远程服务器时触发
        /// <summary>
        /// 断开服务器托管委托
        /// </summary>
        /// <param name="ip">服务器IP地址</param>
        /// <param name="prot">服务器端口号</param>
        public delegate void DisConnectedEventHandler(IPAddress ip, int prot);//与服务器断开连接时触发
        /// <summary>
        /// 与服务器断开连接时触发
        /// </summary>
        [Browsable(true), Category("DisConnected"), Description("与服务器断开连接时触发")]
        public event DisConnectedEventHandler DisConnected;//与服务器断开连接时触发
        /// <summary>
        /// 发送消息时托管委托
        /// </summary>
        /// <param name="msg">发送的消息</param>
        public delegate void SendMsgingEventHandler(ClassMsg msg);
        /// <summary>
        /// 发送消息时触发
        /// </summary>
        [Browsable(true), Category("SendMsging"), Description("发送消息时触发")]
        public event SendMsgingEventHandler SendMsging;
        /// <summary>
        ///  收到小文件时托管委托
        /// </summary>
        /// <param name="file"></param>
        public delegate void SmallFileArrivalEventHandler(SendFileInfo file);
        /// <summary>
        /// 收到小文件时触发
        /// </summary>
        [Browsable(true), Category("SmallFileArrival"), Description("收到小文件时触发")]
        public event SmallFileArrivalEventHandler SmallFileArrival;
        #endregion 控件事件
        #region 客户端Socket
        private Socket ClientSocket;//客户端网络连接
        #endregion 客户端Socket
        #region 客户端线程
        private Thread ThreadClientReceive;//客户端接收聊天信息线程
        private Thread ThreadServerReceive;//服务器端接收聊天信息线程
        #endregion 客户端线程
        #region 线程方法
        /// <summary>
        /// 客户端接受信息处理
        /// </summary>
        private void ClientReceive()
        {
            byte[] DataBuff;
            while (true)
            {
                try
                {
                    DataBuff = new byte[1024 * _BuffCount];
                    //接收数据长度
                    ClientSocket.Receive(DataBuff, 0, DataBuff.Length, SocketFlags.None);
                    //对数据包进行解包
                    int index = -1;
                    index = TcpOperation.Seachbyte(DataBuff);
                    int i = index;
                    if (i == -1)
                    {
                        throw new Exception("数据接收不完整!");
                    }
                    //取数据长度
                    i += 10;
                    int len = BitConverter.ToInt32(DataBuff, i);
                    i += 4;
                    //判断数据是否完整
                    string end = Encoding.ASCII.GetString(DataBuff, i + len, 9);
                    if (end.IndexOf("//:tyyend") < 0)
                    {
                        throw new Exception("数据接收不完全");
                    }
                    else
                    {
                        IPEndPoint iep = ClientSocket.LocalEndPoint as IPEndPoint;
                        ClassMsg msg = MySerializers.DeSerializeBinary(new MemoryStream(DataBuff, index + 14, len)) as ClassMsg;
                        if (msg != null)//检测消息
                        {
                            if (msg.sendKind == SendKind.SendCommand)
                            {
                                if (CommandArrival != null)//事件处理函数
                                {
                                    CommandArrival(msg.msgCommand, msg.Data);
                                }
                            }
                            else if (msg.sendKind == SendKind.SendMsg)
                            {
                                if (TextArrived != null)
                                {
                                    TextArrived(Encoding.Unicode.GetString(msg.Data));
                                }
                            }
                            else if (msg.sendKind == SendKind.SendObject)
                            {
                                if (ObjectArrival != null)
                                {
                                    ObjectArrival(MySerializers.DeSerializeBinary(new MemoryStream(msg.Data)));
                                }
                            }
                            else if (msg.sendKind == SendKind.SendSamllFile)
                            {
                                SendFileInfo file = MySerializers.DeSerializeBinary(new MemoryStream(msg.Data)) as SendFileInfo;
                                if (SmallFileArrival != null)
                                {
                                    SmallFileArrival(file);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion 线程方法
        #region 自定义方法
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg">需要发送的消息</param>
        private void SendData(ClassMsg msg)
        {
            if (SendMsging != null)
            {
                SendMsging(msg);
            }
            byte[] Data = MySerializers.SerializeBinary(msg).ToArray();//转换为byte
            byte[] Send = TcpOperation.SendDataPackage(Data);//加数据头尾
            ClientSocket.Send(Send);//发送
        }
        /// <summary>
        /// 连接服务器
        /// </summary>
        public void ConnectServer()
        {
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(_ConnectIp), _Prot);//建立客户端网络端点
            ClientSocket.Connect(remoteEP);
            ThreadClientReceive = new Thread(new ThreadStart(ClientReceive));
            ThreadClientReceive.IsBackground = true;
            ThreadClientReceive.Start();//客户端开始接收信息
        }
        #endregion 自定义方法
        #region 发送方法
        /// <summary>
        /// 发送对象
        /// </summary>
        /// <param name="SendData">需要发送的对象</param>
        public void SendObject(object obj)
        {
            ClassMsg msg = new ClassMsg();
            msg.Data = MySerializers.SerializeBinary(obj).ToArray();
            msg.sendKind = SendKind.SendObject;
            SendData(msg);
        }
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="msg">需要发送的文本信息</param>
        public void SendMsg(string txtMsg)
        {
            ClassMsg msg = new ClassMsg();
            msg.Data = Encoding.Unicode.GetBytes(txtMsg);
            msg.sendKind = SendKind.SendMsg;
            SendData(msg);
        }
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="obj">与命令关联的数据</param>
        /// <param name="cmd">需要发送的命令</param>
        public void SendCommand(object obj, MsgCommand cmd)
        {
            ClassMsg msg = new ClassMsg();
            msg.Data = MySerializers.SerializeBinary(obj).ToArray();
            msg.sendKind = SendKind.SendCommand;
            msg.msgCommand = cmd;
            SendData(msg);
        }
        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="FilesName">需要发送的文件路径（可以为相对或绝对）</param>
        public void SendSmallFiles(string FilesName)
        {
            SendFileInfo file = new SendFileInfo(FilesName);
            if (file.FileSize > 20480)
            {
                throw new Exception("发送文件的大小不能大于20M！");
            }
            ClassMsg msg = new ClassMsg();
            msg.Data = MySerializers.SerializeBinary(file).ToArray();
            msg.sendKind = SendKind.SendSamllFile;
            SendData(msg);
        }
        /// <summary>
        /// 发送文件返回进度（没有大小限制）
        /// </summary>
        /// <param name="FileName">需要发送的文件路径</param>
        /// <param name="Schedule">当前发送进度</param>
        public void SendFile(string FileName, ref int Schedule)
        {
        }
        #endregion 发送方法
        public TcpClient()
        {
            InitializeComponent();
        }
        public TcpClient(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }
    }
}