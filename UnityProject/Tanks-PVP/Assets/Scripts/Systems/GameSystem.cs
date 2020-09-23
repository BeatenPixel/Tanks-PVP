using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(GameSystem))]
public sealed class GameSystem : UpdateSystem {

    public MapSystem map;
    public BlockSpawnSystem blockSpawnSystem;

    public ArtManager artManager;
    
    public override void OnAwake() {
        Debug.Log("1");
        map.Initialize(blockSpawnSystem,artManager);
    }

    public override void OnUpdate(float deltaTime) {
    }
}