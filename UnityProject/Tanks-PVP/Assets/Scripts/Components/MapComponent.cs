using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using System.Collections.Generic;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[System.Serializable]
public struct MapComponent : IComponent {
    public Texture2D mapSourceImage;
    [HideInInspector] public int width;
    [HideInInspector] public int height;
    [HideInInspector] public Color[] pixels;
    [HideInInspector] public Entity[,] blockEntities;
    //[HideInInspector] public List<>
}