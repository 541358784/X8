using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using Object = UnityEngine.Object;

/// <summary> 
/// 脚本作用说明： 
///     将隶属于这个动画状态机的动画片段内嵌至该状态机，其作用就是方便管理动画片段
/// </summary>
/// <summary> 
/// 操作说明：
///     1.创建好状态机及该状态机对应的动画片段后，点击该状态机文件，单击鼠标右键，选择【将动画片段内嵌至指定状态机】即可
///     2.创建好状态机及该状态机对应的动画片段后，点击左上角Assets栏位，选择【将动画片段内嵌至指定状态机】即可
/// </summary>
public static class AutoAddAnimClipsToTargetAnimatorController
{
    /// <summary>
    /// 功能放置位置说明：
    ///     本来想添加一个Tools栏位用来存放自定义的工具，但是如果那样的话就无法右键显示这个工具了，那样的做法不够灵活。
    /// </summary>
    [MenuItem("Assets/将动画片段内嵌至指定状态机")]
    public static void AutoAddAnimationClipsToNest()
    {
        //声明空的状态机
        AnimatorController animatorController = null;

        //声明空的动画片段数组
        AnimationClip[] clips;

        //判断：如果在Project面板中选中的内容的类型是【动画状态机】
        if (Selection.activeObject is AnimatorController)
        {
            animatorController = (AnimatorController) Selection.activeObject;
            clips = animatorController.animationClips;

            var instantiateMotionDict = new Dictionary<string, Motion>();


            //如果状态机不为空 并且 动画片段数组的长度 > 0
            if (animatorController != null && clips.Length > 0)
            {
                foreach (AnimationClip clipCell in clips)
                {
                    var clipCellAssetPath = AssetDatabase.GetAssetPath(clipCell);
                    if (clipCellAssetPath.EndsWith(".anim"))
                    {
                        var newClipCell = Object.Instantiate(clipCell);
                        newClipCell.name = clipCell.name;
                        instantiateMotionDict.Add(clipCell.name, newClipCell);
                        AssetDatabase.AddObjectToAsset(newClipCell, animatorController); //将目标资源添加至指定的对象中
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newClipCell)); //导入动画片段资源至指定的位置，这里就是AnimatorController
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(clipCell)); //删除多余的动画片段(这里的多余指的是已经导入完成的重复的动画片段)
                    }
                }

                var layer = animatorController.layers[0];
                var sm = layer.stateMachine;         //获取层状态机
                var childAnimatorStates = sm.states; //获取该层状态机的子状态机

                foreach (var childAnimatorState in childAnimatorStates)
                {
                    if (instantiateMotionDict.ContainsKey(childAnimatorState.state.name))
                    {
                        childAnimatorState.state.motion = instantiateMotionDict[childAnimatorState.state.name];
                    }
                }

                instantiateMotionDict.Clear();
                //Log提示，如果操作成功，返回被选中的动画状态机的名字和内嵌的动画片段的数量
                Debug.Log("<color=green> 添加 " + clips.Length.ToString() + " 个动画片段至选中的状态机: </color><color=blue>" +
                    animatorController.name + "</color> 中");
            }
            else
            {
                //Log提示，如果进行操作的状态机为空则返回以下内容
                Debug.Log("<color=red> 未进行任何操作. 请选中一个非空的状态机来完成此次动画片段内嵌操作.</color>");
            }

        }

        //Log提示，如果进行操作的状态机为空则返回以下内容
        Debug.Log("<color=purple> 未进行任何操作. 请选中状态机来完成此次动画片段内嵌操作.</color>");

    }


    [MenuItem("Assets/删除指定状态机内嵌的动画片段")]
    public static void DeletedAnimationClipsToNest()
    {
        Object[] selectedAsstes = Selection.objects;

        if (selectedAsstes.Length < 0)
        {
            Debug.Log("<color=orange> 请选择一个或多个动画文件进行删除 !_!</color>");

            return;
        }

        foreach (Object asset in selectedAsstes)
        {
            if (AssetDatabase.IsSubAsset(asset))
            {
                string path = AssetDatabase.GetAssetPath(asset);
                Object.DestroyImmediate(asset, true);
                AssetDatabase.ImportAsset(path);
                Debug.Log("<color=green> 您已经成功删除" + selectedAsstes.Length + "个嵌套式动画文件 !_!</color>");
            }
        }


    }
}

public class AnimationConfigurationInfo : ScriptableObject
{
    public string[] defaultAnimatons;
}