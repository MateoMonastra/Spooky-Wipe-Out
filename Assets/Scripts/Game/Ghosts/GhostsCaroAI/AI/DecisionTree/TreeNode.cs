using System;

namespace AI.DecisionTree
{
	public abstract class TreeNode
	{
		public delegate void ChangeNodeEvent(TreeNode newNode);

		public Func<object> getData;
		public TreeEvent callback;
		public ChangeNodeEvent ChangeNode;

		/// <summary>
		/// Actions and extra validations.
		/// Called once every frame
		/// </summary>
		public abstract void NodeFunction();

		/// <summary>
		/// Called once when the node changes
		/// </summary>
		public void OnExit()
		{
		}
	}
}