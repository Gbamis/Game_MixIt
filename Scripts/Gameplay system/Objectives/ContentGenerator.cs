using UnityEngine;
using System.Collections.Generic;
using System;

namespace WN
{
    public struct BottleContentData
    {
        public int totalBottlesGenerated;
        public Dictionary<(int, int), Bottle> bottles;
        public Dictionary<(int, int), CoinBooster> coins;
    }

    public class ContentGenerator
    {
        private List<(int, int)> indexForRandomPoints;

        public BottleContentData GenerateBottlesWithContents(
            ProgressionData _data,
            Bottle bottlePrefab,
            TableSystem tableSystem,
            List<(int, int)> pointIndex
            )
        {
            indexForRandomPoints = pointIndex;

            BottleContentData contentData = new();

            (int, int) range = _data.GetBottleRangeForLevel();
            int numBottles = UnityEngine.Random.Range(range.Item1, range.Item2);

            Dictionary<(int, int), Bottle> bottleValues = new();
            for (int i = 0; i < numBottles + 1; i++)
            {
                Bottle bottle = MonoBehaviour.Instantiate(bottlePrefab, tableSystem.spawnedItems);
                bottle.gameObject.SetActive(true);

                (int, int) point = GenerateRandomGridPoint();

                bottleValues.Add(point, bottle);
            }

            contentData.totalBottlesGenerated = bottleValues.Values.Count;
            contentData.bottles = bottleValues;
            return contentData;
        }

        public Dictionary<(int, int), Fruit> GenerateFruits(
            Fruit fruitPrefab,
            TableSystem tableSystem,
            List<Recipie> randomlyChoosenRecipies,
            Action<Recipie> OnFruitAdded)
        {
            Dictionary<(int, int), Fruit> value = new();
            foreach (Recipie rec in randomlyChoosenRecipies)
            {
                Fruit fruit = MonoBehaviour.Instantiate(fruitPrefab, tableSystem.spawnedItems);
                fruit.CreateData(rec, OnFruitAdded);
                fruit.gameObject.SetActive(true);

                (int, int) point = GenerateRandomGridPoint();
                value.Add(point, fruit);
            }
            return value;
        }

        public BottleContentData GenerateCoinBoosters(
           ProgressionData _data,
           CoinBooster coinPrefab,
           TableSystem tableSystem
           )
        {
            BottleContentData contentData = new();

            Dictionary<(int, int), CoinBooster> coinValues = new();
            for (int i = 0; i < 1; i++)
            {
                CoinBooster coin = MonoBehaviour.Instantiate(coinPrefab, tableSystem.spawnedItems);
                coin.gameObject.SetActive(true);

                (int, int) point = GenerateRandomGridPoint();

                coinValues.Add(point, coin);
            }
            contentData.coins = coinValues;
            return contentData;
        }

        private (int, int) GenerateRandomGridPoint()
        {
            int randIndex = UnityEngine.Random.Range(0, indexForRandomPoints.Count);
            (int, int) point = indexForRandomPoints[randIndex];
            indexForRandomPoints.RemoveAt(randIndex);
            return point;
        }


    }

}