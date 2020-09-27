using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(PlayerMovementSystem))]
public sealed class PlayerMovementSystem : UpdateSystem {

    private Filter localPlayerFilter;
    private Filter collidableBlocksFilter;
    private Filter mapFilter;

    private MapSystem mapSystem;

    public override void OnAwake() {
        localPlayerFilter = World.Filter.With<PlayerComponent>().With<LocalPlayerTag>();
        collidableBlocksFilter = World.Filter.With<BlockCollidableComponent>();
        mapFilter = World.Filter.With<MapComponent>();
    }

    public void Initialize(MapSystem _mapSystem) {
        mapSystem = _mapSystem;
    }

    public override void OnUpdate(float deltaTime) {
        Entity localPlayerEnt = localPlayerFilter.First();

        ref PlayerComponent playerComponent = ref localPlayerEnt.GetComponent<PlayerComponent>();
        ref TankComponent tankComponent = ref localPlayerEnt.GetComponent<TankComponent>();

        float newPosX = tankComponent.x, newPosY = tankComponent.y;

        if(playerComponent.inputX != 0) {
            newPosX += tankComponent.moveSpeed * deltaTime * playerComponent.inputX;
            tankComponent.faceDirection = (byte)((playerComponent.inputX == 1) ? 1 : 3);
        } else if (playerComponent.inputY != 0) {
            newPosY += tankComponent.moveSpeed * deltaTime * playerComponent.inputY;
            tankComponent.faceDirection = (byte)((playerComponent.inputY == 1) ? 0 : 2);
        }


        CollisionInfo collision;

        if(!mapSystem.CheckCollision(
            new Vector2(newPosX, newPosY),
            new Vector2(tankComponent.halfSize*2,tankComponent.halfSize*2),
            BlockCollisionLayer.TANK, out collision)) {
            tankComponent.x = newPosX;
            tankComponent.y = newPosY;
        }
        
    }    

}