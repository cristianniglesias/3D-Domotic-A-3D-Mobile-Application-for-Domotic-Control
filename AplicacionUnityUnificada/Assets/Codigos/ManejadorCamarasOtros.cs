using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManejadorCamarasOtros : MonoBehaviour
{

    public Habitacion habitacionActiva; //Habitacion que se esta Viendo
    public string[] ordenCamaras = { "Abajo", "Derecha", "Arriba", "Izquierda" }; //Palabras para el orden de las camaras
    private Casa casa; //Casa que esta cargada
    public int camaraActiva; //Entero que representa que camara es la que esta en uso de las 4
    private Text textHabitacion; //Texto con el nombre de la habitacion
    private bool cambios; // Variable que indica si se produjeron cambios, si es necesario guardar

    // private Text textHabitacionPrueba; //Se borra

    // Start is called before the first frame update
    void Start()
    {
        casa = VariablesGlobales.Instance.casaActual;//Se le asigna la referencia a la casaActual
        Debug.Log("Casa: " + casa.nombre);
        habitacionActiva = casa.habitaciones[0];//Se le da a la habitacionActiva la habitacion 0 de la casaActual
        GameObject.Find("TextCasa").GetComponent<Text>().text = casa.nombre;//Se le pone el nombre de la casa al texto
        textHabitacion = GameObject.Find("HabitacionText").GetComponent<Text>();//Se encuentra la referencia al texto del nombre de la habitacion
        textHabitacion.text = habitacionActiva.nombreFicticio;//Se le pone el nombre de la habitacion al texto
        cambios = false;

        //textHabitacionPrueba = GameObject.Find("HabitacionTextPrueba").GetComponent<Text>();//Se borra 
        //textHabitacionPrueba.text = habitacionActiva.nombre;//Se borra

        GameObject.Find("GlobalObject").GetComponent<ConstructorCasa>().construirCasa(casa);//Se manda a construir la casa (3D)
        camaraActiva = 0;//La camara activa es la de Abajo
        posicionarCamara(ordenCamaras[camaraActiva]);//Se posiciona la camara, Abajo
    }

    public Habitacion getHabitacionActiva()//Devuelve la habitacionActiva
    {
        return habitacionActiva;
    }
    private void posicionarCamara(string lado)//Posiciona la Camara Principal, en el lado elegido
    {
        Vector3 posCamara = new Vector3();
                     switch (lado)
                         {
                         case "Abajo":
                         {
                               posCamara = new Vector3(habitacionActiva.posInicial + ((float)(habitacionActiva.ancho + 1)) / 2, 6f, 0);
                               break;
                          }
                          case "Derecha":
                          {
                                posCamara = new Vector3(habitacionActiva.posInicial + habitacionActiva.ancho + 2, 6f, ((float)(habitacionActiva.largo + 1)) / 2);
                                break;
                          }
                          case "Arriba":
                          {
                                posCamara = new Vector3(habitacionActiva.posInicial + ((float)(habitacionActiva.ancho + 1)) / 2, 6f, habitacionActiva.largo + 2);
                                break;
                          }
                          case "Izquierda":
                          {
                                posCamara = new Vector3(habitacionActiva.posInicial, 6f, ((float)(habitacionActiva.largo + 1)) / 2);
                                break;
                           }
                          default:
                                break;
                       }
        GameObject centroHabitacion = GameObject.Find(habitacionActiva.nombre + "_Centro");
        Camera.main.transform.position = posCamara;
        Camera.main.transform.LookAt(centroHabitacion.transform.position);
    }
 
    
             
        
    

    private void actualizarCamara(bool opcion) //Opcion false paso a la camara anterior, opcion true paso a la camara siguiente
    {//Hace el cambio de la camaraActiva
        if (opcion == true)
        {
            camaraActiva = (camaraActiva + 1) % ordenCamaras.Length;
        }
        else
        {
            if (camaraActiva == 0)
            {
                camaraActiva = ordenCamaras.Length - 1;
            }
            else
            {
                camaraActiva--;
            }
        }
    }
    
    private void actualizarHabitacion(bool opcion) //Opcion false paso a la habitacion anterior, opcion true paso a la habitacion siguiente
    {//Hace el cambio de la habitacionActiva
        int indice = casa.habitaciones.IndexOf(habitacionActiva);
        int cant = casa.habitaciones.Count;
        if (opcion == true)
        {
            indice = (indice + 1) % cant;
        }
        else
        {
            if (indice == 0)
            {
                indice = cant - 1;
            }
            else
            {
                indice--;
            }
        }
        habitacionActiva = casa.habitaciones[indice];
        textHabitacion.text = habitacionActiva.nombreFicticio;

        //textHabitacionPrueba.text = habitacionActiva.nombre;//Se borra

    }

    public void desactivarObjetos(List<GameObject>lista)
    {
        foreach (GameObject obj in lista)
        {
            obj.SetActive(false);
        }
    }

    public string getTipoElemento(GameObject objeto)//Se devuelve el tipo de elemento del cual es el padre del Objeto
    {
        string nombrePadre = objeto.transform.parent.name;
        string[] divisionCadenas = nombrePadre.Split('_');
        int longitudCadena = divisionCadenas.Length;
        return divisionCadenas[longitudCadena - 1];
    }

    //Identifica de que tipo de objeto se trata y lo devuelve en forma de string. Si no coincide ninguno retorna vacio
    public string getTipoObjeto(GameObject obj)
    {
        if (obj.name.Contains("AireAcondicionado"))
        { return "AireAcondicionado"; }
        if (obj.name.Contains("LamparaPref"))
        { return "LamparaPref"; }
        if (obj.name.Contains("LamparaParedPref"))
        { return "LamparaParedPref"; }
        if (obj.name.Contains("Televisor"))
        { return "Televisor"; }
        if (obj.name.Contains("VentiladorPiso"))
        { return "VentiladorPiso"; }
        if (obj.name.Contains("VentiladorTecho"))
        { return "VentiladorTecho"; }
        return "";
    }

    public Casa getCasa()//Se devuelve la casa Actual
    {
        return casa;
    }

    public void setHabitacionActiva(Habitacion h) // cambia la habitacion activa y tmb actualiza la posicion de la camara 
    {
        if (h != null)
        {
            habitacionActiva = h;
            posicionarCamara("Abajo");
        }         
    }

    public void manejadorBotonCamaraSiguiente()
    {
        actualizarCamara(true);
        rotarCamaraDer(ordenCamaras[camaraActiva]);
    }

    public void manejadorBotonCamaraAnterior()
    {
        actualizarCamara(false);
        rotarCamaraIzq(ordenCamaras[camaraActiva]);
    }

    public void manejadorBotonHabitacionSiguiente()
    {
        actualizarHabitacion(true);
        posicionarCamara(ordenCamaras[camaraActiva]);
    }

    public void manejadorBotonHabitacionAnterior()
    {
        actualizarHabitacion(false);
        posicionarCamara(ordenCamaras[camaraActiva]);
    }

    public void manejadorBotonMenu()//Boton para volver al MenuPrincipal
    {
        if (!SceneManager.GetActiveScene().name.Equals("ModoUtilizacion"))
        {//Si no es el ModoUtilizacion, cuando quiero volver al menu, abro la ventana de si desea guardar cambios
            if (cambios) //Si se produjeron cambios
            {
                VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.GUARDAR, "Esta Saliendo  de este modo", "Quiere guardar los cambios realizados.");
                cambios = false;
            }
            else
            {
                SceneManager.LoadScene("MenuPrincipal");
            }
        }
        else
        {
            SceneManager.LoadScene("MenuPrincipal");
        }
    }

    public void manejadorBotonGuardar()
    {
        if (cambios) //Si se produjeron cambios
        {
            VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.GUARDAR, " ",  "¿Desea guardar los cambios realizados?.");
            cambios = false;
        }
    }
    public void cambioRealizado()
    {
        cambios = true;
    }
    public IEnumerator MoverCamara(Vector3 posFut)
    {
        GameObject centroHabitacion = GameObject.Find(habitacionActiva.nombre + "_Centro");
        float tiempoComienzo = 0f;
        while (true)
        {
            tiempoComienzo += Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, posFut, tiempoComienzo);
            Camera.main.transform.LookAt(centroHabitacion.transform.position);
            // If the object has arrived, stop the coroutine
            if (Camera.main.transform.position == posFut)
            {
                yield break;
            }

            // Otherwise, continue next frame
            yield return null;
        }
    }
    private void rotarCamaraDer(string lado)
    {
        
        Vector3 posSig = new Vector3();
        switch (lado)
        {
            case "Derecha":
                {
                    
                    posSig = new Vector3(habitacionActiva.posInicial + habitacionActiva.ancho + 2, 6f, 0);
                    StartCoroutine(MoverCamara(posSig));
                    posSig = new Vector3(habitacionActiva.posInicial + habitacionActiva.ancho + 2, 6f, ((float)(habitacionActiva.largo + 1)) / 2);
                    StartCoroutine(MoverCamara(posSig));
                    break;
                }
            case "Arriba":
                {
                    posSig = new Vector3(habitacionActiva.posInicial + habitacionActiva.ancho + 2, 6f, habitacionActiva.largo + 2);
                    StartCoroutine(MoverCamara(posSig));
                    posSig = new Vector3(habitacionActiva.posInicial + ((float)(habitacionActiva.ancho + 1)) / 2, 6f, habitacionActiva.largo + 2);
                    StartCoroutine(MoverCamara(posSig));
                    break;
                }
            case "Izquierda":
                {
                    posSig = new Vector3(habitacionActiva.posInicial, 6f, habitacionActiva.largo + 2);
                    StartCoroutine(MoverCamara(posSig));
                    posSig = new Vector3(habitacionActiva.posInicial, 6f, ((float)(habitacionActiva.largo + 1)) / 2);
                    StartCoroutine(MoverCamara(posSig));
                    break;
                }
            case "Abajo":
                {
                    posSig = new Vector3(habitacionActiva.posInicial, 6f, ((float)(habitacionActiva.largo + 1)) / 2);
                    StartCoroutine(MoverCamara(posSig));
                    posSig = new Vector3(habitacionActiva.posInicial + ((float)(habitacionActiva.ancho + 1)) / 2, 6f, 0);
                    StartCoroutine(MoverCamara(posSig));
                    break;
                }
        }

    }
    private void rotarCamaraIzq(string lado)
    {
        Vector3 posSig = new Vector3();
        switch (lado)
        {
            case "Abajo":
                {
                    posSig = new Vector3(habitacionActiva.posInicial + habitacionActiva.ancho + 2, 6f, 0);
                    StartCoroutine(MoverCamara(posSig));
                    posSig = new Vector3(habitacionActiva.posInicial + ((float)(habitacionActiva.ancho + 1)) / 2, 6f, 0);
                    StartCoroutine(MoverCamara(posSig));
                    break;
                }
            case "Derecha":
                {
                    posSig = new Vector3(habitacionActiva.posInicial + habitacionActiva.ancho + 2, 6f, habitacionActiva.largo + 2);
                    StartCoroutine(MoverCamara(posSig));
                    posSig = new Vector3(habitacionActiva.posInicial + habitacionActiva.ancho + 2, 6f, ((float)(habitacionActiva.largo + 1)) / 2);
                    StartCoroutine(MoverCamara(posSig));
                    break;
                }
            case "Arriba":
                {
                    posSig = new Vector3(habitacionActiva.posInicial, 6f, habitacionActiva.largo + 2);
                    StartCoroutine(MoverCamara(posSig));
                    posSig = new Vector3(habitacionActiva.posInicial + ((float)(habitacionActiva.ancho + 1)) / 2, 6f, habitacionActiva.largo + 2);
                    StartCoroutine(MoverCamara(posSig));
                    break;
                }
            case "Izquierda":
                {
                    posSig = new Vector3(habitacionActiva.posInicial + ((float)(habitacionActiva.ancho + 1)) / 2, 6f, 0);
                    StartCoroutine(MoverCamara(posSig));
                    posSig = new Vector3(habitacionActiva.posInicial, 6f, ((float)(habitacionActiva.largo + 1)) / 2);
                    StartCoroutine(MoverCamara(posSig));
                    break;
                }
        }

    }

    public Vector3 checkZoom(Vector3 posTouch)
    {
        Vector3 zoom = new Vector3();
        switch (ordenCamaras[camaraActiva])
        {
            case "Abajo":
                {
                    zoom = new Vector3(habitacionActiva.posInicial + ((float)(habitacionActiva.ancho + 1)) / 2, 4f, habitacionActiva.largo / (5 / 2));
                    break;
                }
            case "Derecha":
                {
                    zoom = new Vector3(habitacionActiva.ancho / (5 / 3), 4f, ((float)(habitacionActiva.largo + 1)) / 2);
                    break;
                }
            case "Arriba":
                {
                    zoom = new Vector3(habitacionActiva.posInicial + ((float)(habitacionActiva.ancho + 1)) / 2, 4f, habitacionActiva.largo / (5 / 3));
                    break;
                }
            case "Izquierda":
                {
                    zoom = new Vector3(habitacionActiva.ancho / (5 / 2), 4f, ((float)(habitacionActiva.largo + 1)) / 2);
                    break;
                }
            default:
                break;
        }
        return zoom;
    }

    public IEnumerator ZoomIn(Vector3 posFut, Vector3 direc)
    {
        float tiempoComienzo = 0f;
        while (true)
        {
            tiempoComienzo += Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, posFut, tiempoComienzo);
            Camera.main.transform.LookAt(direc);
            // If the object has arrived, stop the coroutine
            if (Camera.main.transform.position == posFut)
            {
                yield break;
            }

            // Otherwise, continue next frame
            yield return null;
        }
    }



}
