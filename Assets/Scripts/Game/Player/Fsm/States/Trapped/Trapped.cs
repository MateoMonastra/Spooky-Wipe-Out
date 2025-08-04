using System;
using Minigames;
using UnityEngine;

namespace Game.Player
{
    public class Trapped : PlayerState
    {
        private Transform _trappedPos;
        private readonly Minigame _adMinigame;
        private readonly Action _onEscape;

        public Trapped(GameObject player, Minigame adMinigame, Action onEscape)
            : base(player)
        {
            _adMinigame = adMinigame;
            _onEscape = onEscape;
        }

        public override void Enter()
        {
            _adMinigame.OnWin += Escape;
            _adMinigame.OnLose += Escape;
        }

        public override void Tick(float delta)
        {
            if (_trappedPos != null)
                player.transform.position = _trappedPos.position;
        }

        public override void Exit()
        {
            _adMinigame.OnWin -= Escape;
            _adMinigame.OnLose -= Escape;
        }

        private void Escape()
        {
            _onEscape?.Invoke();
        }

        public void SetPos(Transform trappedPos)
        {
            _trappedPos = trappedPos;
        }
    }
}