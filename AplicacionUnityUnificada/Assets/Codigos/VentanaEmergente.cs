using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum tipoVentana { AYUDA, ALERTA, BIEN, CARGANDO,DECISION, ELIMINAR, ERROR, GUARDAR}
public class VentanaEmergente : MonoBehaviour
{
    private GameObject canvasVentana;
    private GameObject ventana;
    private GameObject panelAlerta;
    private GameObject panelCargando;
    private GameObject panelDecision;

    private Button botonAceptarDecision;
    private Button botonCancelarDecision;

    private Text tituloVentana;
    private Text mensajeVentana;
    private Image imagenVentana;

    private struct VentanaGuardada
    {//Estructura para guardar todos los elementos de una ventana
        public tipoVentana _tipo;
        public string _titulo;
        public string _mensaje;
    }
    List<VentanaGuardada> bufferVentanas;//Buffer donde se guardan las ventanas que deben abrirse (De tipo FIFO)
    
    // Start is called before the first frame update
    void Start()
    {
        //Me quedo con las referencias de:
        canvasVentana = GameObject.Find("CanvasVentana");//Del Canvas de la Ventana
        ventana = canvasVentana.transform.Find("PanelVentanaEmergente").gameObject;//Panel de la Ventana Emergente
        tituloVentana = ventana.transform.Find("TextTitulo").GetComponent<Text>();//Del Texto del Titulo
        mensajeVentana = ventana.transform.Find("TextCentro").GetComponent<Text>();//Del Texto del Centro (Mensaje)
        imagenVentana = ventana.transform.Find("ImagenVentana").GetComponent<Image>();
        panelAlerta = ventana.transform.Find("PanelAlerta").gameObject;//Del Panel Alerta(Con un boton Aceptar)
        panelCargando = ventana.transform.Find("PanelCargando").gameObject;//Del Panel Cargando(Sin botones, con barrita de carga)
        panelDecision = ventana.transform.Find("PanelDecision").gameObject;//Del Panel Decision(Con un boton Aceptar y un Cancelar)

        botonAceptarDecision = panelDecision.transform.Find("ButtonAceptarVentana").GetComponent<Button>();
        botonCancelarDecision = panelDecision.transform.Find("ButtonCancelarVentana").GetComponent<Button>();

        bufferVentanas = new List<VentanaGuardada>();

        //Desactivo los Paneles y la Ventana
        panelAlerta.SetActive(false);
        panelCargando.SetActive(false);
        panelDecision.SetActive(false);
        canvasVentana.SetActive(false);
    }

    public void mostrarVentana(tipoVentana tipo,string titulo,string mensaje)
    {
        if (canvasVentana.activeSelf == false)//Si no hay ninguna ventana mostrada
        {
            //Activo la Ventana y le doy el valor al texto del Titulo y al Mensaje
            canvasVentana.SetActive(true);
            tituloVentana.text = titulo;
            mensajeVentana.text = mensaje;
            //Dependiendo el tipo de ventana, que panel activo e icono cargo
            switch (tipo)
            {
                case tipoVentana.AYUDA:
                    {
                        panelAlerta.SetActive(true);
                        imagenVentana.sprite = Resources.Load<Sprite>("Icono Ayuda");
                        break;
                    }
                case tipoVentana.ALERTA:
                    {
                        panelAlerta.SetActive(true);
                        imagenVentana.sprite = Resources.Load<Sprite>("Icono Alerta");
                        break;
                    }
                case tipoVentana.BIEN:
                    {
                        panelAlerta.SetActive(true);
                        imagenVentana.sprite = Resources.Load<Sprite>("Icono Bien");
                        break;
                    }
                case tipoVentana.CARGANDO:
                    {
                        //Formato del titulo, "Cargando-5", la primer etapa es el titulo, el segundo el tiempo en segundos
                        tituloVentana.text = titulo.Split('-')[0];
                        imagenVentana.sprite = Resources.Load<Sprite>("Icono Cargando");
                        StartCoroutine(esperarTiempo(int.Parse(titulo.Split('-')[1])));
                        break;
                    }
                case tipoVentana.ELIMINAR:
                    {
                        panelDecision.SetActive(true);
                        asociarManejadoresBotones(tipo);
                        imagenVentana.sprite = Resources.Load<Sprite>("Icono Alerta");
                        break;
                    }
                case tipoVentana.ERROR:
                    {
                        panelAlerta.SetActive(true);
                        imagenVentana.sprite = Resources.Load<Sprite>("Icono Error");
                        break;
                    }
                case tipoVentana.GUARDAR:
                    {
                        panelDecision.SetActive(true);
                        asociarManejadoresBotones(tipo);
                        imagenVentana.sprite = Resources.Load<Sprite>("Icono Guardar");
                        break;
                    }
                default:
                    {
                        panelAlerta.SetActive(true);
                        mensajeVentana.text += "\nOcurrio un Error inesperado, Error en el tipo de ventana.";
                        imagenVentana.sprite = Resources.Load<Sprite>("Icono Error");
                        break;
                    }
            }
        }
        else//Si ya hay una ventana mostrada
        {
            //Creo una VentanaGuardada
            VentanaGuardada b=new VentanaGuardada();
            b._tipo = tipo;
            b._titulo = titulo;
            b._mensaje = mensaje;
            bufferVentanas.Add(b);//Y la guardo en el buffer de Ventanas a mostrar
        }
    }


