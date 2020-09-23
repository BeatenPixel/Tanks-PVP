using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[System.Serializable]
public struct BlockDestructableComponent : IComponent {
    public int tankLevelThreshold;
    public byte subBlocks;
    public bool isFlag;

    public void Setup(int _tankLevelThreshold, byte _subBlocks, bool _isFlag) {
        tankLevelThreshold = _tankLevelThreshold;
        subBlocks = _subBlocks;
        isFlag = _isFlag;
    }
    
}