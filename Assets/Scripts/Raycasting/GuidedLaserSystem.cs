using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public class GuidedLaserSystem : JobComponentSystem
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
		bool inputSingle = Input.GetKeyDown(KeyCode.C);
		bool inputAll = Input.GetKeyDown(KeyCode.Z);
		if (inputSingle || inputAll)
		{
			var distanceHits = new NativeList<Unity.Physics.DistanceHit>(Allocator.TempJob);
			var laserData = manager.GetComponentData<LaserData>(DataManager.instance.playerEntity);
			var transData = manager.GetComponentData<Translation>(DataManager.instance.playerEntity);
			var collectAll = inputAll;
			JobHandle jobHandle = new DistanceHitJob
			{
				distanceHits = distanceHits,
				physWorld = physWorld.PhysicsWorld,
				laserData = laserData,
				transData = transData,
				collectAll = collectAll
			}.
			Schedule();
			jobHandle.Complete();
			foreach (var hit in distanceHits)
			{
				manager.DestroyEntity(hit.Entity);
				var instance = manager.Instantiate(laserData.laserEntity);
				EntityManager.SetComponentData(instance, new Translation { Value = transData.Value });
				var direction = hit.Position - transData.Value;
				var rotation = quaternion.LookRotation(direction, math.up());
				EntityManager.SetComponentData(instance, new Rotation { Value = rotation });
			}
			distanceHits.Dispose();
			return jobHandle;

		}
		return inputDeps;
	}

	struct DistanceHitJob : IJob
	{
		public NativeList<Unity.Physics.DistanceHit> distanceHits;
		[ReadOnly] public PhysicsWorld physWorld;
		public LaserData laserData;
		public Translation transData;
		public bool collectAll;
		public void Execute()
		{			
			var pointDistanceInput = new PointDistanceInput
			{
				Position = transData.Value,
				MaxDistance = laserData.range,
				Filter = CollisionFilter.Default
			};
			if (collectAll)
			{
				physWorld.CalculateDistance(pointDistanceInput, ref distanceHits);
			}
			else
			{
				if (physWorld.CalculateDistance(pointDistanceInput, out Unity.Physics.DistanceHit hit))
				{
					distanceHits.Add(hit);
				}
			}


		}
	}
}
