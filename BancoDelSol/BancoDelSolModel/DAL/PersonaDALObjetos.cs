﻿using BancoDelSolModel.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoDelSolModel.DAL
{
    class PersonaDALObjetos
    {
        private static List<Persona> personas = new List<Persona>();

        //Metodos
        public void Ingresar(Persona p)
        {
            personas.Add(p);
        }

        public List<Persona> Mostrar()
        {
            return personas;
        }

        public void Eliminar(Persona p)
        {
            personas.Remove(p);
        }
    }
}
