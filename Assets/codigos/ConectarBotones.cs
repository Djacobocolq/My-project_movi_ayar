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

    private player1 jugador;

    void Start()
    {
        // Buscar al jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            jugador = player.GetComponent<player1>();
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

        // Conectar botón izquierda (Pointer Down)
        if (btnIzquierda != null)
        {
            EventTrigger trigger = btnIzquierda.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = btnIzquierda.gameObject.AddComponent<EventTrigger>();

            // Limpiar eventos anteriores
            trigger.triggers.Clear();

            // Pointer Down
            EventTrigger.Entry entryDown = new EventTrigger.Entry();
            entryDown.eventID = EventTriggerType.PointerDown;
            entryDown.callback.AddListener((data) => { jugador.MoverIzquierda(); });
            trigger.triggers.Add(entryDown);

            // Pointer Up
            EventTrigger.Entry entryUp = new EventTrigger.Entry();
            entryUp.eventID = EventTriggerType.PointerUp;
            entryUp.callback.AddListener((data) => { jugador.DetenerMovimiento(); });
            trigger.triggers.Add(entryUp);

            // Pointer Exit
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((data) => { jugador.DetenerMovimiento(); });
            trigger.triggers.Add(entryExit);
        }

        // Conectar botón derecha
        if (btnDerecha != null)
        {
            EventTrigger trigger = btnDerecha.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = btnDerecha.gameObject.AddComponent<EventTrigger>();

            trigger.triggers.Clear();

            EventTrigger.Entry entryDown = new EventTrigger.Entry();
            entryDown.eventID = EventTriggerType.PointerDown;
            entryDown.callback.AddListener((data) => { jugador.MoverDerecha(); });
            trigger.triggers.Add(entryDown);

            EventTrigger.Entry entryUp = new EventTrigger.Entry();
            entryUp.eventID = EventTriggerType.PointerUp;
            entryUp.callback.AddListener((data) => { jugador.DetenerMovimiento(); });
            trigger.triggers.Add(entryUp);

            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((data) => { jugador.DetenerMovimiento(); });
            trigger.triggers.Add(entryExit);
        }

        // Conectar botón saltar (OnClick)
        if (btnSaltar != null)
        {
            btnSaltar.onClick.RemoveAllListeners();
            btnSaltar.onClick.AddListener(() => { jugador.SaltarTouch(); });
        }

        // Conectar botón atacar (OnClick)
        if (btnAtacar != null)
        {
            btnAtacar.onClick.RemoveAllListeners();
            btnAtacar.onClick.AddListener(() => { jugador.AtacarTouch(); });
        }

        // Conectar botón recoger (OnClick)
        if (btnRecoger != null)
        {
            btnRecoger.onClick.RemoveAllListeners();
            btnRecoger.onClick.AddListener(() => { jugador.RecogerFlor(); });
        }

        // Conectar botón pausa (OnClick)
        if (btnPausa != null)
        {
            btnPausa.onClick.RemoveAllListeners();
            btnPausa.onClick.AddListener(() => { jugador.PausarTouch(); });
        }

        Debug.Log("Botones conectados al jugador correctamente");
    }
}