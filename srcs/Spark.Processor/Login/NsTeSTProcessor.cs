﻿using System.Linq;
using System.Threading.Tasks;
using NLog;
using Spark.Core.Server;
using Spark.Game.Abstraction;
using Spark.Network.Option;
using Spark.Network.Session;
using Spark.Packet.Login;

namespace Spark.Processor.Login
{
    public class NsTeSTProcessor : PacketProcessor<NsTeST>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ISessionFactory _sessionFactory;

        public NsTeSTProcessor(ISessionFactory sessionFactory) => _sessionFactory = sessionFactory;

        protected override async Task Process(IClient client, NsTeST packet)
        {
            LoginOption option = client.GetStorage<LoginOption>();
            WorldServer server = packet.Servers.FirstOrDefault(x => option.ServerSelector.Invoke(x));

            if (server == null)
            {
                Logger.Error("Can't found world server");
                return;
            }

            client.Session = await _sessionFactory.CreateSession(server.Ip, packet.EncryptionKey);

            client.SendPacket($"{packet.EncryptionKey}");
            await Task.Delay(1000).ContinueWith(x =>
            {
                client.SendPacket($"{packet.Name} GFMODE 2");
                client.SendPacket("thisifgamemode");
            });
        }
    }
}