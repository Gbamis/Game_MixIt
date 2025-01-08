using UnityEngine;
using UnityEngine.UI;


namespace WN
{
    public class GamePlaySystem : MonoBehaviour
    {
        private ProgressionData progressionData = new();
        [SerializeField] private AppEvent appEvent;
        [SerializeField] private Database database;


        [Header("Game Objectives")]
        [SerializeField] Objective_MakeDrink objective_MakeDrink;
        [SerializeField] Tutorial tutorial;
        public bool done;

        private void OnEnable()
        {
            appEvent.OnGameStart += OnGamePlayStart;
            appEvent.OnQuitGameToMenu += OnGameObjectiveQuit;
        }

        private void OnDisable()
        {
            appEvent.OnGameStart -= OnGamePlayStart;
            appEvent.OnQuitGameToMenu -= OnGameObjectiveQuit;
        }


        private void OnGamePlayStart()
        {
            if (!done)
            {
                tutorial.StartObjective(OnGameObjectiveCompleted);
            }
            else
            {
                OnGameObjectiveCompleted();
            }

        }

        private void OnGameObjectiveCompleted()
        {
            progressionData.LEVEL = database.dBValues.db_level;
            progressionData.streak = database.dBValues.db_streak;
            objective_MakeDrink.StartObjective(progressionData, OnGameObjectiveCompleted);
        }

        private void OnGameObjectiveQuit()
        {
            objective_MakeDrink.QuitObjective();
        }


    }

}