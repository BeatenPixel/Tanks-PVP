using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.SceneManagement;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(GameSystem))]
public sealed class GameSystem : UpdateSystem {

    public MapSystem mapSystem;
    public ObjectSpawnSystem objectSpawnSystem;
    public PlayerInputSystem playerInputSystem;
    public TanksRenderSystem tankRenderSystem;
    public TankShootSystem tankShootSystem;
    public PlayerMovementSystem playerMovementSystem;

    public ArtManager artManager;
    public NetworkEventsManager networkEventsManager;

    private Filter playersFilter;

    public override void OnAwake() {
        playersFilter = World.Filter.With<PlayerComponent>();

        networkEventsManager.Initialize();

        SpawnLocalPlayer();

        mapSystem.Initialize(objectSpawnSystem,artManager);
        tankRenderSystem.Initialize(artManager);
        tankShootSystem.Initialize(objectSpawnSystem, mapSystem, artManager);
        playerMovementSystem.Initialize(mapSystem);
    }

    public override void Dispose() {

        networkEventsManager.Deinitialize();
    }

    public override void OnUpdate(float deltaTime) {
        mapSystem.OnUpdate(deltaTime);
        playerInputSystem.OnUpdate(deltaTime);

        if(Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(1);
        }
    }

    private void SpawnLocalPlayer() {
        PlayerProvider playerProvider = objectSpawnSystem.InstantiatePlayer();
        ref PlayerComponent pc = ref playerProvider.Entity.GetComponent<PlayerComponent>();
        playerProvider.Entity.AddComponent<LocalPlayerTag>();
    }

}