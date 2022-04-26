using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Common
{
    public class Chave
    {
        private string _key;
        public string key
        {
            get
            {
                return _key;
            }
            set 
            {
                _key = value; 
            }
        }

        public void getKey()
        {
            this.key = this.GeraSenhaAleatoria();
        }

        // Gera a senha conforme as definições
        private string GeraSenhaAleatoria()
        {
            const string CAIXA_BAIXA = "abcdefghijklmnopqrstuvwxyz";
            const string CAIXA_ALTA = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string NUMEROS = "0123456789";
            const string ESPECIAIS = @"~!@#$%^&*():;[]{}<>,.?/\|";
            string outros = "";

            // Faz uma lista de caracteres permitidos
            string permitido = "";
            if (true) permitido += CAIXA_BAIXA;
            if (true) permitido += CAIXA_ALTA;
            if (true) permitido += NUMEROS;
            if (false) permitido += ESPECIAIS;
            if (false) permitido += "_";
            if (false) permitido += " ";
            if (false) permitido += outros;

            // Obtem o numero de caracteres .
            int caracteres_minimo = 20;
            int caracteres_maximo = 50;
            int numero_caracteres = Crypto.RandomInteger(caracteres_minimo, caracteres_maximo);

            // Satisfaz as definições
            string _senha = "";
            if (true &&
                (_senha.IndexOfAny(CAIXA_BAIXA.ToCharArray()) == -1))
                _senha += RandomChar(CAIXA_BAIXA);
            if (true &&
                (_senha.IndexOfAny(CAIXA_ALTA.ToCharArray()) == -1))
                _senha += RandomChar(CAIXA_ALTA);
            if (true &&
                (_senha.IndexOfAny(NUMEROS.ToCharArray()) == -1))
                _senha += RandomChar(NUMEROS);
            if (true &&
                (_senha.IndexOfAny(ESPECIAIS.ToCharArray()) == -1))
                _senha += RandomChar(ESPECIAIS);
            if (true &&
                (_senha.IndexOfAny("_".ToCharArray()) == -1))
                _senha += "_";
            if (false &&
                (_senha.IndexOfAny(" ".ToCharArray()) == -1))
                _senha += " ";
            if (false &&
                (_senha.IndexOfAny(outros.ToCharArray()) == -1))
                _senha += RandomChar(outros);

            // adiciona os caracteres restantes aleatorios
            while (_senha.Length < numero_caracteres)
                _senha += RandomChar(permitido);

            // mistura os caracteres requeridos 
            _senha = RandomizeString(_senha);

            return _senha;
        }

        // retorna um caractere aleatorio a partir de uma string
        private string RandomChar(string str)
        {
            return str.Substring(Crypto.RandomInteger(0, str.Length - 1), 1);
        }

        // Retorna uma permutação aleatoria de uma string
        private string RandomizeString(string str)
        {
            string resultado = "";
            while (str.Length > 0)
            {
                // Pega um numero aleatorio
                int i = Crypto.RandomInteger(0, str.Length - 1);
                resultado += str.Substring(i, 1);
                str = str.Remove(i, 1);
            }
            return resultado;
        }

    }
}