using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComportamientoVentiladorTechoPrefab : MonoBehaviour
{
    private int velocidad;
    // Start is called before the first frame update
    void Start()
    {
        velocidad = 0;
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
            this.transform.Find("Cuerpo").transform.Find("ParteRotable").transform.Rotate(0,velocidad/2,0);
        }
    }
}
