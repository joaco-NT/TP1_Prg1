using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlataformaMovil : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField, Tooltip("Metros que se moverá en el eje Z")]
    private float distanciaZ = 5f;
    [SerializeField] private float velocidad = 3f;

    private Rigidbody rb;
    private Vector3 posicionInicial;
    private Vector3 posicionObjetivo;
    private bool yendoHaciaObjetivo = true;

    void Awake()
    {
        InicializarComponentes();
    }

    void Start()
    {
        ConfigurarMovimientoInicial();
    }

    void FixedUpdate()
    {
        MoverPlataforma();
    }

    // ==========================================
    // MÉTODOS AUXILIARES
    // ==========================================
    private void InicializarComponentes()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Auto-asignación de Tag para el jugador
        if (!gameObject.CompareTag("PlataformaMovil"))
        {
            gameObject.tag = "PlataformaMovil";
        }
    }

    private void ConfigurarMovimientoInicial()
    {
        // Inicialización automática al arrancar
        posicionInicial = transform.position;
        posicionObjetivo = posicionInicial + new Vector3(0f, 0f, distanciaZ);

        // Consigna: Uso de Invoke() para el cambio temporizado
        Invoke(nameof(CambiarVelocidad), 5f);
    }

    private void MoverPlataforma()
    {
        // 1. LÓGICA INTERNA: Calcular el movimiento de la plataforma
        Vector3 destinoActual = yendoHaciaObjetivo ? posicionObjetivo : posicionInicial;
        Vector3 nuevaPosicion = Vector3.MoveTowards(rb.position, destinoActual, velocidad * Time.fixedDeltaTime);

        rb.MovePosition(nuevaPosicion);

        // Cambio de dirección al llegar al destino
        if (Vector3.Distance(rb.position, destinoActual) < 0.05f)
        {
            yendoHaciaObjetivo = !yendoHaciaObjetivo;
        }
    }

    private void CambiarVelocidad()
    {
        velocidad *= 1.5f;
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(CambiarVelocidad));
    }
}