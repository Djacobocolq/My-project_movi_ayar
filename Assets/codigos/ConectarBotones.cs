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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            jugador = player.GetComponent<JugadorController>();
            if (jugador != null)
            {
                ConectarBotonesAlJugador();
                Debug.Log("✅ Botones conectados correctamente");
            }
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
            // ELIMINAR Event Trigger si existe (para evitar conflictos)
            EventTrigger triggerViejo = btnIzquierda.GetComponent<EventTrigger>();
            if (triggerViejo != null)
                DestroyImmediate(triggerViejo);

            // Crear NUEVO Event Trigger
            EventTrigger trigger = btnIzquierda.gameObject.AddComponent<EventTrigger>();

            // ==========================================
            // Pointer Down - EMPIEZA a mover
            // ==========================================
            EventTrigger.Entry entryDown = new EventTrigger.Entry();
            entryDown.eventID = EventTriggerType.PointerDown;
            entryDown.callback.AddListener((data) => {
                if (jugador != null)
                {
                    jugador.MoverIzquierda();
                    Debug.Log("⬅️ BtnIzquierda: Pointer Down - MoverIzquierda");
                }
            });
            trigger.triggers.Add(entryDown);

            // ==========================================
            // Pointer Up - DEJA de mover (SOLO cuando sueltas)
            // ==========================================
            EventTrigger.Entry entryUp = new EventTrigger.Entry();
            entryUp.eventID = EventTriggerType.PointerUp;
            entryUp.callback.AddListener((data) => {
                if (jugador != null)
                {
                    jugador.DetenerMovimiento();
                    Debug.Log("⬅️ BtnIzquierda: Pointer Up - DetenerMovimiento");
                }
            });
            trigger.triggers.Add(entryUp);

            // ==========================================
            // Pointer Exit - DEJA de mover (si el dedo sale del botón)
            // ==========================================
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((data) => {
                if (jugador != null)
                {
                    jugador.DetenerMovimiento();
                    Debug.Log("⬅️ BtnIzquierda: Pointer Exit - DetenerMovimiento");
                }
            });
            trigger.triggers.Add(entryExit);
        }

        // ==========================================
        // BOTÓN DERECHA
        // ==========================================
        if (btnDerecha != null)
        {
            // ELIMINAR Event Trigger viejo
            EventTrigger triggerViejo = btnDerecha.GetComponent<EventTrigger>();
            if (triggerViejo != null)
                DestroyImmediate(triggerViejo);

            // Crear NUEVO Event Trigger
            EventTrigger trigger = btnDerecha.gameObject.AddComponent<EventTrigger>();

            // Pointer Down
            EventTrigger.Entry entryDown = new EventTrigger.Entry();
            entryDown.eventID = EventTriggerType.PointerDown;
            entryDown.callback.AddListener((data) => {
                if (jugador != null)
                {
                    jugador.MoverDerecha();
                    Debug.Log("➡️ BtnDerecha: Pointer Down - MoverDerecha");
                }
            });
            trigger.triggers.Add(entryDown);

            // Pointer Up
            EventTrigger.Entry entryUp = new EventTrigger.Entry();
            entryUp.eventID = EventTriggerType.PointerUp;
            entryUp.callback.AddListener((data) => {
                if (jugador != null)
                {
                    jugador.DetenerMovimiento();
                    Debug.Log("➡️ BtnDerecha: Pointer Up - DetenerMovimiento");
                }
            });
            trigger.triggers.Add(entryUp);

            // Pointer Exit
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((data) => {
                if (jugador != null)
                {
                    jugador.DetenerMovimiento();
                    Debug.Log("➡️ BtnDerecha: Pointer Exit - DetenerMovimiento");
                }
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
                if (jugador != null)
                {
                    jugador.SaltarTouch();
                    Debug.Log("🔼 BtnSaltar: Click");
                }
            });
        }

        // ==========================================
        // BOTÓN ATACAR
        // ==========================================
        if (btnAtacar != null)
        {
            btnAtacar.onClick.RemoveAllListeners();
            btnAtacar.onClick.AddListener(() => {
                if (jugador != null)
                {
                    jugador.AtacarTouch();
                    Debug.Log("⚔️ BtnAtacar: Click");
                }
            });
        }

        // ==========================================
        // BOTÓN RECOGER
        // ==========================================
        if (btnRecoger != null)
        {
            btnRecoger.onClick.RemoveAllListeners();
            btnRecoger.onClick.AddListener(() => {
                if (jugador != null)
                {
                    jugador.RecogerTouch();
                    Debug.Log("🌸 BtnRecoger: Click");
                }
            });
        }

        Debug.Log("✅ Todos los botones conectados correctamente");
    }
}