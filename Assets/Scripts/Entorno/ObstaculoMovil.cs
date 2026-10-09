using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObstaculoMovil : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidad = 5f;
    
    [Header("Limpieza (Evitar acumulación)")]
    [SerializeField] private float tiempoDeVida = 8f; // Segundos antes de destruirse

    private Rigidbody rb;
    private Vector3 velocidadCalculada;

    void Start()
    {
        InicializarComponentes();
    }

    void FixedUpdate()
    {
        MoverObstaculo();
    }

    // ==========================================
    // MÉTODOS AUXILIARES
    // ==========================================
    private void InicializarComponentes()
    {
        rb = GetComponent<Rigidbody>();
        // Aseguramos que sea kinematico para que la gravedad no lo afecte, pero se mueva por script
        rb.isKinematic = true;

        // Consigna 3: Gestión de la eliminación
        Destroy(gameObject, tiempoDeVida);
    }

    private void MoverObstaculo()
    {
        // Calculamos el desplazamiento hacia adelante
        Vector3 movimiento = Vector3.forward * velocidad * Time.fixedDeltaTime;
        
        // Movemos la plataforma usando física
        rb.MovePosition(rb.position + transform.TransformDirection(movimiento));

        // Calculamos la velocidad real para que el script del jugador la lea correctamente
        velocidadCalculada = (transform.forward * velocidad);
    }

    // Propiedad pública para que el script del jugador pueda leer la velocidad exacta de este obstáculo
    public Vector3 ObtenerVelocidad()
    {
        return velocidadCalculada;
    }
}