using UnityEngine.UI;
using UnityEngine;

namespace WN
{
    public class UI_Unburn : MonoBehaviour
    {
        [SerializeField] private Text text;

        public void SetValue(int val) => text.text = val.ToString();
    }

}