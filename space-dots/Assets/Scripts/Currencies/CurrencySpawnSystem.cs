using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(PostSimulationSystemGroup))]
public class CurrencySpawnSystem : SystemBase
{
    private CommandBuffers buffers;
    private EntityArchetype spawnArchetype;

    // this should probably use system state component for storage...?
    // but I'm lazy.
    // so let's see if this comes back to annoy me later.
    private const uint RngSeed = 4;
    private Random random;

    protected override void OnCreate()
    {
        base.OnCreate();
        buffers = new CommandBuffers(World);
        spawnArchetype = EntityManager.CreateArchetype(typeof(MoverSpawnRequest));
        random = new Random(RngSeed);
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer buffer = buffers.CreatePostUpdateBuffer();
        EntityArchetype spawnArchetypeCapture = spawnArchetype;
        Random rng = random;

        // If something's dying with XP, drop equivalent XP pickups.
        Entities
            .WithAll<DyingTag>()
            .ForEach((in LinearVelocity velocity, in Translation translation, in CurrencyXP xp) =>
            {
                SpawnXPDrops(xp.Amount, velocity.Value, translation.Value.xy, buffer, spawnArchetypeCapture, rng);
            }).Schedule();

        Entities
            .WithAll<DyingTag>()
            .ForEach((in Translation translation, in CurrencyXP xp) =>
            {
                SpawnXPDrops(xp.Amount, float2.zero, translation.Value.xy, buffer, spawnArchetypeCapture, rng);
            }).Schedule();

        buffers.AddPostUpdateDependency(Dependency);
    }

    private static void SpawnXPDrops(int amount, float2 velocity, float2 translation, EntityCommandBuffer buffer, EntityArchetype spawnArchetypeCapture, Random rng)
    {
        for (int i = 0; i < amount; ++i)
        {
            Entity spawnEntity = buffer.CreateEntity(spawnArchetypeCapture);
            buffer.SetComponent(spawnEntity, CreateSpawnRequest(velocity, translation, rng));
        }
    }

    private static MoverSpawnRequest CreateSpawnRequest(float2 velocity, float2 translation, Random rng)
    {
        return new MoverSpawnRequest
        {
            Rotation = quaternion.RotateZ(rng.NextFloat(0, math.PI * 2f)),
            Position = translation,
            ConfigId = 2,
            Velocity = velocity
        };
    }
}
