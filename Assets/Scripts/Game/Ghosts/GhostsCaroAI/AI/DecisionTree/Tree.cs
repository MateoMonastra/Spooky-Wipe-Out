using System;

namespace AI.DecisionTree
{
	public delegate void TreeEvent(params object[] args);
	public class Tree
	{
		private readonly TreeStart _start;
		private TreeNode _current;
		public TreeNode[] Nodes;
		public TreeEvent Callback;
		private readonly Func<object> _getData;
		public Tree(TreeStart start, Func<object> getData)
		{
			_start = start;
			_getData = getData;
			_start.FirstNode.getData = getData;
		}

		public void RunTree()
		{
			_current = _start;
			_current.ChangeNode = ChangeNode;
			_current.NodeFunction();
		}

		public void ChangeNode(TreeNode newNode)
		{
			_current.OnExit();
			_current = newNode;
			_current.getData = _getData;
			_current.callback = Callback;
			_current.ChangeNode = ChangeNode;
			_current.NodeFunction();
		}
	}
}