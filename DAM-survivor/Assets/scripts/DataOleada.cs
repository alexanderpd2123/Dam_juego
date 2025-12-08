using UnityEngine;
using System.Collections.Generic;

// Clase serializable para agrupar un Prefab con su cantidad
[System.Serializable]
public class EnemyGroup
{
    [Tooltip("El Prefab del enemigo (E1, E2, E3, o E4) a spawnear.")]
    public GameObject EnemyPrefab;
    
    [Tooltip("El número total de este enemigo que será spawneado en este grupo.")]
    public int Count;
    
    // Si la cadencia es única para este grupo, podrías añadir aquí SpawnRate,
    // pero para simplicidad, mantendremos SpawnRate en la clase principal.
}

[CreateAssetMenu(fileName = "OleadaNueva", menuName="Oleadas")]
public class DataOleada : ScriptableObject
{
    [Header("Configuración de Spawn")]
    [Tooltip("Lista de enemigos y cantidades a spawnear en esta oleada.")]
    public List<EnemyGroup> EnemyGroups; // ¡Ahora permite múltiples enemigos!
    
    [Tooltip("La cadencia de spawn: el tiempo de espera (en segundos) entre el spawn de CUALQUIER enemigo en esta oleada.")]
    public float SpawnRate;
    
    [Header("Control de Secuencia")]
    [Tooltip("El tiempo de espera (en segundos) antes de que ESTA OLEADA comience a ejecutarse.")]
    public float TiempoEntreOleadas;
}