using UnityEngine;
using System.Collections;

public class LanzadorArma : MonoBehaviour
{
    // AÑADIDO: Variable de Nivel para el escalado manual
    [Header("Escalado de Poder Manual")]
    [Tooltip("Controla el nivel actual del arma. Cambia manualmente en el Inspector para probar el escalado.")]
    public int level = 1;
    [Tooltip("Porcentaje de reducción del Cooldown por nivel (ej: 0.05 para 5% de reducción).")]
    public float cooldownReductionPerLevel = 0.05f; 
    [Tooltip("Daño base que se añade por nivel (ej: 5 por nivel).")]
    public float damageBonusPerLevel = 5f;
    
    // VARIABLES DE DAÑO BASE (Necesarias para el cálculo de escalado)
    [Header("Daño Base de Armas")]
    public float boomerangBaseDamage = 20f;
    public float frostZoneBaseDamage = 5f;
    public float orbitalShieldBaseDamage = 10f;
    
    // --- Configuración General ---
    public GameObject projectilePrefab; // Usado para el DISPARO AUTOMÁTICO (ej. Bumerán)
    public Transform launchPoint; 
    public float launchForce = 20f; 
    
    public float fireRate = 0.5f; 
    
    // --- Configuración Zona de Escarcha ---
    [Header("Configuración Zona de Escarcha")]
    public GameObject frostZonePrefab; // Prefab para la activación única
    private GameObject activeFrostZone = null; 
    private bool frostZoneActive = false; 

    // --- Configuración Escudo Orbital ---
    [Header("Configuración Escudo Orbital")]
    public GameObject orbitalPrefab;         // Prefab para la activación única
    public float orbitalRadius = 3f;         
    public float orbitalSpeed = 150f;        
    private bool shieldActive = false;       

    private Coroutine autoFireCoroutine;
    
    private int previousLevel = 1;

    void Start()
    {
        // 1. ACTIVACIÓN INMEDIATA DEL ESCUDO ORBITAL
        if (orbitalPrefab != null && !shieldActive)
        {
            ActivateOrbitalShield();
        }
        
        // 2. ACTIVACIÓN INMEDIATA DE LA ZONA DE ESCARCHA
        if (frostZonePrefab != null && !frostZoneActive)
        {
            ActivateFrostZone();
        }
        
        // 3. INICIO DEL DISPARO AUTOMÁTICO (SOLO para proyectiles repetibles)
        if (projectilePrefab != null && projectilePrefab.GetComponent<Boomerang>() != null)
        {
            autoFireCoroutine = StartCoroutine(AutoFireRoutine());
        }
    }

    void LateUpdate()
    {
        if (level != previousLevel)
        {
            // Forzamos la actualización de todas las propiedades persistentes al cambiar el nivel
            previousLevel = level;
            UpdatePersistentWeaponStats();

            // Reiniciamos la corrutina para que use el nuevo FireRate
            if (autoFireCoroutine != null)
            {
                StopCoroutine(autoFireCoroutine);
                autoFireCoroutine = StartCoroutine(AutoFireRoutine());
            }
        }
    }
    
    private IEnumerator AutoFireRoutine()
    {
        while (true)
        {
            float scaledFireRate = CalculateScaledFireRate();
            yield return new WaitForSeconds(scaledFireRate);
            LaunchWeapon();
        }
    }
    
    private float CalculateScaledFireRate()
    {
        float maxReduction = 1f - Mathf.Epsilon; 
        float totalReduction = Mathf.Min( (level - 1) * cooldownReductionPerLevel, maxReduction);
        return fireRate * (1f - totalReduction);
    }
    
    // Calcula el daño base escalado para CUALQUIER arma
    private float CalculateScaledDamage(float baseDamage)
    {
        float damageIncrease = (level - 1) * damageBonusPerLevel;
        return baseDamage + damageIncrease;
    }

