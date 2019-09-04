using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Componente auxiliar que modela el comportamiento de una bateria interna
// Dicha batería se descarga constantemente a menos que se utilize un método para recargar
public class Bateria : MonoBehaviour
{
    public float bateria; // Esta cifra es equivalente a los segundos activos de la batería
    public float capacidadMaximaBateria; // Indica la capacidad máxima de la batería
    public float velocidadDeCarga; // Escalar para multiplicar la velocidad de carga de la batería

    void Update(){
        if(bateria > 0) // esto evita que la batería sea negativa
            bateria -= Time.deltaTime;
    }

    /**
     * Metodo que se encarga de realizar la carga de la bateria dependiendo de la
     * velocidad de la carga y por el transcurso del tiempo desde que se inicia la carga.
     */
    public void Cargar(){
        if(bateria < capacidadMaximaBateria)
            bateria += Time.deltaTime * velocidadDeCarga;
    }

    /**
     * Metodo para saber el nivel actual de la bateria.
     */
    public float NivelDeBateria(){
        return bateria;
    }
}
