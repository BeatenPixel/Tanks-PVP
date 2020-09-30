using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(PlayerMovementSystem))]
public sealed class PlayerMovementSystem : UpdateSystem {

    public float syncSpeed = 20;

    private Filter localPlayerFilter;
    private Filter otherPlayersFilter;
    private Filter collidableBlocksFilter;
    private Filter mapFilter;

    private MapSystem mapSystem;

    public override void OnAwake() {
        localPlayerFilter = World.Filter.With<PlayerComponent>().With<LocalPlayerTag>();
        otherPlayersFilter = World.Filter.With<PlayerComponent>().Without<LocalPlayerTag>();

        collidableBlocksFilter = World.Filter.With<BlockCollidableComponent>();
        mapFilter = World.Filter.With<MapComponent>();
    }

    public void Initialize(MapSystem _mapSystem) {
        mapSystem = _mapSystem;
    }

    public override void OnUpdate(float deltaTime) {
        MoveLocalPlayer(deltaTime);
        MoveOtherPlayers(deltaTime);
    }    

    private void MoveOtherPlayers(float deltaTime) {
        foreach (var playerEnt in otherPlayersFilter) {
            ref PlayerComponent playerComponent = ref playerEnt.GetComponent<PlayerComponent>();
            ref TankComponent tankComponent = ref playerEnt.GetComponent<TankComponent>();

            tankComponent.x = Mathf.Lerp(tankComponent.x, tankComponent.receivedPosX[0], deltaTime * syncSpeed);
            tankComponent.y = Mathf.Lerp(tankComponent.y, tankComponent.receivedPosY[0], deltaTime * syncSpeed);
        }
    }

    private void MoveLocalPlayer(float deltaTime) {
        Entity localPlayerEnt = localPlayerFilter.First();

        ref PlayerComponent playerComponent = ref localPlayerEnt.GetComponent<PlayerComponent>();
        ref TankComponent tankComponent = ref localPlayerEnt.GetComponent<TankComponent>();

        float newPosX = tankComponent.x, newPosY = tankComponent.y;

        if (playerComponent.inputX != 0) {
            newPosX += tankComponent.moveSpeed * deltaTime * playerComponent.inputX;
            tankComponent.faceDirection = (byte)((playerComponent.inputX == 1) ? 1 : 3);
        } else if (playerComponent.inputY != 0) {
            newPosY += tankComponent.moveSpeed * deltaTime * playerComponent.inputY;
            tankComponent.faceDirection = (byte)((playerComponent.inputY == 1) ? 0 : 2);
        }

        CollisionInfo collision;

        if (!mapSystem.CheckCollision(
            new Vector2(newPosX, newPosY),
            new Vector2(tankComponent.halfSize * 2, tankComponent.halfSize * 2),
            BlockCollisionLayer.TANK, out collision)) {
            tankComponent.x = newPosX;
            tankComponent.y = newPosY;
        }
    }

}