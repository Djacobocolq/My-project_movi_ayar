using System.Collections;
using UnityEngine;
using Spine.Unity;

public class EnemigoBase : MonoBehaviour
{
    [Header("MOVIMIENTO")]
    public float velocidadBase = 2f;
    public float velocidadActual;

    [Header("DETECCIÓN")]
    public float distanciaDeteccion = 5f;
    public float distanciaAtaque = 1.5f;

    [Header("VIDA")]
    public int vidaMaxima = 3;
    public int dañoBase = 1;
    public int dañoActual;

    [Header("ATAQUE")]
    public float cooldownAtaque = 2f;
    public float fuerzaEmpujeJugador = 10f; // ← NUEVO

    [Header("REBOTE")]
    public float fuerzaRebote = 10f;
    public float fuerzaReboteJugador = 10f;

    [Header("BUFF")]
    public float intervaloBuff = 4f;
    public float probabilidadBuff = 0.4f;
    public int aumentoVelocidad = 4;
    public int aumentoDaño = 1;
    public bool buffActivo = false;

    [Header("SUELO")]
    public LayerMask capaSuelo;
    public float longitudRaycast = 0.2f;

    private SkeletonAnimation skeletonAnimation;
    private Rigidbody2D rb;
    private Transform jugador;
    private JugadorController scriptJugador;

    private int vidaActual;
    private bool enSuelo;
    private bool atacando;
    private bool recibiendoDanio;
    private bool muerto;
    private bool jugadorMuerto = false;
    private float direccion = 1;
    private float tiempoUltimoAtaque = 0f;

