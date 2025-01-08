using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace WN
{
    public class UI_Store_View : MonoBehaviour
    {
        [SerializeField] private Database database;
        [SerializeField] private UI_StoreItem prefab;
        [SerializeField] private Transform listView;
        [SerializeField] private RectTransform menuRect;

        private void OnEnable() => AnimateDialog().Forget();
        private void OnDisable() => Time.timeScale = 1;

        public void Set_Store_Items(List<StoreItemData> data)
        {
            ClearList();

            foreach (StoreItemData dt in data)
            {
                UI_StoreItem ui = Instantiate(prefab, listView);
                ui.gameObject.SetActive(true);
                ui.SetData(dt, database.dBValues.db_level);
            }
        }

        private void ClearList()
        {
            if (listView.childCount > 0)
            {
                foreach (Transform child in listView)
                {
                    Destroy(child.gameObject);
                }
            }

        }

        private async UniTaskVoid AnimateDialog(bool close = false)
        {
            Vector2 original = menuRect.localScale;
            Vector2 end = original;
            end.x += 0.2f;
            end.y += 0.2f;

            await menuRect.DOScale(end, 0.4f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                menuRect.DOScale(original, 0.3f);
            });
            Time.timeScale = 0;

            if (close)
            {
                gameObject.SetActive(false);
            }
        }
    }

}