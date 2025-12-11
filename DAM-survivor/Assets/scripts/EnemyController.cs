using UnityEngine;
using System.Collections; 
using Random = UnityEngine.Random; // Usar el Random de Unity

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
    // CONFIGURACIÓN ENEMIGO 4 (ENJAMBRE)
    // -----------------------------------------------------------
    [Header("Comportamiento Enjambre")]
    [Tooltip("El prefab del Enemigo 1 (Zángano) a spawnear. Solo para el Enjambre.")]
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

    // -----------------------------------------------------------
    // ¡NUEVO! BOTÍN DE EXP VARIADO (REQUISITO 14)
    // -----------------------------------------------------------
    [Header("Botín de EXP (Requisito 14)")]
    public GameObject orbeVerdePrefab;  // Valor: 10 EXP (60%)
    public GameObject orbeAzulPrefab;   // Valor: 50 EXP (30%)
    public GameObject orbeDoradoPrefab; // Valor: 100 EXP (10%)
    
    
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
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerStats playerStats = other.GetComponent<PlayerStats>();
        
        if (playerStats != null)
        {
            playerStats.RecibirDmg(damage); 
            
            // Opcional: Si quieres que el enemigo se destruya inmediatamente después de golpear:
            // Morir(); 
        }
    }
    
    // ///////////////////////////////// Lógica de Control y Daño ///////////////////////////////
    
    public void Recibirdano(int danio)
    {
        int danioFinal = danio - defense;
        if (danioFinal < 0) danioFinal = 0;

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
        // ¡NUEVO! Generar el orbe de EXP al morir
        SpawnOrbeDeExperiencia();
        
        Destroy(gameObject);
    }
    
    // -----------------------------------------------------------
    // FUNCIÓN PARA GENERAR EL ORBE SEGÚN PROBABILIDAD (REQUISITO 14)
    // -----------------------------------------------------------
    private void SpawnOrbeDeExperiencia()
    {
        // Si no tenemos al menos el orbe básico, salimos.
        if (orbeVerdePrefab == null) return;
        
        float randomValue = Random.value; // Valor entre 0.0 y 1.0
        
        GameObject prefabToSpawn = null;
        
        // 60% de probabilidad (0.0 a 0.6)
        if (randomValue <= 0.6f)
        {
            prefabToSpawn = orbeVerdePrefab;
        }
        // 30% de probabilidad (0.6 a 0.9)
        else if (randomValue <= 0.9f)
        {
            prefabToSpawn = orbeAzulPrefab;
        }
        // 10% de probabilidad (0.9 a 1.0)
        else
        {
            prefabToSpawn = orbeDoradoPrefab;
        }

        // Instanciamos el orbe si se eligió uno
        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        }
    }

    // ... (El resto de funciones como HitFeedbackRoutine, ApplySlow, etc. se mantienen) ...

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