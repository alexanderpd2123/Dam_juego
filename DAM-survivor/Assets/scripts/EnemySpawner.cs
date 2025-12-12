using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Para la finalización de juego en el Editor:
#if UNITY_EDITOR
using UnityEditor; 
#endif

// El ScriptableObject DataOleada (que define patrones) es la base para este script.

public class EnemySpawner : MonoBehaviour
{
    /// ///////////////////// Variables /////////////////////////
    [Header("Configuración de Spawn")]
    [SerializeField]
    private float spawnRadius = 10f; // Radio máximo alrededor del jugador para spawnear enemigos

    [Header("GameObjects")]
    [SerializeField]
    private Transform player; // Referencia esencial para calcular la posición de spawn

    [SerializeField]
    // La lista de todos los Scriptable Objects DataOleada que forman la secuencia del nivel
    private List<DataOleada> oleadas; 
    
    // Bandera que indica que todos los enemigos han terminado de spawnear (clave para la victoria)
    private bool allWavesFinished = false; 


    ////////////////////////////////////// Funciones Unity//////////////////
    void Start()
    {
        // INICIO: Llama a la corrutina principal que gestiona toda la secuencia de oleadas
        StartCoroutine(GenerarOleadas());
    }

    ///////////////////////////// Funciones Propias ////////////////////

    /// Coroutine: Ejecuta un patrón de spawn único (ej. 10 enemigos E1 cada 2s).
   
    private IEnumerator ExecutePattern(SpawnPattern pattern)
    {
        // Repite el patrón según el número de repeticiones configurado
        for (int r = 0; r < pattern.Repetitions; r++)
        {
            // Spawnea la cantidad de enemigos especificada en el patrón
            for (int i = 0; i < pattern.Count; i++)
            {
                if (player == null) yield break; 
                
                // Calcula una posición aleatoria dentro del radio alrededor del jugador
                Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;
                Vector3 spawnPosition = player.position + new Vector3(randomPoint.x, 0f, randomPoint.y); 

                // Crea el enemigo
                Instantiate(pattern.EnemyPrefab, spawnPosition, Quaternion.identity);
                
                // Espera el tiempo definido en el SpawnRate de este patrón
                yield return new WaitForSeconds(pattern.SpawnRate); 
            }
        }
    }
    

    /// Coroutine: Itera a través de todos los DataOleada y los ejecuta.
  
    public IEnumerator GenerarOleadas()
    {
        // Verifica si hay datos para evitar errores
        if (oleadas == null || oleadas.Count == 0)
        {
            Debug.LogWarning("No hay oleadas configuradas. Terminando la ejecución.");
            EndGame(success: true); // Termina el juego si no hay oleadas
            yield break;
        }
        
        // Bucle principal que recorre cada objeto DataOleada
        foreach(DataOleada oleadaActual in oleadas)
        {
            // 1. Pausa inicial: Espera el tiempo de DataOleada.TiempoEntreOleadas
            yield return new WaitForSeconds(oleadaActual.TiempoEntreOleadas);
            
            // 2. Disparar patrones: Inicia todos los patrones configurados en PARALELO
            foreach(SpawnPattern pattern in oleadaActual.SimultaneousPatterns)
            {
                // NOTA CLAVE: StartCoroutine permite que todos los patrones se ejecuten a la vez
                StartCoroutine(ExecutePattern(pattern));
            }
            
            // 3. Espera de duración: Espera el tiempo definido antes de pasar a la siguiente oleada
            yield return new WaitForSeconds(oleadaActual.DurationAfterSpawn);
        }

        // --- Condición de Victoria: Finaliza la fase de spawn ---
        allWavesFinished = true;
        Debug.Log("Todas las oleadas han sido procesadas. Esperando la eliminación total de enemigos.");

        // Inicia el proceso de verificación de victoria
        StartCoroutine(CheckForVictoryCondition());
    }

    // ----------------------------------------------------------------------------------
    // FUNCIÓN DE VERIFICACIÓN DE VICTORIA
    // ----------------------------------------------------------------------------------
    private IEnumerator CheckForVictoryCondition()
    {
        if (!allWavesFinished) yield break;

        // 1. Esperar un buffer de tiempo antes de empezar a chequear
        yield return new WaitForSeconds(3f); 
        
        // 2. Espera activa: Pausa la corrutina hasta que no queden objetos con la etiqueta "Enemy"
        yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);

        // 3. Condición cumplida: Llama a la función de finalización del juego
        Debug.Log("¡CONDICIÓN DE VICTORIA CUMPLIDA! Terminando la ejecución.");
        
        EndGame(success: true);
    }
    
    // ----------------------------------------------------------------------------------
    // GESTIÓN DEL FINAL DEL JUEGO (Salida Absoluta)
    // ----------------------------------------------------------------------------------
    private void EndGame(bool success)
    {
        if (success)
        {
            Time.timeScale = 0f; // Congela el juego
            
            #if UNITY_EDITOR
                EditorApplication.isPlaying = false; // Detiene el juego en el Editor
            #else
                Application.Quit(); // Cierra la aplicación en el ejecutable final
            #endif
        }
    }
}