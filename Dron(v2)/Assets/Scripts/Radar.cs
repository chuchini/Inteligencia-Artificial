using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Componente auxiliar que utiliza un Collider esférico a manera de radar
// para comprobar colisiones con otros elementos.
// Las comprobaciones y métodos son análogos al componente (script) de Sensores.
public class Radar : MonoBehaviour
{
    // Objetos que identificara el radar.
    private bool cercaDeObjeto;
    private bool cercaDePared;

    // Los siguientes metodos sirven para que el sensor decida las posibles interacciones con los objetos que detecta
    // y se le indicaron que detectara. Para saber cuando entra, cuando se mantiene y cuando salen dichos objetos
    // del radar del dron.
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Objeto"))
        {
            cercaDeObjeto = true;
        }

        if (other.gameObject.CompareTag("Pared"))
        {
            cercaDePared = true;
        }
        
    }

    void OnTriggerStay(Collider other){
	
        if(other.gameObject.CompareTag("Objeto"))
        {
            cercaDeObjeto = true;
        }
        
        if(other.gameObject.CompareTag("Pared"))
        {
            cercaDePared = true;
        }
    }

    void OnTriggerExit(Collider other){
        if(other.gameObject.CompareTag("Objeto")){
            cercaDeObjeto = false;
        }
        if(other.gameObject.CompareTag("Pared")){
            cercaDePared = false;
        }
        
    }

    /// <summary>
    /// Metodo que indica si el dron esta cerca o no de la pared por medio del sensor
    /// </summary>
    /// <returns>true, si el dron esta cerca de la pared, false en caso contrario</returns>
    public bool CercaDePared(){
        return cercaDePared;
    }
    
    /// <summary>
    /// Metodo que indica si el dron esta cerca o no del objeto por medio del sensor.
    /// </summary>
    /// <returns>true, si el dron esta cerca del objeto, false en caso contrario</returns>
    public bool CercaDeObjeto()
    {
        return cercaDeObjeto;
    }
}
