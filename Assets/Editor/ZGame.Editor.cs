using System.IO;
using HybridCLR.Editor;
using UnityEditor;
using UnityEngine;

namespace ZGame.Editor
{

    public class ZGameEditor
    {
        static string AA = string.Format("{0}/AA", Application.dataPath);

        [MenuItem("ZGame/BuildAB")]
        static void BuildAB()
        {
            // -------------------------- HybridCLR 热更新处理
            // 热更代码编译
            HybridCLR.Editor.Commands.PrebuildCommand.GenerateAll();

            // 复制到热更文件到打包目录
            var hotUpdateOutput = string.Format("{0}/HybridCLR", AA);
            if (!Directory.Exists(hotUpdateOutput)) Directory.CreateDirectory(hotUpdateOutput);
            for (int i = 0; i < SettingsUtil.HotUpdateAssemblyFilesExcludePreserved.Count; i++)
            {
                var fileName = SettingsUtil.HotUpdateAssemblyFilesExcludePreserved[i];
                var dllPath = string.Format("{0}/{1}/{2}", SettingsUtil.HotUpdateDllsRootOutputDir, EditorUserBuildSettings.activeBuildTarget, fileName);
                var outputPath = string.Format("{0}/{1}.bytes", hotUpdateOutput, fileName);
                if (File.Exists(dllPath))
                {
                    Debug.LogFormat("Copy hot update dll: {0}", fileName);
                    File.Copy(dllPath, outputPath, true);
                }
                else
                {
                    Debug.LogError("can`t find dll: " + dllPath);
                }
            }
            // -------------------------- HybridCLR 热更新处理
        }
    }
}