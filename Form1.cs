using System.IO.Ports;
using System.Security.Cryptography;
using System.Security.Policy;

namespace ComPortsApp
{
    public partial class Form1 : Form
    {
        private Com communication;
        string[] ports = Com.ChoosePorts();
        public Form1()
        {
            InitializeComponent();
            communication = new Com();
            communication.DataReceived += OnDataReceived;
            labelChange();
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

            communication.OpenPorts(ports[0], ports[1]);

        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                listBox1.Items.Add($"Данные: {e.Data} (с {e.FromPort} на {e.ToPort}){Environment.NewLine}");
            }));
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string inputText = textBox1.Text;
                communication.SendData(inputText);
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
            label1.Text = $"Скорость порта: {communication.returnBaudRate} бит/сек\n" +
                $"Отправлено байт: {communication.returnBytesCount}\n" +
                $"Паритет: {communication.getParity()}";
        }

        private void comboBoxParity_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedParity = comboBoxParity.SelectedItem.ToString();
            if (selectedParity != null)
                communication.changeParity(selectedParity);
        }
    }
}
