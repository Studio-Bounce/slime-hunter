//Copyright © 2021 Bus Stop Studios

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
//documentation files (the “Software”), to deal in the Software without restriction, including without limitation
//the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and
//to permit persons to whom the Software is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or substantial portions of
//the Software.

//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
//TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
//THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
//CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
//DEALINGS IN THE SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Project.Build.Commands
{
	public class BuildCommands : IPreprocessBuildWithReport, IPostprocessBuildWithReport
	{
		public static string GetBuildCommandPath()
		{
			string[] res = System.IO.Directory.GetFiles(Application.dataPath, "BuildCommands.cs", SearchOption.AllDirectories);
			if (res.Length == 0)
			{
				Debug.LogError("Unable to locate BuildCommands.cs");
				return null;
			}
			// Get the path
			string path = System.IO.Path.GetDirectoryName(res[0]);
			// Swap the \ to /
			path = path.Replace("\\", "/");
			// Remove the Application Path
			string assetPath = path.Replace(Application.dataPath, "");
			// Remove Editor from the end (this should be fixed based off the asset and can only have on Editor in the path)
			assetPath = assetPath.Replace("Editor", "");

			return assetPath;
		}
		/// <summary>
		/// 
		/// </summary>
		public static void UpdateBuildData()
		{
			string assetPath = GetBuildCommandPath();

			BuildData buildData = (BuildData)AssetDatabase.LoadAssetAtPath("Assets" + assetPath + "BuildData.asset", typeof(BuildData));
			if (buildData != null)
			{
				bool saveAsset = false;
				if (buildData.updateBuildInfo)
				{
					saveAsset = true;
					buildData.buildNumber++;
				}

				if (buildData.useGit)
				{
					saveAsset = true;
					string command = "\"" + Application.dataPath + assetPath + "Editor/Build Scripts/get_git_details.ps1\"";
					//command = "-NoExit -ExecutionPolicy Bypass -File " + command;
					command = "-ExecutionPolicy Bypass -File " + command;
					Debug.Log(command);
					System.Diagnostics.Process process = new System.Diagnostics.Process();
					process.StartInfo = new System.Diagnostics.ProcessStartInfo("powershell.exe", command);
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.UseShellExecute = false;
					process.Start();

					string standardOutput = "";
					while (!process.HasExited)
					{
						standardOutput += process.StandardOutput.ReadToEnd();
					}
					standardOutput += process.StandardOutput.ReadToEnd();
					process.Close();

					string[] outputLines = standardOutput.Split('\n');
					foreach (var line in outputLines)
					{
						if (line.Contains("sha"))
						{
							string shaline = line.Replace("sha", "");
							shaline = shaline.Replace(" ", "");
							buildData.sha = shaline;
						}
						if (line.Contains("branch_name"))
						{
							string branchline = line.Replace("branch_name", "");
							branchline = branchline.Replace(" ", "");
							buildData.branchName = branchline;
						}
					}
				}

				if (saveAsset)
				{
					EditorUtility.SetDirty(buildData);
					AssetDatabase.SaveAssets();
				}
			}
		}

		public int callbackOrder => 0;

		public void OnPreprocessBuild(BuildReport report)
		{
			UpdateBuildData();
		}

		public static bool IsIL2CPPEnabled()
		{
			return PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup) == ScriptingImplementation.IL2CPP;
		}

		public static void ButlerUpload(BuildReport report, BuildData buildData)
		{
			string outputPath = System.IO.Path.GetDirectoryName(report.summary.outputPath);
			string assetPath = GetBuildCommandPath();

			string butler = "\'" + Application.dataPath + assetPath + "Editor/Butler/butler.exe\'";
			string pushFolder = "\"" + outputPath + "\"";

			string platform = "";
			switch(report.summary.platform)
			{
				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneWindows64:
					platform = "windows";
					break;

				case BuildTarget.StandaloneOSX:
					platform = "osx";
					break;

				case BuildTarget.iOS:
					platform = "iOS";
					break;

				case BuildTarget.Android:
					platform = "android";
					break;

				case BuildTarget.StandaloneLinux64:
					platform = "linux";
					break;

				case BuildTarget.WebGL:
					platform = "webGL";
					break;

				default:
					platform = "build";
					break;
			}

			string command = "& " + butler + " -v";
			if (IsIL2CPPEnabled())
			{
				command += " --ignore \'" + Application.productName + "_BackUpThisFolder_ButDontShipItWithYourGame\'";
			}
			foreach (var ignore in buildData.ignore)
			{
				command += " --ignore \'" + ignore + "\'";
			}
			command  += " push \'" + pushFolder + "\' " + buildData.studioName + "/" + buildData.projectName + ":" + platform + " --userversion " + buildData.buildNumber;
			//command = "-NoExit -Command " + command;
			command = "-Command " + command;
			System.Diagnostics.Process process = System.Diagnostics.Process.Start("powershell.exe", command);
			process.WaitForExit();
			process.Close();
		}

		public void OnPostprocessBuild(BuildReport report)
		{
			string assetPath = GetBuildCommandPath();

			BuildData buildData = (BuildData)AssetDatabase.LoadAssetAtPath("Assets" + assetPath + "BuildData.asset", typeof(BuildData));
			if (buildData != null && buildData.uploadBuild)
			{
				try
				{
					ButlerUpload(report, buildData);
				}
				catch (System.Exception ex)
				{
					Debug.LogError(ex.Message);
				}
			}
		}
	}

}
