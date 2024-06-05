using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Management;
using System.Web;
using Microsoft.Win32;

namespace ColetaInfoMaquinas
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Coletando as informações
                string sistemaOperacional = GetOSInfo();
                string dataBios = GetBiosDate();
                string serialNumber = GetSerialNumber();
                string systemModel = GetSystemModel();
                string totalRam = GetTotalRam();
                string antivirus  = GetAntivirus();
                string nameMachine = GetNameMachine();


                Console.WriteLine("Coletando Versão do OS...");
                Console.WriteLine(sistemaOperacional);
                Console.WriteLine("Coletando Data da Bios...");
                Console.WriteLine(dataBios);
                Console.WriteLine("Coletando Serial Number...");
                Console.WriteLine(serialNumber);
                Console.WriteLine("Coletando Model...");
                Console.WriteLine(systemModel);
                Console.WriteLine("Coletando Ram...");
                Console.WriteLine(totalRam);
                Console.WriteLine("Coletando antivirus...");
                Console.WriteLine(antivirus);
                Console.WriteLine("Coletando nome...");
                Console.WriteLine(nameMachine);


            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
            }


            /****************************************************************
                 Funções que Buscam as informações da maquina 
            ****************************************************************/


            /// <summary>
            /// Coleta a versão do Sistema Operacional
            /// </summary>
            /// <returns>OS Name</returns>
            string GetOSInfo()
            {
                string osName = "Desconhecido";
                string edition = "";
                string version = "";
                string installationDate = "";
                string buildNumber = "";
                string experiencePack = "";

                // Obtém informações do registro
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                {
                    if (key != null)
                    {
                        osName = key.GetValue("ProductName").ToString();
                        edition = key.GetValue("EditionID").ToString();
                        installationDate = key.GetValue("InstallDate").ToString(); // Segundos desde 1/1/1970
                        buildNumber = key.GetValue("CurrentBuild").ToString();
                    }
                }

                // Converte a data de instalação para um formato legível
                DateTime installDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                installDateTime = installDateTime.AddSeconds(long.Parse(installationDate)).ToLocalTime();
                installationDate = installDateTime.ToString("dd/MM/yyyy");

                // Obtém informações sobre o Experience Pack usando WMI
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OptionalFeature WHERE Name LIKE '%%Experience%%'"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        experiencePack = obj["Name"].ToString();
                    }
                }

                version = $"{Environment.OSVersion.Version.Major}.{Environment.OSVersion.Version.Minor}";

                return $"{osName} {edition} (Versão {version}, Compilação {buildNumber}, Instalado em {installationDate}, {experiencePack})";
            }


            /// <summary>
            /// Coleta da data da BIOS
            /// </summary>
            /// <returns>dd/MM/yyyy</returns>

            string GetBiosDate()
            {
                using (var searcher = new ManagementObjectSearcher("SELECT ReleaseDate FROM Win32_BIOS"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        return ManagementDateTimeConverter.ToDateTime(obj["ReleaseDate"].ToString()).ToString("dd/MM/yyyy");
                    }
                }

                return "Data da Bios Não encontrada!";
            }


            string GetSerialNumber()
            {
                using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BIOS"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        return obj["SerialNumber"].ToString();
                    }
                }
                return "Não encontrado";
            }


            string GetAntivirus()
            {
                using (var searcher = new ManagementObjectSearcher(@"root\SecurityCenter2", "SELECT displayName FROM AntivirusProduct"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        return obj["displayName"].ToString();
                    }
                }
                return "Não encontrado";
            }


            string GetSystemModel()
            {
                using (var searcher = new ManagementObjectSearcher("SELECT Manufacturer, Model FROM Win32_ComputerSystem"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        return $"{obj["Manufacturer"]} {obj["Model"]}";
                    }
                }
                return "Não encontrado";
            }

            string GetTotalRam()
            {
                try
                {
                    using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem"))
                    {
                        foreach (ManagementObject obj in searcher.Get())
                        {
                            ulong ramBytes = (ulong)obj["TotalPhysicalMemory"];
                            ulong ramGB = ramBytes / 1024 / 1024 / 1024;
                            return $"{ramGB} GB";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao obter a memória RAM: " + ex.Message);
                }

                return "Não encontrado";
            }


            string GetNameMachine()
            {
                using (var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_ComputerSystem"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        return $"{obj["Name"]} ";
                    }
                }
                return "Não encontrado";
            }

        } //void main
    }
}
