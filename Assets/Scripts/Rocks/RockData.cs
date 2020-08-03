using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct RockData : IComponentData
{
    public float3 planetUpDirection;
    public float3 pivot;
    public float spinSpeed;
    public float planetMass;
    //part2
    public bool isControlled;
    public bool explode;
    public float3 explosionPosition;
}
