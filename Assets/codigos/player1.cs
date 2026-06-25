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

    // ==========================================
    // CONTROL DE BOTONES
    // ==========================================
    private bool botonIzquierdaPresionado = false;
    private bool botonDerechaPresionado = false;

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
        // ==========================================
        // CONTROLES DE BOTONES (prioridad)
        // ==========================================
        if (botonIzquierdaPresionado)
        {
            movimientoHorizontal = -1f;
        }
        else if (botonDerechaPresionado)
        {
            movimientoHorizontal = 1f;
        }
        else
        {
            // ==========================================
            // CONTROLES DE TECLADO (PC)
            // ==========================================
            if (keyboard != null)
            {
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                    movimientoHorizontal = -1f;
                else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                    movimientoHorizontal = 1f;
                else
                    movimientoHorizontal = 0f;
            }
            else
            {
                movimientoHorizontal = 0f;
            }
        }

        if (!muerto)
        {
            if (!atacando)
            {
                Movimiento();

                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capaSuelo);
                enSuelo = hit.collider != null;

                if (enSuelo && keyboard != null && keyboard.spaceKey.wasPressedThisFrame && !recibiendoDanio)
                {
                    rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
                }
            }

            if (keyboard != null && keyboard.eKey.wasPressedThisFrame && !atacando && enSuelo && !recibiendoDanio)
            {
                Atacar();
            }

            // ==========================================
            // RECOGER FLOR CON TECLA R
            // ==========================================
            if (keyboard != null && keyboard.rKey.wasPressedThisFrame && !atacando && !recibiendoDanio && !muerto)
            {
                RecogerFlor();
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

        Invoke("DesactivaAtaque", 0.6f);
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

    // ==========================================
    // MÉTODO PARA RECOGER FLOR
    // ==========================================
    public void RecogerFlor()
    {
        // Buscar todas las flores
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
                    break; // Solo recoger una flor por vez
                }
            }
        }
    }

    // ==========================================
    // MÉTODOS PARA CONTROLES TÁCTILES (BOTONES)
    // ==========================================

    public void MoverIzquierda()
    {
        Debug.Log("MoverIzquierda llamado");
        botonIzquierdaPresionado = true;
        botonDerechaPresionado = false;
        movimientoHorizontal = -1f;
    }

    public void MoverDerecha()
    {
        Debug.Log("MoverDerecha llamado");
        botonDerechaPresionado = true;
        botonIzquierdaPresionado = false;
        movimientoHorizontal = 1f;
    }

    public void DetenerMovimiento()
    {
        Debug.Log("DetenerMovimiento llamado");
        botonIzquierdaPresionado = false;
        botonDerechaPresionado = false;
        movimientoHorizontal = 0f;
    }

    public void SaltarTouch()
    {
        Debug.Log("SaltarTouch llamado");
        if (enSuelo && !recibiendoDanio && !muerto)
        {
            rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
        }
    }

    public void AtacarTouch()
    {
        Debug.Log("AtacarTouch llamado");
        if (!atacando && enSuelo && !recibiendoDanio && !muerto)
        {
            Atacar();
        }
    }

    public void PausarTouch()
    {
        Debug.Log("PausarTouch llamado");
        pausar_juego pausa = FindFirstObjectByType<pausar_juego>();
        if (pausa != null)
        {
            if (pausa.juegoPausado)
                pausa.Reanudar();
            else
                pausa.Pausar();
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