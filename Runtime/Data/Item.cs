using Sirenix.OdinInspector;
using UnityEngine;

namespace CG.ItemDatabaseSystem.Data
{
    public class Item : ScriptableObject
    {
        [HideInInspector]
        [SerializeField] private string key;
        [SerializeField, LabelText("Name")] private string itemName;
        [SerializeField] private bool consumable;
        [SerializeField] private int maxQuantity;

        public string Key => key;
        
        public string Name => itemName;

        public bool Consumable => consumable;

        public int MaxQuantity => maxQuantity;

        public void SetKey()
        {
            var id = itemName
                .Replace("_", "")
                .Replace("-", "")
                .Replace(" ", "")
                .Replace("!", "")
                .Replace("@", "")
                .Replace("'", "");
            
            key = $"Items/{id}";
        }
    }
}