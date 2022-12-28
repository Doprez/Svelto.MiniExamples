﻿using Svelto.DataStructures;
using Svelto.ECS.Example.Survive.OOPLayer;
using Svelto.ECS.Example.Survive.Player.Gun;

namespace Svelto.ECS.Example.Survive.Player
{
    public static class PlayerLayerContext
    {
        public static void Setup(IRayCaster rayCaster, ITime time, IEntityFunctions entityFunctions,
            IEntityStreamConsumerFactory entityStreamConsumerFactory, FasterList<IStepEngine> unorderedEngines,
            FasterList<IStepEngine> orderedEngines, EnginesRoot enginesRoot)
        {
            //Player related engines. ALL the dependencies must be solved at this point through constructor injection.
            var playerShootingEngine = new PlayerFiresGunEngine(
                rayCaster,
                time,
                GAME_LAYERS.ENEMY_LAYER,
                GAME_LAYERS.SHOOTABLE_MASK, GAME_LAYERS.ENEMY_MASK);
            var playerMovementEngine = new PlayerMovementEngine(rayCaster);
            var playerAnimationEngine = new PlayerAnimationEngine();
            var playerDeathEngine = new PlayerDeathEngine(entityFunctions);
            var playerInputEngine = new PlayerInputEngine();
            var playerGunShootingFXsEngine = new PlayerGunShootingFXsEngine(entityStreamConsumerFactory);

//Player engines
            enginesRoot.AddEngine(playerInputEngine);
            enginesRoot.AddEngine(playerMovementEngine);
            enginesRoot.AddEngine(playerAnimationEngine);
            enginesRoot.AddEngine(playerShootingEngine);
            enginesRoot.AddEngine(playerGunShootingFXsEngine);
            enginesRoot.AddEngine(playerDeathEngine);
    
            unorderedEngines.Add(playerInputEngine);
            unorderedEngines.Add(playerMovementEngine);
            unorderedEngines.Add(playerGunShootingFXsEngine);
            unorderedEngines.Add(playerAnimationEngine);

            orderedEngines.Add(playerShootingEngine);
            orderedEngines.Add(playerDeathEngine);
        }
    }
}