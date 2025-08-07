using System.Collections;
using FSM;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace Game.Ghosts.MagicGhost
{
    public class DropTrash : State
    {
        private readonly NavMeshAgent _agent;
        private readonly GameObject[] _trashPrefabs;
        private readonly Transform _spawnPoint;
        private readonly MonoBehaviour _coroutineRunner;
        private readonly float _delayBeforeSpawn = 1.5f;

        private bool _hasDropped = false;
        private readonly System.Action _onDone;

        public DropTrash(NavMeshAgent agent, GameObject[] trashPrefabs, Transform spawnPoint, MonoBehaviour coroutineRunner, System.Action onDone)
        {
            _agent = agent;
            _trashPrefabs = trashPrefabs;
            _spawnPoint = spawnPoint;
            _coroutineRunner = coroutineRunner;
            _onDone = onDone;
        }

        public override void Enter()
        {
            _agent.isStopped = true;
            _hasDropped = false;

            _coroutineRunner.StartCoroutine(DropAfterDelay());
        }

        private IEnumerator DropAfterDelay()
        {
            yield return new WaitForSeconds(_delayBeforeSpawn);

            if (_hasDropped || _trashPrefabs == null || _trashPrefabs.Length == 0)
            {
                _onDone?.Invoke();
                yield break;
            }

            int randomIndex = Random.Range(0, _trashPrefabs.Length);
            GameObject selectedTrash = _trashPrefabs[randomIndex];

            if (selectedTrash != null)
            {
                GameObject instance = Object.Instantiate(selectedTrash, _spawnPoint.position, Quaternion.identity);

                SceneManager.MoveGameObjectToScene(instance, _spawnPoint.gameObject.scene);
            }

            _hasDropped = true;
            _onDone?.Invoke();
        }

        public override void Exit()
        {
            _agent.isStopped = false;
        }
    }
}