using System;
using System.Globalization;
using System.Windows;
using TextCopy;
using System.IO;
using System.Collections.Generic;

namespace Kolokwium_Pspice_wzmacniacz
{
    internal class Program
    {
        static string? ReadLine()
        {
            return Console.ReadLine()?.Replace('.', ',');
        }

        static void Main(string[] args)
        {

            NumberFormatInfo setPrec = new();
            setPrec.NumberDecimalDigits = 3;

            double Uce = 0, Ic = 0, Ucc = 0, Ro = 0, Fd = 0, Ib = 0, Ib1 = 0, Ib2 = 0, Ube = 0, Beta = 0, Ie = 0, Rc = 0, Re = 0, Rb1 = 0, Rb2 = 0, gm = 0, Rl = 0, ku = 0;
            
            if(args.Length > 0 ) 
            {

                if (args[0] == "-h" || args[0] == "--help")
                { Manual(); return; }
                if (args[0] == "-a" || args[0] == "--arg")
                {
                    if(args.Length >= 2)
                        Uce = Convert.ToDouble(args[1].Replace('.',','));
                    if (args.Length >= 3)
                        Ic = Convert.ToDouble(args[1].Replace('.', ','));
                    if (args.Length >= 4)
                        Ucc = Convert.ToDouble(args[1].Replace('.', ','));
                    if (args.Length >= 5)
                        Ro = Convert.ToDouble(args[1].Replace('.', ','));
                    if (args.Length >= 6)
                        Fd = Convert.ToDouble(args[1].Replace('.', ','));
                }                
            }

            if (Uce != 0)
            {
                Console.Write("Podaj dane:\nUce[V] = ");
                Uce = Convert.ToDouble(ReadLine());
            }

            if (Ic != 0)
            {
                Console.Write("Ic[mA] = ");
                Ic = Convert.ToDouble(ReadLine()) / 1000;
            }

            if (Ucc != 0)
            {
                Console.Write("Ucc[V] = ");
                Ucc = Convert.ToDouble(ReadLine());
            }

            if (Ro != 0)
            {
                Console.Write("Ro[kOhm] = ");
                Ro = Convert.ToDouble(ReadLine()) * 1000;
            }

            if (Fd != 0)
            {
                Console.Write("Fd[Hz] = ");
                Fd = Convert.ToDouble(ReadLine());
            }

            Rc = Math.Round((Ucc - Uce) / (1.1 * Ic), 2);
            Re = Math.Round(Rc / 10, 2);
            
            Console.WriteLine("\n\nOdpal ten program(już jest w folderze programu)\nUtwórz wykres dla wartości IC(Q1) i V(1)\nUstaw wskaźnik na punkt gdzie IC={0} \nOdczytaj wartości z wykresu V1 w formacie: (IB , UBE)", Ic);

            string GotowyProgram = "*Wzmacniacz\n" +
                ".lib europe.lib\n\n" +
                $"V1 2 0 {Uce}\n" +
                $"Q1 2 1 0 bc107b\n" +
                $"\n" +
                $"I1 0 1 DC 1\n" +
                $".DC I1 0 20u 0.1u\n" +
                $".probe\n" +
                $".end";
            GotowyProgram = GotowyProgram.Replace(',', '.');

            Console.WriteLine("\n\n" + GotowyProgram + "\n\n");

            if(!Directory.Exists(@".\Gotowiec"))
                Directory.CreateDirectory(@".\Gotowiec");
            File.WriteAllText(".\\Gotowiec\\Program.cir", GotowyProgram);
            File.Copy(@".\EUROPE.LIB", ".\\Gotowiec\\EUROPE.LIB", true);

            Console.WriteLine("Zamknij Pspice i kliknij ENTER!!!");
            ReadLine();

            Console.Write("Ib[uA] = ");
            Ib = Convert.ToDouble(ReadLine()) / 1000000;

            Console.Write("Ube[mV] = ");
            Ube = Convert.ToDouble(ReadLine()) / 1000;

            Beta = Ic / Ib;
            Ie = Ic + Ib;
            Ib1 = 11 * Ib;
            Ib2 = 10 * Ib;
            Rb2 = (Ube + Ic * Re) / Ib2;
            Rb1 = (Ucc - (Ube + Ic * Re)) / Ib1;
            gm = Ie / 25;
            Rl = (Rc * Ro) / Rc + Ro;
            ku = -gm * Rl;

            Console.WriteLine("Gotowy program: \n\n");

            GotowyProgram = "*Wzmacniacz z C\n" +
                    $".lib europe.lib\n" +
                    $"Q1 4 2 5 BC107B\n" +
                    $"Re 5 0 {Re}\n" +
                    $"Rc 3 4 {Rc}\n" +
                    $"Rb1 3 2 {Rb1}\n" +
                    $"Rb2 2 0 {Rb2}\n" +
                    $"R0 6 0 {Ro}\n" +
                    $"C1 1 2 100u\n" +
                    $"C2 4 6 100u\n" +
                    $"Ce 5 0 1m\n" +
                    $"V1 3 0 DC 5.5\n" +
                    $"V2 1 0 AC 150m SIN 0 150m 1k 0 0 0\n" +
                    $".AC dec 100 .1 1000meg\n" +
                    $".TRAN 10n 10m 0 1u\n" +
                    $".FOUR 1k V(6) V(4)\n" +
                    $".probe\n" +
                    $".end\n\n\n";
            GotowyProgram = GotowyProgram.Replace(',', '.');
            File.WriteAllText(".\\Gotowiec\\Program.cir", GotowyProgram);
            Console.WriteLine(GotowyProgram);

            //Uce, Ic, Ucc, Ro, Fd, Ib, Ib1, Ib2, Ube, Beta, Ie, Rc, Re, Rb1, Rb2, gm, Rl, ku;

            string zmienne = $"Uce = {Uce}, Ic = {Ic}, Ucc = {Ucc}, Ro = {Ro}, Fd = {Fd}, Ib = {Ib}, Ib1 = {Ib1}, Ib2 = {Ib2}, Ube = {Ube}, Beta = {Beta}, Ie = {Ie}, Rc = {Rc}, Re = {Re}, Rb1 = {Rb1}, Rb2 = {Rb2}, gm = {gm}, Rl = {Rl}, ku = {ku}";
            zmienne = zmienne.Replace(", ", "\n");
            File.WriteAllText("Zmienne.txt", zmienne);
            Console.WriteLine("Wszytstkie zmienne są w lokalizacji .\\Zmienne.txt");

            Console.ReadKey();

        }

        static void Manual()
        {
            string man = "Instrukcja:\n\t-h\t--help\tOtwarcie manuala\n\t-a\t--arg <Uce[V]> <Ic[mA]> <Ucc[V]> <Ro[kOhm]> <Fd[Hz]> \tArgumenty startowe - trzeba podać w kolejności";
        }
    }
}