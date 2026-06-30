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
        gameObject.SetActive(false);

        if (botonContinuar != null)
        {
            botonContinuar.onClick.AddListener(ContinuarDialogo);
        }

        // ==========================================
        // VERIFICAR TEXTOS
        // ==========================================
        if (textoNombre == null)
            Debug.LogError("❌ textoNombre NO está asignado!");
        else
        {
            Debug.Log("✅ textoNombre: " + textoNombre.name);
            textoNombre.text = "Nombre de prueba"; // Texto de prueba
        }

        if (textoDialogo == null)
            Debug.LogError("❌ textoDialogo NO está asignado!");
        else
        {
            Debug.Log("✅ textoDialogo: " + textoDialogo.name);
            textoDialogo.text = "Texto de prueba con varias líneas para verificar que el TextMeshPro funcione correctamente y muestre todo el contenido sin cortarse.";
        }
    }

    void Update()
    {
        if (cerrarConClic && dialogoActivo)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ContinuarDialogo();
                Debug.Log("🖱️ Clic detectado");
            }
        }
    }

    public void IniciarDialogo()
    {
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

        // ==========================================
        // ACTUALIZAR NOMBRE
        // ==========================================
        if (textoNombre != null)
        {
            textoNombre.text = linea.nombre;
            textoNombre.color = linea.colorNombre;
            Debug.Log("📝 Nombre asignado: " + linea.nombre);
            Debug.Log("📝 Color nombre: " + linea.colorNombre);
        }
        else
        {
            Debug.LogError("❌ textoNombre es NULL!");
        }

        // ==========================================
        // ACTUALIZAR TEXTO
        // ==========================================
        if (textoDialogo != null)
        {
            if (coroutineEscritura != null)
                StopCoroutine(coroutineEscritura);

            coroutineEscritura = StartCoroutine(EscribirTexto(linea.texto));
            Debug.Log("📝 Texto asignado: " + linea.texto.Substring(0, Mathf.Min(50, linea.texto.Length)) + "...");
        }
        else
        {
            Debug.LogError("❌ textoDialogo es NULL!");
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
        if (!dialogoActivo)
        {
            Debug.Log("⚠️ Diálogo no activo");
            return;
        }

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
        Debug.Log("➡️ Avanzando a línea: " + indiceActual);

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