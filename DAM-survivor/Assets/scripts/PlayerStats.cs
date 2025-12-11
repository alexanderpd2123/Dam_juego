using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; 

public class PlayerStats : MonoBehaviour
{
    // --- Referencia al Lanzador para subir de nivel ---
    private LanzadorArma lanzadorArma;

    // --- Stats de Juego ---
    private int currentHealth;
    private int maxHealth = 100;
    private int ataque = 5;
    private int defensa = 0;
    private float velMov = 5f;
    private float velAtk = 1f;
    private bool estaVivo = true; 

    // --- Sistema de Niveles ---
    [Header("Sistema de Progresión")]
    [Tooltip("Experiencia actual del jugador.")]
    public int currentEXP = 0;
    [Tooltip("EXP necesaria para el siguiente nivel.")]
    public int EXPToNextLevel = 100;
    [Tooltip("Factor de incremento (Ej: 1.5 significa 150% de la EXP anterior).")]
    public float EXPScaleFactor = 1.5f;


    // --- Variables de Feedback Visual ---
    [Header("Feedback Visual")]
    public Color hitColor = Color.red;
    public float hitDuration = 0.1f;

    private Renderer playerRenderer;
    private Color originalColor;
    private bool isInvulnerable = false; 

    [Header("Configuración de Game Over")]
    public string gameOverSceneName = "GameScene"; 


    private void Awake() 
    {
        currentHealth = maxHealth;
        estaVivo = true;

        playerRenderer = GetComponentInChildren<Renderer>();
        if (playerRenderer != null)
        {
            originalColor = playerRenderer.material.color;
        }

        // Obtener la referencia al LanzadorArma (Asumiendo que está en el mismo GameObject)
        lanzadorArma = GetComponent<LanzadorArma>();
        if (lanzadorArma == null)
        {
            Debug.LogError("PlayerStats: No se encontró el componente LanzadorArma. La progresión de nivel no funcionará.");
        }
    }
    
    void Update()
    {
        // Opcional: Podrías añadir lógica aquí para debug de EXP si es necesario.
    }
    
    
    //////////////////////////////// Funciones de Progresión /////////////////////////
    
    /// <summary>
    /// Añade experiencia y comprueba si se sube de nivel.
    /// Esta función debe ser llamada desde el script de muerte del enemigo.
    /// </summary>
    public void GainEXP(int expAmount)
    {
        if (!estaVivo || lanzadorArma == null) return;

        currentEXP += expAmount;
        Debug.Log($"Ganaste {expAmount} EXP. Total: {currentEXP}");

        CheckForLevelUp();
    }

    /// <summary>
    /// Comprueba si se ha alcanzado la experiencia necesaria para subir de nivel.
    /// </summary>
    private void CheckForLevelUp()
    {
        if (lanzadorArma != null && estaVivo)
        {
            // Bucle para subir múltiples niveles si la EXP acumulada es suficiente
            while (currentEXP >= EXPToNextLevel && lanzadorArma.level < LanzadorArma.MAX_LEVEL)
            {
                LevelUp();
            }
            
            if (lanzadorArma.level >= LanzadorArma.MAX_LEVEL)
            {
                Debug.Log("¡Nivel Máximo alcanzado!");
            }
        }
    }

    /// <summary>
    /// Aplica la subida de nivel, escala la dificultad de EXP y notifica al LanzadorArma.
    /// </summary>
    private void LevelUp()
    {
        if (lanzadorArma.level >= LanzadorArma.MAX_LEVEL) return;
        
        // 1. Incrementar el nivel en LanzadorArma (el cual escala las stats automáticamente)
        lanzadorArma.level++;
        
        // 2. Restar la EXP requerida
        currentEXP -= EXPToNextLevel;
        
        // 3. Calcular la nueva EXP requerida para el siguiente nivel
        EXPToNextLevel = Mathf.FloorToInt(EXPToNextLevel * EXPScaleFactor);
        
        // 4. Feedback al jugador
        Debug.Log($"¡SUBIDA DE NIVEL! Nuevo nivel: {lanzadorArma.level}. Próxima EXP: {EXPToNextLevel}");

        // Aquí iría el código para mostrar la UI de Nivel Subido.
    }


    //////////////////////////////// Funciones de Combate y Estado /////////////////////////

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
    
    void GameOver()
    {
        Debug.Log("Juego Terminado: El jugador ha muerto.");
        Time.timeScale = 1f; 
        SceneManager.LoadScene(gameOverSceneName);
    }
    
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