using System.Security.Cryptography;
using System.Text;
namespace System.NJCT.TYY.MyWindows.MyNet
{
    /// <summary>
    /// 传输数据操作类
    /// 加密，解密及封包解包方法
    /// </summary>
    public static class TcpOperation
    {
        #region 字段
        public static CspParameters cspParams = new CspParameters();//创建容器
        public static RSACryptoServiceProvider provider;//密钥对
        private static bool IsInit = false;//是否进行过初始化
        #endregion 字段
        #region 数据包完整性方法
        /// <summary>
        /// 发送数据封装
        /// </summary>
        /// <param name="data">需要发送的数据</param>
        /// <returns>封装后的数据</returns>
        public static byte[] SendDataPackage(byte[] data)
        {
            string Head = "httptyy://";//数据头
            string End = "//:tyyend";//数据尾
            int len = data.Length;
            len += 23;
            byte[] DataHead = Encoding.ASCII.GetBytes(Head);//头部数据
            byte[] DataCount = new byte[4];//四个字节的数据长度
            DataCount = BitConverter.GetBytes(data.Length);//数据长度
            byte[] DataEnd = Encoding.ASCII.GetBytes(End);//数据尾部
            //定义结果字节
            byte[] DataResult = new byte[len];
            //复制偏移量
            int off = 0;
            //封装数据
            for (int i = 0; i < DataHead.Length; i++)//数据头值复制
            {
                DataResult[off] = DataHead[i];//逐个复制
                off++;
            }
            for (int i = 0; i < DataCount.Length; i++)
            {
                DataResult[off] = DataCount[i];//逐个复制
                off++;
            }
            for (int i = 0; i < data.Length; i++)//数据区复制
            {
                DataResult[off] = data[i];//逐个复制
                off++;
            }
            for (int i = 0; i < DataEnd.Length; i++)//数据结尾验证码
            {
                DataResult[off] = DataEnd[i];//逐个复制
                off++;
            }
            ////加密数据
            //DataResult = DataEncryption(DataResult);
            //返回
            return DataResult;
        }
        /// <summary>
        /// 搜索数据头返回数据头索引值
        /// 去除数据报头，去除后去除数据长度解包
        /// </summary>
        /// <param name="data">收到的数据</param>
        /// <returns></returns>
        public static int Seachbyte(byte[] data)
        {
            int i = 0;
            string str = "httptyy://";//数据头
            byte[] DataHead = Encoding.ASCII.GetBytes(str);//字节型数据头
            for (int j = 0; i < data.Length; j++)
            {
                if (data[j] == DataHead[0])
                {
                    for (int s = 1; s < 10; s++)
                    {
                        if (data[s + j] != DataHead[s])
                        {
                            continue;
                        }
                        i = j;
                        if (s == 9)//循环完毕
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }
        #endregion 数据包完整性方法
        #region 加解密方法
        ///// <summary>
        ///// 加密方法
        ///// RSA公钥加密
        ///// </summary>
        ///// <param name="Encryption">需要加密的数据</param>
        ///// <returns></returns>
        //public static byte[] DataEncryption(byte[] Encryption)
        //{
        //    if (!IsInit) //没有进行过初始化时先初始化
        //    {
        //        //创建容器
        //        string password = "hp)(*&^%$#@!qaz.123";
        //        cspParams.KeyContainerName = "TyyDataEncryptionKey";
        //        cspParams.ProviderType = 1;//PROV_RSA_FULL
        //        //System.Security.SecureString seStr = new System.Security.SecureString();//定义密码
        //        //foreach (char c in password)//循环添加密码
        //        //{
        //        //    seStr.AppendChar(c);
        //        //}
        //        //cspParams.KeyPassword = seStr;//密码
        //        //创建密钥对，并添加到容器中
        //        provider = new RSACryptoServiceProvider(cspParams);
        //    }
        //    byte[] bEncrypt = new byte[Encryption.Length];//加密后字节组
        //    //公钥加密
        //    RSACryptoServiceProvider CPSEncryption = new RSACryptoServiceProvider(cspParams);
        //    bEncrypt = CPSEncryption.Encrypt(Encryption, true);
        //    return bEncrypt;
        //}
        ///// <summary>
        ///// 解密方法
        ///// 私钥解密
        ///// </summary>
        ///// <param name="Decrypt">需要解密的数据</param>
        ///// <returns></returns>
        //public static byte[] DataDecrypt(byte[] Decrypt)
        //{
        //    byte[] bDecrypt;//解密后字节组
        //    //私钥解密
        //    RSACryptoServiceProvider CPSDeEncryption = new RSACryptoServiceProvider(cspParams);
        //    bDecrypt = CPSDeEncryption.Decrypt(Decrypt, true);
        //    return bDecrypt;
        //}
        #endregion 加解密方法
    }
}