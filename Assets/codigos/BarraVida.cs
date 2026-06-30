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
        if (jugador == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                jugador = player.GetComponent<JugadorController>();
        }

        if (barraVida == null)
            barraVida = GetComponent<Image>();

        ActualizarBarra();
    }

    void Update()
    {
        ActualizarBarra();
    }

    // ==========================================
    // MÉTODO PÚBLICO PARA ACTUALIZAR LA BARRA
    // ==========================================
    public void ActualizarBarra()
    {
        if (jugador == null || barraVida == null) return;

        float vidaNormalizada = (float)jugador.vida / vidaMaxima;
        vidaNormalizada = Mathf.Clamp01(vidaNormalizada);

        barraVida.fillAmount = vidaNormalizada;

        if (vidaNormalizada > 0.6f)
            barraVida.color = Color.green;
        else if (vidaNormalizada > 0.3f)
            barraVida.color = new Color(1f, 0.8f, 0f);
        else
            barraVida.color = Color.red;
    }
}