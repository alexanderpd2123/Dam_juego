using UnityEngine;

public class OrbitalShield : MonoBehaviour
{
    // --- Configuración y Estado ---
    public float damage = 10f;             // Daño infligido (se escala desde LanzadorArma)
    
    public Transform playerTransform;       // Referencia al centro de la órbita (el jugador)
    private float orbitRadius;              // Radio de la órbita
    private float orbitSpeed;               // Velocidad angular (grados/segundo)
    private float angle;                    // Ángulo actual en radianes

    // Inicializa los parámetros de órbita y convierte el ángulo inicial a radianes
    public void Initialize(Transform center, float radius, float speed, float initialAngle)
    {
        playerTransform = center;
        orbitRadius = radius;
        orbitSpeed = speed;
        angle = initialAngle * Mathf.Deg2Rad; // Convierte grados iniciales a radianes
    }

    void Update()
    {
        // Guardián: Destruye si el centro (jugador) desaparece
        if (playerTransform == null)
        {
            Destroy(gameObject);
            return;
        }
        
        // 1. Incrementa el ángulo de rotación usando la velocidad (convertida a radianes)
        angle += orbitSpeed * Time.deltaTime * Mathf.Deg2Rad; 

        // 2. Cálculo de posición: Usa trigonometría para determinar las coordenadas X y Z de la órbita
        float x = Mathf.Cos(angle) * orbitRadius;
        float z = Mathf.Sin(angle) * orbitRadius; 

        // 3. Aplica la nueva posición: Mueve el orbe alrededor del jugador
        transform.position = playerTransform.position + new Vector3(x, 0f, z);
    }

    // APLICACIÓN DE DAÑO: Detección de Colisión (solo si es un trigger)
    private void OnTriggerEnter(Collider other)
    {
        // Intenta obtener el componente del enemigo
        EnemyController enemy = other.GetComponent<EnemyController>();
        
        if (enemy != null)
        {
            // Aplica el daño. 
            enemy.Recibirdano((int)damage);
        }
    }
}