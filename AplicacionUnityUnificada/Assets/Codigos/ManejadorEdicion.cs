using UnityEngine;
using UnityEngine.UI;

public class ManejadorEdicion : MonoBehaviour
{
    private ManejadorCamarasOtros auxiliar;

    // Start is called before the first frame update
    void Start()
    {        
        auxiliar = GameObject.Find("CodigosModoEdicion").GetComponent<ManejadorCamarasOtros>();        
    }
    // Update is called once per frame
    void Update()
    {
        //Busco, para actualizar el text con la cantidad de habitaciones
        GameObject textoCantidad = GameObject.Find("CantidadText");
        textoCantidad.GetComponent<Text>().text = "Cantidad de Habitaciones: " + auxiliar.getCasa().habitaciones.Count;
        if (auxiliar.getCasa().habitaciones.Count != 0)
        {
            GameObject.Find("PanelHabitacion").transform.Find("ButtonEliminar").gameObject.SetActive(true);
        }
        else
        {
            GameObject.Find("PanelHabitacion").transform.Find("ButtonEliminar").gameObject.SetActive(false);
        }
        
    }

    //Manejador que se le agrega al boton crear, para que mueva la camara a la habitacion creada y ademas indica que se produjo un cambio para guardar el archivo
    public void manejadorCrearEdicion()
    {
        int indiceHabitacionActiva = auxiliar.getCasa().buscarHabitacion(auxiliar.getHabitacionActiva().nombre);
        for (int i = indiceHabitacionActiva; i < auxiliar.getCasa().habitaciones.Count-1; i++)//Desde la posicion en la que estoy hasta la de la habitacion nueva
        {
            auxiliar.manejadorBotonHabitacionSiguiente();
        }
        auxiliar.cambioRealizado();//Indico que se produjo un cambio para guardar el Archivo
    }

    public void manejadorBotonEliminar()
    {
        Habitacion habitacionEliminar = auxiliar.getHabitacionActiva();//Me quedo con la referencia de la Habitacion a eliminar
        auxiliar.manejadorBotonHabitacionSiguiente();//Muevo la camara
        GameObject.Destroy(GameObject.Find(habitacionEliminar.nombre));//Elimino el objeto habitacion (3D) de la escena       
        int indiceEliminar = auxiliar.getCasa().buscarHabitacion(habitacionEliminar.nombre);//Me quedo con el indice de la habitacion que deseo eliminar
        auxiliar.getCasa().habitaciones.RemoveAt(indiceEliminar);//Elimino la habitacion de la lista en la casa  
        corrimientoNombresHabitacionesLista(indiceEliminar);//Corro los nombres de la Lista de Habitaciones
        corrimientoNombresHabitaciones3D(indiceEliminar);//Corro los nombres de los objetos 3D
        GameObject.Find("HabitacionText").GetComponent<Text>().text = auxiliar.getHabitacionActiva().nombreFicticio;//Actualizo el texto con el nombre de la habitacion nueva (se agrego, porque el cambio de camara se tiene que hacer antes, sino falla)
        auxiliar.cambioRealizado();//Indico que se produjo un cambio para guardar el Archivo

        GameObject.Find("HabitacionTextPrueba").GetComponent<Text>().text = auxiliar.getHabitacionActiva().nombre;//Para chekeos, Se borra
    }

    //Sirve para cambiar los nombres de las Habitaciones en la lista
    private void corrimientoNombresHabitacionesLista(int indice)
    {
        for(int i=indice;i<auxiliar.getCasa().habitaciones.Count;i++)
        {
            string nombreACambiar = auxiliar.getCasa().habitaciones[i].nombre;
            int numero = int.Parse(nombreACambiar.Substring(10))-1;
            auxiliar.getCasa().habitaciones[i].nombre = "Habitacion" + numero;
         }
    }

    //Sirve para cambiar los nombres de las Habitaciones 3D
    private void corrimientoNombresHabitaciones3D(int indice)
    {
        GameObject[] todos = GameObject.FindObjectsOfType<GameObject>();
        for (int j = 0; j < auxiliar.getCasa().habitaciones.Count; j++)
        {
            for (int i = 0; i < todos.Length; i++)
            {
                if (todos[i].name.Contains("Habitacion" + (indice + 1)))
                {
                    string[] partesNombre = todos[i].transform.name.Split('_');

                    todos[i].transform.name = "Habitacion" + indice;
                    if (partesNombre.Length > 1)
                    {
                        todos[i].name = todos[i].name + "_" + partesNombre[1];
                    }
                }
            }
            indice++;
        }
    }
}
