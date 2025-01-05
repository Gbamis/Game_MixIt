using UnityEngine;
using UnityEngine.UI;

namespace WN
{
    public class UI_GamePaused_View : MonoBehaviour
    {
        [SerializeField] private AppEvent appEvent;
        [SerializeField] private Button resumeBtn;
        [SerializeField] private Button menuBtn;



        private void OnEnable()
        {
            Time.timeScale = 0;
            resumeBtn.onClick.AddListener(() => Resume());
            menuBtn.onClick.AddListener(() => Menu());
        }

        private void OnDisable()
        {
            resumeBtn.onClick.RemoveAllListeners();
            menuBtn.onClick.RemoveAllListeners();
        }

        private void Resume()
        {
            Time.timeScale = 1;
            gameObject.SetActive(false);
        }

        private void Menu()
        {
            Time.timeScale = 1;
            gameObject.SetActive(false);
            appEvent.OnQuitGameToMenu();
        }
    }

}