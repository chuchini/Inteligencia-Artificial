using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComportamientoAutomatico : MonoBehaviour
{

	private Sensores sensor;
	private Actuadores actuador;
	
	private enum Percepcion
	{
		CercaDePared	= 0,
		NoCercaDePared	= 1,
		NoBasuraCerca	= 2,
		BasuraCerca		= 3,
		TocandoABasura	= 4
	}

	private enum Estado
	{
		Avanzar		= 0,
		Retroceder	= 1,
		Detenerse	= 2,
		Girar		= 3,
		Limpiar		= 4
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
		
		if(!sensor.FrenteAPared())
			Debug.Log("No Frente a Pared!");

		if(sensor.CercaDeBasura())
			Debug.Log("Cerca de una basura!");
		if(sensor.CercaDePared())
			Debug.Log("Cerca de una pared!");

		if(sensor.FrenteAPared())
			Debug.Log("Frente a pared!");
		
	}

	Estado TablaDeTransicion(Estado estado, Percepcion percepcion)
	{
		switch (estado)
		{
			case Estado.Avanzar:
				switch (percepcion)
				{
					case Percepcion.CercaDePared:
						estado = Estado.Detenerse;
						break;
					case Percepcion.NoCercaDePared:
						estado = Estado.Avanzar;
						break;
					case Percepcion.BasuraCerca: 
						estado = Estado.Avanzar; 
						break;
					case Percepcion.NoBasuraCerca: 
						estado = Estado.Avanzar; 
						break;
					case Percepcion.TocandoABasura: 
						estado = Estado.Detenerse; 
						break;
				}
				break;
			
			case Estado.Retroceder:
				switch (percepcion)
				{
					case Percepcion.CercaDePared:
						estado = Estado.Retroceder;
						break;
					case Percepcion.NoCercaDePared:
						estado = Estado.Girar;
						break;
					case Percepcion.BasuraCerca:
						estado = Estado.Avanzar;
						break;
					case Percepcion.NoBasuraCerca:
						estado = Estado.Avanzar;
						break;
					case Percepcion.TocandoABasura:
						estado = Estado.Detenerse;
						break;
				}
				break;
			
			case Estado.Detenerse:
				switch (percepcion)
				{
					case Percepcion.CercaDePared:
						estado = Estado.Retroceder;
						break;
					case Percepcion.NoCercaDePared:
						estado = Estado.Avanzar;
						break;
					case Percepcion.NoBasuraCerca:
						estado = Estado.Avanzar;
						break;
					case Percepcion.BasuraCerca:
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
					case Percepcion.CercaDePared:
						estado = Estado.Retroceder;
						break;
					case Percepcion.NoCercaDePared:
						estado = Estado.Avanzar;
						break;
					case Percepcion.BasuraCerca: 
						estado = Estado.Avanzar; 
						break;
					case Percepcion.NoBasuraCerca: 
						estado = Estado.Avanzar; 
						break;
					case Percepcion.TocandoABasura: 
						estado = Estado.Detenerse; 
						break;
				}
				break;

			case Estado.Limpiar: 
				switch (percepcion)
				{
					case Percepcion.CercaDePared:
						estado = Estado.Retroceder;
						break;
					case Percepcion.NoCercaDePared:
						estado = Estado.Avanzar;
						break;
					case Percepcion.BasuraCerca: 
						estado = Estado.Avanzar; 
						break;
					case Percepcion.NoBasuraCerca: 
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
	
	void Atras()
	{
		actuador.Flotar();
		actuador.Atras();
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
		Percepcion percepcionActual = Percepcion.NoCercaDePared;
		if (sensor.CercaDePared())
			percepcionActual = Percepcion.CercaDePared;
		else if (!sensor.CercaDePared())
			percepcionActual = Percepcion.NoCercaDePared;
		else if (sensor.CercaDeBasura()) 
			percepcionActual = Percepcion.BasuraCerca;
		else if (!sensor.CercaDeBasura()) 
			percepcionActual = Percepcion.NoBasuraCerca;
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
			case Estado.Retroceder:
				Atras();
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