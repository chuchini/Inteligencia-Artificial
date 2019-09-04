using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Clase que sirve para manejar al dron de manera manual y probar los actuadores, no es necesaria
/// para el comportamiento automatico pero se mantiene la clase para futuras pruebas y comodidad.
/// </summary>
public class Controlador : MonoBehaviour
{
    // Idealmente sólo se requiere de sensores y actuadores para programar el comportamiento
    private Actuadores actuador;
    private Sensores sensor;

    // Asignaciones de componentes
    void Start(){
        actuador = GetComponent<Actuadores>();
        sensor = GetComponent<Sensores>();
    }

    // Update y FixedUpdate son similares en uso, pero por regla general se recomienda usar
    // FixedUpdate para calcular elementos físicos como el uso de Rigidbody
    void FixedUpdate(){

        // El agente no realiza ninguna acción si no tiene batería
        if(sensor.Bateria() <= 0)
            return;

        // A continuación se muestran ejemplos de uso de actuadores y sensores
        // para ser utilizados de manera manual (por una persona):

        if(Input.GetKey(KeyCode.I))
            actuador.Ascender();
        if(Input.GetKey(KeyCode.K))
            actuador.Descender();
        if(!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K))
            actuador.Flotar();

        if(Input.GetAxis("Vertical") > 0)
            actuador.Adelante();
        if(Input.GetAxis("Vertical") < 0)
            actuador.Atras();

        if(Input.GetKey(KeyCode.J))
            actuador.GirarIzquierda();
        if(Input.GetKey(KeyCode.L))
            actuador.GirarDerecha();

        if(Input.GetAxis("Horizontal") > 0)
            actuador.Derecha();
        if(Input.GetAxis("Horizontal") < 0)
            actuador.Izquierda();

        
        if(sensor.TocandoPared())
            Debug.Log("Tocando pared!");
        if(sensor.CercaDePared())
            Debug.Log("Cerca de una pared!");
        if(sensor.FrenteAPared())
            Debug.Log("Frente a pared!");
        if(Input.GetKey(KeyCode.F))
            actuador.Detener();
    }
}
