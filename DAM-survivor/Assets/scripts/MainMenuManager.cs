using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    // VARIABLES DE UI (Arrastra los paneles a estas ranuras en el Inspector)
    public GameObject panelTitulo; // Panel con el botón 'Jugar' y 'Salir'
    public GameObject panelPausa;  // Panel con el botón 'Continuar' y 'Volver al Título'
    
    private bool juegoPausado = false; // Estado del juego: True si está en pausa

    void Start()
    {
        // Al iniciar la escena, siempre detenemos el tiempo y mostramos el menú de título.
        MostrarMenuTitulo();
    }

    void Update()
    {
        // 1. Manejo de la Tecla ESC para pausar/reanudar
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Solo se permite pausar si NO estamos en el menú de título (o sea, ya hemos presionado Jugar)
            if (!panelTitulo.activeSelf) 
            {
                if (juegoPausado)
                    Continuar();
                else
                    Pausar();
            }
        }
    }

    // ==============================================
    // A. FUNCIONES DEL MENÚ DE TÍTULO
    // ==============================================

    public void MostrarMenuTitulo() // Llamado al inicio y desde el Menú de Pausa
    {
        panelTitulo.SetActive(true);
        panelPausa.SetActive(false);
        Time.timeScale = 0f; // Detiene el tiempo
        juegoPausado = false;
    }

    public void Jugar() // Conectado al botón 'Jugar' del Panel de Título
    {
        panelTitulo.SetActive(false); // Oculta el menú de título
        Time.timeScale = 1f;          // Reanuda el tiempo
    }

    public void Salir() // Conectado al botón 'Salir'
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // ==============================================
    // B. FUNCIONES DEL MENÚ DE PAUSA
    // ==============================================

    public void Pausar() // Llamado por la tecla ESC
    {
        panelPausa.SetActive(true); // Muestra el menú de pausa
        Time.timeScale = 0f;        // Detiene el tiempo
        juegoPausado = true;
    }

    public void Continuar() // Conectado al botón 'Continuar'
    {
        panelPausa.SetActive(false); // Oculta el menú de pausa
        Time.timeScale = 1f;         // Reanuda el tiempo
        juegoPausado = false;
    }
}