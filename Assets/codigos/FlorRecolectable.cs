using UnityEngine;
using UnityEngine.InputSystem;

public class FlorRecolectable : MonoBehaviour
{
    [Header("CONFIGURACIÓN")]
    public float distanciaInteraccion = 3f; // ← AUMENTADO
    public bool recolectada = false;

    [Header("REFERENCIAS")]
    public JugadorController jugador;
    public GameObject botonRecoger;

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

        if (botonRecoger != null)
            botonRecoger.SetActive(false);
    }

    void Update()
    {
        if (recolectada || jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.transform.position);
        bool estaCerca = distancia <= distanciaInteraccion;

        if (botonRecoger != null)
            botonRecoger.SetActive(estaCerca);

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

        if (botonRecoger != null)
            botonRecoger.SetActive(false);

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (colliderFlor != null)
            colliderFlor.enabled = false;
    }

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