//Primero los include guards
#ifndef CONTROLADORTV_H
#define CONTROLADOTV_H
 
//Cambia Arduino.h por Wprogram.h si usas una version antigua de la IDE.
#include <Arduino.h> //Permite utilizar los comandos de Arduino
#include <TM1637Display.h>
class ControladorTV //Definicion de la clase
{
 
    public: 
    //Constructor de la clase
    ControladorTV(int CLOCK,int DATAIO); 
    
    void apagar();
	void resetTV();
    void prender();
    void subirCanal();
    void bajarCanal();  
	void cambiarCanal(int chanel);
	uint8_t valorArreglo(int valor);
	
    private:		
    void mostrarInfo();
	void cambiarValor(uint8_t arreglo[], int tam, int canal);
	int CLK; //Set the CLK pin connection to the display
	int DIO; //Set the DIO pin connection to the display

	bool estado;
	unsigned char canal;
	const unsigned char MINCANAL = 2, MAXCANAL = 99;

	TM1637Display *display; //set up the 4-Digit Display.
	//Arreglo de 4 posiciones, coincidente con el del display, cada elemento de 1 byte
	uint8_t data[4]; //Elementos de 7 segmentos	
	//Enumerador que guarda la relacion de posicion de los elementos, con el arreglo de letras
	enum SIMBOLO {
		NUM0, NUM1, NUM2, NUM3, NUM4, NUM5, NUM6, NUM7, NUM8, NUM9,
		ESPACIO, GUION, PREGUNTA1, PREGUNTA2,
		LETRAA, LETRAC, LETRAE, LETRAF, LETRAH, LETRAI, LETRAJ, LETRAL, LETRAN, LETRAO, LETRAP, LETRAS, LETRAU
	};
	/*
	//    A
	//    -
	// F | | B
	//    G
	// E | | C
	//    -
	//    D
	//0bXGFEDCBA
	*/
	//Arreglo que contiene  la palabra de byte de cada simbolo
	//{ A, B, C, D, E, F, G, X }//Formato correspodiente a la figura de arriba
	const byte letras[27][8] = {
	  { 1, 1, 1, 1, 1, 1, 0, 0 }, // 0
	  { 0, 1, 1, 0, 0, 0, 0, 0 }, // 1
	  { 1, 1, 0, 1, 1, 0, 1, 0 }, // 2
	  { 1, 1, 1, 1, 0, 0, 1, 0 }, // 3
	  { 0, 1, 1, 0, 0, 1, 1, 0 }, // 4
	  { 1, 0, 1, 1, 0, 1, 1, 0 }, // 5
	  { 1, 0, 1, 1, 1, 1, 1, 0 }, // 6
	  { 1, 1, 1, 0, 0, 0, 0, 0 }, // 7
	  { 1, 1, 1, 1, 1, 1, 1, 0 }, // 8
	  { 1, 1, 1, 0, 0, 1, 1, 0 }, // 9  
	  { 0, 0, 0, 0, 0, 0, 0, 0 }, // Nada, espacio  
	  { 0, 0, 0, 0, 0, 0, 1, 0 }, // -
	  { 0, 1, 0, 1, 1, 0, 1, 0 }, // ¿
	  { 1, 1, 0, 0, 1, 0, 1, 1 }, // ?
	  { 1, 1, 1, 0, 1, 1, 1, 0 }, // A
	  { 1, 0, 0, 1, 1, 1, 0, 0 }, // C  
	  { 1, 0, 0, 1, 1, 1, 1, 0 }, // E
	  { 1, 0, 0, 0, 1, 1, 1, 0 }, // F    
	  { 0, 1, 1, 0, 1, 1, 1, 0 }, // H
	  { 0, 1, 1, 0, 0, 0, 0, 0 }, // I
	  { 0, 1, 1, 1, 1, 0, 0, 0 }, // J  
	  { 0, 0, 0, 1, 1, 1, 0, 0 }, // L
	  { 1, 1, 1, 0, 1, 1, 0, 0 }, // N 
	  { 1, 1, 1, 1, 1, 1, 0, 0 }, // O
	  { 1, 1, 0, 0, 1, 1, 1, 0 }, // P
	  { 1, 0, 1, 1, 0, 1, 1, 0 }, // S
	  { 0, 1, 1, 1, 1, 1, 0, 0 }  // U
	};
};
 
#endif

