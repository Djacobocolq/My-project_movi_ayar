using UnityEngine;
using Spine.Unity;
using UnityEngine.InputSystem;

public class JugadorController : MonoBehaviour
{
    // ============================================
    // CONFIGURACIÓN
    // ============================================
    [Header("MOVIMIENTO")]
    public float velocidad = 5f;
    public float fuerzaSalto = 10f;
    public int vida = 3;

    [Header("ATAQUE")]
    public GameObject espadaTrigger;
    public float distanciaAtaque = 1.5f;
    public LayerMask capaEnemigo;

    [Header("SUELO")]
    public LayerMask capaSuelo;
    public float longitudRaycast = 0.2f;

    // ============================================
    // REFERENCIAS
    // ============================================
    private SkeletonAnimation skeletonAnimation;
    private Rigidbody2D rb;
    private float movimientoHorizontal = 0f;
    private bool enSuelo;
    private bool atacando;
    private bool recibiendoDanio;
    public bool muerto;

    // ============================================
    // INPUT SYSTEM
    // ============================================
    private Keyboard keyboard;

    // ============================================
    // NOMBRES DE ANIMACIONES
    // ============================================
    private string IDLE = "idle_side";
    private string WALK = "walk_side";
    private string JUMP = "jump_side";
    private string ATTACK = "attack_side";
    private string HIT = "damaged_side";
    private string DEATH = "death_side";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
        keyboard = Keyboard.current;

        if (espadaTrigger != null)
            espadaTrigger.SetActive(false);

