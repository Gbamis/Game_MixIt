using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ModestTree;

namespace WN
{
    public class Tutorial : MonoBehaviour
    {
        private TutorialInput input_select_bottle;
        private TutorialInput input_select_receipe;

        private TutorialInput input_select_blockedpath;
        private TutorialInput input_clear_blockedPath;
        private TutorialInput input_select_resetTime;

        [Header("Tutorial UI")]
        private TutorialGroup_UI current_tutorial;
        public List<TutorialGroup_UI> tutorialUIs;



        private Action complete_callback;
        private readonly List<Recipie> currentlySelectedRecipies = new();
        private readonly List<Recipie> randomlyChoosenRecipies = new();
        private readonly List<(int, int)> defaultIndicies = new();
        private List<(int, int)> randIndicies = new();
        private List<int> recipieIndicies = new();
        private List<Action> available_tutorials = new();
        private int tutorialIndex = 0;

        [Header("Table Congif")]
        public TableSystem tableSystem;
        public int rowCount;
        public int colCount;

        [Header("Content")]
        [SerializeField] private Coin coin;
        [SerializeField] private UI_Toast_Tutorial_Dialog tutorial_Dialog;
        [SerializeField] private UI_TableItem_View tableList_view;
        [SerializeField] private UI_Timer_View timer_view;
        [SerializeField] private Bottle bottlePrefab;
        [SerializeField] private Fruit fruitPrefab;
        [SerializeField] private List<Recipie> availableReciepe;

        private void Awake()
        {
            //tableSystem.CreateGameGrid(rowCount, colCount, in defaultIndicies); 
            tutorial_Dialog.gameObject.SetActive(false);

            CreateTutorialObjects();

            available_tutorials.Add(Load_Tutorial_Bottle);

            available_tutorials.Add(Load_Tutorial_SecondBottle);

            available_tutorials.Add(Load_Tutorial_ThirdBottle);

            available_tutorials.Add(Load_Tutorial_TryClearPath);

            available_tutorials.Add(Load_Tutorial_ClearPath);

            available_tutorials.Add(Load_Tutorial_TimeReset);

        }

        #region 
        private void CreateTutorialObjects()
        {

            input_select_bottle = new() { name = "select Bottle" };
            input_select_receipe = new() { name = "select recipie" };
            input_select_blockedpath = new() { name = "select a blocked path" };
            input_clear_blockedPath = new() { name = "clear a blocked path" };



        }


        private void BlockInput() => tableSystem.isBoardActive = false;
        private void AllowInput() => tableSystem.isBoardActive = true;

        private void QuitTutorial()
        {
            gameObject.SetActive(false);
            tableSystem.CleanSpawnedItems();
            complete_callback();
        }
        private void RePlayTutorial()
        {
            tableSystem.CleanSpawnedItems();
            SpawnBoard();
            LoadNextTutorial();
        }

        private void LoadNextTutorial()
        {
            if (tutorialIndex < available_tutorials.Count)
            {
                available_tutorials[tutorialIndex].Invoke();
                tutorialIndex++;
            }
            else
            {
                tutorialIndex = 0;
                BlockInput();
                tutorial_Dialog.gameObject.SetActive(true);
                tutorial_Dialog.Display(RePlayTutorial, QuitTutorial);
            }


        }

        private void Load_Tutorial_Bottle()
        {
            int ui = 0;

            tutorialUIs[ui].gameObject.SetActive(true);
            tutorialUIs[ui].InjectActionsAndStart(new List<TutorialInput>(){
                input_select_bottle,
                input_select_receipe
            }, LoadNextTutorial, BlockInput, AllowInput);

            current_tutorial = tutorialUIs[ui];
        }

        private void Load_Tutorial_SecondBottle()
        {
            int ui = 1;

            tutorialUIs[ui].gameObject.SetActive(true);
            tutorialUIs[ui].InjectActionsAndStart(new List<TutorialInput>(){
                input_select_bottle,
                input_select_receipe
            }, LoadNextTutorial, BlockInput, AllowInput);

            current_tutorial = tutorialUIs[ui];
        }

        private void Load_Tutorial_ThirdBottle()
        {
            int ui = 2;

            tutorialUIs[ui].gameObject.SetActive(true);
            tutorialUIs[ui].InjectActionsAndStart(new List<TutorialInput>(){
                input_select_bottle,
                input_select_blockedpath
            }, LoadNextTutorial, BlockInput, AllowInput);

            current_tutorial = tutorialUIs[ui];
        }

        private void Load_Tutorial_TryClearPath()
        {
            Ads.Instance.Reward_Add_Two_Unburns();

            int ui = 3;
            tutorialUIs[ui].gameObject.SetActive(true);
            tutorialUIs[ui].InjectActionsAndStart(new List<TutorialInput>(){
                input_clear_blockedPath
            }, LoadNextTutorial, BlockInput, AllowInput);

            current_tutorial = tutorialUIs[ui];
        }

