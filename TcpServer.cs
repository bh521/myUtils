 
namespace System.NJCT.TYY.MyWindows.MyNet
{
    public partial class TcpServer : Component
    {
        #region 服务器Socket
        private Socket SocketListener;//网络连接侦听
        private Socket ServerSocket;//服务器网络连接
        #endregion 服务器Socket
        #region 组件属性
        private bool active = false;//是否启动监听
       
        /// 是否启动监听
        
        [Browsable(true), Category("Active"), Description("是否启动监听")]
        public bool Active
        {
            get { return active; }
        }
        private bool anyIpCanConnect = true;//是否所有IP可以连接
       
        /// 是否所有IP可以连接
        
        [Browsable(true), Category("AnyIpCanConnect"), Description("是否所有IP可以连接")]
        public bool AnyIpCanConnect
        {
            get { return anyIpCanConnect; }
            set { anyIpCanConnect = value; }
        }
        private int prot = 7777;//端口号
       
        /// 端口号
        
        [Browsable(true), Category("Prot"), Description("端口号")]
        public int Prot
        {
            get { return prot; }
            set { prot = value; }
        }
        private string connectip = "";//如果不是全部可以连接则设置可以连接的IP地址
       
        /// 如果不是全部可以连接则设置可以连接的IP地址
        
        [Browsable(true), Category("ConnectIp"), Description("如果不是全部可以连接则设置可以连接的IP地址")]
        public string ConnectIp
        {
            get { return connectip; }
            set { connectip = value; }
        }
        private int clientcount = 5;//允许连接的最大客户端数量
       
        /// 允许连接的最大客户端数量
        
        [Browsable(true), Category("MaxClientCount"), Description("允许连接的最大客户端数量")]
        public int ClientCount
        {
            get { return clientcount; }
            set { clientcount = value; }
        }
        private int _BuffCount = 100;//接收数据缓冲区大小，以M为单位
       
        /// 接收数据缓冲区大小，以M为单位
        
