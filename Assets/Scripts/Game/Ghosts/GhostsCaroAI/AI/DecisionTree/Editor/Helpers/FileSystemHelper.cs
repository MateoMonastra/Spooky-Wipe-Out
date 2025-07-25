using UnityEditor;

namespace AI.DecisionTree.Editor.Helpers
{
	public static class FileSystemHelper
	{
		/// <summary>
		/// Validates the path to check if it's null and, if so, resets it to default
		/// </summary>
		/// <param name="path">path to check</param>
		/// <param name="defPath">Default path</param>
		public static void ValidatePath(ref string path, string defPath)
		{
			if (string.IsNullOrEmpty(path))
				path = defPath;
		}

		/// <summary>
		/// Forces the existance of a path by creating every folder that is not in the project
		/// </summary>
		/// <param name="desiredPath"></param>
		public static void ForcePath(string desiredPath)
		{
			if (AssetDatabase.IsValidFolder(desiredPath)) return;
			string[] pathList = desiredPath.Split('/');
			string tempPath = pathList[0];
			for (int i = 1; i < pathList.Length - 1; i++)
			{
				tempPath += "/" + pathList[i];
			}
			ForcePath(tempPath);
			AssetDatabase.CreateFolder(tempPath, pathList[pathList.Length - 1]);
		}

		/// <summary>
		/// Checks if the filename has the specified extension
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="extension">extension to check, without the . (Example: "json")</param>
		public static void CheckFileExtension(ref string fileName, string extension)
		{
			if (fileName == "") fileName = "save";
			var partsOfFileName = fileName.Split('.');
			if (partsOfFileName.Length < 2 || partsOfFileName[partsOfFileName.Length - 1] != extension)
			{
				fileName += "." + extension;
			}
		}

	}
}