        private void Load_Tutorial_ClearPath()
        {
            Ads.Instance.Reward_Add_Two_Unburns();

            input_clear_blockedPath = new() { name = "clear a blocked path" };

            int ui = 4;
            tutorialUIs[ui].gameObject.SetActive(true);
            tutorialUIs[ui].InjectActionsAndStart(new List<TutorialInput>(){
                 input_clear_blockedPath ,
                 input_select_receipe
            }, LoadNextTutorial, BlockInput, AllowInput);

            current_tutorial = tutorialUIs[ui];
        }

        private void Load_Tutorial_TimeReset()
        {
            Ads.Instance.Reward_Add_Time_Reset();
            input_select_resetTime = new() { name = "reset time" };

            timer_view.SetTimer(70, () => { }, OnTimerClicked).Forget();


            int ui = 5;

            tutorialUIs[ui].gameObject.SetActive(true);
            tutorialUIs[ui].InjectActionsAndStart(new List<TutorialInput>(){
                input_select_resetTime
            }, LoadNextTutorial, BlockInput, AllowInput);

            current_tutorial = tutorialUIs[ui];
        }

        #endregion

        public void StartObjective(Action OnComplete)
        {

            LoadNextTutorial();
            complete_callback = OnComplete;
            SpawnBoard();
        }

        private void SpawnBoard()
        {
            randIndicies = defaultIndicies.GetRange(0, defaultIndicies.Count);

            recipieIndicies.Clear();
            randomlyChoosenRecipies.Clear();

            tableSystem.SpawnGridItems(
                GenerateBottlesWithContents(),
                GenerateFruits(),
                Callback_Clear_SelectedRecipies,
                OnBurntTileSelected,
                OnClearBurntTile);

            AllowInput();
        }

        private void Callback_Clear_SelectedRecipies() => currentlySelectedRecipies.Clear();
        private void OnFruitAdded(Recipie recipie) => currentlySelectedRecipies.Add(recipie);

        private Dictionary<(int, int), Bottle> GenerateBottlesWithContents()
        {
            Dictionary<(int, int), Bottle> value = new();
            (int, int) point = (4, 2);
            value.Add(point, CreateBottle(1, 3));

            (int, int) point_2 = (1, 6);
            value.Add(point_2, CreateBottle(3, 4));

            (int, int) point_3 = (3, 4);
            value.Add(point_3, CreateBottle(4, 5));
            return value;
        }
        private Dictionary<(int, int), Fruit> GenerateFruits()
        {
            Dictionary<(int, int), Fruit> value = new();
            (int, int) point_1 = (0, 2);
            Fruit fruit_1 = CreateFruit(availableReciepe[0]);

            (int, int) point_2 = (1, 2);
            Fruit fruit_2 = CreateFruit(availableReciepe[1]);

            (int, int) point_3 = (3, 2);
            Fruit fruit_3 = CreateFruit(availableReciepe[2]);

            (int, int) point_4 = (1, 0);
            Fruit fruit_4 = CreateFruit(availableReciepe[3]);

            (int, int) point_5 = (3, 0);
            Fruit fruit_5 = CreateFruit(availableReciepe[4]);

            // value.Add(point_1, fruit_1);
            value.Add(point_2, fruit_2);
            value.Add(point_3, fruit_3);
            value.Add(point_4, fruit_4);
            value.Add(point_5, fruit_5);
            return value;
        }

        private Bottle CreateBottle(int min, int max)
        {
            int randRecpCount = UnityEngine.Random.Range(1, 5);

            Bottle bottle = Instantiate(bottlePrefab, tableSystem.spawnedItems);
            bottle.CreateData(GenerateRandomRecipies(min, max), OnBottleSelected);
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
        private List<Recipie> GenerateRandomRecipies(int min, int max)
        {
            List<Recipie> data = new();
            for (int i = min; i < max; i++)
            {
                Recipie recipie = availableReciepe[i];
                data.Add(recipie);
                if (!randomlyChoosenRecipies.Contains(recipie))
                {
                    randomlyChoosenRecipies.Add(recipie);
                }
            }
            return data;
        }

        public void OnBottleSelected(List<Recipie> recipies, Vector2 pos)
        {
            current_tutorial.SubmitTutorialInput(input_select_bottle);

            tableList_view.SetItems(recipies);

            if (currentlySelectedRecipies.IsEmpty()) { tableSystem.DeSelectAllTableTile(); return; }

            if (currentlySelectedRecipies.SequenceEqual(recipies) && currentlySelectedRecipies.Count == recipies.Count)
            {
                coin.CreditFake(10, pos);
                tableSystem.BurnTableTiles();
                currentlySelectedRecipies.Clear();

                current_tutorial.SubmitTutorialInput(input_select_receipe);
            }
            else
            {
                currentlySelectedRecipies.Clear();
                tableSystem.DeSelectAllTableTile();
            }
        }

        private void OnBurntTileSelected() => current_tutorial.SubmitTutorialInput(input_select_blockedpath);
        private void OnClearBurntTile() => current_tutorial.SubmitTutorialInput(input_clear_blockedPath);
        private void OnTimerClicked() => current_tutorial.SubmitTutorialInput(input_select_resetTime);



    }

}