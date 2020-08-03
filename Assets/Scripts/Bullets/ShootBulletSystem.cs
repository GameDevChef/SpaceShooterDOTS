using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ShootBulletSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		var mouseDown = Input.GetMouseButton(0);

		float dt = Time.DeltaTime;
		Entities.WithoutBurst().WithStructuralChanges().
		   ForEach((ref Translation position, ref Rotation rotation, ref PlayerShipData shipData) =>
		   {
			   shipData.currentFireWaitTime += dt;
			   if (shipData.currentFireWaitTime >= shipData.fireRate && mouseDown)
			   {
				   shipData.currentFireWaitTime = 0;
				   var instance = EntityManager.Instantiate(shipData.bulletEntityPrefab);
				   EntityManager.SetComponentData(instance, new Translation
				   {
					   Value = position.Value +
					   math.mul(rotation.Value, new float3(0, 0, 2))
				   });
				   EntityManager.SetComponentData(instance, new Rotation { Value = rotation.Value });
				   EntityManager.SetComponentData(instance, new LifeTimeData { lifetime = 5f });
			   }
		   })
		   .Run();
		   
		return inputDeps;
	}
}
