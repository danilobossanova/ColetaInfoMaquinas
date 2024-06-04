using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Management;

namespace ColetaInfoMaquinas
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Coletando as informações
                string os = GetOSInfo();



                /****************************************************************
                 Funções que Buscam as informações da maquina 
                ****************************************************************/

                string GetOSInfo()
                {
                    return Environment.OSVersion.VersionString;
                }

            }
            catch { 
                
            }
        }
    }
}
