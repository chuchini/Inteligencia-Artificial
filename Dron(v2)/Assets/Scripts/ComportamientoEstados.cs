using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ejemplo de un comportamiento automático para el agente (basado en modelos)
public class ComportamientoEstados : MonoBehaviour {

	private Sensores sensor;
	private Actuadores actuador;

	private enum Percepcion {NoParedCerca=0, ParedCerca=1}; // Lista predefinida de posibles percepciones con los sensores
	private enum Estado {Avanzar=0, Detenerse=1, Girar=2}; // Lista de estados para la maquina de estados y tabla de transiciones
	private Estado estadoActual;
	private Percepcion percepcionActual;

	void Start(){
		sensor = GetComponent<Sensores>();
		actuador = GetComponent<Actuadores>();
		estadoActual = Estado.Avanzar;
	}

	void FixedUpdate() {
		if(sensor.Bateria() <= 0)
			return;

		percepcionActual = PercibirMundo();
		estadoActual = TablaDeTransicion(estadoActual, percepcionActual);
		AplicarEstado(estadoActual);
	}

	// A partir de este punto se representa un agente basado en modelos.
	// La idea es similar a crear una máquina de estados finita donde se hacen las siguientes consideraciones:
	// - El alfabeto es un conjunto predefinido de percepciones hechas con sensores del agente
	// - El conjunto de estados representa un conjunto de métodos con acciones del agente
	// - La función de transición es un método
	// - El estado inicial se inicializa en Start()
	// - El estado final es opcional (pero recomendable de indicar)

	// Tabla de transición que representa el conjunto de reglas
	// -----------------------------------------------
	// | Estado\Percepcion | paredCerca | !paredCerca |
	// |-------------------|------------|-------------|
	// | Avanzar           | Detenerse  | Avanzar     |
	// |-------------------|------------|-------------|
	// | Detenerse         | Detenerse  | Detenerse   |
	// ------------------------------------------------
	Estado TablaDeTransicion(Estado estado, Percepcion percepcion){
		switch(estado){
			case Estado.Avanzar:
				switch(percepcion){
					case Percepcion.ParedCerca:
						estado = Estado.Detenerse;
						break;
					case Percepcion.NoParedCerca:
						estado = Estado.Avanzar;
						break;
				}
				break;
			case Estado.Detenerse:
				switch(percepcion){
					case Percepcion.ParedCerca:
						estado = Estado.Detenerse;
						break;
					case Percepcion.NoParedCerca:
						estado = Estado.Detenerse;
						break;
				}
				break;
		}
		return estado;
	}

	// Representación de los ESTADOS como métodos

	// El estado AVANZAR significa moverse hacia adelante siempre.
	void Avanzar(){
		actuador.Flotar();
		actuador.Adelante();
	}
	// El estado DETENERSE representa mantenerse en el mismo punto
	void Detenerse(){
		actuador.Flotar();
		actuador.Detener();
	}

	// Usar sensores para determinar el tipo de percepción actual
	Percepcion PercibirMundo(){
		Percepcion percepcionActual = Percepcion.NoParedCerca;
		if(sensor.CercaDePared())
			percepcionActual = Percepcion.ParedCerca;
		else
			percepcionActual = Percepcion.NoParedCerca;
		return percepcionActual;
	}

	// Aplicar el estado actual, i.e, mandar a llamar al método del estado dado como parámetro
	void AplicarEstado(Estado estado){
		switch(estado){
			case Estado.Avanzar:
				Avanzar();
				break;
			case Estado.Detenerse:
				Detenerse();
				break;
			default:
				Detenerse();
				break;
		}
	}
}
