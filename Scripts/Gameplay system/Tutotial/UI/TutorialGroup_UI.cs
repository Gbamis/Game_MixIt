using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace WN
{
    [Serializable]
    public struct TutorialInput
    {
        public string name;
    }

    public class TutorialGroup_UI : MonoBehaviour
    {
        private Action OnCompleted;
        private Action blockInput;
        private Action allowInput;

        private TutorialInput expected_input;
        private TutorialStep_UI active_ui;
        private int currentstepIndex;
        private Dictionary<TutorialInput, TutorialStep_UI> all_actions_with_ui;


        [SerializeField] private List<TutorialStep_UI> ui_steps;


        public void InjectActionsAndStart(List<TutorialInput> input_actions, Action complete, Action block, Action allow)
        {
            all_actions_with_ui = new();
            for (int i = 0; i < ui_steps.Count; i++)
            {
                TutorialStep_UI step = ui_steps[i];
                all_actions_with_ui.Add(input_actions[i], step);
            }
            blockInput = block;
            allowInput = allow;
            OnCompleted = complete;

            currentstepIndex = 0;
            ShowCurrentStep();
        }

        private void ShowCurrentStep()
        {
            blockInput();
            expected_input = all_actions_with_ui.Keys.ElementAt(currentstepIndex);
            active_ui = all_actions_with_ui.Values.ElementAt(currentstepIndex);

            active_ui.gameObject.SetActive(true);
            active_ui.PlayStep(allowInput).Forget();
        }

        public void SubmitTutorialInput(TutorialInput input)
        {
            if (input.name == expected_input.name)
            {
                NextStep();
            }
        }



        private void NextStep()
        {
            active_ui.gameObject.SetActive(false);
            if (currentstepIndex >= all_actions_with_ui.Count - 1)
            {
                OnCompleted();
                gameObject.SetActive(false);
                return;
            }
            currentstepIndex++;
            ShowCurrentStep();
        }
    }

}