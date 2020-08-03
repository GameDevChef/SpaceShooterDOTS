using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class BulletCollisionEventSystem : JobComponentSystem
{
	BuildPhysicsWorld physWorld;
	StepPhysicsWorld stepWorld;

	protected override void OnCreate()
	{
		base.OnCreate();
		physWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
		stepWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
	}

	struct CollisionEventJob : ICollisionEventsJob
	{
		[ReadOnly] public ComponentDataFromEntity<BulletData> bulletGroup;
		public ComponentDataFromEntity<RockData> rockGroup;


		public void Execute(CollisionEvent collisionEvent)
		{
			var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
			Entity entityA = collisionEvent.Entities.EntityA;
			Entity entityB = collisionEvent.Entities.EntityB;

			bool isBulletA = bulletGroup.Exists(entityA);
			bool isBulletB = bulletGroup.Exists(entityB);

			bool isRockA = rockGroup.Exists(entityA);
			bool isRockB = rockGroup.Exists(entityB);

			if (isBulletA && isRockB)
			{
				var rockComponent = rockGroup[entityB];
				rockComponent.isControlled = false;
				rockGroup[entityB] = rockComponent;
				manager.SetComponentData(entityA, new LifeTimeData { lifetime = 0f });

			}

			if (isBulletB && isRockA)
			{
				var rockComponent = rockGroup[entityA];
				rockComponent.isControlled = false;
				rockGroup[entityA] = rockComponent;
				manager.SetComponentData(entityB, new LifeTimeData { lifetime = 0f });
			}

		}
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		JobHandle jobHandle = new CollisionEventJob
		{
			bulletGroup = GetComponentDataFromEntity<BulletData>(),
			rockGroup = GetComponentDataFromEntity<RockData>(),
		}.
		Schedule(stepWorld.Simulation, ref physWorld.PhysicsWorld, inputDeps);
		jobHandle.Complete();
		return jobHandle;

	}
}
