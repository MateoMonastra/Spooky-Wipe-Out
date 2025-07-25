using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Rest : AI.DecisionTree.TreeAction
{
    public override void NodeFunction()
    {
        callback.Invoke(GetType());
    }
}
