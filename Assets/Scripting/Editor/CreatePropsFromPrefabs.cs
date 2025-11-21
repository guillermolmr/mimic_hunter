using UnityEngine;
using UnityEditor;
using System.IO;

public class CreatePropsFromPrefabs : Editor
{
    [MenuItem("Assets/Crear Props desde Prefabs", true)]
    private static bool ValidateCreateProps()
    {
        // Habilitar solo si la selección incluye prefabs
        foreach (var obj in Selection.objects)
        {
            if (PrefabUtility.IsPartOfPrefabAsset(obj))
                return true;
        }
        return false;
    }

    [MenuItem("Assets/Crear Props desde Prefabs")]
    private static void CreateProps()
    {
        // Elegir carpeta de destino
        string folderPath = EditorUtility.OpenFolderPanel(
            "Seleccionar carpeta destino para Props",
            "Assets",
            ""
        );

        if (string.IsNullOrEmpty(folderPath))
            return;

        // Convertir a ruta relativa (Unity requiere Assets/…)
        if (folderPath.StartsWith(Application.dataPath))
        {
            folderPath = "Assets" + folderPath.Substring(Application.dataPath.Length);
        }

        foreach (var obj in Selection.objects)
        {
            if (!PrefabUtility.IsPartOfPrefabAsset(obj))
                continue;

            GameObject prefab = obj as GameObject;
            if (prefab == null)
                continue;

            // Crear instancia del ScriptableObject
            Prop newProp = ScriptableObject.CreateInstance<Prop>();

            // Asignar el prefab
            newProp.prop = prefab;

            // Ruta del archivo .asset
            string assetPath = Path.Combine(folderPath, prefab.name + ".asset");

            // Crear el asset
            AssetDatabase.CreateAsset(newProp, assetPath);
            EditorUtility.SetDirty(newProp);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Props creados correctamente.");
    }
}
