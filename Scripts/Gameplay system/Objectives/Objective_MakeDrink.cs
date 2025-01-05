using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using ModestTree;
using UnityEngine.UI;
using HT;

namespace WN
{
    public class Objective_MakeDrink : MonoBehaviour
    {
        private Action complete_callback;
        private readonly List<Recipie> currentlySelectedRecipies = new();
        private readonly List<Recipie> randomlyChoosenRecipies = new();

        private int bottlesLeft;
        private int totalBottleStreak;
        private int targetCoin;
        private int outstandingTax;
        private float streakDuration = 60;
        private ProgressionData _data;

        private readonly List<(int, int)> defaultIndicies = new();
        private List<(int, int)> randIndicies = new();
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
        [SerializeField] private UI_Fill_View streak_view;

        [Header("Content")]
        [SerializeField] private Database database;
        [SerializeField] private Coin coin;
        [SerializeField] private Economy economy;
        [SerializeField] private Bottle bottlePrefab;
        [SerializeField] private Fruit fruitPrefab;
        [SerializeField] private List<Recipie> availableReciepe;



        private void Awake() => tableSystem.CreateGameGrid(rowCount, colCount, in defaultIndicies);

        private void Start()
        {
            gameover_view.gameObject.SetActive(false);
            levelUpDialog.gameObject.SetActive(false);
        }

        public void StartObjective(ProgressionData data, Action OnComplete)
        {
            _data = data;

            randIndicies = defaultIndicies.GetRange(0, defaultIndicies.Count);

            recipieIndicies.Clear();
            randomlyChoosenRecipies.Clear();

            complete_callback = OnComplete;

            streak_view.SetFill(data.streak);
            level_view.SetFill(data.LEVEL);

            targetCoin = ResolveCoinsByLevel(data.LEVEL);
           // targetCoin = 1;

            streakDuration = 30;
            coin_View.SetTargetCoin(targetCoin);


            outstandingTax = database.dBValues.db_outstanding_tax;
            if (outstandingTax != 0)
            {
                ShowTaxDialog(FixOutstandingTaxation);
                return;
            }

            tableSystem.SpawnGridItems(GenerateBottlesWithContents(), GenerateFruits(), Callback_Clear_SelectedRecipies);
            timer_view.SetTimer(streakDuration, CompleteStreak).Forget();
            tableSystem.isBoardActive = true;
        }

        public void QuitObjective()
        {

            totalBottleStreak = 0;
            bottlesLeft = 0;
            tableSystem.CleanSpawnedItems();
            Callback_Clear_SelectedRecipies();
            recipieIndicies.Clear();
            randomlyChoosenRecipies.Clear();
            tableSystem.isBoardActive = false;
            timer_view.StopTimer(true);

            levelUpDialog.gameObject.SetActive(false);
            gameover_view.gameObject.SetActive(false);
            taxDialog.gameObject.SetActive(false);
            //complete_callback = null;

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
                    targetCoin = _data.GetNextLevelTargetCoins(coin.account_balance);
                    //targetCoin = 1;
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
            Debug.Log("should remove " + outstandingTax);
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


        private Dictionary<(int, int), Bottle> GenerateBottlesWithContents()
        {
            (int, int) range = _data.GetBottleRangeForLevel();
            int numBottles = UnityEngine.Random.Range(range.Item1, range.Item2);

            Dictionary<(int, int), Bottle> value = new();
            for (int i = 0; i < numBottles + 1; i++)
            {
                (int, int) point = GenerateRandomGridPoint();

                value.Add(point, CreateBottle());
                totalBottleStreak++;
            }
            bottlesLeft = value.Values.Count;
            return value;
        }
        private Dictionary<(int, int), Fruit> GenerateFruits()
        {
            Dictionary<(int, int), Fruit> value = new();
            foreach (Recipie rec in randomlyChoosenRecipies)
            {
                (int, int) point = GenerateRandomGridPoint();
                Fruit fruit = CreateFruit(rec);
                value.Add(point, fruit);
            }
            return value;
        }

        private Bottle CreateBottle()
        {
            Bottle bottle = Instantiate(bottlePrefab, tableSystem.spawnedItems);
            bottle.CreateData(GenerateRandomRecipies(), OnBottleSelected);
            bottle.gameObject.SetActive(true);
            return bottle;
        }
        private Fruit CreateFruit(Recipie recipie)
        {
            Fruit fruit = Instantiate(fruitPrefab, tableSystem.spawnedItems);
            fruit.CreateData(recipie, OnFruitAdded);
            fruit.gameObject.SetActive(true);
            return fruit;
        }

        private (int, int) GenerateRandomGridPoint()
        {
            int randIndex = UnityEngine.Random.Range(0, randIndicies.Count);
            (int, int) point = randIndicies[randIndex];
            randIndicies.RemoveAt(randIndex);
            return point;
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

        private void Callback_Clear_SelectedRecipies() => currentlySelectedRecipies.Clear();
        private void OnFruitAdded(Recipie recipie) => currentlySelectedRecipies.Add(recipie);

        public void OnBottleSelected(List<Recipie> recipies, Vector2 pos)
        {

            tableList_view.SetItems(recipies);

            if (currentlySelectedRecipies.IsEmpty()) { tableSystem.DeSelectAllTableTile(); return; }

            if (currentlySelectedRecipies.SequenceEqual(recipies) && currentlySelectedRecipies.Count == recipies.Count)
            {
                coin.Credit(recipies.Count * economy.cost_of_recipie, pos);
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

        private int ResolveCoinsByLevel(int level)
        {
            int coin = 0;
            switch (level)
            {
                case 1:
                    coin = 45;
                    streakDuration = 10;
                    break;
                case 2:
                    coin = 75;
                    streakDuration = 45;
                    break;
                case 3:
                    coin = 200;
                    streakDuration = 40;
                    break;
                case 4:
                    coin = 500;
                    streakDuration = 40;
                    break;
            }
            return coin;
        }

        private void Show_DefaultAds() => Ads.Instance.Display(CompleteDefaultAds).Forget();
        private void CompleteDefaultAds() { tableSystem.isBoardActive = true; ContinueCallback(); }

    }
}
