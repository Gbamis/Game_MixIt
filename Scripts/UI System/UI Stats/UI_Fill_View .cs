using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WN
{
    public class UI_Fill_View : MonoBehaviour
    {
        [SerializeField] private Sprite emptyStreak;
        [SerializeField] private Sprite filledStreak;
        [SerializeField] private List<Image> streakImages;


        public void SetFill(int value)
        {
            int index = value - 1;
            for (int i = 0; i < streakImages.Count; i++)
            {
                if (i <= index)
                {
                    streakImages[i].sprite = filledStreak;
                }
                else
                {
                    streakImages[i].sprite = emptyStreak;
                }
            }
        }
    }

}