using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConectarBotones : MonoBehaviour
{
    [Header("Botones")]
    public Button btnIzquierda;
    public Button btnDerecha;
    public Button btnSaltar;
    public Button btnAtacar;
    public Button btnRecoger;
    public Button btnPausa;

    private JugadorController jugador;

    void Start()
    {
        // Buscar al jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            jugador = player.GetComponent<JugadorController>();
            if (jugador != null)
            {
                ConectarBotonesAlJugador();
                Debug.Log("Botones conectados correctamente");
            }
            else
            {
                Debug.LogError("No se encontró JugadorController en el jugador");
            }
        }
        else
        {
            Debug.LogError("No se encontró al jugador con Tag 'Player'");
        }
    }

    void ConectarBotonesAlJugador()
    {
        if (jugador == null) return;

        // ==========================================
        // BOTÓN IZQUIERDA
        // ==========================================
        if (btnIzquierda != null)
        {
            // Agregar Event Trigger si no existe
            EventTrigger trigger = btnIzquierda.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = btnIzquierda.gameObject.AddComponent<EventTrigger>();

            trigger.triggers.Clear();

            // Pointer Down
            EventTrigger.Entry entryDown = new EventTrigger.Entry();
            entryDown.eventID = EventTriggerType.PointerDown;
            entryDown.callback.AddListener((data) => {
                jugador.MoverIzquierda();
                Debug.Log("BtnIzquierda: Pointer Down");
            });
            trigger.triggers.Add(entryDown);

            // Pointer Up
            EventTrigger.Entry entryUp = new EventTrigger.Entry();
            entryUp.eventID = EventTriggerType.PointerUp;
            entryUp.callback.AddListener((data) => {
                jugador.DetenerMovimiento();
                Debug.Log("BtnIzquierda: Pointer Up");
            });
            trigger.triggers.Add(entryUp);

            // Pointer Exit
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((data) => {
                jugador.DetenerMovimiento();
                Debug.Log("BtnIzquierda: Pointer Exit");
            });
            trigger.triggers.Add(entryExit);
        }

        // ==========================================
        // BOTÓN DERECHA
        // ==========================================
        if (btnDerecha != null)
        {
            EventTrigger trigger = btnDerecha.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = btnDerecha.gameObject.AddComponent<EventTrigger>();

            trigger.triggers.Clear();

            // Pointer Down
            EventTrigger.Entry entryDown = new EventTrigger.Entry();
            entryDown.eventID = EventTriggerType.PointerDown;
            entryDown.callback.AddListener((data) => {
                jugador.MoverDerecha();
                Debug.Log("BtnDerecha: Pointer Down");
            });
            trigger.triggers.Add(entryDown);

            // Pointer Up
            EventTrigger.Entry entryUp = new EventTrigger.Entry();
            entryUp.eventID = EventTriggerType.PointerUp;
            entryUp.callback.AddListener((data) => {
                jugador.DetenerMovimiento();
                Debug.Log("BtnDerecha: Pointer Up");
            });
            trigger.triggers.Add(entryUp);

            // Pointer Exit
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((data) => {
                jugador.DetenerMovimiento();
                Debug.Log("BtnDerecha: Pointer Exit");
            });
            trigger.triggers.Add(entryExit);
        }

        // ==========================================
        // BOTÓN SALTAR
        // ==========================================
        if (btnSaltar != null)
        {
            btnSaltar.onClick.RemoveAllListeners();
            btnSaltar.onClick.AddListener(() => {
                jugador.SaltarTouch();
                Debug.Log("BtnSaltar: Click");
            });
        }

        // ==========================================
        // BOTÓN ATACAR
        // ==========================================
        if (btnAtacar != null)
        {
            btnAtacar.onClick.RemoveAllListeners();
            btnAtacar.onClick.AddListener(() => {
                jugador.AtacarTouch();
                Debug.Log("BtnAtacar: Click");
            });
        }

        // ==========================================
        // BOTÓN RECOGER
        // ==========================================
        if (btnRecoger != null)
        {
            btnRecoger.onClick.RemoveAllListeners();
            btnRecoger.onClick.AddListener(() => {
                jugador.RecogerTouch();
                Debug.Log("BtnRecoger: Click");
            });
        }

        Debug.Log("✅ Todos los botones conectados");
    }
}