
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Collections;
using System.Linq;
using Hash128 = Unity.Entities.Hash128;
using UnityEngine.UIElements;
using System.Runtime.CompilerServices;


public class TestWorld : MonoBehaviour
{
    [SerializeField] Mesh _mesh;
    [SerializeField] Material _mat;

 
    void Start()
    {
#if true
        var world = new World("Test");
        var initGroup = world.GetOrCreateSystem(typeof(InitializationSystemGroup)) as InitializationSystemGroup;
        var simurationGroup = world.GetOrCreateSystem(typeof(SimulationSystemGroup)) as SimulationSystemGroup;
        var presentationGroup = world.GetOrCreateSystem(typeof(PresentationSystemGroup)) as PresentationSystemGroup;

        // initialize
        var convertEntitySystem = world.GetOrCreateSystem(typeof(ConvertToEntitySystem));
        initGroup.AddSystemToUpdateList(convertEntitySystem);
        var worldTimeSystem = world.GetOrCreateSystem(typeof(UpdateWorldTimeSystem));
        initGroup.AddSystemToUpdateList(worldTimeSystem);
        
        // presentation group
        var renderSystem = world.GetOrCreateSystem(typeof(RenderMeshSystemV2));
        presentationGroup.AddSystemToUpdateList(renderSystem);
        var commandBufferSystem = world.GetOrCreateSystem(typeof(BeginPresentationEntityCommandBufferSystem));
        presentationGroup.AddSystemToUpdateList(commandBufferSystem);
//        var renderBoundsSystem = world.GetOrCreateSystem(typeof(Unity.Rendering.))

        // simuration group
        var testSystem = world.GetOrCreateSystem(typeof(NoiseHeightSystem));
        simurationGroup.AddSystemToUpdateList(testSystem);
        var commandBufferSys = world.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        simurationGroup.AddSystemToUpdateList(commandBufferSys);

        var transformGroup = world.GetOrCreateSystem(typeof(TransformSystemGroup)) as TransformSystemGroup;
        var worldToLocalSystem = world.GetOrCreateSystem(typeof(EndFrameWorldToLocalSystem));
        transformGroup.AddSystemToUpdateList(worldToLocalSystem);
        var transformLtoWSystem = world.GetOrCreateSystem(typeof(EndFrameTRSToLocalToWorldSystem));
        transformGroup.AddSystemToUpdateList(transformLtoWSystem);
        simurationGroup.AddSystemToUpdateList(transformGroup);

        World.DefaultGameObjectInjectionWorld = world;
        ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);
 #else

        var world = World.DefaultGameObjectInjectionWorld;
 #endif

        SetIsEnableWorld(world, true);
        Setup(world);
    }
    
    public void SetIsEnableWorld(World world, bool isEnable)
    {
        if (world == null) {
            return;
        }
        
        foreach(var system in world.Systems) {
            system.Enabled = isEnable;
        }
    }
    
    void Setup(World world)
    {
        var manager = world.EntityManager;

        var archeType = manager.CreateArchetype(
            typeof(Prefab),
            typeof(RenderBounds),
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld));
        var prefab = manager.CreateEntity(archeType);
        
        manager.SetSharedComponentData(prefab, new RenderMesh()
        {
            mesh = _mesh,
            material = _mat,
            subMesh = 0,
            castShadows = UnityEngine.Rendering.ShadowCastingMode.Off,
            receiveShadows = false
        });

        // create. 100 * 100 tile entities
        const int SIZE = 100;
        using (NativeArray<Entity> entityList = new NativeArray<Entity>(SIZE * SIZE, Allocator.Temp)) {

            manager.Instantiate(prefab, entityList);

            for (int x = 0; x < SIZE; x++) {
                for (int z = 0; z < SIZE; z++) {

                    int index = x + z * SIZE;
                    manager.SetComponentData(entityList[index], new Translation {
                        Value = new float3(x, 0, z)
                    });
                }
            }
        }
    }
}