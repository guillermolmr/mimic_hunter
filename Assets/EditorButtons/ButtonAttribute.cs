using System;

[AttributeUsage(AttributeTargets.Method)]
public class ButtonAttribute : Attribute
{
    // Opcional: puedes agregar un nombre personalizado para el botón
    public string ButtonName;

    public ButtonAttribute(string buttonName = null)
    {
        ButtonName = buttonName;
    }
}