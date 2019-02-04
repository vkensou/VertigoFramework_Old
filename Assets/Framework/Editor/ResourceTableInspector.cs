using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

[CustomEditor(typeof(ResourceTable))]
public class ResourceTableInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var table = target as ResourceTable;

        Assert.IsNotNull(table.names);
        Assert.IsNotNull(table.objects);
        Assert.AreEqual(table.names.Count, table.objects.Count);

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            int del = -1;
            for (int i = 0; i < table.names.Count; ++i)
            {
                EditorGUILayout.BeginHorizontal();
                int oldIndex = 0;
                int.TryParse(table.names[i], out oldIndex);
                int resIndex = EditorGUILayout.IntField(oldIndex);
                table.names[i] = resIndex.ToString();
                table.objects[i] = EditorGUILayout.ObjectField(table.objects[i], typeof(Object), false);
                if (GUILayout.Button("x", EditorStyles.miniButton))
                {
                    del = i;
                }
                EditorGUILayout.EndHorizontal();
            }
            if (del != -1)
            {
                table.names.RemoveAt(del);
                table.objects.RemoveAt(del);
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                table.names.Add("0");
                table.objects.Add(null);
            }
            EditorGUILayout.EndHorizontal();

            if (check.changed)
                EditorUtility.SetDirty(table);
        }
    }

    [MenuItem("Assets/Create/Framework/Resource Table")]
    public static void Add()
    {
        CreateAsset<ResourceTable>();
    }

    public static T CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
        return asset;
    }
}