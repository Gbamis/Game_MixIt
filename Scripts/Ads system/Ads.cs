using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using System.Collections;

namespace WN
{
    public class Ads : MonoBehaviour
    {

        private static Ads m_instance;
        public static Ads Instance { set => m_instance = value; get => m_instance; }

        [SerializeField] private Database database;
        [SerializeField] private Coin coin;

        [SerializeField] private GameObject fakeAbdBanner;


        private void Awake()
        {
            Instance = this;
            fakeAbdBanner.SetActive(false);
        }

        public void Reward_Add_Two_Unburns() => database.Write_Unburn_Rem(database.dBValues.db_unburn_remain + 2);
        public void Reward_Add_Unburn_Bought() => database.Write_Unburn_Bought();
        public void Reward_Add_Time_Reset() => database.Write_Time_Reset(2);

        public void Reward_Add_Coins(Vector2 pos)
        {
            coin.Credit(10, pos);
            database.Write_Coins();
        }

        public async UniTaskVoid Display(Action OnCompleted)
        {
            fakeAbdBanner.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(4), ignoreTimeScale: true);
            fakeAbdBanner.SetActive(false);
            OnCompleted();

        }
    }

}