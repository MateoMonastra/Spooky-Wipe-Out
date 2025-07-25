using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DecisionTree.Editor;
using AI.DecisionTree.Editor.Helpers;
using AI.DecisionTree.Helpers;
using UnityEditor;
using UnityEngine;

namespace AI.DecisionTree.Editor
{
	public class TreeEditor : EditorWindow
	{
		private const int LeftPanelWidth = 300;
		private const int LabelMAXWidth = 80;
		private const int InputMAXWidth = 130;
		private const int BezierForce = 50;
		private const int LineWidth = 3;
		private Vector2 _polygonSize = new Vector2(65, 80);
		private const string DeletionMessage = "This action will delete all unsaved progress!";
		private const string DefaultDirectory = "Assets/IA/DecisionTree/Saves";

		private int _selectedNode;
		private NodeKind _selectedKind;
		private List<Type> _allNodes;
		private List<Type> _nodeLobby;
		private List<BaseNode> _activeNodes = new List<BaseNode>();
		private string _path = DefaultDirectory;
		private string _fileName = "";
		private int _idToDelete;
		private PopUp _popUp;
		private TextAsset _file;

		[MenuItem("Tools/Tree Editor")]
		public static void OpenWindow()
		{
			var w = GetWindow<TreeEditor>();
			w._popUp = ScriptableObject.CreateInstance<PopUp>();
			w._allNodes = TreeHelper.FindNodeTypesInAllAssemblies().ToList();
			if (w._allNodes != null)
				w._nodeLobby = w._allNodes.GetRange(0, w._allNodes.Count);
			w.Show();
		}

		private void OnGUI()
		{
			minSize = new Vector2(LeftPanelWidth + 200, 150);
			EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), new Color(.2f, .2f, .2f));

			#region Node Creation

			//Left Panel
			Rect leftPanelRect = new Rect(0, 0, LeftPanelWidth, position.height);
			EditorGUI.DrawRect(leftPanelRect, new Color(.4f, .4f, .4f));
			//Left Panel Controls
			EditorGUILayout.BeginVertical(GUILayout.Width(LeftPanelWidth - 6));
			EditorGUI.BeginDisabledGroup(_popUp.IsActive);
			EditorGUI.BeginDisabledGroup(_nodeLobby.Count <= 0);
			EditorGUILayout.LabelField("Create Node");
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Type", GUILayout.MaxWidth(LabelMAXWidth));
			_selectedNode = EditorGUILayout.Popup(_selectedNode, _nodeLobby.Select(n => n.Name).ToArray(),
												GUILayout.MaxWidth(InputMAXWidth));
			_selectedNode = Mathf.Clamp(_selectedNode, 0, _nodeLobby.Count - 1);
			if (GUILayout.Button("Add"))
			{
				_selectedKind = _nodeLobby[_selectedNode].IsSubclassOf(typeof(TreeQuestion))
									? NodeKind.Question
									: NodeKind.Action;
				AddNode(_nodeLobby[_selectedNode], _selectedKind);
			}

			EditorGUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();

			_file = (TextAsset)EditorGUILayout.ObjectField("File: ",
															_file,
															typeof(TextAsset),
															false);
			var filePath = AssetDatabase.GetAssetPath(_file);
			if (_file)
			{
				if (!IsJson(filePath))
				{
					_file = null;
					return;
				}

				var pathSplit = filePath.Split('/');
				_path = pathSplit.Take(pathSplit.Length - 1).Aggregate((current, s) => current + "/" + s);
				_fileName = pathSplit.Last();
			}

			EditorGUI.BeginDisabledGroup(_file != null);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Path", GUILayout.MaxWidth(LabelMAXWidth));
			_path = EditorGUILayout.TextField(_path);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("File Name", GUILayout.MaxWidth(LabelMAXWidth));
			_fileName = EditorGUILayout.TextField(_fileName);
			EditorGUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();

			#endregion

			#region Controls

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Save Tree"))
			{
				FileSystemHelper.ValidatePath(ref _path, DefaultDirectory);
				FileSystemHelper.ForcePath(_path);
				FileSystemHelper.CheckFileExtension(ref _fileName, "json");
				if (File.Exists(_path + "/" + _fileName))
				{
					_popUp = PopUp.OpenPopUp(position, new GUIContent("Override Tree"), "File Already Exists", SaveTree,
											"Override");
				}
				else SaveTree();
			}

