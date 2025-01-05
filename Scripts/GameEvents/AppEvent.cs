using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AppEvent", menuName = "Games/Wine/AppEvent")]
public class AppEvent : ScriptableObject
{
    public Action OnNewGameStarted;
    public Action OnContinueGame;
    public Action OnGameStart;
    public Action OnQuitGameToMenu;
    
    public Action OnAdsCompleted;
}
