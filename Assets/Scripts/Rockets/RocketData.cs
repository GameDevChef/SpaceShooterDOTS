using Unity.Entities;

[GenerateAuthoringComponent]
public struct RocketData : IComponentData
{
    public Entity explosionVolume;
    public bool spawnExplosion;
}