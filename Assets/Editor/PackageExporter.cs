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
		private const string _packageName = "FlashlightSystem";

		public static void ExportPackage()
		{
			try
			{
				if (!Directory.Exists(_exportPackagePath))
				{
					Directory.CreateDirectory(_exportPackagePath);
					Debug.Log($"Created output directory: {_exportPackagePath}");
				}

				var outputPath = Path.Combine(_exportPackagePath, $"{_packageName}");

				AssetDatabase.ExportPackage(_packageRootPath, outputPath, ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
				Debug.Log($"Successfully exported '{_packageName}' to '{outputPath}'.");
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed to export package '{_packageName}'. Error: {e.Message}\n{e.StackTrace}");
			}
		}
	}
}