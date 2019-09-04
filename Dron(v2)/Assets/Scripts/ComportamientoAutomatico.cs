using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComportamientoAutomatico : MonoBehaviour
{

	private Sensores sensor;
	private Actuadores actuador;

	private float speed = 0.001F;

	private float startTime;

	private float journeyLength;
	
	private enum Percepcion
	{
		CercaDePared	= 0,
		NoCercaDePared	= 1,
		FrenteAPared	= 2,
		NoFrenteAPared	= 3,
		CercaDeObjeto	= 4,
		FrenteAObjeto	= 5,
		NoFrenteAObjeto	= 6,
		NoCercaDeObjeto	= 7,
		BateriaBaja		= 8,
		//NoBateriaBaja	= 9,
		TocandoBase		= 9,
		//NoTocandoBase	= 11
	}

	private enum Estado
	{
		Avanzar		= 0,
		Retroceder	= 1,
		Detenerse	= 2,
		Girar		= 3,
		Retornar	= 4,
		Cargarse	= 5,
		Ascender	= 6
	}

	private Estado estadoActual;
	private Percepcion percepcionActual;

	private void Start()
	{
		sensor = GetComponent<Sensores>();
		actuador = GetComponent<Actuadores>();
		estadoActual = Estado.Avanzar;

		startTime = Time.time;
		
		journeyLength = Vector3.Distance(sensor.Ubicacion(), sensor.UbicacionBase());
	}

	void FixedUpdate()
	{
		if (sensor.Bateria() <= 0)
		{
			return;
		}
		
		if (sensor.Bateria() <= 50)
		{
			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / journeyLength;
			transform.position = Vector3.Lerp(sensor.Ubicacion(), sensor.UbicacionBase(), fracJourney);
			Debug.Log("Estoy atorado aqui");
		}

		percepcionActual = PercibirMundo();
		estadoActual = TablaDeTransicion(estadoActual, percepcionActual);
		AplicarEstado(estadoActual);
	}

	Estado TablaDeTransicion(Estado estado, Percepcion percepcion)
	{
		switch (estado)
		{
			case Estado.Avanzar:
				switch (percepcion)
				{
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DE LA PARED
					/*case Percepcion.CercaDePared:
						estado = Estado.Detenerse;
						break;
					case Percepcion.NoCercaDePared:
						estado = Estado.Avanzar;
						break;
					case Percepcion.FrenteAPared:
						estado = Estado.Detenerse;
						break;
					case Percepcion.NoFrenteAPared:
						estado = Estado.Avanzar;
						break;
					*/
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DEL OBJETO
					case Percepcion.CercaDeObjeto:
						estado = Estado.Detenerse;
						break;
					case Percepcion.NoCercaDeObjeto:
					//	if (sensor.CercaDePared() == false)
					//	{
							estado = Estado.Avanzar;
					//	}
					//	else
					//	{
					//		estado = Estado.Detenerse;
					//	}
						break;
					case Percepcion.FrenteAObjeto:
						estado = Estado.Detenerse;
						break;
					case Percepcion.NoFrenteAObjeto:
						if (sensor.CercaDeObjeto() == false)
						{
							estado = Estado.Avanzar;
						}
						else
						{
							estado = Estado.Detenerse;
						}
						break;
					
					case Percepcion.BateriaBaja:
						estado = Estado.Retornar;
						break;
					case Percepcion.TocandoBase:
						estado = Estado.Cargarse;
						break;
					//case Percepcion.NoTocandoBase:
					//	if (sensor.Bateria() > 50)
					//	{
					//	estado = Estado.Avanzar;
					//	Debug.Log("Me meti aqui");
							
					//	}
					//	else
					//	{
					//		estado = Estado.Retornar;
					//	}
					//	break;
					//case Percepcion.NoBateriaBaja:
						//if (sensor.Bateria() < sensor.BateriaMaxima())
						//{
						//	estado = Estado.Cargarse;
						//}
						//else
						//{
					//	if (sensor.CercaDeObjeto() == false)
					//	{
					//		estado = Estado.Avanzar;
					//	}
					//	else //if (sensor.CercaDeObjeto() == true)
					//	{
					//		estado = Estado.Detenerse;
					//	}
						//}
					//	break;
				}
				break;
			
			case Estado.Detenerse:
				switch (percepcion)
				{
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DE LA PARED
					/*case Percepcion.CercaDePared:
						estado = Estado.Retroceder;
						break;
					case Percepcion.NoCercaDePared:
						estado = Estado.Avanzar;
						break;
					case Percepcion.FrenteAPared:
						estado = Estado.Girar;
						break;
					case Percepcion.NoFrenteAPared:
						estado = Estado.Avanzar;
						break;
					*/
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DEL OBJETO
					case Percepcion.CercaDeObjeto:
						estado = Estado.Girar;
						break;
					case Percepcion.NoCercaDeObjeto:
					//	if (sensor.CercaDePared() == false)
					//	{
							estado = Estado.Avanzar;
					
					//	}
					//	else
					//	{
					//		estado = Estado.Retroceder;
					//	}
						break;
					case Percepcion.FrenteAObjeto:
						estado = Estado.Detenerse;
						Debug.Log("Me meti aqui");
						break;
					case Percepcion.NoFrenteAObjeto:
						estado = sensor.CercaDeObjeto() == false ? Estado.Avanzar : Estado.Girar;
						break;
					case Percepcion.BateriaBaja:
						estado = Estado.Retornar;
						break;
					case Percepcion.TocandoBase:
						if (sensor.Bateria() > 50)
						{
							estado = Estado.Ascender;
						}
						else
						{
							estado = Estado.Cargarse;
						}
						break;
					//case Percepcion.NoTocandoBase:
					//	if (sensor.Bateria() > 50)
					//	{
					//		estado = Estado.Avanzar;
					//	}
					//	else
					//	{
					//		estado = Estado.Retornar;
					//	}
					//	break;
					//case Percepcion.NoBateriaBaja:
						//if (sensor.Bateria() < sensor.BateriaMaxima())
						//{
						//	estado = Estado.Cargarse;
						//}
						//else
						//{
					//	estado = Estado.Avanzar;
					//	Debug.Log("Me meti aqui");
					//}
					//break;
					
				}
				break;
			
			case Estado.Girar:
				switch (percepcion)
				{
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DE LA PARED
					/*case Percepcion.CercaDePared:
						estado = Estado.Retroceder; //Antes avanzaba
						break;
					case Percepcion.NoCercaDePared:
						estado = Estado.Avanzar;
						break;
					case Percepcion.FrenteAPared:
						estado = Estado.Girar;
						break;
					case Percepcion.NoFrenteAPared:
						estado = Estado.Avanzar;
						break;
					*/
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DEL OBJETO
					case Percepcion.CercaDeObjeto:
						estado = Estado.Detenerse;
						break;
					case Percepcion.NoCercaDeObjeto:
					//	if (sensor.CercaDePared() == false)
					//	{
							estado = Estado.Avanzar;
					//	}
					//	else
					//	{
					//		estado = Estado.Retroceder;
					//	}
						break;
					case Percepcion.FrenteAObjeto:
						estado = Estado.Detenerse;
						break;
					case Percepcion.NoFrenteAObjeto:
						if (sensor.CercaDeObjeto() == false)
						{
							estado = Estado.Avanzar;
						}
						else
						{
							estado = Estado.Girar;
						}
						break;
					case Percepcion.BateriaBaja:
						estado = Estado.Retornar;
						break;
					case Percepcion.TocandoBase:
						estado = sensor.Bateria() > 50 ? Estado.Avanzar : Estado.Cargarse;
						break;
					//case Percepcion.NoTocandoBase:
					//	estado = sensor.Bateria() > 50 ? Estado.Avanzar : Estado.Retornar;
					//	break;
					//case Percepcion.NoBateriaBaja:
						//if (sensor.Bateria() < sensor.BateriaMaxima())
						//{
						//	estado = Estado.Cargarse;
						//}
						//else
						//{
					//	estado = Estado.Avanzar;
					//}
					//break;
				}
				break;
			
			case Estado.Retroceder:
				switch (percepcion)
				{
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DE LA PARED
					/*case Percepcion.CercaDePared:
						estado = Estado.Retroceder;
						break;
					case Percepcion.NoCercaDePared:
						estado = Estado.Avanzar;
						break;
					case Percepcion.FrenteAPared:
						estado = Estado.Detenerse;
						break;
					case Percepcion.NoFrenteAPared:
						estado = Estado.Avanzar;
						break;
					*/
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DEL OBJETO
					case Percepcion.CercaDeObjeto:
						estado = Estado.Detenerse;
						break;
					case Percepcion.NoCercaDeObjeto:
					//	if (sensor.CercaDePared() == false)
					//	{
							estado = Estado.Avanzar;
					//	}
					//	else
					//	{
					//		estado = Estado.Retroceder;
					//	}
						
						break;
					case Percepcion.FrenteAObjeto:
						estado = Estado.Detenerse;
						break;
					case Percepcion.NoFrenteAObjeto:
						estado = sensor.CercaDeObjeto() == false ? Estado.Retroceder : Estado.Detenerse;
						break;
					case Percepcion.BateriaBaja:
						estado = Estado.Retornar;
						break;
					case Percepcion.TocandoBase:
						estado = sensor.Bateria() > 50 ? Estado.Avanzar : Estado.Cargarse;
						break;
					//case Percepcion.NoTocandoBase:
					//	estado = sensor.Bateria() > 50 ? Estado.Avanzar : Estado.Retornar;
					//	break;
					//case Percepcion.NoBateriaBaja:
						//if (sensor.Bateria() < sensor.BateriaMaxima())
						//{
						//	estado = Estado.Cargarse;
						//}
						//else
						//{
					//	estado = Estado.Avanzar;
					//}
					//break;
					
				}
				break;
			
			case Estado.Retornar:
				switch (percepcion)
				{
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DE LA PARED
					/*case Percepcion.CercaDePared:
						estado = Estado.Retroceder;
						break;
					case Percepcion.NoCercaDePared:
						estado = Estado.Avanzar;
						break;
					case Percepcion.FrenteAPared:
						estado = Estado.Detenerse;
						break;
					case Percepcion.NoFrenteAPared:
						estado = Estado.Avanzar;
						break;
					*/
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DEL OBJETO
					case Percepcion.CercaDeObjeto:
						estado = Estado.Retornar;
						break;
					case Percepcion.NoCercaDeObjeto:
						estado = Estado.Retornar;
						break;
					case Percepcion.FrenteAObjeto:
						estado = Estado.Retornar;
						break;
					case Percepcion.NoFrenteAObjeto:
						estado = Estado.Retornar;
						break;
					case Percepcion.BateriaBaja:
						estado = Estado.Retornar;
						break;
					case Percepcion.TocandoBase:
						estado = sensor.Bateria() > 50 ? Estado.Avanzar : Estado.Cargarse;
						break;
					//case Percepcion.NoTocandoBase:
					//	estado = sensor.Bateria() > 50 ? Estado.Avanzar : Estado.Retornar;
					//	break;
					//case Percepcion.NoBateriaBaja:
						//if (sensor.Bateria() < sensor.BateriaMaxima())
						//{
						//	estado = Estado.Cargarse;
						//}
						//else
						//{
					//	estado = Estado.Avanzar;
					//}
					//break;
				}
				break;
			
			case Estado.Cargarse:
				switch (percepcion)
				{
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DE LA PARED
					/*case Percepcion.CercaDePared:
						estado = Estado.Retroceder;
						break;
					case Percepcion.NoCercaDePared:
						estado = Estado.Avanzar;
						break;
					case Percepcion.FrenteAPared:
						estado = Estado.Detenerse;
						break;
					case Percepcion.NoFrenteAPared:
						estado = Estado.Avanzar;
						break;
					*/
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DEL OBJETO
					case Percepcion.CercaDeObjeto:
						estado = Estado.Retornar;
						break;
					case Percepcion.NoCercaDeObjeto:
						estado = Estado.Retornar;
						break;
					case Percepcion.FrenteAObjeto:
						estado = Estado.Retornar;
						break;
					case Percepcion.NoFrenteAObjeto:
						estado = Estado.Retornar;
						break;
					case Percepcion.BateriaBaja:
						estado = Estado.Retornar;
						break;
					case Percepcion.TocandoBase:
						estado = Estado.Cargarse;
						if (sensor.Bateria() < sensor.BateriaMaxima())
						{
							estado = Estado.Cargarse;
							Debug.Log("Estoy atorado aqui 3");
						}
						else
						{
							Debug.Log("Ya no ocupo carga");
							estado = Estado.Ascender;
						}
						break;
					//case Percepcion.NoTocandoBase:
					//	estado = Estado.Retornar;
					//	break;
					//case Percepcion.NoBateriaBaja:
					//	if (sensor.Bateria() < sensor.BateriaMaxima())
					//	{
					//		estado = Estado.Cargarse;
					//		Debug.Log("Estoy atorado aqui 2");
					//	}
					//	else
					//	{
					//		Debug.Log("Ya no ocupo carga");
					//		estado = Estado.Avanzar;
					//	}
					//	break;
				}
				break;
			
			case Estado.Ascender:
				estado = transform.position.y >= 1.83 ? Estado.Detenerse : Estado.Ascender;
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

	void Retornar()
	{
		actuador.Flotar();
		actuador.Detener();
	}

	void Cargarse()
	{
		actuador.Flotar();
		actuador.CargarBateria();
	}

	void Ascender()
	{
		if (transform.position.y >= 1.84)
		{
			actuador.Detener();
		}
		else
			actuador.Ascender();
	}
	
	
	Percepcion PercibirMundo() 
	{ 
		Percepcion percepcionActual = Percepcion.NoCercaDeObjeto;
		/*if (sensor.CercaDePared())
		{
			percepcionActual = Percepcion.CercaDePared;
			Debug.Log("Cerca de Pared");
		}

		if (!sensor.CercaDePared())
		{
			percepcionActual = Percepcion.NoCercaDePared;
			Debug.Log("No Cerca de Pared!");
		}

		if (sensor.FrenteAPared())
		{
			percepcionActual = Percepcion.FrenteAPared;
			Debug.Log("Frente a Pared");
		}

		if (!sensor.FrenteAPared())
		{
			percepcionActual = Percepcion.NoFrenteAPared;
			Debug.Log("No Frente a Pared");
		}*/
		
		if (sensor.CercaDeObjeto())
		{
			percepcionActual = Percepcion.CercaDeObjeto;
			Debug.Log("Cerca de Objeto");
		}

		if (!sensor.CercaDeObjeto())
		{
			percepcionActual = Percepcion.NoCercaDeObjeto;
			Debug.Log("No Cerca de Objeto");
		}

		if (sensor.FrenteAObjeto())
		{
			percepcionActual = Percepcion.FrenteAObjeto;
			Debug.Log("Frente a Objeto");
		}

		if (!sensor.FrenteAObjeto())
		{
			percepcionActual = Percepcion.NoFrenteAObjeto;
			Debug.Log("No Fremte a Objeto");
		}

		if (sensor.Bateria() < 50)
		{
			percepcionActual = Percepcion.BateriaBaja;
			Debug.Log("Bateria Baja");
		}
		
		//if (sensor.Bateria() > 50)
		//{
		//	percepcionActual = Percepcion.NoBateriaBaja;
		//	Debug.Log("Bateria No Baja");
		//}

		if (sensor.TocandoBase())
		{
			percepcionActual = Percepcion.TocandoBase;
			Debug.Log("Tocando Base");
		}

		//if (!sensor.TocandoBase())
		//{
		//	percepcionActual = Percepcion.NoTocandoBase;
		//	Debug.Log("No tocando base");
		//}

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
			case Estado.Retornar:
				Retornar();
				break;
			case Estado.Cargarse:
				Cargarse();
				break;
			case Estado.Ascender:
				Ascender();
				break;
			default: 
				Detenerse(); 
				break;
		}
	}
}
