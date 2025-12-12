using UnityEngine;
using System.Collections; 
using Random = UnityEngine.Random; 

public class EnemyController : MonoBehaviour
{
    // --- Referencias y Stats ---
    public EnemyStats Stats; // Referencia al Scriptable Object (SO) con las estadísticas base
    private GameObject player; // Referencia al jugador para el movimiento
    
    // Stats obtenidos del SO
    private int maxHP;
    private int currentHP;
    private int damage; 
    private int defense;
    private float baseSpeed; 
    private float currentSpeed; // Velocidad actual (puede ser modificada por ralentización)
    
    // -----------------------------------------------------------
    // CONFIGURACIÓN ENEMIGO 4 (ENJAMBRE)
    // -----------------------------------------------------------
    [Header("Comportamiento Enjambre")]
    public GameObject swarmPrefab; // Prefab del enemigo más pequeño del enjambre
    public int swarmCount = 10; // Número de unidades a generar

    // -----------------------------------------------------------
    // FEEDBACK VISUAL DE GOLPE
    // -----------------------------------------------------------
    [Header("Feedback Visual")]
    public Color hitColor = Color.red; // Color al recibir daño
    public float hitDuration = 0.1f; // Duración del cambio de color

    private Renderer enemyRenderer; // Componente para cambiar el color
    private Color originalColor; // Color base para restaurar el material

    // -----------------------------------------------------------
    // BOTÍN DE EXP VARIADO (REQUISITO 14)
    // -----------------------------------------------------------
    [Header("Botín de EXP")]
    public GameObject orbeVerdePrefab;  // Prefab de EXP común (60% prob.)
    public GameObject orbeAzulPrefab;   // Prefab de EXP medio (30% prob.)
    public GameObject orbeDoradoPrefab; // Prefab de EXP raro (10% prob.)
    
    
    // ///////////////////////////////// Funciones Unity ///////////////////////////////
    
    void Awake()
    {
        // Inicializa las estadísticas internas tomando los valores del Scriptable Object
        maxHP = Stats.MaxHP;
        currentHP = maxHP;
        damage = Stats.Damage; 
        defense = Stats.Defense;
        baseSpeed = Stats.Speed;
        currentSpeed = baseSpeed;

        // Prepara el renderer para el feedback visual
        enemyRenderer = GetComponentInChildren<Renderer>();
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }
    }
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Encuentra al jugador
        
        HandleUniqueBehavior(); // Ejecuta comportamientos especiales (como el enjambre)
    }

    void Update()
    {
        if (player != null)
        {
            // Calcula la dirección hacia el jugador
            Vector3 direccion = player.transform.position - transform.position;
            direccion.Normalize();
            
            // Mueve al enemigo hacia el jugador (comportamiento de persecución simple)
            transform.position += direccion * currentSpeed * Time.deltaTime;
        }
    }
    
    // Colisión con el jugador (si el collider es un trigger)
    private void OnTriggerEnter(Collider other)
    {
        PlayerStats playerStats = other.GetComponent<PlayerStats>();
        
        if (playerStats != null)
        {
            playerStats.RecibirDmg(damage); // Aplica daño al jugador
        }
    }
    
    // ///////////////////////////////// Lógica de Control y Daño ///////////////////////////////
    
    // Función pública para recibir daño desde proyectiles u otras fuentes
    public void Recibirdano(int danio)
    {
        // Aplica la defensa: el daño final nunca es negativo
        int danioFinal = danio - defense;
        if (danioFinal < 0) danioFinal = 0;

        currentHP -= danioFinal;
        
        if (currentHP > 0)
        {
            StartCoroutine(HitFeedbackRoutine()); // Muestra feedback visual si sigue vivo
        }
        else
        {
            Morir(); // Inicia la secuencia de muerte
        }
    }

    private void Morir()
    {
        SpawnOrbeDeExperiencia(); // Genera el orbe de EXP (botín)
        Destroy(gameObject); // Destruye el enemigo
    }
    
    // -----------------------------------------------------------
    // FUNCIÓN PARA GENERAR EL ORBE SEGÚN PROBABILIDAD 
    // -----------------------------------------------------------
    private void SpawnOrbeDeExperiencia()
    {
        if (orbeVerdePrefab == null) return;
        
        float randomValue = Random.value; // Genera un valor aleatorio entre 0.0 y 1.0
        
        GameObject prefabToSpawn = null;
        
        // Asigna el prefab según el rango de probabilidad
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

        if (prefabToSpawn != null)
        {
            // Instancia el orbe seleccionado en la posición del enemigo
            Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        }
    }

    // Rutina que cambia temporalmente el color para indicar que el enemigo fue golpeado
    private IEnumerator HitFeedbackRoutine()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = hitColor;
            yield return new WaitForSeconds(hitDuration);
            enemyRenderer.material.color = originalColor;
        }
    }
    
    // Aplica ralentización reduciendo la velocidad actual
    public void ApplySlow(float slowPercentage)
    {
        currentSpeed = baseSpeed * (1f - slowPercentage);
    }

    // Restaura la velocidad a su valor base
    public void RemoveSlow()
    {
        currentSpeed = baseSpeed;
    }
    
    // Llama a la función de comportamiento único (ej. generar enjambre)
    private void HandleUniqueBehavior()
    {
        if (swarmPrefab != null)
        {
            SpawnSwarmUnits();
        }
    }

    // Genera las unidades del enjambre con un pequeño desplazamiento aleatorio
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