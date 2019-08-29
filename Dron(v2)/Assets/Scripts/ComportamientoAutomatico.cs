using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComportamientoAutomatico : MonoBehaviour
{

	/*private Sensores sensor;
	private Actuadores actuador;

	void Start(){
		sensor = GetComponent<Sensores>();
		actuador = GetComponent<Actuadores>();
	}

	void FixedUpdate () {
		if(sensor.Bateria() <= 0)
			return;

		if (sensor.CercaDePared()) {
			actuador.Flotar();
			actuador.Detener();
		} else {
			actuador.Flotar();
			actuador.Adelante();
		}
	}*/

	private Sensores sensor;
	private Actuadores actuador;
	
	private enum Percepcion
	{
		NoParedCerca	= 0,
		ParedCerca		= 1,
		FrentePared		= 2,
		NoBasuraCerca	= 3,
		BasuraCerca		= 4,
		FrenteBasura	= 5,
		TocandoABasura	= 6
	}

	private enum Estado
	{
		Avanzar		= 0,
		Detenerse	= 1,
		Girar		= 2,
		Limpiar		= 3
	}

	private Estado estadoActual;
	private Percepcion percepcionActual;

	private void Start()
	{
		sensor = GetComponent<Sensores>();
		actuador = GetComponent<Actuadores>();
		estadoActual = Estado.Avanzar;
	}

	void FixedUpdate()
	{
		if (sensor.Bateria() <= 0)
			return;

		percepcionActual = PercibirMundo();
		estadoActual = TablaDeTransicion(estadoActual, percepcionActual);
		AplicarEstado(estadoActual);
		
		if(sensor.TocandoBasura()){
			Debug.Log("Tocando basura!");
			actuador.Limpiar(sensor.GetBasura());
		}
	}

	Estado TablaDeTransicion(Estado estado, Percepcion percepcion)
	{
		switch (estado)
		{
			case Estado.Avanzar:
				switch (percepcion)
				{
					case Percepcion.NoParedCerca: 
						estado = Estado.Avanzar; 
						break; 
					case Percepcion.ParedCerca: 
						estado = Estado.Detenerse; 
						break;
					case Percepcion.FrentePared:
						estado = Estado.Detenerse; 
						break;
					case Percepcion.NoBasuraCerca: 
						estado = Estado.Avanzar; 
						break;
					case Percepcion.BasuraCerca: 
						estado = Estado.Avanzar; 
						break;
					case Percepcion.FrenteBasura: 
						estado = Estado.Avanzar; 
						break;
					case Percepcion.TocandoABasura: 
						estado = Estado.Limpiar; 
						break;
				}
				break;

			case Estado.Detenerse:
				switch (percepcion)
				{
					case Percepcion.NoParedCerca:
						estado = Estado.Avanzar;
						break;
					case Percepcion.ParedCerca:
						estado = Estado.Girar;
						break;
					case Percepcion.FrentePared:
						estado = Estado.Girar;
						break;
					case Percepcion.NoBasuraCerca:
						estado = Estado.Avanzar;
						break;
					case Percepcion.BasuraCerca:
						estado = Estado.Avanzar;
						break;
					case Percepcion.FrenteBasura:
						estado = Estado.Avanzar;
						break;
					case Percepcion.TocandoABasura:
						estado = Estado.Limpiar;
						break;
				}
				break;

			case Estado.Girar: 
				switch (percepcion) 
				{ 
					case Percepcion.NoParedCerca:
						estado = Estado.Avanzar; 
						break;
					case Percepcion.ParedCerca: 
						estado = Estado.Avanzar;
						break;
					case Percepcion.FrentePared: 
						estado = Estado.Avanzar; 
						break;
					case Percepcion.NoBasuraCerca: 
						estado = Estado.Avanzar; 
						break;
					case Percepcion.BasuraCerca: 
						estado = Estado.Avanzar; 
						break;
					case Percepcion.FrenteBasura: 
						estado = Estado.Avanzar;
						break;
					case Percepcion.TocandoABasura: 
						estado = Estado.Limpiar; 
						break;
				}
				break;

			case Estado.Limpiar: 
				switch (percepcion) 
				{ 
					case Percepcion.NoParedCerca: 
						estado = Estado.Avanzar; 
						break;
					case Percepcion.ParedCerca: 
						estado = Estado.Avanzar; 
						break;
					case Percepcion.FrentePared: 
						estado = Estado.Girar; 
						break;
					case Percepcion.NoBasuraCerca: 
						estado = Estado.Avanzar; 
						break;
					case Percepcion.BasuraCerca: 
						estado = Estado.Avanzar; 
						break;
					case Percepcion.FrenteBasura: 
						estado = Estado.Avanzar; 
						break;
					case Percepcion.TocandoABasura: 
						estado = Estado.Limpiar; 
						break;
				} 
				break; 
		}
		
		return estado;
	}

	void Avanzar()
	{ 
		actuador.Flotar();
		actuador.Adelante();
	}
	
	void Detenerse() 
	{ 
		actuador.Flotar(); 
		actuador.Detener();
	}
	
	void Girar() 
	{ 
		actuador.Flotar(); 
		actuador.GirarDerecha();
	}
	
	void Limpiar() 
	{ 
		actuador.Flotar(); 
		actuador.Limpiar(sensor.GetBasura());
	}
	
	Percepcion PercibirMundo() 
	{ 
		Percepcion percepcionActual = Percepcion.NoParedCerca; 
		if (sensor.CercaDePared()) 
			percepcionActual = Percepcion.ParedCerca;
		else if (sensor.FrenteAPared()) 
			percepcionActual = Percepcion.FrentePared;
		else if (!sensor.CercaDePared()) 
			percepcionActual = Percepcion.NoParedCerca;
		else if (sensor.CercaDeBasura()) 
			percepcionActual = Percepcion.BasuraCerca;
		else if (!sensor.CercaDeBasura()) 
			percepcionActual = Percepcion.NoBasuraCerca;
		else if (sensor.FrenteABasura()) 
			percepcionActual = Percepcion.FrenteBasura;
		else
			percepcionActual = Percepcion.TocandoABasura;
		
		return percepcionActual;
	}
	
	void AplicarEstado(Estado estado) 
	{ 
		switch (estado) 
		{ 
			case Estado.Avanzar: 
				Avanzar(); 
				break;
			case Estado.Detenerse: 
				Detenerse(); 
				break;
			case Estado.Girar: 
				Girar(); 
				break;
			case Estado.Limpiar: 
				Limpiar(); 
				break;
			default: 
				Detenerse(); 
				break;
		}
	}
}