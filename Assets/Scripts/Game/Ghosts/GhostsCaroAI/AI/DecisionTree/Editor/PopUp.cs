using UnityEditor;
using UnityEngine;

namespace DecisionTree.Editor
{
	public delegate void PopUpEvent();
	public class PopUp : EditorWindow
	{
		private const int Height = 70;
		private const int Width = 300;

		private string _message;
		private string _confirmButtonText;
		private string _cancelButtonText;

		private PopUpEvent _confirm;
		private PopUpEvent _cancel;

		public bool IsActive { get; private set; }

		private void OnGUI()
		{
			Focus();
			EditorGUILayout.LabelField(_message);
			if (GUILayout.Button(_confirmButtonText))
			{
				_confirm?.Invoke();
				this.IsActive = false;
				this.Close();
			}
			if (GUILayout.Button(_cancelButtonText))
			{
				_cancel?.Invoke();
				this.IsActive = false;
				this.Close();
			}
		}

		public static PopUp OpenPopUp(Rect mainWindowRect,
									GUIContent title,
									string message,
									PopUpEvent confirm,
									string confirmText = "Accept",
									PopUpEvent cancel = null,
									string cancelText = "Cancel")
		{
			var p = ScriptableObject.CreateInstance<PopUp>();
			float x = mainWindowRect.x + mainWindowRect.width / 2 - Width / 2;
			float y = mainWindowRect.y + mainWindowRect.height / 2 - Height / 2;
			Rect rect = new Rect(x, y, Width, Height);
			p.position = rect;
			if (cancel != null) p._cancel = cancel;
			p._confirm = confirm;
			p.titleContent = title;
			p._message = message;
			p._confirmButtonText = confirmText;
			p._cancelButtonText = cancelText;

			p.IsActive = true;
			p.ShowPopup();
			return p;
		}
	}
}