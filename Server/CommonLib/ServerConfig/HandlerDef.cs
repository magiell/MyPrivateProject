using Server.CommonLib.MglServerConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.CommonLib.MglServerConfig
{
    public class HandlerDef
    {
        /// <summary>
        /// sending Packet
        /// </summary>
        /// <param name="session"></param>
        /// <param name="requestInfo"></param>
        /// <param name="protocolNo"></param>
        public static void SendingData(Session session, MglBinaryRequestInfo requestInfo, ProtocolKey protocolNo)
        {
            List<byte> dataSource = new List<byte>();
            dataSource.AddRange(BitConverter.GetBytes((uint)protocolNo));
            dataSource.AddRange(BitConverter.GetBytes(session.data.ServerNo));
            dataSource.AddRange(BitConverter.GetBytes(session.data.ChannelNo));
            dataSource.AddRange(BitConverter.GetBytes((uint)requestInfo.Body.Length));
            dataSource.AddRange(requestInfo.Body);
            // command, length, body 형식
            session.Send(dataSource.ToArray(), 0, dataSource.Count);
        }

        public void Login(Session session, MglBinaryRequestInfo requestInfo)
        {
            if(session.AppServer.GetSessions(x=>x.user_id == session.user_id).Count() > 1)
            {
                //중복 아이디
            }
            else
            {
                try
                {
                    if (string.IsNullOrEmpty(session.user_id))
                    {
                        session.user_id = Encoding.Unicode.GetString(requestInfo.Body);
                        session.data = new SessionData() { ServerNo = requestInfo.value1, ChannelNo = requestInfo.value2, PrevProtocolStackHistory = new List<uint>() { (uint)ProtocolKey.Login } };
                    }
                    SendingData(session, , ProtocolKey.Login_Success);
                }
                catch (Exception e)
                {
                    SendingData(session, , ProtocolKey.Login_Fail);
                }
                //아이디 등록 및 허용
                

                
            }
        }
        public void Echo(Session session, MglBinaryRequestInfo requestInfo)
        {
            Console.WriteLine("echo");
            List<byte> dataSource = new List<byte>();
            dataSource.AddRange(BitConverter.GetBytes((uint)ProtocolKey.ECHO));
            dataSource.AddRange(BitConverter.GetBytes((uint)1));
            dataSource.AddRange(BitConverter.GetBytes((uint)1));
            dataSource.AddRange(BitConverter.GetBytes((uint)requestInfo.Body.Length));
            dataSource.AddRange(requestInfo.Body);
            // command, length, body 형식
            session.Send(dataSource.ToArray(), 0, dataSource.Count);
        }

        public void SendMsg(Session session, MglBinaryRequestInfo requestInfo)
        {            
            Console.WriteLine("SendMsg");
            var sessionlist = session.AppServer.GetSessions(x => x.data.ChannelNo == session.data.ChannelNo);
            List<byte> dataSource = new List<byte>();
            dataSource.AddRange(BitConverter.GetBytes((uint)ProtocolKey.MSG));
            dataSource.AddRange(BitConverter.GetBytes((uint)session.data.ServerNo));
            dataSource.AddRange(BitConverter.GetBytes((uint)session.data.ChannelNo));
            dataSource.AddRange(BitConverter.GetBytes((uint)requestInfo.Body.Length));
            dataSource.AddRange(requestInfo.Body);

            //나도 받고
            Echo(session, requestInfo);
            //너도 받고
            foreach (var rcvsession in sessionlist)
            {
                rcvsession.Send(dataSource.ToArray(), 0, dataSource.Count);
            }
            
        }
    }
}
