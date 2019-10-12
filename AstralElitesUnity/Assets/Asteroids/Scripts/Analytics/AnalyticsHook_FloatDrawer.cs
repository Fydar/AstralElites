using System.Reflection;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer (typeof (AnalyticsHook_Float))]
public class AnalyticsHook_FloatDrawer : PropertyDrawer
{
	public override bool CanCacheInspectorGUI (SerializedProperty property)
	{
		return false;
	}

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		return 120;
	}

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		Rect labelRect = new Rect (position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
		Rect graphRect = new Rect (position.x, labelRect.yMax, position.width, position.yMax - labelRect.yMax);

		EditorGUI.LabelField (labelRect, label);

		EditorGUI.DrawRect (graphRect, new Color (0.1f, 0.1f, 0.1f, 1.0f));

		AnalyticsHook_Float target = (AnalyticsHook_Float)GetTargetObjectOfProperty (property);

		if (target == null || target.data == null)
			return;

		float maxValue = float.MinValue;
		for (int i = 0; i < target.data.Count; i++)
			maxValue = Mathf.Max (maxValue, target.data[i]);

		Vector3[] graphInfo = new Vector3[target.data.Count];
		for (int i = 0; i < target.data.Count; i++)
		{
			float time = (float)i / (target.data.Count - 1);
			float value = target.data[i] / maxValue;

			float yPos = Mathf.Lerp (graphRect.yMax, graphRect.yMin, value);
			float xPos = Mathf.Lerp (graphRect.xMin, graphRect.xMax, time);

			graphInfo[i] = new Vector3 (xPos, yPos, 0);
		}

		Handles.BeginGUI ();
		Handles.color = Color.green;
		Handles.DrawPolyLine (graphInfo);
		Handles.EndGUI ();
	}

	public static object GetTargetObjectOfProperty (SerializedProperty prop)
	{
		var path = prop.propertyPath.Replace (".Array.data[", "[");
		object obj = prop.serializedObject.targetObject;
		var elements = path.Split ('.');

		foreach (var element in elements)
		{
			if (element.Contains ("["))
			{
				var elementName = element.Substring (0, element.IndexOf ("["));
				var index = System.Convert.ToInt32 (element.Substring (element.IndexOf ("[")).Replace ("[", "").Replace ("]", ""));
				obj = GetValue_Imp (obj, elementName, index);
			}
			else
			{
				obj = GetValue_Imp (obj, element);
			}
		}
		return obj;
	}

	private static object GetValue_Imp (object source, string name)
	{
		if (source == null)
			return null;
		var type = source.GetType ();

		while (type != null)
		{
			var f = type.GetField (name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if (f != null)
				return f.GetValue (source);

			var p = type.GetProperty (name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
			if (p != null)
				return p.GetValue (source, null);

			type = type.BaseType;
		}
		return null;
	}

	private static object GetValue_Imp (object source, string name, int index)
	{
		var enumerable = GetValue_Imp (source, name) as System.Collections.IEnumerable;

		if (enumerable == null)
			return null;

		var enm = enumerable.GetEnumerator ();

		for (int i = 0; i <= index; i++)
		{
			if (!enm.MoveNext ())
				return null;
		}
		return enm.Current;
	}
}
