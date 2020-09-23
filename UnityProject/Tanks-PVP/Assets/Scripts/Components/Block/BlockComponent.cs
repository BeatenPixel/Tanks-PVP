using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[System.Serializable]
public struct BlockComponent : IComponent {
    public BlockType type;
    public int x;
    public int y;
}

public enum BlockType {
    NONE,
    AIR,
    WALL,
    BRICK,
    ICE,
    WATER,
    LEAVES,
    FLAG
}