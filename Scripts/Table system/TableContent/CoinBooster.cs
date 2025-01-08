using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace WN
{
    public class CoinBooster : MonoBehaviour, ITableContent
    {
        public bool soft { set; get; }
        private Action OnAdd;

        [SerializeField] private RectTransform rect;
        [SerializeField] private RectTransform childRect;

        public void CreateData(Action added)
        {
            OnAdd = added;
            soft = true;
        }

        public void UseContent() => OnAdd();
        public void Place(Vector2 pos)
        {
            rect.localPosition = pos;
            childRect = rect.GetChild(0).GetComponent<RectTransform>();
            BounceFX().Forget();
        }
        public void Clean() => Destroy(gameObject);

        private async UniTaskVoid BounceFX()
        {
            Vector2 original = rect.localScale;
            Vector2 endScale = original;
            endScale.x += .02f;
            endScale.y += .02f;

            float rand = UnityEngine.Random.Range(0.3f, 0.5f);
            await  childRect.DOScale(endScale, rand)
            .SetLoops(-1,LoopType.Yoyo)
            .SetEase(Ease.InOutBounce).OnComplete(() =>
            {
                 childRect.DOScale(original, rand);
            });
        }
    }

}