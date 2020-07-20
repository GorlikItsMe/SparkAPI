﻿using System;
using System.Collections.Generic;
using Spark.Core.Option;
using Spark.Game.Abstraction;
using Spark.Game.Abstraction.Entities;
using Spark.Network;

namespace Spark.Game
{
    public sealed class Client : IClient
    {
        private INetwork _network;

        public Client(INetwork network)
        {
            Id = Guid.NewGuid();
            Network = network;
            Options = new Dictionary<Type, object>();
        }

        public Dictionary<Type, object> Options { get; }

        public INetwork Network
        {
            get => _network;
            set
            {
                if (_network != null)
                {
                    _network.PacketReceived -= ProcessPacket;
                }

                _network = value;
                _network.PacketReceived += ProcessPacket;
            }
        }

        public Guid Id { get; }
        public ICharacter Character { get; set; }

        public event Action<string> PacketReceived;

        public void SendPacket(string packet)
        {
            Network.SendPacket(packet);
        }

        public T GetOption<T>() where T : IOption => (T)Options.GetValueOrDefault(typeof(T));

        public void AddOption<T>(T storage) where T : IOption
        {
            Options[typeof(T)] = storage;
        }

        private void ProcessPacket(string packet)
        {
            PacketReceived?.Invoke(packet);
        }

        public bool Equals(IClient other) => other != null && other.Id.Equals(Id);
    }
}