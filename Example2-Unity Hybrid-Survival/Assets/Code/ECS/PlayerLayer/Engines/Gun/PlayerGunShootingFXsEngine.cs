using System.Collections;

namespace Svelto.ECS.Example.Survive.Player.Gun
{
    public class PlayerGunShootingFXsEngine : IQueryingEntitiesEngine, IStepEngine
    {
        public EntitiesDB entitiesDB { set; private get; }

        public PlayerGunShootingFXsEngine(IEntityStreamConsumerFactory factory)
        {
            _factory = factory;
        }

        public void Ready()
        {
            _playerHasShot = PlayerHasShot();
        }

        public void Step()
        {
            _playerHasShot.MoveNext();
        }

        public string name => nameof(PlayerGunShootingFXsEngine);

        IEnumerator PlayerHasShot()
        {
            void SetValues(GunAttributesComponent gunComponent, out float waitTime,
                ref GunEntityComponent gunFXComponent)
            {
                // Play the gun shot audioclip.
                gunFXComponent.playAudio = true;

                // Enable the light.
                gunFXComponent.lightEnabled = true;

                // Stop the particles from playing if they were, then start the particles.
                gunFXComponent.play = false;
                gunFXComponent.play = true;

                var shootRay = gunFXComponent.shootRay;

                // Enable the line renderer and set it's first position to be the end of the gun.
                gunFXComponent.lineEnabled       = true;
                gunFXComponent.lineStartPosition = shootRay.origin;
                gunFXComponent.lineEndPosition   = gunComponent.lastTargetPosition;

                waitTime = gunComponent.timeBetweenBullets * gunFXComponent.effectsDisplayTime;
            }

            var consumer                 = _factory.GenerateConsumer<GunAttributesComponent>("GunFireConsumer", 1);
            var waitForSecondsEnumerator = new WaitForSecondsEnumerator(0);

            while (true)
            {
                //Consume the entity change sent from PlayerGunShootingEngine
                while (consumer.TryDequeue(out var gunComponent, out var egid))
                {
                    SetValues(gunComponent, out var wait, ref entitiesDB.QueryEntity<GunEntityComponent>(egid));

                    waitForSecondsEnumerator.Reset(wait);
                    while (waitForSecondsEnumerator.MoveNext())
                        yield return null;
                    // ... disable the effects.
                    DisableEffects(ref entitiesDB.QueryEntity<GunEntityComponent>(egid));
                }

                yield return null;
            }
        }

        void DisableEffects(ref GunEntityComponent gunFXComponent)
        {
            // Disable the line renderer and the light.
            gunFXComponent.lineEnabled  = false;
            gunFXComponent.lightEnabled = false;
            gunFXComponent.play         = false;
        }

        readonly IEntityStreamConsumerFactory _factory;
        IEnumerator                           _playerHasShot;
    }
}