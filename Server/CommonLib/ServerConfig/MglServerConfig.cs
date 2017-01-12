using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;

namespace Server.CommonLib.MglServerConfig
{
    class MglServer : AppServer<Session, MglBinaryRequestInfo>
    {
        //핸들러
        private Dictionary<uint, Action<Session, MglBinaryRequestInfo>> handlerMap = new Dictionary<uint, Action<Session, MglBinaryRequestInfo>>();

        private IServerConfig config;
        private HandlerDef handlerDef = new HandlerDef();
        //1 ~ 127 server No
        private readonly int ServerNo = int.Parse(ConfigurationManager.AppSettings["ServerNo"]);        

        public MglServer()
            : base(new DefaultReceiveFilterFactory<ReceiveFilter, MglBinaryRequestInfo>())
        {
            NewSessionConnected += new SessionHandler<Session>(OnConnected);
            SessionClosed += new SessionHandler<Session, CloseReason>(OnClosed);
            NewRequestReceived += new RequestHandler<Session, MglBinaryRequestInfo>(RequestReceived);            
        }       

        void RegistHandler()
        {
            handlerMap.Add((int)ProtocolKey.ECHO, handlerDef.Echo);
            handlerMap.Add((int)ProtocolKey.MSG, handlerDef.SendMsg);
        }

        public void InitConfig()
        {
            config = new ServerConfig
            {
                Port = 8000,
                Ip = "Any",
                MaxConnectionNumber = 100,
                Mode = SocketMode.Tcp,
                Name = "BoardServerNet"
            };
        }

        public void CreateServer()
        {
            var flag = Setup(new RootConfig(), config, logFactory: new Log4NetLogFactory());

            if (flag == false)
            {                
                return;
            }
            RegistHandler();
        }

        public bool IsRunning(ServerState eCurState)
        {
            if (eCurState == ServerState.Running)
            {
                return true;
            }

            return false;
        }


        private void RequestReceived(Session session, MglBinaryRequestInfo requestInfo)
        {
            Console.WriteLine("세션 번호 {0} 받은 데이터 크기 : {1}, ThreadId : {2}", session.SessionID, requestInfo.Body.Length, Thread.CurrentThread.ManagedThreadId);

            var id = requestInfo.key;
            var value1 = requestInfo.value1;
            var value2 = requestInfo.value2;
            if(string.IsNullOrEmpty(session.user_id))
            {
                session.user_id = Encoding.Unicode.GetString(requestInfo.Body);
                session.data = new SessionData() { ServerNo = value1, ChannelNo = value2 };
            }

            if (handlerMap.ContainsKey(id))
            {
                handlerMap[id](session, requestInfo);
            }
            else
            {
                Console.Write("세션 번호 {0} 받은 데이터 크기: {1}", session.SessionID, requestInfo.Body.Length);
            }

            Console.WriteLine("요청 받음");
        }

        private void OnClosed(Session session, CloseReason value)
        {
            Console.WriteLine("세션 번호 {0} 접속해제 : {1}", session.SessionID, value.ToString());
            Console.WriteLine("세션 종료 요청 받음");
        }

        private void OnConnected(Session session)
        {
            Console.WriteLine("세션 번호 {0} 접속", session.SessionID);

            //세션 검색후에 새로운 세션인 경우에 리스트에 포함            
            Console.WriteLine("Server Connected : {0}", session.SessionID);            

            Console.WriteLine("세션 연결 요청 받음");
        }
    }


}
