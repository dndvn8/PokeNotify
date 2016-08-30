using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PokeNotify.Core
{
    public class Listener
    {
        public delegate void PokeNotifyEventHandler(object sender, PokeInfoModel sniperInfo);
        public event PokeNotifyEventHandler RecieveEventHandler;
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
                            while (client.Connected)
                            {
                                var line = sr.ReadLine();
                                if (line == null)
                                    throw new Exception("Unable to ReadLine from sniper socket");

                                var info = JsonConvert.DeserializeObject<PokeInfoModel>(line);
                                OnReceive(info);
                            }   
                        }
                    }
                }
                catch (SocketException)
                {
                    // this is spammed to often. Maybe add it to debug log later
                }
                catch (Exception)
                {
                    // most likely System.IO.IOException
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
    }
}
