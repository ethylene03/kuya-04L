using UnityEditor;
using UnityEngine;

public class ImageToPrefabConverter : MonoBehaviour
{
    [MenuItem("Tools/Convert Images to Prefabs")]
    private static void ConvertImagesToPrefabs()
    {
        
        string prefabFolder = "Assets/prefab";
        
        Object[] selectedAssets = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);

        if (selectedAssets.Length == 0) {
            Debug.LogWarning("No images selected!");
            return;
        }

        foreach (Object asset in selectedAssets) {
            
            Texture2D texture = asset as Texture2D;
            if (texture == null) continue;

            string assetPath = AssetDatabase.GetAssetPath(texture);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

            GameObject newGO = new GameObject(sprite.name);
            SpriteRenderer spriteRenderer = newGO.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;

            newGO.AddComponent<BoxCollider2D>(); 
            Rigidbody2D rig = newGO.AddComponent<Rigidbody2D>(); 
            rig.gravityScale = 0;
            newGO.AddComponent<carControl>(); 

            newGO.transform.localScale = new Vector3(0.7406f, 0.6575f, 1f);

            string prefabPath = $"{prefabFolder}/{sprite.name}.prefab";
            PrefabUtility.SaveAsPrefabAsset(newGO, prefabPath);

            Object.DestroyImmediate(newGO);
        }
    }
}