    IEnumerator esperarTiempo(int segundos)
    {
        panelCargando.SetActive(true);//Muestro Ventana Cargando
        mensajeVentana.text = "Cargando";
        Slider cargando = GameObject.Find("SliderCargando").GetComponent<Slider>();
        cargando.value = 0;
        float rotacion = (float)360/segundos;
        for (int i = 1; i <= segundos; i++)
        {
            yield return new WaitForSeconds(1);//Espero un segundo y le agrego un "." al texto
            mensajeVentana.text = mensajeVentana.text + ".";
            imagenVentana.transform.Rotate(0,0, rotacion);
            cargando.value = (float)i / segundos;
        }
        //Desactivo el Panel Cargando y la Ventana
        panelCargando.SetActive(false);
        canvasVentana.SetActive(false);
    }

    private void revisionVentanasGuardada()
    {
        if (bufferVentanas.Count != 0)//Si hay ventanas por abrir
        {
            //Muestro la primer ventana guardada
            mostrarVentana(bufferVentanas[0]._tipo, bufferVentanas[0]._titulo, bufferVentanas[0]._mensaje);
            //Y la borro del buffer
            bufferVentanas.RemoveAt(0);
        }
    }

    public void manejadorBotonAceptarAlerta()//Manejador del boton Aceptar del Panel Alerta
    {
        //Desactivo el Panel Alerta y la Ventana
        panelAlerta.SetActive(false);
        canvasVentana.SetActive(false);
        //Reviso de que no haya ventanas por abrir
        revisionVentanasGuardada();
    }

    private void asociarManejadoresBotones(tipoVentana tipo)
    {
        switch (tipo)
        {
            case tipoVentana.ELIMINAR:
            {
                botonAceptarDecision.onClick.AddListener(manejadorBotonEliminarCasa);
                botonCancelarDecision.onClick.AddListener(cerrarVentanaPanelDecision);
                break;
            }
            case tipoVentana.GUARDAR:
            {
                    string nombreEscena = SceneManager.GetActiveScene().name;
                    if (nombreEscena.Equals("MenuPrincipal"))//Para guardado en Modo Configuracion
                    {
                        botonAceptarDecision.onClick.AddListener(manejadorBotonGuardarCasa);
                        botonCancelarDecision.onClick.AddListener(manejadorBotonVolverConCasa);
                    }
                    else
                    {
                        if(nombreEscena.Equals("ModoConstruccion"))
                        {
                            botonAceptarDecision.onClick.AddListener(manejadorGuardarNuevaCasa);
                            botonCancelarDecision.onClick.AddListener(manejadorBotonVolverSinCasa);
                        }
                        else
                        {
                            if (nombreEscena.Equals("ModoColocacion"))
                            {
                                botonAceptarDecision.onClick.AddListener(manejadorActualizarCasa);
                                botonCancelarDecision.onClick.AddListener(manejadorBotonVolverSinGuardar);

                            }
                            else //Para guardado en Modo Edicion
                            {
                                botonAceptarDecision.onClick.AddListener(manejadorGuardarCasaExistente);
                                botonCancelarDecision.onClick.AddListener(manejadorBotonVolverAlMenuPrincipal);
                            }
                        }
                    }
                break;
            }
            default:
            {
                mensajeVentana.text += "\nOcurrio un Error inesperado, Error en la asociacion de botones.\nSe asocio para cerrar la ventana.";
                botonAceptarDecision.onClick.AddListener(cerrarVentanaPanelDecision);
                botonCancelarDecision.onClick.AddListener(cerrarVentanaPanelDecision);
                break;
            }
        }
    }
    private void limpiarManejadoresBotones()
    {
        botonAceptarDecision.onClick.RemoveAllListeners();
        botonCancelarDecision.onClick.RemoveAllListeners();
    }

