using AI.DecisionTree;
using UnityEngine;

namespace DecisionTree.Actions
{
	public class Action_AttackExample : TreeAction
	{
		public override void NodeFunction()
		{
			Debug.Log("Attack!");
		}
	}
}