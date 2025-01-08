using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Zenject;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace WN
{
    public class UI_GameOver_View : MonoBehaviour
    {
        private RectTransform rect;

        private Action OnCompletedAds;
        private RectTransform watchAdsRect;
        private Color activeColor;

        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI message;
        [SerializeField] private TextMeshProUGUI menuText;
        [SerializeField] private TextMeshProUGUI adsText;
        [SerializeField] private Button watchAdsBtn;
        [SerializeField] private Image watchImage;
        [SerializeField] private Button menuBtn;



        private void Awake()
        {
            activeColor = watchImage.color;
            watchAdsRect = watchAdsBtn.GetComponent<RectTransform>();
            rect = GetComponent<RectTransform>();
        }
        private void OnEnable() => AnimateDialog().Forget();
        private void OnDisable()
        {
            watchAdsBtn.onClick.RemoveAllListeners();
            menuBtn.onClick.RemoveAllListeners();
            // OnCompletedAds = null;
        }

        private void OnClose()
        {
            AnimateDialog(true).Forget();
        }

        private void ApplyReward_From_Button()
        {
            Ads.Instance.Reward_Add_Coins(watchAdsRect.position);
            OnCompletedAds();
        }


        public void Print(string _title, string _text, string noBtn, string yesBtn, Action menu, Action complete)
        {
            title.text = _title;
            message.text = _text;
            menuText.text = noBtn;
            adsText.text = yesBtn;
            OnCompletedAds = complete;


            watchAdsBtn.interactable = true;
            watchImage.color = activeColor;

            menuBtn.onClick.AddListener(() =>
            {
                OnClose();
                menu?.Invoke();
            });

            watchAdsBtn.onClick.AddListener(() =>
            {
                OnClose();
                watchAdsBtn.interactable = false;
                watchImage.color = Color.cyan;
                Ads.Instance.Display(ApplyReward_From_Button).Forget();
            });
        }

        private async UniTaskVoid AnimateDialog(bool close = false)
        {
            Vector2 original = rect.localScale;
            Vector2 end = original;
            end.x += .1f;
            end.y += .1f;

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
