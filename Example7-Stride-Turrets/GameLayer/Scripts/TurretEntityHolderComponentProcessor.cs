using System;
using Stride.Core.Mathematics;
using Stride.Engine;
using Svelto.ECS.MiniExamples.Turrets.EnemyLayer;
using Svelto.ECS.MiniExamples.Turrets.PhysicLayer;
using Svelto.ECS.MiniExamples.Turrets.StrideLayer;

namespace Svelto.ECS.MiniExamples.Turrets
{
    /// <summary>
    /// This is a naive and super focused example on how to possibly create factories in Stride that automatically
    /// convert Stride Entities into Svelto Entities. I didn't have the time and it wasn't in the scope to make
    /// this more generic, but it's surely possible to do so. Current version is very dedicated to the demo purposes.
    /// </summary>
    public class TurretEntityHolderComponentProcessor : EntityProcessor<TurretEntityHolder>
    {
        protected override void OnSystemAdd()
        {
            _entityFactory = Services.GetService<IEntityFactory>();
            _ecsManager    = Services.GetService<ECSStrideEntityManager>();
        }

        protected override void OnEntityComponentAdding
            (Entity turretStrideEntity, TurretEntityHolder turretStrideEntityComponent, TurretEntityHolder data)
        {
            var botStrideEntityTransform    = turretStrideEntityComponent.child.Transform;
            var turretStrideEntityTransform = turretStrideEntity.Transform;

            turretStrideEntityTransform.UpdateWorldMatrix();
            botStrideEntityTransform.UpdateWorldMatrix();

            var turretWorldMatrix    = turretStrideEntityTransform.WorldMatrix;
            var turretBotWorldMatrix = botStrideEntityTransform.WorldMatrix;

            turretWorldMatrix.Decompose(out Vector3 scaleA, out Quaternion rotationA, out Vector3 translationA);
            turretWorldMatrix.Invert();
            turretBotWorldMatrix *= turretWorldMatrix;
            turretBotWorldMatrix.Decompose(out Vector3 scaleB, out Quaternion rotationB, out Vector3 translationB);
            
            turretStrideEntityTransform.UseTRS = false;
            botStrideEntityTransform.UseTRS    = false;
            
            var sveltoEntityID      = _ecsManager.RegisterStrideEntity(turretStrideEntity);
            var sveltoEntityIDChild = _ecsManager.RegisterStrideEntity(turretStrideEntityComponent.child);

            var botInitializer = _entityFactory.BuildEntity(new EGID(sveltoEntityIDChild, BotTag.BuildGroup)
                                                          , turretStrideEntityComponent.GetChildDescriptor());
            var turretInitializer = _entityFactory.BuildEntity(new EGID(sveltoEntityID, TurretTag.BuildGroup)
                                                             , turretStrideEntityComponent.GetDescriptor());

            botInitializer.Init(new ChildComponent(turretInitializer.reference));
            botInitializer.Init(new PositionComponent(translationB));
            botInitializer.Init(new RotationComponent(rotationB));
            botInitializer.Init(new ScalingComponent(scaleB));
            botInitializer.Init(new DirectionComponent() { vector  = Vector3.UnitX });
            botInitializer.Init(new ShootingComponent()
            {
                randomTime =  _rand.Next(2, 5)
            });
            
            turretInitializer.Init(new StartPositionsComponent(translationA));
            turretInitializer.Init(new PositionComponent(translationA));
            turretInitializer.Init(new RotationComponent(rotationA));
            turretInitializer.Init(new ScalingComponent(scaleA));
        }

        IEntityFactory         _entityFactory;
        ECSStrideEntityManager _ecsManager;
        readonly Random        _rand = new Random();
    }
}