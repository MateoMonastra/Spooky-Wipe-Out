using UnityEngine;
using FSM;
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

            _activeMinigame = FindActiveMinigame();

            if (_activeMinigame != null)
            {
                _activeMinigame.OnWin += EndStruggle;
                _activeMinigame.OnLose += EndStruggle;
                _activeMinigame.OnStop += EndStruggle;

                Debug.Log($"[Struggle] Subscribed to minigame: {_activeMinigame.name}");
            }
            else
            {
                Debug.LogWarning("[Struggle] No active minigame found.");
            }
        }

        public override void Exit()
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            if (_activeMinigame != null)
            {
                _activeMinigame.OnWin -= EndStruggle;
                _activeMinigame.OnLose -= EndStruggle;
                _activeMinigame.OnStop -= EndStruggle;
                _activeMinigame = null;
            }
        }

        private void EndStruggle()
        {
            Debug.Log("[Struggle] Minigame ended. Cleaning up...");
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
