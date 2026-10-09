using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GestorPowerUpsCentral : MonoBehaviour
{
    public static GestorPowerUpsCentral Instance;

    [System.Serializable]
    public class PowerUpConfigPersonalizada
    {
        public PotenciadorItem itemEnEscena;    // El objeto físico en el mapa
        [Header("Valor específico para este objeto")]
        [Tooltip("Multiplicador de gravedad (ej. 0.2 para baja gravedad estilo luna)")]
        public float gravedadPersonalizada = 0.2f; 
    }

    [Header("Lista de Power-Ups con valores independientes")]
    [SerializeField] private List<PowerUpConfigPersonalizada> powerUpsConfig = new List<PowerUpConfigPersonalizada>();

    [Header("Tiempos Globales")]
    public float duracionEfecto = 5f;
    public float tiempoCooldown = 3f;

    [Header("Animación Global")]
    public float velocidadRotacion = 50f;
    public float amplitudFlote = 0.5f;
    public float frecuenciaFlote = 2f;

    void Awake()
    {
        InicializarInstancia();
    }

    // ==========================================
    // MÉTODOS AUXILIARES
    // ==========================================
    private void InicializarInstancia()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ActivarCicloItem(PotenciadorItem item, MeshRenderer renderer, Collider col)
    {
        float multiplicadorSeleccionado = 0.2f; // Valor por defecto seguro

        // Buscamos qué valor personalizado tiene este item en la lista del inspector
        foreach (var config in powerUpsConfig)
        {
            if (config.itemEnEscena == item)
            {
                multiplicadorSeleccionado = config.gravedadPersonalizada;
                break;
            }
        }

        StartCoroutine(EjecutarRutinaCooldown(item, renderer, col, multiplicadorSeleccionado));
    }

    private IEnumerator EjecutarRutinaCooldown(PotenciadorItem item, MeshRenderer renderer, Collider col, float valorPersonalizado)
    {
        // 1. Ocultar y apagar colisión del objeto en el mapa
        if (renderer) renderer.enabled = false;
        if (col) col.enabled = false;

        Debug.Log("Aplicando power-up de gravedad con valor: " + valorPersonalizado);

        // 2. BUSCAR AL JUGADOR Y APLICAR EL PODER
        GameObject jugadorObj = GameObject.FindWithTag("Player");
        if (jugadorObj != null)
        {
            MovimientoJugador jugador = jugadorObj.GetComponent<MovimientoJugador>();
            if (jugador != null)
            {
                jugador.ActivarPowerUpGravedad(valorPersonalizado, duracionEfecto);
            }
        }
        else
        {
            Debug.LogWarning("[GestorPowerUpsCentral] No se encontró ningún objeto con la etiqueta 'Player' en la escena.");
        }

        // 3. Esperar la duración activa del efecto en el jugador
        yield return new WaitForSeconds(duracionEfecto);

        // 4. Esperar el tiempo de cooldown (recarga) antes de volver a habilitar el item
        yield return new WaitForSeconds(tiempoCooldown);

        // 5. Volver a mostrar y habilitar el objeto en el mapa
        if (renderer) renderer.enabled = true;
        if (col) col.enabled = true;
        
        item.TerminarCooldown();
    }
}