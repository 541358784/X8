using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using System.IO;
using UnityEditor.iOS.Xcode;
#endif

public class CopyPrivacyInfoToXcodeProject
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
#if UNITY_IOS
        if (buildTarget == BuildTarget.iOS)
        {
            // 将".xcprivacy"文件复制到Xcode项目的"Resources"目录下
            string sourcePath = Path.Combine(Application.dataPath, "Plugins/iOS/PrivacyInfo.xcprivacy");
            //string destinationPath = Path.Combine(buildPath, "Unity-iPhone/PrivacyInfo.xcprivacy");
            //FileUtil.CopyFileOrDirectory(sourcePath, destinationPath);

            AddFileToXcodeProject(buildPath, sourcePath);
        }
#endif
    }
    
#if UNITY_IOS
    // 将文件添加到 Xcode 项目中的方法
    private static void AddFileToXcodeProject(string buildPath, string filePath)
    {
        string projPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";

        string xcodeProjectPath = projPath;

        // 将文件添加到 Xcode 项目中
        PBXProject proj = new PBXProject();
        proj.ReadFromFile(xcodeProjectPath);
        
        string targetGUID = "";
#if UNITY_2019_3_OR_NEWER
        targetGUID = proj.GetUnityMainTargetGuid();
#else
        targetGUID = proj.TargetGuidByName("Unity-iPhone");
#endif
        
        string fileGuid = proj.AddFile(filePath, "PrivacyInfo.xcprivacy");
        proj.AddFileToBuild(targetGUID, fileGuid);

        // 保存修改
        proj.WriteToFile(xcodeProjectPath);
    }
#endif
}