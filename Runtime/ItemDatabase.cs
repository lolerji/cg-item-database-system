using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CG.ItemDatabaseSystem.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace CG.ItemDatabaseSystem
{
    public static class ItemDatabase
    {
        private static bool init;
        private static List<Item> items;
        private static Dictionary<string, Item> itemDictionary;

        public static IEnumerable<Item> Items => items;

        public static async Task Initialize()
        {
            items = new List<Item>();
            itemDictionary = new Dictionary<string, Item>();

            var locationsOp = Addressables.LoadResourceLocationsAsync("Items");
            var locations = await locationsOp.Task;

            if (locations.Count > 0)
            {
                var loadItemsOp = Addressables.LoadAssetsAsync<Item>(locations, item =>
                {
                    items.Add(item);
                    itemDictionary.Add(item.Key, item);
                });

                await loadItemsOp.Task;
                Addressables.Release(loadItemsOp);
            }
            
            Addressables.Release(locationsOp);
            init = true;
        }

        public static Item GetItem(string id)
        {
            if (!init)
            {
                throw new InvalidOperationException("Item database is not initialized!");
            }

            if (!itemDictionary.TryGetValue(id, out var item))
            {
                throw new KeyNotFoundException($"Item {id} doesn't exist!");
            }

            return item;
        }
    }
}