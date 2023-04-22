using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FaRUtils.Systems.ItemSystem
{
    [CreateAssetMenu(menuName = "Jueguito Granjil/Item Database")]
    public class Database : ScriptableObject
    {
        [SerializeField] private List<InventoryItemData> _itemDatabase;

        public List<InventoryItemData> ItemDatabase => _itemDatabase;

        [ContextMenu("Fijar IDs")]
        public void SetItemIDs()
        {
            _itemDatabase = new List<InventoryItemData>();
        
            var foundItems = UnityEngine.Resources.LoadAll<InventoryItemData>("ItemData").OrderBy(i => i.ID).ToList();
        
            var hasIDInRange = foundItems.Where(i => i.ID != -1 && i.ID < foundItems.Count).OrderBy(i => i.ID).ToList();
            var hasIDNotInRange = foundItems.Where(i => i.ID != -1 && i.ID >= foundItems.Count).OrderBy(i => i.ID).ToList();
            var noID = foundItems.Where(i => i.ID == -1).ToList();

            var index = 0;
            for (int i = 0; i < foundItems.Count; i++)
            {
                Debug.Log($"Checkeando ID {i}");
                var itemToAdd = hasIDInRange.Find(d => d.ID == i);
             
                if (itemToAdd != null)
                {
                    Debug.Log($"Se encontró un item {itemToAdd} que tiene un ID de {itemToAdd.ID}");
                    _itemDatabase.Add(itemToAdd);
                }
                else if(index < noID.Count)
                {
                    noID[index].ID = i;
                    Debug.Log($"Fijando item {noID[index]} que tenía un ID inválido al ID {i}");
                    itemToAdd = noID[index];
                    index++;
                    _itemDatabase.Add(itemToAdd);
                }
#if UNITY_EDITOR
                if (itemToAdd) EditorUtility.SetDirty(itemToAdd);
#endif
               
            }
         
            foreach (var item in hasIDNotInRange)
            {
                _itemDatabase.Add(item);
#if UNITY_EDITOR
                if (item) EditorUtility.SetDirty(item);
#endif
            }
 #if UNITY_EDITOR       
            AssetDatabase.SaveAssets();
#endif
        
        
        }

        public InventoryItemData GetItem(int id)
        {
            for (int i=0; i< _itemDatabase.Count; i++) {
                if (_itemDatabase[i].ID == id) {
                    return _itemDatabase[i] as InventoryItemData;
                }
            }
            return null;
        }
    
        public InventoryItemData GetItem(string displayName)
        {
            return _itemDatabase.Find(i => i.Nombre == displayName) as InventoryItemData;
        }
    }
}