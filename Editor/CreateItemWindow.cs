using System;
using System.Collections.Generic;
using System.Linq;
using CG.ItemDatabaseSystem.Data;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UniModules.UniGame.AddressableExtensions.Editor;
using UnityEditor;
using UnityEngine;

namespace CG.ItemDatabaseSystem.Editor
{
    public class CreateItemWindow : OdinEditorWindow
    {
        private List<ValueDropdownItem<string>> typeList;

        [ShowInInspector]
        [OnValueChanged(nameof(OnTypeChanged))]
        [ValueDropdown(nameof(typeList))]
        private string Type
        {
            get => EditorPrefs.GetString("CreateItemWindow_Type", typeof(Item).FullName);
            set => EditorPrefs.SetString("CreateItemWindow_Type", value);
        }
        
        [InlineEditor]
        [ShowInInspector]
        private Item Item { get; set; }
        
        protected override void OnEnable()
        {
            base.OnEnable();

            InitializeTypeSelection();

            if (!Item)
            {
                Item = CreateInstance(Type) as Item;
                LoadEditProgress();
            }
        }

        private void InitializeTypeSelection()
        {
            typeList = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(ass => ass.GetTypes().Where(t => t != typeof(Item) && typeof(Item).IsAssignableFrom(t)))
                .Select(t => new ValueDropdownItem<string>
                {
                    Text = t.Name,
                    Value = t.FullName
                }).ToList();
            
            typeList.Insert(0, new ValueDropdownItem<string>
            {
                Text = nameof(Data.Item),
                Value = typeof(Item).FullName
            });

            if (string.IsNullOrWhiteSpace(Type))
            {
                Type = typeof(Item).FullName;
            }
        }

        [Button(ButtonSizes.Large)]
        private void CreateItem()
        {
            var path = $"Assets/Clone Games/Items/{Item.Name}.asset";
                
            if (!AssetDatabase.IsValidFolder("Assets/Clone Games"))
            {
                AssetDatabase.CreateFolder("Assets", "Clone Games");
            }

            if (!AssetDatabase.IsValidFolder("Assets/Clone Games/Items"))
            {
                AssetDatabase.CreateFolder("Assets/Clone Games", "Items");
            }

            if (EditorUtility.DisplayDialog("Creating Item",
                    $"Are you sure you want to create {Item.Name} (ID: {Item.Key}) at path {path}?",
                    "Create", "Cancel"))
            {
                AssetDatabase.CreateAsset(Item, path);
                AddressableHelper.CreateAssetEntry(Item, "Items", "Items");
                Item.SetKey();
                Item.SetAddressableAssetAddress(Item.Key);
                ItemEditorUtility.CreateItemIdFile();
                ClearEditProgress();
                Item = CreateInstance(Type) as Item;
            }
        }

        [Button(ButtonSizes.Large)]
        private void Clear()
        {
            ClearEditProgress();
            Item = CreateInstance(Type) as Item;
        }

        private void OnTypeChanged()
        {
            SaveEditProgress();

            if (AssetDatabase.GetAssetPath(Item).Length < 1)
            {
                DestroyImmediate(Item);
            }
            
            Item = CreateInstance(Type) as Item;
            LoadEditProgress();
        }

        private void LoadEditProgress()
        {
            JsonUtility.FromJsonOverwrite(EditorPrefs.GetString("CreateItemEditor_Item"), Item);
        }
        
        private void SaveEditProgress()
        {
            EditorPrefs.SetString("CreateItemEditor_Item", JsonUtility.ToJson(Item));
        }

        private void ClearEditProgress()
        {
            EditorPrefs.DeleteKey("CreateItemEditor_Item");
        }
    }
}