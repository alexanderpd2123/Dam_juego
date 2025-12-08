using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // ... (Variables de Spawner se mantienen) ...
    [Header("Configuración de Spawn")]
    [SerializeField]
    private float spawnRadius = 10f;

    [Header("GameObjects")]
    [SerializeField]
    private Transform player;

    [SerializeField]
    private List<DataOleada> oleadas; // Lista de las Oleadas (ahora más complejas)
    
    // ... (Funciones Unity se mantienen) ...

    ///////////////////////////// Funciones Propias ////////////////////

    /// <summary>
    /// Spawnea todos los enemigos definidos en una única DataOleada.
    /// </summary>
    private IEnumerator spawn(DataOleada data)
    {
        // 1. Itera sobre cada tipo de enemigo definido en el Scriptable Object
        foreach (EnemyGroup enemyGroup in data.EnemyGroups)
        {
            // 2. Itera la cantidad definida para ese tipo de enemigo
            for (int i = 0; i < enemyGroup.Count; i++)
            {
                Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;
                Vector3 spawnPosition = player.position + new Vector3(randomPoint.x, 0f, randomPoint.y); 

                // Instancia el enemigo
                Instantiate(enemyGroup.EnemyPrefab, spawnPosition, Quaternion.identity);
                
                // 3. Espera la cadencia entre CADA spawn.
                yield return new WaitForSeconds(data.SpawnRate); 
            }
        }
    }
    
    /// <summary>
    /// Ejecuta la secuencia de oleadas.
    /// </summary>
    public IEnumerator GenerarOleadas()
    {
        foreach(DataOleada oleadaActual in oleadas)
        {
            yield return new WaitForSeconds(oleadaActual.TiempoEntreOleadas);
            // El spawn se ejecuta secuencialmente (primero E1, luego E2, etc.)
            yield return StartCoroutine(spawn(oleadaActual)); 
        }
    }
}