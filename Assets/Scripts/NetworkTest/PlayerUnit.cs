using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//Un PlayerUnit es una unidad controlada por un jugador

public class PlayerUnit : NetworkBehaviour {

    Vector3 velocity;

    //La posición que creemos que es la más correcta para este player
    //  NOTA: si somos la autoridad, entonces este será
    //  exactamente el mismo que transform.position
    Vector3 bestGuessPosition;

    //Este es un valor actualizado constantemente de nuestra latencia al servidor
    //segundos que tarda en recibir un mensaje
    float nuestraLatencia;


    //Cuanto más alto sea este valor, más rápido se parecerá nuestra posición a la best guess
    float latencySmoothingFactor = 10;

	void Start () {
		
	}
	
	void Update () {
        //Esta función se ejecuta en TODOS los PlayerUnit

        //El código que se ejecute aquí se está ejecutando para TODAS
        //las versiones de este objeto, incluso si no es la copia con autoridad

        if (!hasAuthority)
        {
            //No somos la autoridad local de este objeto, pero aún así necesitamos
            //actualizar nuestra posición local para este objeto basándonos en 
            //donde esté probablemente en la pantalla del player autoridad

            bestGuessPosition = bestGuessPosition + (velocity * Time.deltaTime);

            //En vez de TELEPORTAR nuestra posición a la best guess
            //podemos lerpearla

            transform.position = Vector3.Lerp(transform.position, bestGuessPosition, Time.deltaTime * latencySmoothingFactor);

            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            this.transform.Translate(0, 1, 0);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Destroy(this.gameObject);
        }

        if (true)
        {
            //El player está pidiendo cambiar nuestra dirección/velocidad

            velocity = new Vector3(1, 0, 0);

            CmdUpdateVelocity(velocity, transform.position);
        }

    }

    [Command]
    void CmdUpdateVelocity(Vector3 v, Vector3 p)
    {
        //Estoy en el servidor
        transform.position = p;
        velocity = v;

        //Si sabemos cuál es la latencia, podemos hacer algo así
        //transform.position = p + (v * (latenciaDeEsteJugadorAlServidor))

        //Ahora hagamos que los clientes sepan la posición correcta de este objeto
        RpcUpdateVelocity(velocity, transform.position);

    }

    [ClientRpc]
    void RpcUpdateVelocity(Vector3 v, Vector3 p)
    {
        if (hasAuthority)
        {
            //Hey este es my objeto. Yo "debería" tener ya 
            //la posición/velocidad más precisa
            //Dependiendo del juego, PUEDE que quiera cambiar esta info
            //desde el servidor, a pesar de que se vea algo
            return;
        }

        //Soy un cliente sin autoridad, así que necesito definitivamente
        //escuchar al servidor

        //Si sabemos cuál es la latencia, podemos hacer algo así
        //transform.position = p + (v * (nuestraLatencia))

        //transform.position = p;

        velocity = v;
        bestGuessPosition = p + (velocity * (nuestraLatencia));


        //Ahora la posición del player está tan cerca como posible en todas las pantallas

        //DE HECHO, no queremos actualizar directamente transform.position, porque
        //los jugadores se teletransportarían constantemente
    }
}
