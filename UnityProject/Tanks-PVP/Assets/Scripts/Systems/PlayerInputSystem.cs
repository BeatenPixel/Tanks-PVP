using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Morpeh.Globals;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(PlayerInputSystem))]
public sealed class PlayerInputSystem : UpdateSystem {

    private Filter localPlayerFilter;

    public override void OnAwake() {
        localPlayerFilter = World.Filter.With<PlayerComponent>().With<TankComponent>().With<LocalPlayerTag>();
    }

    public override void OnUpdate(float deltaTime) {
        Entity playerEntity = localPlayerFilter.First();

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

        if(player.fire && Time.time >= player.lastFireTime + player.fireRate) {
            player.fire = false;
        }
        
        if(Input.GetKey(KeyCode.Space) && Time.time > player.lastFireTime + player.fireRate) {
            player.fire = true;
            player.lastFireTime = Time.time;
            Debug.Log("GetKeyShoot");
            NetworkEventsManager.inst.PublishEvent("tankShootRequest",
                player.networkPlayer.networkViewID.ToString(), Photon.Realtime.ReceiverGroup.All);
        }

        tank.inputX = player.inputX;
        tank.inputY = player.inputY;
    }

}