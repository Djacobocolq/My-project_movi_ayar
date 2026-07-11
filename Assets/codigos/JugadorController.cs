using UnityEngine;
using Spine.Unity;
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
    public ArmaController arma;

    [Header("EMPUJE")]
    public float fuerzaEmpujeInicial = 10f;
    public float distanciaEmpujeInicial = 2f;
    public float fuerzaEmpujeBaston = 30f;
    public float distanciaEmpujeBaston = 5f;

    [Header("SUELO")]
    public LayerMask capaSuelo;
    public float longitudRaycast = 0.5f;

    [Header("ARMA")]
    public GameObject baston;

    // ============================================
    // REFERENCIAS PRIVADAS
    // ============================================
    public SkeletonAnimation skeletonAnimation;
    private Rigidbody2D rb;
    private float movimientoHorizontal = 0f;
    private bool enSuelo;
    private bool estabaEnSuelo;
    private bool atacando;
    private bool recibiendoDanio;
    public bool muerto;

    // ============================================
    // ESTADO DEL BASTÓN
    // ============================================
    private bool bastonActivo = false;
    private bool transicionEnCurso = false;
    private float tiempoTransicion = 0f;
    private float duracionTransicion = 3f;

    // ============================================
    // VALORES ACTUALES DE EMPUJE
    // ============================================
    private float fuerzaEmpujeActual;
    private float distanciaEmpujeActual;
    private LayerMask capaEnemigo;

    // ============================================
    // CONTROL DE ANIMACIONES POR POSICIÓN
    // ============================================
    private Vector3 posicionAnterior;
    private bool estabaEnMovimiento = false;

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
    // NOMBRES DE ANIMACIONES CON BASTÓN
    // ============================================
    private string STAFF_IDLE = "staff_idle_side";
    private string STAFF_WALK = "staff_walk_side";
    private string STAFF_ATTACK = "twohand_attack_side";
    private string STAFF_CAST = "staff_cast_side";

    // ============================================
    // INICIALIZACIÓN
    // ============================================
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
        keyboard = Keyboard.current;
        posicionAnterior = transform.position;

        // ==========================================
        // INICIALIZAR BASTÓN
        // ==========================================
        if (baston != null)
        {
            baston.SetActive(false);
            arma = baston.GetComponent<ArmaController>();
            if (arma != null);
        }

        // ==========================================
        // VALORES INICIALES DE EMPUJE
        // ==========================================
        fuerzaEmpujeActual = fuerzaEmpujeInicial;
        distanciaEmpujeActual = distanciaEmpujeInicial;
        capaEnemigo = LayerMask.GetMask("Enemy");
        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationName = IDLE;
            skeletonAnimation.loop = true;
        }

        Debug.Log("✅ JugadorController: Inicializado (empuje inicial: " + fuerzaEmpujeActual + ")");
    }

    void Update()
    {
        if (muerto) return;

        // ==========================================
        // TRANSICIÓN DEL BASTÓN
        // ==========================================
        if (transicionEnCurso)
        {
            tiempoTransicion += Time.unscaledDeltaTime;
            if (tiempoTransicion >= duracionTransicion)
            {
                FinalizarTransicion();
            }
            return;
        }

        // ==========================================
        // MOVIMIENTO CON TECLADO
        // ==========================================
        if (keyboard != null && !botonIzquierdaPresionado && !botonDerechaPresionado)
        {
            float movimientoAnterior = movimientoHorizontal;
            movimientoHorizontal = 0f;

            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                movimientoHorizontal = -1f;
            else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                movimientoHorizontal = 1f;

            // ==========================================
            // ACTUALIZAR DIRECCIÓN DEL ARMA CUANDO CAMBIA
            // ==========================================
            if (movimientoHorizontal != 0 && movimientoHorizontal != movimientoAnterior)
            {
                if (arma != null && bastonActivo)
                {
                    arma.CambiarDireccion(movimientoHorizontal > 0 ? 1f : -1f);
                }
            }
        }

        // ==========================================
        // MOVER AL JUGADOR (SIEMPRE QUE NO ESTÉ ATACANDO O RECIBIENDO DAÑO)
        // ==========================================
        if (!atacando && !recibiendoDanio)
        {
            Mover();
        }

        // ==========================================
        // DETECTAR SUELO
        // ==========================================
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
                    skeletonAnimation.AnimationName = bastonActivo ? STAFF_WALK : WALK;
                    skeletonAnimation.loop = true;
                }
                else
                {
                    skeletonAnimation.AnimationName = bastonActivo ? STAFF_IDLE : IDLE;
                    skeletonAnimation.loop = true;
                }
            }
        }

        // ==========================================
        // SALTO
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
        // ATAQUE
        // ==========================================
        if (keyboard != null && keyboard.eKey.wasPressedThisFrame && enSuelo && !atacando && !recibiendoDanio)
        {
            Atacar();
        }

        // ==========================================
        // RECOGER FLOR
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
        float velocidadX = movimientoHorizontal * velocidad * Time.deltaTime;
        transform.position += new Vector3(velocidadX, 0, 0);

        if (movimientoHorizontal != 0 && skeletonAnimation != null)
        {
            skeletonAnimation.Skeleton.ScaleX = movimientoHorizontal > 0 ? 1 : -1;
        }

        // ==========================================
        // ACTUALIZAR DIRECCIÓN DEL ARMA (FUERA DEL IF)
        // ==========================================
        if (arma != null && bastonActivo)
        {
            if (movimientoHorizontal > 0.1f)
            {
                arma.CambiarDireccion(1f);
            }
            else if (movimientoHorizontal < -0.1f)
            {
                arma.CambiarDireccion(-1f);
            }
        }
    }

    // ============================================
    // ACTUALIZAR ANIMACIONES (Basado en posición)
    // ============================================
    void ActualizarAnimaciones()
    {
        if (skeletonAnimation == null || atacando || recibiendoDanio || muerto || transicionEnCurso) return;

        Vector3 posicionActual = transform.position;
        float distanciaRecorrida = Vector3.Distance(posicionAnterior, posicionActual);
        bool enMovimiento = distanciaRecorrida > 0.01f;

        posicionAnterior = posicionActual;

        if (enSuelo)
        {
            if (enMovimiento)
            {
                string anim = bastonActivo ? STAFF_WALK : WALK;
                if (skeletonAnimation.AnimationName != anim)
                {
                    skeletonAnimation.AnimationName = anim;
                    skeletonAnimation.loop = true;
                }
            }
            else
            {
                string anim = bastonActivo ? STAFF_IDLE : IDLE;
                if (skeletonAnimation.AnimationName != anim)
                {
                    skeletonAnimation.AnimationName = anim;
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

        estabaEnMovimiento = enMovimiento;
    }

    // ============================================
    // TRANSICIÓN DEL BASTÓN
    // ============================================
    public void IniciarTransicionBaston(float duracion, GameObject objetoBaston)
    {
        if (transicionEnCurso) return;

        baston = objetoBaston;
        duracionTransicion = duracion;
        transicionEnCurso = true;
        tiempoTransicion = 0f;

        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationName = STAFF_CAST;
            skeletonAnimation.loop = false;
        }

        Debug.Log("🔄 Transición iniciada: " + duracion + " segundos");
    }

    void FinalizarTransicion()
    {
        transicionEnCurso = false;
        bastonActivo = true; // ← Asegurar que esté en true

        if (baston != null)
        {
            baston.SetActive(true);
            if (arma != null)
            {
                arma.Activar();
                // Forzar dirección inicial (derecha)
                arma.CambiarDireccion(1f);
            }
        }

        fuerzaEmpujeActual = fuerzaEmpujeBaston;
        distanciaEmpujeActual = distanciaEmpujeBaston;

        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationName = STAFF_IDLE;
            skeletonAnimation.loop = true;
        }

        Debug.Log("✅ Bastón activado! Fuerza: " + fuerzaEmpujeActual + " | Distancia: " + distanciaEmpujeActual);
    }

    // ============================================
    // ATAQUE = EMPUJE
    // ============================================
    void Atacar()
    {
        if (atacando) return;

        atacando = true;

        // ==========================================
        // ANIMACIÓN DE ATAQUE
        // ==========================================
        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationName = bastonActivo ? STAFF_ATTACK : ATTACK;
            skeletonAnimation.loop = false;
        }

        // ==========================================
        // ACTIVAR ARMA (BASTÓN)
        // ==========================================
        if (bastonActivo && arma != null)
        {
            arma.ActivarAtaque();
        }

        // ==========================================
        // EMPUJAR ENEMIGOS
        // ==========================================
        EmpujarEnemigos();

        Invoke("DesactivarAtaque", 0.6f);
    }

    // ============================================
    // EMPUJAR ENEMIGOS
    // ============================================
    void EmpujarEnemigos()
    {
        // Detectar enemigos en el área
        Collider2D[] enemigos = Physics2D.OverlapCircleAll(transform.position, distanciaEmpujeActual, capaEnemigo);

        Debug.Log("🔍 Enemigos detectados: " + enemigos.Length + " | Fuerza: " + fuerzaEmpujeActual);

        foreach (Collider2D col in enemigos)
        {
            EnemigoBase enemigo = col.GetComponent<EnemigoBase>();
            if (enemigo != null)
            {
                // Dirección desde el jugador hacia el enemigo
                Vector2 direccion = (col.transform.position - transform.position).normalized;
                Vector2 direccionEmpuje = new Vector2(direccion.x, 0.3f).normalized;

                enemigo.Empujar(direccionEmpuje, fuerzaEmpujeActual);
                Debug.Log("👊 Enemigo empujado! Fuerza: " + fuerzaEmpujeActual);
            }
        }
    }

    void DesactivarAtaque()
    {
        atacando = false;

        if (skeletonAnimation != null && !muerto)
        {
            string anim = bastonActivo ? STAFF_IDLE : IDLE;
            skeletonAnimation.AnimationName = anim;
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
            // ==========================================
            // REBOTE EN LA DIRECCIÓN DEL GOLPE CON FUERZA 10
            // ==========================================
            Vector2 rebote = direccion * 10f;
            rb.AddForce(rebote, ForceMode2D.Impulse);
            Debug.Log("💨 Jugador rebota! Dirección: " + rebote);
            Invoke("DesactivarDanio", 0.3f);
        }
    }

    void DesactivarDanio()
    {
        recibiendoDanio = false;
        rb.linearVelocity = Vector2.zero;

        if (skeletonAnimation != null && !muerto)
        {
            string anim = bastonActivo ? STAFF_IDLE : IDLE;
            skeletonAnimation.AnimationName = anim;
            skeletonAnimation.loop = true;
        }
    }

    // ============================================
    // MUERTE
    // ============================================
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

    // ============================================
    // REVIVIR
    // ============================================
    public void Revivir(int vidaNueva)
    {
        muerto = false;
        vida = vidaNueva;
        recibiendoDanio = false;
        atacando = false;
        movimientoHorizontal = 0f;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = true;

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = Vector2.zero;
        }

        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationName = bastonActivo ? STAFF_IDLE : IDLE;
            skeletonAnimation.loop = true;
        }

        Debug.Log("❤️ Jugador revivido con vida: " + vidaNueva);
    }

    // ============================================
    // MÉTODOS TÁCTILES (para botones)
    // ============================================

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

    // ============================================
    // RECOGER FLOR
    // ============================================
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

    // ============================================
    // VISUALIZACIÓN EN EL EDITOR
    // ============================================
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * (longitudRaycast + 0.2f));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, distanciaEmpujeActual);
    }
}