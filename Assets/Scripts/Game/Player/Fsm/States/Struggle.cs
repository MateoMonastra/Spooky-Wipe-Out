using UnityEngine;
using FSM;
using Game.Minigames;
using Game.Minigames.CatchZone;
using Minigames;
using VacuumCleaner;

namespace Game.Player
{
    public class Struggle : PlayerState
    {
        private readonly CleanerController _cleanerController;
        private readonly CatchZoneController _catchZoneController;
        private readonly SkillCheckController _skillCheckController;
        private readonly System.Action _onStruggleEnd;

        private Rigidbody _rigidbody;
        private Minigame _activeMinigame;

        public Struggle(
            GameObject player,
            CleanerController cleanerController,
            CatchZoneController catchZoneController,
            SkillCheckController skillCheckController,
            System.Action onStruggleEnd)
            : base(player)
        {
            _cleanerController = cleanerController;
            _catchZoneController = catchZoneController;
            _skillCheckController = skillCheckController;
            _onStruggleEnd = onStruggleEnd;
        }

        public override void Enter()
        {
            _rigidbody = player.GetComponent<Rigidbody>();
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;

            _cleanerController.SwitchToTool(1);
            
            _skillCheckController.OnLose += EndStruggle;
            _skillCheckController.OnStop += EndStruggle;
            
            _catchZoneController.OnLose += EndStruggle;
            _catchZoneController.OnStop += EndStruggle;
        }

        public override void Exit()
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            
            _skillCheckController.OnLose -= EndStruggle;
            _skillCheckController.OnStop -= EndStruggle;
            
            _catchZoneController.OnLose -= EndStruggle;
            _catchZoneController.OnStop -= EndStruggle;
        }

        private void EndStruggle()
        {
            _onStruggleEnd?.Invoke();
        }

        private Minigame FindActiveMinigame()
        {
            if (_catchZoneController != null && _catchZoneController.GetActive())
                return _catchZoneController;

            if (_skillCheckController != null && _skillCheckController.GetActive())
                return _skillCheckController;

            return null;
        }
    }
}