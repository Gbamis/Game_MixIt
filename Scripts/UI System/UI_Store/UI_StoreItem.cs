using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WN
{
    public class UI_StoreItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private Image btnIcon;
        [SerializeField] private TextMeshProUGUI btnTxt;
        [SerializeField] private Button useBtn;
        [SerializeField] private UI_Fill_View fill_View;
        [SerializeField] private GameObject blocker;

        public void SetData(StoreItemData storeItemData, int level)
        {
            description.text = storeItemData.description;
            btnIcon.sprite = storeItemData.buttonIcon;
            btnTxt.text = storeItemData.buttonText;
            fill_View.SetFill(storeItemData.level_max);

            useBtn.onClick.AddListener(() =>
            {
                storeItemData.OnButtonUse();
            });

            blocker.SetActive(level < storeItemData.level_max);
        }

    }

}