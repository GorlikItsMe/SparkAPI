﻿using Spark.Database;
using Spark.Database.Data;
using Spark.Game.Abstraction;
using Spark.Game.Abstraction.Factory;

namespace Spark.Game.Factory
{
    public class SkillFactory : ISkillFactory
    {
        private readonly IDatabase database;

        public SkillFactory(IDatabase database) => this.database = database;

        public ISkill CreateSkill(int gameId)
        {
            SkillData skillData = database.Skills.GetValue(gameId);
            if (skillData == null)
            {
                return default;
            }

            return new Skill(gameId, skillData);
        }
    }
}