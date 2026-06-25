using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camara_moviment : MonoBehaviour
{
    public Transform objetivo;
    public float suavizado = 5f;
    public Vector3 offset = new Vector3(0, 0, -10);

    void Start()
    {
        BuscarJugador();
    }

    void Update()
    {
        // Buscar al jugador si se pierde la referencia
        if (objetivo == null)
        {
            BuscarJugador();
        }
    }

    void BuscarJugador()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            objetivo = player.transform;
            Debug.Log("Cámara: Jugador encontrado");
        }
    }

    void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 posicionDeseada = objetivo.position + offset;
        transform.position = Vector3.Lerp(transform.position, posicionDeseada, suavizado * Time.deltaTime);
    }
}