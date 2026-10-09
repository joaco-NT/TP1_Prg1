using UnityEngine;

public class GeneradorObstaculos : MonoBehaviour
{
    [Header("Configuración del Spawner")]
    [SerializeField] private GameObject prefabObstaculo; 
    [SerializeField] private float tiempoInicio = 2f;    
    [SerializeField] private float intervaloRepeticion = 4f; 

    [Header("Control Total (Opcional)")]
    [SerializeField] private bool estaActivo = true; // Permite encender o apagar el spawner desde el inspector o código

    private float temporizador;

    void Start()
    {
        InicializarTemporizador();
    }

    void Update()
    {
        ControlarCicloSpawn();
    }

    // ==========================================
    // MÉTODOS AUXILIARES
    // ==========================================
    private void InicializarTemporizador()
    {
        // Inicializamos el temporizador con el tiempo de retraso inicial
        temporizador = tiempoInicio;
    }

    private void ControlarCicloSpawn()
    {
        // Si el spawner está desactivado, no hacemos nada
        if (!estaActivo) return;

        // Reducimos el temporizador con el tiempo que pasa en cada frame
        temporizador -= Time.deltaTime;

        // Cuando el temporizador llega a cero o menos, spawneamos
        if (temporizador <= 0f)
        {
            InstanciarObstaculo();

            // Reiniciamos el temporizador para el siguiente ciclo
            temporizador = intervaloRepeticion;
        }
    }

    private void InstanciarObstaculo()
    {
        if (prefabObstaculo != null)
        {
            Instantiate(prefabObstaculo, transform.position, transform.rotation);
        }
        else
        {
            Debug.LogWarning("¡Falta asignar el Prefab del obstáculo en el Spawner!");
        }
    }

    // Métodos públicos para tener control total desde otros scripts o botones
    public void PausarSpawner() => estaActivo = false;
    public void ReanudarSpawner() => estaActivo = true;
}