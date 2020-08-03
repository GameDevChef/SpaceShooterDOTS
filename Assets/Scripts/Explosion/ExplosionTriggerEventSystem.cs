using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class ExplosionTriggerEventSystem : JobComponentSystem
{
	BuildPhysicsWorld physWorld;
	StepPhysicsWorld stepWorld;

	protected override void OnCreate()
	{
		base.OnCreate();
		physWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
		stepWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
	}

	struct ExplosionTriggerJob : ITriggerEventsJob
	{
		public ComponentDataFromEntity<RockData> rockGroup;
		[ReadOnly] public ComponentDataFromEntity<ExplosionVolumeData> explosionData;
		public ComponentDataFromEntity<Translation> translationGroup;

		public void Execute(TriggerEvent triggerEvent)
		{
			Entity entityA = triggerEvent.Entities.EntityA;
			Entity entityB = triggerEvent.Entities.EntityB;

			bool isRockA = rockGroup.Exists(entityA);
			bool isRockB = rockGroup.Exists(entityB);

			bool isExplosionA = explosionData.Exists(entityA);
			bool isExplosionB = explosionData.Exists(entityB);

			bool isTransA = translationGroup.Exists(entityA);
			bool isTransB = translationGroup.Exists(entityB);

			if (isRockA && isExplosionB && isTransB)
			{
				var rockComponent = rockGroup[entityA];
				rockComponent.explode = true;
				rockComponent.explosionPosition = translationGroup[entityB].Value;
				rockGroup[entityA] = rockComponent;
			}

			if (isExplosionA && isRockB && isTransA)
			{
				var rockComponent = rockGroup[entityB];
				rockComponent.explode = true;
				rockComponent.explosionPosition = translationGroup[entityA].Value;
				rockGroup[entityB] = rockComponent;
			}			
		}
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		JobHandle jobHandle = new ExplosionTriggerJob
		{
			explosionData = GetComponentDataFromEntity<ExplosionVolumeData>(),
			rockGroup = GetComponentDataFromEntity<RockData>(),
			translationGroup = GetComponentDataFromEntity<Translation>(),
		}
		.Schedule(stepWorld.Simulation, ref physWorld.PhysicsWorld, inputDeps);
		jobHandle.Complete();
		return jobHandle;

	}
}

