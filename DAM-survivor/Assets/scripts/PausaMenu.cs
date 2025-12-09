using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Configuración de Pausa")]
    [Tooltip("Referencia al Panel del Menú de Pausa en la jerarquía.")]
    public GameObject pauseMenuUI; 
    
    // ¡NUEVO! Nombre de la acción que crearás en el Input Manager (Ej: "Cancel" o "PauseInput")
    [Tooltip("Nombre del Input Button que configuraste en el Input Manager (Ej: Cancel).")]
    public string pauseButtonName = "Cancel"; 

    public static bool GameIsPaused = false; 

    void Update()
    {
        // Detectar si el input configurado se presiona
        if (Input.GetButtonDown(pauseButtonName))
        {
            if (GameIsPaused)
            {
                Resume(); 
            }
            else
            {
                Pause(); 
            }
        }
    }

    // ///////////////////////////////// FUNCIONES DEL JUEGO /////////////////////////////////

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Time.timeScale = 1f; 
        Application.Quit();
        
        // Si tienes un menú principal (escena 0):
        // SceneManager.LoadScene(0); 
    }
}