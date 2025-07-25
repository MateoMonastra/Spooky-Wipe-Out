using System;
using AI.DecisionTree.Helpers;
using UnityEngine;

namespace DecisionTree.Editor
{
	public class BaseNode
	{
		public Rect Rect;
		public readonly Type type;
		public string[] Outcomes;
		public int[] OutcomeSelection;
		public readonly NodeKind Kind;
		public Color Color;

		public BaseNode(Rect rect, NodeKind kind, Type type)
		{
			Rect = rect;
			Kind = kind;
			this.type = type;
		}

		public NodeData GetNodeStruct()
		{
			NodeData nodeData;
			nodeData.Kind = Kind;
			nodeData.ClassType = type.Name;
			nodeData.OutcomeClassNames = Outcomes;
			nodeData.PositionInEditor = Rect.position;
			return nodeData;
		}
	}
}