using Game.Minigames.CatchZone;
using Minigames;
using UnityEngine;

namespace Audio
{
    public class MinigamesAudio : MonoBehaviour
    {
        [SerializeField] private SkillCheckController skillCheckController;
        [SerializeField] private ADController adController;
        [SerializeField] private CatchZoneController catchZoneController;

        private void OnEnable()
        {
            skillCheckController.OnStart += OnGhostCatch;
            skillCheckController.OnLose += OnGhostRelease;
            skillCheckController.OnWin += OnGhostRelease;
            skillCheckController.OnStop += OnGhostRelease;
            skillCheckController.OnCheckPass += OnGhostSkillCheckPass;
            skillCheckController.OnCheckFail += OnGhostSkillCheckFail; 
            
            catchZoneController.OnStart += OnGhostCatch;
            catchZoneController.OnLose += OnGhostRelease;
            catchZoneController.OnWin += OnGhostRelease;
            catchZoneController.OnStop += OnGhostRelease;

            adController.OnStart += OnPaintCatch;
            adController.OnLose += OnPaintRelease;
            adController.OnWin += OnPaintRelease;
            adController.OnStop += OnPaintRelease;
        }

        private void OnDisable()
        {
            skillCheckController.OnStart -= OnGhostCatch;
            skillCheckController.OnLose -= OnGhostRelease;
            skillCheckController.OnWin -= OnGhostRelease;
            skillCheckController.OnStop -= OnGhostRelease;
            skillCheckController.OnCheckPass -= OnGhostSkillCheckPass;
            skillCheckController.OnCheckFail -= OnGhostSkillCheckFail;
            
            catchZoneController.OnStart -= OnGhostCatch;
            catchZoneController.OnLose -= OnGhostRelease;
            catchZoneController.OnWin -= OnGhostRelease;
            catchZoneController.OnStop -= OnGhostRelease;

            adController.OnStart -= OnPaintCatch;
            adController.OnLose -= OnPaintRelease;
            adController.OnWin -= OnPaintRelease;
            adController.OnStop -= OnPaintRelease;
        }

        private void OnGhostCatch()
        {
            AkSoundEngine.PostEvent("GhostCatch", gameObject);
        }

        private void OnGhostRelease()
        {
            AkSoundEngine.PostEvent("GhostRelease", gameObject);
        }

        private void OnPaintCatch()
        {
            AkSoundEngine.PostEvent("PaintCatch", gameObject);
        }

        private void OnPaintRelease()
        {
            AkSoundEngine.PostEvent("PaintRelease", gameObject);
        }

        private void OnGhostSkillCheckPass()
        {
            AkSoundEngine.PostEvent("GhostSkillCheckPass", gameObject);
        }

        private void OnGhostSkillCheckFail()
        {
            AkSoundEngine.PostEvent("GhostSkillCheckFail", gameObject);
        }
    }
}