using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente
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

        public Carta(eTipo tipo, int valor, bool sentido)
        {
            setValor(valor);
            setTipo(tipo);
            setSentido(sentido);
        }

        public void setTipo(eTipo tipo)
        {
            this.tipo = tipo;
        }
        public void setValor(int valor)
        {
            if (valor < 3)
            {
                valor = 3;
            }else if(valor > 7)
            {
                valor = 7;
            }
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
