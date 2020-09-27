using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Morpeh.Globals;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(PlayerInputSystem))]
public sealed class PlayerInputSystem : UpdateSystem {

    [SerializeField] private GlobalEventInt onTankShootRequestedEvent;

    private Filter playerFilter;

    public override void OnAwake() {
        playerFilter = World.Filter.With<PlayerComponent>().With<TankComponent>().With<LocalPlayerTag>();
    }

    public override void OnUpdate(float deltaTime) {
        Entity playerEntity = playerFilter.First();

        ref PlayerComponent player = ref playerEntity.GetComponent<PlayerComponent>();
        ref TankComponent tank = ref playerEntity.GetComponent<TankComponent>();

        player.inputX = 0;
        player.inputY = 0;

        if(Input.GetKey(KeyCode.W)) {
            player.inputY = 1;
            player.inputX = 0;
        } else if(Input.GetKey(KeyCode.S)) {
            player.inputY = -1;
            player.inputX = 0;
        } else if(Input.GetKey(KeyCode.A)) {
            player.inputX = -1;
            player.inputY = 0;
        } else if(Input.GetKey(KeyCode.D)) {
            player.inputX = 1;
            player.inputY = 0;
        }

        if(player.fire) {
            player.fire = false;
        }

        if(Input.GetKey(KeyCode.F) && Time.time > player.lastFireTime + player.fireRate) {
            player.fire = true;
            player.lastFireTime = Time.time;

            onTankShootRequestedEvent.Publish(playerEntity.ID);
        }

        tank.inputX = player.inputX;
        tank.inputY = player.inputY;
    }
}