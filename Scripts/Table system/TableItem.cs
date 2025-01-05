using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;


namespace WN
{
    public class TableItem : MonoBehaviour, IPointerEnterHandler
    {
        private Color defaultColor;
        private Color selectColor;
        private Color burntColor;

        private bool selected;
        private bool burntFX;

        public bool isUsed;
        private ITableContent tableContent;
        private Action<TableItem> OnClick;

        [SerializeField] private RectTransform rect;
        [SerializeField] private Image bg;

        public void Initialize(Action<TableItem> pointerAction, ITableContent item = null)
        {
            if (item != null)
            {
                tableContent = item;
                item.Place(rect.localPosition);
            }
            OnClick = pointerAction;

            float r = UnityEngine.Random.Range(0.5f, 1);
            float g = UnityEngine.Random.Range(0.5f, 1);
            float b = UnityEngine.Random.Range(0.5f, 1);

            //defaultColor = new Color(r,g,b,1);
            //bg.color = defaultColor;

            defaultColor = Color.white;
            selectColor = Color.gray;
            burntColor = Color.black;


        }

        public void Clean()
        {
            burntFX = false;
            isUsed = false;
            UnSelect();
            if (tableContent != null)
            {
                tableContent.Clean();
                tableContent = null;
            }
        }

        public bool IsSoftItem()
        {
            if (tableContent == null)
            {
                return false;
            }
            return tableContent.soft;
        }

        public void Select()
        {
            //bg.color = selectColor;
            selected = true;
            if (!burntFX)
            {
                LerpColor().Forget();
            }
            else
            {
                bg.color = selectColor;
            }

        }

        public void UnSelect()
        {
            bg.color = defaultColor;
            selected = false;
        }

        public void Burn()
        {
            if (tableContent == null)
            {
                isUsed = true;
            }
            else
            {
                isUsed = !tableContent.soft;
            }
            bg.color = burntColor;
            burntFX = true;
        }
        public void UnBurn() { isUsed = false; bg.color = Color.white; burntFX = false; }

        public bool TryUnBurn()
        {
            if (tableContent != null) { return false; }
            isUsed = false;
            bg.color = Color.white;
            return true;
        }

        public void OnPointerEnter(PointerEventData ped)
        {
            OnClick?.Invoke(this);

            if (tableContent != null && !isUsed)
            {
                tableContent.UseContent();
            }
        }

        private async UniTaskVoid LerpColor()
        {
            float sec = 0;
            while (sec < 2 && selected)
            {
                sec += Time.deltaTime * 1.5f;
                bg.color = Color.Lerp(Color.black, selectColor, sec);
                await UniTask.Yield();
            }
            if (!selected)
            {
                bg.color = defaultColor;
            }
            if (burntFX)
            {
                bg.color = burntColor;
            }
        }

    }

}