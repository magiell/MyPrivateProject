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
            dataSource.AddRange(BitConverter.GetBytes((int)ProtocolKey.ECHO));
            dataSource.AddRange(BitConverter.GetBytes((short)0));
            dataSource.AddRange(BitConverter.GetBytes((short)1));
            dataSource.AddRange(BitConverter.GetBytes(requestInfo.Body.Length));
            dataSource.AddRange(requestInfo.Body);
            // command, length, body 형식
            session.Send(dataSource.ToArray(), 0, dataSource.Count);
        }
    }
}
