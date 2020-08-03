using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;
using Unity.Physics;
using Unity.Physics.Extensions;

public class MoveRockOnExplosionSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		float deltaTime = Time.DeltaTime;
		var jobHandle = Entities
			   .WithName("MoveRockOnExplosionSystem")
			   .ForEach((ref Translation position, ref RockData rockData, ref PhysicsVelocity velocity) =>
			   {
				   if (rockData.explode)
				   {
					   var direction = position.Value - rockData.explosionPosition;
					   velocity.Linear = direction * 2f;
					   rockData.explode = false;
					   rockData.isControlled = false;
				   }
			   })
			   .Schedule(inputDeps);

		jobHandle.Complete();

		return jobHandle;
	}

}
