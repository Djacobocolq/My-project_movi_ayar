using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConectarBotones : MonoBehaviour
{
    [Header("Referencias a los botones")]
    public Button btnIzquierda;
    public Button btnDerecha;
    public Button btnSaltar;
    public Button btnAtacar;
    public Button btnRecoger;
    public Button btnPausa;

    private JugadorController jugador; // ← CAMBIADO

    void Start()
    {
        // Buscar al jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            jugador = player.GetComponent<JugadorController>(); // ← CAMBIADO
            ConectarBotonesAlJugador();
        }
        else
        {
            Debug.LogWarning("No se encontró al jugador");
        }
    }

    void ConectarBotonesAlJugador()
    {
        if (jugador == null) return;

        // ==========================================
        // BOTÓN IZQUIERDA (Pointer Down/Up)
        // ==========================================
        if (btnIzquierda != null)
        {
            EventTrigger trigger = btnIzquierda.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = btnIzquierda.gameObject.AddComponent<EventTrigger>();

            trigger.triggers.Clear();

            // Pointer Down → MoverIzquierda
            EventTrigger.Entry entryDown = new EventTrigger.Entry();
            entryDown.eventID = EventTriggerType.PointerDown;
            entryDown.callback.AddListener((data) => { jugador.MoverIzquierda(); });
            trigger.triggers.Add(entryDown);

            // Pointer Up → DetenerMovimiento
            EventTrigger.Entry entryUp = new EventTrigger.Entry();
            entryUp.eventID = EventTriggerType.PointerUp;
            entryUp.callback.AddListener((data) => { jugador.DetenerMovimiento(); });
            trigger.triggers.Add(entryUp);

            // Pointer Exit → DetenerMovimiento
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((data) => { jugador.DetenerMovimiento(); });
            trigger.triggers.Add(entryExit);
        }

        // ==========================================
        // BOTÓN DERECHA (Pointer Down/Up)
        // ==========================================
        if (btnDerecha != null)
        {
            EventTrigger trigger = btnDerecha.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = btnDerecha.gameObject.AddComponent<EventTrigger>();

            trigger.triggers.Clear();

            // Pointer Down → MoverDerecha
            EventTrigger.Entry entryDown = new EventTrigger.Entry();
            entryDown.eventID = EventTriggerType.PointerDown;
            entryDown.callback.AddListener((data) => { jugador.MoverDerecha(); });
            trigger.triggers.Add(entryDown);

            // Pointer Up → DetenerMovimiento
            EventTrigger.Entry entryUp = new EventTrigger.Entry();
            entryUp.eventID = EventTriggerType.PointerUp;
            entryUp.callback.AddListener((data) => { jugador.DetenerMovimiento(); });
            trigger.triggers.Add(entryUp);

            // Pointer Exit → DetenerMovimiento
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((data) => { jugador.DetenerMovimiento(); });
            trigger.triggers.Add(entryExit);
        }

        // ==========================================
        // BOTÓN SALTAR
        // ==========================================
        if (btnSaltar != null)
        {
            btnSaltar.onClick.RemoveAllListeners();
            btnSaltar.onClick.AddListener(() => { jugador.SaltarTouch(); });
        }

        // ==========================================
        // BOTÓN ATACAR
        // ==========================================
        if (btnAtacar != null)
        {
            btnAtacar.onClick.RemoveAllListeners();
            btnAtacar.onClick.AddListener(() => { jugador.AtacarTouch(); });
        }

        // ==========================================
        // BOTÓN RECOGER
        // ==========================================
        if (btnRecoger != null)
        {
            btnRecoger.onClick.RemoveAllListeners();
            btnRecoger.onClick.AddListener(() => { jugador.RecogerTouch(); });
        }

        // ==========================================
        // BOTÓN PAUSA
        // ==========================================
        if (btnPausa != null)
        {
            btnPausa.onClick.RemoveAllListeners();
            btnPausa.onClick.AddListener(() => {
                Pausar_juego pausa = FindObjectOfType<Pausar_juego>();
                if (pausa != null)
                    pausa.AlternarPausa();
                else
                    Debug.LogWarning("No se encontró el sistema de pausa");
            });
        }

        Debug.Log("Botones conectados al jugador correctamente");
    }
}