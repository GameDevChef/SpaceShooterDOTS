using Unity.Entities;

[GenerateAuthoringComponent]
public struct PlayerShipData : IComponentData
{
    public float rotationSpeed;
    public float moveSpeed;
    public float throttle;
    public float currentRotationY;
    public float currentRotationX;
    public float currentRotationZ;
    public float currentFireWaitTime;
    public float throttleSpeed;
    public float fireRate;
    public Entity bulletEntityPrefab;
    public Entity triggerBulletEntityPrefab;

}
