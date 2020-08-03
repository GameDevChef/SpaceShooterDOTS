using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;

public class MoveProjectileSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		float deltaTime = Time.DeltaTime;
		var jobHandle = Entities
			   .WithName("MoveBulletSystem")
			   .ForEach((ref Translation position, ref Rotation rotation, ref ProjectileData projectileData) =>
			   {
				   position.Value += math.forward(rotation.Value) * deltaTime * projectileData.speed;
			   })
			   .Schedule(inputDeps);

		jobHandle.Complete();

		return jobHandle;
	}

}
