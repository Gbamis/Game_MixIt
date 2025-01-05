using UnityEngine;
using UnityEngine.UI;

namespace WN
{
    public class UI_System : MonoBehaviour
    {
        private RectTransform newRect;
        private RectTransform conRect;
        private RectTransform exitRect;

        [SerializeField] private AppEvent appEvent;

        public RectTransform selector;

        public GameObject menuCanvas;
        public GameObject statCanvas;
        public GameObject tableCanvas;


        public Button newBtn;
        public Button continueBtn;
        public Button exitButn;

        public void Awake()
        {
            newRect = newBtn.GetComponent<RectTransform>();
            conRect = continueBtn.GetComponent<RectTransform>();
            exitRect = exitButn.GetComponent<RectTransform>();

            Load_Menu_UI();
        }

        private void OnEnable()
        {
            appEvent.OnQuitGameToMenu += Load_Menu_UI;
        }
        private void OnDisable()
        {
            newBtn.onClick.RemoveAllListeners();
            continueBtn.onClick.RemoveAllListeners();
            exitButn.onClick.RemoveAllListeners();

            appEvent.OnQuitGameToMenu -= Load_Menu_UI;
        }

        private void Start()
        {
            Subscribe();
        }

        private void Subscribe()
        {
            newBtn.onClick.AddListener(() =>
            {
                Load_Game_UI();

                selector.position = newRect.position;
                appEvent.OnNewGameStarted();
            });

            continueBtn.onClick.AddListener(() =>
           {
               Load_Game_UI();

               selector.position = conRect.position;
               appEvent.OnContinueGame();
           });

            exitButn.onClick.AddListener(() =>
            {
                selector.position = exitRect.position;
            });

        }

        public void Load_Menu_UI()
        {
            menuCanvas.SetActive(true);
            statCanvas.SetActive(false);
            tableCanvas.SetActive(false);
        }

        private void Load_Game_UI()
        {
            menuCanvas.SetActive(false);
            tableCanvas.SetActive(true);
            statCanvas.SetActive(true);
        }

    }

}