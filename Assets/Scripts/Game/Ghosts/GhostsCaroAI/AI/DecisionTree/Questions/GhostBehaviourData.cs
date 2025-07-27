using Game.Ghosts.WalkingGhost;
using UnityEngine;
using Ghosts;

public struct GhostBehaviourData
{
    public GhostBehaviourData(ChainGhostAgent self, Transform hunter)
    {
        this.self = self;
        this.hunter = hunter;
    }

    public ChainGhostAgent self { get; set; }

    public Transform hunter{ get; set; }
}
