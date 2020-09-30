using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "TanksPVP/ArtManager", fileName = "ArtManager")]
public class ArtManager : ScriptableObject {

    public static ArtManager inst;

    [SerializeField] private Sprite[] blockSprites;
    [SerializeField] private Sprite[] brickSprites;
    [SerializeField] private List<BlockAnimation> blockAnimations;
    public List<TankAnimation> tankAnimations;
    
    public void Initialize() {
        inst = this;
    }

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

    public BlockAnimation GetBlockAnimation(BlockAnimationContent content) {
        return blockAnimations.First((x) => x.content == content);
    }

    public Sprite GetTankSprite(int tankType, int tankLevel, int faceDirection, int animationFrame) {
        return tankAnimations[tankType].sprites[tankLevel * 8 + 4 * animationFrame + faceDirection];
    }

}

[System.Serializable]
public struct BlockAnimation {
    public BlockAnimationContent content;
    public Sprite[] sprites;
}

[System.Serializable]
public struct TankAnimation {
    public int tankType;
    public Sprite[] sprites;
}
