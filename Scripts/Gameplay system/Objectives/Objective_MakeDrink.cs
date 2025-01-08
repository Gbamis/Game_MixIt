using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using ModestTree;
using UnityEngine.UI;
using HT;
using TMPro;

namespace WN
{
    public class Objective_MakeDrink : MonoBehaviour
    {
        private Action complete_callback;
        private readonly List<Recipie> currentlySelectedRecipies = new();
        private readonly List<Recipie> randomlyChoosenRecipies = new();

        [SerializeField] private int bottlesLeft;
        private int totalBottleStreak;
        private int targetCoin;
        private int outstandingTax;
        private bool isFreshLevel = true;
        private int hasCoinBooster = 1;
        private ProgressionData _data;

        private readonly List<(int, int)> defaultIndicies = new();
        private List<int> recipieIndicies = new();

        [Header("Table Congif")]
        public TableSystem tableSystem;
        public int rowCount;
        public int colCount;

        [Header("UI")]
        [SerializeField] private UI_Coin_View coin_View;
        [SerializeField] private UI_Timer_View timer_view;
        [SerializeField] private UI_TableItem_View tableList_view;
        [SerializeField] private UI_Toast_LevelDialog levelUpDialog;
        [SerializeField] private UI_GameOver_View gameover_view;
        [SerializeField] private UI_Toast_TaxDialog taxDialog;
        [SerializeField] private UI_Fill_View level_view;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private UI_Fill_View streak_view;

        [Header("Content")]
        [SerializeField] private Database database;
        [SerializeField] private Coin coin;
        [SerializeField] private Economy economy;
        [SerializeField] private Bottle bottlePrefab;
        [SerializeField] private Fruit fruitPrefab;
        [SerializeField] private CoinBooster coinBoostPrefab;
        [SerializeField] private List<Recipie> availableReciepe;

        private ContentGenerator contentGenerator;



        private void Awake() => tableSystem.CreateGameGrid(rowCount, colCount, in defaultIndicies);

        private void Start()
        {
            gameover_view.gameObject.SetActive(false);
            levelUpDialog.gameObject.SetActive(false);

            contentGenerator = new();
        }

        public void StartObjective(ProgressionData data, Action OnComplete)
        {
            _data = data;
            complete_callback = OnComplete;

            
            streak_view.SetFill(data.streak);
            level_view.SetFill(data.LEVEL);
            levelText.text = data.LEVEL.ToString();


            outstandingTax = database.dBValues.db_outstanding_tax;
            if (outstandingTax != 0) { ShowTaxDialog(FixOutstandingTaxation); return; }

            if (isFreshLevel)
            {
                targetCoin = _data.GetNextLevelTargetCoins(coin.account_balance);
                isFreshLevel = false;
            }
            coin_View.SetTargetCoin(targetCoin);
            
            ClearObjectiveData();
            //Create_Simple_ObjectiveDataByProgression();
            Create_Simple_ObjectiveData_With_Coin_ByProgression();
        }


        public void QuitObjective()
        {
            totalBottleStreak = 0;
            bottlesLeft = 0;
            ClearObjectiveData();
            tableSystem.isBoardActive = false;
            timer_view.StopTimer(true);

            levelUpDialog.gameObject.SetActive(false);
            gameover_view.gameObject.SetActive(false);
            taxDialog.gameObject.SetActive(false);
        }

        private void ClearObjectiveData()
        {
            recipieIndicies.Clear();
            randomlyChoosenRecipies.Clear();

            tableSystem.CleanSpawnedItems();
            Callback_Clear_SelectedRecipies();
        }

        private void Create_Simple_ObjectiveDataByProgression()
        {
            BottleContentData bottleContentData = contentGenerator.GenerateBottlesWithContents(
                _data, bottlePrefab, tableSystem, tableSystem.indexForRandomPoints);
            foreach (KeyValuePair<(int, int), Bottle> keyValue in bottleContentData.bottles)
            {
                keyValue.Value.CreateData(GenerateRandomRecipies(), OnBottleSelected);
            }

            Dictionary<(int, int), Fruit> generatedFruits = contentGenerator.GenerateFruits(
                fruitPrefab, tableSystem, randomlyChoosenRecipies, OnFruitAdded);

            tableSystem.SpawnGridItems(bottleContentData.bottles, generatedFruits, Callback_Clear_SelectedRecipies);
            timer_view.SetTimer(30, CompleteStreak).Forget();

            bottlesLeft = bottleContentData.totalBottlesGenerated;
            totalBottleStreak += bottleContentData.totalBottlesGenerated;

            tableSystem.isBoardActive = true;

            PowerUps();
        }

