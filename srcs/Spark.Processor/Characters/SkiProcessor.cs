﻿using System.Collections.Generic;
using Spark.Core.Enum;
using Spark.Game.Abstraction;
using Spark.Game.Abstraction.Factory;
using Spark.Packet.Characters;

namespace Spark.Processor.Characters
{
    public class SkiProcessor : PacketProcessor<Ski>
    {
        private readonly ISkillFactory _skillFactory;

        public SkiProcessor(ISkillFactory skillFactory)
        {
            _skillFactory = skillFactory;
        }
        
        protected override void Process(IClient client, Ski packet)
        {
            var skills = new List<ISkill>();

            foreach (int skillGameId in packet.Skills)
            {
                ISkill skill = _skillFactory.CreateSkill(skillGameId);
                if (skill.Category == SkillCategory.Player)
                {
                    skills.Add(skill);
                }
            }

            client.Character.Skills = skills;
        }
    }
}