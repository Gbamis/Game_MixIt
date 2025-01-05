using UnityEngine.UI;
using UnityEngine;

namespace WN
{
    public class UI_Coin_View : MonoBehaviour
    {
        private int maxCoin;
        [SerializeField] private AppEvent appEvent;
        [SerializeField] private Coin coin;
        [SerializeField] private Transform root;
        [SerializeField] private UI_AddCoin ui_addCoinPrefab;
        [SerializeField] private Text balannce;
        [SerializeField] private Text targetCoin;
        [SerializeField] private Image progressBar;

        private void OnEnable()
        {
            appEvent.OnGameStart += UpdateCoinText;
            coin.OnAccountChanged += (val, pos) => SetCoinValue(val, pos);
        }
        private void OnDisable()
        {
            appEvent.OnGameStart -= UpdateCoinText;
            coin.OnAccountChanged -= (val, pos) => SetCoinValue(val, pos);
        }

        private void Start()
        {
            // UpdateCoinText();
        }

        private void SetCoinValue(int val, Vector2 pos)
        {
            if (balannce != null)
            {
                balannce.text = coin.account_balance.ToString();
                progressBar.fillAmount = (float)coin.account_balance / maxCoin;
            }
            if (ui_addCoinPrefab != null)
            {
                UI_AddCoin add = Instantiate(ui_addCoinPrefab, root);
                add.gameObject.SetActive(true);
                add.SetAmount(val, pos);
            }
        }

        private void UpdateCoinText()
        {
            if (balannce != null) { balannce.text = coin.account_balance.ToString(); }
        }

        public void SetTargetCoin(int val)
        {
            targetCoin.text = val.ToString();
            maxCoin = val;
            progressBar.fillAmount = (float)coin.account_balance / maxCoin;
        }
    }

}