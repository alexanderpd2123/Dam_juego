using UnityEngine;

[CreateAssetMenu(fileName = "OleadaNueva", menuName="Oleadas")]
public class DataOleada : ScriptableObject
{
    [Header("Propiedades de la Oleada")]
    public GameObject EnemyPrefab;
    public float SpawnRate;
    public int CantidadDeEnemigos;
    public float TiempoEntreOleadas;
}