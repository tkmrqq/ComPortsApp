using System.IO.Ports;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;

namespace ComPortsApp
{
    public partial class Form1 : Form
    {
        private Com communication;
        string[] ports;
        public Form1()
        {
            InitializeComponent();
            try
            {
                ports = Com.ChoosePorts();
                communication = new Com();
                communication.DataReceived += OnDataReceived;
                labelChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxPorts.Items.Add($"{ports[0]} -> {ports[1]}");
            comboBoxPorts.Items.Add($"{ports[2]} <- {ports[3]}");

            comboBoxParity.Items.Add("None");
            comboBoxParity.Items.Add("Odd");
            comboBoxParity.Items.Add("Even");
            comboBoxParity.Items.Add("Mark");
            comboBoxParity.Items.Add("Space");

            if (comboBoxPorts.Items.Count > 0)
                comboBoxPorts.SelectedIndex = 0;

            if (comboBoxParity.Items.Count > 0)
                comboBoxParity.SelectedIndex = 0;

            for (int i = 1; i <= 10; i++)
            {
                comboBoxGroupNumber.Items.Add(i.ToString());
            }
            comboBoxGroupNumber.SelectedIndex = 0;

            communication.OpenPorts(ports[0], ports[1]);

        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                listBox1.Items.Add($"������: {e.Data} (� {e.FromPort} �� {e.ToPort}){Environment.NewLine}");

                // ��������� ��-��������
                byte[] receivedBytes = Encoding.UTF8.GetBytes(e.Data);
                byte[] unstuffedData = ByteStuffing.Unstuff(receivedBytes);

                // ����� ��������� �����
                Frame receivedFrame = ParseFrame(unstuffedData);

                // ������� ����
                listBox1.Items.Add($"Flag: {receivedFrame.Flag[0]:X2} {receivedFrame.Flag[1]:X2}");

                // ������� ������
                listBox1.Items.Add($"Destination Address: {receivedFrame.DestinationAddress:X2}");
                listBox1.Items.Add($"Source Address: {receivedFrame.SourceAddress:X2}");

                // ������� ������ � �������� ���������� �����
                for (int i = 0; i < Math.Min(receivedBytes.Length, unstuffedData.Length); i++)
                {
                    if (receivedBytes[i] != unstuffedData[i])
                    {
                        listBox1.Items.Add($"������� ����: {receivedBytes[i]:X2} -> {unstuffedData[i]:X2} *");
                    }
                    else
                    {
                        listBox1.Items.Add($"����: {receivedBytes[i]:X2}");
                    }
                }

                // ���� ���� ������ ����� � �������� ������
                if (receivedBytes.Length > unstuffedData.Length)
                {
                    for (int i = unstuffedData.Length; i < receivedBytes.Length; i++)
                    {
                        listBox1.Items.Add($"������ ����: {receivedBytes[i]:X2}");
                    }
                }
                // ���� ���� ������ ����� � ������������ ������ (����� ��-���������)
                else if (unstuffedData.Length > receivedBytes.Length)
                {
                    for (int i = receivedBytes.Length; i < unstuffedData.Length; i++)
                    {
                        listBox1.Items.Add($"����������� ���� ����� ��-���������: {unstuffedData[i]:X2}");
                    }
                }
                // ������� FCS
                listBox1.Items.Add($"FCS: {receivedFrame.FCS:X2}");
                listBox1.Items.Add("============================");
                byte[] received = Encoding.UTF8.GetBytes(e.Data); // ��������������, ��� inputData - ��� ������ �����.
                listBox1.Items.Add($"������� �����: {string.Join(" ", received.Select(b => b.ToString("X2")))}");
                listBox1.Items.Add($"�� ���������: {string.Join(" ", received.Select(b => b.ToString("X2")))}");
                byte[] stuffedData = ByteStuffing.Stuff(received);
                listBox1.Items.Add($"����� ���������: {string.Join(" ", stuffedData.Select(b => b.ToString("X2")))}");
            }));
        }

        private Frame ParseFrame(byte[] frameBytes)
        {
            if (frameBytes.Length < 5) throw new Exception("�������� ������ �����");

            byte[] flag = new byte[] { frameBytes[0], frameBytes[1] };
            byte destinationAddress = frameBytes[2];
            byte sourceAddress = frameBytes[3];

            byte[] data = new byte[frameBytes.Length - 5];
            Array.Copy(frameBytes, 4, data, 0, data.Length);

            byte fcs = frameBytes[frameBytes.Length - 1];

            return new Frame(sourceAddress, data, flag[1] - 'a');
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string inputText = textBox1.Text;
                int groupNumber = comboBoxGroupNumber.SelectedIndex; // �������� ����� ������
                communication.SendData(inputText, groupNumber);
                textBox1.Clear();
                e.SuppressKeyPress = true;
                labelChange();
            }
        }

        private void comboBoxPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedDirection = comboBoxPorts.SelectedItem.ToString();

            if (selectedDirection.Contains("->"))
            {
                communication.OpenPorts(ports[0], ports[1]);
            }
            else if (selectedDirection.Contains("<-"))
            {
                communication.OpenPorts(ports[2], ports[3]);
            }
        }

        private void labelChange()
        {
            label1.Text = $"�������� �����: {communication.returnBaudRate} ���/���\n" +
                $"���������� ����: {communication.returnBytesCount}\n" +
                $"�������: {communication.getParity()}";
        }

        private void comboBoxParity_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedParity = comboBoxParity.SelectedItem.ToString();
            if (selectedParity != null)
                communication.changeParity(selectedParity);
        }
    }
}
