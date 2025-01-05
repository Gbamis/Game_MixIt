using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace WN
{
    public class UI_Toast_TaxDialog : MonoBehaviour
    {
        private RectTransform rect;
        [SerializeField] private TextMeshProUGUI bottleCount;
        [SerializeField] private TextMeshProUGUI totalTax;
        [SerializeField] private TextMeshProUGUI coinsTxt;
        [SerializeField] private Button payBtn;
        [SerializeField] private Button menuBtn;
        [SerializeField] private Image targetCoinProgress;


        public float animScale;

        private void Awake() => rect = GetComponent<RectTransform>();

        private void OnEnable() => AnimateDialog().Forget();
        private void OnClose()
        {
            AnimateDialog(true).Forget();
        }


        public void Print(int bottle, int cost, Action pay, Action menu, int coins, int target)
        {
            bottleCount.text = bottle.ToString();
            totalTax.text = cost.ToString();
            targetCoinProgress.fillAmount = (float)coins / target;
            coinsTxt.text = coins.ToString() + "/" + target.ToString();

            payBtn.onClick.AddListener(() =>
            {
                pay();
                payBtn.onClick.RemoveAllListeners();
                OnClose();
            });
            menuBtn.onClick.AddListener(() =>
            {
                menu();
                menuBtn.onClick.RemoveAllListeners();
                OnClose();
            });

        }

        private async UniTaskVoid AnimateDialog(bool close = false)
        {
            Vector2 original = rect.localScale;
            Vector2 end = original;
            end.x += animScale;
            end.y += animScale;

            await rect.DOScale(end, 0.2f).OnComplete(() =>
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