        private void Create_Simple_ObjectiveData_With_Coin_ByProgression()
        {
            int size = tableSystem.indexForRandomPoints.Count;

            List<(int, int)> indexForRandomPoints = tableSystem.indexForRandomPoints.GetRange(0, size);

            BottleContentData bottleContentData = contentGenerator.GenerateBottlesWithContents(
                _data, bottlePrefab, tableSystem, indexForRandomPoints);
            foreach (KeyValuePair<(int, int), Bottle> keyValue in bottleContentData.bottles)
            {
                keyValue.Value.CreateData(GenerateRandomRecipies(), OnBottleSelected);
            }

            Dictionary<(int, int), Fruit> generatedFruits = contentGenerator.GenerateFruits(
                fruitPrefab, tableSystem, randomlyChoosenRecipies, OnFruitAdded);

            BottleContentData generatedCoinData = contentGenerator.GenerateCoinBoosters(
                _data, coinBoostPrefab, tableSystem);
            foreach (KeyValuePair<(int, int), CoinBooster> keyValue in generatedCoinData.coins)
            {
                keyValue.Value.CreateData(OnCoinBoosterAdded);
            }

            tableSystem.SpawnGridItemsWithCoinBoost(
                bottleContentData.bottles,
                generatedFruits, generatedCoinData.coins,
                Callback_Clear_SelectedRecipies);

            timer_view.SetTimer(50, CompleteStreak).Forget();
            bottlesLeft = bottleContentData.totalBottlesGenerated;
            totalBottleStreak += bottleContentData.totalBottlesGenerated;

            tableSystem.isBoardActive = true;

            PowerUps();
        }

        private void PowerUps()
        {
            if (database.dBValues.db_unburn_remain < 2)
            {
                Ads.Instance.Reward_Add_Two_Unburns();
            }
            if (isFreshLevel)
            {
                Ads.Instance.Reward_Add_Time_Reset();
            }
        }

        private List<Recipie> GenerateRandomRecipies()
        {
            (int, int) range = _data.GetReceipeRangeForLevel();
            int randRecpCount = UnityEngine.Random.Range(range.Item1, range.Item2);

            recipieIndicies = Enumerable.Range(0, availableReciepe.Count - 1).ToList();
            List<Recipie> data = new();
            for (int i = 0; i < randRecpCount; i++)
            {
                int index = UnityEngine.Random.Range(0, recipieIndicies.Count);
                int val = recipieIndicies[index];

                Recipie recipie = availableReciepe[val];
                data.Add(recipie);
                if (!randomlyChoosenRecipies.Contains(recipie))
                {
                    randomlyChoosenRecipies.Add(recipie);
                }
                recipieIndicies.RemoveAt(index);
            }
            return data;
        }

        private void ContinueCallback()
        {
            tableSystem.CleanSpawnedItems();
            complete_callback();
        }

        public void CompleteStreak()
        {
            if (_data.IsDone())
            {
                _data.Reset();
                database.Write_Streak(_data.streak);

                if (coin.account_balance >= targetCoin)
                {
                    tableSystem.isBoardActive = false;
                    levelUpDialog.gameObject.SetActive(true);
                    levelUpDialog.Print(_data.LEVEL, "", "X10 Coins", "Continue", () => { }, ContinueCallback);
                    _data.LevelUp();
                    database.Write_Level(_data.LEVEL);

                    isFreshLevel = true;
                }
                else
                {
                    //int cost = totalBottleStreak * economy.cost_of_bottle;
                    outstandingTax = totalBottleStreak * economy.cost_of_bottle;
                    if (coin.CanDebit(outstandingTax))
                    {
                        ShowTaxDialog(ApplyTaxation);
                    }
                    else
                    {
                        tableSystem.isBoardActive = false;
                        gameover_view.gameObject.SetActive(true);
                        gameover_view.Print("GAME OVER", "Insufficient Coins", "Menu", "Revive", () => { }, ContinueCallback);
                    }
                }
                totalBottleStreak = 0;
            }
            else
            {
                _data.Next();
                database.Write_Streak(_data.streak);
                ContinueCallback();
            }

        }

        private void ShowTaxDialog(Action callback)
        {
            tableSystem.isBoardActive = false;
            taxDialog.gameObject.SetActive(true);
            database.Write_Tax(outstandingTax);
            taxDialog.Print(totalBottleStreak, outstandingTax, callback, QuitObjective, coin.account_balance, targetCoin);
        }

        private void ApplyTaxation()
        {
            coin.Debit(outstandingTax);
            outstandingTax = 0;
            database.Write_Coins();
            database.Write_Tax(0);

            Show_DefaultAds();
        }

        private void FixOutstandingTaxation()
        {
            coin.Debit(outstandingTax);
            database.Write_Coins();
            database.Write_Tax(0);
            ContinueCallback();
        }


        private void Callback_Clear_SelectedRecipies()
        {
            currentlySelectedRecipies.Clear();
            hasCoinBooster = 1;
        }
        private void OnFruitAdded(Recipie recipie) => currentlySelectedRecipies.Add(recipie);
        private void OnCoinBoosterAdded() => hasCoinBooster = 2;

        public void OnBottleSelected(List<Recipie> recipies, Vector2 pos)
        {

            tableList_view.SetItems(recipies);

            if (currentlySelectedRecipies.IsEmpty()) { tableSystem.DeSelectAllTableTile(); return; }

            if (currentlySelectedRecipies.SequenceEqual(recipies) && currentlySelectedRecipies.Count == recipies.Count)
            {
                coin.Credit(recipies.Count * economy.cost_of_recipie * hasCoinBooster, pos);
                database.Write_Coins();

                tableSystem.BurnTableTiles();
                currentlySelectedRecipies.Clear();

                bottlesLeft--;
                if (bottlesLeft == 0)
                {
                    timer_view.StopTimer();
                }
            }
            else
            {
                currentlySelectedRecipies.Clear();
                tableSystem.DeSelectAllTableTile();
            }
        }



        private void Show_DefaultAds() => Ads.Instance.Display(CompleteDefaultAds).Forget();
        private void CompleteDefaultAds() { tableSystem.isBoardActive = true; ContinueCallback(); }

    }
}
