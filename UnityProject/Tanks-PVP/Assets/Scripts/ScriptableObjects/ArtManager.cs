using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TanksPVP/ArtManager", fileName = "ArtManager")]
public class ArtManager : ScriptableObject {

    [SerializeField] private Sprite[] blockSprites;
    [SerializeField] private Sprite[] brickSprites;
    [SerializeField] private Sprite[] waterSprites;

    public Sprite GetBlockSprite(BlockType blockType) {
        try {
            return blockSprites[(int) blockType - 2];
        }
        catch {
            Debug.Log(blockType);
            return null;
        }
    }

    public Sprite GetBrickSprite(byte subBlocks) {
        List<int> indexValues = new List<int>() { 1, 2, 3, 4, 5, 8, 10, 12, 15 };
        return brickSprites[indexValues.FindIndex(x => x == subBlocks)];
    }

    public Sprite[] GetWaterAnimationSprites() {
        return waterSprites;
    }

}
