using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace WN
{
    public class UI_AddCoin : MonoBehaviour
    {
        private RectTransform rect;
        [SerializeField] private Text amountTxt;

        private void Awake() => rect = GetComponent<RectTransform>();
        public void SetAmount(int value, Vector2 pos)
        {
            amountTxt.text = "+" + value.ToString();
            Animate(pos).Forget();
        }

        private async UniTaskVoid Animate(Vector2 pos)
        {
            if (pos != Vector2.zero)
            {
                rect.position = pos;
            }

            Vector2 end = pos;
            end.y += 1;
            await rect.DOLocalMoveY(end.y, 1);
            //await rect.DOMove(end, .5f);
            Destroy(gameObject);
        }
    }

}