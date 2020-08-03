using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ShootRocketSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		var input = Input.GetMouseButtonDown(1);
		if (input)
		{
			Entities.WithoutBurst().WithStructuralChanges().
			  ForEach((ref Translation position, ref Rotation rotation, ref PlayerShipData shipData) =>
			  {
				  var instance = EntityManager.Instantiate(shipData.triggerBulletEntityPrefab);
				  EntityManager.SetComponentData(instance, new Translation
				  {
					  Value = position.Value +
					  math.mul(rotation.Value, new float3(0, 0, 2))
				  });

				  EntityManager.SetComponentData(instance, new Rotation { Value = rotation.Value });
				  EntityManager.SetComponentData(instance, new LifeTimeData { lifetime = 5f });
			  })
			  .Run();
		}
		return inputDeps;
	}
}
