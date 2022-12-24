﻿using Svelto.DataStructures;

namespace Svelto.ECS.Example.Survive.OOPLayer
{
    /// <summary>
    /// The point of the OOPLayer is to encapsulate and abstract the use of objects. This is achievable through
    /// sync points. All the engines in this context are sync engines between entities and objects. Sync
    /// can be bi-directional.
    /// 
    /// </summary>
    public static class OOPLayerContext
    {
        public static void Setup(FasterList<IStepEngine> orderedEngines, EnginesRoot enginesRoot,
            GameObjectResourceManager gameObjectResourceManager)
        {
            var syncEntitiesToObjectsGroup = new SyncOOPEnginesGroup();

            IStepEngine syncEngine = null;

            syncEngine = new SyncCameraEntitiesToObjects(gameObjectResourceManager);
            enginesRoot.AddEngine(syncEngine);
            syncEntitiesToObjectsGroup.Add(syncEngine);
            
            syncEngine = new SyncEntitiesAnimationsToObjects(gameObjectResourceManager);
            enginesRoot.AddEngine(syncEngine);
            syncEntitiesToObjectsGroup.Add(syncEngine);
            
            syncEngine = new SyncEntitiesPositionToObjects(gameObjectResourceManager);
            enginesRoot.AddEngine(syncEngine);
            syncEntitiesToObjectsGroup.Add(syncEngine);
            
            syncEngine = new SyncPhysicEntitiesToObjects(gameObjectResourceManager);
            enginesRoot.AddEngine(syncEngine);
            syncEntitiesToObjectsGroup.Add(syncEngine);
            
            syncEngine = new SyncGunEntitiesToObjects(gameObjectResourceManager);
            enginesRoot.AddEngine(syncEngine);
            syncEntitiesToObjectsGroup.Add(syncEngine);
            
            var syncObjectsToEntities = new SyncPositionObjectsToEntities(gameObjectResourceManager);
            enginesRoot.AddEngine(syncObjectsToEntities);
            
            orderedEngines.Add(syncEntitiesToObjectsGroup);
            orderedEngines.Add(syncObjectsToEntities);
        }
    }
}