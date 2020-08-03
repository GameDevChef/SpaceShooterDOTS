using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class RockMoveSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		float deltaTime = Time.DeltaTime;

		float gravityConstant = DataManager.instance.gravityConstant;
		float simulationScale = DataManager.instance.simulationScale;

		var jobHandle = Entities
			   .WithName("RockMoveSystem")
			   .ForEach((ref Translation position, ref Rotation rotation, ref RockData rockData) =>
			   {
				   if (!rockData.isControlled)
					   return;
				   var orbitSpeed = deltaTime * math.sqrt(
					   gravityConstant * rockData.planetMass / math.distance(position.Value, rockData.pivot)) * simulationScale;
				   position.Value = math.mul(Quaternion.AngleAxis(orbitSpeed, rockData.planetUpDirection), position.Value - rockData.pivot) + rockData.pivot;
				   rotation.Value = math.mul(rotation.Value, quaternion.RotateY(rockData.spinSpeed * deltaTime));
			   })
			   .Schedule(inputDeps);
		return jobHandle;
	}
}
