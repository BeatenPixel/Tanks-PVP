using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class SpriteSlicer : EditorWindow {

    private Texture2D targetTexture;

    [MenuItem("Forsis/SpriteSlicer")]
    static void Init() {
        SpriteSlicer window = (SpriteSlicer)EditorWindow.GetWindow(typeof(SpriteSlicer));
        window.Show();
    }

    private void OnGUI() {
        
        if(GUILayout.Button("Process")) {
            string textureFilePath = EditorUtility.OpenFilePanel("Select texture", Application.dataPath, "");
            string localTexturePath = textureFilePath.Substring(textureFilePath.LastIndexOf("Tanks-PVP/") + 10);
            Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(localTexturePath, typeof(Texture2D));
            EditorGUILayout.ObjectField("Target sprite", texture, typeof(Texture2D));

            SliceSprite(texture, textureFilePath);
        }
    }

    private void SliceSprite(Texture2D texture, string path) {
        string localTexturePath = path.Substring(path.LastIndexOf("Tanks-PVP/") + 10);

        TextureImporter importer = AssetImporter.GetAtPath(localTexturePath) as TextureImporter;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.filterMode = FilterMode.Point;

        List<SpriteMetaData> spritesMetaData = new List<SpriteMetaData>(importer.spritesheet);
        for (int i = spritesMetaData.Count-1; i >= 0; i--) {
            if(spritesMetaData[i].name.Contains("Tank")) {
                spritesMetaData.RemoveAt(i);
            }
        }

        ArtManager artManager = (ArtManager)AssetDatabase.LoadAssetAtPath("Assets/Data/ArtManager.asset", typeof(ArtManager));
        List<TankAnimation> tanksAnimations = new List<TankAnimation>();

        if (importer != null && texture != null) {
            int offsetX = 3, offsetY = 1;
            int tW = texture.width, tH = texture.height;

            Debug.Log("123");

            for (int tankNumber = 0; tankNumber < 7; tankNumber++) {
                string tankName = "Tank_" + tankNumber;

                for (int tankLevel = 0; tankLevel < 4; tankLevel++) {

                    int groupX = offsetX + tankNumber * 128;
                    int groupY = tH - offsetY - tankLevel * 64;

                    for (int tankAnimFrame = 0; tankAnimFrame < 2; tankAnimFrame++) {
                        for (int faceDirection = 0; faceDirection < 4; faceDirection++) {

                            int localX, localY;
                            localX = (faceDirection % 2 == 0 ? 0 : -2) + 32 * faceDirection;
                            localY = (faceDirection % 2 == 0 ? 2 : 4) - (tankAnimFrame + 1) * 32;

                            int sizeX, sizeY;
                            if (faceDirection % 2 == 0) {
                                sizeX = 26; sizeY = 30;
                            } else {
                                sizeX = 30; sizeY = 26;
                            }

                            SpriteMetaData metaData = new SpriteMetaData();
                            metaData.pivot = new Vector2(0.5f, 0.5f);
                            metaData.name = tankName + "_LVL" + tankLevel + "_ANIM" + tankAnimFrame + "_F" + faceDirection;
                            metaData.rect = new Rect(groupX + localX, groupY + localY, sizeX, sizeY);

                            spritesMetaData.Add(metaData);
                        }
                    }
                }
                
            }

            importer.spritesheet = spritesMetaData.ToArray();
            AssetDatabase.SaveAssets();

            Object[] spritesObjects = AssetDatabase.LoadAllAssetsAtPath(localTexturePath);
            Sprite[] sprites = spritesObjects.Where(x => x is Sprite).Cast<Sprite>().ToArray();

            for (int tankNumber = 0; tankNumber < 7; tankNumber++) {
                string tankName = "Tank_" + tankNumber;
                TankAnimation tankAnimation = new TankAnimation();
                tankAnimation.tankType = tankNumber;
                tankAnimation.sprites = new Sprite[4 * 2 * 4];

                for (int tankLevel = 0; tankLevel < 4; tankLevel++) {

                    int groupX = offsetX + tankNumber * 128;
                    int groupY = tH - offsetY - tankLevel * 64;

                    for (int tankAnimFrame = 0; tankAnimFrame < 2; tankAnimFrame++) {
                        for (int faceDirection = 0; faceDirection < 4; faceDirection++) {
                            string spriteName = tankName + "_LVL" + tankLevel + "_ANIM" + tankAnimFrame + "_F" + faceDirection;

                            tankAnimation.sprites[tankLevel * 8 + tankAnimFrame * 4 + faceDirection] = sprites.First((x) => x.name == spriteName);
                        }
                    }
                }

                tanksAnimations.Add(tankAnimation);
            }

            artManager.tankAnimations = tanksAnimations;
            EditorUtility.SetDirty(artManager);

            AssetDatabase.SaveAssets();
        }
    }

}
