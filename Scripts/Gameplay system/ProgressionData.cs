using UnityEngine;

namespace WN
{
    [System.Serializable]
    public class ProgressionData
    {
        [Range(1, 5)] public int LEVEL;
        [Range(1, 3)] public int streak;

        public void LevelUp() => LEVEL++;

        public bool IsDone() => streak == 3;
        public void Next() => streak++;
        public void Reset() => streak = 1;

        public int GetNextLevelTargetCoins(int coin)
        {
            int val = LEVEL++ * 20;
            if (coin > val)
            {
                val += coin + (LEVEL * 10);
            }
            return val;
        }

        public (int, int) GetBottleRangeForLevel()
        {
            (int, int) range = (1, 1);
            switch (LEVEL)
            {
                case 1:
                    range = (1, 2);
                    break;
                case 2:
                    range = (1, 2);
                    break;
                case 3:
                    range = (2, 3);
                    break;
                case 4:
                case 5:
                case 6:
                    range = (2, 4);
                    break;
                case 7:
                case 8:
                case 9:
                case 10:
                    range = (3, 3);
                    break;
            }
            return range;
        }

        public (int, int) GetReceipeRangeForLevel()
        {
            (int, int) range = (1, 1);
            switch (LEVEL)
            {
                case 1:
                    range = (1, 3);
                    break;
                case 2:
                    range = (2, 4);
                    break;
                case 3:
                    range = (2, 4);
                    break;
                case 4:
                case 5:
                case 6:
                    range = (2, 4);
                    break;
                case 7:
                case 8:
                case 9:
                case 10:
                    range = (3, 3);
                    break;
            }
            return range;
        }
    }

}