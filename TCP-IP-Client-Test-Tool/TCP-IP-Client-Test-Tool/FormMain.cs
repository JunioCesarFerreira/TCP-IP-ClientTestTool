using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Windows.Forms;

namespace TCP_IP_Client_Test_Tool
{
    public partial class FormMain : Form
    {
        private TcpClient tcpClient;

        public FormMain()
        {
            InitializeComponent();

            TextBox_HostName.Text = Dns.GetHostName();
            // Get IPs Host Addresses
            IPAddress[] ips = Dns.GetHostAddresses(TextBox_HostName.Text);
            foreach (IPAddress ip in ips)
            {
                byte[] bts = ip.GetAddressBytes();
                if (bts.Length == 4)
                {
                    string Aux = "";
                    foreach (byte b in bts)
                    {
                        Aux += b.ToString() + ".";
                    }
                    Aux = Aux.Remove(Aux.Length - 1);
                    ComboBox_Ip_Host.Items.Add(Aux);
                }
            }
            // Defaults to Kestrel tests
            ComboBox_Ip_Host.SelectedIndex = 0;
            TextBox_Ip_Server.Text = "127.0.0.1";
            TextBox_Port_Server.Text = "5000";
        }

        private void Button_Send_Click(object sender, EventArgs e)
        {
            Encoding conv = Encoding.GetEncoding("ISO-8859-1"); // Encoding suitable for transmission
            byte[] buffer = conv.GetBytes(TB_Client.Text);      // ISO-8859-1 buffer
            try
            {
                tcpClient = new TcpClient(TextBox_Ip_Server.Text, Convert.ToInt32(TextBox_Port_Server.Text));
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            tcpClient.NoDelay = true; // Disable delay between packets
            tcpClient.ReceiveBufferSize = 1024;  
            try
            {
                NetworkStream networkStream = tcpClient.GetStream();
                networkStream.Write(buffer, 0, buffer.Length);
                buffer = new byte[1024];
                int bytes = networkStream.Read(buffer, 0, buffer.Length);
                TB_Server.Text = conv.GetString(buffer, 0, bytes);
                networkStream.Flush();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
            tcpClient.Close();
        }

        private void Button_OpenRequest_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Open a request sample...",
                DefaultExt = "*.txt",
                Filter = "Text(*.txt)|*.txt|All files(*.*)|*.*",
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    TB_Client.Text = File.ReadAllText(openFileDialog.FileName);
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Button_SaveRequest_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Registrar request",
                DefaultExt = "*.txt",
                Filter = "Text(*.txt)|*.txt|All files(*.*)|*.*",
                RestoreDirectory = true
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(saveFileDialog.FileName, TB_Client.Text);
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Button_SaveResponse_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Registrar response",
                DefaultExt = "*.txt",
                Filter = "Text(*.txt)|*.txt|All files(*.*)|*.*",
                RestoreDirectory = true
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(saveFileDialog.FileName, TB_Server.Text);
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
