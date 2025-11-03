using UnityEngine;
using UnityEditor;

namespace SomeWhere
{
    [CustomPropertyDrawer(typeof(PathSegment))]
    public class PathSegmentDrawer : PropertyDrawer
    {
        public static float Height { get => EditorGUIUtility.singleLineHeight; }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(rect, label, property))
            {
                EditorGUIUtility.labelWidth = 60;
                rect.height = EditorGUIUtility.singleLineHeight;

                var count = 4;
                var index = 1;
                var point1Rect = new Rect(rect)
                {
                    width = rect.width / count - count*5,
                };

                var point2Rect = new Rect(point1Rect)
                {
                    x = point1Rect.x + rect.width / count + 5,
                };
                index++;

                var cp1Rect = new Rect(point1Rect)
                {
                    x = point1Rect.x + rect.width / count * index + 5 ,
                };
                index++;

                var cp2Rect = new Rect(point1Rect)
                {
                    x = point1Rect.x + rect.width / count * index + 5 ,
                };
                index++;

                var beginProperty = property.FindPropertyRelative("p1");
                var endProperty = property.FindPropertyRelative("p2");
                var cp1Property = property.FindPropertyRelative("cp1");
                var cp2Property = property.FindPropertyRelative("cp2");

                beginProperty.objectReferenceValue = EditorGUI.ObjectField(point1Rect, beginProperty.objectReferenceValue, typeof(PathPoint), true);
                endProperty.objectReferenceValue = EditorGUI.ObjectField(point2Rect, endProperty.objectReferenceValue, typeof(PathPoint), true);
                cp1Property.objectReferenceValue = EditorGUI.ObjectField(cp1Rect, cp1Property.objectReferenceValue, typeof(Transform), true);
                cp2Property.objectReferenceValue = EditorGUI.ObjectField(cp2Rect, cp2Property.objectReferenceValue, typeof(Transform), true);

                if (beginProperty.objectReferenceValue != null && endProperty.objectReferenceValue != null)
                {
                    if (cp1Property.objectReferenceValue != null)
                    {
                        var pointId1 = beginProperty.objectReferenceValue.name;
                        var pointId2 = endProperty.objectReferenceValue.name;
                        var newName = $"{pointId1}->{pointId2}_1";
                        var control = cp1Property.objectReferenceValue as Transform;
                        if (!control.gameObject.name.Equals(newName))
                        {
                            control.gameObject.name = newName;
                        }
                    }

                    if (cp2Property.objectReferenceValue != null)
                    {
                        var pointId1 = beginProperty.objectReferenceValue.name;
                        var pointId2 = endProperty.objectReferenceValue.name;
                        var newName = $"{pointId1}->{pointId2}_2";
                        var control = cp2Property.objectReferenceValue as Transform;
                        if (!control.gameObject.name.Equals(newName))
                        {
                            control.gameObject.name = newName;
                        }
                    }
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Height;
        }
    }
}
