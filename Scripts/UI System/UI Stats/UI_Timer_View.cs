using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.EventSystems;

namespace WN
{
    public class UI_Timer_View : MonoBehaviour, IPointerClickHandler
    {
        private Action OnTutorialTimerTapped;
        private bool active;
        private float reset_time;
        private float remain_time;
        private int pause_remaining;

        private bool forcedQuit;

        [SerializeField] private Database database;
        [SerializeField] private Image progress;
        [SerializeField] private GameObject availableBg;
        [SerializeField] private Text sec;


        private void OnEnable()
        {
            database.OnDatabaseChanged += GetTimePause;
            GetTimePause();
            forcedQuit = false;
        }

        private void OnDisable()
        {
            database.OnDatabaseChanged += GetTimePause;
        }

        public void OnPointerClick(PointerEventData ped)
        {
            if (pause_remaining > 0)
            {
                remain_time = reset_time;
                pause_remaining--;
                database.Write_Time_Reset(pause_remaining);

                OnTutorialTimerTapped?.Invoke();
            }
        }

        private void GetTimePause()
        {
            pause_remaining = database.dBValues.db_time_reset;
            availableBg.SetActive(pause_remaining > 0);
        }

        public async UniTaskVoid SetTimer(float max, Action OnStop,Action tutorialTimer=null)
        {
            OnTutorialTimerTapped = tutorialTimer;

            active = true;
            remain_time = reset_time = max;

            while (active)
            {
                remain_time -= Time.deltaTime;
                progress.fillAmount = remain_time / max;
                sec.text = ((int)remain_time).ToString();

                if (remain_time <= 0) { active = false; }
                await UniTask.Yield();
            }
            if (!forcedQuit)
            {
                OnStop?.Invoke();
            }

        }

        public void StopTimer(bool isForced = false)
        {
            active = false;
            forcedQuit = isForced;
        }
    }
}
