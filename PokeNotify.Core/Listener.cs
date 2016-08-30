using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using PokeNotify.Core.Models;

namespace PokeNotify.Core
{
    public class Listener
    {
        public delegate void DelegateRecieveLogEventHandler(object sender, string log);
        public delegate void DelegatePokeNotifyEventHandler(object sender, PokeInfoModel sniperInfo);
        public event DelegatePokeNotifyEventHandler RecieveEventHandler;
        public event DelegateRecieveLogEventHandler RecieveLogEventHandler;
        //private Logger.Logger _logger = PokeNotify.Logger.LogManager.GetLogger(typeof(Listener));
        public Task AsyncTask(string server, int port, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => Start(server, port, cancellationToken), cancellationToken);
        }
        private async Task Start(string server, int port, CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    using (var client = new TcpClient())
                    {
                        client.Connect(server, port);
                        using (var sr = new StreamReader(client.GetStream()))
                        {
                            if (client.Connected)
                                OnLogging($"Connected to {server} in port {port}");
                            while (client.Connected)
                            {
                                var line = sr.ReadLine();
                                if (line == null)
                                    throw new Exception("Unable to ReadLine from sniper socket");

                                var info = JsonConvert.DeserializeObject<PokeInfoModel>(line);
                                OnReceive(info);
                            }
                            OnLogging($"Disconnected from {server}");   
                        }
                    }
                }
                catch (SocketException se)
                {
                    OnLogging(se.Message);
                }
                catch (Exception ex)
                {
                    OnLogging(ex.Message);
                }
                await Task.Delay(5000, cancellationToken);
            }
        }
        protected virtual void OnReceive(PokeInfoModel pokeInfo)
        {
            Task.Run(() =>
            {
                RecieveEventHandler?.Invoke(this, pokeInfo);
            });
        }

        protected virtual void OnLogging(string log)
        {
            Task.Run(() =>
            {
                RecieveLogEventHandler?.Invoke(this, log);
            });
        }
    }
}
