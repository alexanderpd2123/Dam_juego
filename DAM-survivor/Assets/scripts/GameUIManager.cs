using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
// Esta directiva permite que el código de UnityEditor se compile SOLO en el Editor.
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameUIManager : MonoBehaviour
{
    // --- Referencias de Paneles UI ---
    [Header("Paneles UI")]
    public GameObject mainMenuUI; // Panel de la pantalla de título (activo al inicio)
    public GameObject pauseMenuUI; // Panel del menú de pausa (desactivado al inicio)
    
    // --- Configuración de Control ---
    [Header("Input Control")]
    public InputActionAsset controls; // Referencia al Input Action Asset
    private InputAction pauseAction; // Objeto de acción para la pausa
    
    // --- Estado Global (Estatica) ---
    public static bool GameIsPaused = false; // Bandera estática global para el estado de pausa

    void Awake()
    {
        // Busca y obtiene la acción específica de pausa en el Asset de Input
        pauseAction = controls.FindAction("Pausa/pausa"); 

        if (pauseAction != null)
        {
             // Suscribe la función TogglePause al evento de la acción de pausa (ej: tecla ESC)
             pauseAction.performed += _ => TogglePause();
        } else {
             Debug.LogError("Error: No se encontró la acción de pausa en la Input Asset. Se esperaba la ruta 'Pausa/pausa'.");
        }
        
        // Establecer el estado inicial del juego (detenido en el menú principal)
        ShowMainMenu();
    }
    
    void OnEnable()
    {
        if (pauseAction != null) pauseAction.Enable(); // Activa la escucha del Input de pausa
    }
    
    void OnDisable()
    {
        if (pauseAction != null) pauseAction.Disable(); // Desactiva la escucha del Input de pausa
    }

    // ///////////////////////////////// ESTADOS DE PANTALLA /////////////////////////////////

    private void ShowMainMenu()
    {
        mainMenuUI.SetActive(true); // Muestra el menú de título
        pauseMenuUI.SetActive(false); 
        Time.timeScale = 0f; 
        GameIsPaused = true; 
    }
    

    /// Inicia el juego y reanuda el tiempo. (Función auxiliar privada)

    private void StartGame()
    {
        mainMenuUI.SetActive(false); 
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Reanuda el tiempo a velocidad normal
        GameIsPaused = false; // El juego corre
    }
    
    // ///////////////////////////////// GESTIÓN DE PAUSA /////////////////////////////////

    // Función clave llamada por el Input (tecla de pausa)
    void TogglePause()
    {
        // Evita pausar si ya estamos en el Menú Principal
        if (mainMenuUI.activeInHierarchy) return;

        // Alterna entre los estados de Pausa/Reanudar
        if (GameIsPaused)
        {
            Resume(); 
        }
        else
        {
            Pause(); 
        }
    }

    // Función pública para reanudar (llamada por el botón o TogglePause)
    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Oculta el panel de pausa
        Time.timeScale = 1f; // Reanuda el tiempo
        GameIsPaused = false;
    }

    // Función para pausar
    void Pause()
    {
        pauseMenuUI.SetActive(true); // Muestra el panel de pausa
        Time.timeScale = 0f; // Congela el juego
        GameIsPaused = true;
    }

    // ///////////////////////////////// FUNCIONES LLAMADAS POR BOTONES /////////////////////////////////

    /// LLAMADO POR BOTÓN "JUGAR" (Desde el MainMenu).

    public void OnClick_StartGame()
    {
        StartGame(); // Simplemente inicia el juego
    }
    

    /// LLAMADO POR BOTÓN "VOLVER AL MENÚ" (Desde la Pausa).
    public void OnClick_GoToMainMenu()
    {
        Time.timeScale = 1f; // Asegura que el tiempo esté corriendo antes de llamar a ShowMainMenu (aunque ShowMainMenu lo detiene de nuevo)
        ShowMainMenu();
    }


    /// LLAMADO POR BOTÓN "SALIR".
    public void OnClick_QuitGame()
    {
        // Lógica condicional para salir correctamente del juego
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}