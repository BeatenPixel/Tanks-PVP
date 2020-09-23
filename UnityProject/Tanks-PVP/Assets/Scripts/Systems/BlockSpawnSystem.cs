using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(BlockSpawnSystem))]
public sealed class BlockSpawnSystem : UpdateSystem {

    public BlockProvider basicBlockProvider;
    
    public override void OnAwake() {
        
    }

    public override void OnUpdate(float deltaTime) {
    }

    public Entity InstantiateBlock() {
        BlockProvider newBlock = Instantiate(basicBlockProvider) as BlockProvider;
        //Morpeh.Providers.ObjectsProvider.
        return newBlock.Entity;
    }
    
}