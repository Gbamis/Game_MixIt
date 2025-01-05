using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace WN
{
    public class UI_Toast_Tutorial_Dialog : MonoBehaviour
    {
        private RectTransform rect;
        [SerializeField] private Button retryBtn;
        [SerializeField] private Button playGameBtn;
        public float animScale;

        private void Awake() => rect = GetComponent<RectTransform>();

        private void OnEnable() => AnimateDialog().Forget();
        private void OnClose()
        {
            retryBtn.onClick.RemoveAllListeners();
            playGameBtn.onClick.RemoveAllListeners();
            AnimateDialog(true).Forget();
        }

        public void Display(Action retry, Action playGame)
        {
            retryBtn.onClick.AddListener(() =>
            {
                retry();
                OnClose();
            });
            playGameBtn.onClick.AddListener(() =>
            {
                playGame();
                OnClose();
            });
        }



        private async UniTaskVoid AnimateDialog(bool close = false)
        {
            Vector2 original = rect.localScale;
            Vector2 end = original;
            end.x += animScale;
            end.y += animScale;

            await rect.DOScale(end, 0.2f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                rect.DOScale(original, 0.2f);
            });

            if (close)
            {
                gameObject.SetActive(false);
            }
        }
    }

}