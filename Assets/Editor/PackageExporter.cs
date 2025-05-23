using UnityEditor;
using UnityEngine;
using System.IO;
using System;

namespace Editor
{
	public class PackageExporter
	{
		private const string _packageRootPath = "Assets";
		private const string _exportPackagePath = "Rivgo/ExportPackage";
		private const string _packageName = "FlashlightSystem.unitypackage";

		public static void ExportPackage()
		{
			try
			{
				if (!Directory.Exists(_exportPackagePath))
				{
					Directory.CreateDirectory(_exportPackagePath);
					Debug.Log($"Created output directory: {_exportPackagePath}");
				}

				var outputPath = Path.Combine(_exportPackagePath, _packageName);

				AssetDatabase.ExportPackage(_packageRootPath, outputPath, ExportPackageOptions.Recurse);
				Debug.Log($"Successfully exported '{_packageName}' to '{outputPath}'.");
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed to export package '{_packageName}'. Error: {e.Message}\n{e.StackTrace}");
			}
		}
	}
}