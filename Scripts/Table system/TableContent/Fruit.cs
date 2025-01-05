using UnityEngine;
using System;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace WN
{
    public class Fruit : MonoBehaviour, ITableContent
    {
        private bool inUse;
        public bool soft { set; get; }
        private Action<Recipie> OnAdd;
        private Recipie recipie;

        [SerializeField] private RectTransform rect;
        [SerializeField] private Image icon;

        public void CreateData(Recipie _recp, Action<Recipie> add)
        {
            OnAdd = add;
            recipie = _recp;
            icon.sprite = _recp.icon;
            soft = true;
        }

        public void UseContent()
        {
            OnAdd(recipie);
        }
        public void Place(Vector2 pos)
        {
            rect.localPosition = pos;
            BounceFX().Forget();
        }
        public void Clean() => Destroy(gameObject);

        private async UniTaskVoid BounceFX()
        {
            Vector2 original = rect.localScale;
            Vector2 endScale = original;
            endScale.x += .2f;
            endScale.y += .2f;

            float rand = UnityEngine.Random.Range(0.1f, 0.3f);
            await rect.DOScale(endScale, rand).SetEase(Ease.InOutBounce).OnComplete(() =>
            {
                rect.DOScale(original, rand);
            });
        }
    }

}