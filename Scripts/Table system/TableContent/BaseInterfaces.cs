using UnityEngine;

namespace WN
{
    public interface ITableContent
    {
        bool soft { set; get; }
        void Place(Vector2 pos);
        void UseContent();
        void Clean();
    }

}