        // Animación inicial
        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationName = IDLE;
            skeletonAnimation.loop = true;
        }

        Debug.Log("JugadorController: Inicializado correctamente");
    }

    void Update()
    {
        if (muerto) return;

        // ==========================================
        // MOVIMIENTO CON INPUT SYSTEM
        // ==========================================
        if (keyboard != null)
        {
            // Reiniciar movimiento
            movimientoHorizontal = 0f;

            // Leer teclas
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                movimientoHorizontal = -1f;
            }
            else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                movimientoHorizontal = 1f;
            }
        }

        // ==========================================
        // MOVER AL JUGADOR (SI NO ESTÁ ATACANDO O RECIBIENDO DAÑO)
        // ==========================================
        if (!atacando && !recibiendoDanio)
        {
            Mover();
        }

        // ==========================================
        // DETECTAR SUELO
        // ==========================================
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capaSuelo);
        enSuelo = hit.collider != null;

        // ==========================================
        // SALTO (Espacio)
        // ==========================================
        if (enSuelo && keyboard != null && keyboard.spaceKey.wasPressedThisFrame && !recibiendoDanio && !atacando)
        {
            rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
            if (skeletonAnimation != null)
            {
                skeletonAnimation.AnimationName = JUMP;
                skeletonAnimation.loop = false;
            }
        }

        // ==========================================
        // ATAQUE (E)
        // ==========================================
        if (keyboard != null && keyboard.eKey.wasPressedThisFrame && enSuelo && !atacando && !recibiendoDanio)
        {
            Atacar();
        }

        // ==========================================
        // RECOGER FLOR (R)
        // ==========================================
        if (keyboard != null && keyboard.rKey.wasPressedThisFrame && !atacando && !recibiendoDanio)
        {
            RecogerFlor();
        }

        // ==========================================
        // ACTUALIZAR ANIMACIONES
        // ==========================================
        ActualizarAnimaciones();
    }

    void Mover()
    {
        // Calcular velocidad
        float velocidadX = movimientoHorizontal * velocidad * Time.deltaTime;

        // Aplicar movimiento
        transform.position += new Vector3(velocidadX, 0, 0);

        // Voltear personaje
        if (movimientoHorizontal != 0 && skeletonAnimation != null)
        {
            skeletonAnimation.Skeleton.ScaleX = movimientoHorizontal > 0 ? 1 : -1;
        }
    }

    void ActualizarAnimaciones()
    {
        if (skeletonAnimation == null || atacando || recibiendoDanio || muerto) return;

        if (enSuelo)
        {
            if (Mathf.Abs(movimientoHorizontal) > 0.1f)
            {
                if (skeletonAnimation.AnimationName != WALK)
                {
                    skeletonAnimation.AnimationName = WALK;
                    skeletonAnimation.loop = true;
                }
            }
            else
            {
                if (skeletonAnimation.AnimationName != IDLE)
                {
                    skeletonAnimation.AnimationName = IDLE;
                    skeletonAnimation.loop = true;
                }
            }
        }
        else
        {
            if (skeletonAnimation.AnimationName != JUMP)
            {
                skeletonAnimation.AnimationName = JUMP;
                skeletonAnimation.loop = false;
            }
        }
    }

    // ==========================================
    // ATAQUE
    // ==========================================
    void Atacar()
    {
        atacando = true;
        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationName = ATTACK;
            skeletonAnimation.loop = false;
        }

        // Detectar enemigos
        float direccionX = transform.localScale.x > 0 ? 1 : -1;
        Vector2 origen = transform.position;
        Vector2 direccion = new Vector2(direccionX, 0);

        RaycastHit2D hit = Physics2D.Raycast(origen, direccion, distanciaAtaque, capaEnemigo);
        Debug.DrawRay(origen, direccion * distanciaAtaque, Color.yellow, 0.5f);

        if (hit.collider != null)
        {
            EnemigoBase enemigo = hit.collider.GetComponent<EnemigoBase>();
            if (enemigo != null)
            {
                Vector2 direccionDanio = new Vector2(hit.collider.transform.position.x, 0);
                enemigo.RecibeDanio(direccionDanio, 1);
                Debug.Log("¡Golpe al enemigo!");
            }
        }

        if (espadaTrigger != null)
            espadaTrigger.SetActive(true);

        Invoke("DesactivarAtaque", 0.6f);
    }

    void DesactivarAtaque()
    {
        atacando = false;
        if (espadaTrigger != null)
            espadaTrigger.SetActive(false);

        if (skeletonAnimation != null && !muerto)
        {
            skeletonAnimation.AnimationName = IDLE;
            skeletonAnimation.loop = true;
        }
    }

    // ==========================================
    // RECIBIR DAÑO
    // ==========================================
    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (recibiendoDanio || muerto) return;

        recibiendoDanio = true;
        vida -= cantDanio;
        Debug.Log("Jugador recibe daño. Vida: " + vida);

        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationName = HIT;
            skeletonAnimation.loop = false;
        }

        if (vida <= 0)
        {
            Morir();
        }
        else
        {
            Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.2f).normalized;
            rb.AddForce(rebote * 6f, ForceMode2D.Impulse);
            Invoke("DesactivarDanio", 0.3f);
        }
    }

    void DesactivarDanio()
    {
        recibiendoDanio = false;
        rb.linearVelocity = Vector2.zero;

        if (skeletonAnimation != null && !muerto)
        {
            skeletonAnimation.AnimationName = IDLE;
            skeletonAnimation.loop = true;
        }
    }

    // ==========================================
    // MUERTE
    // ==========================================
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
    }

    // ==========================================
    // MÉTODOS TÁCTILES (para botones)
    // ==========================================

    public void MoverIzquierda()
    {
        movimientoHorizontal = -1f;
        Debug.Log("MoverIzquierda (táctil)");
    }

    public void MoverDerecha()
    {
        movimientoHorizontal = 1f;
        Debug.Log("MoverDerecha (táctil)");
    }

    public void DetenerMovimiento()
    {
        movimientoHorizontal = 0f;
        Debug.Log("DetenerMovimiento (táctil)");
    }

    public void SaltarTouch()
    {
        if (enSuelo && !recibiendoDanio && !muerto)
        {
            rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
            if (skeletonAnimation != null)
            {
                skeletonAnimation.AnimationName = JUMP;
                skeletonAnimation.loop = false;
            }
            Debug.Log("SaltarTouch");
        }
    }

    public void AtacarTouch()
    {
        if (!atacando && enSuelo && !recibiendoDanio && !muerto)
        {
            Atacar();
            Debug.Log("AtacarTouch");
        }
    }

    public void RecogerTouch()
    {
        if (!atacando && !recibiendoDanio && !muerto)
        {
            RecogerFlor();
            Debug.Log("RecogerTouch");
        }
    }

    // ==========================================
    // RECOGER FLOR
    // ==========================================
    void RecogerFlor()
    {
        FlorRecolectable[] flores = FindObjectsByType<FlorRecolectable>(FindObjectsSortMode.None);
        foreach (FlorRecolectable flor in flores)
        {
            if (!flor.recolectada)
            {
                float distancia = Vector2.Distance(transform.position, flor.transform.position);
                if (distancia <= flor.distanciaInteraccion)
                {
                    flor.RecogerTouch();
                    Debug.Log("¡Flor recolectada!");
                    break;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);

        Gizmos.color = Color.yellow;
        float direccionX = transform.localScale.x > 0 ? 1 : -1;
        Vector3 finRaycast = transform.position + new Vector3(direccionX * distanciaAtaque, 0, 0);
        Gizmos.DrawLine(transform.position, finRaycast);
    }
}