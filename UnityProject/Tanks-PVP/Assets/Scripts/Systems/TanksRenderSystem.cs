using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Forsis.Utils;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(TanksRenderSystem))]
public sealed class TanksRenderSystem : UpdateSystem {

    private Filter tanksFilter;

    private Timer tankAnimationUpdateTimer;

    public override void OnAwake() {
        tankAnimationUpdateTimer = new Timer(0.05f);

        tanksFilter = World.Filter.With<TankComponent>().With<TankRenderComponent>();
    }

    public void Initialize() {
        
    }

    public override void OnUpdate(float deltaTime) {

        bool updateAnimation = tankAnimationUpdateTimer.Check();

        foreach (var ent in tanksFilter) {
            ref TankComponent tankComponent = ref ent.GetComponent<TankComponent>();
            ref TankRenderComponent tankRenderComponent = ref ent.GetComponent<TankRenderComponent>();

            if (updateAnimation && (tankComponent.inputX != 0 || tankComponent.inputY != 0)) {
                tankRenderComponent.animationFrame++;
                if(tankRenderComponent.animationFrame >= 2) {
                    tankRenderComponent.animationFrame = 0;
                }
            }

            tankRenderComponent.m_Transform.position = new Vector3(tankComponent.x, tankComponent.y, 0);
            tankRenderComponent.spriteRenderer.sprite = ArtManager.inst.GetTankSprite(tankComponent.tankType, tankComponent.currentLevel, tankComponent.faceDirection, tankRenderComponent.animationFrame);
        }

    }
}