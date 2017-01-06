using SuperSocket.SocketBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.CommonLib.MglServerConfig
{
    public class Session : AppSession<Session, MglBinaryRequestInfo>
    {
        public string user_id { get; set; }
        public SessionData data { get; set; }        
        protected override void OnSessionStarted()
        {
            base.OnSessionStarted();
        }

        protected override void HandleException(Exception e)
        {
            base.HandleException(e);
        }

        protected override void HandleUnknownRequest(MglBinaryRequestInfo requestInfo)
        {
            base.HandleUnknownRequest(requestInfo);
        }
    }

    [Serializable]
    public class SessionData
    {
        public short value1 { get; set; }
        public short value2 { get; set; }
    }
}
;