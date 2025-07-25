using System.Collections.Generic;
using Ghosts;
using UnityEngine;

namespace Game.Ghosts.WallGhost
{
    public class PaintGhostManager : MonoBehaviour
    {
        [SerializeField] private List<WallGhostAgent> allGhosts;
        [SerializeField] private int maxActiveGhosts = 1;

        private int _lastActivatedIndex = -1;
        private List<int> _activeIndices = new();

        private void Start()
        {
            DeactivateAllGhosts();
            ActivateRandomGhosts(maxActiveGhosts);
        }

        private void DeactivateAllGhosts()
        {
            foreach (var ghost in allGhosts)
            {
                ghost.SetCollisionEnabled(false);
                ghost.OnDeath.AddListener(_ => OnGhostDeath(ghost));
            }
        }

        private void ActivateRandomGhosts(int count)
        {
            int tries = 0;
            while (_activeIndices.Count < count && tries < 100)
            {
                int index = Random.Range(0, allGhosts.Count);

                if (index != _lastActivatedIndex && !_activeIndices.Contains(index))
                {
                    _activeIndices.Add(index);
                    _lastActivatedIndex = index;
                    allGhosts[index].SetCollisionEnabled(true);
                }

                tries++;
            }
        }

        private void OnGhostDeath(WallGhostAgent ghost)
        {
            int index = allGhosts.IndexOf(ghost);
            if (index != -1)
            {
                ghost.SetCollisionEnabled(false);
                _activeIndices.Remove(index);
                ActivateRandomGhosts(1);
            }
        }

    }
}
