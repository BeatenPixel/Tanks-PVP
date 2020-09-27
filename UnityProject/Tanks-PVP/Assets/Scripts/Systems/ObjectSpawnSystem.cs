using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(ObjectSpawnSystem))]
public sealed class ObjectSpawnSystem : UpdateSystem {

    public BlockProvider basicBlockProvider;
    public AnimatedBlockProvider animatedBlockProvider;
    public ProjectileProvider projectileProvider;

    public override void OnAwake() {
        
    }

    public override void OnUpdate(float deltaTime) {
    }

    public BlockProvider InstantiateBlock() {
        BlockProvider newBlock = Instantiate(basicBlockProvider) as BlockProvider;
        return newBlock;
    }

    public Entity InstantiateAnimatedBlock(out BlockAnimatedRenderComponent component) {
        AnimatedBlockProvider newBlock = Instantiate(animatedBlockProvider) as AnimatedBlockProvider;
        component = newBlock.GetData();
        return newBlock.Entity;
    }

    public ProjectileProvider InstantiateBullet() {
        ProjectileProvider newProjectile = Instantiate(projectileProvider) as ProjectileProvider;
        return newProjectile;
    }
    
}