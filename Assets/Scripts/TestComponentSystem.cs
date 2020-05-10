
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

#if false
public class TestComponentSystem : ComponentSystem
{
    protected override void OnCreate()
    {
        base.OnCreate();

        this.query = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadWrite<Translation>() },
        });
    }

    protected override void OnUpdate()
    {
        var time = UnityEngine.Time.deltaTime;

        Entities.ForEach((ref Translation translation) => {
            translation.Value.y = 3 * noise.snoise(new float2(time + 0.02f * translation.Value.x, time + 0.02f * translation.Value.z));
        });
    }

    EntityQuery query;
}
#endif