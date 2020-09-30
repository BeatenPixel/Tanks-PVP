using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Photon.Pun;

[CreateAssetMenu(menuName = "Forsis/" + nameof(ObjectSpawner))]
public sealed class ObjectSpawner : ScriptableObject {

    public static ObjectSpawner inst;

    public BlockProvider basicBlockProvider;
    public AnimatedBlockProvider animatedBlockProvider;
    public ProjectileProvider projectileProvider;
    public PlayerProvider playerProviderPrefab;

    public void Initialize() {
        inst = this;
    }

    public BlockProvider InstantiateBlock() {
        BlockProvider newBlock = Instantiate(basicBlockProvider) as BlockProvider;
        return newBlock;
    }

    public Entity InstantiateAnimatedBlock(out BlockAnimatedRenderComponent component) {
        AnimatedBlockProvider newBlock = Instantiate(animatedBlockProvider) as AnimatedBlockProvider;
        component = newBlock.GetData();
        return newBlock.Entity;
    }

    public ProjectileProvider InstantiateBullet() {
        //ProjectileProvider newProjectile = PhotonNetwork.Instantiate("Prefabs/" + projectileProvider.name, Vector3.zero, Quaternion.identity).GetComponent<ProjectileProvider>();
        ProjectileProvider newProjectile = Instantiate(projectileProvider) as ProjectileProvider;
        return newProjectile;
    }

    public PlayerProvider InstantiatePlayer() {
        
        PlayerProvider newPlayer = PhotonNetwork.Instantiate("Prefabs/" + playerProviderPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<PlayerProvider>();
        
        //PlayerProvider newPlayer = Instantiate(playerProviderPrefab) as PlayerProvider;
        

        return newPlayer;
    }
    
}