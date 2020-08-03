using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class EntityTracker : MonoBehaviour
{
	private Entity EntityToTrack = Entity.Null;
	public void SetReceivedEntity(Entity entity)
	{
		EntityToTrack = entity;
	}

	void LateUpdate()
	{
		if (EntityToTrack != Entity.Null)
		{

			var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
			transform.position = entityManager.GetComponentData<Translation>(EntityToTrack).Value;
			transform.rotation = entityManager.GetComponentData<Rotation>(EntityToTrack).Value;
		}
	}
}