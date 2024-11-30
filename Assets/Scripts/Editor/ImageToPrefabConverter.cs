using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ImageToPrefabConverter : MonoBehaviour
{
    [SerializeField] private GameObject Background;
    [SerializeField] private GameObject spawnOppositeCar;

    [MenuItem("Tools/Convert Images to Prefabs")]
    private static void ConvertImagesToPrefabs()
    {
        
        string prefabFolder = "Assets/prefab/cars";
        
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

            BoxCollider2D box = newGO.AddComponent<BoxCollider2D>();
            box.offset = new Vector2(-0.2203777f, -1.192093e-07f);
            box.size = new Vector2(2.849597f, 3.98f);

            Rigidbody2D rig = newGO.AddComponent<Rigidbody2D>(); 
            rig.gravityScale = 0;

            newGO.AddComponent<oppCarControl>();

            newGO.transform.localScale = new Vector3(0.7302f, 0.8383f, 1f);
            newGO.transform.Rotate(180f, 0f, 0f);

            string prefabPath = $"{prefabFolder}/{sprite.name}.prefab";
            PrefabUtility.SaveAsPrefabAsset(newGO, prefabPath);

            Object.DestroyImmediate(newGO);
        }
    }
}
