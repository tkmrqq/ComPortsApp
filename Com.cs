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

            comPort1.DataReceived += ComPort1_DataReceived; // COMx -> COMy+1
            comPort2.DataReceived += ComPort2_DataReceived; // COMx+1 <- COMy

            comPort3.DataReceived += ComPort3_DataReceived; // COMy -> COMx+1
            comPort4.DataReceived += ComPort4_DataReceived; // COMy+1 <- COMx

            comPort1.Open();
            comPort2.Open();
            comPort3.Open();
            comPort4.Open();

        }

        private void ComPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = comPort1.ReadExisting();
            DataReceived?.Invoke(this, new DataReceivedEventArgs(data, comPort1.PortName, comPort2.PortName));
        }

        private void ComPort2_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = comPort2.ReadExisting();
            DataReceived?.Invoke(this, new DataReceivedEventArgs(data, comPort2.PortName, comPort1.PortName));
        }
        private void ComPort3_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = comPort3.ReadExisting();
            comPort4.Write(data);
            DataReceived?.Invoke(this, new DataReceivedEventArgs(data, comPort3.PortName, comPort4.PortName));
        }

        private void ComPort4_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = comPort4.ReadExisting();
            DataReceived?.Invoke(this, new DataReceivedEventArgs(data, comPort4.PortName, comPort3.PortName));
        }

        public void SendData(string data)
        {
            if (comPort1.IsOpen)
            {
                comPort1.Write(data);
                sentBytesCount += Encoding.ASCII.GetByteCount(data);
                DataReceived?.Invoke(this, new DataReceivedEventArgs(data, comPort1.PortName, comPort2.PortName));
            }
            else if (comPort3.IsOpen)
            {
                comPort3.Write(data);
                sentBytesCount += Encoding.ASCII.GetByteCount(data);
                DataReceived?.Invoke(this, new DataReceivedEventArgs(data, comPort3.PortName, comPort4.PortName));
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
            ConfigurePortsForSend(fromPort, toPort);
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

        public void Dispose()
        {
            comPort1?.Close();
            comPort2?.Close();
            comPort3?.Close();
            comPort4?.Close();
        }

        public void ConfigurePortsForSend(string fromPort, string toPort)
        {
            comPort1.DataReceived -= ComPort1_DataReceived;
            comPort2.DataReceived -= ComPort2_DataReceived;
            comPort3.DataReceived -= ComPort3_DataReceived;
            comPort4.DataReceived -= ComPort4_DataReceived;

            if (fromPort == comPort1.PortName && toPort == comPort2.PortName)
            {
                comPort1.DataReceived += (sender, e) =>
                {
                    string data = comPort1.ReadExisting();
                    comPort2.Write(data);
                    DataReceived?.Invoke(this, new DataReceivedEventArgs(data, comPort1.PortName, comPort2.PortName));
                };
            }
            else if (fromPort == comPort3.PortName && toPort == comPort4.PortName)
            {
                comPort3.DataReceived += (sender, e) =>
                {
                    string data = comPort3.ReadExisting();
                    comPort4.Write(data);
                    DataReceived?.Invoke(this, new DataReceivedEventArgs(data, comPort3.PortName, comPort4.PortName));
                };
            }
        }

    }
}
