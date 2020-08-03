using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public class LaserSystem : JobComponentSystem
{
	BuildPhysicsWorld physWorld;
	EntityManager manager;

	protected override void OnCreate()
	{
		base.OnCreate();
		physWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
		manager = World.DefaultGameObjectInjectionWorld.EntityManager;
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		bool inputSingle = Input.GetKeyDown(KeyCode.E);
		bool inputAll = Input.GetKeyDown(KeyCode.Q);
		if (inputSingle || inputAll)
		{
			var raycastHits = new NativeList<Unity.Physics.RaycastHit>(Allocator.TempJob);
			var laserData = manager.GetComponentData<LaserData>(DataManager.instance.playerEntity);
			var transData = manager.GetComponentData<Translation>(DataManager.instance.playerEntity);
			var rotData = manager.GetComponentData<Rotation>(DataManager.instance.playerEntity);
			var collectAll = inputAll;
			JobHandle jobHandle = new RaycastHitJob
			{
				raycastHits = raycastHits,
				physWorld = physWorld.PhysicsWorld,
				laserData = laserData,
				rotData = rotData,
				transData = transData,
				collectAll = collectAll
			}.
			Schedule();
			jobHandle.Complete();

			foreach (var hit in raycastHits)
			{
				var rockData = manager.GetComponentData<RockData>(hit.Entity);
				var translationData = manager.GetComponentData<Translation>(hit.Entity);
				rockData.explode = true;
				rockData.explosionPosition = translationData.Value - math.forward(rotData.Value) * 100f;
				manager.SetComponentData(hit.Entity, rockData);
			}
			var instance = manager.Instantiate(laserData.laserEntity);
			EntityManager.SetComponentData(instance, new Translation
			{
				Value = transData.Value + math.mul(rotData.Value, new float3(0, 0, 3))
			});
			EntityManager.SetComponentData(instance, new Rotation { Value = rotData.Value });
			raycastHits.Dispose();
			return jobHandle;

		}
		return inputDeps;
	}

	struct RaycastHitJob : IJob
	{
		public NativeList<Unity.Physics.RaycastHit> raycastHits;
		[ReadOnly] public PhysicsWorld physWorld;
		public LaserData laserData;
		public Translation transData;
		public Rotation rotData;
		public bool collectAll;
		public void Execute()
		{			
			var raycastInput = new RaycastInput
			{
				Start = transData.Value,
				End = transData.Value + math.forward(rotData.Value) * laserData.distance,
				Filter = CollisionFilter.Default
			};
			if (collectAll)
			{
				physWorld.CastRay(raycastInput, ref raycastHits);
			}
			else
			{
				if (physWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit hit))
				{
					raycastHits.Add(hit);
				}
			}
		}
	}
}
