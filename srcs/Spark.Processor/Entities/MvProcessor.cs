﻿using System.Threading.Tasks;
using NLog;
using Spark.Core;
using Spark.Event;
using Spark.Event.Entities;
using Spark.Game.Abstraction;
using Spark.Game.Abstraction.Entities;
using Spark.Packet.Entities;

namespace Spark.Processor.Entities
{
    public class MvProcessor : PacketProcessor<Mv>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IEventPipeline _eventPipeline;

        public MvProcessor(IEventPipeline eventPipeline) => _eventPipeline = eventPipeline;

        protected override void Process(IClient client, Mv packet)
        {
            IMap map = client.Character.Map;
            if (map == null)
            {
                Logger.Error("Can't process in packet, character map is null");
                return;
            }

            ILivingEntity entity = map.GetEntity<ILivingEntity>(packet.EntityType, packet.EntityId);
            if (entity == null)
            {
                return;
            }

            Position from = entity.Position.Equals(Position.Origin) ? packet.Position : entity.Position;
            Position to = packet.Position;
            
            entity.Position = to;
            entity.Speed = packet.Speed;

            _eventPipeline.Emit(new EntityMoveEvent(client, entity, from, to));
            
            Logger.Trace($"Entity {entity.EntityType} with id {entity.Id} moved to {entity.Position}");
        }
    }
}