using UnityEngine;

[CreateAssetMenu(fileName = "Sello_", menuName = "UltraProcessed/Sello Nutricional", order = 5)]
public class SelloNutricional : ScriptableObject
{
    [Tooltip("Nombre del sello, ej: Exceso en Azucares")]
    public string nombreSello;

    [Tooltip("Sprite del sello (opcional, para mostrar en feedback)")]
    public Sprite sprite;

    [TextArea(3, 6)]
    [Tooltip("Descripcion educativa que se muestra en el feedback")]
    public string descripcion;
}
