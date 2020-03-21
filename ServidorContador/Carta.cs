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

        public Carta(eTipo tipo,int valor,bool sentido)
        {
            this.tipo = tipo;
            this.valor = valor;
            this.sentido = sentido;
        }

        public void setTipo(eTipo tipo)
        {
            this.tipo = tipo;
        }
        public void setValor(int valor)
        {
            this.valor = valor;
        }
        public void setSentido(bool sentido)
        {
            this.sentido = sentido;
        }
        public eTipo getTipo()
        {
            return this.tipo;
        }
        public int getValor()
        {
            return this.valor;
        }
        public bool getSentido()
        {
            return this.sentido;
        }
    }
}
