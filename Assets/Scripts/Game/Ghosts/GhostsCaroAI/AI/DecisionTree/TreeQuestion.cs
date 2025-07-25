using AI.DecisionTree.Helpers;

namespace AI.DecisionTree
{
	[SkippedInEditor]
	public abstract class TreeQuestion : TreeNode
	{
		public TreeNode[] Outcomes { get; set; }

		public virtual int GetOutcomesQty() => 2;
		public override void NodeFunction()
		{
			int outcomeIndex = CheckCondition();
			if (outcomeIndex >= Outcomes.Length) return;
			ChangeNode?.Invoke(Outcomes[outcomeIndex]);
		}

		/// <summary>
		/// This method should return 0 if the condition is true
		/// </summary>
		/// <returns></returns>
		public abstract int CheckCondition();
	}
}