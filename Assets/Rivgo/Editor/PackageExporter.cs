using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

namespace Rivgo.Editor
{
	public class PackageExporter
	{
		private const string _packageRootPath = "Assets/Rivgo";

		[MenuItem("Rivgo/Export Package")]
		public static void ExportPackageMenuItem()
		{
			string defaultVersion = "0.1.0";
			string suggestedName = $"Flashlight_v{defaultVersion}_Rivgo.unitypackage";
			string outputPath = EditorUtility.SaveFilePanel("Export Rivgo Package", "", suggestedName, "unitypackage");

			if (string.IsNullOrEmpty(outputPath))
			{
				Debug.Log("Package export cancelled by user.");
				return;
			}

			string version = EditorInputDialog.Show("Enter Package Version", "Enter the version for this package:", defaultVersion);
			if (string.IsNullOrEmpty(version))
			{
				Debug.Log("Package export cancelled or version not provided.");
				return;
			}

			ExportPackageLogic(version, outputPath, false);
		}

		public static void ExportPackage()
		{
			string[] args = Environment.GetCommandLineArgs();
			string version = null;
			string outputPath = null;

			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == "-customParameters")
				{
					string parameters = args[i + 1];
					var paramDict = parameters.Split(';')
						.Select(p => p.Split('='))
						.Where(p => p.Length == 2)
						.ToDictionary(p => p[0], p => p[1]);

					paramDict.TryGetValue("version", out version);
					paramDict.TryGetValue("outputPath", out outputPath);
					break;
				}
			}

			if (string.IsNullOrEmpty(version))
			{
				for (int i = 0; i < args.Length; i++)
				{
					if (args[i].StartsWith("version=")) version = args[i].Substring("version=".Length);
					if (args[i].StartsWith("outputPath=")) outputPath = args[i].Substring("outputPath=".Length);
				}
			}


			if (string.IsNullOrEmpty(version) || string.IsNullOrEmpty(outputPath))
			{
				Debug.LogError("PackageExporter: Missing 'version' or 'outputPath' command line arguments.");

				if (Application.isBatchMode)
					EditorApplication.Exit(1);

				return;
			}

			ExportPackageLogic(version, outputPath, Application.isBatchMode);
		}

		private static void ExportPackageLogic(string version, string outputPath, bool isBatchMode)
		{
			string packageName = Path.GetFileName(outputPath);
			Debug.Log($"Attempting to export package: {packageName} (Version: {version}) to path: {outputPath}");

			try
			{
				string outputDir = Path.GetDirectoryName(outputPath);
				if (!Directory.Exists(outputDir))
				{
					Directory.CreateDirectory(outputDir);
					Debug.Log($"Created output directory: {outputDir}");
				}

				AssetDatabase.ExportPackage(_packageRootPath, outputPath, ExportPackageOptions.Recurse);
				Debug.Log($"Successfully exported '{packageName}' to '{outputPath}'.");

				if (isBatchMode)
					EditorApplication.Exit(0);
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed to export package '{packageName}'. Error: {e.Message}");
				Debug.LogException(e);

				if (isBatchMode)
					EditorApplication.Exit(1);
			}
		}
	}

	public class EditorInputDialog : EditorWindow
	{
		private string _description = "Please enter a value:";
		private string _inputText = "";
		private Action<string> _onOk;

		public static string Show(string title, string description, string initialValue = "")
		{
			EditorInputDialog window = GetWindow<EditorInputDialog>(true, title, true);
			window.titleContent = new GUIContent(title);
			window._description = description;
			window._inputText = initialValue;
			string enteredText = null;
			window._onOk = (text) => enteredText = text;
			window.ShowModal();
			return enteredText;
		}

		private void OnGUI()
		{
			EditorGUILayout.LabelField(_description, EditorStyles.wordWrappedLabel);
			GUILayout.Space(10);
			_inputText = EditorGUILayout.TextField(_inputText);
			GUILayout.Space(10);

			if (GUILayout.Button("OK"))
			{
				if (_onOk != null) _onOk(_inputText);
				Close();
			}
			if (GUILayout.Button("Cancel"))
			{
				if (_onOk != null) _onOk(null);
				Close();
			}
		}
	}
}