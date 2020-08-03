using Unity.Entities;

[GenerateAuthoringComponent]
public struct LaserData : IComponentData
{
    public float distance;
    public float range;
    public Entity laserEntity;
}
