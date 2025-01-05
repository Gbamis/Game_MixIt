using UnityEngine;
using UnityEngine.UI;

namespace WN
{
    public class UI_Fruit : MonoBehaviour
    {
        [SerializeField] private Image icon;

        public Sprite Icon
        {
            set => icon.sprite = value;
            get => icon.sprite;
        }
    }

}