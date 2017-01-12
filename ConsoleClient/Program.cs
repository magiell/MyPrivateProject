using Server.CommonLib.MglServerConfig;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient
{
    class Program
    {
        private static AsyncTcpSession Session { get; set; }
        static void Main(string[] args)
        {
            Console.WriteLine("Connect to Server");
            Console.Write("ID :");
            var user_id = Console.ReadLine();
            var serverendpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"),
                8000);

            Session = new AsyncTcpSession();

            Session.Connected += Session_Connected;
            Session.Closed += Session_Closed;
            Session.DataReceived += Session_DataReceived;
            Session.Error += Session_Error;

            Session.Connect(serverendpoint);
            SendMsg(ProtocolKey.ECHO, user_id);

            bool quitflag = false;
            while (!quitflag)
            {
                var msg = Console.ReadLine();
                if (msg.ToLower().Equals("quit"))
                {
                    quitflag = true;
                }
                else
                {
                    SendMsg(ProtocolKey.MSG, msg);
                }
            }
            Session.Close();
        }

        private static void SendMsg(ProtocolKey key, string v)
        {
            if (!string.IsNullOrEmpty(v))
            {
                var body = Encoding.Unicode.GetBytes(v);
                var dataSource = new List<byte>();
                dataSource.AddRange(BitConverter.GetBytes((UInt32)key));
                dataSource.AddRange(BitConverter.GetBytes((UInt32)1));
                dataSource.AddRange(BitConverter.GetBytes((UInt32)1));
                dataSource.AddRange(BitConverter.GetBytes((UInt32)body.Length));
                dataSource.AddRange(body);
                Session.Send(dataSource.ToArray(), 0, dataSource.Count);
            }
        }

        private static void Session_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("세션 에러 이벤트 발생");
        }

        private static void Session_DataReceived(object sender, DataEventArgs e)
        {
            if(true == Session.IsConnected)
            {
                var data = new byte[e.Length];
                Buffer.BlockCopy(e.Data, e.Offset, data, 0, e.Length);
                var length = BitConverter.ToInt32(data, 12);
                Console.WriteLine("확인 : " + Encoding.Unicode.GetString(data, 16, length));

                var key = BitConverter.ToUInt32(data, 0);
                var value1 = BitConverter.ToUInt32(data, 4);
                var value2 = BitConverter.ToUInt32(data, 8);                
                var text = Encoding.Unicode.GetString(data, 16, length);

                Console.WriteLine("key : {0}, value1 : {1}, value2 : {2}, text : {3}", key, value1, value2, text);
            }
            Console.WriteLine("세션 데이터 받기 이벤트 발생");
        }

        private static void Session_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("세션 종료 이벤트 발생");

            Session.Close();
        }

        private static void Session_Connected(object sender, EventArgs e)
        {
            Console.WriteLine("세션 연결 이벤트 발생");
            if(true == Session.IsConnected)
            {
                //SendMsg(ProtocolKey.ECHO, "user_id");
            }       
        }
    }
}
