﻿
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class NoiseHeightSystem : JobComponentSystem
{

    [BurstCompile]
    struct TranslationNoise : IJobForEach<Translation>
    {
        public float time;

        public void Execute(ref Translation translation)
        {
            translation.Value.y = noise.snoise(new float2(time + 0.01f * translation.Value.x, time + 0.01f * translation.Value.z));
        }
    }

    protected override void OnCreate()
    {
        base.OnCreate();

        this.query = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadWrite<Translation>() },
        });
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new TranslationNoise() { time = UnityEngine.Time.realtimeSinceStartup };
        return job.Schedule(this, inputDeps);
    }

    EntityQuery query;
}

