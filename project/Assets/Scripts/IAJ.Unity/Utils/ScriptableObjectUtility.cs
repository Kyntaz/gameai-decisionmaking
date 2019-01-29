using UnityEngine;
using UnityEditor;
using System.IO;

namespace Assets.Scripts.IAJ.Unity.Utils
{
    public static class ScriptableObjectUtility
    {
        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files.
        //  taken from http://wiki.unity3d.com/index.php/CreateScriptableObjectAsset
        /// </summary>
        public static void CreateAsset<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            CreateAssetFromInstance<T>(asset);
        }

        public static void CreateAssetFromInstance<T>(T assetInstance) where T : ScriptableObject
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }
            
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path +  "/" + typeof(T).Name.ToString() + ".asset");

            AssetDatabase.CreateAsset(assetInstance, assetPathAndName);
            EditorUtility.SetDirty(assetInstance);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = assetInstance;
        }
    }
}
