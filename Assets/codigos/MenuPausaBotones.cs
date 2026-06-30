using UnityEngine;
using UnityEngine.UI;

public class MenuPausaBotones : MonoBehaviour
{
    public Button btnReanudar;
    public Button btnMenu;

    private pausar_juego sistemaPausa;

    void Start()
    {
        sistemaPausa = FindFirstObjectByType<pausar_juego>();

        if (sistemaPausa == null)
        {
            Debug.LogError("❌ No se encontró pausar_juego!");
            return;
        }

        if (btnReanudar == null)
            btnReanudar = GameObject.Find("BtnReanudar")?.GetComponent<Button>();

        if (btnMenu == null)
            btnMenu = GameObject.Find("BtnMenu")?.GetComponent<Button>();

        if (btnReanudar != null)
        {
            btnReanudar.onClick.RemoveAllListeners();
            btnReanudar.onClick.AddListener(Reanudar);
            Debug.Log("✅ BtnReanudar conectado");
        }

        if (btnMenu != null)
        {
            btnMenu.onClick.RemoveAllListeners();
            btnMenu.onClick.AddListener(IrAlMenu);
            Debug.Log("✅ BtnMenu conectado");
        }
    }

    void Reanudar()
    {
        if (sistemaPausa != null)
            sistemaPausa.Reanudar();
    }

    void IrAlMenu()
    {
        if (sistemaPausa != null)
            sistemaPausa.IrAlMenu();
    }

    void OnDestroy()
    {
        if (btnReanudar != null)
            btnReanudar.onClick.RemoveAllListeners();
        if (btnMenu != null)
            btnMenu.onClick.RemoveAllListeners();
    }
}