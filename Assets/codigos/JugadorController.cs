using Spine.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class JugadorController : MonoBehaviour
{
    // ============================================
    // CONFIGURACIÓN EN EL INSPECTOR
    // ============================================
    [Header("MOVIMIENTO")]
    public float velocidad = 5f;
    public float fuerzaSalto = 10f;
    public int vida = 3;
    public float fuerzaEmpuje = 15f;

    [Header("ATAQUE")]
    public GameObject espadaTrigger;
    public float distanciaAtaque = 1.8f;
    public LayerMask capaEnemigo;

    [Header("SUELO")]
    public LayerMask capaSuelo;
    public float longitudRaycast = 0.5f;

    // ============================================
    // REFERENCIAS PRIVADAS
    // ============================================
    public SkeletonAnimation skeletonAnimation; // ← AHORA PÚBLICA
    private Rigidbody2D rb;
    private float movimientoHorizontal = 0f;
    private bool enSuelo;
    private bool estabaEnSuelo;
    private bool atacando;
    private bool recibiendoDanio;
    public bool muerto;

    private Keyboard keyboard;
    private bool botonIzquierdaPresionado = false;
    private bool botonDerechaPresionado = false;

    // ============================================
    // NOMBRES DE ANIMACIONES
    // ============================================
    private string IDLE = "basket_idle_side";
    private string WALK = "[down]walk_side";
    private string JUMP = "twohand_damaged_side";
    private string ATTACK = "sword_attack_stab_side";
    private string HIT = "[emo]damaged_side";
    private string DEATH = "dead4_side";

    // ============================================
    // INICIALIZACIÓN
    // ============================================
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
        keyboard = Keyboard.current;

        if (espadaTrigger != null)
            espadaTrigger.SetActive(false);

        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationName = IDLE;
            skeletonAnimation.loop = true;
        }

        Debug.Log("✅ JugadorController: Inicializado correctamente");
    }

    void Update()
    {
        if (muerto) return;

        // Movimiento con teclado
        if (keyboard != null && !botonIzquierdaPresionado && !botonDerechaPresionado)
        {
            movimientoHorizontal = 0f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                movimientoHorizontal = -1f;
            else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                movimientoHorizontal = 1f;
        }

        if (!atacando && !recibiendoDanio)
        {
            Mover();
        }

        // Detectar suelo
        estabaEnSuelo = enSuelo;
        float raycastLength = longitudRaycast + 0.2f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastLength, capaSuelo);
        Debug.DrawRay(transform.position, Vector2.down * raycastLength, Color.green);
        enSuelo = hit.collider != null;

        if (!enSuelo)
        {
            Vector2 feetPosition = new Vector2(transform.position.x, transform.position.y - 0.3f);
            RaycastHit2D hitFeet = Physics2D.Raycast(feetPosition, Vector2.down, 0.3f, capaSuelo);
            enSuelo = hitFeet.collider != null;
            Debug.DrawRay(feetPosition, Vector2.down * 0.3f, Color.yellow);
        }

        if (enSuelo && !estabaEnSuelo)
        {
            if (skeletonAnimation != null && !atacando && !recibiendoDanio)
            {
                if (Mathf.Abs(movimientoHorizontal) > 0.1f)
                {
                    skeletonAnimation.AnimationName = WALK;
                    skeletonAnimation.loop = true;
                }
                else
                {
                    skeletonAnimation.AnimationName = IDLE;
                    skeletonAnimation.loop = true;
                }
            }
        }

        // Salto
        if (enSuelo && keyboard != null && keyboard.spaceKey.wasPressedThisFrame && !recibiendoDanio && !atacando)
        {
            rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
            if (skeletonAnimation != null)
            {
                skeletonAnimation.AnimationName = JUMP;
                skeletonAnimation.loop = false;
            }
        }

        // Ataque
        if (keyboard != null && keyboard.eKey.wasPressedThisFrame && enSuelo && !atacando && !recibiendoDanio)
        {
            Atacar();
        }

        // Recoger flor
        if (keyboard != null && keyboard.rKey.wasPressedThisFrame && !atacando && !recibiendoDanio)
        {
            RecogerFlor();
        }

        ActualizarAnimaciones();
    }

    void Mover()
    {
        float velocidadX = movimientoHorizontal * velocidad * Time.deltaTime;
        transform.position += new Vector3(velocidadX, 0, 0);

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
    // ATAQUE CON EMPUJE
    // ==========================================
    void Atacar()
    {
        if (atacando) return;

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
        Debug.DrawRay(origen, direccion * distanciaAtaque, Color.blue, 1f);

        if (hit.collider != null)
        {
            EnemigoBase enemigo = hit.collider.GetComponent<EnemigoBase>();
            if (enemigo != null)
            {
                Vector2 direccionEmpuje = new Vector2(direccionX, 0.3f).normalized;
                enemigo.Empujar(direccionEmpuje, fuerzaEmpuje);
                Debug.Log("👊 ¡Enemigo empujado!");
            }
        }

        if (espadaTrigger != null)
            espadaTrigger.SetActive(true);

        Invoke("DesactivarAtaque", 0.5f);
    }

    void DesactivarAtaque()
    {
        atacando = false;
        if (espadaTrigger != null)
            espadaTrigger.SetActive(false);

        if (skeletonAnimation != null && !muerto)
        {
            if (enSuelo)
            {
                if (Mathf.Abs(movimientoHorizontal) > 0.1f)
                {
                    skeletonAnimation.AnimationName = WALK;
                    skeletonAnimation.loop = true;
                }
                else
                {
                    skeletonAnimation.AnimationName = IDLE;
                    skeletonAnimation.loop = true;
                }
            }
            else
            {
                skeletonAnimation.AnimationName = JUMP;
                skeletonAnimation.loop = false;
            }
        }
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (recibiendoDanio || muerto) return;

        recibiendoDanio = true;
        vida -= cantDanio;
        Debug.Log("💥 Jugador recibe daño. Vida: " + vida);

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
            if (enSuelo)
            {
                if (Mathf.Abs(movimientoHorizontal) > 0.1f)
                {
                    skeletonAnimation.AnimationName = WALK;
                    skeletonAnimation.loop = true;
                }
                else
                {
                    skeletonAnimation.AnimationName = IDLE;
                    skeletonAnimation.loop = true;
                }
            }
            else
            {
                skeletonAnimation.AnimationName = JUMP;
                skeletonAnimation.loop = false;
            }
        }
    }

    void Morir()
    {
        muerto = true;
        Debug.Log("💀 Jugador murió");

        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationName = DEATH;
            skeletonAnimation.loop = false;
        }

        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;

        // Activar Game Over después de 2 segundos
        Invoke("ActivarGameOver", 2f);
    }

    void ActivarGameOver()
    {
        GameOverManager gameOver = FindFirstObjectByType<GameOverManager>();
        if (gameOver != null)
        {
            gameOver.MostrarGameOver();
        }
        else
        {
            Debug.LogError("❌ No se encontró GameOverManager");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // ==========================================
    // MÉTODO PARA REVIVIR (LLAMADO DESDE POSICIONARJUGADOR)
    // ==========================================
    public void Revivir(int vidaNueva)
    {
        // Resetear estado
        muerto = false;
        vida = vidaNueva;
        recibiendoDanio = false;
        atacando = false;
        movimientoHorizontal = 0f;

        // Reactivar collider
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = true;

        // Reactivar Rigidbody
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = Vector2.zero;
        }

        // Resetear animación
        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationName = IDLE;
            skeletonAnimation.loop = true;
        }

        Debug.Log("❤️ Jugador revivido con vida: " + vidaNueva);
    }

    // ==========================================
    // MÉTODOS TÁCTILES
    // ==========================================

    public void MoverIzquierda()
    {
        botonIzquierdaPresionado = true;
        botonDerechaPresionado = false;
        movimientoHorizontal = -1f;
    }

    public void MoverDerecha()
    {
        botonDerechaPresionado = true;
        botonIzquierdaPresionado = false;
        movimientoHorizontal = 1f;
    }

    public void DetenerMovimiento()
    {
        botonIzquierdaPresionado = false;
        botonDerechaPresionado = false;
        movimientoHorizontal = 0f;
    }

    public void SaltarTouch()
    {
        if (enSuelo && !recibiendoDanio && !muerto && !atacando)
        {
            rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
            if (skeletonAnimation != null)
            {
                skeletonAnimation.AnimationName = JUMP;
                skeletonAnimation.loop = false;
            }
        }
    }

    public void AtacarTouch()
    {
        if (!atacando && enSuelo && !recibiendoDanio && !muerto)
        {
            Atacar();
        }
    }

    public void RecogerTouch()
    {
        if (!atacando && !recibiendoDanio && !muerto)
        {
            RecogerFlor();
        }
    }

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
                    Debug.Log("🌺 ¡Flor recolectada!");
                    break;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * (longitudRaycast + 0.2f));

        Gizmos.color = Color.blue;
        float direccionX = transform.localScale.x > 0 ? 1 : -1;
        Vector3 finRaycast = transform.position + new Vector3(direccionX * distanciaAtaque, 0, 0);
        Gizmos.DrawLine(transform.position, finRaycast);
    }
}