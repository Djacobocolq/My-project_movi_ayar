using UnityEngine;
using UnityEngine.InputSystem;

public class FlorRecolectable : MonoBehaviour
{
    [Header("CONFIGURACIÓN")]
    public float distanciaInteraccion = 2f;
    public bool recolectada = false;

    [Header("REFERENCIAS")]
    public JugadorController jugador;
    public GameObject botonRecoger; // Botón táctil para móvil (opcional)

    private SpriteRenderer spriteRenderer;
    private Collider2D colliderFlor;
    private Keyboard keyboard;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        colliderFlor = GetComponent<Collider2D>();
        keyboard = Keyboard.current;

        if (jugador == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                jugador = player.GetComponent<JugadorController>();
        }

        // Ocultar botón al inicio
        if (botonRecoger != null)
            botonRecoger.SetActive(false);
    }

    void Update()
    {
        if (recolectada || jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.transform.position);
        bool estaCerca = distancia <= distanciaInteraccion;

        // Mostrar/ocultar botón según distancia
        if (botonRecoger != null)
            botonRecoger.SetActive(estaCerca);

        // Recoger con tecla R
        if (estaCerca && keyboard != null && keyboard.rKey.wasPressedThisFrame)
        {
            Recolectar();
        }
    }

    void Recolectar()
    {
        if (recolectada) return;

        recolectada = true;
        Debug.Log("¡Flor recolectada!");

        // Ocultar botón
        if (botonRecoger != null)
            botonRecoger.SetActive(false);

        // Desaparecer la flor
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (colliderFlor != null)
            colliderFlor.enabled = false;

        // Opcional: Destruir la flor después de un tiempo
        // Destroy(gameObject, 0.5f);
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distanciaInteraccion);
    }
}