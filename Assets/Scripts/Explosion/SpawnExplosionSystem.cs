using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class SpawnExplosionSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{

		Entities.WithoutBurst().WithStructuralChanges().
		  ForEach((ref Entity entity, ref Translation position, ref Rotation rotation,
			ref RocketData rocketData, ref LifeTimeData lifeData) =>
		  {
			  if (rocketData.spawnExplosion)
			  {
				  var instance = EntityManager.Instantiate(rocketData.explosionVolume);
				  EntityManager.SetComponentData(instance, new Translation { Value = position.Value });
				  EntityManager.SetComponentData(instance, new Rotation { Value = rotation.Value });
				  EntityManager.DestroyEntity(entity);
			  }
		  })
		  .Run();

		return inputDeps;
	}
}
