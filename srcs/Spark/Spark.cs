﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Spark.Core;
using Spark.Core.Server;
using Spark.Database;
using Spark.Event;
using Spark.Extension;
using Spark.Game.Abstraction;
using Spark.Game.Abstraction.Factory;
using Spark.Game.Factory;
using Spark.Gameforge;
using Spark.Network.Session;
using Spark.Packet;
using Spark.Processor;

namespace Spark
{
    public sealed class Spark : ISpark
    {
        private static bool Created { get; set; }

        public IClientFactory ClientFactory { get; }
        public IEventPipeline EventPipeline { get; }
        public IPacketManager PacketManager { get;}
        public IPacketFactory PacketFactory { get; }
        public IGameDataProvider GameDataProvider { get; }
        public IGameforgeService GameforgeService { get; }
        public IEnumerable<IEventHandler> BuiltInEventHandlers { get; }
        public IEnumerable<IPacketProcessor> BuildInPacketProcessors { get; }

        public Spark(IClientFactory clientFactory, IEventPipeline eventPipeline, IPacketManager packetManager, IPacketFactory packetFactory, IGameDataProvider gameDataProvider, IGameforgeService gameforgeService, IEnumerable<IEventHandler> builtInEventHandlers, IEnumerable<IPacketProcessor> builtInPacketProcessors)
        {
            if (Created)
            {
                throw new InvalidOperationException("Can't create multiple instance of Spark");
            }
            
            ClientFactory = clientFactory;
            EventPipeline = eventPipeline;
            PacketManager = packetManager;
            PacketFactory = packetFactory;
            GameDataProvider = gameDataProvider;
            GameforgeService = gameforgeService;
            BuiltInEventHandlers = builtInEventHandlers;
            BuildInPacketProcessors = builtInPacketProcessors;

            Created = true;
        }

        public async Task<IClient> CreateRemoteClient(IPEndPoint ip, string token, Predicate<WorldServer> serverSelector, Predicate<SelectableCharacter> characterSelector)
        {
            IClient client = await ClientFactory.CreateClient(ip, serverSelector, characterSelector);
            
            client.SendPacket($"NoS0577 {token} {GameforgeService.InstallationId} 007C762C 20.9.3.3127 0 D0C4D9B41720BC5E00E1C6C7DC6B8B22");

            return client;
        }

        public void Initialize()
        {
            EventPipeline.AddEventHandlers(BuiltInEventHandlers);
            PacketManager.AddPacketProcessors(BuildInPacketProcessors);
            
            GameDataProvider.EnsureCreated();
        }

        public Task<GameforgeResponse<string>> GetSessionToken(string email, string password, string locale, Predicate<GameforgeAccount> predicate) =>
            GameforgeService.GetSessionToken(email, password, locale, predicate);

        public void AddEventHandler<T>(T handler) where T : IEventHandler
        {
            EventPipeline.AddEventHandler(handler);
        }

        public static ISpark CreateInstance()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddImplementingTypes<IPacketProcessor>();
            services.AddImplementingTypes<IEventHandler>();

            services.AddTransient<IGameforgeService, GameforgeService>();
            services.AddTransient<IClientFactory, ClientFactory>();
            services.AddTransient<IMapFactory, MapFactory>();
            services.AddTransient<ISessionFactory, SessionFactory>();

            services.AddSingleton<IPacketFactory, PacketFactory>();
            services.AddSingleton<IPacketManager, PacketManager>();
            services.AddSingleton<IEventPipeline, EventPipeline>();
            services.AddSingleton<IGameDataProvider, GameDataProvider>();

            services.AddSingleton<Spark>();

            Spark spark = services.BuildServiceProvider().GetService<Spark>();

            spark.Initialize();
            
            return spark;
        }

        public void Dispose()
        {
            
        }
    }
}