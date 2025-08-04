using UnityEngine;
using FSM;
using Minigames;
using VacuumCleaner;

namespace Game.Player
{
    public class Struggle : PlayerState
    {
        private readonly CleanerController _cleanerController;
        private readonly Minigame _minigame;
        private readonly System.Action _onStruggleEnd;
        private Rigidbody _rigidbody;

        public Struggle(GameObject player, CleanerController cleanerController, Minigame minigame, System.Action onStruggleEnd)
            : base(player)
        {
            _cleanerController = cleanerController;
            _minigame = minigame;
            _onStruggleEnd = onStruggleEnd;
        }

        public override void Enter()
        {
            _rigidbody = player.GetComponent<Rigidbody>();
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;

            _cleanerController.SwitchToTool(1);

            _minigame.OnWin += EndStruggle;
            _minigame.OnLose += EndStruggle;
            _minigame.OnStop += EndStruggle;
        }

        public override void Exit()
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            _minigame.OnWin -= EndStruggle;
            _minigame.OnLose -= EndStruggle;
            _minigame.OnStop -= EndStruggle;
        }

        private void EndStruggle()
        {
            _onStruggleEnd?.Invoke();
        }
    }
}