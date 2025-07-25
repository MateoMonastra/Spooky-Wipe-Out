using AI.DecisionTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Patrolling : AI.DecisionTree.TreeAction
{
    public override void NodeFunction()
    {
        callback.Invoke(GetType());
    }
}
