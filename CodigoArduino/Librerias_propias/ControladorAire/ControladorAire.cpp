#include "ControladorAire.h"
 
ControladorAire::ControladorAire(int RS,int EN,int D4,int D5,int D6,int D7)
{
    lcd = new LiquidCrystal(RS, EN, D4, D5, D6, D7);
    lcd->begin(16,2); // inicializar al lcd con 2 filas y 16 columnas   
    estado = false;
    temp = 24;
    split = false;
    mostrarInfo();
    lcd->noDisplay();   
}

void ControladorAire::mostrarInfo()
{
    lcd->clear();    
    lcd->print("Temp: " + (String)temp + " C");
    lcd->setCursor(0,1);    
    String cadSplit;
    if (split)
        cadSplit = "ON";
    else
        cadSplit = "OFF";
    lcd->print("Split " + cadSplit);
}

void ControladorAire::apagar()
{
    if (estado)
    {
        estado = false;
        lcd->noDisplay(); // apaga el display, sin perder el texto actual 
    }      
}
void ControladorAire::resetAire()
{
	estado = false;
	temp = 24;
	split = false;
	lcd->noDisplay(); // apaga el display, sin perder el texto actual 
	
}

void ControladorAire::prender()
{
    if (!estado)
    {
        estado = true;        
        lcd->display(); // prender lcd y mostrar texto
    }    
}

void ControladorAire::apagarSplit()
{
    if (estado)
    {
        split = false;
        mostrarInfo();
    }    
}

void ControladorAire::prenderSplit()
{
    if (estado)
    {
        split = true;
        mostrarInfo();
    }  
}

void ControladorAire::subirTemp()
{
    if (estado)
    {
        if (temp < MAXTEMP)
        temp++;
        mostrarInfo();
    }  
}

void ControladorAire::bajarTemp() 
{
    if (estado)
    {
        if (temp > MINTEMP)
        temp--;
        mostrarInfo();
    }    
}  