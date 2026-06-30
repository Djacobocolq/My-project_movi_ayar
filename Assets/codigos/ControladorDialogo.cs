using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ControladorDialogo : MonoBehaviour
{
    [Header("UI DEL DIÁLOGO")]
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoDialogo;
    public TextMeshProUGUI textoContinuar;
    public Button botonContinuar;

    [Header("CONFIGURACIÓN")]
    public float velocidadEscritura = 0.05f;
    public bool cerrarConClic = true;

    [Header("DIÁLOGO")]
    public List<DialogoLinea> lineasDialogo = new List<DialogoLinea>();

    public event System.Action OnDialogoTerminado;

    private int indiceActual = 0;
    private bool escribiendo = false;
    private bool dialogoActivo = false;
    private Coroutine coroutineEscritura;

    [System.Serializable]
    public class DialogoLinea
    {
        public string nombre;
        public string texto;
        public Color colorNombre = Color.white;
    }

    void Start()
    {
        // Asegurar que el GameObject esté desactivado al inicio
        gameObject.SetActive(false);
        Debug.Log("🔍 ControladorDialogo: Start - Oculto");

        if (botonContinuar != null)
        {
            botonContinuar.onClick.AddListener(ContinuarDialogo);
        }

        if (textoNombre == null)
            Debug.LogError("❌ textoNombre NO está asignado!");
        if (textoDialogo == null)
            Debug.LogError("❌ textoDialogo NO está asignado!");
    }

    void Update()
    {
        if (cerrarConClic && dialogoActivo)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ContinuarDialogo();
            }
        }
    }

    public void IniciarDialogo()
    {
        Debug.Log("💬 IniciarDialogo() llamado");

        if (lineasDialogo.Count == 0)
        {
            Debug.LogError("❌ No hay líneas de diálogo!");
            return;
        }

        // ==========================================
        // FORZAR ACTIVACIÓN DEL GAMEOBJECT
        // ==========================================
        gameObject.SetActive(true);
        Debug.Log("✅ ControladorDialogo: GameObject activado");

        // Activar todos los hijos también
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
            Debug.Log("✅ Hijo activado: " + child.name);
        }

        indiceActual = 0;
        dialogoActivo = true;
        MostrarLinea(indiceActual);
        Debug.Log("💬 Diálogo iniciado. Total líneas: " + lineasDialogo.Count);
    }

    void MostrarLinea(int indice)
    {
        if (indice >= lineasDialogo.Count)
        {
            TerminarDialogo();
            return;
        }

        DialogoLinea linea = lineasDialogo[indice];

        if (textoNombre != null)
        {
            textoNombre.text = linea.nombre;
            textoNombre.color = linea.colorNombre;
            Debug.Log("📝 Nombre: " + linea.nombre);
        }

        if (textoDialogo != null)
        {
            if (coroutineEscritura != null)
                StopCoroutine(coroutineEscritura);

            coroutineEscritura = StartCoroutine(EscribirTexto(linea.texto));
        }

        if (textoContinuar != null)
            textoContinuar.gameObject.SetActive(false);
    }

    IEnumerator EscribirTexto(string texto)
    {
        escribiendo = true;
        textoDialogo.text = "";

        foreach (char letra in texto)
        {
            textoDialogo.text += letra;
            yield return new WaitForSecondsRealtime(velocidadEscritura);
        }

        escribiendo = false;

        if (textoContinuar != null)
            textoContinuar.gameObject.SetActive(true);
    }

    public void ContinuarDialogo()
    {
        if (!dialogoActivo) return;

        if (escribiendo)
        {
            if (coroutineEscritura != null)
                StopCoroutine(coroutineEscritura);

            textoDialogo.text = lineasDialogo[indiceActual].texto;
            escribiendo = false;

            if (textoContinuar != null)
                textoContinuar.gameObject.SetActive(true);
            return;
        }

        indiceActual++;

        if (indiceActual < lineasDialogo.Count)
        {
            if (textoContinuar != null)
                textoContinuar.gameObject.SetActive(false);
            MostrarLinea(indiceActual);
        }
        else
        {
            TerminarDialogo();
        }
    }

    void TerminarDialogo()
    {
        dialogoActivo = false;
        Time.timeScale = 1;
        gameObject.SetActive(false);

        Debug.Log("💬 Diálogo terminado");

        if (OnDialogoTerminado != null)
        {
            OnDialogoTerminado();
        }
    }

    void OnDestroy()
    {
        if (botonContinuar != null)
            botonContinuar.onClick.RemoveAllListeners();
    }
}