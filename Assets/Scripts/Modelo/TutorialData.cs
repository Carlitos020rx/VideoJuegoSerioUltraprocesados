using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PasoTutorial
{
    [TextArea(2, 5)]
    [Tooltip("Texto que se escribe con efecto typewriter")]
    public string texto;

    [Tooltip("Voz en off opcional que suena mientras se escribe el texto")]
    public AudioClip vozEnOff;

    [Tooltip("Imagen opcional para este paso (captura del prototipo)")]
    public Sprite imagen;
}

[CreateAssetMenu(fileName = "TutorialNivel1", menuName = "UltraProcessed/Tutorial", order = 11)]
public class TutorialData : ScriptableObject
{
    [Tooltip("Secuencia de pasos del tutorial, en orden")]
    public List<PasoTutorial> pasos = new List<PasoTutorial>();
}
