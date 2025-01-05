using System;
using System.Collections.Generic;
using UnityEngine;

namespace WN
{
    public class Bottle : MonoBehaviour, ITableContent
    {
        private Action<List<Recipie>,Vector2> OnBottleClicked;
        private List<Recipie> recipies;
        public  bool soft { set; get; }

        [SerializeField] private RectTransform rect;

        public void CreateData(List<Recipie> _recp, Action<List<Recipie>,Vector2> checkRecp)
        {
            recipies = _recp;
            OnBottleClicked = checkRecp;
            soft = false;
        }


        public void UseContent() => OnBottleClicked?.Invoke(recipies,Vector2.zero);
        public void Place(Vector2 pos) => rect.localPosition = pos;
        public void Clean() => Destroy(gameObject);
    }
}
