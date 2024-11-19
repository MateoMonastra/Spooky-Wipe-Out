using System;
using System.Collections.Generic;
using AK.Wwise;
using Fsm_Mk2;
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

        public Action OnRested;

        [SerializeField] private Minigame minigame;
        [SerializeField] private Transform trappingPos;
        [SerializeField] private float restTime;
        
        private List<State> _states = new List<State>();

        private Fsm _fsm;

        private Transition _huntToCatch;
        private Transition _catchToDead;
        private Transition _deadToHunt;

        private WallGhostCollision _wallGhostCollision;

        public void Start()
        {
            _wallGhostCollision = GetComponentInChildren<WallGhostCollision>();

            _wallGhostCollision.OnPlayerCollision += minigame.StartGame;
            _wallGhostCollision.OnPlayerCollision += SetCatchState;

            OnRested += SetHuntState;
            State _hunt = new Hunt();
            _states.Add(_hunt);

            State _catch = new Catch();
            _states.Add(_catch);

            State _dead = new Dead(restTime, OnRested);
            _states.Add(_dead);

            _huntToCatch = new Transition() { From = _hunt, To = _catch };
            _hunt.transitions.Add(_huntToCatch);

            _catchToDead = new Transition() { From = _catch, To = _dead };
            _catch.transitions.Add(_catchToDead);

            _deadToHunt = new Transition() { From = _dead, To = _hunt };
            _dead.transitions.Add(_catchToDead);

            _fsm = new Fsm(_hunt);
        }

        private void SetHuntState() 
        {
            OnHunt?.Invoke(true);
            _fsm.ApplyTransition(_deadToHunt);
            _wallGhostCollision.gameObject.SetActive(true);
        }

        private void SetCatchState()
        {
            OnCatch?.Invoke(true);
            minigame.OnWin += SetDeadState;
            minigame.OnLose += SetDeadState;
            _fsm.ApplyTransition(_huntToCatch);
        }
        
        private void SetDeadState()
        {
            OnDeath?.Invoke(true);
            _wallGhostCollision.gameObject.SetActive(false);
            minigame.OnWin -= SetDeadState;
            minigame.OnLose -= SetDeadState;
            _fsm.ApplyTransition(_catchToDead);
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