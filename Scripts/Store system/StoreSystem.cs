using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WN
{
    public class StoreSystem : MonoBehaviour
    {
        private bool isOpen;
        private List<StoreItemData> data;

        private StoreItemData add_two_unburn;
        private StoreItemData buy_unburns;
        private StoreItemData add_time_reset;

        private StoreItemData add_four_unburn;

        [SerializeField] private AppEvent appEvent;
        [SerializeField] private Database database;
        [SerializeField] private Button storeBtn;
        [SerializeField] private Button HomeBtn;

        [SerializeField] private UI_Store_View store_View;
        [SerializeField] private UI_Unburn unburn_view;

        [SerializeField] private Sprite watchAds_sprite;
        [SerializeField] private Sprite payment_sprite;



        private void OnEnable()
        {
            storeBtn.onClick.AddListener(() =>
            {
                isOpen = !isOpen;
                store_View.gameObject.SetActive(isOpen);
                if (isOpen)
                {
                    store_View.Set_Store_Items(data);
                }
            });

            HomeBtn.onClick.AddListener(() =>
            {
                appEvent.OnQuitGameToMenu();
                unburn_view.gameObject.SetActive(false);
                CloseView();
            });

            database.OnDatabaseChanged += Show_Unburn_Item;
        }
        private void OnDisable()
        {
            storeBtn.onClick.RemoveAllListeners();
            database.OnDatabaseChanged -= Show_Unburn_Item;
        }

        private void CloseView()
        {
            isOpen = false;
            store_View.gameObject.SetActive(false);
        }

        public void Start()
        {
            store_View.gameObject.SetActive(false);

            Show_Unburn_Item();
            CreateStoreItems();
        }

        private void CreateStoreItems()
        {
            add_two_unburn = new()
            {
                description = "(x2) Free a blocks",
                buttonIcon = watchAds_sprite,
                buttonText = "Free",
                level_max = 1,
                OnButtonUse = STORE_ACTION_Get_Two_Unburn

            };

            buy_unburns = new()
            {
                description = "Clear 2 blocks",
                buttonIcon = payment_sprite,
                buttonText = "$0.9",
                level_max = 1,
                OnButtonUse = STORE_ACTION_Get_Buy_Unburn
            };

            add_time_reset = new()
            {
                description = "(x2) Reset Time",
                buttonIcon = watchAds_sprite,
                buttonText = "Free",
                level_max = 2,
                OnButtonUse = STORE_ACTION_Get_Time_Reset
            };

            add_four_unburn = new()
            {
                description = "(x4) Free a blocks",
                buttonIcon = watchAds_sprite,
                buttonText = "Free",
                level_max = 3,
                OnButtonUse = STORE_ACTION_Get_Four_Unburn

            };


            data = new(){
                add_two_unburn,
                buy_unburns,
                add_time_reset,
                add_four_unburn
            };
        }

        private void Show_Unburn_Item()
        {
            int val = database.dBValues.db_unburn_remain;
            if (val > 0)
            {
                unburn_view.gameObject.SetActive(true);
                unburn_view.SetValue(val);
            }
            else
            {
                unburn_view.gameObject.SetActive(false);
            }
        }

        private void STORE_ACTION_Get_Two_Unburn() => Ads.Instance.Display(Claim_Unburn).Forget();
        private void Claim_Unburn()
        {
            Ads.Instance.Reward_Add_Two_Unburns(); unburn_view.gameObject.SetActive(true); CloseView();
        }

        private void STORE_ACTION_Get_Buy_Unburn()
        {
            Ads.Instance.Reward_Add_Unburn_Bought(); unburn_view.gameObject.SetActive(true); CloseView();
        }

        private void STORE_ACTION_Get_Time_Reset() => Ads.Instance.Display(Claim_TimeReset).Forget();
        private void Claim_TimeReset() { Ads.Instance.Reward_Add_Time_Reset(); CloseView(); }

        private void STORE_ACTION_Get_Four_Unburn() => Ads.Instance.Display(Claim_Four_Unburn).Forget();
        private void Claim_Four_Unburn()
        {
            Ads.Instance.Reward_Add_Two_Unburns();
            Ads.Instance.Reward_Add_Two_Unburns();
            unburn_view.gameObject.SetActive(true); CloseView();
        }




    }

}