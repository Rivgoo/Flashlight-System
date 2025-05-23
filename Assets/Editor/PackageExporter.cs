using UnityEditor;
using UnityEngine;
using System.IO;
using System;

namespace Editor
{
	public class PackageExporter
	{
		private const string _packageRootPath = "Assets/Rivgo";
		private const string _exportPackagePath = "ExportPackage";
		private const string _packageName = "FlashlightSystem.unitypackage";

		public static void ExportPackage()
		{
			try
			{
				var exportPackagePath = Path.Combine(_packageRootPath, _exportPackagePath);

				if (!Directory.Exists(exportPackagePath))
				{
					Directory.CreateDirectory(exportPackagePath);
					Debug.Log($"Created output directory: {exportPackagePath}");
				}

				var outputPath = Path.Combine(exportPackagePath, _packageName);

				AssetDatabase.ExportPackage(_packageRootPath, outputPath, ExportPackageOptions.Recurse);
				Debug.Log($"Successfully exported '{_packageName}' to '{Path.GetFullPath(outputPath)}'.");
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed to export package '{_packageName}'. Error: {e.Message}\n{e.StackTrace}");
			}
		}
	}
}