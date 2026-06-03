
using UnityEngine;

public class DecorativeFloat : MonoBehaviour
{
    [Header("Flotacion")]
    public float amplitud = 15f;
    public float velocidadFlotacion = 1.2f;

    [Header("Rotacion")]
    public float velocidadRotacion = 25f;

    [Header("Offset aleatorio")]
    [Tooltip("Se asigna automaticamente al iniciar para que no todos se muevan igual")]
    public float offsetAleatorio = 0f;

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.localPosition;
        // Offset aleatorio para que cada panel empiece en un punto distinto del ciclo
        offsetAleatorio = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        // Flotacion arriba y abajo con Seno
        float nuevaY = posicionInicial.y + Mathf.Sin(Time.time * velocidadFlotacion + offsetAleatorio) * amplitud;
        float nuevaX = posicionInicial.x + Mathf.Sin(Time.time * velocidadFlotacion * 0.6f + offsetAleatorio) * (amplitud * 0.5f);
        transform.localPosition = new Vector3(nuevaX, nuevaY, posicionInicial.z);

        // Rotacion continua
        transform.Rotate(0f, 0f, velocidadRotacion * Time.deltaTime);
    }
}
