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

    private JugadorController jugador;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            jugador = player.GetComponent<JugadorController>();
            ConectarBotonesAlJugador();
        }
    }

    void ConectarBotonesAlJugador()
    {
        if (jugador == null) return;

        // BOTÓN IZQUIERDA
        if (btnIzquierda != null)
        {
            EventTrigger trigger = btnIzquierda.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = btnIzquierda.gameObject.AddComponent<EventTrigger>();

            trigger.triggers.Clear();

            EventTrigger.Entry entryDown = new EventTrigger.Entry();
            entryDown.eventID = EventTriggerType.PointerDown;
            entryDown.callback.AddListener((data) => { jugador.MoverIzquierda(); });
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

        // BOTÓN DERECHA
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

        // BOTÓN SALTAR
        if (btnSaltar != null)
        {
            btnSaltar.onClick.RemoveAllListeners();
            btnSaltar.onClick.AddListener(() => { jugador.SaltarTouch(); });
        }

        // BOTÓN ATACAR
        if (btnAtacar != null)
        {
            btnAtacar.onClick.RemoveAllListeners();
            btnAtacar.onClick.AddListener(() => { jugador.AtacarTouch(); });
        }

        // BOTÓN RECOGER
        if (btnRecoger != null)
        {
            btnRecoger.onClick.RemoveAllListeners();
            btnRecoger.onClick.AddListener(() => { jugador.RecogerTouch(); });
        }


        Debug.Log("Botones conectados correctamente");
    }
}