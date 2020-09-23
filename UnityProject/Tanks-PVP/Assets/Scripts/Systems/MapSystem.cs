using System.Collections.Generic;
using Morpeh;
using System.Linq;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(MapSystem))]
public sealed class MapSystem : UpdateSystem {

    private Filter mapFilter;
    private Filter allBlocksFilter;

    private BlockSpawnSystem blockSpawnSystem;
    private ArtManager artManager;
    
    public override void OnAwake() {
        
    }

    public void Initialize(BlockSpawnSystem _blockSpawnSystem, ArtManager _artManager) {
        blockSpawnSystem = _blockSpawnSystem;
        artManager = _artManager;
        
        mapFilter = World.Filter.With<MapComponent>();
        allBlocksFilter = World.Filter.With<BlockComponent>();

        LoadMapFromImage();
    }

    public override void OnUpdate(float deltaTime) {
    }

    private void LoadMapFromImage() {
        foreach (var ent in mapFilter) {
            ref var map = ref ent.GetComponent<MapComponent>();

            int mapWidth = map.mapSourceImage.width;
            int mapHeight = map.mapSourceImage.height-1;
            
            map.map = new BlockComponent[mapWidth,mapHeight];
            Color[] firstStripOfPixels = map.mapSourceImage.GetPixels(0,mapHeight, mapWidth, 1);
            Debug.Log(firstStripOfPixels[0]);
            List<Color> paletteList = new List<Color>();
            for (int i = 0; i < mapWidth; i++) {
                Debug.Log(firstStripOfPixels[i]);
                if (firstStripOfPixels[i] == Color.white) {
                    break;
                }
                else {
                    paletteList.Add(firstStripOfPixels[i]);
                }
            }

            Color[] palette = paletteList.ToArray();

            Color[] pixels = map.mapSourceImage.GetPixels(0,0, mapWidth, mapHeight);

            World.UpdateFilters();
            allBlocksFilter = World.Filter.With<BlockComponent>(); 
            
            Entity[] blockEntities = allBlocksFilter.ToArray();
            
            int entityID = 0;
            for (int x = 0; x < mapWidth; x++) {
                for (int y = 0; y < mapHeight; y++) {
                    BlockType blockType = GetBlockType(pixels[y * mapWidth + x], palette);

                    if(blockType == BlockType.AIR || blockType == BlockType.NONE) {
                        continue;
                    } 

                    Entity blockEntity;
                    if(entityID < blockEntities.Length) {
                        blockEntity = blockEntities[entityID];
                    } else {
                        blockEntity = blockSpawnSystem.InstantiateBlock();
                    }

                    ref BlockComponent blockComponent = ref blockEntity.GetComponent<BlockComponent>();

                    blockComponent.x = x;
                    blockComponent.y = y;
                    blockComponent.type = blockType;
                    
                    if (blockType == BlockType.AIR) {
                        blockEntity.RemoveComponent<BlockRenderComponent>();
                    }
                    else {
                        ref BlockRenderComponent renderComponent = ref blockEntity.GetComponent<BlockRenderComponent>();
                        renderComponent.m_Transform.position = GetBlockPosition(x, y);
                        renderComponent.spriteRenderer.sprite = artManager.GetBlockSprite(blockType);
                    }

                    if (blockType == BlockType.WALL) {
                        ref BlockCollidableComponent collidableComponent = ref blockEntity.AddComponent<BlockCollidableComponent>();
                        collidableComponent.collisionLayer = BlockCollisionLayer.TANK | BlockCollisionLayer.BULLET;
                        
                        ref BlockDestructableComponent destructableComponent = ref blockEntity.AddComponent<BlockDestructableComponent>();
                        destructableComponent.Setup(3, 15, false);
                    }

                    if (blockType == BlockType.BRICK) {
                        ref BlockCollidableComponent collidableComponent = ref blockEntity.AddComponent<BlockCollidableComponent>();
                        collidableComponent.collisionLayer = BlockCollisionLayer.TANK | BlockCollisionLayer.BULLET;
                        
                        ref BlockDestructableComponent destructableComponent = ref blockEntity.AddComponent<BlockDestructableComponent>();
                        destructableComponent.Setup(1, 15, false);
                    }

                    if (blockType == BlockType.WATER) {
                        ref BlockCollidableComponent collidableComponent = ref blockEntity.AddComponent<BlockCollidableComponent>();
                        collidableComponent.collisionLayer = BlockCollisionLayer.TANK;
                    }

                    if (blockType == BlockType.FLAG) {
                        ref BlockCollidableComponent collidableComponent = ref blockEntity.AddComponent<BlockCollidableComponent>();
                        collidableComponent.collisionLayer = BlockCollisionLayer.TANK | BlockCollisionLayer.BULLET;
                        
                        ref BlockDestructableComponent destructableComponent = ref blockEntity.AddComponent<BlockDestructableComponent>();
                        destructableComponent.Setup(1, 15, true);
                    }
                    
                    entityID++;
                }
            }
        }
    }
    
    /*
    
    public enum BlockType {
    NONE,
    AIR,
    WALL,
    BRICK,
    ICE,
    WATER,
    LEAVES
}
    
     */

    private Vector3 GetBlockPosition(int x, int y) {
        return new Vector3(x,y,0);
    }

    private BlockType GetBlockType(Color c, Color[] palette) {
        for (int i = 0; i < palette.Length; i++) {
            if (palette[i] == c)
                return (BlockType) (i + 2);
        }

        return BlockType.AIR;
    }
}