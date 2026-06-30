using UnityEngine;

public class ActivadorDialogo : MonoBehaviour
{
    [Header("CONFIGURACIÓN")]
    public GameObject canvasDialogo;   // ← CanvasDialogo
    public GameObject canvasPrincipal; // ← Canvas (principal)

    private bool activado = false;
    private ControladorDialogo controladorDialogo;

    void Start()
    {
        // Buscar CanvasDialogo
        if (canvasDialogo == null)
        {
            canvasDialogo = GameObject.Find("CanvasDialogo");
            if (canvasDialogo != null)
                Debug.Log("✅ CanvasDialogo encontrado: " + canvasDialogo.name);
            else
                Debug.LogError("❌ No se encontró CanvasDialogo!");
        }

        // Buscar Canvas principal
        if (canvasPrincipal == null)
        {
            canvasPrincipal = GameObject.Find("Canvas");
            if (canvasPrincipal != null)
                Debug.Log("✅ Canvas principal encontrado: " + canvasPrincipal.name);
            else
                Debug.LogError("❌ No se encontró Canvas!");
        }

        // Buscar DialogoUI dentro de CanvasDialogo
        if (canvasDialogo != null)
        {
            Transform dialogoUITransform = canvasDialogo.transform.Find("DialogoUI");
            if (dialogoUITransform != null)
            {
                GameObject dialogoUI = dialogoUITransform.gameObject;
                controladorDialogo = dialogoUI.GetComponent<ControladorDialogo>();

                if (controladorDialogo != null)
                {
                    controladorDialogo.OnDialogoTerminado += EliminarActivador;
                    Debug.Log("✅ ControladorDialogo encontrado en DialogoUI");
                }
                else
                {
                    Debug.LogError("❌ No se encontró ControladorDialogo en DialogoUI!");
                }
            }
            else
            {
                Debug.LogError("❌ No se encontró DialogoUI dentro de CanvasDialogo!");
            }
        }

        // Desactivar CanvasDialogo al inicio
        if (canvasDialogo != null)
            canvasDialogo.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("🔍 ActivadorDialogo: Algo entró: " + collision.gameObject.name);

        if (collision.CompareTag("Player") && !activado)
        {
            activado = true;
            ActivarDialogo();
            Debug.Log("✅ ActivadorDialogo: ¡Jugador detectado!");
        }
    }

    void ActivarDialogo()
    {
        // ==========================================
        // DESACTIVAR CANVAS PRINCIPAL
        // ==========================================
        if (canvasPrincipal != null)
        {
            canvasPrincipal.SetActive(false);
            Debug.Log("✅ Canvas principal DESACTIVADO");
        }

        // ==========================================
        // ACTIVAR CANVAS DIÁLOGO
        // ==========================================
        if (canvasDialogo != null)
        {
            canvasDialogo.SetActive(true);
            Debug.Log("✅ CanvasDialogo ACTIVADO");

            // ==========================================
            // ACTIVAR TODOS LOS HIJOS DE CanvasDialogo
            // ==========================================
            foreach (Transform child in canvasDialogo.transform)
            {
                child.gameObject.SetActive(true);
                Debug.Log("✅ Hijo activado: " + child.name);
            }
        }

        // ==========================================
        // ACTIVAR DialogoUI ESPECÍFICAMENTE
        // ==========================================
        if (canvasDialogo != null)
        {
            Transform dialogoUITransform = canvasDialogo.transform.Find("DialogoUI");
            if (dialogoUITransform != null)
            {
                GameObject dialogoUI = dialogoUITransform.gameObject;
                dialogoUI.SetActive(true);

                // Activar todos los hijos de DialogoUI (textos)
                foreach (Transform child in dialogoUI.transform)
                {
                    child.gameObject.SetActive(true);
                    Debug.Log("✅ Hijo de DialogoUI activado: " + child.name);
                }

                Debug.Log("✅ DialogoUI ACTIVADO y todos sus hijos activados");
            }
            else
            {
                Debug.LogError("❌ No se encontró DialogoUI dentro de CanvasDialogo!");
            }
        }

        // Iniciar diálogo
        if (controladorDialogo != null)
        {
            controladorDialogo.IniciarDialogo();
            Debug.Log("✅ ControladorDialogo: IniciarDialogo() llamado");
        }

        // Pausar el juego
        Time.timeScale = 0;
        Debug.Log("⏸️ Tiempo pausado");
    }

    void EliminarActivador()
    {
        Debug.Log("🗑️ Diálogo terminado - Eliminando activador");

        // ==========================================
        // REACTIVAR CANVAS PRINCIPAL
        // ==========================================
        if (canvasPrincipal != null)
        {
            canvasPrincipal.SetActive(true);
            Debug.Log("✅ Canvas principal REACTIVADO");
        }

        // ==========================================
        // DESACTIVAR CANVAS DIÁLOGO
        // ==========================================
        if (canvasDialogo != null)
        {
            canvasDialogo.SetActive(false);
            Debug.Log("✅ CanvasDialogo DESACTIVADO");
        }

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        // ==========================================
        // ASEGURAR QUE TODO VUELVA A LA NORMALIDAD
        // ==========================================
        if (canvasPrincipal != null && !canvasPrincipal.activeSelf)
        {
            canvasPrincipal.SetActive(true);
            Debug.Log("✅ Canvas principal REACTIVADO (desde OnDestroy)");
        }

        if (canvasDialogo != null && canvasDialogo.activeSelf)
        {
            canvasDialogo.SetActive(false);
            Debug.Log("✅ CanvasDialogo DESACTIVADO (desde OnDestroy)");
        }

        if (controladorDialogo != null)
            controladorDialogo.OnDialogoTerminado -= EliminarActivador;
    }
}