    // ==========================================
    // ESTADO DESACTIVADO (QUIETO)
    // ==========================================
    private bool enemigoDesactivado = false;

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
            scriptJugador = player.GetComponent<JugadorController>();
            Debug.Log("Enemigo: Jugador encontrado: " + player.name);
        }
        else
        {
            Debug.LogError("Enemigo: No se encontró al jugador con Tag 'Player'");
        }

        vidaActual = vidaMaxima;
        velocidadActual = velocidadBase;
        dañoActual = dañoBase;
        buffActivo = false;

        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationName = IDLE;
            skeletonAnimation.loop = true;
        }

        StartCoroutine(CicloBuff());
    }

    // ============================================
    // ACTUALIZACIÓN CADA FRAME
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

        // Voltear sprite
        if (jugador != null && !jugadorMuerto && !muerto)
        {
            if (jugador.position.x < transform.position.x)
                skeletonAnimation.Skeleton.ScaleX = -1;
            else
                skeletonAnimation.Skeleton.ScaleX = 1;
        }

        // Detectar suelo
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capaSuelo);
        enSuelo = hit.collider != null;

        // ==========================================
        // COMPORTAMIENTO IA
        // ==========================================
        if (!muerto && !recibiendoDanio && !atacando && jugador != null && !jugadorMuerto && !enemigoDesactivado)
        {
            float distancia = Vector2.Distance(transform.position, jugador.position);

            if (distancia <= distanciaAtaque)
            {
                if (Time.time >= tiempoUltimoAtaque + cooldownAtaque)
                {
                    Atacar();
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
        else if (!muerto && (recibiendoDanio || atacando || enemigoDesactivado))
        {
            if (atacando || enemigoDesactivado)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }

        ActualizarAnimaciones();
    }

    // ============================================
    // SISTEMA DE BUFF CON COROUTINE
    // ============================================
    IEnumerator CicloBuff()
    {
        while (!muerto)
        {
            yield return new WaitForSeconds(intervaloBuff);

            if (muerto || jugadorMuerto) continue;

            float random = Random.value;

            if (random <= probabilidadBuff)
            {
                if (!buffActivo)
                {
                    velocidadActual = velocidadBase + aumentoVelocidad;
                    dañoActual = dañoBase + aumentoDaño;
                    buffActivo = true;
                    Debug.Log("🔥 ¡BUFF ACTIVADO! Velocidad: " + velocidadActual + " | Daño: " + dañoActual);
                }
                else
                {
                    Debug.Log("⚡ Buff ya activo, manteniendo...");
                }
            }
            else
            {
                velocidadActual = velocidadBase;
                dañoActual = dañoBase;
                buffActivo = false;
                Debug.Log("❌ Buff falló - Valores reiniciados. Velocidad: " + velocidadActual + " | Daño: " + dañoActual);
            }
        }
    }

    public void ReiniciarBuff()
    {
        velocidadActual = velocidadBase;
        dañoActual = dañoBase;
        buffActivo = false;
        Debug.Log("🔄 Buff reiniciado manualmente");
    }

    // ============================================
    // CELEBRAR VICTORIA (BAILAR)
    // ============================================
    void CelebrarVictoria()
    {
        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationName = DANCE;
            skeletonAnimation.loop = true;
            Debug.Log("🎉 ¡El enemigo está bailando!");
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
    // MOVIMIENTO
    // ============================================
    void MoverHaciaJugador()
    {
        if (jugador.position.x < transform.position.x)
            direccion = -1;
        else
            direccion = 1;

        if (!recibiendoDanio && !atacando)
            rb.linearVelocity = new Vector2(direccion * velocidadActual, rb.linearVelocity.y);
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
        if (atacando || muerto) return;
        if (skeletonAnimation == null) return;

        if (recibiendoDanio) return;

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
    // ATAQUE (CORREGIDO - DIRECCIÓN CORRECTA)
    // ============================================
    void Atacar()
    {
        if (atacando) return;

        atacando = true;
        tiempoUltimoAtaque = Time.time;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationName = ATTACK;
            skeletonAnimation.loop = false;
        }

        if (jugador != null && scriptJugador != null && !jugadorMuerto)
        {
            float distancia = Vector2.Distance(transform.position, jugador.position);
            if (distancia <= distanciaAtaque + 0.5f)
            {
                // ==========================================
                // DIRECCIÓN DESDE EL ENEMIGO HACIA EL JUGADOR
                // ==========================================
                Vector2 direccionAtaque = (jugador.position - transform.position).normalized;
                scriptJugador.RecibeDanio(direccionAtaque, dañoActual);
                Debug.Log("⚔️ Enemigo atacó al jugador! Dirección: " + direccionAtaque + " | Daño: " + dañoActual);
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
        Debug.Log("💥 Enemigo recibe daño. Vida: " + vidaActual);

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
    }

    // ============================================
    // EMPUJAR
    // ============================================
    public void Empujar(Vector2 direccion, float fuerza)
    {
        if (muerto) return;

        rb.AddForce(direccion * fuerza, ForceMode2D.Impulse);
        Debug.Log("💨 Enemigo empujado! Fuerza: " + fuerza);

        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationName = HIT;
            skeletonAnimation.loop = false;
        }

        recibiendoDanio = true;
        CancelInvoke("DesactivarDanioEmpuje");
        Invoke("DesactivarDanioEmpuje", 0.2f);
    }

    void DesactivarDanioEmpuje()
    {
        recibiendoDanio = false;

        if (skeletonAnimation != null && !muerto)
        {
            skeletonAnimation.AnimationName = IDLE;
            skeletonAnimation.loop = true;
        }
    }

    // ============================================
    // DESACTIVAR ENEMIGO (QUIETO) - PARA PRIMER ACTO
    // ============================================
    public void DesactivarEnemigo(float tiempo)
    {
        if (enemigoDesactivado) return;

        enemigoDesactivado = true;
        rb.linearVelocity = Vector2.zero;

        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationName = IDLE;
            skeletonAnimation.loop = true;
        }

        Debug.Log("🛑 Enemigo desactivado por " + tiempo + " segundos");

        Invoke("ReactivarEnemigo", tiempo);
    }

    void ReactivarEnemigo()
    {
        enemigoDesactivado = false;
        Debug.Log("✅ Enemigo reactivado");
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
    // COLISIÓN CON EL JUGADOR (REBOTE SIN DAÑO)
    // ============================================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            JugadorController jugadorScript = collision.gameObject.GetComponent<JugadorController>();
            if (jugadorScript != null && !jugadorScript.muerto)
            {
                Rigidbody2D rbJugador = collision.gameObject.GetComponent<Rigidbody2D>();
                if (rbJugador != null && rbJugador.linearVelocity.y < -0.5f)
                {
                    Vector2 direccionRebote = Vector2.up;
                    jugadorScript.RecibeDanio(direccionRebote, 0);
                    rbJugador.linearVelocity = new Vector2(rbJugador.linearVelocity.x, 0);
                    rbJugador.AddForce(Vector2.up * fuerzaReboteJugador, ForceMode2D.Impulse);
                    rb.AddForce(Vector2.down * 2f, ForceMode2D.Impulse);
                    Debug.Log("⬆️ ¡El jugador saltó encima del enemigo y rebotó!");
                }
            }
        }
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
            Debug.Log("🗡️ Enemigo golpeado por la espada!");
        }
    }

    // ============================================
    // VISUALIZACIÓN EN EL EDITOR
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