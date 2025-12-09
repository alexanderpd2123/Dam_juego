using UnityEngine;
using System.Collections.Generic;

// --- CLASE 1: Define un patrón de spawn único (Ej: 60 E1 cada 2s) ---
[System.Serializable]
public class SpawnPattern
{
    [Tooltip("El Prefab del enemigo (E1, E2, etc.) que se va a spawnear.")]
    public GameObject EnemyPrefab;

    [Tooltip("El número total de este enemigo que será spawneado.")]
    public int Count = 10;
    
    [Tooltip("La cadencia de spawn: el tiempo de espera (en segundos) entre cada enemigo de este patrón.")]
    public float SpawnRate = 2f; 
    
    [Tooltip("Número de veces que este patrón se debe repetir (Ej: 4x E1).")]
    public int Repetitions = 1; 
}

[CreateAssetMenu(fileName = "OleadaNueva", menuName="Oleadas")]
public class DataOleada : ScriptableObject
{
    [Header("Configuración de Spawn")]
    [Tooltip("Lista de patrones de spawn que se ejecutarán SIMULTÁNEAMENTE al inicio de esta oleada.")]
    public List<SpawnPattern> SimultaneousPatterns; // ¡Ahora usa esta lista!
    
    [Header("Control de Secuencia")]
    [Tooltip("El tiempo de espera (en segundos) antes de que ESTA OLEADA comience a ejecutarse.")]
    public float TiempoEntreOleadas;
    
    [Tooltip("Tiempo de espera después de que se lancen todos los patrones antes de que el spawner pase a la siguiente DataOleada.")]
    public float DurationAfterSpawn = 30f;
}