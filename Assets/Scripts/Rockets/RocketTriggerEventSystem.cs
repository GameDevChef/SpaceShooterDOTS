using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class RocketTriggerEventSystem : JobComponentSystem
{
	BuildPhysicsWorld physWorld;
	StepPhysicsWorld stepWorld;

	protected override void OnCreate()
	{
		base.OnCreate();
		physWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
		stepWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
	}

	struct RocketTriggerJob : ITriggerEventsJob
	{
		public ComponentDataFromEntity<RockData> rockGroup;
		public ComponentDataFromEntity<RocketData> rocketGroup;

		public void Execute(TriggerEvent triggerEvent)
		{
			var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
			Entity entityA = triggerEvent.Entities.EntityA;
			Entity entityB = triggerEvent.Entities.EntityB;

			bool isRockA = rockGroup.Exists(entityA);
			bool isRockB = rockGroup.Exists(entityB);

			bool isRocketA = rocketGroup.Exists(entityA);
			bool isRocketB = rocketGroup.Exists(entityB);

			if (isRockA && isRocketB)
			{
				var rocketComponent = rocketGroup[entityB];
				rocketComponent.spawnExplosion = true;
				rocketGroup[entityB] = rocketComponent;
			}

			if (isRocketA && isRockB)
			{
				var rocketComponent = rocketGroup[entityA];
				rocketComponent.spawnExplosion = true;
				rocketGroup[entityA] = rocketComponent;
			}
		}
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		JobHandle jobHandle = new RocketTriggerJob
		{
			rocketGroup = GetComponentDataFromEntity<RocketData>(),
			rockGroup = GetComponentDataFromEntity<RockData>(),
		}
		.Schedule(stepWorld.Simulation, ref physWorld.PhysicsWorld, inputDeps);
		//jobHandle.Complete();
		return jobHandle;

	}
}

