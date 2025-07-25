using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI.DecisionTree.Helpers
{
	public static class TreeHelper
	{
		/// <summary>
		/// Loads a Tree from a json file
		/// </summary>
		/// <param name="path"></param>
		/// <param name="getData"></param>
		/// <returns></returns>
		public static Tree LoadTree(string path, Func<object> getData)
		{
			List<NodeData> nodes = JsonHelper.Load<NodeData>(path);
			var nodesForTree = new Dictionary<Type, TreeNode>();
			var nodeTypes = FindNodeTypesInAllAssemblies().ToArray();
			#region Generate Tree
			foreach (var n in nodes)
			{
				Type nodeType = nodeTypes.FirstOrDefault(t => t.Name == n.ClassType);
				if (nodeType == null)
					throw new TypeLoadException($"Class not found for node {n.ClassType}");
				switch (n.Kind)
				{
					case NodeKind.Action:
					{
						nodesForTree.Add(nodeType, (TreeAction)Activator.CreateInstance(nodeType));
						break;
					}
					case NodeKind.Question:
					{
						List<TreeNode> outcomes = new List<TreeNode>();
						foreach (var outcome in n.OutcomeClassNames)
						{
							outcomes.Add(GetNode(nodesForTree, Type.GetType(outcome)));
						}
						TreeQuestion question = (TreeQuestion)Activator.CreateInstance(nodeType);
						question.Outcomes = outcomes.ToArray();
						nodesForTree.Add(nodeType, question);
						break;
					}
				}
			}
			TreeStart start = new TreeStart(GetNode(nodesForTree, Type.GetType(nodes[nodes.Count - 1].ClassType)));
			Tree tree = new Tree(start, getData);
			#endregion
			return tree;
		}

		/// <summary>
		/// Loads a Tree from a json file
		/// </summary>
		/// <param name="treeFile"></param>
		/// <param name="getData"></param>
		/// <returns></returns>
		public static Tree LoadTree(TextAsset treeFile, Func<object> getData)
		{
			IEnumerable<NodeData> nodes = JsonHelper.Load<NodeData>(treeFile);
			var nodesForTree = new Dictionary<Type, TreeNode>();
			List<Type> nodeTypes = FindNodeTypesInAllAssemblies().ToList();
			#region Generate Tree
			foreach (var n in nodes)
			{
				// Type nodeType = Type.GetType(n.ClassType);
				Type nodeType = nodeTypes.FirstOrDefault(t => t.Name == n.ClassType);
				if (nodeType == null)
					throw new TypeLoadException($"Class not found for node {n.ClassType}");
				switch (n.Kind)
				{
					case NodeKind.Action:
					{
						nodesForTree.Add(nodeType, (TreeAction)Activator.CreateInstance(nodeType));
						break;
					}
					case NodeKind.Question:
					{
						List<TreeNode> outcomes = new List<TreeNode>();
						foreach (var outcome in n.OutcomeClassNames)
						{
							outcomes.Add(GetNode(nodesForTree, nodeTypes.Find(t => t.Name == outcome)));
						}
						TreeQuestion question = (TreeQuestion)Activator.CreateInstance(nodeType);
						question.Outcomes = outcomes.ToArray();
						nodesForTree.Add(nodeType, question);
						break;
					}
				}
			}
			TreeStart start = new TreeStart(GetNode(nodesForTree,nodeTypes.Find(t => t.Name == nodes.Last().ClassType)));
			Tree tree = new Tree(start, getData);
			#endregion
			return tree;
		}

		/// <summary>
		/// Serializes a Tree to a json file
		/// </summary>
		/// <param name="actions">list of actions in FSM</param>
		/// <param name="questions">list of questions in FSM</param>
		/// <param name="path">json file path</param>
		public static void SaveTree(List<TreeAction> actions, List<TreeQuestion> questions, string path)
		{
			List<NodeData> nodeStructs = new List<NodeData>();
			#region Serialize Actions
			foreach (var a in actions)
			{
				NodeData newAction = new NodeData
				{
					Kind = NodeKind.Action,
					ClassType = a.GetType().ToString()
				};
				nodeStructs.Add(newAction);
			}
			#endregion
			#region Serialize Questions
			foreach (var q in questions)
			{
				NodeData newQuestion = new NodeData
				{
					Kind = NodeKind.Question,
					ClassType = q.GetType().ToString()
				};
				List<string> outcomesStr = new List<string>();
				foreach (var outcome in q.Outcomes)
				{
					if (outcome.GetType() == typeof(TreeQuestion))
					{
						TreeQuestion temp = (TreeQuestion)outcome;
						outcomesStr.Add(temp.GetType().ToString());
					}
					else outcomesStr.Add(outcome.ToString());
				}
				newQuestion.OutcomeClassNames = outcomesStr.ToArray();
				nodeStructs.Add(newQuestion);
			}
			#endregion
			JsonHelper.Save<NodeData>(path, nodeStructs);
		}

		/// <summary>
		/// Searches for a specific node inside a given fsm
		/// </summary>
		/// <param name="treeNodes"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		static TreeNode GetNode(Dictionary<Type, TreeNode> treeNodes, Type type)
		{
			if (treeNodes.ContainsKey(type))
				return treeNodes[type];
			TreeNode result = (TreeNode)Activator.CreateInstance(type);
			treeNodes.Add(type, result);
			return result;
		}
		
		/// <summary>
		/// Gets node types from all assemblies
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<Type> FindNodeTypesInAllAssemblies()
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			IEnumerable<Type> allTypes = assemblies.SelectMany(a => a.GetTypes());
			Type skippedAttribute = typeof(SkippedInEditorAttribute);
			var treeNodeTypes
				= allTypes.Where(t => t.IsSubclassOf(typeof(TreeNode))
									&& t.GetCustomAttributes(skippedAttribute, false).Length == 0);
			return treeNodeTypes;
		}
	}
}