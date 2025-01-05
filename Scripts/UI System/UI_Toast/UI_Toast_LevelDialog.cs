using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using Zenject;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace WN
{
    public class UI_Toast_LevelDialog : MonoBehaviour
    {
        private RectTransform titleRect;
        private RectTransform declineRect;
        private Color declineColor;

        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI noText;
        [SerializeField] private TextMeshProUGUI yesText;
        [SerializeField] private Button acceptBtn;
        [SerializeField] private Button declineBtn;
        [SerializeField] private Image declineImage;



        private void Awake()
        {
            titleRect = title.GetComponent<RectTransform>();
            declineColor = declineImage.color;
            declineRect = declineBtn.GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            acceptBtn.gameObject.SetActive(false);
            declineBtn.gameObject.SetActive(false);
        }
        private void OnDisable()
        {
            acceptBtn.onClick.RemoveAllListeners();
            declineBtn.onClick.RemoveAllListeners();
        }

        private void ApplyReward_From_Button() => Ads.Instance.Reward_Add_Coins(declineRect.position);



        public void Print(int level, string _title, string noBtn, string yesBtn, Action decline, Action accept)
        {
            title.text = "Level " + level;
            noText.text = noBtn;
            yesText.text = yesBtn;

            declineImage.color = declineColor;
            declineBtn.interactable = true;
            AnimateLevelUp().Forget();

            // continue game
            acceptBtn.onClick.AddListener(() =>
            {
                accept?.Invoke();
                gameObject.SetActive(false);
            });

            //show ads to gain coins
            declineBtn.onClick.AddListener(() =>
            {
                decline?.Invoke();
                declineImage.color = Color.green;
                declineBtn.interactable = false;
                Ads.Instance.Display(ApplyReward_From_Button).Forget();
            });
        }

        private async UniTaskVoid AnimateLevelUp()
        {
            Vector2 t_original = titleRect.localScale;
            Vector2 t_end = t_original;
            t_end.x += .3f;
            t_end.y += .3f;

            await titleRect.DOScale(t_end, .2f).OnComplete(() =>
            {
                titleRect.DOScale(t_original, .1f);
            });

            await UniTask.Delay(TimeSpan.FromSeconds(.5));

            acceptBtn.gameObject.SetActive(true);
            declineBtn.gameObject.SetActive(true);
        }
    }

}