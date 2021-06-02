using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComportamientoVentiladorPisoPrefab : MonoBehaviour
{
    private int velocidad;//Velocidad de la rotacion
    private bool sentido;//El sentido de la rotacion
    private int rotado;//Cuanto roto
    // Start is called before the first frame update
    void Start()
    {
        velocidad = 0;
        sentido = true;
        rotado = 0;
    }

    public void velocidad1()
    {
        velocidad = 10;
    }

    public void velocidad2()
    {
        velocidad = 22;
    }

    public void velocidad3()
    {
        velocidad = 40;
    }

    public void apagar()
    {
        velocidad = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (velocidad != 0)
        {//Rotar la parte rotable, en relacion a la velocidad
            if (sentido)
            {
                this.transform.Find("Cuerpo").transform.Find("ParteRotable").transform.Rotate(0, velocidad / 2, 0);
                rotado+=velocidad;
                if (rotado>=100)
                {
                    sentido = !sentido;
                }
            }
            else
            {
                this.transform.Find("Cuerpo").transform.Find("ParteRotable").transform.Rotate(0, - (velocidad / 2), 0);
                rotado-=velocidad;
                if (rotado<=-100)
                {
                    sentido = !sentido;
                }
            }

        }
    }
}
