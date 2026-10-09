# 🎮 Plataformas 3D

Proyecto desarrollado para el **Trabajo Práctico Nº 1 de Programación de Videojuegos I**. Consiste en un juego de plataformas en 3D en tercera persona donde el jugador debe recorrer un escenario lleno de obstáculos móviles, utilizar plataformas dinámicas, recolectar power-ups de gravedad, y transportar un objeto especial hasta una zona de meta final.

---

## Tabla de Contenidos

1. [Version de Unity](#version-de-unity)
2. [Controles del Juego](#controles-del-juego)
3. [Mecanicas Implementadas (Scripts)](#mecanicas-implementadas-scripts)
4. [Instrucciones de Instalacion y Ejecucion](#instrucciones-de-instalacion-y-ejecucion)
5. [Captura del Escenario](#captura-del-escenario)

---

## Version de Unity
- **Unity Version:** **Unity 6 LTS**
- **Dependencias:** Utiliza el paquete oficial **Input System** de Unity para la gestión de periféricos (teclado y mouse).

---

## Controles del Juego

| Acción | Tecla / Control |
| :--- | :--- |
| **Movimiento** | Teclas `W`, `A`, `S`, `D` o Flechas Direccionales |
| **Correr** | Pulsar dos veces seguidas la tecla `W` (Doble W) |
| **Saltar** | Barra Espaciadora (`Space`) |
| **Cámara** | Movimiento del Mouse (Vista orbital en 3D) |
| **Agarrar Objeto** | Tecla `E` (Cercano al objeto transportable) |
| **Soltar Objeto** | Tecla `Q` |
| **Liberar Cursor** | Tecla `Escape` (`Esc`) |

---

## Mecanicas Implementadas (Scripts)

1. **Movimiento y Control de Cámara:**  
   * Personaje impulsado por `Rigidbody` con restricciones físicas, salto y control aéreo (`MovimientoJugador.cs`).  
   * Cámara en tercera persona con seguimiento suave e inclinación limitada mediante el mouse (`CamaraSeguimiento.cs`).

2. **Plataforma Móvil Dinámica:**  
   * Plataformas que oscilan en el escenario (`PlataformaMovil.cs`) utilizando `Vector3.MoveTowards` y un cambio de velocidad temporizado mediante la función `Invoke()`. El jugador hereda la inercia de movimiento al pararse sobre ellas.

3. **Generador Procedural de Obstáculos (Spawner):**  
   * Sistema que instancia obstáculos dinámicos en intervalos regulares (`GeneradorObstaculos.cs` y `ObstaculoMovil.cs`). Incluye un sistema de auto-limpieza por tiempo de vida (`Destroy`) para evitar la acumulación infinita de objetos en memoria.

4. **Sistema de Checkpoints y Caídas:**  
   * Gestor avanzado de puntos de control y una **Hitbox de Caída Global** (`GestorRespawnAvanzado.cs` y `EscuchaTrigger.cs`) que reubica automáticamente al jugador en su último checkpoint activo si cae al vacío.

5. **Power-Up de Gravedad (con Corrutina y Cooldown):**  
   * Ítem consumible coleccionable (`PotenciadorItem.cs` y `GestorPowerUpsCentral.cs`) que modifica temporalmente la gravedad del personaje (estilo lunar) y aumenta su salto. Emplea corrutinas (`WaitForSeconds`) para gestionar la duración del efecto y el tiempo de recarga (*cooldown*), evitando que se reactive antes de tiempo.

6. **Transporte de Objetos y Zona de Meta:**  
   * Mecánica para recoger objetos mediante la jerarquía de scripts y `SetParent` (`ObjetoAgarrable.cs`), transportarlos y depositarlos en una **Zona de Meta** configurada por triggers (`ZonaMeta.cs`), la cual valida el objeto correcto, activa un mensaje de victoria por consola y despliega un efecto visual de flotación.

---

## Instrucciones de Instalacion y Ejecucion

1. **Clonar o descargar** este repositorio en tu equipo.
2. Abre **Unity Hub**.
3. Haz clic en el botón **Add** (Añadir) y selecciona la carpeta raíz del proyecto descargado.
4. Asegúrate de abrir el proyecto utilizando la versión compatible de Unity (**Unity 6 LTS**).
5. Una vez dentro del editor, dirígete a la carpeta `Assets/Scenes/` y abre la escena principal del juego (por ejemplo, `Nivel1.unity`).
6. Presiona el botón **Play ▶** en la parte superior central de Unity para ejecutar y probar el juego.

---

## Captura del Escenario

![Captura del juego](/Capturas/juego.png)