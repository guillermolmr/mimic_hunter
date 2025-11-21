using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
public class ButtonDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        // Dibuja el inspector normal primero
        base.OnInspectorGUI();

        // Accede al objeto MonoBehaviour
        MonoBehaviour mono = (MonoBehaviour)target;

        // Obtiene todos los métodos (públicos y privados)
        MethodInfo[] methods = mono.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var method in methods)
        {
            // Solo métodos sin parámetros
            if (method.GetCustomAttribute<ButtonAttribute>() != null && method.GetParameters().Length == 0)
            {
                ButtonAttribute attr = method.GetCustomAttribute<ButtonAttribute>();
                string buttonLabel = string.IsNullOrEmpty(attr.ButtonName) ? method.Name : attr.ButtonName;

                if (GUILayout.Button(buttonLabel))
                {
                    method.Invoke(mono, null);
                }
            }
        }
    }
}
