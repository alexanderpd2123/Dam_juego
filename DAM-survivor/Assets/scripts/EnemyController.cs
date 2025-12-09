using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    // Info del Scriptable Object (SO)
    public EnemyStats Stats; 
    
    // Referencia al jugador
    private GameObject player;

    // Stats propios
    private int maxHP;
    private int currentHP;
    private int damage; // Daño que aplica al jugador
    private int defense;
    private float baseSpeed; 
    private float currentSpeed; 
    
    // -----------------------------------------------------------
    // CONFIGURACIÓN ENEMIGO 4 (ENJAMBRE)
    // -----------------------------------------------------------
    [Header("Comportamiento Enjambre")]
    [Tooltip("El prefab del Enemigo 1 (Zángano) a spawnear.")]
    public GameObject swarmPrefab; 
    [Tooltip("Número de unidades a spawnear al inicio.")]
    public int swarmCount = 10;

    // -----------------------------------------------------------
    // FEEDBACK VISUAL DE GOLPE
    // -----------------------------------------------------------
    [Header("Feedback Visual")]
    public Color hitColor = Color.red;
    public float hitDuration = 0.1f;

    private Renderer enemyRenderer;
    private Color originalColor;
    
    
    // ///////////////////////////////// Funciones Unity ///////////////////////////////
    
    void Awake()
    {
        // Inicializar stats desde el Scriptable Object
        maxHP = Stats.MaxHP;
        currentHP = maxHP;
        damage = Stats.Damage; // ¡Aquí se carga el daño que aplica!
        defense = Stats.Defense;
        
        baseSpeed = Stats.Speed;
        currentSpeed = baseSpeed;

        // Inicializar Renderer para el Feedback de Golpe
        enemyRenderer = GetComponentInChildren<Renderer>();
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }
    }
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
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
    
    // -----------------------------------------------------------
    // ¡NUEVO! LÓGICA DE DAÑO AL JUGADOR
    // -----------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si la colisión fue con el Jugador
        PlayerStats playerStats = other.GetComponent<PlayerStats>();
        
        if (playerStats != null)
        {
            // Aplicar daño al jugador usando la stat de daño del enemigo.
            playerStats.RecibirDmg(damage); 
            
            // Nota: Aquí se podría añadir un cooldown de daño para el enemigo.
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
        
        if (currentHP > 0)
        {
            StartCoroutine(HitFeedbackRoutine()); 
        }
        else
        {
            Morir();
        }
    }

    private void Morir()
    {
        Destroy(gameObject);
    }
    
    // ... (Corrutinas de Feedback y Slow se mantienen) ...

    private IEnumerator HitFeedbackRoutine()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = hitColor;
            yield return new WaitForSeconds(hitDuration);
            enemyRenderer.material.color = originalColor;
        }
    }
    
    public void ApplySlow(float slowPercentage)
    {
        currentSpeed = baseSpeed * (1f - slowPercentage);
    }

    public void RemoveSlow()
    {
        currentSpeed = baseSpeed;
    }
    
    private void HandleUniqueBehavior()
    {
        if (swarmPrefab != null)
        {
            SpawnSwarmUnits();
        }
    }

    private void SpawnSwarmUnits()
    {
        for (int i = 0; i < swarmCount; i++)
        {
            Vector3 spawnOffset = Random.insideUnitSphere * 1f;
            spawnOffset.y = 0; 
            Vector3 spawnPosition = transform.position + spawnOffset;
            Instantiate(swarmPrefab, spawnPosition, Quaternion.identity);
        }
    }
}