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
        public UInt32 key { get; set; }
        public UInt32 value1 { get; set; }
        public UInt32 value2 { get; set; }

        public MglBinaryRequestInfo(uint handlerkey, uint shortvalue, uint shortvalue2, uint length, byte[] body)
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
            : base(16)
        {

        }
        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(header, offset + 12, 4);

            var nBodySize = BitConverter.ToInt32(header, offset + 12);
            return nBodySize;
            //return (int)header[offset + 8] * 256 + (int)header[offset + 9];
        }

        protected override MglBinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)        
        {
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(header.Array, 0, 16);

            return new MglBinaryRequestInfo(BitConverter.ToUInt32(header.Array, 0),
                                           BitConverter.ToUInt32(header.Array, 0 + 4),
                                           BitConverter.ToUInt32(header.Array, 0 + 8),
                                           BitConverter.ToUInt32(header.Array, 0 + 12),
                                           bodyBuffer.CloneRange(offset, length));
        }
    }
}
