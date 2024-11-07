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
        public UnityEvent OnCatch;
        public UnityEvent<bool> OnDeath;
        
        [SerializeField] private Minigame minigame;
        [SerializeField] private Transform trappingPos;
        
        private List<State> _states = new List<State>();

        private Fsm _fsm;

        private Transition _huntToCatch;
        private Transition _catchToDead;

        private WallGhostCollision _wallGhostCollision;

        public void Start()
        {
            _wallGhostCollision = GetComponentInChildren<WallGhostCollision>();

            _wallGhostCollision.OnPlayerCollision += minigame.StartGame;
            _wallGhostCollision.OnPlayerCollision += SetCatchState;
            minigame.OnWin += SetDeadState;
            minigame.OnLose += SetDeadState;

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

            _fsm = new Fsm(_hunt);
        }

        private void SetCatchState()
        {
            OnCatch?.Invoke();
            _fsm.ApplyTransition(_huntToCatch);
        }
        
        private void SetDeadState()
        {
            OnDeath?.Invoke(true);
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