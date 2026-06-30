using UnityEngine;
using UnityEngine.UI;

public class BarraVida : MonoBehaviour
{
    [Header("REFERENCIAS")]
    public JugadorController jugador;
    public Image barraVida;
    public int vidaMaxima = 3;

    void Start()
    {
        // Buscar al jugador si no está asignado
        if (jugador == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                jugador = player.GetComponent<JugadorController>();
                Debug.Log("BarraVida: Jugador encontrado automáticamente");
            }
            else
            {
                Debug.LogError("BarraVida: No se encontró al jugador con Tag 'Player'");
            }
        }

        // Buscar la barra de vida si no está asignada
        if (barraVida == null)
        {
            barraVida = GetComponent<Image>();
            if (barraVida == null)
            {
                Debug.LogError("BarraVida: No se encontró la imagen de la barra. Asigna la referencia manualmente.");
            }
        }

        // Actualizar al inicio
        ActualizarBarra();
    }

    void Update()
    {
        // Actualizar la barra cada frame
        ActualizarBarra();
    }

    void ActualizarBarra()
    {
        // Verificar que todo esté asignado
        if (jugador == null)
        {
            // Intentar buscar nuevamente
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                jugador = player.GetComponent<JugadorController>();
            }
            return;
        }

        if (barraVida == null)
        {
            Debug.LogWarning("BarraVida: La imagen de la barra no está asignada");
            return;
        }

        // Calcular la vida normalizada (0-1)
        float vidaNormalizada = (float)jugador.vida / vidaMaxima;
        vidaNormalizada = Mathf.Clamp01(vidaNormalizada);

        // Actualizar la barra
        barraVida.fillAmount = vidaNormalizada;

        // Cambiar color según la vida
        if (vidaNormalizada > 0.6f)
            barraVida.color = Color.green;
        else if (vidaNormalizada > 0.3f)
            barraVida.color = new Color(1f, 0.8f, 0f); // Amarillo
        else
            barraVida.color = Color.red;
    }
}