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

		[MenuItem("Rivgo/Export Package (Manual)")]
		public static void ExportPackageMenuItem()
		{
			string defaultVersion = "0.1.0";
			string basePackageName = "FlashlightSystem";
			string suggestedName = $"{basePackageName}_v{defaultVersion}_Rivgo.unitypackage";
			string outputPath = EditorUtility.SaveFilePanel("Export Rivgo Package", "", suggestedName, "unitypackage");

			if (string.IsNullOrEmpty(outputPath))
			{
				Debug.Log("Package export cancelled by user.");
				return;
			}

			string versionFromDialog = EditorInputDialog.Show("Enter Package Version", "Enter the version for this package:", defaultVersion);
			if (string.IsNullOrEmpty(versionFromDialog))
			{
				Debug.Log("Package export cancelled or version not provided.");
				return;
			}

			string chosenFileName = Path.GetFileNameWithoutExtension(outputPath);
			string chosenExtension = Path.GetExtension(outputPath);
			string directory = Path.GetDirectoryName(outputPath);

			bool nameIncludesVersion = System.Text.RegularExpressions.Regex.IsMatch(chosenFileName, @"_v\d+\.\d+\.\d+");

			if (!nameIncludesVersion)
				outputPath = Path.Combine(directory, $"{chosenFileName}_v{versionFromDialog}{chosenExtension}");
			else
				Debug.LogWarning($"Output filename '{chosenFileName}' already seems to contain a version. The version from input dialog '{versionFromDialog}' will be used for metadata/logging if different, but filename remains as chosen.");


			ExportPackageLogic(versionFromDialog, outputPath, false);
		}

		public static void ExportPackage()
		{
			Debug.Log("Attempting to export package from command line...");
			string[] args = Environment.GetCommandLineArgs();
			string version = null;
			string outputPath = null;

			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == "-customParameters")
				{
					if (i + 1 < args.Length)
					{
						string parameters = args[i + 1];
						Debug.Log($"Received -customParameters: {parameters}");
						var paramDict = parameters.Split(';')
							.Select(p => p.Split('='))
							.Where(p => p.Length == 2)
							.ToDictionary(p => p[0].Trim(), p => p[1].Trim(), StringComparer.OrdinalIgnoreCase);

						paramDict.TryGetValue("version", out version);
						paramDict.TryGetValue("outputPath", out outputPath);
						break;
					}
				}
			}

			if (string.IsNullOrEmpty(version) || string.IsNullOrEmpty(outputPath))
			{
				Debug.Log("Did not find parameters via -customParameters, checking for direct key=value arguments.");
				for (int i = 0; i < args.Length; i++)
				{
					if (args[i].StartsWith("version=", StringComparison.OrdinalIgnoreCase))
						version = args[i]["version=".Length..];
					if (args[i].StartsWith("outputPath=", StringComparison.OrdinalIgnoreCase))
						outputPath = args[i]["outputPath=".Length..];
				}
			}

			Debug.Log($"Parsed parameters: Version='{version}', OutputPath='{outputPath}'");

			if (string.IsNullOrEmpty(version) || string.IsNullOrEmpty(outputPath))
			{
				Debug.LogError("PackageExporter: Missing 'version' or 'outputPath' command line arguments. Required format: -customParameters \"version=1.0.0;outputPath=/path/to/package.unitypackage\" or individual arguments: version=1.0.0 outputPath=/path/to/package.unitypackage");
				if (Application.isBatchMode)
					EditorApplication.Exit(1);
				return;
			}

			ExportPackageLogic(version, outputPath, Application.isBatchMode);
		}

		private static void ExportPackageLogic(string version, string outputPath, bool isBatchMode)
		{
			string packageNameWithVersion = Path.GetFileName(outputPath);
			Debug.Log($"Starting package export. Version: '{version}', Package Name: '{packageNameWithVersion}', Output Path: '{outputPath}'");
			Debug.Log($"Root path for export: '{_packageRootPath}'");

			try
			{
				string outputDir = Path.GetDirectoryName(outputPath);
				if (!Directory.Exists(outputDir))
				{
					Directory.CreateDirectory(outputDir);
					Debug.Log($"Created output directory: {outputDir}");
				}

				AssetDatabase.ExportPackage(_packageRootPath, outputPath, ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
				Debug.Log($"Successfully exported '{packageNameWithVersion}' to '{outputPath}'.");

				if (isBatchMode)
					EditorApplication.Exit(0);
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed to export package '{packageNameWithVersion}'. Error: {e.Message}\n{e.StackTrace}");
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
		private bool _initialized = false;

		public static string Show(string title, string description, string initialValue = "")
		{
			EditorInputDialog window = GetWindow<EditorInputDialog>(true, title, true);
			window.titleContent = new GUIContent(title);
			window._description = description;
			window._inputText = initialValue;
			string enteredText = null;
			window._onOk = (text) => enteredText = text;
			window.minSize = new Vector2(300, 150);
			window.maxSize = new Vector2(300, 150);
			window.ShowModal();
			return enteredText;
		}

		private void OnGUI()
		{
			EditorGUILayout.LabelField(_description, EditorStyles.wordWrappedLabel);
			GUILayout.Space(10);

			GUI.SetNextControlName("InputField");
			_inputText = EditorGUILayout.TextField(_inputText);
			GUILayout.Space(20);

			if (!_initialized)
			{
				EditorGUI.FocusTextInControl("InputField");
				_initialized = true;
			}

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			if (GUILayout.Button("OK", GUILayout.Width(100)))
			{
				_onOk?.Invoke(_inputText);
				Close();
			}
			if (GUILayout.Button("Cancel", GUILayout.Width(100)))
			{
				_onOk?.Invoke(null);
				Close();
			}
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(10);
		}
	}
}