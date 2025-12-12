using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Define los tipos de arma para el inventario
public enum WeaponType { 
    Boomerang, 
    FrostZone, // Zona de Escarcha (AoE persistente)
    OrbitalShield // Escudo Orbital (proyectiles giratorios)
}

// Clase para rastrear el estado de un arma en el inventario
[System.Serializable]
public class WeaponInventoryItem
{
    public WeaponType type; // Tipo de arma
    public bool isOwned; // Si el jugador posee esta arma
    public int level; // Nivel de la propia arma (aunque el escalado usa el nivel del jugador)
}


public class LanzadorArma : MonoBehaviour
{
    // --- Configuración de Escalado ---
    public const int MAX_LEVEL = 10; // Límite máximo de nivel
    [Header("Escalado de Poder")]
    public int level = 1; // Nivel actual del jugador (clave para el escalado)
    
    // Factores de escalado por nivel
    public float cooldownReductionPerLevel = 0.05f; // Reducción de Cooldown del Bumerán por nivel (ej. 5%)
    public float damageBonusPerLevel = 5f; // Daño base añadido por nivel
    
    [Header("Daño Base de Armas")]
    public float boomerangBaseDamage = 20f; // Daño base del Bumerán
    public float frostZoneBaseDamage = 5f; // Daño base de la Zona de Escarcha
    public float orbitalShieldBaseDamage = 10f; // Daño base del Escudo Orbital
    
    // --- Configuración Bumerán ---
    public GameObject projectilePrefab; // Prefab del Bumerán
    public Transform launchPoint; // Punto desde donde se lanza
    public float launchForce = 20f; // Fuerza de lanzamiento (no usada en Translate del Bumerán.cs)
    public float fireRate = 0.5f; // Cadencia base (antes de reducción por nivel)
    
    // --- Variables de Inventario ---
    [Header("Inventario de Armas")]
    public List<WeaponInventoryItem> inventory = new List<WeaponInventoryItem>(); // Lista de las armas poseídas
    
    // --- Configuración Zona de Escarcha ---
    [Header("Configuración Zona de Escarcha")]
    public GameObject frostZonePrefab; 
    private GameObject activeFrostZone = null; // Referencia a la zona activa
    private bool frostZoneActive = false; // Bandera de estado
    
    // --- Configuración Escudo Orbital ---
    [Header("Configuración Escudo Orbital")]
    public GameObject orbitalPrefab;         
    public float orbitalRadius = 3f; // Radio de órbita        
    public float orbitalSpeed = 150f; // Velocidad angular de la órbita        
    private bool shieldActive = false; // Bandera de estado       

    private Coroutine autoFireCoroutine; // Referencia a la corrutina de disparo automático
    
    private int previousLevel = 1; // Para detectar cambios de nivel

    void Awake()
    {
        // Inicialización del inventario: Solo el Bumerán se posee al inicio
        inventory.Add(new WeaponInventoryItem { type = WeaponType.Boomerang, isOwned = true, level = 1 });
        inventory.Add(new WeaponInventoryItem { type = WeaponType.FrostZone, isOwned = false, level = 0 });
        inventory.Add(new WeaponInventoryItem { type = WeaponType.OrbitalShield, isOwned = false, level = 0 });
    }

    void Start()
    {
        // Activa el disparo automático del Bumerán si está poseído
        if (IsWeaponOwned(WeaponType.Boomerang) && projectilePrefab != null)
        {
            autoFireCoroutine = StartCoroutine(AutoFireRoutine());
        }
    }

    void LateUpdate()
    {
        // Limita el nivel para que no exceda el MAX_LEVEL
        if (level > MAX_LEVEL)
        {
            level = MAX_LEVEL;
        }

        // Detecta si el nivel del jugador ha cambiado
        if (level != previousLevel)
        {
            previousLevel = level;
            UpdatePersistentWeaponStats(); // Actualiza estadísticas de armas persistentes

            // Reinicia el disparo automático para aplicar el nuevo FireRate escalado
            if (IsWeaponOwned(WeaponType.Boomerang))
            {
                if (autoFireCoroutine != null)
                {
                    StopCoroutine(autoFireCoroutine);
                }
                autoFireCoroutine = StartCoroutine(AutoFireRoutine());
            }
        }
    }
    
    // ----------------------------------------------------------------------------------
    // MÉTODOS PÚBLICOS PARA DEBUG Y OBTENCIÓN DE ARMAS
    // ----------------------------------------------------------------------------------
    
