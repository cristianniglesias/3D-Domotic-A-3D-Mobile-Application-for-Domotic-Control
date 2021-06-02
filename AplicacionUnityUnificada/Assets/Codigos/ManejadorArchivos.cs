using System.Collections.Generic;
using System.IO;
using UnityEngine;


public static class ManejadorArchivos {


#if UNITY_STANDALONE_WIN
    //Si compila para windows, la Ruta del Escritorio:
    private static string rutaGuardado = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop).ToString();
    private const char separadorRutas = '\\';
#endif
#if UNITY_ANDROID
    //Si compila para Android, la ruta es:
    private static string rutaGuardado = Application.persistentDataPath;
    private const char separadorRutas = '/';
    //El Application.persistentDataPath es la ruta especial
    //Se encuentra en C:\Users\CONICET\AppData\LocalLow\DefaultCompany\AplicacionUnity
    //Las ultimas dos carpetas estan dadas por las configuraciones del proyecto ("Compania"/NombreProyecto")
#endif
    private static string cadenaJSON;
    private static string direccionArchivo;

    public static void Guardar(Casa casa)
    {
        cadenaJSON = JsonUtility.ToJson(casa, true);        
        direccionArchivo = rutaGuardado + Path.DirectorySeparatorChar + casa.nombre + ".map";
        //direccionArchivo = rutaGuardado + "\\" + casa.nombre + ".map";
        File.WriteAllText(direccionArchivo, cadenaJSON);
    }

    public static Casa Cargar(string nombreCasa)
    {
        direccionArchivo = rutaGuardado + Path.DirectorySeparatorChar + nombreCasa + ".map";
        Debug.Log(rutaGuardado + direccionArchivo);
        //direccionArchivo = rutaGuardado + "\\" + nombreCasa + ".map";
        Casa casa = null;
        if (File.Exists(direccionArchivo))
        {            
            cadenaJSON = File.ReadAllText(direccionArchivo); 
            casa = JsonUtility.FromJson<Casa>(cadenaJSON);
            Debug.Log(casa.habitaciones.Count);
        } 
        return casa;
    }

    public static List<string> GetListaArchivos()
    {
        List<string> listaArchivos = new List<string>();//Creo lista de string
        foreach (string file in System.IO.Directory.GetFiles(rutaGuardado))
        {//Foreach con la busqueda de archivos la ruta
            if (file.EndsWith(".map"))//Si el archivo es .map
            {
                //string[] cadena = file.Split('\\');//La ruta al archivo, separando en '\'
                string[] cadena = file.Split(Path.DirectorySeparatorChar);//La ruta al archivo, separando en '\'
                cadena = cadena[cadena.Length - 1].Split('.');//Al nombre del archivo.map, separo en '.'
                listaArchivos.Add(cadena[0]);//Agrego a la lista, solo el nombre del archivo
            }
        }
        return listaArchivos;
    }

    public static void Eliminar(string nombreCasa)
    {
        direccionArchivo = rutaGuardado + Path.DirectorySeparatorChar + nombreCasa + ".map";
        //direccionArchivo = rutaGuardado + "\\" + nombreCasa + ".map";
        if (File.Exists(direccionArchivo))
        {
            File.Delete(direccionArchivo);
            VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.BIEN, "Borrado Exitoso", "La Casa fue borrada con éxito.");
        } else
        {
            VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.ERROR, "Error de Borrado", "No se encuentra el archivo que de sea eliminar.");
        }
    }
}
