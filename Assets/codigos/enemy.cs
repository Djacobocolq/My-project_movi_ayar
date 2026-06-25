using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 5.0f;
    public float speed = 2.0f;
    public float fuerzaRebote = 6f;
    public int vida = 3;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool enMovimiento;
    private bool muerto;
    private bool recibiendoDanio;
    private bool playerVivo;

    private Animator animator;
    void Start()
    {
        playerVivo = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Buscar al jugador si se pierde la referencia
        if (player == null)
        {
            BuscarJugador();
        }

        if (playerVivo && !muerto && player != null)
        {
            Movimiento();
        }

        animator.SetBool("enMovimiento", enMovimiento);
        animator.SetBool("muerto", muerto);
    }
    void BuscarJugador()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            Debug.Log("Enemy: Jugador encontrado");
        }
    }
    private void Movimiento()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            if (direction.x < 0)
            {
                transform.localScale = new Vector3(-12, 12, 12);
            }
            if (direction.x > 0)
            {
                transform.localScale = new Vector3(12, 12, 12);
            }

            movement = new Vector2(direction.x, 0);

            enMovimiento = true;
        }
        else
        {
            movement = Vector2.zero;
            enMovimiento = false;
        }
        if (!recibiendoDanio)
            rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direccionDanio = new Vector2(transform.position.x, 0);
            player1 playerScript = collision.gameObject.GetComponent<player1>();

            playerScript.RecibeDanio(direccionDanio, 1);
            playerVivo = !playerScript.muerto;
            if (!playerVivo)
            {
                enMovimiento = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Espada"))
        {
            Vector2 direccionDanio = new Vector2(collision.gameObject.transform.position.x, 0);

            RecibeDanio(direccionDanio, 1);
        }
    }
    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (!recibiendoDanio)
        {
            vida -= cantDanio;
            recibiendoDanio = true;
            if (vida <= 0)
            {
                muerto = true;
                enMovimiento = false;
            }
            else
            {
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.2f).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
                StartCoroutine(DesactivaDanio());
            }
        }
    }
    IEnumerator DesactivaDanio()
    {
        yield return new WaitForSeconds(0.4f);
        recibiendoDanio = false;
        rb.linearVelocity = Vector2.zero;
    }
    public void EliminarCuerpo()
    {
        Destroy(gameObject);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}