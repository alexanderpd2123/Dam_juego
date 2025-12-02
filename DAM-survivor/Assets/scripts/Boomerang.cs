using UnityEngine;

public class Boomerang : MonoBehaviour
{
    [Header("Configuración de Daño")]
    public float damage = 20f; // Ahora es público para el escalado
    
    [Header("Configuración de Movimiento")]
    public float speed = 15f;
    public float maxDistance = 20f;
    public float destructionThreshold = 0.5f;

    private Transform player;
    private Vector3 initialPosition;
    private Vector3 launchDirection;
    private bool isReturning = false;

    public void Initialize(Transform launcher, Vector3 direction)
    {
        player = launcher;
        initialPosition = transform.position;
        launchDirection = direction.normalized;
        isReturning = false;
    }

    void Update()
    {
        if (player == null)
        {
            Destroy(gameObject);
            return;
        }
        if (!isReturning)
        {
            HandleLaunchPhase();
        }
        else
        {
            HandleReturnPhase();
        }
    }

    private void HandleLaunchPhase()
    {
        transform.Translate(launchDirection * speed * Time.deltaTime, Space.World);
        if (Vector3.Distance(initialPosition, transform.position) >= maxDistance)
        {
            isReturning = true;
        }
    }

    private void HandleReturnPhase()
    {
        Vector3 targetDirection = (player.position - transform.position).normalized;
        transform.Translate(targetDirection * speed * Time.deltaTime, Space.World);
        CheckForPickup();
    }

    private void CheckForPickup()
    {
        if (Vector3.Distance(transform.position, player.position) < destructionThreshold)
        {
            Destroy(gameObject); 
        }
    }

    // APLICACIÓN DE DAÑO: Colisión y conversión
    private void OnTriggerEnter(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        
        if (enemy != null)
        {
            // CRUCIAL: Conversión de float a int para la función Recibirdano
            enemy.Recibirdano((int)damage); 
        }
    }
}