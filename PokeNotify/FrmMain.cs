using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PokeNotify
{
    public partial class FrmMain : Form
    {
        Queue json_message = new Queue();
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            new Task(async () =>
            {
                await GetJsonInfo();
            }).Start();
            btnStart.Enabled = false;
        }

        private void Print(string _message)
        {
            rtbLog.Invoke((Action) (() => rtbLog.AppendText($"[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}] {_message} \r\n")));
            rtbLog.Invoke((Action)(() => rtbLog.ScrollToCaret()));
            if (notifyIcon1.Visible)
            {
                notifyIcon1.BalloonTipText = $"{_message}";
                notifyIcon1.ShowBalloonTip(500);
            }
        }
        private void Print(Color _color, string _message)
        {
            rtbLog.Invoke((Action)(() => rtbLog.SelectionStart = rtbLog.TextLength));
            rtbLog.Invoke((Action)(() => rtbLog.SelectionLength = 0));

            rtbLog.Invoke((Action)(() => rtbLog.SelectionColor = _color));
            rtbLog.Invoke(
                (Action)
                    (() => rtbLog.AppendText($"[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}] {_message}\r\n")));

            rtbLog.Invoke((Action)(() => rtbLog.SelectionColor = rtbLog.ForeColor));
            rtbLog.Invoke((Action)(() => rtbLog.ScrollToCaret()));
            if (notifyIcon1.Visible)
            {
                notifyIcon1.BalloonTipText = $"{_message}";
                notifyIcon1.ShowBalloonTip(10);
            }
        }
        private async Task GetJsonInfo()
        {
            string s = string.Empty;
            byte[] data = new byte[1024];
            string input = "";
            string strData;
            loop:
            using (TcpClient client = new TcpClient())
            {
                try
                {
                    Print(Color.Yellow, "Đang kết nối...");
                    client.Connect("127.0.0.1", 16969);
                    Print("Kết nối thành công.");
                    NetworkStream ns = client.GetStream();
                    while (true)
                    {
                        data = new byte[1024];
                        ns.Read(data, 0, data.Length);
                        strData = Encoding.UTF8.GetString(data);
                        //Debug.Print(strData);

                        var jArray = strData.Split('\n');
                        foreach (var json in jArray)
                        {
                            if (!json.Contains("EncounterId")) break;
                            var info = JsonConvert.DeserializeObject<PokemonInfo>(json);
                            if (info.Id == PokemonId.Dratini || info.IV > 99.0)
                            {
                                Console.Beep(1500, 500);
                                Print(Color.Chartreuse,$"{info.Id} - {info.IV}%");
                            }
                            else
                            {
                                Console.Beep(1000, 100);
                                Print($"{info.Id} - {info.IV}%");
                            }
                            await Task.Delay(1000);
                        }
                        //json_message.Enqueue(info);
                        
                    }
                }
                catch (SocketException se)
                {
                    Print(Color.Red, $"Ngắt kết nối. Lý do : {se.Message}");
                }
                catch (Exception ex)
                {
                    Print(Color.Red, $"Lỗi : {ex.Message}");
                } 
            }

            goto loop;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
            }
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            // Catch this pokemon if you want
        }
    }
}
