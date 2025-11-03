using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SomeWhere
{
    public class ProcessorAnimation
    {
        [MenuItem("Assets/压缩动画精度")]
        static void OnPostprocessModel()
        {
            foreach (var obj in Selection.objects)
            {
                var clip = obj as AnimationClip;
                if (clip == null) continue;

                try
                {
                    //浮点数精度压缩为f4
                    var bindings = AnimationUtility.GetCurveBindings(clip);
                    foreach (var binding in bindings)
                    {
                        var curve = AnimationUtility.GetEditorCurve(clip, binding);
                        if (curve == null || curve.keys == null)
                        {
                            continue;
                        }

                        var keyFrames = curve.keys;
                        for (var i = 0; i < keyFrames.Length; i++)
                        {
                            var key = keyFrames[i];
                            key.value = float.Parse(key.value.ToString("f4"));
                            key.inTangent = float.Parse(key.inTangent.ToString("f4"));
                            key.outTangent = float.Parse(key.outTangent.ToString("f4"));
                            keyFrames[i] = key;
                        }

                        curve.keys = keyFrames;
                        AnimationUtility.SetEditorCurve(clip, binding, curve);
                    }


                    // var curves = AnimationUtility.GetAllCurves(clip);
                    // for (var ii = 0; ii < curves.Length; ++ii)
                    // {
                    //     var curveDate = curves[ii];
                    //     if (curveDate.curve == null || curveDate.curve.keys == null)
                    //     {
                    //         continue;
                    //     }
                    //
                    //     var keyFrames = curveDate.curve.keys;
                    //     for (var i = 0; i < keyFrames.Length; i++)
                    //     {
                    //         var key = keyFrames[i];
                    //         key.value = float.Parse(key.value.ToString("f4"));
                    //         key.inTangent = float.Parse(key.inTangent.ToString("f4"));
                    //         key.outTangent = float.Parse(key.outTangent.ToString("f4"));
                    //         keyFrames[i] = key;
                    //     }
                    //
                    //     curveDate.curve.keys = keyFrames;
                    //     clip.SetCurve(curveDate.path, curveDate.type, curveDate.propertyName, curveDate.curve);
                    // }
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }
    }
}