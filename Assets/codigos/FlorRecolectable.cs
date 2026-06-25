using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem; // ← Agregar para Input System

public class FlorRecolectable : MonoBehaviour
{
    [Header("CONFIGURACIÓN")]
    public float distanciaInteraccion = 2f;
    public GameObject textoInteraccion;
    public GameObject botonRecoger; // ← AGREGADO
    public bool recolectada = false;

    [Header("REFERENCIAS")]
    public player1 jugador;

    private SpriteRenderer spriteRenderer;
    private Collider2D colliderFlor;
    private Keyboard keyboard; // ← NUEVO para Input System

    void Start()
    {
        // Obtener componentes
        spriteRenderer = GetComponent<SpriteRenderer>();
        colliderFlor = GetComponent<Collider2D>();

        // Obtener teclado
        keyboard = Keyboard.current;

        // Buscar al jugador si no está asignado
        if (jugador == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                jugador = player.GetComponent<player1>();
        }

        // Ocultar texto y botón al inicio
        if (textoInteraccion != null)
            textoInteraccion.SetActive(false);

        if (botonRecoger != null)
            botonRecoger.SetActive(false);
    }

    void Update()
    {
        // Si ya fue recolectada, no hacer nada
        if (recolectada) return;

        // Verificar si el jugador está cerca
        if (jugador != null)
        {
            float distancia = Vector2.Distance(transform.position, jugador.transform.position);

            if (distancia <= distanciaInteraccion)
            {
                // Mostrar texto y botón de interacción
                MostrarInteraccion(true);

                // ==========================================
                // RECOGER CON INPUT SYSTEM (CORREGIDO)
                // ==========================================
                if (keyboard != null && keyboard.eKey.wasPressedThisFrame)
                {
                    Recolectar();
                }
            }
            else
            {
                // Ocultar si está lejos
                MostrarInteraccion(false);
            }
        }
    }

    void MostrarInteraccion(bool mostrar)
    {
        // Mostrar/ocultar texto
        if (textoInteraccion != null)
            textoInteraccion.SetActive(mostrar);

        // Mostrar/ocultar botón táctil
        if (botonRecoger != null)
            botonRecoger.SetActive(mostrar);
    }

    void Recolectar()
    {
        if (recolectada) return;

        recolectada = true;
        Debug.Log("¡Flor recolectada!");

        // Ocultar interacción
        MostrarInteraccion(false);

        // Desaparecer la flor
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (colliderFlor != null)
            colliderFlor.enabled = false;

        // Puedes agregar lógica aquí (ej: sumar puntos)
        // if (jugador != null) jugador.SumarPuntos(10);
    }

    // ==========================================
    // MÉTODO PARA RECOGER DESDE BOTÓN TÁCTIL
    // ==========================================
    public void RecogerTouch()
    {
        if (!recolectada && jugador != null)
        {
            float distancia = Vector2.Distance(transform.position, jugador.transform.position);
            if (distancia <= distanciaInteraccion)
            {
                Recolectar();
            }
        }
    }

    // Visualizar el rango de interacción en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distanciaInteraccion);
    }
}