using System;
using System.Collections.Generic;
using System.Text;

namespace ServidorContador
{
    class Carta
    {
        public enum eTipo
        {
            Numero,
            Sentido,
            Efecto
        }
        private eTipo tipo;
        private int valor;
        private bool sentido;
        public eTipo Tipo { get; set; }
        public int Valor
        {
            set
            {
                if (valor < 3)
                {
                    valor = 3;
                }
                else if (valor > 7)
                {
                    valor = 7;
                }
                this.valor = value;
            }
            get
            {
                return this.valor;
            }
        }
        public bool Sentido { get; set; }
        public Carta(eTipo tipo, int valor, bool sentido)
        {
            Valor = valor;
            Tipo = tipo;
            Sentido = sentido;
        }
    }
}