    private void LaunchWeapon()
    {
        // El daño escalado se calcula usando el daño base del Bumerán
        float scaledDamage = CalculateScaledDamage(boomerangBaseDamage);
        
        if (projectilePrefab == null || launchPoint == null)
        {
            return;
        }

        Vector3 direction = transform.forward; 
        
        GameObject newObject = Instantiate(projectilePrefab, launchPoint.position, launchPoint.rotation);

        // --- Caso BUMERÁN ---
        Boomerang boomerangComponent = newObject.GetComponent<Boomerang>();
        if (boomerangComponent != null)
        {
            boomerangComponent.damage = scaledDamage;
            boomerangComponent.Initialize(this.transform, direction); 
            return; 
        }

        // --- PREVENCIÓN DE CICLO DE DISPARO ---
        if (newObject.GetComponent<FrostZone>() != null || newObject.GetComponent<OrbitalShield>() != null)
        {
            Destroy(newObject);
            return;
        }

        // 3. Inicialización General (Proyectil simple) - CORREGIDO linearVelocity
        Rigidbody rb = newObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * launchForce; 
        }
    }

    // ----------------------------------------------------------------------------------
    // FUNCIÓN DE ESCALADO para armas persistentes (AoE, Orbital) - CORREGIDA
    // ----------------------------------------------------------------------------------
    private void UpdatePersistentWeaponStats()
    {
        // 1. ESCALADO DE ZONA DE ESCARCHA
        if (activeFrostZone != null)
        {
            FrostZone frostZoneComponent = activeFrostZone.GetComponent<FrostZone>();
            if (frostZoneComponent != null)
            {
                float scaledDamage = CalculateScaledDamage(frostZoneBaseDamage);
                frostZoneComponent.damagePerTick = scaledDamage; 
            }
        }

        // 2. ESCALADO DE ESCUDO ORBITAL
        // CORREGIDO: Usamos FindObjectsByType para evitar el warning/error de obsoletismo
        OrbitalShield[] orbs = FindObjectsByType<OrbitalShield>(FindObjectsSortMode.None);
        
        foreach (OrbitalShield orb in orbs)
        {
            // CORREGIDO: Comprobamos si es nuestro orbe (playerTransform debe ser público en OrbitalShield.cs)
            if (orb.playerTransform == this.transform)
            {
                float scaledDamage = CalculateScaledDamage(orbitalShieldBaseDamage);
                orb.damage = scaledDamage;
            }
        }
    }
    
    // ----------------------------------------------------------------------------------
    // FUNCIÓN DE ACTIVACIÓN ÚNICA: ZONA DE ESCARCHA
    // ----------------------------------------------------------------------------------
    private void ActivateFrostZone()
    {
        if (frostZonePrefab == null) return;
        if (activeFrostZone != null) Destroy(activeFrostZone); 

        GameObject zone = Instantiate(frostZonePrefab, transform.position, Quaternion.identity);

        FrostZone frostZoneComponent = zone.GetComponent<FrostZone>();
        if (frostZoneComponent != null)
        {
            // ESCALADO INICIAL: Aplicar el daño escalado al crearse
            frostZoneComponent.damagePerTick = CalculateScaledDamage(frostZoneBaseDamage);
            
            frostZoneComponent.Initialize(this.transform);
            activeFrostZone = zone;
            frostZoneActive = true;
        }
    }

    // ----------------------------------------------------------------------------------
    // FUNCIÓN DE ACTIVACIÓN ÚNICA: ESCUDO ORBITAL
    // ----------------------------------------------------------------------------------
    private void ActivateOrbitalShield()
    {
        if (orbitalPrefab == null) return;
        
        float angleSpacing = 360f / 3f; 
        
        for (int i = 0; i < 3; i++)
        {
            float initialAngle = i * angleSpacing;
            
            GameObject orb = Instantiate(orbitalPrefab, transform.position, Quaternion.identity);
            
            OrbitalShield orbitalComponent = orb.GetComponent<OrbitalShield>();
            
            if (orbitalComponent != null)
            {
                // ESCALADO INICIAL: Aplicar el daño escalado al crearse
                orbitalComponent.damage = CalculateScaledDamage(orbitalShieldBaseDamage);
                
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
}