using System.Collections.Generic;
using Morpeh;
using System.Linq;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Morpeh.Globals;
using Forsis.Utils;
using UnityEngine.UIElements;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(MapSystem))]
public sealed class MapSystem : UpdateSystem {

    private Filter mapFilter;
    private Filter allBlocksFilter;
    private Filter flagBlocksFilter;
    private Filter animatedBlocksFilter;
    private Filter collidableBlocksFilter;

    private Timer animationUpdateTimer;

    private Vector2 blockSize = new Vector2(1f,1f);

    private int mapWidth;
    private int mapHeight;

    public override void OnAwake() {
        animationUpdateTimer = new Timer(0.7f);
    }

    public void Initialize() {
        mapFilter = World.Filter.With<MapComponent>();
        allBlocksFilter = World.Filter.With<BlockComponent>();
        flagBlocksFilter = World.Filter.With<BlockComponent>().With<BlockFlagComponent>();
        animatedBlocksFilter = World.Filter.With<BlockComponent>().With<BlockAnimatedRenderComponent>();
        collidableBlocksFilter = World.Filter.With<BlockComponent>().With<BlockCollidableComponent>();

        World.UpdateFilters();

        ref var map = ref mapFilter.First().GetComponent<MapComponent>();

        LoadMapFromImage(ref map);
        World.UpdateFilters();
        ProcessMapData(ref map);
    }

    public override void OnUpdate(float deltaTime) {
        World.UpdateFilters();

        ref MapComponent map = ref mapFilter.First().GetComponent<MapComponent>();

        if (animationUpdateTimer.Check()) {
            UpdateAnimation(ref map);
        }
    }

