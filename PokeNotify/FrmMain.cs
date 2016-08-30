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
using PokeNotify.Core;

namespace PokeNotify
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            var pokeListener = new Listener();
            pokeListener.RecieveEventHandler += ReceiveEventHandler;
            pokeListener.AsyncTask("localhost", 16969);
            btnStart.Enabled = false;
        }
        private void ReceiveEventHandler(object sender, PokeInfoModel info)
        {
            if (info.Id == PokemonId.Dratini || info.IV > 99.0)
            {
                Console.Beep(1500, 500);
                Print(Color.Chartreuse, $"{info.Id} - {info.IV}%");
            }
            else
            {
                Console.Beep(1000, 100);
                Print($"{info.Id} - {info.IV}%");
            }
        }
        private void Print(string _message)
        {
            rtbLog.Invoke((Action) (() => rtbLog.AppendText($"[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}] {_message} \r\n")));
            rtbLog.Invoke((Action)(() => rtbLog.ScrollToCaret()));
            if (notifyIcon1.Visible)
            {
                notifyIcon1.BalloonTipText = $"{_message}";
                notifyIcon1.ShowBalloonTip(100);
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
                notifyIcon1.ShowBalloonTip(100);
            }
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
