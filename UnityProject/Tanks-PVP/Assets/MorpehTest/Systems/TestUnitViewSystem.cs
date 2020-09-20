using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(TestUnitViewSystem))]
public sealed class TestUnitViewSystem : UpdateSystem {

    private Filter filter;
    private Filter filterInitialize;
    
    public override void OnAwake() {
        var commonFilter = World.Filter.With<UnitComponent>().With<UnitViewComponent>();
        
        this.filter = commonFilter.With<UnitViewInitializedMarker>();
        this.filterInitialize = commonFilter.Without<UnitViewInitializedMarker>();
    }

    public override void OnUpdate(float deltaTime) {
        this.InitializeUnits();
        this.UpdateViews();
    }

    private void InitializeUnits() {
        var units = this.filterInitialize.Select<UnitComponent>();
        var views = this.filterInitialize.Select<UnitViewComponent>();

        for (int i = 0; i < this.filterInitialize.Length; i++) {
            ref var unit = ref units.GetComponent(i);
            ref var view = ref views.GetComponent(i);

            unit.Position = view.t.position;
            var ent = this.filterInitialize.GetEntity(i);
            ent.AddComponent<UnitViewInitializedMarker>();
        }
    }

    private void UpdateViews() {
        var units = this.filter.Select<UnitComponent>();
        var views = this.filter.Select<UnitViewComponent>();

        for (int i = 0; i < this.filter.Length; i++) {
            ref var unit = ref units.GetComponent(i);
            ref var view = ref views.GetComponent(i);

            view.t.position = unit.Position;
        }
    }
}