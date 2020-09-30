using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Forsis.Utils;
using Morpeh.Globals;
using System.Linq;
using UnityEngine.UI;
using System;
using System.Net.Http.Headers;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(TankShootSystem))]
public sealed class TankShootSystem : UpdateSystem {

    [SerializeField] private GlobalEventString onTankShootRequestedEvent;

    private MapSystem mapSystem;

    private Filter tanksFilter;
    private Filter projectilesFilter;
    private Filter playersFilter;

    public override void OnAwake() {
        playersFilter = World.Filter.With<PlayerComponent>();
        tanksFilter = World.Filter.With<TankComponent>();
        projectilesFilter = World.Filter.With<ProjectileComponent>();
    }

    public void Initialize(MapSystem _mapSystem) {
        mapSystem = _mapSystem;
    }

    public override void OnUpdate(float deltaTime) {

        if(onTankShootRequestedEvent.IsPublished) {

            foreach (var networkViewIDStr in onTankShootRequestedEvent.BatchedChanges) {
                int networkViewID = int.Parse(networkViewIDStr);
                PlayerProvider playerProvider = null;

                foreach (var p in playersFilter) {
                    ref PlayerComponent pc = ref p.GetComponent<PlayerComponent>();
                    if(pc.networkPlayer.networkViewID == networkViewID) {
                        playerProvider = pc.networkPlayer.GetPlayerProvider();
                        break;
                    }
                }

                if (playerProvider == null)
                    continue;

                Entity tankEntity = playerProvider.Entity;
                
                ref TankComponent tank = ref tankEntity.GetComponent<TankComponent>();

                ProjectileProvider pp = ObjectSpawner.inst.InstantiateBullet();
                ref ProjectileComponent projectile = ref pp.Entity.GetComponent<ProjectileComponent>();
                ref ProjectileRenderComponent projectileRender = ref pp.Entity.GetComponent<ProjectileRenderComponent>();

                Vector2 faceDirVector = tank.faceDirection.DirToVector2D();
                projectile.pos = new Vector2(tank.x, tank.y) + faceDirVector * tank.halfSize; ;
                projectile.dir = faceDirVector;

                projectile.speed = 10;
                projectile.level = 0;

                projectileRender.m_Transform.position = projectile.pos;
                projectileRender.m_Transform.rotation = Quaternion.FromToRotation(Vector3.up, faceDirVector);
            }
        }

        SimulateProjectiles(deltaTime);
    }

    private void SimulateProjectiles(float deltaTime) {
        foreach (var ent in projectilesFilter) {
            ref ProjectileComponent projectile = ref ent.GetComponent<ProjectileComponent>();
            ref ProjectileRenderComponent projectileRender = ref ent.GetComponent<ProjectileRenderComponent>();

            projectile.pos += projectile.dir*deltaTime*projectile.speed;

            CollisionInfo collision;

            if(mapSystem.CheckCollision(projectile.pos, projectile.size, BlockCollisionLayer.BULLET, out collision)) {
                if (!collision.isMapBorderCollision) {
                    DestroyBlockPiece(collision, projectile.dir.DirToByte());
                }

                Destroy(projectileRender.m_Transform.gameObject);
                World.RemoveEntity(ent);
            } else {
                projectileRender.m_Transform.position = projectile.pos;
            }            
        }
    }

    private void DestroyBlockPiece(CollisionInfo c, byte dir) {
        

        ref BlockComponent block = ref c.entity.GetComponent<BlockComponent>();
        ref BlockCollidableComponent blockCol = ref c.entity.GetComponent<BlockCollidableComponent>();
        ref BlockRenderComponent blockRender = ref c.entity.GetComponent<BlockRenderComponent>();

        /* subBlocks bytes layout
        2  3
        0  1

        directions
            0
         3     1  
            2
        */

        byte[] affectingBytes = new byte[8] { 0, 1, 0, 2,  2, 3, 1, 3 };

        switch(dir) {
            case 0:
                if(c.subBlockID >= 2) {
                    affectingBytes[dir * 2] = 2;
                    affectingBytes[dir * 2 + 1] = 3;
                }
                break;
            case 1:
                if (c.subBlockID != 0 && c.subBlockID != 2) {
                    affectingBytes[dir * 2] = 3;
                    affectingBytes[dir * 2 + 1] = 1;
                }
                break;
            case 2:
                if (c.subBlockID <= 1) {
                    affectingBytes[dir * 2] = 0;
                    affectingBytes[dir * 2 + 1] = 1;
                }
                break;
            case 3:
                if (c.subBlockID != 1 && c.subBlockID != 3) {
                    affectingBytes[dir*2] = 0;
                    affectingBytes[dir * 2 + 1] = 2;
                }
                break;
        }

        byte mask = (byte)~(1 << affectingBytes[dir * 2] | 1 << affectingBytes[dir * 2 + 1]);
        blockCol.collisionSubBlocks = (byte)(blockCol.collisionSubBlocks & mask);

        if (blockCol.collisionSubBlocks > 0) {
            if(block.type == BlockType.BRICK) {
                blockRender.spriteRenderer.sprite = ArtManager.inst.GetBrickSprite(blockCol.collisionSubBlocks);
            }
        } else {
            Destroy(blockRender.m_Transform.gameObject);
            World.RemoveEntity(c.entity);
            World.UpdateFilters();
        }
    }
}