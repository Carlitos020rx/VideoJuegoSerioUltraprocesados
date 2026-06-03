using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnciclopediaConfig", menuName = "UltraProcessed/Enciclopedia Config", order = 4)]
public class EnciclopediaConfig : ScriptableObject
{
    [Header("Paginas del cuaderno")]
    [Tooltip("Cada Sprite es un cuaderno abierto completo (2 paginas adentro). Se navegan en orden.")]
    public List<Sprite> paginas = new List<Sprite>();

    [Header("Reglas de arrastre")]
    [Tooltip("Porcentaje minimo del cuaderno que debe quedar visible al arrastrar (0.5 = 50%)")]
    [Range(0.1f, 1f)]
    public float porcentajeMinimoVisible = 0.5f;

    [Header("Animacion")]
    [Tooltip("Duracion de la animacion de subir/bajar la enciclopedia (segundos)")]
    public float duracionAnimacion = 0.5f;

    [Tooltip("Distancia extra hacia abajo cuando el cuaderno se cierra (en pixeles). 0 = justo debajo de la pantalla.")]
    public float offsetCerrado = 0f;

    [Header("Hover de la pestaña")]
    [Tooltip("Escala que toma la pestaña al hacer hover")]
    public float escalaHover = 1.15f;
    public float duracionHover = 0.2f;
}
