﻿using Spark.Event;
using Spark.Event.Login;
using Spark.Game.Abstraction;
using Spark.Packet.CharacterSelector;

namespace Spark.Packet.Processor.CharacterSelector
{
    public class OkProcessor : PacketProcessor<Ok>
    {
        private readonly IEventPipeline _eventPipeline;

        public OkProcessor(IEventPipeline eventPipeline) => _eventPipeline = eventPipeline;

        protected override void Process(IClient client, Ok packet)
        {
            client.SendPacket("game_start");
            _eventPipeline.Emit(new GameStartEvent(client));
        }
    }
}