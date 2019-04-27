using UnityEngine;
using UnityEditor;

public class TexturePostProcessor : AssetPostprocessor {

	void OnPreprocessTexture()
     {
        TextureImporter textureImporter = (TextureImporter)assetImporter;
		textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
 
        TextureImporterSettings textureSettings = new TextureImporterSettings();
        textureImporter.ReadTextureSettings(textureSettings);
        textureSettings.spriteMeshType = SpriteMeshType.FullRect;
    	textureSettings.spriteExtrude = 0;
	  
		textureSettings.filterMode = FilterMode.Point;
		textureSettings.spritePixelsPerUnit = 8;
        textureImporter.SetTextureSettings(textureSettings);      
     }

	void OnPostprocessTexture (Texture2D texture) {
		TextureImporter importer = assetImporter as TextureImporter;
		importer.anisoLevel = 0;
		importer.textureCompression = TextureImporterCompression.Uncompressed;
	//	importer.SaveAndReimport();
	}

/*	void OnPostprocessTexture(Texture2D texture) {
		TextureImporter importer = assetImporter as TextureImporter;
		importer.anisoLevel = 0;
		importer.filterMode = FilterMode.Point;
		importer.spritePixelsPerUnit = 16;
		importer.textureCompression = TextureImporterCompression.Uncompressed;

	}*/
}