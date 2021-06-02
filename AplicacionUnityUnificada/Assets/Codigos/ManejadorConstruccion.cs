using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManejadorConstruccion : MonoBehaviour {
   
    private Text textoCasa;
    private Casa casa;    
    private const float separacionHab = 3f; // distancia de seperacion entre cada habitacion    
    private GameObject buttonDeshacer;
    private InputField inputNombreHabitacion;
    // Use this for initialization

    void Awake ()
    {
        casa = VariablesGlobales.Instance.casaActual;        
        textoCasa = GameObject.Find("TextCasa").GetComponent<Text>();
        buttonDeshacer = GameObject.Find("ButtonDeshacer");
        inputNombreHabitacion = GameObject.Find("InputNombre").GetComponent<InputField>();
        string texto = casa.nombre;
        if (texto.Length >= 17)
        {
            textoCasa.text = texto.Substring(0, 16);
        } else
        {
            textoCasa.text = texto;
        }       
	}      

    // Update is called once per frame
    void Update ()
    {
        //Si estoy en el modo Construccion
        if (SceneManager.GetActiveScene().name.Equals("ModoConstruccion"))
        {
            //Busco, para actualizar el text con la cantidad de habitaciones
            GameObject d = GameObject.Find("CantidadText");
            d.GetComponent<Text>().text = "Cantidad de Habitaciones: " + casa.habitaciones.Count;
            if (casa.habitaciones.Count != 0)
            {
                buttonDeshacer.SetActive(true);
            } else
            {
                buttonDeshacer.SetActive(false);
            }
        }
    }

    public void manejadorBotonCrearHabitacion()
    {
        if (!inputNombreHabitacion.text.Equals(""))//Si se ingreso un nombre a la habitacion
        {
            GameObject inputAncho = GameObject.Find("TextAncho");//Encuentro el objeto text del Ancho
            GameObject inputLargo = GameObject.Find("TextLargo");//Encuentro el objeto text del Largo
            if (!inputAncho.GetComponent<Text>().text.Equals("") && !inputLargo.GetComponent<Text>().text.Equals(""))
            {//Si puse algun valor a ambos campos, creo  las variables enteras de dichos campos
                int metrosAncho = int.Parse(inputAncho.GetComponent<Text>().text);
                int metrosLargo = int.Parse(inputLargo.GetComponent<Text>().text);
                string nombreHab = "Habitacion" + casa.habitaciones.Count;
                float inicioHab = 0;
                if (casa.habitaciones.Count != 0)
                {
                    Habitacion anterior = casa.habitaciones[casa.habitaciones.Count - 1];
                    inicioHab = anterior.posInicial + anterior.ancho + separacionHab;
                }
                Habitacion nuevaHab = GameObject.Find("GlobalObject").GetComponent<ConstructorCasa>().crearHabitacion(metrosAncho, metrosLargo, inicioHab, nombreHab, inputNombreHabitacion.text);
                if (nuevaHab != null)
                {
                    casa.agregarHabitacion(nuevaHab);
                }
            }
            else
            {
                VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.ALERTA, "Crear Habitacion", "Debe ingresar valores en Ancho y Largo.");
            }
        }
        else
        {
            VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.ALERTA, "Crear Habitacion", "Debe ingresarle un nombre a la Habitacion.");
        }
    }

    public void manejadorBotonGuardar()//En desuso, por el momento
    {
        if (casa.habitaciones.Count > 0)
        {
            ManejadorArchivos.Guardar(casa);
        } else
        {
            Debug.Log("Cree al menos una habitacion para guardar");
        }
    }

    public void manejadorBotonDeshacer()
    {
        Habitacion ultima = casa.habitaciones[casa.habitaciones.Count-1];
        GameObject.Destroy(GameObject.Find(ultima.nombre)); //Elimino el objeto habitacion de la escena       
        casa.habitaciones.Remove(ultima);      
        if (casa.habitaciones.Count != 0)
        {
            ultima = casa.habitaciones[casa.habitaciones.Count - 1];
            GameObject posCamara = GameObject.Find(ultima.nombre + "_Camara" + "Abajo");
            GameObject centroHabitacion = GameObject.Find(ultima.nombre + "_Centro");
            Camera.main.transform.position = posCamara.transform.position;
            Camera.main.transform.LookAt(centroHabitacion.transform.position);
        }       
    }

    public void manejadorBotonVolverMenu()//Esta asociado SOLO al ModoConstruccion 
    {
        if (casa.habitaciones.Count > 0)
        {
            VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.GUARDAR, "Esta Saliendo  de este modo", "Quiere guardar la Casa realizada.");
        }
        else
        {
            VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.ALERTA, "No hay Habitaciones", "Al no tener habitaciones esta casa no se guardará.");
            VariablesGlobales.Instance.casaActual.nombre = "";
            SceneManager.LoadScene("MenuPrincipal");
        }
    }
}
