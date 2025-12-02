using UnityEngine;
using System.Collections;

public class FrostZone : MonoBehaviour
{
    public float damagePerTick = 5f; // Debe ser público para el escalado
    public float damageTickInterval = 0.5f;
    public float slowAmount = 0.3f; 

    private Transform playerTransform;
    
    // ... (Inicialización y Update de seguimiento) ...
    public void Initialize(Transform launcher)
    {
        playerTransform = launcher;
    }

    void Update()
    {
        if (playerTransform != null)
        {
            transform.position = playerTransform.position;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.ApplySlow(slowAmount);
            StartCoroutine(DamageOverTime(enemy));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.RemoveSlow();
            StopAllCoroutines(); 
        }
    }

    private IEnumerator DamageOverTime(EnemyController enemy)
    {
        while (enemy != null)
        {
            // CRUCIAL: Conversión de float a int
            enemy.Recibirdano((int)damagePerTick); 
            yield return new WaitForSeconds(damageTickInterval);
        }
    }
}