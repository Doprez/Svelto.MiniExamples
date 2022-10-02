﻿using Svelto.ECS.Example.Survive.Camera;
//using Svelto.ECS.Example.Survive.Enemies;

namespace Svelto.ECS.Example.Survive.Player
{
    public class PlayerEntityDescriptor : ExtendibleEntityDescriptor<DamageableEntityDescriptor>
    {
        public PlayerEntityDescriptor()
        {
            ExtendWith(new IComponentBuilder[]
            {
                new ComponentBuilder<PlayerEntityComponent>()
              , new ComponentBuilder<CameraTargetEntityViewComponent>()
             // , new ComponentBuilder<EnemyTargetEntityViewComponent>()
              , new ComponentBuilder<PlayerInputDataComponent>()
                , new ComponentBuilder<AnimationComponent>()
//              , new ComponentBuilder<PlayerWeaponComponent>()
  //            , new ComponentBuilder<SpeedComponent>()
              , new ComponentBuilder<CameraReferenceComponent>()
            });
        }
    }
}
