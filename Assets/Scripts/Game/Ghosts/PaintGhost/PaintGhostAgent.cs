using System;
using System.Collections.Generic;
using FSM;
using Game.Ghosts.PaintGhost;
using Ghosts.PaintGhost;
using Minigames;
using UnityEngine;
using UnityEngine.Events;

namespace Ghosts
{
    public class PaintGhostAgent : Ghost
    {
        public UnityEvent<bool> OnCatch;
        public UnityEvent<bool> OnDeath;
        public UnityEvent<bool> OnHunt;

        [SerializeField] private Minigame minigame;
        [SerializeField] private Transform trappingPos;
        [SerializeField] private float restTime;

        private List<State> _states = new List<State>();

        private Fsm _fsm;

        private PaintGhostCollision _paintGhostCollision;

        private string _toHuntID = "ToHunt";
        private string _toDeadID = "ToDead";
        private string _toCatchID = "ToCatch";


        public void OnEnable()
        {
            _paintGhostCollision = GetComponentInChildren<PaintGhostCollision>();

            _paintGhostCollision.OnPlayerCollision += minigame.StartGame;
            _paintGhostCollision.OnPlayerCollision += SetCatchState;
        }

        public void Start()
        {
            State _hunt = new Hunt();
            _states.Add(_hunt);

            State _catch = new Catch();
            _states.Add(_catch);

            State _dead = new Dead();
            _states.Add(_dead);

            _hunt.AddTransition(new Transition() { From = _hunt, To = _catch, ID = _toCatchID });

            _catch.AddTransition(new Transition() { From = _catch, To = _dead, ID = _toDeadID });

            _dead.AddTransition(new Transition() { From = _dead, To = _hunt, ID = _toHuntID });

            _fsm = new Fsm(_hunt);
        }

        public void SetHuntState()
        {
            OnHunt?.Invoke(true);
            _fsm.TryTransitionTo(_toHuntID);
            SetCollisionEnabled(true);
        }

        private void SetCatchState()
        {
            OnCatch?.Invoke(true);
            GameManager.GetInstance().GetMinigameSkillCheckController().StopGame();
            minigame.OnWin += SetDeadState;
            minigame.OnLose += SetDeadState;
            _fsm.TryTransitionTo(_toCatchID);
        }

        public void SetDeadState()
        {
            OnDeath?.Invoke(true);
            SetCollisionEnabled(false);
            minigame.OnWin -= SetDeadState;
            minigame.OnLose -= SetDeadState;
            _fsm.TryTransitionTo(_toDeadID);
        }

        public void SetCollisionEnabled(bool isEnabled)
        {
            if (_paintGhostCollision != null)
            {
                _paintGhostCollision.SetActiveCollision(isEnabled);
            }
        }


        private void Update()
        {
            _fsm.Update();
        }

        private void FixedUpdate()
        {
            _fsm.FixedUpdate();
        }
    }
}