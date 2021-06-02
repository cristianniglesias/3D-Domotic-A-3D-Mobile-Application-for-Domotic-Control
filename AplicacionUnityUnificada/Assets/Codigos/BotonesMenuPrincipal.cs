using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class BotonesMenuPrincipal : MonoBehaviour
{
    private Dropdown barritaCasa;
    private InputField inputCasa;
    private Button botonConstruir;
    private GameObject panelSinCasa, panelConCasa, panelConfiguracion, panelConexion;
    
    // Use this for initialization
    void Start()
    {
        barritaCasa = GameObject.Find("DropdownCasas").GetComponent<Dropdown>();//Me quedo con la referencia a la barrita
        barritaCasa.ClearOptions();//Vacio la barrita
        barritaCasa.gameObject.SetActive(false);//La deshabilito, para que no se vea     
        inputCasa = GameObject.Find("InputCasa").GetComponent<InputField>();
        panelSinCasa = GameObject.Find("PanelSinCasa");//Sin Casa Cargada
        panelConCasa = GameObject.Find("PanelConCasa");//Con Casa Cargada
        panelConfiguracion = GameObject.Find("PanelConfiguracion");//Configuracion de los elementos
        panelConfiguracion.SetActive(false);//Deshabilito el panel de Configuracion
        panelConexion = GameObject.Find("PanelConexion");//Para Conectar y mostrar estado de conexion
        panelConexion.SetActive(false);//Deshabilito el panel de Conexion


        if (VariablesGlobales.Instance.casaActual.nombre.Equals(""))
            panelConCasa.SetActive(false);
        else
            panelSinCasa.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (panelConCasa.activeSelf || panelConfiguracion.activeSelf)//Si estoy en el panel de una Casa cargada o su configuracion
        {
            //Si tiene una IP guardada, muestra el panel Conexion
            if (VariablesGlobales.Instance.casaActual.IPBroker[0] != -1)
            {
                panelConexion.SetActive(true);//Habilito panel Conexion 
                if (VariablesGlobales.Instance.auxiliarMQTT.client!=null && VariablesGlobales.Instance.auxiliarMQTT.client.IsConnected)
                {
                    panelConexion.transform.Find("ButtonConectarCasa").gameObject.SetActive(false);                   
                }
                else
                {
                    panelConexion.transform.Find("ButtonConectarCasa").gameObject.SetActive(true);
                }
                controlarImagenConexion();
            }
            else
            {
                panelConexion.SetActive(false);//Deshabilito panel Conexion
            }
        }
    }
    private bool existeNombre(string nombreCasa)
    {
        bool existe = false;
        List<string> archivosCasa = ManejadorArchivos.GetListaArchivos();
        for(int i=0; i<archivosCasa.Count;i++)
        {
            if(archivosCasa[i].Equals(nombreCasa))
            {
                existe = true;
                break;
            }
        }
        return existe;
    }

    public void manejadorBotonConstruccion()
    {
        string nombreCasa = inputCasa.text;
        if (!nombreCasa.Equals(""))
        {
            if (!existeNombre(inputCasa.text))
            {
                VariablesGlobales.Instance.casaActual = new Casa(nombreCasa);
                SceneManager.LoadScene("ModoConstruccion");
            }
            else
            {
                VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.ALERTA, "No se puede crear esa casa", "Ya existe una casa con ese nombre");
            }
        }
        else
        {
            VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.ALERTA, "No se puede crear esa casa", "Ingrese nombre para la casa");
        }
    }

    public void manejadorBotonSalir()
    {
        Application.Quit();
    }

    public void manejadorBotonCargarCasas()
    {
        List<string> archivosCasa = ManejadorArchivos.GetListaArchivos();
        if (archivosCasa.Count != 0)
        {
            //A la barrita, la habilito
            barritaCasa.gameObject.SetActive(true);
            //Cargo la barrita con la lista de  los .map de las casas encontradas
            barritaCasa.ClearOptions();//Vacio la barrita
            barritaCasa.AddOptions(archivosCasa);//Agrego la lista de string
            GameObject.Find("ButtonCargar").SetActive(true);
        }
        else
        {
            VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.ALERTA, "No hay Casas", "No se encontraron Casas creadas");
        }

    }    

    public void manejadorBotonCargar()
    {
        string casaSeleccionada = barritaCasa.options[barritaCasa.value].text;
        if (!casaSeleccionada.Equals(""))
        {
            VariablesGlobales.Instance.casaActual = ManejadorArchivos.Cargar(casaSeleccionada);
            panelSinCasa.SetActive(false);
            panelConCasa.SetActive(true);

            //Si tiene una IP guardada, muestra el panel Conexion
            if (VariablesGlobales.Instance.casaActual.IPBroker[0] != -1)
            {
                panelConexion.SetActive(true);//Habilito panel Conexion
                controlarImagenConexion();
            }
        }    
    }

    public void manejadorBotonConectarCasa()
    {
        //Si tiene una IP guardada, intenta conectarse
        if (VariablesGlobales.Instance.casaActual.IPBroker[0] != -1)
        {
            if (VariablesGlobales.Instance.auxiliarMQTT.conexionBroker(VariablesGlobales.Instance.casaActual.IPBroker))
            {
                VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.BIEN, "Autoconexión al Broker", "Conexión establecida");
            }
            else
            {
                VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.ERROR, "Autoconexión al Broker", "No se pudo establecer conexión");
            }
            controlarImagenConexion();
        }
    }

    private void controlarImagenConexion()
    {
        //Si tiene una IP guardada
        if (VariablesGlobales.Instance.casaActual.IPBroker[0] != -1)
        {
            if (VariablesGlobales.Instance.auxiliarMQTT.client != null && VariablesGlobales.Instance.auxiliarMQTT.client.IsConnected)//Pongo imagen, dependiendo si esta conectado o no
            {
                panelConexion.transform.Find("ImagenConexion").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icono Conectado");
            }
            else
            {
                panelConexion.transform.Find("ImagenConexion").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icono Desconectado");
            }
        }
    }

    public void manejadorBotonEditar()
    {
        SceneManager.LoadScene("ModoEdicion");
    }

    public void manejadorBotonColocarObjetos()
    {
        SceneManager.LoadScene("ModoColocacion");
    }

    public void manejadorBotonUsar()
    {
        SceneManager.LoadScene("ModoUtilizacion");
    }

    public void manejadorBotonConfiguracion()
    {
        panelConCasa.SetActive(false);
        panelConfiguracion.SetActive(true);
    }

    public void manejadorBotonVolverConCasa()
    {
        panelConfiguracion.SetActive(false);
        panelConCasa.SetActive(true);
    }

    public void manejadorBotonVolverPrincipal()
    {
        panelConCasa.SetActive(false);
        panelConexion.SetActive(false);
        panelSinCasa.SetActive(true);
        VariablesGlobales.Instance.auxiliarMQTT.limpiarMQTT();
    }

    public void manejadorBotonEliminar()
    {        
        string casaSeleccionada = barritaCasa.options[barritaCasa.value].text; //Tomo el valor de la opcion seleccionada del dropdown        
        if (!casaSeleccionada.Equals(""))
        {
            VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.ELIMINAR, "Borrar Casa", "Esta seguro de eliminar esta Casa: "+casaSeleccionada);
            List<string> archivosCasa = ManejadorArchivos.GetListaArchivos();
            if (archivosCasa.Count != 0)
            {                                
                barritaCasa.ClearOptions();//Vacio la barrita                
                barritaCasa.AddOptions(archivosCasa);//Agrego la lista de string 
                barritaCasa.value = 0; // Dejo seleccionado el primer elemento del dropdown                
            } else
            {
                barritaCasa.gameObject.SetActive(false);
            }
        }
    }

}
