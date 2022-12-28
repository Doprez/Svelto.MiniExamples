using System.Collections;
using Svelto.Common;
using Svelto.ECS.Example.Survive.Damage;
using Svelto.ECS.Example.Survive.OOPLayer;
using AudioType = Svelto.ECS.Example.Survive.Damage.AudioType;

namespace Svelto.ECS.Example.Survive.Enemies
{
    [Sequenced(nameof(EnemyEnginesNames.EnemySpawnEffectOnDamage))]
    public class EnemySpawnEffectOnDamageEngine: IQueryingEntitiesEngine, IStepEngine
    {
        public EnemySpawnEffectOnDamageEngine(IEntityStreamConsumerFactory consumerFactory)
        {
            //this consumer will process only changes from DamageableComponent published from the EnemiesGroup
            _consumerHealth = consumerFactory.GenerateConsumer<DamageableComponent>("EnemyAnimationEngine", 15);
            _checkForEnemyDamage = SpawnEffectOnDamage();
        }

        public void Step()
        {
            _checkForEnemyDamage.MoveNext();
        }
        public string name => nameof(EnemySpawnEffectOnDamageEngine);

        public EntitiesDB entitiesDB { set; private get; }

        public void Ready() { }

        IEnumerator SpawnEffectOnDamage() 
        {
            void CheckDamageEnemy(EGID egid, DamageableComponent component)
            {
                entitiesDB.QueryEntity<DamageSoundComponent>(egid).playOneShot =
                        (int)AudioType.damage;
                ref var enemyEntityViewsStructs = ref entitiesDB.QueryEntity<VFXComponent>(egid);

                enemyEntityViewsStructs.vfxEvent = new VFXEvent(component.damageInfo.damagePoint);
            }

            while (true)
            {
                while (_consumerHealth.TryDequeue(out var component, out var egid))
                {
                    //publisher/consumer pattern will be replaces with better patterns in future for these cases.
                    //The problem is obvious, DeathComponent is abstract and could have came from the player
                    if (EnemiesGroup.Includes(egid.groupID))
                    {
                        CheckDamageEnemy(egid, component);
                    }
                }

                yield return null;
            }
        }

        readonly IEnumerator          _checkForEnemyDamage;
        Consumer<DamageableComponent> _consumerHealth;
    }
}