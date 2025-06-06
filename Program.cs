using System;
using System.Collections.Generic;
using System.Net;

class Program
{
    static void Main()
    {
        while (true)
        {
            MostrarLogo();

            Console.Write("Ingrese la IP base (ej. 192.168.0.0): ");
            string? ipInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(ipInput) || !IPAddress.TryParse(ipInput, out IPAddress baseIp))
            {
                Console.WriteLine("IP no válida.");
                return;
            }

            List<int> listaHosts = new();

            Console.WriteLine("\nIngrese la cantidad de hosts (presione Enter para finalizar):");
            while (true)
            {
                Console.Write("> ");
                string? entrada = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(entrada)) break;

                if (int.TryParse(entrada, out int cantidad) && cantidad > 0)
                    listaHosts.Add(cantidad);
                else
                    Console.WriteLine("⚠️  Entrada inválida, intente de nuevo.");
            }

            if (listaHosts.Count == 0)
            {
                Console.WriteLine("No se ingresaron valores. Saliendo...");
                return;
            }

            Console.WriteLine();
            MostrarTabla(baseIp, listaHosts);

            
            Console.Write("\n¿Desea realizar otra acción? (S/N): ");
            string? respuesta = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(respuesta) || !respuesta.Trim().ToUpper().StartsWith("S"))
            {
                Console.WriteLine("\n¡Proceso completado!");
                break;
            }

            Console.Clear(); 
        }
    }

    static void MostrarLogo()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
   _____       _     _     _           _ _           
  / ____|     | |   (_)   | |         (_) |          
 | (___  _   _| |__  _  __| |_ __ ___  _| |_ ___ _ __ 
  \___ \| | | | '_ \| |/ _` | '_ ` _ \| | __/ _ \ '__|
  ____) | |_| | |_) | | (_| | | | | | | | ||  __/ |   
 |_____/ \__,_|_.__/|_|\__,_|_| |_| |_|_|\__\___|_|   
                                                     
       *** SUBNET CALCULATOR CMD TOOL ***
        Creado por: Jose Rodriguez
");
        Console.ResetColor();
    }

    static void MostrarTabla(IPAddress baseIp, List<int> cantidades)
    {
        byte[] baseBytes = baseIp.GetAddressBytes();
        uint baseIpInt = BytesToUInt(baseBytes);
        uint ipActual = baseIpInt;

        string separador = "+--------+---------------+--------+-----------------+-----------------+--------------------------+";
        Console.WriteLine(separador);
        Console.WriteLine("| Hosts  | Máscara       | /CIDR  | Dirección Red   | Broadcast       | Rango de Hosts           |");
        Console.WriteLine(separador);

        foreach (int hosts in cantidades)
        {
            int bitsHost = (int)Math.Ceiling(Math.Log(hosts + 2, 2));
            int cidr = 32 - bitsHost;
            int totalIPs = (int)Math.Pow(2, bitsHost);
            IPAddress mascara = CalcularMascara(cidr);

            uint redIpInt = ipActual;
            uint broadcastInt = redIpInt + (uint)(totalIPs - 1);
            IPAddress redIp = UIntToIP(redIpInt);
            IPAddress broadcast = UIntToIP(broadcastInt);
            IPAddress primera = UIntToIP(redIpInt + 1);
            IPAddress ultima = UIntToIP(broadcastInt - 1);

            Console.WriteLine($"| {hosts,-6} | {mascara,-13} | /{cidr,-5} | {redIp,-15} | {broadcast,-15} | {primera} - {ultima,-20} |");

            ipActual = broadcastInt + 1; 
        }

        Console.WriteLine(separador);
    }

    static IPAddress CalcularMascara(int cidr)
    {
        uint mask = 0xffffffff << (32 - cidr);
        return UIntToIP(mask);
    }

    static uint BytesToUInt(byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToUInt32(bytes, 0);
    }

    static IPAddress UIntToIP(uint ip)
    {
        byte[] bytes = BitConverter.GetBytes(ip);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return new IPAddress(bytes);
    }
}
