using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

namespace WN
{
    public class TutorialStep_UI : MonoBehaviour
    {
        public async UniTaskVoid PlayStep(Action complete)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            complete();
        }
    }

}