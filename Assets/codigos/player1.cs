using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class player1 : MonoBehaviour
{
    public float velocidad = 5f;
    public int vida = 3;

    public float fuerzaSalto = 10f;
    public float fuerzaRebote = 6f;
    public float longitudRaycast = 0.1f;
    public LayerMask capaSuelo;

    [Header("ATAQUE")]
    public GameObject espadaTrigger;
    public float distanciaAtaque = 1.5f;
    public LayerMask capaEnemigo;

    private bool enSuelo;
    private bool recibiendoDanio;
    private bool atacando;
    public bool muerto;

    float scaleZ;
    private Rigidbody2D rb;

    public Animator animator;

    private Keyboard keyboard;
    private float movimientoHorizontal;

    void Start()
    {
        scaleZ = transform.localScale.z;
        rb = GetComponent<Rigidbody2D>();
        keyboard = Keyboard.current;

        if (espadaTrigger != null)
            espadaTrigger.SetActive(false);
    }

    void Update()
    {
        if (keyboard != null)
        {
            movimientoHorizontal = 0f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                movimientoHorizontal = -1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                movimientoHorizontal = 1f;

            if (!muerto)
            {
                if (!atacando)
                {
                    Movimiento();

                    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capaSuelo);
                    enSuelo = hit.collider != null;

                    if (enSuelo && keyboard.spaceKey.wasPressedThisFrame && !recibiendoDanio)
                    {
                        rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
                    }
                }

                if (keyboard.eKey.wasPressedThisFrame && !atacando && enSuelo && !recibiendoDanio)
                {
                    Atacar();
                }
            }
        }

        animator.SetBool("ensuelo", enSuelo);
        animator.SetBool("recibeDanio", recibiendoDanio);
        animator.SetBool("Atacando", atacando);
        animator.SetBool("muerto", muerto);
    }

    public void Movimiento()
    {
        float velocidadX = movimientoHorizontal * Time.deltaTime * velocidad;

        animator.SetFloat("movement", velocidadX * velocidad);

        if (velocidadX < 0)
        {
            transform.localScale = new Vector3(-scaleZ, 1, 1);
        }
        if (velocidadX > 0)
        {
            transform.localScale = new Vector3(scaleZ, 1, 1);
        }

        Vector3 posicion = transform.position;

        if (!recibiendoDanio)
            transform.position = new Vector3(velocidadX + posicion.x, posicion.y, posicion.z);
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (!recibiendoDanio && !muerto)
        {
            recibiendoDanio = true;

            if (cantDanio > 0)
            {
                vida -= cantDanio;
                Debug.Log("Jugador recibe daño. Vida: " + vida);
                if (vida <= 0)
                {
                    muerto = true;
                    Debug.Log("¡Jugador murió!");
                }
            }

            if (!muerto)
            {
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.2f).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
            }
        }
    }

    public void DesactivaDanio()
    {
        recibiendoDanio = false;
        rb.linearVelocity = Vector2.zero;
    }

    public void Atacar()
    {
        atacando = true;

        // Detectar enemigos con Raycast
        float direccionX = transform.localScale.x > 0 ? 1 : -1;
        Vector2 origen = transform.position;
        Vector2 direccion = new Vector2(direccionX, 0);

        RaycastHit2D hit = Physics2D.Raycast(origen, direccion, distanciaAtaque, capaEnemigo);
        Debug.DrawRay(origen, direccion * distanciaAtaque, Color.yellow, 0.5f);

        if (hit.collider != null)
        {
            EnemigoMecha enemigo = hit.collider.GetComponent<EnemigoMecha>();
            if (enemigo != null)
            {
                Vector2 direccionDanio = new Vector2(hit.collider.transform.position.x, 0);
                enemigo.RecibeDanio(direccionDanio, 1);
                Debug.Log("¡Golpe al enemigo!");
            }
        }

        if (espadaTrigger != null)
            espadaTrigger.SetActive(true);

        // ==========================================
        // AUMENTAR EL TIEMPO DEL ATAQUE
        // ==========================================
        Invoke("DesactivaAtaque", 0.6f); // ← Cambiado de 0.3f a 0.6f
    }

    public void Atacando()
    {
        atacando = true;
    }

    public void DesactivaAtaque()
    {
        atacando = false;

        if (espadaTrigger != null)
            espadaTrigger.SetActive(false);
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