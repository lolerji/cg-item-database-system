using UnityEditor;
using UnityEngine;

namespace CG.ItemDatabaseSystem.Editor
{
    public static class Menu
    {
        [MenuItem("Clone Games/Item Database/Create Item", false, 0)]
        public static void OpenCreateItemWindow()
        {
            var window = EditorWindow.GetWindow<CreateItemWindow>();
            window.Show();
        }

        [MenuItem("Clone Games/Item Database/Update Id File", false, 1)]
        public static void GenerateIds()
        {
            ItemEditorUtility.CreateItemIdFile();
        }
    }
}