using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComPortsApp
{
    internal class ByteStuffing
    {
        private static readonly byte ESCAPE = 0x1B; // Escape символ
        private static readonly byte FLAG = (byte)'$'; // Флаг

        public static byte[] Stuff(byte[] data)
        {
            List<byte> stuffedData = new List<byte>();

            foreach (var b in data)
            {
                if (b == FLAG || b == ESCAPE)
                {
                    stuffedData.Add(ESCAPE);
                    stuffedData.Add((byte)(b ^ 0x20)); // Используем XOR для изменения байта
                }
                else
                {
                    stuffedData.Add(b);
                }
            }

            return stuffedData.ToArray();
        }

        public static byte[] Unstuff(byte[] data)
        {
            List<byte> unstuffedData = new List<byte>();
            bool isEscape = false;

            foreach (var b in data)
            {
                if (isEscape)
                {
                    unstuffedData.Add((byte)(b ^ 0x20)); // Обратный XOR
                    isEscape = false;
                }
                else if (b == ESCAPE)
                {
                    isEscape = true;
                }
                else
                {
                    unstuffedData.Add(b);
                }
            }

            return unstuffedData.ToArray();
        }
    }
}
