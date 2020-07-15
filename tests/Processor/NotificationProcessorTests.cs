﻿using Spark.Event.Notification;
using Spark.Packet.Notification;
using Spark.Tests.Attributes;

namespace Spark.Tests.Processor
{
    public class NotificationProcessorTests : ProcessorTests
    {
        [ProcessorTest(typeof(QNamli2))]
        [EventTest(typeof(RaidNotifyEvent))]
        public void QNamli2_Test()
        {
            using (GameContext context = CreateContext())
            {
                context.Process(new QNamli2
                {
                    Raid = new QNamli2.RaidInfo
                    {
                        Owner = "MyNameIs"
                    }
                });
                
                context.Verify<RaidNotifyEvent>(x => x.Owner == "MyNameIs");
            }
        }
    }
}