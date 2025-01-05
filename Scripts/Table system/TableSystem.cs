using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace WN
{
    public class TableSystem : MonoBehaviour
    {
        private Action OnClearReciepies_callback;
        private Action OnBurnSelected;

        private int unburnRemaining = 0;
        private bool isDragging;
        [HideInInspector] public bool isBoardActive;

        private (int, int)[,] points;
        private Dictionary<(int, int), TableItem> links = new();
        private readonly List<TableItem> selectedTableTiles = new();

        [SerializeField] private Database database;
        [SerializeField] private TableItem tilePrefab;
        [SerializeField] private Transform origin;
        [SerializeField] private float x_Span;
        [SerializeField] private float y_span;
        public Transform spawnedItems;


        private void OnEnable()
        {
            database.OnDatabaseChanged += Fetch_Unburn;
        }

        private void OnDisable()
        {
            database.OnDatabaseChanged -= Fetch_Unburn;
        }

        public void CreateGameGrid(int rowMax, int colMax, in List<(int, int)> index)
        {
            //build table grid layout

            points = new (int, int)[rowMax, colMax];

            for (int col = 0; col < colMax; col++)
            {
                for (int row = 0; row < rowMax; row++)
                {
                    points[row, col] = (row, col);

                    Vector2 pos = GridToWorld((row, col));
                    TableItem clone = Instantiate(tilePrefab, origin);
                    clone.gameObject.SetActive(true);
                    clone.GetComponent<RectTransform>().localPosition = pos;
                    clone.Initialize(OnSelectTableTile, null);

                    links.Add((row, col), clone);

                    (int, int) point = (row, col);
                    index.Add(point);
                }
            }

        }

        public void SpawnGridItems(Dictionary<(int, int), Bottle> bottles,
        Dictionary<(int, int), Fruit> fruits,
         Action OnClearRecipies,
         Action OnBurnTapped = null)
        {

            OnClearReciepies_callback = OnClearRecipies;
            OnBurnSelected = OnBurnTapped;

            foreach (KeyValuePair<(int, int), Bottle> keyValue in bottles)
            {
                TableItem tableItem = links[keyValue.Key];
                Bottle bottle = keyValue.Value;

                tableItem.Initialize(OnSelectTableTile, bottle);
            }

            foreach (KeyValuePair<(int, int), Fruit> keyValue in fruits)
            {
                TableItem tableItem = links[keyValue.Key];
                Fruit fruit = keyValue.Value;

                tableItem.Initialize(OnSelectTableTile, fruit);
            }
        }

        private void Fetch_Unburn() => unburnRemaining = database.dBValues.db_unburn_remain;

        public void CleanSpawnedItems()
        {
            isDragging = false;
            OnClearReciepies_callback = null;
            UnBurnTableTiles();
            DeSelectAllTableTile();
            foreach (Transform child in spawnedItems) { Destroy(child.gameObject); }
        }


        private Vector2 GridToWorld((int, int) point)
        {
            float x = point.Item1 * x_Span;
            float y = point.Item2 * y_span;
            return new Vector2(x, y);
        }

        private void Update() => CheckDrag();

        private void CheckDrag()
        {
            if (!isBoardActive) { return; }

            if (OnClearReciepies_callback == null) { return; }
            if (Input.GetMouseButtonDown(0) && !isDragging) { isDragging = true; }

            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                OnClearReciepies_callback();
                DeSelectAllTableTile();
                isDragging = false;
            }
        }

        public void OnSelectTableTile(TableItem tableItem)
        {
            if (!isBoardActive) { return; }

            if (tableItem.isUsed || selectedTableTiles.Contains(tableItem))
            {
                if (unburnRemaining > 0)
                {
                    if (tableItem.TryUnBurn())
                    {
                        OnBurnSelected?.Invoke();

                        tableItem.UnSelect();
                        selectedTableTiles.Remove(tableItem);
                        unburnRemaining -= 1;
                        database.Write_Unburn_Rem(unburnRemaining);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    DeSelectAllTableTile();
                    return;
                }

            }

            selectedTableTiles.Add(tableItem);
            tableItem.Select();

        }

        public void DeSelectAllTableTile()
        {
            foreach (TableItem item in selectedTableTiles) { item.UnSelect(); }
            selectedTableTiles.Clear();
        }

        public void BurnTableTiles()
        {
            foreach (TableItem item in selectedTableTiles)
            {
                item.UnSelect();
                item.Burn();
                if (!item.IsSoftItem())
                {

                }

            }
            selectedTableTiles.Clear();
        }
        public void UnBurnTableTiles()
        {
            foreach (KeyValuePair<(int, int), TableItem> keyValue in links)
            {
                keyValue.Value.UnBurn();
                keyValue.Value.Clean();
            }
        }
    }
}
