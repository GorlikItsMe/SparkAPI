﻿using NLog;
using Spark.Event;
using Spark.Event.Characters;
using Spark.Game.Abstraction;
using Spark.Game.Abstraction.Factory;
using Spark.Packet.Characters;

namespace Spark.Processor.Characters
{
    public class AtProcessor : PacketProcessor<At>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IMapFactory _mapFactory;
        private readonly IEventPipeline _eventPipeline;

        public AtProcessor(IMapFactory mapFactory, IEventPipeline eventPipeline)
        {
            _mapFactory = mapFactory;
            _eventPipeline = eventPipeline;
        }
        
        protected override void Process(IClient client, At packet)
        {
            IMap map = _mapFactory.CreateMap(packet.MapId);
            if (map == null)
            {
                Logger.Error($"Failed to create map {packet.MapId}");
                return;
            }

            client.Character.Position = packet.Position;
            client.Character.Direction = packet.Direction;
            
            map.AddEntity(client.Character);
            _eventPipeline.Emit(new MapJoinEvent(client, map));
        }
    }
}