using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComportamientoTelevisionPrefab : MonoBehaviour
{
    private int _canal;
    private Color[] colorLuz = { Color.green, Color.red, Color.yellow, Color.cyan };
    private int indiceLuz;
    private bool _estado;
    // Start is called before the first frame update
    void Start()
    {
        _canal = 2;
        indiceLuz = 0;
        _estado = false;
    }

    public bool estado
    {
        get{return _estado;}
    }

    public int canal
    {
        get { return _canal; }
    }

    public void apagarTV()
    {
       this.transform.Find("Luz").gameObject.SetActive(false);
        _estado = false;
    }

    public void prenderTV()
    {
        this.transform.Find("Luz").gameObject.SetActive(true);
        _estado = true;
    }

    public void subirCanal()
    {
        if (_canal < 99 )
            _canal++;
        else
            _canal = 2;
        cambioCanalSubirLuz();
    }

    public void bajarCanal()
    {
        if (_canal > 2)
            _canal--;
        else
            _canal = 99;
        cambioCanalBajarLuz();
    }

    public void cambiarCanal(int nuevoCanal)
    {
        if ((nuevoCanal < 100 && nuevoCanal > 1)&& (nuevoCanal != _canal))
        {
            _canal = nuevoCanal;
            cambioCanalSubirLuz();
        }
    }

    private void cambioCanalSubirLuz()
    {
        if (indiceLuz < colorLuz.Length-1)
            indiceLuz++;
        else
            indiceLuz = 0;
        this.GetComponentInChildren<Light>().color = colorLuz[indiceLuz];
    }

    private void cambioCanalBajarLuz()
    {
        if (indiceLuz > 0)
            indiceLuz--;
        else
            indiceLuz = colorLuz.Length - 1;
        this.GetComponentInChildren<Light>().color = colorLuz[indiceLuz];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
