using Unity.Entities;
using Unity.Jobs;

[UpdateAfter(typeof(MoveProjectileSystem))]
public class TimedDestroySystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float dt = Time.DeltaTime;
        Entities.WithoutBurst().WithStructuralChanges()
            .ForEach((Entity entity, ref LifeTimeData lifetimeData) =>
            {
                lifetimeData.lifetime -= dt;
                if (lifetimeData.lifetime <= 0f)
                    EntityManager.DestroyEntity(entity);
            })
        .Run();
        return inputDeps;
    }

}