    private void LoadMapFromImage(ref MapComponent map) {
        map.width = map.mapSourceImage.width;
        map.height = map.mapSourceImage.height-1;

        map.blockEntities = new Entity[map.width, map.height];
        Color[] firstStripOfPixels = map.mapSourceImage.GetPixels(0, map.height, map.width, 1);

        List<Color> paletteList = new List<Color>();
        for (int i = 0; i < map.width; i++) {
            if (firstStripOfPixels[i] == Color.white) {
                break;
            }
            else {
                paletteList.Add(firstStripOfPixels[i]);
            }
        }

        Color[] palette = paletteList.ToArray();

        Color[] pixels = map.mapSourceImage.GetPixels(0,0, map.width, map.height);

        World.UpdateFilters();
        allBlocksFilter = World.Filter.With<BlockComponent>(); 
            
        Entity[] blockEntities = allBlocksFilter.ToArray();
            
        int entityID = 0;
        for (int x = 0; x < map.width; x++) {
            for (int y = 0; y < map.height; y++) {
                BlockType blockType = GetBlockType(pixels[y * map.width + x], palette);

                if(blockType == BlockType.AIR || blockType == BlockType.NONE) {
                    continue;
                }

                BlockProvider blockProvider = null;
                Entity blockEntity;

                if (entityID < blockEntities.Length) {
                    blockEntity = blockEntities[entityID];
                } else {
                    blockProvider = ObjectSpawner.inst.InstantiateBlock();
                    blockEntity = blockProvider.Entity;
                }                

                map.blockEntities[x, y] = blockEntity;

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
                    renderComponent.spriteRenderer.sprite = ArtManager.inst.GetBlockSprite(blockType);
                }

                if (blockType == BlockType.WALL) {
                    ref BlockCollidableComponent collidableComponent = ref blockEntity.AddComponent<BlockCollidableComponent>();
                    collidableComponent.collisionLayer = BlockCollisionLayer.TANK | BlockCollisionLayer.BULLET;
                    collidableComponent.collisionSubBlocks = 15;

                    ref BlockDestructableComponent destructableComponent = ref blockEntity.AddComponent<BlockDestructableComponent>();
                    destructableComponent.Setup(3, 15, false);
                }

                if (blockType == BlockType.BRICK) {
                    ref BlockCollidableComponent collidableComponent = ref blockEntity.AddComponent<BlockCollidableComponent>();
                    collidableComponent.collisionLayer = BlockCollisionLayer.TANK | BlockCollisionLayer.BULLET;
                    collidableComponent.collisionSubBlocks = 15;

                    ref BlockDestructableComponent destructableComponent = ref blockEntity.AddComponent<BlockDestructableComponent>();
                    destructableComponent.Setup(1, 15, false);
                }

                if (blockType == BlockType.WATER) {
                    ref BlockCollidableComponent collidableComponent = ref blockEntity.AddComponent<BlockCollidableComponent>();
                    collidableComponent.collisionLayer = BlockCollisionLayer.TANK;
                    collidableComponent.collisionSubBlocks = 15;

                    ref BlockAnimatedRenderComponent animatedRenderComponent = ref blockEntity.AddComponent<BlockAnimatedRenderComponent>();
                    animatedRenderComponent.spriteRenderer = blockProvider.spriteRend;
                    animatedRenderComponent.content = BlockAnimationContent.WATER;
                    animatedRenderComponent.currentAnimationFrame = 0;
                }

                if (blockType == BlockType.FLAG) {
                    ref BlockCollidableComponent collidableComponent = ref blockEntity.AddComponent<BlockCollidableComponent>();
                    collidableComponent.collisionLayer = BlockCollisionLayer.TANK | BlockCollisionLayer.BULLET;
                    collidableComponent.collisionSubBlocks = 15;

                    ref BlockDestructableComponent destructableComponent = ref blockEntity.AddComponent<BlockDestructableComponent>();
                    destructableComponent.Setup(1, 15, true);

                    ref BlockFlagComponent flagComponent = ref blockEntity.AddComponent<BlockFlagComponent>();
                }
                
                if(blockType == BlockType.LEAVES) {
                    ref BlockRenderComponent renderComponent = ref blockEntity.GetComponent<BlockRenderComponent>();
                    renderComponent.spriteRenderer.sortingOrder = 3;
                }

                entityID++;
            }
        }
    }

    private void ProcessMapData(ref MapComponent map) {

        mapWidth = map.width;
        mapHeight = map.height;

        foreach (var ent in flagBlocksFilter) {

            ref BlockComponent blockComponent = ref ent.GetComponent<BlockComponent>();
            int x = blockComponent.x;
            int y = blockComponent.y;
            int w = map.width;
            int h = map.height;

            if(IsCoordCorrect(x-1,y-1,w,h)) {
                SetBlock(ref map, x - 1, y - 1, BlockType.BRICK, (byte)8);
            }
            if (IsCoordCorrect(x - 1, y, w, h)) {
                SetBlock(ref map, x - 1, y, BlockType.BRICK, (byte)10);
            }
            if (IsCoordCorrect(x - 1, y + 1, w, h)) {
                SetBlock(ref map, x - 1, y + 1, BlockType.BRICK, (byte)2);
            }
            if (IsCoordCorrect(x, y + 1, w, h)) {
                SetBlock(ref map, x, y + 1, BlockType.BRICK, (byte)3);
            }
            if (IsCoordCorrect(x + 1, y + 1, w, h)) {
                SetBlock(ref map, x + 1, y + 1, BlockType.BRICK, (byte)1);
            }
            if (IsCoordCorrect(x + 1, y, w, h)) {
                SetBlock(ref map, x + 1, y, BlockType.BRICK, (byte)5);
            }
            if (IsCoordCorrect(x + 1, y - 1, w, h)) {
                SetBlock(ref map, x + 1, y - 1, BlockType.BRICK, (byte)4);
            }
            if (IsCoordCorrect(x, y - 1, w, h)) {
                SetBlock(ref map, x, y - 1, BlockType.BRICK, (byte)12);
            }
        }
    }

    private bool IsCoordCorrect(int x, int y, int w, int h) {
        return (x >= 0 && x < w && y >= 0 && y < h);
    }

    private BlockComponent[] GetBlocksOfType(ref MapComponent m, BlockType type) {
        List<BlockComponent> blocks = new List<BlockComponent>();
        for (int i = 0; i < m.width; i++) {
            for (int j = 0; j < m.height; j++) {
                if(m.blockEntities[i,j].GetComponent<BlockComponent>().type == type) {
                    blocks.Add(m.blockEntities[i, j].GetComponent<BlockComponent>());
                }
            }
        }
        return blocks.ToArray();
    }

    private void SetBlock(ref MapComponent m, int x, int y, BlockType type, params object[] args) {
        Entity blockEntity = m.blockEntities[x, y];
        
        if(blockEntity == null) {
            blockEntity = ObjectSpawner.inst.InstantiateBlock().Entity;
        }

        ref BlockComponent blockComponent = ref blockEntity.GetComponent<BlockComponent>();
        blockComponent.x = x;
        blockComponent.y = y;
        blockComponent.type = type;

        if (args != null && args.Length > 0) {
            if(type == BlockType.BRICK) {
                ref BlockRenderComponent renderComponent = ref blockEntity.GetComponent<BlockRenderComponent>();

                // Don't know how to create a ref variable and then set it depending on condition
                // for that reason copy-pasting code below
                bool hasCollidableComponent = blockEntity.Has<BlockCollidableComponent>();

                if(hasCollidableComponent) {
                    ref BlockCollidableComponent collidableComponent = ref blockEntity.GetComponent<BlockCollidableComponent>();

                    byte subBlocks = (byte)args[0];

                    renderComponent.m_Transform.position = GetBlockPosition(x, y);
                    renderComponent.spriteRenderer.sprite = ArtManager.inst.GetBrickSprite(subBlocks);

                    collidableComponent.collisionLayer = BlockCollisionLayer.TANK | BlockCollisionLayer.BULLET;
                    collidableComponent.collisionSubBlocks = subBlocks;
                }  else {
                    ref BlockCollidableComponent collidableComponent = ref blockEntity.AddComponent<BlockCollidableComponent>();

                    byte subBlocks = (byte)args[0];

                    renderComponent.m_Transform.position = GetBlockPosition(x, y);
                    renderComponent.spriteRenderer.sprite = ArtManager.inst.GetBrickSprite(subBlocks);

                    collidableComponent.collisionLayer = BlockCollisionLayer.TANK | BlockCollisionLayer.BULLET;
                    collidableComponent.collisionSubBlocks = subBlocks;
                }
            }
        }
    }

    private void SurroundFlagWithBlock() {

    }

    private void UpdateAnimation(ref MapComponent map) {
        foreach (var ent in animatedBlocksFilter) {
            ref BlockAnimatedRenderComponent animRend = ref ent.GetComponent<BlockAnimatedRenderComponent>();
            BlockAnimation blockAnimationData = ArtManager.inst.GetBlockAnimation(animRend.content);
            animRend.currentAnimationFrame++;
            if(animRend.currentAnimationFrame >= blockAnimationData.sprites.Length) {
                animRend.currentAnimationFrame = 0;
            }

            animRend.spriteRenderer.sprite = blockAnimationData.sprites[animRend.currentAnimationFrame];
        }
    }

    public bool CheckCollision(Vector2 pos, Vector2 size, BlockCollisionLayer collisionLayer, out CollisionInfo collisionInfo) {
        bool flag = false;

        Entity collisionEntity = null;
        byte _hitSubBlock = 255;
        bool _isMapBorderCollision = false;

        Vector2[] subBlocksLocalPos = new Vector2[4] {
            new Vector2(-blockSize.x*0.25f,-blockSize.y*0.25f),
            new Vector2(blockSize.x*0.25f,-blockSize.y*0.25f),
            new Vector2(-blockSize.x*0.25f,blockSize.y*0.25f),
            new Vector2(blockSize.x*0.25f,blockSize.y*0.25f)
        };

        Vector2 subBlockSize = new Vector2(blockSize.x * 0.5f, blockSize.y * 0.5f);

        foreach (var blockEnt in collidableBlocksFilter) {
            ref BlockComponent block = ref blockEnt.GetComponent<BlockComponent>();
            ref BlockCollidableComponent collidableComponent = ref blockEnt.GetComponent<BlockCollidableComponent>();

            if (!Utils.IsInMask(collisionLayer, collidableComponent.collisionLayer))
                continue;

            Vector2 blockPos = new Vector2(block.x, block.y);

            for (byte i = 0; i < 4; i++) {
                if (collidableComponent.collisionSubBlocks != (collidableComponent.collisionSubBlocks | (1 << i)))
                    continue;

                if (CheckAABBCollision(pos - size * 0.5f, pos + size * 0.5f, blockPos + subBlocksLocalPos[i] - subBlockSize * 0.5f, blockPos + subBlocksLocalPos[i] + subBlockSize * 0.5f, out _isMapBorderCollision)) {
                    flag = true;
                    _hitSubBlock = i;
                    collisionEntity = blockEnt;
                    break;
                }
            }

            if (flag == true)
                break;

        }

        if (_isMapBorderCollision)
            collisionEntity = null;

        collisionInfo = new CollisionInfo() {
            entity = collisionEntity,
            subBlockID = _hitSubBlock,
            isMapBorderCollision = _isMapBorderCollision
        };

        return flag;
    }

    private bool CheckAABBCollision(Vector2 minA, Vector2 maxA, Vector2 minB, Vector2 maxB, out bool isMapBorderCollision, bool mapSpecific = true) {
        if(mapSpecific) {
            if(minA.x <= -blockSize.x*0.5f || maxA.x >= (mapWidth-0.5f)*blockSize.x ||
                minA.y <= -blockSize.y * 0.5f || maxA.y >= (mapHeight - 0.5f) * blockSize.y) {
                isMapBorderCollision = true;
                return true;
            }
        }

        isMapBorderCollision = false;

        if (maxA.x < minB.x || minA.x > maxB.x) return false;
        if (maxA.y < minB.y || minA.y > maxB.y) return false;

        return true;
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

public struct CollisionInfo {
    public Entity entity;
    public byte subBlockID;
    public bool isMapBorderCollision;
}