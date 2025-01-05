using UnityEngine;

namespace WN
{
    [CreateAssetMenu(fileName = "Recipie", menuName = "Games/Wine/Recipie")]
    public class Recipie : ScriptableObject
    {
        public Sprite icon;
        public string desc;
        public int value;
    }
}
