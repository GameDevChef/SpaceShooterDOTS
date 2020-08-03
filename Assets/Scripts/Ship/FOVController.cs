using Unity.Entities;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FOVController : MonoBehaviour
{
	public float fovDelta = 20f;
	private Entity EntityToTrack = Entity.Null;
	private float originalFov;
	private Camera cam;

	private void Awake()
	{
		cam = Camera.main;
		originalFov = cam.fieldOfView;
	}

	public void SetReceivedEntity(Entity entity)
	{
		EntityToTrack = entity;
	}

	void LateUpdate()
	{
		if (EntityToTrack != Entity.Null)
		{
			var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
			var throttle = entityManager.GetComponentData<PlayerShipData>(EntityToTrack).throttle;
			var delta = throttle * fovDelta;
			cam.fieldOfView = originalFov + delta;
		}
	}
}
