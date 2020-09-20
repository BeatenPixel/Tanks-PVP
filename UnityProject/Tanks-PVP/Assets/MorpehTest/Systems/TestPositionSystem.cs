using Morpeh;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Morpeh.Globals;
using UnityEngine;

[CreateAssetMenu(menuName = "ECS/Systems/"+nameof(TestPositionSystem))]
public class TestPositionSystem : UpdateSystem {

    public GlobalEvent StopEvent;
    public GlobalEvent FreeEvent;
    
    private Filter filterMovableUnits;
    private Filter filterStoppedUnits;
    
    public override void OnAwake()
    {
        this.filterMovableUnits = World.Filter.With<UnitComponent>().Without<UnitStoppedMarker>();
        this.filterStoppedUnits = World.Filter.With<UnitComponent>().With<UnitStoppedMarker>();
    }

    public override void OnUpdate(float deltaTime) {
        this.FreeUnits();
        this.StopUnits();
        this.MoveUnits(deltaTime);
    }

    private void FreeUnits() {
        if (this.FreeEvent.IsPublished) {
            foreach (var entity in this.filterStoppedUnits) {
                entity.RemoveComponent<UnitStoppedMarker>();
            }
        }
    }
    
    private void StopUnits() {
        if (this.StopEvent.IsPublished) {
            foreach (var entity in this.filterMovableUnits) {
                entity.AddComponent<UnitStoppedMarker>(out _);
            }
        }
    }
    
    private void MoveUnits(float deltaTime) {
        foreach (var entity in this.filterMovableUnits) {
            ref var unit = ref entity.GetComponent<UnitComponent>(out _);
            unit.Position = unit.Position + new Vector3(0, 1, 0) * deltaTime;
        }
    }
    
}