    /// <summary>
    /// Otorga una nueva arma al jugador (usado típicamente para Debug o selección de nivel).
    /// </summary>
    public void GrantWeapon(WeaponType typeToGrant)
    {
        WeaponInventoryItem item = inventory.Find(w => w.type == typeToGrant);
        
        if (item != null && !item.isOwned)
        {
            item.isOwned = true;
            item.level = 1; 

            // Activa el arma si es persistente (Escudo, Zona) o si requiere reiniciar corrutina (Bumerán)
            if (typeToGrant == WeaponType.OrbitalShield)
            {
                ActivateOrbitalShield();
            }
            else if (typeToGrant == WeaponType.FrostZone)
            {
                ActivateFrostZone(); 
            }
            else if (typeToGrant == WeaponType.Boomerang && autoFireCoroutine == null)
            {
                 autoFireCoroutine = StartCoroutine(AutoFireRoutine());
            }
        }
    }

    // ----------------------------------------------------------------------------------
    // LÓGICA DE ESCALADO Y ACTIVACIÓN
    // ----------------------------------------------------------------------------------

    // Corrutina para el disparo automático (Bumerán)
    private IEnumerator AutoFireRoutine()
    {
        while (true)
        {
            float scaledFireRate = CalculateScaledFireRate(); // Calcula la cadencia escalada
            yield return new WaitForSeconds(scaledFireRate); // Espera la cadencia
            LaunchWeapon(); // Lanza el proyectil
        }
    }
    
    // Calcula la tasa de disparo final aplicando la reducción por nivel
    private float CalculateScaledFireRate()
    {
        float maxReduction = 1f - Mathf.Epsilon; // Casi 100% de reducción
        // Calcula la reducción total, limitada por 'maxReduction'
        float totalReduction = Mathf.Min( (level - 1) * cooldownReductionPerLevel, maxReduction);
        // Aplica el porcentaje de reducción a la cadencia base
        return fireRate * (1f - totalReduction);
    }
    
    // Calcula el daño final aplicando el bonus por nivel
    private float CalculateScaledDamage(float baseDamage)
    {
        float damageIncrease = (level - 1) * damageBonusPerLevel;
        return baseDamage + damageIncrease;
    }

    // Lanza el proyectil Bumerán con el daño escalado
    private void LaunchWeapon()
    {
        if (projectilePrefab == null || launchPoint == null) return;

        if (IsWeaponOwned(WeaponType.Boomerang))
        {
            float scaledDamage = CalculateScaledDamage(boomerangBaseDamage);
            Vector3 direction = transform.forward; 
            
            GameObject newObject = Instantiate(projectilePrefab, launchPoint.position, launchPoint.rotation);

            Boomerang boomerangComponent = newObject.GetComponent<Boomerang>();
            if (boomerangComponent != null)
            {
                boomerangComponent.damage = scaledDamage; // Sobrescribe el daño con el valor escalado
                boomerangComponent.Initialize(this.transform, direction); // Inicializa el movimiento
            }
        }
    }

    // Método a implementar para actualizar las propiedades de armas que ya están activas (Ej: tamaño de la Zona de Escarcha)
    private void UpdatePersistentWeaponStats()
    {
        // Lógica de escalado de armas persistentes...
    }
    
    // Activa la Zona de Escarcha y escala su daño
    public void ActivateFrostZone() 
    {
        if (frostZonePrefab == null || frostZoneActive) return;

        GameObject zone = Instantiate(frostZonePrefab, transform.position, Quaternion.identity);

        FrostZone frostZoneComponent = zone.GetComponent<FrostZone>();
        if (frostZoneComponent != null)
        {
            frostZoneComponent.damagePerTick = CalculateScaledDamage(frostZoneBaseDamage); // Escala daño
            frostZoneComponent.Initialize(this.transform); // La ata al jugador
            activeFrostZone = zone;
            frostZoneActive = true;
        }
    }

    // Activa y configura el Escudo Orbital (spawnea 3 orbes)
    public void ActivateOrbitalShield() 
    {
        if (orbitalPrefab == null || shieldActive) return;
        
        float angleSpacing = 360f / 3f; // Calcula el ángulo entre los 3 orbes
        
        for (int i = 0; i < 3; i++) // Itera 3 veces para 3 orbes
        {
            float initialAngle = i * angleSpacing; // Calcula el ángulo inicial de cada orbe
            
            GameObject orb = Instantiate(orbitalPrefab, transform.position, Quaternion.identity);
            
            OrbitalShield orbitalComponent = orb.GetComponent<OrbitalShield>();
            
            if (orbitalComponent != null)
            {
                orbitalComponent.damage = CalculateScaledDamage(orbitalShieldBaseDamage); // Escala daño
                
                orbitalComponent.Initialize(
                    center: this.transform, 
                    radius: orbitalRadius, 
                    speed: orbitalSpeed, 
                    initialAngle: initialAngle
                );
            }
        }
        
        shieldActive = true;
    }

    // Función auxiliar para verificar si un arma está poseída
    private bool IsWeaponOwned(WeaponType type)
    {
        return inventory.Find(w => w.type == type)?.isOwned ?? false;
    }
}