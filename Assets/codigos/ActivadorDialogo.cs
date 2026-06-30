using UnityEngine;

public class ActivadorDialogo : MonoBehaviour
{
    [Header("CONFIGURACIÓN")]
    public GameObject dialogoUI;

    private bool activado = false;
    private ControladorDialogo controladorDialogo;

    void Start()
    {
        if (dialogoUI != null)
        {
            dialogoUI.SetActive(false);
            controladorDialogo = dialogoUI.GetComponent<ControladorDialogo>();

            if (controladorDialogo != null)
            {
                controladorDialogo.OnDialogoTerminado += EliminarActivador;
            }
        }

        Debug.Log("🔍 ActivadorDialogo: Iniciado");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("🔍 ActivadorDialogo: Algo entró - " + collision.gameObject.name);

        if (collision.CompareTag("Player") && !activado)
        {
            activado = true;
            ActivarDialogo();
            Debug.Log("✅ ActivadorDialogo: ¡Jugador detectado!");
        }
    }

    void ActivarDialogo()
    {
        if (dialogoUI != null)
        {
            dialogoUI.SetActive(true);
            Time.timeScale = 0;

            if (controladorDialogo != null)
            {
                controladorDialogo.IniciarDialogo();
            }

            Debug.Log("💬 Diálogo activado");
        }
    }

    void EliminarActivador()
    {
        Debug.Log("🗑️ Diálogo terminado - Eliminando activador");
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (controladorDialogo != null)
        {
            controladorDialogo.OnDialogoTerminado -= EliminarActivador;
        }
    }
}