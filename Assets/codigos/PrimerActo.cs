using UnityEngine;
using System.Collections;

public class PrimerActo : MonoBehaviour
{
    [Header("CONFIGURACIÓN")]
    public JugadorController jugador;
    public float duracionCast = 3f;
    public GameObject baston;
    public float tiempoQuietoEnemigo = 4f; // Tiempo que el enemigo se queda quieto
    public float tiempoDesaparicion = 1f; // Tiempo para desaparecer

    private bool activado = false;

    void Start()
    {
        if (baston != null)
            baston.SetActive(false);

        // Ignorar colisión con el jugador
        if (jugador != null)
        {
            Collider2D colJugador = jugador.GetComponent<Collider2D>();
            Collider2D colActivador = GetComponent<Collider2D>();

            if (colJugador != null && colActivador != null)
            {
                Physics2D.IgnoreCollision(colActivador, colJugador, true);
                Debug.Log("✅ Colisión entre jugador y primer_acto desactivada");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ==========================================
        // SOLO EL ENEMIGO ACTIVA EL PRIMER ACTO
        // ==========================================
        if (collision.CompareTag("Enemy") && !activado)
        {
            activado = true;
            Debug.Log("🎬 ¡Primer acto activado por el enemigo!");

            // ==========================================
            // 1. DESACTIVAR AL ENEMIGO (QUIETO)
            // ==========================================
            EnemigoBase enemigo = collision.GetComponent<EnemigoBase>();
            if (enemigo != null)
            {
                enemigo.DesactivarEnemigo(tiempoQuietoEnemigo);
                Debug.Log("🛑 Enemigo desactivado por " + tiempoQuietoEnemigo + " segundos");
            }

            // ==========================================
            // 2. ACTIVAR TRANSICIÓN DEL BASTÓN
            // ==========================================
            ActivarTransicion();

            // ==========================================
            // 3. DESAPARECER EL ACTIVADOR
            // ==========================================
            StartCoroutine(Desaparecer());
        }
        else
        {
            Debug.Log("🔍 Algo tocó primer_acto: " + collision.gameObject.name + " (Tag: " + collision.tag + ")");
        }
    }

    void ActivarTransicion()
    {
        if (jugador != null)
        {
            jugador.IniciarTransicionBaston(duracionCast, baston);
            Debug.Log("🔄 Transición del bastón iniciada");
        }
    }

    // ==========================================
    // CORRUTINA PARA DESAPARECER
    // ==========================================
    IEnumerator Desaparecer()
    {
        // Esperar el tiempo de desaparición
        yield return new WaitForSeconds(tiempoDesaparicion);

        // Ocultar el activador
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.enabled = false;

        // Desactivar el collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        // Destruir después de un tiempo
        Destroy(gameObject, 0.5f);

        Debug.Log("🗑️ Activador primer_acto desaparecido");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }
    }
}