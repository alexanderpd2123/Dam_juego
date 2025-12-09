using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTA: Para que este script compile correctamente, la clase SpawnPattern
// debe estar definida arriba o en un archivo separado.

public class EnemySpawner : MonoBehaviour
{
    /// <summary>
    /// ///////////////////// Variables /////////////////////////
    /// </summary>
    [Header("Configuración de Spawn")]
    [SerializeField]
    private float spawnRadius = 10f;

    [Header("GameObjects")]
    [SerializeField]
    private Transform player;

    [SerializeField]
    // La lista ahora contiene los objetos DataOleada complejos
    private List<DataOleada> oleadas; 
    
    ////////////////////////////////////// Funciones Unity//////////////////
    void Start()
    {
        StartCoroutine(GenerarOleadas());
    }

    ///////////////////////////// Funciones Propias ////////////////////

    /// <summary>
    /// Coroutine para ejecutar un patrón de spawn específico (Ej: 60 E1 cada 2s, 4 veces).
    /// </summary>
    private IEnumerator ExecutePattern(SpawnPattern pattern)
    {
        for (int r = 0; r < pattern.Repetitions; r++)
        {
            for (int i = 0; i < pattern.Count; i++)
            {
                Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;
                Vector3 spawnPosition = player.position + new Vector3(randomPoint.x, 0f, randomPoint.y); 

                Instantiate(pattern.EnemyPrefab, spawnPosition, Quaternion.identity);
                
                // ¡LA CLAVE! Usa el SpawnRate ESPECÍFICO del patrón
                yield return new WaitForSeconds(pattern.SpawnRate); 
            }
        }
    }
    
    /// <summary>
    /// Ejecuta la secuencia completa de oleadas (DataOleada).
    /// </summary>
    public IEnumerator GenerarOleadas()
    {
        foreach(DataOleada oleadaActual in oleadas)
        {
            // 1. Pausa antes de la oleada
            yield return new WaitForSeconds(oleadaActual.TiempoEntreOleadas);
            
            // 2. Disparar todos los patrones internos de la oleada en PARALELO
            foreach(SpawnPattern pattern in oleadaActual.SimultaneousPatterns)
            {
                // StartCoroutine dispara el patrón y pasa inmediatamente al siguiente patrón/pausa
                StartCoroutine(ExecutePattern(pattern));
            }
            
            // 3. Esperar la duración total de la oleada antes de la siguiente DataOleada
            yield return new WaitForSeconds(oleadaActual.DurationAfterSpawn);
        }
    }
}