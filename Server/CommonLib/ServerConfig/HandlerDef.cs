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
