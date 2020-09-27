using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpriteProcessor : AssetPostprocessor {

    void OnPreprocessTexture() {
        TextureImporter textureImporter = (TextureImporter)assetImporter;
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Multiple;
        textureImporter.mipmapEnabled = false;
        textureImporter.filterMode = FilterMode.Point;
    }

    public void OnPostprocessTexture(Texture2D texture) {
        if(texture.name == "Tanks 1") {
            if(assetImporter.userData == "") {

                Debug.Log("Texture2D: (" + texture.width + "x" + texture.height + ")");

                int spriteSize = 64;
                int colCount = texture.width / spriteSize;
                int rowCount = texture.height / spriteSize;

                List<SpriteMetaData> metas = new List<SpriteMetaData>();

                for (int r = 0; r < rowCount; ++r) {
                    for (int c = 0; c < colCount; ++c) {
                        SpriteMetaData meta = new SpriteMetaData();
                        meta.rect = new Rect(c * spriteSize, r * spriteSize, spriteSize, spriteSize);
                        meta.name = c + "-" + r;
                        metas.Add(meta);
                    }
                }

                TextureImporter textureImporter = (TextureImporter)assetImporter;

                textureImporter.spritesheet = metas.ToArray();

                assetImporter.userData = "1";
                AssetDatabase.SaveAssets();
            }
        }        
    }

    public void OnPostprocessSprites(Texture2D texture, Sprite[] sprites) {
        Debug.Log("Sprites: " + sprites.Length);
    }

}
