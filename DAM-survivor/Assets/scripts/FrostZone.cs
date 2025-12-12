using UnityEngine;
using System.Collections;

public class FrostZone : MonoBehaviour
{
    // --- Configuración de Daño y Efecto ---
    public float damagePerTick = 5f; // Daño infligido por cada intervalo (público para escalado)
    public float damageTickInterval = 0.5f; // Frecuencia con la que se aplica el daño
    public float slowAmount = 0.3f; // Cantidad de ralentización (ej. 0.3 = 30% más lento)

    private Transform playerTransform; // Referencia al Transform del jugador
    
    // Inicializa la referencia al jugador
    public void Initialize(Transform launcher)
    {
        playerTransform = launcher;
    }

    // Mantiene la zona de congelación centrada en la posición del jugador
    void Update()
    {
        if (playerTransform != null)
        {
            transform.position = playerTransform.position;
        }
    }
    
    // --- Lógica de Entrada a la Zona ---
    private void OnTriggerEnter(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.ApplySlow(slowAmount); 
            StartCoroutine(DamageOverTime(enemy)); 
        }
    }

    // --- Lógica de Salida de la Zona ---
    private void OnTriggerExit(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.RemoveSlow(); 
            StopAllCoroutines(); 
        }
    }

    // --- Corrutina de Daño Periódico (DoT) ---
    private IEnumerator DamageOverTime(EnemyController enemy)
    {
        // El bucle se mantiene mientras el enemigo siga existiendo
        while (enemy != null)
        {
            // Aplica el daño por tick. CRUCIAL: Conversión de float a int
            enemy.Recibirdano((int)damagePerTick); 
            
            // Espera el intervalo de tiempo antes del siguiente tick de daño
            yield return new WaitForSeconds(damageTickInterval);
        }
       
    }
}