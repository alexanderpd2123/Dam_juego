using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Info del Scriptable Object (SO)
    public EnemyStats Stats; 
    
    // Referencia al jugador
    private GameObject player;

    // Stats propios
    private int maxHP;
    private int currentHP;
    private int damage;
    private int defense;
    private float baseSpeed; 
    private float currentSpeed; 
    
    // -----------------------------------------------------------
    // AÑADIDO: CONFIGURACIÓN ENEMIGO 4 (ENJAMBRE)
    // -----------------------------------------------------------
    [Header("Comportamiento Enjambre")]
    [Tooltip("El prefab del Enemigo 1 (Zángano) a spawnear. Solo para el Enjambre.")]
    public GameObject swarmPrefab; 
    [Tooltip("Número de unidades a spawnear al inicio.")]
    public int swarmCount = 10;
    
    
    // ///////////////////////////////// Funciones Unity ///////////////////////////////
    
    void Awake()
    {
        // Inicializar stats desde el Scriptable Object
        maxHP = Stats.MaxHP;
        currentHP = maxHP;
        damage = Stats.Damage;
        defense = Stats.Defense;
        
        baseSpeed = Stats.Speed;
        currentSpeed = baseSpeed;
    }
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
        // Ejecuta el comportamiento especial (Spawn del Enjambre) al aparecer
        HandleUniqueBehavior();
    }

    void Update()
    {
        if (player != null)
        {
            Vector3 direccion = player.transform.position - transform.position;
            direccion.Normalize();
            
            transform.position += direccion * currentSpeed * Time.deltaTime;
        }
    }
    
    // ///////////////////////////////// Lógica de Control y Daño ///////////////////////////////
    
    public void Recibirdano(int danio)
    {
        int danioFinal = danio - defense;
        if (danioFinal < 0)
        {
            danioFinal = 0;
        }
        currentHP -= danioFinal;
        
        if (currentHP <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        Destroy(gameObject);
    }
    
    public void ApplySlow(float slowPercentage)
    {
        currentSpeed = baseSpeed * (1f - slowPercentage);
    }

    public void RemoveSlow()
    {
        currentSpeed = baseSpeed;
    }
    
    // -----------------------------------------------------------
    // FUNCIÓN DE COMPORTAMIENTO ÚNICO (Para Enemigo 4)
    // -----------------------------------------------------------

    private void HandleUniqueBehavior()
    {
        // Si el 'swarmPrefab' está asignado, este es el Enemigo 4 (Enjambre) y debe spawnear
        if (swarmPrefab != null)
        {
            SpawnSwarmUnits();
        }
    }

    private void SpawnSwarmUnits()
    {
        // Spawnea 'swarmCount' unidades del Enemigo 1 (Zángano) alrededor del Enjambre
        for (int i = 0; i < swarmCount; i++)
        {
            // Pequeño desplazamiento aleatorio para evitar que se superpongan
            Vector3 spawnOffset = Random.insideUnitSphere * 1f;
            spawnOffset.y = 0; // Aseguramos que se mantenga en el plano (si es 3D)
            
            Vector3 spawnPosition = transform.position + spawnOffset;

            // ¡Instancia las unidades del Zángano!
            Instantiate(swarmPrefab, spawnPosition, Quaternion.identity);
        }
    }
}