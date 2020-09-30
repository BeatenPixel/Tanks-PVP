using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[System.Serializable]
public struct TankComponent : IComponent {
    public float x;
    public float y;

    public float[] receivedPosX;
    public float[] receivedPosY;

    public byte faceDirection; // 0=up 1=right 2=down 3=left
    public int currentLevel;
    public int tankType;
    public int inputX;
    public int inputY;
    public float moveSpeed;
    public float halfSize;
}