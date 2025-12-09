using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Necesario para cargar escenas

public class PlayerStats : MonoBehaviour
{
    // --- Stats de Juego ---
    private int currentHealth;
    private int maxHealth = 100;
    private int ataque = 5;
    private int defensa = 0;
    private float velMov = 5f;
    private float velAtk = 1f;
    private bool estaVivo = true; 

    // --- Variables de Feedback Visual ---
    [Header("Feedback Visual")]
    public Color hitColor = Color.red;
    public float hitDuration = 0.1f;

    private Renderer playerRenderer;
    private Color originalColor;
    private bool isInvulnerable = false; 

    [Header("Configuración de Game Over")]
    [Tooltip("Nombre de la escena a cargar cuando el jugador muere. Cargar MainMenu, o la escena de juego para reiniciar.")]
    public string gameOverSceneName = "GameScene"; // ¡AJUSTE TEMPORAL! Usa "MainMenu" cuando la crees.


    private void Awake() 
    {
        currentHealth = maxHealth;
        estaVivo = true;

        playerRenderer = GetComponentInChildren<Renderer>();
        if (playerRenderer != null)
        {
            originalColor = playerRenderer.material.color;
        }
    }
    
    // ... (Start y Update se mantienen) ...
    
    //////////////////////////////// Funciones propias /////////////////////////
    
    public void RecibirDmg(int dmg)
    {
        if (!estaVivo || isInvulnerable) return;

        if (dmg > defensa)
        {
            currentHealth -= dmg - defensa;

            StartCoroutine(DamageFeedbackRoutine());

            if (currentHealth <= 0) 
            {
                currentHealth = 0;
                estaVivo = false;
                
                GameOver(); 
            }
        }
        
        Debug.Log("Jugador recibió daño. Vida restante: " + currentHealth);
    }
    
    /// <summary>
    /// Función que detiene el juego y carga la escena de Game Over/Reiniciar.
    /// </summary>
    void GameOver()
    {
        Debug.Log("Juego Terminado: El jugador ha muerto. Reiniciando...");
        
        Time.timeScale = 1f; 
        
        // Carga la escena cuyo nombre está configurado en el Inspector (temporalmente la escena actual).
        SceneManager.LoadScene(gameOverSceneName);
    }
    

    // -----------------------------------------------------------
    // CORRUTINA PARA EL FEEDBACK VISUAL
    // -----------------------------------------------------------

    private IEnumerator DamageFeedbackRoutine()
    {
        if (playerRenderer != null)
        {
            isInvulnerable = true; 

            playerRenderer.material.color = hitColor;
            yield return new WaitForSeconds(hitDuration);
            playerRenderer.material.color = originalColor;

            yield return new WaitForSeconds(0.1f); 
            
            isInvulnerable = false; 
        }
    }
}