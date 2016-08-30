using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using PokeNotify.Core;
using PokeNotify.Core.Enums;
using PokeNotify.Core.Models;

namespace PokeNotify.Views
{
    public partial class FrmMain : MetroForm
    {
        private Settings _settings = new Settings();
        private bool _started = false;
        public FrmMain()
        {
            InitializeComponent();
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!_started)
            {
                _started = true;
                var pokeListener = new Listener();
                pokeListener.RecieveEventHandler += ReceiveEventHandler;
                pokeListener.RecieveLogEventHandler += OnRecieveLogEventHandler;
                pokeListener.AsyncTask("localhost", 16969);
                btnStart.Text = @"Exit";
            }
            else
            {
                this.FrmMain_FormClosing(null, null);
            }
        }
        private void ReceiveEventHandler(object sender, PokeInfoModel info)
        {
            var time = info.ExpirationTimestamp.Year == 0001
                ? "unknow"
                : (info.ExpirationTimestamp - DateTime.Now).ToString("mm\\:ss");
            if (info.IV > 99.0)
            {
                var pokesnipe2 = Utils.Pokesnipe2Builder(notifyIcon1.BalloonTipText);
                Utils.RandomLocation(_settings);
                txtDefaultLatitude.Text = _settings.Latitude.ToString("N9", CultureInfo.InvariantCulture);
                txtDefaultLongitude.Text = _settings.Longitude.ToString("N9", CultureInfo.InvariantCulture);
                PokeSnipers(pokesnipe2);
            }
            if (info.Id == PokemonId.Dratini || info.IV > 95.0)
            {
                Console.Beep(1500, 500);
                Print(Color.Chartreuse, $"{info.Id} - {info.IV}% - Lat/Lng:{info.Latitude},{info.Longitude} - {time}");
            }
            else
            {
                Console.Beep(1000, 100);
                Print($"{info.Id} - {info.IV}% - Lat/Lng:{info.Latitude},{info.Longitude} - {time}");
            }
        }
        private void OnRecieveLogEventHandler(object sender, string log)
        {
            Print(log);
        }
        private void Print(string _message)
        {
            rtbLog.Invoke((Action) (() => rtbLog.AppendText($"[{DateTime.Now.ToString("dd/MM/yy HH:mm:ss.fff")}] {_message} \r\n")));
            rtbLog.Invoke((Action)(() => rtbLog.ScrollToCaret()));
            //if (notifyIcon1.Visible)
            //{
                //notifyIcon1.BalloonTipText = $"{_message}";
                //notifyIcon1.ShowBalloonTip(100);
            //}
        }
        private void Print(Color _color, string _message)
        {
            rtbLog.Invoke((Action)(() => rtbLog.SelectionStart = rtbLog.TextLength));
            rtbLog.Invoke((Action)(() => rtbLog.SelectionLength = 0));

            rtbLog.Invoke((Action)(() => rtbLog.SelectionColor = _color));
            rtbLog.Invoke(
                (Action)
                    (() => rtbLog.AppendText($"[{DateTime.Now.ToString("dd/MM/yy HH:mm:ss.fff")}] {_message}\r\n")));

            rtbLog.Invoke((Action)(() => rtbLog.SelectionColor = rtbLog.ForeColor));
            rtbLog.Invoke((Action)(() => rtbLog.ScrollToCaret()));
            //if (notifyIcon1.Visible)
            //{
                notifyIcon1.BalloonTipText = $"{_message}";
                notifyIcon1.ShowBalloonTip(100);
            //}
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            //notifyIcon1.Visible = false;
        }
        private void FrmMain_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = false;
                notifyIcon1.Visible = true;
                this.Hide();
            }
            //else if (FormWindowState.Normal == this.WindowState)
            //{
            //    notifyIcon1.Visible = false;
            //}
        }
        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            // Catch this pokemon if you want
            var pokesnipe2 = Utils.Pokesnipe2Builder(notifyIcon1.BalloonTipText);
            Utils.RandomLocation(_settings);
            txtDefaultLatitude.Text = _settings.Latitude.ToString("N9", CultureInfo.InvariantCulture);
            txtDefaultLongitude.Text = _settings.Longitude.ToString("N9", CultureInfo.InvariantCulture);
            var msg = MessageBox.Show(this, $"Do you want snipe this pokemon?", @"Confirm", MessageBoxButtons.YesNo);
            if (msg == DialogResult.Yes)
            {
                PokeSnipers(pokesnipe2);
            }
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }
        private void btnMap_Click(object sender, EventArgs e)
        {
            Maps map = new Maps(Utils.ConvertToDouble(txtDefaultLatitude.Text),
                Utils.ConvertToDouble(txtDefaultLongitude.Text));

            if (map.ShowDialog() == DialogResult.OK)
            {
                txtDefaultLatitude.Text = map.Lat.ToString(CultureInfo.InvariantCulture);
                txtDefaultLongitude.Text = map.Lng.ToString(CultureInfo.InvariantCulture);
            }
        }
        private void FrmMain_Load(object sender, EventArgs e)
        {
            if (File.Exists(_settings.Pokesnipe2Path))
            {
                Utils.LoadSniper2Config(_settings);
                txtPokeSnipePath.Text = _settings.Pokesnipe2Path;
                txtDefaultLatitude.Text = _settings.Latitude.ToString("N9", CultureInfo.InvariantCulture);
                txtDefaultLongitude.Text = _settings.Longitude.ToString("N9", CultureInfo.InvariantCulture);
            }
            else
            {
                txtPokeSnipePath.Text = @"";
                txtDefaultLatitude.Text = _settings.Latitude.ToString("N9", CultureInfo.InvariantCulture);
                txtDefaultLongitude.Text = _settings.Longitude.ToString("N9", CultureInfo.InvariantCulture);
            }
        }
        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            _settings.Pokesnipe2Path = txtPokeSnipePath.Text;
            _settings.Latitude = Utils.ConvertToDouble(txtDefaultLatitude.Text);
            _settings.Longitude = Utils.ConvertToDouble(txtDefaultLongitude.Text);
            Utils.SaveSniper2Config(_settings);
        }
        private void btnRemovePath_Click(object sender, EventArgs e)
        {
            txtPokeSnipePath.Text = @"";
        }
        private void txtPokeSnipePath_DoubleClick(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                var sniper2Path = Path.Combine(fbd.SelectedPath, "PokeSniper2.exe");
                if (File.Exists(sniper2Path))
                    txtPokeSnipePath.Text = sniper2Path;
            }
        }
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
            Environment.Exit(0);
        }
        private void StartProcessWithPath(string arg)
        {
            try
            {
                var process = new Process();
                var sniperFilePath = _settings.Pokesnipe2Path;
                var sniperFileDir = System.IO.Path.GetDirectoryName(sniperFilePath);
                process.StartInfo.FileName = sniperFilePath;
                process.StartInfo.WorkingDirectory = sniperFileDir;
                process.StartInfo.Arguments = arg;
                    //$"pokesniper2://{info.Id}/{info.Latitude.ToString("N6", CultureInfo.InvariantCulture).Replace(",", ".")},{info.Longitude.ToString("N6", CultureInfo.InvariantCulture).Replace(",", ".")}";
                process.Start();
                KillProcessLater(process);
            }
            catch (Exception e)
            {
                //
            }
        }
        public void PokeSnipers(string arg)
        {
            try
            {
                if (_settings.Pokesnipe2Path.Contains(".exe"))
                {
                    Print($"using the path: {_settings.Pokesnipe2Path} to start pokesniper2 ");
                    StartProcessWithPath(arg);
                }
                else
                {
                    Print("using url to start pokesniper2 ");
                    var process = Process.Start(arg);
                    KillProcessLater(process);
                }
            }
            catch (Exception e)
            {
                Print( $"Error while launching pokesniper2 {e}");
            }
        }
        private void KillProcessLater(Process process)
        {
            if (process != null)
            {
                Task.Run(async () => await AfterDelay(() =>
                {
                    process.Kill();
                    process.Dispose();
                }, 30000));
            }
        }
        private async Task AfterDelay(Action action, int delay)
        {
            try
            {
                await Task.Delay(delay);
                action.Invoke();
            }
            catch (Exception ex)
            {

            }
        }
        
    }
}
