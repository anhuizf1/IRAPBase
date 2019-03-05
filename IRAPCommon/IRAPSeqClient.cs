using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IRAPCommon
{

    public static class IRAPSeqClient
    {
        //新增队列
        public static int AddSequence(string hostIP, string sequenceName, long initValue)
        {
            byte[] data = new byte[128];
            Socket newclient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            newclient.SendTimeout = 10;
            string ipadd = (string)hostIP;

            int port = Convert.ToInt32("13000");
            IPEndPoint ie = new IPEndPoint(IPAddress.Parse(ipadd), port);//服务器的IP和端口
            string sendcommand;
            try
            {
                //因为客户端只是用来向特定的服务器发送信息，所以不需要绑定本机的IP和端口。不需要监听。
                newclient.Connect(ie);
                sendcommand = string.Format("add|{0}|{1}", sequenceName, initValue.ToString());
                newclient.Send(Encoding.ASCII.GetBytes(sendcommand));
                data = new byte[128];
                int recv = newclient.Receive(data);
                String TransactNostr = Encoding.ASCII.GetString(data, 0, recv);

                return int.Parse(TransactNostr);
            }
            catch (SocketException e)
            {
                // Console.WriteLine("unable to connect to server");
               Console.WriteLine(e.ToString());
                newclient.Close();
                return -1;
            }
            finally
            {

                newclient.Close();
            }

        }

        //重置序列
        public static int ResetSequence(string hostIP, string sequenceName, long startValue)
        {
            byte[] data = new byte[128];
            Socket newclient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            newclient.SendTimeout = 10;
            string ipadd = (string)hostIP;

            int port = Convert.ToInt32("13000");
            IPEndPoint ie = new IPEndPoint(IPAddress.Parse(ipadd), port);//服务器的IP和端口
            string sendcommand;
            try
            {
                //因为客户端只是用来向特定的服务器发送信息，所以不需要绑定本机的IP和端口。不需要监听。
                newclient.Connect(ie);
                sendcommand = string.Format("reset|{0}|{1}", sequenceName, startValue.ToString());
                newclient.Send(Encoding.ASCII.GetBytes(sendcommand));
                data = new byte[128];
                int recv = newclient.Receive(data);
                String TransactNostr = Encoding.ASCII.GetString(data, 0, recv);

                return int.Parse(TransactNostr);
            }
            catch (SocketException e)
            {
                // Console.WriteLine("unable to connect to server");
                // Console.WriteLine(e.ToString());
                newclient.Close();
                return -1;
            }
            finally
            {

                newclient.Close();
            }
        }

       //申请序列号
        public static long GetSequenceNo(string hostIP, string sequenceName, long count)
        {
            byte[] data = new byte[128];
            Socket newclient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            newclient.SendTimeout = 10;
            string ipadd = (string)hostIP;

            int port = Convert.ToInt32("13000");
            IPEndPoint ie = new IPEndPoint(IPAddress.Parse(ipadd), port);//服务器的IP和端口
            string sendcommand;
            try
            {
                //因为客户端只是用来向特定的服务器发送信息，所以不需要绑定本机的IP和端口。不需要监听。
                newclient.Connect(ie);
                sendcommand = string.Format("{0}|{1}", sequenceName, count.ToString());
                newclient.Send(Encoding.ASCII.GetBytes(sendcommand));
                data = new byte[128];
                int recv = newclient.Receive(data);
                String TransactNostr = Encoding.ASCII.GetString(data, 0, recv);
                // newclient.Shutdown(SocketShutdown.Both);

                return Int64.Parse(TransactNostr);
            }
            catch (SocketException e)
            {
                // Console.WriteLine("unable to connect to server");
                // Console.WriteLine(e.ToString());
                newclient.Close();
                throw e;
            }
            finally
            {

                newclient.Close();
            }

        }
    }
}
