using System.Collections; // ← AGREGADO
using UnityEngine;
using Spine.Unity;

public class EnemigoBase : MonoBehaviour
{
    // ============================================
    // CONFIGURACIÓN EN EL INSPECTOR
    // ============================================
    [Header("MOVIMIENTO")]
    public float velocidad = 2f;

    [Header("DETECCIÓN")]
    public float distanciaDeteccion = 5f;
    public float distanciaAtaque = 1.5f;

    [Header("VIDA")]
    public int vidaMaxima = 3;
    public int dañoAtaque = 1;

    [Header("REBOTE")]
    public float fuerzaRebote = 6f;

    [Header("SUELO")]
    public LayerMask capaSuelo;
    public float longitudRaycast = 0.2f;

    // ============================================
    // REFERENCIAS INTERNAS
    // ============================================
    private SkeletonAnimation skeletonAnimation;
    private Rigidbody2D rb;
    private Transform jugador;
    private player1 scriptJugador;

    private int vidaActual;
    private bool enSuelo;
    private bool atacando;
    private bool recibiendoDanio;
    private bool muerto;
    private bool jugadorMuerto = false;
    private float direccion = 1;

    // ============================================
    // NOMBRES DE ANIMACIONES
    // ============================================
    private string IDLE = "idle_side";
    private string WALK = "walk_side";
    private string ATTACK = "attack_side";
    private string HIT = "damaged_side";
    private string DEATH = "dead_side";
    private string DANCE = "dance_side";