			if (GUILayout.Button("Load Tree"))
			{
				_popUp = PopUp.OpenPopUp(position, new GUIContent("Load Tree"), DeletionMessage, LoadTree, "Load");
			}

			if (GUILayout.Button("Reset"))
			{
				_popUp = PopUp.OpenPopUp(position, new GUIContent("Reset"), DeletionMessage, ResetNodes, "Reset");
			}

			EditorGUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndVertical();

			#endregion

			#region Draw Nodes and Connections

			//Background
			// Rect nodeBGRect = new Rect(leftPanelWidth, 0, position.width - leftPanelWidth, position.height);
			// DrawTiles((Texture2D)Resources.Load("Polygon Texture"), polygonSize, new Vector2(leftPanelWidth, 0));
			// EditorGUI.DrawTextureTransparent(nodeBGRect, (Texture2D)Resources.Load("Polygon Texture"));
			DrawLines();
			BeginWindows();
			for (int i = 0; i < _activeNodes.Count; i++)
			{
				_activeNodes[i].Rect = GUI.Window(i, _activeNodes[i].Rect, DrawNode, _activeNodes[i].type.Name);
			}

			EndWindows();

			#endregion

			if (_popUp.IsActive)
				EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), new Color(0, 0, 0, 0.4f));
		}

		private static bool IsJson(string filePath)
		{
			var fileExtension = filePath.Split('.').Last();
			return fileExtension == "json";
		}

		/// <summary>
		/// Resets Nodes
		/// </summary>
		private void ResetNodes()
		{
			_activeNodes = new List<BaseNode>();
			_nodeLobby = _allNodes.GetRange(0, _allNodes.Count);
			Repaint();
		}

		private void DeleteNode()
		{
			_nodeLobby.Add(_activeNodes[_idToDelete].type);
			_activeNodes.RemoveAt(_idToDelete);
			Repaint();
		}

		/// <summary>
		/// Saves the tree into a json
		/// </summary>
		private void SaveTree()
		{
			List<BaseNode> actions = _activeNodes.Where(n => n.Kind == NodeKind.Action).ToList();
			List<BaseNode> questions = _activeNodes.Where(n => n.Kind == NodeKind.Question).ToList();
			List<NodeData> nodeStructs = actions.Select(a => a.GetNodeStruct()).ToList();
			questions = OrderQuestions(questions);
			nodeStructs.AddRange(questions.Select(a => a.GetNodeStruct()).ToList());
			Debug.LogWarning("File saved into: " + _path);
			JsonHelper.Save<NodeData>(_path, nodeStructs, _fileName);
		}

		/// <summary>
		/// Orders a list of questions to be saved in a way that the loader won't bug
		/// </summary>
		/// <param name="questions"></param>
		/// <returns></returns>
		private static List<BaseNode> OrderQuestions(List<BaseNode> questions)
		{
			//Algoritmo simple de ordenamiento
			for (int k = 0; k < questions.Count; k++)
			{
				for (int j = 0; j < questions.Count; j++)
				{
					//Tengo que revisar todos los outcomes
					for (int i = 0; i < questions[j].Outcomes.Length; i++)
					{
						//Si no tiene outcome
						if (questions[j].Outcomes[i] == null)
							continue;
						//Si el outcome es una accion, va a devolver NULL. Por lo tanto, continuo
						BaseNode outcome = questions.Find(q => q.type.Name == questions[j].Outcomes[i]);
						if (outcome == null)
							continue;
						//checkeo si la question está antes que el outcome en la lista. Si es así, las cambio de lugar
						int outcomeIndex = questions.IndexOf(outcome);
						if (outcomeIndex > j)
						{
							(questions[outcomeIndex], questions[j]) = (questions[j], questions[outcomeIndex]);
						}
					}
				}
			}

			return questions;
		}

		/// <summary>
		/// Resets window and loads specified tree
		/// </summary>
		private void LoadTree()
		{
			ResetNodes();
			FileSystemHelper.ValidatePath(ref _path, DefaultDirectory);
			FileSystemHelper.CheckFileExtension(ref _fileName, "json");
			if (!File.Exists(_path + "/" + _fileName))
			{
				Debug.LogWarning("json Tree File not found");
				return;
			}

			List<NodeData> nodeStructs = JsonHelper.Load<NodeData>(_path, _fileName);
			foreach (NodeData n in nodeStructs)
			{
				Type type = _allNodes.Find(t => t.Name == n.ClassType);
				try
				{
					NodeKind kind = type.IsSubclassOf(typeof(TreeQuestion)) ? NodeKind.Question : NodeKind.Action;
					AddNode(type, kind);
				}
				catch (NullReferenceException exception)
				{
					Debug.LogError($"TreeLoader: Type not found ({n.ClassType}) in file {_path + _fileName}");
					Debug.LogException(exception);
					return;
				}

				BaseNode actualNode = FindNode(_activeNodes, type);
				actualNode.Outcomes = n.OutcomeClassNames;
				actualNode.Rect.position = n.PositionInEditor;
			}

			PrepareNodeOutcomes(ref _activeNodes, _allNodes);
		}

		private void DrawTiles(Texture2D texture, Vector2 tileSize, Vector2 offset = new Vector2())
		{
			int numberOfTilesWidth = (int)((position.width - offset.x) / tileSize.x) + 1;
			int numberOfTilesHeight = (int)((position.height - offset.y) / tileSize.y) + 1;
			for (int i = 0; i < numberOfTilesHeight; i++)
			{
				for (int j = 0; j < numberOfTilesWidth; j++)
				{
					GUI.DrawTexture(new Rect(new Vector2(tileSize.x * j + offset.x, tileSize.y * i + offset.y), tileSize),
									texture);
				}
			}
		}

		private static void PrepareNodeOutcomes(ref List<BaseNode> nodes, IList<Type> outcomeCandidates)
		{
			var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes());
			var enumerable = types as Type[] ?? types.ToArray();
			foreach (var node in nodes)
			{
				if (node.Kind != NodeKind.Question)
					continue;
				List<Type> candidates = outcomeCandidates.Where(t => t != node.type).ToList();
				for (var i = 0; i < node.Outcomes.Length; i++)
				{
					var outcome = enumerable.FirstOrDefault(t => t.Name == node.Outcomes[i]);
					if (outcome == null)
						continue;

					node.OutcomeSelection[i] = candidates.IndexOf(outcome);
				}
			}
		}

		/// <summary>
		/// Draws a node on the window
		/// </summary>
		/// <param name="id">id</param>
		private void DrawNode(int id)
		{
			#region Delete Button

			GUIStyle xStyle = new GUIStyle();
			//TODO:This doesn't align it
			xStyle.alignment = TextAnchor.UpperRight;
			xStyle.fixedWidth = 10;
			if (GUILayout.Button("✖", xStyle))
			{
				_popUp = PopUp.OpenPopUp(position,
										new GUIContent("Delete Node"),
										"Confirm node Deletion",
										DeleteNode,
										"Delete");
				_idToDelete = id;
			}

			#endregion

			#region Outcomes

			if (_activeNodes[id].Kind == NodeKind.Question)
			{
				EditorGUILayout.BeginVertical();
				GUILayout.Label("Outcomes");
				for (int i = 0; i < _activeNodes[id].Outcomes.Length; i++)
				{
					List<Type> candidates = _allNodes.Where(t => t != _activeNodes[id].type).ToList();
					EditorGUILayout.BeginHorizontal();
					int outcomeSelection = _activeNodes[id].OutcomeSelection[i];
					outcomeSelection
						= EditorGUILayout.Popup(outcomeSelection, candidates.Select(n => n.Name).ToArray());

					#region Connection

					if (GUILayout.Button("Connect"))
					{
						_activeNodes[id].Outcomes[i] = candidates[outcomeSelection].Name;
						BaseNode connection = FindNode(_activeNodes, candidates[outcomeSelection]);
						if (connection == null)
						{
							NodeKind kind = candidates[outcomeSelection].IsSubclassOf(typeof(TreeQuestion))
												? NodeKind.Question
												: NodeKind.Action;
							Debug.LogWarning("Node Created: " + candidates[outcomeSelection]);
							AddNode(candidates[outcomeSelection], kind);
							connection = FindNode(_activeNodes, candidates[outcomeSelection]);
						}
					}

					#endregion

					_activeNodes[id].OutcomeSelection[i] = outcomeSelection;
					EditorGUILayout.EndHorizontal();
				}

				EditorGUILayout.EndVertical();
			}

			#endregion

			#region Drag

			GUI.DragWindow();

			if (_activeNodes[id].Rect.x < LeftPanelWidth)
				_activeNodes[id].Rect.x = LeftPanelWidth;
			else if (_activeNodes[id].Rect.x + _activeNodes[id].Rect.width > position.width)
				_activeNodes[id].Rect.x = position.width - _activeNodes[id].Rect.width;
			if (_activeNodes[id].Rect.y < 0)
				_activeNodes[id].Rect.y = 0;
			else if (_activeNodes[id].Rect.y + _activeNodes[id].Rect.height > position.height)
				_activeNodes[id].Rect.y = position.height - _activeNodes[id].Rect.height;

			#endregion
		}

		//TODO:Make static
		/// <summary>
		/// Draws the lines to connect each node with it's outcomes
		/// </summary>
		private void DrawLines()
		{
			foreach (BaseNode q in _activeNodes.Where(n => n.Kind == NodeKind.Question).ToList())
			{
				for (int i = 0; i < q.Outcomes.Length; i++)
				{
					if (q.Outcomes[i] == null) continue;
					Type outcomeType = _allNodes.Find(t => t.Name == q.Outcomes[i]);
					BaseNode connection = FindNode(_activeNodes, outcomeType);
					if (connection == null)
					{
						q.Outcomes[i] = null;
						continue;
					}

					Vector2 origin = new Vector2(q.Rect.x + _activeNodes[i].Rect.width, q.Rect.y + q.Rect.height / 2);
					Vector2 objective = new Vector2(connection.Rect.x, connection.Rect.y + connection.Rect.height / 2);
					Handles.DrawBezier(origin, objective, origin + Vector2.right * BezierForce,
										objective + Vector2.left * BezierForce, connection.Color,
										Texture2D.whiteTexture, LineWidth);
				}
			}
		}
		//
		// /// <summary>
		// /// Gets node types from all assemblies
		// /// </summary>
		// /// <returns></returns>
		// private static IEnumerable<Type> FindNodeTypesInAllAssemblies()
		// {
		// 	var assemblies = AppDomain.CurrentDomain.GetAssemblies();
		// 	IEnumerable<Type> allTypes = assemblies.SelectMany(a => a.GetTypes());
		// 	Type skippedAttribute = typeof(SkippedInEditorAttribute);
		// 	var treeNodeTypes
		// 		= allTypes.Where(t => t.IsSubclassOf(typeof(TreeNode))
		// 									&& t.GetCustomAttributes(skippedAttribute, false).Length == 0);
		// 	return treeNodeTypes;
		// }

		/// <summary>
		/// Finds in the active nodes for a specific type
		/// </summary>
		/// <param name="nodes">list of nodes</param>
		/// <param name="t">type to search for</param>
		/// <returns></returns>
		private static BaseNode FindNode(IEnumerable<BaseNode> nodes, Type t)
			=> nodes.FirstOrDefault(n => n.type == t);

		/// <summary>
		/// Adds a node to the node list
		/// </summary>
		/// <param name="t"></param>
		/// <param name="kind"></param>
		private void AddNode(Type t, NodeKind kind)
		{
			BaseNode node = new BaseNode(new Rect(300, 0, 200, 150), kind, t);
			if (kind == NodeKind.Question)
			{
				TreeQuestion temp = (TreeQuestion)Activator.CreateInstance(t);
				node.Outcomes = new string[temp.GetOutcomesQty()];
				node.OutcomeSelection = new int[temp.GetOutcomesQty()];
			}

			float r = UnityEngine.Random.Range(.3f, 1f);
			float g = UnityEngine.Random.Range(.3f, 1f);
			float b = UnityEngine.Random.Range(.3f, 1f);
			node.Color = new Color(r, g, b);
			_activeNodes.Add(node);
			_nodeLobby.Remove(t);
			Repaint();
		}

		private void OnDestroy()
		{
			if (_popUp.IsActive)
				_popUp.Close();
		}
	}
}