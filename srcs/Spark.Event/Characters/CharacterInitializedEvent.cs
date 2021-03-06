﻿using Spark.Game.Abstraction;
using Spark.Game.Abstraction.Entities;

namespace Spark.Event.Characters
{
    public class CharacterInitializedEvent : IEvent
    {
        public CharacterInitializedEvent(IClient client, ICharacter character)
        {
            Character = character;
            Client = client;
        }

        public ICharacter Character { get; }

        public IClient Client { get; }
    }
}