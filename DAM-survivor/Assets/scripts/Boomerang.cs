using UnityEngine;

public class Boomerang : MonoBehaviour
{
    // --- CONFIGURACIÓN DE PARÁMETROS ---
    [Header("Configuración de Daño")]
    public float damage = 20f; // Daño a infligir
    
    [Header("Configuración de Movimiento")]
    public float speed = 15f; // Velocidad de vuelo
    public float maxDistance = 20f; // Distancia máxima antes de regresar
    public float destructionThreshold = 0.5f; // Distancia para recoger/destruir

    // --- VARIABLES DE ESTADO Y REFERENCIAS ---
    private Transform player; // Referencia al lanzador
    private Vector3 initialPosition; // Punto de origen
    private Vector3 launchDirection; // Dirección de vuelo inicial
    private bool isReturning = false; // Control de fase de vuelo

    // Inicializa referencias y estado al momento del lanzamiento
    public void Initialize(Transform launcher, Vector3 direction)
    {
        player = launcher;
        initialPosition = transform.position;
        launchDirection = direction.normalized;
        isReturning = false; 
    }

    void Update()
    {
        // Limpia el bumerán si el jugador desaparece
        if (player == null)
        {
            Destroy(gameObject);
            return;
        }
        
        // Ejecuta la fase de lanzamiento o de regreso
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
        // Mueve en la dirección inicial
        transform.Translate(launchDirection * speed * Time.deltaTime, Space.World);
        
        // Inicia el regreso si ha alcanzado la distancia máxima
        if (Vector3.Distance(initialPosition, transform.position) >= maxDistance)
        {
            isReturning = true;
        }
    }

    private void HandleReturnPhase()
    {
        // Calcula la dirección hacia el jugador
        Vector3 targetDirection = (player.position - transform.position).normalized;
        
        // Mueve hacia el jugador
        transform.Translate(targetDirection * speed * Time.deltaTime, Space.World);
        
        // Comprueba si ya debe ser recogido
        CheckForPickup();
    }

    private void CheckForPickup()
    {
        // Destruye el objeto si está dentro del umbral de recogida
        if (Vector3.Distance(transform.position, player.position) < destructionThreshold)
        {
            Destroy(gameObject); 
        }
    }

    // Aplica daño al colisionar con un trigger
    private void OnTriggerEnter(Collider other)
    {
        // Intenta obtener el componente del enemigo
        EnemyController enemy = other.GetComponent<EnemyController>();
        
        if (enemy != null)
        {
            // Aplica daño (requiere conversión a int)
            enemy.Recibirdano((int)damage); 
        }
    }
}