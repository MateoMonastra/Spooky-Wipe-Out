using System;
using System.Collections.Generic;
using AK.Wwise;
using Fsm_Mk2;
using Game.Ghosts.WallGhost;
using Ghosts.WallGhost;
using Minigames;
using UnityEngine;
using UnityEngine.Events;
using State = Fsm_Mk2.State;

namespace Ghosts
{
    public class WallGhostAgent : Ghost
    {
        public UnityEvent<bool> OnCatch;
        public UnityEvent<bool> OnDeath;
        public UnityEvent<bool> OnHunt;

        [SerializeField] private Minigame minigame;
        [SerializeField] private Transform trappingPos;
        [SerializeField] private float restTime;
        
        private List<State> _states = new List<State>();

        private Fsm _fsm;

        private Transition _huntToCatch;
        private Transition _catchToDead;
        private Transition _deadToHunt;

        private WallGhostCollision _wallGhostCollision;

        public void OnEnable()
        {
            _wallGhostCollision = GetComponentInChildren<WallGhostCollision>();

            _wallGhostCollision.OnPlayerCollision += minigame.StartGame;
            _wallGhostCollision.OnPlayerCollision += SetCatchState;
        }

        public void Start()
        {
            State _hunt = new Hunt();
            _states.Add(_hunt);

            State _catch = new Catch();
            _states.Add(_catch);

            State _dead = new Dead();
            _states.Add(_dead);

            _huntToCatch = new Transition() { From = _hunt, To = _catch };
            _hunt.transitions.Add(_huntToCatch);

            _catchToDead = new Transition() { From = _catch, To = _dead };
            _catch.transitions.Add(_catchToDead);

            _deadToHunt = new Transition() { From = _dead, To = _hunt };
            _dead.transitions.Add(_catchToDead);

            _fsm = new Fsm(_hunt);
        }

        public void SetHuntState() 
        {
            OnHunt?.Invoke(true);
            _fsm.ApplyTransition(_deadToHunt);
            SetCollisionEnabled(true);
        }

        private void SetCatchState()
        {
            OnCatch?.Invoke(true);
            GameManager.GetInstance().GetMinigameSkillCheckController().StopGame();
            minigame.OnWin += SetDeadState;
            minigame.OnLose += SetDeadState;
            _fsm.ApplyTransition(_huntToCatch);
        }
        
        public void SetDeadState()
        {
            OnDeath?.Invoke(true);
            SetCollisionEnabled(false);
            minigame.OnWin -= SetDeadState;
            minigame.OnLose -= SetDeadState;
            _fsm.ApplyTransition(_catchToDead);
        }

        public void SetCollisionEnabled(bool isEnabled)
        {
            if (_wallGhostCollision != null)
            {
                _wallGhostCollision.SetActiveCollision(isEnabled);
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