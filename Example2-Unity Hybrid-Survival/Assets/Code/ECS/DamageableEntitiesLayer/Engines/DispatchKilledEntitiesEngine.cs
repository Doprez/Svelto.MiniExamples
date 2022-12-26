﻿namespace Svelto.ECS.Example.Survive.Damage
{
    /// <summary>
    /// What happens when the health reach 0? The fact the entity "dies" is one consequence. I could have merged
    /// this engine with ApplyDamageEngine, but I decided to split it to show that the consequence of something
    /// happening may not have to happen in the same code. This engine is also taking the responsibility of deciding
    /// if the entity must die or not and will communicate it through the use of the Publisher/Consumer pattern.
    /// </summary>
    public class  DispatchKilledEntitiesEngine : IQueryingEntitiesEngine, IStepEngine
    {
        public void Ready() { }

        public EntitiesDB entitiesDB { set; private get; }

        public void Step()
        {
            foreach (var ((healths, egids, count), fromGroup) in entitiesDB.QueryEntities<HealthComponent>(
                Damageable.Groups))
            {
                for (int i = 0; i < count; ++i)
                    if (healths[i].currentHealth <= 0)
                    {
                        entitiesDB.PublishEntityChange<DeathComponent>(new EGID(egids[i], fromGroup));
                    }
            }
        }

        public string name => nameof(DispatchKilledEntitiesEngine);
    }
}