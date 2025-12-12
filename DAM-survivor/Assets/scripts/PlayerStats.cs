using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; 

public class PlayerStats : MonoBehaviour
{
    // --- Referencia al Lanzador para subir de nivel ---
    private LanzadorArma lanzadorArma; // Script que contiene el nivel y usa el nivel para escalar armas

    // --- Stats de Juego ---
    private int currentHealth;
    private int maxHealth = 100;
    private int ataque = 5; 
    private int defensa = 0;
    private float velMov = 5f;
    private float velAtk = 1f;
    private bool estaVivo = true; // Bandera de estado de vida

    // --- Sistema de Niveles ---
    [Header("Sistema de Progresión")]
    public int currentEXP = 0; // EXP actual
    public int EXPToNextLevel = 100; // EXP necesaria para el próximo nivel
    public float EXPScaleFactor = 1.5f; // Factor para aumentar la EXP requerida (1.5 = 50% más)


    // --- Variables de Feedback Visual ---
    [Header("Feedback Visual")]
    public Color hitColor = Color.red; // Color al recibir golpe
    public float hitDuration = 0.1f; // Duración del cambio de color
    private Renderer playerRenderer;
    private Color originalColor;
    private bool isInvulnerable = false; // Bandera para la invulnerabilidad temporal

    [Header("Configuración de Game Over")]
    public string gameOverSceneName = "GameScene"; // Nombre de la escena a cargar al morir


    private void Awake() 
    {
        currentHealth = maxHealth;
        estaVivo = true;

        // Prepara el renderer para el feedback de daño
        playerRenderer = GetComponentInChildren<Renderer>();
        if (playerRenderer != null)
        {
            originalColor = playerRenderer.material.color;
        }

        // Obtener la referencia esencial al LanzadorArma (el nivel del jugador está allí)
        lanzadorArma = GetComponent<LanzadorArma>();
        if (lanzadorArma == null)
        {
            Debug.LogError("PlayerStats: No se encontró el componente LanzadorArma. La progresión de nivel no funcionará.");
        }
    }
    
    // ... (Update se mantiene) ...
    
    
    //////////////////////////////// Funciones de Progresión /////////////////////////
    

    /// Función clave: Añade EXP y comprueba si se debe subir de nivel.
    public void GainEXP(int expAmount)
    {
        if (!estaVivo || lanzadorArma == null) return;

        currentEXP += expAmount;
        Debug.Log($"Ganaste {expAmount} EXP. Total: {currentEXP}");

        CheckForLevelUp();
    }


    /// Comprueba y ejecuta la subida de nivel (puede subir varios niveles a la vez).
    private void CheckForLevelUp()
    {
        if (lanzadorArma != null && estaVivo)
        {
            // Bucle que permite subir múltiples niveles si la EXP acumulada lo permite
            while (currentEXP >= EXPToNextLevel && lanzadorArma.level < LanzadorArma.MAX_LEVEL)
            {
                LevelUp(); // Sube un nivel
            }
            
            if (lanzadorArma.level >= LanzadorArma.MAX_LEVEL)
            {
                Debug.Log("¡Nivel Máximo alcanzado!");
            }
        }
    }


    /// Aplica la lógica para subir un nivel.
    private void LevelUp()
    {
        if (lanzadorArma.level >= LanzadorArma.MAX_LEVEL) return;
        
        // 1. Aumenta el nivel en el script LanzadorArma (esto escala las armas)
        lanzadorArma.level++;
        
        // 2. Resta la EXP requerida
        currentEXP -= EXPToNextLevel;
        
        // 3. Incrementa la EXP requerida para el siguiente nivel usando el factor de escala
        EXPToNextLevel = Mathf.FloorToInt(EXPToNextLevel * EXPScaleFactor);
        
        // 4. Feedback
        Debug.Log($"¡SUBIDA DE NIVEL! Nuevo nivel: {lanzadorArma.level}. Próxima EXP: {EXPToNextLevel}");

        // Aquí se mostraría la UI de selección de mejora.
    }


    //////////////////////////////// Funciones de Combate y Estado /////////////////////////

    // Recibe daño de un enemigo o proyectil
    public void RecibirDmg(int dmg)
    {
        if (!estaVivo || isInvulnerable) return; // Ignora si está muerto o invulnerable

        if (dmg > defensa)
        {
            currentHealth -= dmg - defensa; // Aplica el daño después de la defensa

            StartCoroutine(DamageFeedbackRoutine()); // Inicia el feedback de golpe e invulnerabilidad

            if (currentHealth <= 0) 
            {
                currentHealth = 0;
                estaVivo = false;
                GameOver(); // El jugador muere
            }
        }
    }
    
    // Lógica al morir
    void GameOver()
    {
        Debug.Log("Juego Terminado: El jugador ha muerto.");
        Time.timeScale = 1f; // Asegura que el tiempo corra para cargar la escena
        SceneManager.LoadScene(gameOverSceneName); // Carga la escena de Game Over
    }
    
    // Corrutina para el feedback visual y la invulnerabilidad temporal
    private IEnumerator DamageFeedbackRoutine()
    {
        if (playerRenderer != null)
        {
            isInvulnerable = true; // Activa la invulnerabilidad
            playerRenderer.material.color = hitColor; 
            yield return new WaitForSeconds(hitDuration); // Espera la duración del color
            playerRenderer.material.color = originalColor; // Restaura el color
            yield return new WaitForSeconds(0.1f); // Pequeña pausa extra para invulnerabilidad
            isInvulnerable = false; 
        }
    }
}