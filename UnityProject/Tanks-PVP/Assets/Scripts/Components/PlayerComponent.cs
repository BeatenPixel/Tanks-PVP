using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[System.Serializable]
public struct PlayerComponent : IComponent {
    public int inputX;
    public int inputY;

    public bool fire;
    public float lastFireTime;
    public float fireRate;

    public NetworkPlayer networkPlayer;
}