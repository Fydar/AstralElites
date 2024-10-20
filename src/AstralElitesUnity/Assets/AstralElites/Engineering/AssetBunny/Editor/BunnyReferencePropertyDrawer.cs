using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class AdvancedGUI
{
    public static T GetPropertyValue<T>(SerializedProperty property)
    {
        return (T)GetPropertyValue(property);
    }

    public static object GetPropertyValue(SerializedProperty property)
    {
        object baseObject = property.serializedObject.targetObject;

        var remainingPropertyPath = property.propertyPath.AsSpan();
        do
        {
            if (remainingPropertyPath.StartsWith("Array.data["))
            {
                int endArrayIndex = remainingPropertyPath.IndexOf(']');
                var arrayIndexSpan = remainingPropertyPath[11..endArrayIndex];
                remainingPropertyPath = remainingPropertyPath[(endArrayIndex + 1)..];

                baseObject = GetArrayValue(baseObject, int.Parse(arrayIndexSpan), out _);
            }
            else
            {
                int nextSeparatorIndex = remainingPropertyPath.IndexOf('.');
                var currentElement = remainingPropertyPath;
                if (nextSeparatorIndex != -1)
                {
                    currentElement = remainingPropertyPath[..nextSeparatorIndex];
                    remainingPropertyPath = remainingPropertyPath[(nextSeparatorIndex + 1)..];
                }
                else
                {
                    remainingPropertyPath = ReadOnlySpan<char>.Empty;
                }

                baseObject = GetMemberValue(baseObject, currentElement);
            }
        } while (remainingPropertyPath.Length != 0);

        return baseObject;
    }

    public static object[] GetPropertyValues(SerializedProperty property)
    {
        object[] baseObjects = new object[property.serializedObject.targetObjects.Length];
        property.serializedObject.targetObjects.CopyTo(baseObjects, 0);

        var remainingPropertyPath = property.propertyPath.AsSpan();
        do
        {
            if (remainingPropertyPath.StartsWith("Array.data["))
            {
                int endArrayIndex = remainingPropertyPath.IndexOf(']');
                var arrayIndexSpan = remainingPropertyPath[11..endArrayIndex];
                remainingPropertyPath = remainingPropertyPath[(endArrayIndex + 1)..];

                for (int i = 0; i < baseObjects.Length; i++)
                {
                    baseObjects[i] = GetArrayValue(baseObjects[i], int.Parse(arrayIndexSpan), out _);
                }
            }
            else
            {
                int nextSeparatorIndex = remainingPropertyPath.IndexOf('.');
                var currentElement = remainingPropertyPath;
                if (nextSeparatorIndex != -1)
                {
                    currentElement = remainingPropertyPath[..nextSeparatorIndex];
                    remainingPropertyPath = remainingPropertyPath[(nextSeparatorIndex + 1)..];
                }
                else
                {
                    remainingPropertyPath = ReadOnlySpan<char>.Empty;
                }

                for (int i = 0; i < baseObjects.Length; i++)
                {
                    baseObjects[i] = GetMemberValue(baseObjects[i], currentElement);
                }
            }
        } while (remainingPropertyPath.Length != 0);

        return baseObjects;
    }

    public static void SetPropertyValues(SerializedProperty property, Func<object> valueFactory)
    {
        object[] baseObjects = new object[property.serializedObject.targetObjects.Length];
        property.serializedObject.targetObjects.CopyTo(baseObjects, 0);

        var remainingPropertyPath = property.propertyPath.AsSpan();
        do
        {
            if (remainingPropertyPath.StartsWith("Array.data["))
            {
                int endArrayIndex = remainingPropertyPath.IndexOf(']');
                var arrayIndexSpan = remainingPropertyPath[11..endArrayIndex];
                remainingPropertyPath = remainingPropertyPath[(endArrayIndex + 1)..];

                if (remainingPropertyPath.Length == 0)
                {
                    for (int i = 0; i < baseObjects.Length; i++)
                    {
                        SetArrayValue(baseObjects[i], int.Parse(arrayIndexSpan), valueFactory.Invoke());
                    }
                    return;
                }
                else
                {
                    for (int i = 0; i < baseObjects.Length; i++)
                    {
                        baseObjects[i] = GetArrayValue(baseObjects[i], int.Parse(arrayIndexSpan), out _);
                    }
                }
            }
            else
            {
                int nextSeparatorIndex = remainingPropertyPath.IndexOf('.');
                var currentElement = remainingPropertyPath;
                if (nextSeparatorIndex != -1)
                {
                    currentElement = remainingPropertyPath[..nextSeparatorIndex];
                    remainingPropertyPath = remainingPropertyPath[(nextSeparatorIndex + 1)..];
                }
                else
                {
                    for (int i = 0; i < baseObjects.Length; i++)
                    {
                        SetMemberValue(baseObjects[i], currentElement, valueFactory.Invoke());
                    }
                    return;
                }

                for (int i = 0; i < baseObjects.Length; i++)
                {
                    baseObjects[i] = GetMemberValue(baseObjects[i], currentElement);
                }
            }
        } while (remainingPropertyPath.Length != 0);
    }

    private static object GetMemberValue(object source, ReadOnlySpan<char> name)
    {
        var fieldInfo = GetField(source, name);
        return fieldInfo?.GetValue(source);
    }

    private static void SetMemberValue(object source, ReadOnlySpan<char> name, object value)
    {
        var fieldInfo = GetField(source, name);
        fieldInfo?.SetValue(source, value);
    }

    private static object GetArrayValue(object source, int index, out bool isOutOfRange)
    {
        if (source is not IList list || index >= list.Count)
        {
            isOutOfRange = true;
            return null;
        }
        isOutOfRange = false;
        return list[index];
    }

    private static void SetArrayValue(object source, int index, object value)
    {
        if (source is not IList list)
        {
            return;
        }
        list[index] = value;
    }

    private static FieldInfo GetField(object source, ReadOnlySpan<char> name)
    {
        if (source == null)
        {
            return null;
        }

        var typeInfo = source.GetType().GetTypeInfo();
        var fieldInfo = typeInfo.GetField(name.ToString(), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        return fieldInfo;
    }
}

[CustomPropertyDrawer(typeof(BunnyReference<>), true)]
public class BunnyReferencePropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var guidProperty = property.FindPropertyRelative("guid");

        string assetPath = AssetDatabase.GUIDToAssetPath(guidProperty.stringValue);

        var fieldType = fieldInfo.FieldType;
        if (fieldType.IsArray)
        {
            fieldType = fieldType.GetElementType();
        }
        var arguments = fieldType.GetGenericArguments();
        var currentValue = AssetDatabase.LoadMainAssetAtPath(assetPath);

        EditorGUI.BeginChangeCheck();
        var newValue = EditorGUI.ObjectField(position, currentValue, arguments[0], false);
        if (EditorGUI.EndChangeCheck())
        {
            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(newValue, out string guid, out long _))
            {
                guidProperty.stringValue = guid;
            }
        }
    }
}
