using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComPortsApp
{
    internal class Frame
    {
        public byte[] Flag { get; set; } // 2 байта: $ и 'a' + n
        public byte DestinationAddress { get; set; } = 0x00; // 1 байт (нулевой)
        public byte SourceAddress { get; set; } // 1 байт (номер передающего порта)
        public byte[] Data { get; set; } // n + 1 байт
        public byte FCS { get; set; } = 0x00; // 1 байт (нулевой)
        public Frame(byte sourceAddress, byte[] data, int groupNumber)
        {
            Flag = new byte[] { (byte)'$', (byte)('a' + groupNumber) };
            SourceAddress = sourceAddress;
            Data = data; // Data имеет фиксированную длину n + 1
        }

        public byte[] ToBytes()
        {
            List<byte> frameBytes = new List<byte>();
            frameBytes.AddRange(Flag);
            frameBytes.Add(DestinationAddress);
            frameBytes.Add(SourceAddress);
            frameBytes.AddRange(Data);
            frameBytes.Add(FCS);
            return frameBytes.ToArray();
        }
    }
}
