using UnityEngine;
using System.Collections.Generic;

// [System.Serializable] permite que esta clase sea visible y configurable en el Inspector de Unity
[System.Serializable]
public class SpawnPattern
{
    // El Prefab del enemigo que se generará en este patrón
    public GameObject EnemyPrefab;

    // Número total de este enemigo que se generará
    public int Count = 10;
    
    // Tiempo de espera (cadencia) entre el spawn de cada unidad
    public float SpawnRate = 2f; 
    
    // Cuántas veces se debe repetir esta secuencia completa de 'Count'
    public int Repetitions = 1; 
}

[CreateAssetMenu(fileName = "OleadaNueva", menuName="Oleadas")]
public class DataOleada : ScriptableObject // Clase base para crear activos de datos persistentes
{
    [Header("Configuración de Spawn")]
    // Lista de patrones que se lanzarán SIMULTÁNEAMENTE al inicio de esta oleada
    public List<SpawnPattern> SimultaneousPatterns;
    
    [Header("Control de Secuencia")]
    // Tiempo de espera ANTES de que esta oleada comience a spawnear
    public float TiempoEntreOleadas;
    
    public float DurationAfterSpawn = 30f;
}