    // ============================================
    // INICIALIZACIÓN
    // ============================================
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            jugador = player.transform;
            scriptJugador = player.GetComponent<player1>();
        }

        vidaActual = vidaMaxima;

        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationName = IDLE;
            skeletonAnimation.loop = true;
        }

        InvokeRepeating("ComportamientoIA", 0f, 0.5f);
    }

    // ============================================
    // ACTUALIZACIÓN
    // ============================================
    void Update()
    {
        if (muerto || skeletonAnimation == null) return;

        // Detectar estado del jugador
        if (scriptJugador != null)
        {
            if (scriptJugador.muerto && !jugadorMuerto)
            {
                jugadorMuerto = true;
                CelebrarVictoria();
            }

            if (!scriptJugador.muerto && jugadorMuerto)
            {
                jugadorMuerto = false;
                DejarDeBailar();
            }
        }

        // Forzar que mire al jugador
        if (jugador != null && !jugadorMuerto && !muerto)
        {
            if (jugador.position.x < transform.position.x)
                skeletonAnimation.Skeleton.ScaleX = -1;
            else
                skeletonAnimation.Skeleton.ScaleX = 1;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capaSuelo);
        enSuelo = hit.collider != null;

        ActualizarAnimaciones();
    }

    // ============================================
    // BAILAR
    // ============================================
    void CelebrarVictoria()
    {
        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationName = DANCE;
            skeletonAnimation.loop = true;
            Debug.Log("¡El enemigo está bailando!");
        }
        rb.linearVelocity = Vector2.zero;
    }

    void DejarDeBailar()
    {
        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationName = IDLE;
            skeletonAnimation.loop = true;
            Debug.Log("El enemigo dejó de bailar.");
        }
    }

    // ============================================
    // INTELIGENCIA ARTIFICIAL
    // ============================================
    void ComportamientoIA()
    {
        if (muerto || recibiendoDanio || atacando) return;
        if (jugador == null || jugadorMuerto) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia <= distanciaAtaque)
        {
            Atacar();
        }
        else if (distancia <= distanciaDeteccion)
        {
            MoverHaciaJugador();
        }
        else
        {
            if (skeletonAnimation != null)
            {
                skeletonAnimation.AnimationName = IDLE;
                skeletonAnimation.loop = true;
            }
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    // ============================================
    // MOVIMIENTO
    // ============================================
    void MoverHaciaJugador()
    {
        if (jugador.position.x < transform.position.x)
            direccion = -1;
        else
            direccion = 1;

        if (!recibiendoDanio && !atacando)
            rb.linearVelocity = new Vector2(direccion * velocidad, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (skeletonAnimation != null && !atacando)
        {
            skeletonAnimation.AnimationName = WALK;
            skeletonAnimation.loop = true;
        }
    }

    // ============================================
    // ANIMACIONES
    // ============================================
    void ActualizarAnimaciones()
    {
        if (jugadorMuerto) return;
        if (atacando || recibiendoDanio || muerto) return;
        if (skeletonAnimation == null) return;

        if (enSuelo && Mathf.Abs(rb.linearVelocity.x) < 0.1f)
        {
            if (skeletonAnimation.AnimationName != IDLE)
            {
                skeletonAnimation.AnimationName = IDLE;
                skeletonAnimation.loop = true;
            }
        }
    }

    // ============================================
    // ATAQUE
    // ============================================
    void Atacar()
    {
        atacando = true;

        // Detener movimiento durante el ataque
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationName = ATTACK;
            skeletonAnimation.loop = false;
        }

        // Dañar al jugador
        if (jugador != null && scriptJugador != null && !jugadorMuerto)
        {
            float distancia = Vector2.Distance(transform.position, jugador.position);
            if (distancia <= distanciaAtaque + 0.5f)
            {
                Vector2 direccionAtaque = (jugador.position - transform.position).normalized;
                scriptJugador.RecibeDanio(direccionAtaque, dañoAtaque);
                Debug.Log("¡Enemigo atacó al jugador! Daño: " + dañoAtaque);
            }
        }

        Invoke("DesactivarAtaque", 0.5f);
    }

    void DesactivarAtaque()
    {
        atacando = false;
        if (skeletonAnimation != null && !jugadorMuerto)
        {
            skeletonAnimation.AnimationName = IDLE;
            skeletonAnimation.loop = true;
        }
    }

    // ============================================
    // RECIBIR DAÑO
    // ============================================
    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (recibiendoDanio || muerto) return;

        recibiendoDanio = true;
        vidaActual -= cantDanio;
        Debug.Log("Enemigo recibe daño. Vida: " + vidaActual);

        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationName = HIT;
            skeletonAnimation.loop = false;
        }

        Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.2f).normalized;
        rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);

        if (vidaActual <= 0)
            Morir();
        else
            StartCoroutine(DesactivarDanio());
    }

    IEnumerator DesactivarDanio()
    {
        yield return new WaitForSeconds(0.3f);
        recibiendoDanio = false;
        rb.linearVelocity = Vector2.zero;
    }

    // ============================================
    // MUERTE
    // ============================================
    void Morir()
    {
        muerto = true;
        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationName = DEATH;
            skeletonAnimation.loop = false;
        }

        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 2f);
    }

    // ============================================
    // TRIGGER CON LA ESPADA
    // ============================================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Espada"))
        {
            Vector2 direccionDanio = new Vector2(collision.gameObject.transform.position.x, 0);
            RecibeDanio(direccionDanio, 1);
            Debug.Log("¡Enemigo golpeado por la espada!");
        }
    }

    // ============================================
    // COLISIÓN CON EL JUGADOR
    // ============================================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player1 jugadorScript = collision.gameObject.GetComponent<player1>();
            if (jugadorScript != null && !jugadorScript.muerto)
            {
                Rigidbody2D rbJugador = collision.gameObject.GetComponent<Rigidbody2D>();
                if (rbJugador != null && rbJugador.linearVelocity.y < -0.5f)
                {
                    Vector2 direccionRebote = Vector2.up;
                    jugadorScript.RecibeDanio(direccionRebote, 0);
                    Debug.Log("¡El jugador saltó encima del enemigo!");
                }
            }
        }
    }

    // ============================================
    // VISUALIZACIÓN
    // ============================================
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }
}