        [Browsable(true), Category("BuffCount"), Description("接收数据缓冲区大小，以M为单位")]
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
                    _BuffCount = 500;//不超过500M
                }
            }
        }
        #endregion 组件属性
        #region 组件事件
       
        /// 数据到达事件委托
        
        /// <param name="Msg">发送到的消息</param>
        public delegate void MsgArrivalEventHandler(string Msg);//文本信息到达事件委托
       
        /// 文本到达时触发
        
        [Browsable(true), Category("TextArrival"), Description("文本到达时触发")]
        public event MsgArrivalEventHandler TextArrival;//通过托管委托定义事件
       
        ///   命令到达时触发委托
        
        /// <param name="cmd">到达的命令</param>
        /// <param name="data">与命令相关联的数据</param>
        public delegate void CommandArrivalEventHandler(MsgCommand cmd, byte[] data);//命令到达时触发委托
       
        /// 命令到达时触发
        
        [Browsable(true), Category("CommandArrival"), Description("命令到达时触发")]
        public event CommandArrivalEventHandler CommandArrival;
       
        ///   命令到达时触发委托
        
        /// <param name="obj">到达的对象</param>
        public delegate void ObjectArrivalEventHandler(object obj);
       
        /// 对象到达时触发
        
        [Browsable(true), Category("ObjectArrival"), Description("对象到达时触发")]
        public event ObjectArrivalEventHandler ObjectArrival;
       
        /// 客户连接托管委托
        
        /// <param name="Ip">客户端IP</param>
        /// <param name="Prot">客户端服务器</param>
        public delegate void ClientConnectedEventHandler(IPAddress Ip, int Prot);//客户端连接事件委托
       
        /// 客户端连接时触发
        
        [Browsable(true), Category("ClientConnected"), Description("客户端连接时触发")]
        public event ClientConnectedEventHandler ClientConnected;//客户端连接事件
       
        /// 关闭套接字时的委托
        
        /// <param name="iep">正在连接中的客户端</param>
        public delegate void SocketCloseDisConnectedEventHandler(IPEndPoint iep);//客户离开事件委托
       
        /// 关闭套接字时触发
        
        [Browsable(true), Category("SocketClose"), Description("关闭套接字时触发")]
        public event SocketCloseDisConnectedEventHandler SocketClose;//客户断开连接时间
       
        ///  收到小文件时托管委托
        
        /// <param name="file"></param>
        public delegate void SmallFileArrivalEventHandler(SendFileInfo file);
       
        /// 收到小文件时触发
        
        [Browsable(true), Category("SmallFileArrival"), Description("收到小文件时触发")]
        public event SmallFileArrivalEventHandler SmallFileArrival;
        #endregion 组件事件
        #region 服务器线程
        private Thread ThreadListener;//服务器端侦听线程
        private Thread ThreadServerReceive;//服务器端接收聊天信息线程
        #endregion 服务器线程
        #region 线程方法
       
        /// 结束侦听结束接收信息
        
        public void StopListen()
        {
            //关闭套接字
            if (ServerSocket != null)
            {
                ServerSocket.Close();
            }
            //终止线程
            if (ThreadListener != null)
            {
                if (ThreadListener.IsAlive)
                {
                    ThreadListener.Abort();
                }
            }
            //关闭侦听
            if (SocketListener != null)
            {
                SocketListener.Close();
            }
            active = false;
            if (SocketClose != null)
            {
                IPEndPoint iep = new IPEndPoint(IPAddress.Parse("1.1.1.1"), 0);
                SocketClose(iep);
            }
        }
       
        /// 开始侦听允许客户端连接
        
        private void StartListen()
        {
            try
            {
                if (!active)
                {
                    IPEndPoint IEP;
                    SocketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//定义接受连接套接字
                    if (anyIpCanConnect)
                    {
                        IEP = new IPEndPoint(IPAddress.Any, prot);//定义终端允许连接
                    }
                    else
                    {
                        IEP = new IPEndPoint(IPAddress.Parse(connectip), prot);//定义终端允许连接
                    }
                    SocketListener.Bind(IEP);//绑定端口
                    SocketListener.Listen(clientcount);
                    //while (true)
                    //{
                    Socket s = SocketListener.Accept();//开始侦听
                    IPEndPoint client = s.RemoteEndPoint as IPEndPoint;
                    list_client.Add(client);//客户连接加入集合
                    if (ClientConnected != null)
                    {
                        ClientConnected(client.Address, client.Port);
                    }
                    if (ServerSocket != null)
                    {
                        ServerSocket.Close();//每次只允许连接一个IP地址
                    }
                    ServerSocket = s;
                    ThreadServerReceive = new Thread(new ThreadStart(ServerReceive));
                    ThreadServerReceive.IsBackground = true;
                    ThreadServerReceive.Start();//服务器开始接收信息
                    //}
                }
                else
                {
                    StopListen();
                }
            }
            catch (Exception ex)
            {
                StopListen();
                throw ex;
            }
        }
       
        /// 服务器接受信息处理
        
        private void ServerReceive()
        {
            byte[] DataBuff;
            while (true)
            {
                try
                {
                    DataBuff = new byte[1024 * _BuffCount];
                    //接收数据长度
                    ServerSocket.Receive(DataBuff, 0, DataBuff.Length, SocketFlags.None);
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
                        IPEndPoint iep = ServerSocket.LocalEndPoint as IPEndPoint;
                        ClassMsg msg = MySerializers.DeSerializeBinary(new MemoryStream(DataBuff, index + 14, len)) as ClassMsg;
                        if (msg != null)//检测消息
                        {
                            if (msg.sendKind == SendKind.SendCommand)
                            {
                                if (CommandArrival != null)//命令到达时触发
                                {
                                    CommandArrival(msg.msgCommand, msg.Data);
                                }
                            }
                            else if (msg.sendKind == SendKind.SendMsg)
                            {
                                if (TextArrival != null)
                                {
                                    TextArrival(Encoding.Unicode.GetString(msg.Data));
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
        #region 类属性
        private List<IPEndPoint> list_client = new List<IPEndPoint>();//当前连接的客户端
       
        /// 当前连接的客户端集合
        
        [Browsable(false)]
        public List<IPEndPoint> List_Client
        {
            get { return list_client; }
            set { list_client = value; }
        }
        #endregion 类属性
        #region 自定义方法
       
        /// 发送消息
        
        /// <param name="msg">需要发送的消息</param>
        private void SendData(ClassMsg msg)
        {
            byte[] Data = MySerializers.SerializeBinary(msg).ToArray();//转换为byte
            byte[] Send = TcpOperation.SendDataPackage(Data);//加数据头尾
            ServerSocket.Send(Send);//发送
        }
       
        /// 开始监听
        
        public void Listen()
        {
            Thread th = new Thread(new ThreadStart(StartListen));
            th.Start();
        }
        #endregion 自定义方法
        #region 发送方法
       
        /// 发送对象
        
        /// <param name="SendData">需要发送的对象</param>
        public void SendObject(object obj)
        {
            ClassMsg msg = new ClassMsg();
            msg.Data = MySerializers.SerializeBinary(obj).ToArray();
            msg.sendKind = SendKind.SendObject;
            SendData(msg);
        }
       
        /// 发送信息
        
        /// <param name="msg">需要发送的文本信息</param>
        public void SendMsg(string txtMsg)
        {
            ClassMsg msg = new ClassMsg();
            msg.Data = Encoding.Unicode.GetBytes(txtMsg);
            msg.sendKind = SendKind.SendMsg;
            SendData(msg);
        }
       
        /// 发送命令
        
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
        //发送文件功能预留，日后完善 tyy 2012-1-23 17:07:06
       
        /// 发送文件 tyy add 2012-8-26 21:58:26
        
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
       
        /// 发送文件返回进度（没有大小限制）
        
        /// <param name="FileName">需要发送的文件路径</param>
        /// <param name="Schedule">当前发送进度</param>
        public void SendFile(string FileName, ref int Schedule)
        {
        }
        #endregion 发送方法
        public TcpServer()
        {
            InitializeComponent();
        }
        public TcpServer(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }
    }
}