using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EventSystems.EventSceneManager;
using Game.Garbage;
using Game.Minigames.CatchZone;
using Gameplay.Timer;
using Ghosts;
using Minigames;
using Player.FSM;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public Action OnFinish;

        public Timer timer;

        public List<Ghost> ghosts;
        public List<Trash> garbage;
        public List<Ectoplasm> ectoplasms;

        [SerializeField] private SkillCheckController skMinigame;
        [SerializeField] private ADController adMinigame;
        [SerializeField] private CatchZoneController catchZoneMinigame;


        [SerializeField] private ObjectivesUI objectivesUI;
        [SerializeField] private GameObject playerUI;
        [SerializeField] private InputReader inputReader;

        [SerializeField] private string nextScene;
        [SerializeField] private EventChannelSceneManager eventChannelSceneManager;

        private static GameManager _instance;

        IEnumerator Start()
        {
            yield return null;

            foreach (Ghost ghost in ghosts)
            {
                ghost.OnBeingDestroy += RemoveGhost;
            }

            foreach (Ectoplasm ectoplasm in ectoplasms)
            {
                ectoplasm.OnBeingDestroy += RemoveEctoplasm;
            }

            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                // Si ya existe una instancia y no es esta, se destruye el objeto duplicado
                Destroy(gameObject);
            }

            objectivesUI.SetTrashQnty(garbage.Count);
            objectivesUI.SetGhostQnty(ghosts.Count);
            objectivesUI.SetEctoplasmQnty(ectoplasms.Count);

            inputReader.OnWinCheat += FinishGame;

            Time.timeScale = 1f;
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        private void RemoveTrash(Trash trash)
        {
            trash.OnBeingDestroy -= RemoveTrash;
            garbage.Remove(trash);
            objectivesUI.SetTrashQnty(garbage.Count);

            GameIsOver();
        }

        private void RemoveGhost(Ghost ghost)
        {
            ghost.OnBeingDestroy -= RemoveGhost;
            ghosts.Remove(ghost);
            objectivesUI.SetGhostQnty(ghosts.Count);

            GameIsOver();
        }

        private void RemoveEctoplasm(Ectoplasm ectoplasm)
        {
            ectoplasm.OnBeingDestroy -= RemoveEctoplasm;
            ectoplasms.Remove(ectoplasm);
            objectivesUI.SetEctoplasmQnty(ectoplasms.Count);

            GameIsOver();
        }

        public static GameManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }

            return _instance;
        }

        public bool IsAnyGhost()
        {
            return ghosts.Any(ghost => ghost.isActiveAndEnabled);
        }

        public bool IsAnyGarbage()
        {
            return garbage.Any(trash => trash.isActiveAndEnabled);
        }

        public bool IsAnyEctoplasm()
        {
            return ectoplasms.Any(ectoplasm => ectoplasm.isActiveAndEnabled);
        }

        public void SetPlayerUIState(bool state)
        {
            playerUI.SetActive(state);
        }

        private void GameIsOver()
        {
            if (!IsAnyGhost() && !IsAnyGarbage() && !IsAnyEctoplasm())
            {
                FinishGame();
            }
        }

        private void FinishGame()
        {
            SetPlayerUIState(false);
            OnFinish?.Invoke();
        }

        public ADController GetMinigameADController()
        {
            return adMinigame;
        }
    
        public SkillCheckController GetMinigameSkillCheckController()
        {
            return skMinigame;
        }
    
        public CatchZoneController GetMinigameCatchZoneController()
        {
            return catchZoneMinigame;
        }

        public bool IsAnyMinigameActive()
        {
            if (catchZoneMinigame != null && catchZoneMinigame.GetActive())
                return true;

            if (skMinigame != null && skMinigame.GetActive())
                return true;
        
            if(adMinigame != null && adMinigame.GetActive())
                return true;

            return false;
        }

        public void AddTrash(Trash trash)
        {
            trash.OnBeingDestroy += RemoveTrash;
            garbage.Add(trash);
            objectivesUI.SetTrashQnty(garbage.Count);
        }

        public void SetMinigamesBloquedStatus(bool status)
        {
            skMinigame.SetBloqued(status);
            adMinigame.SetBloqued(status);
            catchZoneMinigame.SetBloqued(status);
        }
    }
}