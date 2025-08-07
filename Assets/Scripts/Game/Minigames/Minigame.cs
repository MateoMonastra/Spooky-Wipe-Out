using System;
using UnityEngine;

namespace Game.Minigames
{
    public abstract class Minigame: MonoBehaviour
    {
        public Action OnWin;
        public Action OnLose;
        public Action OnStart;
        public Action OnStop;

        protected bool IsActive;
        protected bool IsBloqued;
        protected float Progress;

        public bool GetActive() => IsActive;

        protected virtual void WinGame() => OnWin?.Invoke();
        protected virtual void LoseGame() => OnLose?.Invoke();
        protected abstract void ResetGame();
        public abstract void StartGame();
        public abstract void StopGame();
        
        public float GetProgress() => Progress;
        public void SetBloqued(bool isbloqued) => IsBloqued = isbloqued;
    }
}