    public void manejadorBotonEliminarCasa()
    {
        string[] partesMensaje = mensajeVentana.text.Split(' ');
        string nombreCasaBorrar = partesMensaje[partesMensaje.Length - 1];
        ManejadorArchivos.Eliminar(nombreCasaBorrar);
        cerrarVentanaPanelDecision();
        mostrarVentana(tipoVentana.BIEN, "Eliminacion Realizada", "Se elimino la casa.");
        GameObject.Find("CodigosMenuPrincipal").GetComponent<BotonesMenuPrincipal>().manejadorBotonCargarCasas();
    }
    public void cerrarVentanaPanelDecision()//Desactivo el Panel Decision, la Ventana y limpio los botones
    {
        panelDecision.SetActive(false);
        canvasVentana.SetActive(false);
        limpiarManejadoresBotones();

        //Reviso de que no haya ventanas por abrir
        revisionVentanasGuardada();
    }
    public void manejadorBotonGuardarCasa()//Contemplado para Modo Configuracion
    {
        cerrarVentanaPanelDecision();
        ManejadorArchivos.Guardar(VariablesGlobales.Instance.casaActual);
        mostrarVentana(tipoVentana.BIEN, "Guardado Realizado", "El guardado de la casa: " + VariablesGlobales.Instance.casaActual.nombre + " se realizó con éxito.");
    }
    public void manejadorBotonVolverSinGuardar()
    {
        cerrarVentanaPanelDecision();
        manejadorBotonVolverConCasa();
    }
    public void manejadorGuardarCasaExistente()//Contemplado para Modo Edicion 
    {
        manejadorBotonGuardarCasa();
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void manejadorActualizarCasa()//Contemplado para Modo Colocacion
    {
        manejadorBotonGuardarCasa();
    }

    public void manejadorGuardarNuevaCasa()//Contemplado para Modo Construccion
    {
        manejadorBotonGuardarCasa();
        VariablesGlobales.Instance.casaActual.nombre = "";
        SceneManager.LoadScene("MenuPrincipal");
    }
    public void manejadorBotonVolverConCasa()//Contemplado para Modo Configuracion
    {
        cerrarVentanaPanelDecision();
        //Cuando vuelvo al menu, me tengo que asegurar que la casa no haya quedado con cambios, entonces la cargo
        VariablesGlobales.Instance.casaActual = ManejadorArchivos.Cargar(VariablesGlobales.Instance.casaActual.nombre);
    }
    public void manejadorBotonVolverAlMenuPrincipal()//Contemplado para Modo Edicion 
    {
        manejadorBotonVolverConCasa();
        SceneManager.LoadScene("MenuPrincipal");        
    }
    public void manejadorBotonVolverSinCasa()//Contemplado para Modo Construccion
    {
        cerrarVentanaPanelDecision();
        VariablesGlobales.Instance.casaActual.nombre = "";
        SceneManager.LoadScene("MenuPrincipal");
    }

    // Update is called once per frame
    void Update()
    {

    }
    
}
