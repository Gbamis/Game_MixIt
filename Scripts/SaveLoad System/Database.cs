using System;
using UnityEngine;

namespace WN
{
    [Serializable]
    public struct DBValues
    {
        public int db_coin;
        public int db_streak;
        public int db_level;
        public int db_outstanding_tax;
        public bool db_has_Unburn;
        public int db_unburn_remain;
        public int db_time_reset;
    }

    [CreateAssetMenu(fileName = "Database", menuName = "Games/Wine/Database")]

    public class Database : ScriptableObject
    {
        public Action OnDatabaseChanged;

        [SerializeField] private Coin coin;
        public DBValues dBValues;

        public void Reset_DB()
        {
            coin.account_balance = dBValues.db_coin = 20;
            dBValues.db_level = 1;
            dBValues.db_streak = 1;
            dBValues.db_outstanding_tax = 0;
            dBValues.db_unburn_remain = 0;
            dBValues.db_has_Unburn = false;
            dBValues.db_time_reset = 0;

            OnDatabaseChanged?.Invoke();
        }
        public void SetValues(DBValues values)
        {
            dBValues = values;
            coin.account_balance = dBValues.db_coin;
        }

        public void Write_Coins()
        {
            dBValues.db_coin = coin.account_balance;
            OnDatabaseChanged?.Invoke();
        }

        public void Write_Level(int level)
        {
            dBValues.db_level = level;
            OnDatabaseChanged?.Invoke();
        }

        public void Write_Streak(int value)
        {
            dBValues.db_streak = value;
            OnDatabaseChanged?.Invoke();
        }

        public void Write_Tax(int value)
        {
            dBValues.db_outstanding_tax = value;
            OnDatabaseChanged?.Invoke();
        }

        public void Write_Unburn_Bought()
        {
            dBValues.db_has_Unburn = true;
            OnDatabaseChanged?.Invoke();
        }

        public void Write_Unburn_Rem(int val)
        {
            dBValues.db_unburn_remain = val;
            OnDatabaseChanged?.Invoke();
        }

        public void Write_Time_Reset(int val)
        {
            dBValues.db_time_reset = val;
            OnDatabaseChanged?.Invoke();
        }
    }
}
