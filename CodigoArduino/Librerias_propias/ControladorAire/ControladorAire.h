//Primero los include guards
#ifndef CONTROLADORAIRE_H
#define CONTROLADORAIRE_H
 
//Cambia Arduino.h por Wprogram.h si usas una version antigua de la IDE.
#include <Arduino.h> //Permite utilizar los comandos de Arduino
#include <LiquidCrystal.h>

 
class ControladorAire //Definicion de la clase
{
 
    public:
 
    //Constructor de la clase
	ControladorAire(int RS, int EN, int D4, int D5, int D6, int D7);

    void apagar();
	void resetAire();
    void prender();
    void apagarSplit();
    void prenderSplit();
    void subirTemp();
    void bajarTemp();   
 
    private:

    void mostrarInfo(); 
    bool estado;
    unsigned char temp;
    bool split;
    LiquidCrystal* lcd;
    const unsigned char MINTEMP = 15, MAXTEMP = 31;    
};
 
#endif