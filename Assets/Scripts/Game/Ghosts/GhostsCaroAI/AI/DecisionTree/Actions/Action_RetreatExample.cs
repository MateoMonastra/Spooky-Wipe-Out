using AI.DecisionTree;
using UnityEngine;

namespace DecisionTree.Actions
{
	public class Action_RetreatExample : TreeAction
	{
		public override void NodeFunction()
		{
			Debug.Log("Retreat!");
		}
	}
}