using SuperSocket.ClientEngine;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.CommonLib.MglServerConfig
{
    /// <summary>
    /// 바이너리 요청 정보 정의 클래스
    /// </summary>
    public class MglBinaryRequestInfo : BinaryRequestInfo
    {
        public Int32 key { get; set; }
        public Int16 value1 { get; set; }
        public Int16 value2 { get; set; }

        public MglBinaryRequestInfo(int handlerkey, short shortvalue, short shortvalue2, int length, byte[] body)
           : base(null, body)
        {
            key = handlerkey;
            value1 = shortvalue;
            value2 = shortvalue2;
        }
    }

    /// <summary>
    /// 리시브 필터 정의 / 고정 헤더 필
    /// </summary>
    public class ReceiveFilter : FixedHeaderReceiveFilter<MglBinaryRequestInfo>
    {
        public ReceiveFilter()
            : base(12)
        {

        }
        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(header, offset + 8, 4);

            var nBodySize = BitConverter.ToInt32(header, offset + 8);
            return nBodySize;
        }

        protected override MglBinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(header.Array, 0, 12);

            return new MglBinaryRequestInfo(BitConverter.ToInt32(header.Array, 0),
                                           BitConverter.ToInt16(header.Array, 0 + 4),
                                           BitConverter.ToInt16(header.Array, 0 + 6),
                                           BitConverter.ToInt32(header.Array, 0 + 8),
                                           bodyBuffer.CloneRange(offset, length));
        }
    }
}
