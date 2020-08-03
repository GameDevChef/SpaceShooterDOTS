using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
	EntityManager manager;
	public EntityTracker tracker;
	public FOVController fovController;
	public GameObject[] rockPrefabs;
	public GameObject bulletPrefab;
	public GameObject rocketPrafeb;
	public List<Entity> rockEntities;
	public int numOfRocks = 100;
	public Transform[] planetTransforms;
	public float circeRange;
	public float circleHeight;
	public float minDistanceFromPlanet;
	public float planetMinMass;
	public float planetMaxMass;
	public float minRockSpinSpeed;
	public float maxRockSpinSpeed;
	BlobAssetStore blob;

	private void Start()
	{
		var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blob);

		var playerQuery = manager.CreateEntityQuery(ComponentType.ReadOnly<PlayerShipData>());
		var playerEntities = playerQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);
		var playerEntity = playerEntities[0];

		DataManager.instance.playerEntity = playerEntity;
		tracker.SetReceivedEntity(playerEntity);
		fovController.SetReceivedEntity(playerEntity);

		var bulletEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(bulletPrefab, settings);
		var triggerbulletEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(rocketPrafeb, settings);
		var playerData = manager.GetComponentData<PlayerShipData>(playerEntity);

		playerData.bulletEntityPrefab = bulletEntity;
		playerData.triggerBulletEntityPrefab = triggerbulletEntity;
		manager.SetComponentData(playerEntity, playerData);


		playerEntities.Dispose();
	}

	void Awake()
	{
		InitManager();
		for (int i = 0; i < planetTransforms.Length; i++)
		{
			var planetMass = UnityEngine.Random.Range(planetMinMass, planetMaxMass);
			InitializePlanetRing(planetTransforms[i], planetMass);
		}
	}

	private void InitManager()
	{
		manager = World.DefaultGameObjectInjectionWorld.EntityManager;
		blob = new BlobAssetStore();
		var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blob);
		rockEntities = new List<Entity>();
		for (int i = 0; i < rockPrefabs.Length; i++)
		{
			var rockEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(rockPrefabs[i], settings);
			rockEntities.Add(rockEntity);
		}
	}

	private void InitializePlanetRing(Transform planetTransform, float planetMass)
	{
		for (int i = 0; i < numOfRocks; i++)
		{
			//Position
			var rockEntity = SelectRockEntity();
			var instance = manager.Instantiate(rockEntity);
			Vector3 randomPosition = GetRandomRockPosition(planetTransform.position);

			 float3 targetPosition = math.mul(
					  planetTransform.rotation,
					  (float3)randomPosition - (float3)planetTransform.position) + (float3)planetTransform.position;

			manager.SetComponentData(instance, new Translation { Value = targetPosition });

			//Rotation
			float xRot = UnityEngine.Random.Range(0, 360);
			float yRot = UnityEngine.Random.Range(0, 360);
			float zRot = UnityEngine.Random.Range(0, 360);
			var rotation = Quaternion.Euler(new Vector3(xRot, yRot, zRot));
			manager.SetComponentData(instance, new Rotation { Value = new quaternion(rotation.x, rotation.y, rotation.z, rotation.w) });

			//Rock Data
			float spinSpeed = UnityEngine.Random.Range(minRockSpinSpeed, maxRockSpinSpeed);
			manager.SetComponentData(instance, new RockData
			{
				spinSpeed = spinSpeed,
				planetMass = planetMass,
				planetUpDirection = planetTransform.up,
				pivot = planetTransform.position,
				isControlled = true
			});		
		}
	}

	private Entity SelectRockEntity()
	{
		return rockEntities[UnityEngine.Random.Range(0, rockEntities.Count)];
	}

	private Vector3 GetRandomRockPosition(Vector3 planetPoition)
	{
		var localPosition = (Vector3)UnityEngine.Random.insideUnitCircle * circeRange;
		localPosition = new float3(localPosition.x, 0, localPosition.y);
		localPosition.y = UnityEngine.Random.Range(-circleHeight, circleHeight);
		var position = planetPoition + minDistanceFromPlanet * localPosition.normalized + localPosition;
		return position;
	}



	private void OnDestroy()
	{
		blob.Dispose();
	}
}