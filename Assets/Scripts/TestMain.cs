
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Collections;


public class TestMain : MonoBehaviour
{
    void Start()
    {
        foreach(var a in World.All) {
            Test(a.EntityManager);
        }
    }


    void Test(EntityManager manager)
    {

        // Entity が持つ Components を設計（Prefabとして）
        var archetype = manager.CreateArchetype(
            ComponentType.ReadOnly<Prefab>(),
            ComponentType.ReadWrite<LocalToWorld>(),
            ComponentType.ReadWrite<Translation>(),
            ComponentType.ReadOnly<RenderMesh>());

        // 上記の Components を持つ Entity を作成
        var prefab = manager.CreateEntity(archetype);

        // Entity の Component の値をセット（位置）
        manager.SetComponentData(prefab, new Translation() { Value = new float3(0, 3, 0) });

        // キューブオブジェクトの作成
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // Entity の Component の値をセット（描画メッシュ）
        manager.SetSharedComponentData(prefab, new RenderMesh()
        {
            mesh = cube.GetComponent<MeshFilter>().sharedMesh,
            material = cube.GetComponent<MeshRenderer>().sharedMaterial,
            subMesh = 0,
            castShadows = UnityEngine.Rendering.ShadowCastingMode.Off,
            receiveShadows = false
        });

        // キューブオブジェクトの削除
        Destroy(cube);

        const int SIDE = 100;
        using (NativeArray<Entity> entities = new NativeArray<Entity>(SIDE * SIDE, Allocator.Temp, NativeArrayOptions.UninitializedMemory)) {

            // Prefab Entity をベースに 10000 個の Entity を作成
            manager.Instantiate(prefab, entities);

            // 平面に敷き詰めるように Translation を初期化
            for (int x = 0; x < SIDE; x++) {
                for (int z = 0; z < SIDE; z++) {

                    int index = x + z * SIDE;
                    manager.SetComponentData(entities[index], new Translation{
                        Value = new float3(x, 0, z)
                    });
                }
            }
        }
    }
}