using UnityEditor;
using UnityEngine;

namespace AppIconChanger.Editor
{
    [CustomEditor(typeof(AlternateIcon))]
    public class AlternateIconEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var typed = (AlternateIcon) target;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("platform"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("iconName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("type"));
            
            if (typed.type == AlternateIconType.AutoGenerate)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("source"));
                EditorGUILayout.HelpBox("If you build this project using the -nographics property, the automatic icon resizing process will not work. Please use Manual mode.", MessageType.Warning);
            }
            else
            {
                if (typed.platform == RuntimePlatform.IPhonePlayer)
                {
                    EditorGUILayout.Space();
                    {
                        EditorGUILayout.LabelField("iPhone Notification");
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("iPhoneNotification40px"),
                            new GUIContent("x2 - 40px"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("iPhoneNotification60px"),
                            new GUIContent("x3 - 60px"));
                    }
                    {
                        EditorGUILayout.LabelField("iPhone Settings");
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("iPhoneSettings58px"),
                            new GUIContent("x2 - 58px"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("iPhoneSettings87px"),
                            new GUIContent("x3 - 87px"));
                    }
                    {
                        EditorGUILayout.LabelField("iPhone Spotlight");
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("iPhoneSpotlight80px"),
                            new GUIContent("x2 - 80px"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("iPhoneSpotlight120px"),
                            new GUIContent("x3 - 120px"));
                    }
                    {
                        EditorGUILayout.LabelField("iPhone App");
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("iPhoneApp120px"),
                            new GUIContent("x2 - 120px"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("iPhoneApp180px"),
                            new GUIContent("x3 - 180px"));
                    }
                    {
                        EditorGUILayout.LabelField("iPad Notifications");
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("iPadNotifications20px"),
                            new GUIContent("x1 - 20px"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("iPadNotifications40px"),
                            new GUIContent("x2 - 40px"));
                    }
                    {
                        EditorGUILayout.LabelField("iPad Settings");
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("iPadSettings29px"),
                            new GUIContent("x1 - 29px"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("iPadSettings58px"),
                            new GUIContent("x2 - 58px"));
                    }
                    {
                        EditorGUILayout.LabelField("iPad Spotlight");
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("iPadSpotlight40px"),
                            new GUIContent("x1 - 40px"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("iPadSpotlight80px"),
                            new GUIContent("x2 - 80px"));
                    }
                    {
                        EditorGUILayout.LabelField("iPad App");
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("iPadApp76px"),
                            new GUIContent("x1 - 76px"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("iPadApp152px"),
                            new GUIContent("x2 - 152px"));
                    }
                    {
                        EditorGUILayout.LabelField("iPadPro App");
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("iPadProApp167px"),
                            new GUIContent("x2 - 167px"));
                    }
                    {
                        EditorGUILayout.LabelField("AppStore");
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("appStore1024px"),
                            new GUIContent("x1 - 1024px"));
                    }
                }
                else
                {
                    EditorGUILayout.Space();
                    {
                        EditorGUILayout.LabelField("mipmap-ldpi");
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("androidMipmap_ldpi36"),
                            new GUIContent("36px"));
                        EditorGUILayout.LabelField("mipmap-mdpi");
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("androidMipmap_mdpi48"),
                            new GUIContent("48px"));
                        EditorGUILayout.LabelField("mipmap-hdpi");
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("androidMipmap_hdpi72"),
                            new GUIContent("72px"));
                        EditorGUILayout.LabelField("mipmap-xhdpi");
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("androidMipmap_xhdpi96"),
                            new GUIContent("96px"));
                        EditorGUILayout.LabelField("mipmap-xxhdpi");
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("androidMipmap_xxhdpi144"),
                            new GUIContent("144px"));
                        EditorGUILayout.LabelField("mipmap-xxxhdpi");
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("androidMipmap_xxxhdpi192"),
                            new GUIContent("192px"));
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
