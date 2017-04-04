using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace EmailCheck
{
    public  class EmailCheckHelper
    {
        TcpClient tcpc;
        NetworkStream s;
        string strDomain;
        byte[] bb;
        int len;
        string read;
        string stringTosend;
        byte[] arrayToSend;
        private string fromEmail = "check@verify-email.org";
        private StringBuilder sbMessage = new StringBuilder(40);

        public string SbMessage
        {
            get { return sbMessage.ToString(); }
            //set { sbMessage = value; }
        }
        public EmailCheckHelper()
        { }
        public EmailCheckHelper(string email)
        {
            this.fromEmail = email;
        }
        private string getMailServer(string strEmail)
        {
            strDomain = strEmail.Split('@')[1];
            ProcessStartInfo info = new ProcessStartInfo();   //指定启动进程时使用的一组值。  
            info.UseShellExecute = false;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.FileName = "nslookup";
            info.CreateNoWindow = true;
            info.Arguments = "-type=mx " + strDomain;
            Process ns = Process.Start(info);        //提供对本地和远程进程的访问并使您能够启动和停止本地系统进程。  
            StreamReader sout = ns.StandardOutput;
            Regex reg = new Regex(@"mail exchanger = (?<mailServer>[^\s]+)");
            string strResponse = "";
            while ((strResponse = sout.ReadLine()) != null)
            {

                Match amatch = reg.Match(strResponse);   // Match  表示单个正则表达式匹配的结果。  

                if (reg.Match(strResponse).Success)
                {
                    return amatch.Groups["mailServer"].Value;   //获取由正则表达式匹配的组的集合  

                }
            }
            return null;
        }

        private bool Connect(string mailServer)
        {
            bool isResult=true;
            try
            {
                tcpc.Connect(mailServer, 25);
                s = tcpc.GetStream();
                len = s.Read(bb, 0, bb.Length);
                read = Encoding.UTF8.GetString(bb);
                if (read.StartsWith("220") == true)
                {
                    this.sbMessage.Append("连接服务器成功！" + "\r\n");
                }
            }
            catch (Exception e)
            {
                isResult=false;
                this.sbMessage.Append("连接服务器失败！" + "\r\n");
            }
            return isResult;
        } 

        private bool SendCommand(string command)
        {
            try
            {
                arrayToSend = Encoding.UTF8.GetBytes(command.ToCharArray());
                s.Write(arrayToSend, 0, arrayToSend.Length);
                len = s.Read(bb, 0, bb.Length);
                read = Encoding.UTF8.GetString(bb);
                this.sbMessage.Append( "收到：" + read.Substring(0, len) + "\r\n");
            }
            catch (IOException e)
            {
                this.sbMessage.Append("error：" + e.Message + "\r\n");
            }
            if (read.StartsWith("250"))
            {
                return true;
            }
            return false;
        }

        public bool checkEmail(string mailAddress)
        {
            this.sbMessage.Clear();
            Regex reg = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            if (!reg.IsMatch(mailAddress))
            {
                //Email地址形式上就不对 
                this.sbMessage.Append("邮件地址格式存在问题！\r\n");
                return false;
            } 
            string mailServer = getMailServer(mailAddress);
            if (string.IsNullOrEmpty(mailServer))
            {
                this.sbMessage.Append("邮件服务器不存在！\r\n");
                return false;
                //邮件服务器探测错误  
            }
            this.sbMessage.Append("解析出域名" + strDomain + "的mx记录：" + mailServer + "\r\n");
            tcpc = new TcpClient();      //为 TCP 网络服务提供客户端连接。  
            tcpc.NoDelay = true;
            tcpc.ReceiveTimeout = 3000;
            tcpc.SendTimeout = 3000;
            bb = new byte[512];
            try
            {
                //创建连接 
                if (!Connect(mailServer))
                {
                    return false;
                }
               //写入HELO命令  
               stringTosend = "helo " + mailServer + "\r\n";
               this.sbMessage.Append( "发送：" + stringTosend);
               if (!SendCommand(stringTosend)) 
               {
                   return false;
               }
               //写入Mail From命令  
                stringTosend = "mail from:<" + fromEmail + ">" + "\r\n";
                this.sbMessage.Append( "发送：" + stringTosend);
                if (!SendCommand(stringTosend)) {
                    return false;
                }
                //写入RCPT命令，这是关键的一步，后面的参数便是查询的Email的地址  
                stringTosend = "rcpt to:<" + mailAddress + ">" + "\r\n";
                this.sbMessage.Append( "发送：" + stringTosend);
                return SendCommand(stringTosend);
            }
            catch (Exception ee)
            {
                this.sbMessage.Append(ee.Message+"\r\n");
            }
            return false;
        }

        public override string ToString()
        {
             return this.SbMessage;
        }
    }


}
