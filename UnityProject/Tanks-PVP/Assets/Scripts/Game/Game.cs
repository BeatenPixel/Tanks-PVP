using Morpeh;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public static Game inst;
    public GameState gameState;

    public MapSystem mapSystem;
    public PlayerInputSystem playerInputSystem;
    public TanksRenderSystem tankRenderSystem;
    public TankShootSystem tankShootSystem;
    public PlayerMovementSystem playerMovementSystem;

    public ObjectSpawner objectSpawner;
    public ArtManager artManager;

    public NetworkEventsManager networkEventsManager;

    private Filter playersFilter;

    private void Awake() {
        inst = this;
        gameState = GameState.LOBBY;

        Initialize();
    }

    private void Start() {

    }

    private void Update() {
        
    }

    private void Initialize() {
        playersFilter = World.Default.Filter.With<PlayerComponent>();

        objectSpawner.Initialize();
        artManager.Initialize();
        networkEventsManager.Initialize();

        mapSystem.Initialize();
        tankRenderSystem.Initialize();
        tankShootSystem.Initialize(mapSystem);
        playerMovementSystem.Initialize(mapSystem);

        SpawnLocalPlayer();
    }

    private void OnDisable() {
        networkEventsManager.Deinitialize();
    }

    private void SpawnLocalPlayer() {
        PlayerProvider playerProvider = ObjectSpawner.inst.InstantiatePlayer();
        ref PlayerComponent pc = ref playerProvider.Entity.GetComponent<PlayerComponent>();
        playerProvider.Entity.AddComponent<LocalPlayerTag>();
    }


}

public enum GameState {
    NONE,
    LOBBY,
    SEARCHING_GAME,
    WAITING_IN_ROOM,
    LAUNCHING,
    RUNNING,
    MATCH_END
}
