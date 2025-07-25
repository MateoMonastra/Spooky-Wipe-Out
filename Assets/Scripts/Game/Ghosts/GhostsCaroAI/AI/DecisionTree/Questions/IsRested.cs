using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsRested : AI.DecisionTree.TreeQuestion
{
    public override int CheckCondition()
    {
        if (getData() is GhostBehaviourData data)
        {
            var isRested = data.self.isRested;

            return isRested ? 0 : 1;
        }

        return 1;
    }
}