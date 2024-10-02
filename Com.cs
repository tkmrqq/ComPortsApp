using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Ports;

namespace ComPortsApp
{
    public class DataReceivedEventArgs : EventArgs
    {
        public string Data { get; }
        public string FromPort { get; }
        public string ToPort { get; }

        public DataReceivedEventArgs(string data, string fromPort, string toPort)
        {
            Data = data;
            FromPort = fromPort;
            ToPort = toPort;
        }
    }

    public class Com : IDisposable
    {   
        private SerialPort comPort1;
        private SerialPort comPort2;
        private SerialPort comPort3;
        private SerialPort comPort4;

        private int sentBytesCount = 0;

        public event EventHandler<DataReceivedEventArgs> DataReceived;

        public Com()
        {
            string[] ports = ChoosePorts();
            comPort1 = new SerialPort(ports[0], 9600, Parity.None, 8, StopBits.One);
            comPort2 = new SerialPort(ports[1], 9600, Parity.None, 8, StopBits.One);
            comPort3 = new SerialPort(ports[2], 9600, Parity.None, 8, StopBits.One);
            comPort4 = new SerialPort(ports[3], 9600, Parity.None, 8, StopBits.One);

            comPort1.DataReceived += new SerialDataReceivedEventHandler(ComPort1_DataReceived); // COMx -> COMy+1
            comPort2.DataReceived += new SerialDataReceivedEventHandler(ComPort2_DataReceived); // COMx+1 <- COMy

            comPort3.DataReceived += new SerialDataReceivedEventHandler(ComPort3_DataReceived); // COMy -> COMx+1
            comPort4.DataReceived += new SerialDataReceivedEventHandler(ComPort4_DataReceived); // COMy+1 <- COMx

            comPort1.Open();
            comPort2.Open();
            comPort3.Open();
            comPort4.Open();

        }

        private void ComPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = comPort1.ReadExisting();
        }

        private void ComPort2_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = comPort2.ReadExisting();
            DataReceived?.Invoke(this, new DataReceivedEventArgs(data, comPort1.PortName, comPort2.PortName));
        }
        private void ComPort3_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = comPort3.ReadExisting();
            DataReceived?.Invoke(this, new DataReceivedEventArgs(data, comPort4.PortName, comPort3.PortName));
        }

        private void ComPort4_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //string data = comPort4.ReadExisting();
            //DataReceived?.Invoke(this, new DataReceivedEventArgs(data, comPort4.PortName, comPort3.PortName));
        }

        public void SendData(string data, int groupNumber)
        {
            byte[] dataBytes = Encoding.ASCII.GetBytes(data);
            byte[] frameData = new byte[groupNumber + 1]; // Длина n + 1
            Array.Copy(dataBytes, frameData, Math.Min(dataBytes.Length, frameData.Length));

            // Создаем кадр
            Frame frame = new Frame((byte)comPort1.PortName[3], frameData, groupNumber);

            // Выполняем байт-стаффинг
            byte[] stuffedFrame = ByteStuffing.Stuff(frame.ToBytes());

            if (comPort1.IsOpen)
            {
                comPort1.Write(stuffedFrame, 0, stuffedFrame.Length);
                sentBytesCount += stuffedFrame.Length;
            }
            else if (comPort4.IsOpen)
            {
                comPort4.Write(stuffedFrame, 0, stuffedFrame.Length);
                sentBytesCount += stuffedFrame.Length;
            }
        }

        public void OpenPorts(string fromPort, string toPort)
        {
            if (comPort1.IsOpen) comPort1.Close();
            if (comPort2.IsOpen) comPort2.Close();
            if (comPort3.IsOpen) comPort3.Close();
            if (comPort4.IsOpen) comPort4.Close();

            if (fromPort == comPort1.PortName && toPort == comPort2.PortName)
            {
                comPort1.Open();
                comPort2.Open();
            }
            else if (fromPort == comPort3.PortName && toPort == comPort4.PortName)
            {
                comPort3.Open();
                comPort4.Open();
            }
        }

        public int returnBaudRate => comPort1.BaudRate;
        public int returnBytesCount => sentBytesCount;

        public static string[] ChoosePorts()
        {
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length >= 4)
            {
                return new string[] { ports[0], ports[1], ports[2], ports[3] };
            }
            else
            {
                throw new Exception("Недостаточно доступных COM-портов.");
            }
        }

        public void changeParity(string selectedParity)
        {
            Parity parity = (Parity)Enum.Parse(typeof(Parity), selectedParity);
            comPort1.Parity = parity;
            comPort2.Parity = parity;
            comPort3.Parity = parity;
            comPort4.Parity = parity;
        }

        public string getParity()
        {
            Parity parity = comPort1.Parity;
            return parity.ToString();
        }

        public void Dispose()
        {
            comPort1?.Close();
            comPort2?.Close();
            comPort3?.Close();
            comPort4?.Close();
        }
    }
}
