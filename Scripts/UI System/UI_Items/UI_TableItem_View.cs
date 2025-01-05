using System.Collections.Generic;
using UnityEngine;

namespace WN
{
    public class UI_TableItem_View : MonoBehaviour
    {
        private List<UI_Fruit> poolData = new();
        [SerializeField] private UI_Fruit fruit_ui_prefab;
        [SerializeField] private int poolCount;

        private void Start() => CreatePool();

        private void CreatePool()
        {
            for (int i = 0; i < poolCount; i++)
            {
                UI_Fruit ui = Instantiate(fruit_ui_prefab, transform);
                ui.gameObject.SetActive(false);
                poolData.Add(ui);
            }
        }

        public void SetItems(List<Recipie> recipies)
        {
            DisableItems();
            for (int i = 0; i < recipies.Count; i++)
            {
                poolData[i].Icon = recipies[i].icon;
                poolData[i].gameObject.SetActive(true);
            }
        }

        private void DisableItems()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

}