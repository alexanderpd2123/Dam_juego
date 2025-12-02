using UnityEngine;

public class OrbitalShield : MonoBehaviour
{
    public float damage = 10f;             // Daño, es float para el escalado
    
    public Transform playerTransform;       // Público para el lanzador
    private float orbitRadius;              
    private float orbitSpeed;               
    private float angle;                    

    public void Initialize(Transform center, float radius, float speed, float initialAngle)
    {
        playerTransform = center;
        orbitRadius = radius;
        orbitSpeed = speed;
        angle = initialAngle * Mathf.Deg2Rad; 
    }

    void Update()
    {
        if (playerTransform == null)
        {
            Destroy(gameObject);
            return;
        }
        // ... (Lógica de movimiento circular) ...
        angle += orbitSpeed * Time.deltaTime * Mathf.Deg2Rad; 

        float x = Mathf.Cos(angle) * orbitRadius;
        float z = Mathf.Sin(angle) * orbitRadius; 

        transform.position = playerTransform.position + new Vector3(x, 0f, z);
    }

    // APLICACIÓN DE DAÑO: Colisión y conversión
    private void OnTriggerEnter(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        
        if (enemy != null)
        {
            // CRUCIAL: Conversión de float a int
            enemy.Recibirdano((int)damage);
        }
    }
}