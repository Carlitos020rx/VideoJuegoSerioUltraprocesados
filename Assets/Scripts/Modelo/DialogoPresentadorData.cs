using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ImagenLinea
{
    [Tooltip("Sprite que aparecera en pantalla")]
    public Sprite sprite;

    [Tooltip("Posicion en el canvas (anchoredPosition X, Y). 0,0 = centro")]
    public Vector2 posicion;

    [Tooltip("Tamaño en pixeles (width, height)")]
    public Vector2 tamano = new Vector2(200f, 200f);
}

[System.Serializable]
public class LineaPresentador
{
    [TextArea(2, 5)]
    [Tooltip("Texto que se escribe con efecto typewriter")]
    public string texto;

    [Tooltip("Sprite del presentador para esta linea (opcional)")]
    public Sprite spritePresentador;

    [Tooltip("Voz en off opcional de esta linea")]
    public AudioClip vozEnOff;

    [Tooltip("Imagenes que aparecen en pantalla durante esta linea (con animacion pop)")]
    public List<ImagenLinea> imagenes = new List<ImagenLinea>();
}

[CreateAssetMenu(fileName = "DialogoPresentador_", menuName = "UltraProcessed/Dialogo Presentador", order = 12)]
public class DialogoPresentadorData : ScriptableObject
{
    [Tooltip("Lineas de dialogo de este segmento, en orden")]
    public List<LineaPresentador> lineas = new List<LineaPresentador>();

    [Tooltip("PNG opcional del titulo que cae antes de iniciar este dialogo. Dejar vacio si no quiere titulo")]
    public Sprite tituloEntrada;
}
