using System;
using System.Globalization;
using System.Windows;
using System.IO;
using System.Collections.Generic;

namespace Kolokwium_Pspice_wzmacniacz
{
    internal class Program
    {
        static string? ReadLine()
        {
            string? line = Console.ReadLine()?.Replace('.', ',');

            if(line == null) return "0";

            line = line.Trim();

            foreach (char ch in line)
            {
                if(ch != '1' && ch != '2' && ch != '3' && ch != '4' && ch != '5' && ch != '6' && ch != '7' && ch != '8' && ch != '9' && ch != '0' && ch != ',')
                    return "0";
            }
            return line;
        }

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                FILTR f = new();
                switch (args[0].ToUpper())
                {

                    case "FPP":
                        f.FPP();
                        break;

                    case "FPZ":
                        f.FPZ();
                        break;

                    case "FDP":
                        f.FDP();
                        break;

                    case "FGP":
                        f.FGP();
                        break;

                }

                return;
            }

            for (int i = 0; i < args.Length; i++) { args[i] = args[i].ToLower(); }

            double Uce = 0, Ic = 0, Ucc = 0, Ro = 0, Fd = 0, Ib = 0, Ib1 = 0, Ib2 = 0, Ube = 0, Beta = 0, Ie = 0, Rc = 0, Re = 0, Rb1 = 0, Rb2 = 0, gm = 0, Rl = 0, ku = 0;
            string Q1Number = "", LIB = @".\Biblioteki\EUROPE.LIB";

            if (args.Length > 0)
            {
                if (args[0] != "-h" && args[0] != "--help" && args[0] == "-a" & args[0] == "--arg" && args[0] == "-q")
                {
                    return;
                }
                else if (args[0] == "-h" || args[0] == "--help")
                {
                    Manual(); return;
                }
                GetValuesFromArgs(args, ref Uce, ref Ic, ref Ucc, ref Ro, ref Fd, ref Q1Number, LIB);
            }

            SettingFirstValues(ref Uce, ref Ic, ref Ucc, ref Ro, ref Fd, ref Q1Number, ref LIB);

            Rc = Math.Round((Ucc - Uce) / (1.1 * Ic), 1);
            Re = Math.Round(Rc / 10, 1);

            Console.Clear();
            Console.WriteLine("\n\nOdpal ten program(już jest w folderze programu)\nUtwórz wykres dla wartości IC(Q1) i V(1)\nUstaw wskaźnik na punkt gdzie IC={0} \nOdczytaj wartości z wykresu V1 w formacie: (IB , UBE)", Ic);

            string GotowyProgram = "*Wzmacniacz\n" +
                ".lib europe.lib\n\n" +
                $"V1 2 0 {Uce}\n" +
                $"Q1 2 1 0 {Q1Number}\n" +
                "\n" +
                "I1 0 1 DC 1\n" +
                ".DC I1 0 20u 0.1u\n" +
                ".probe\n" +
                ".end";
            GotowyProgram = GotowyProgram.Replace(',', '.');

            Console.WriteLine("\n\n" + GotowyProgram + "\n\n");

            if (!Directory.Exists(@".\Gotowiec"))
                Directory.CreateDirectory(@".\Gotowiec");
            File.WriteAllText(".\\Gotowiec\\Program.cir", GotowyProgram);
            File.Copy(LIB, ".\\Gotowiec\\EUROPE.LIB", true);

            Console.WriteLine("Zamknij Pspice i kliknij ENTER!!!");
            ReadLine();

            while (Ib == 0)
            {
                Console.Write("Ib[uA] = ");
                Ib = Convert.ToDouble(ReadLine()) / 1000000;
            }

            while (Ube == 0)
            {
                Console.Write("Ube[mV] = ");
                Ube = Convert.ToDouble(ReadLine()) / 1000;
            }

            FastMath(Ic, Ucc, Ro, Ib, out Ib1, out Ib2, Ube, out Beta, out Ie, Rc, Re, out Rb1, out Rb2, out gm, out Rl, out ku);

            GotowyProgram = "";

            while (true)
            {
                Console.Clear();
                Console.Write("Czy podano wartości Kondensatorów(T/N): ");
                
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.T:
                        double C1 = 0, C2 = 0, Ce = 0;
                        Console.Write("\nCzy wartości to C1 = C2 = 100uF, Ce = 1m(T/N)? ");
                        if (Console.ReadKey().Key == ConsoleKey.T)
                        {
                            C1 = ((double)100 / (double)1000000);
                            C2 = C1;
                            Ce = ((double)1 / (double)1000);
                        }
                        else
                            SetCapasitors(ref C1, ref C2, ref Ce);
                        GotowyProgram +=
                            "*Wzmacniacz z C\n" +
                                $".lib europe.lib\n" +
                                $"Q1 4 2 5 {Q1Number}\n" +
                                $"Re 5 0 {Re}\n" +
                                $"Rc 3 4 {Rc}\n" +
                                $"Rb1 3 2 {Rb1}\n" +
                                $"Rb2 2 0 {Rb2}\n" +
                                $"R0 6 0 {Ro}\n" +
                                $"C1 1 2 {C1}\n" +
                                $"C2 4 6 {C2}\n" +
                                $"Ce 5 0 {Ce}\n" +
                                $"V1 3 0 DC 5.5\n" +
                                $"V2 1 0 AC 1m SIN 0 1m 1k 0 0 0\n" +
                                $".AC dec 100 .1 1000meg\n" +
                                $".TRAN 10n 10m 0 1u\n" +
                                $".FOUR 1k V(6) V(4)\n" +
                                $".probe\n" +
                                $".end\n\n\n";
                        break;

                    case ConsoleKey.N:
                        GotowyProgram +=
                            "*Wzmacniacz z C\n" +
                                $".lib europe.lib\n" +
                                $"Q1 4 2 5 {Q1Number}\n" +
                                $"Re 5 0 {Re}\n" +
                                $"Rc 3 4 {Rc}\n" +
                                $"Rb1 3 2 {Rb1}\n" +
                                $"Rb2 2 0 {Rb2}\n" +
                                $"R0 4 0 {Ro}\n" +
                                $"V1 3 0 DC 5.5\n" +
                                $"V2 2 0 AC 1m SIN 0 1m 1k 0 0 0\n" +
                                $".AC dec 100 .1 1000meg\n" +
                                $".TRAN 10n 10m 0 1u\n" +
                                $".probe\n" +
                                $".end\n\n\n";
                        break;

                    default:
                        continue;
                }
                break;
            }

            SaveAs(Uce, Ic, Ucc, Ro, Fd, Ib, Ib1, Ib2, Ube, Beta, Ie, Rc, Re, Rb1, Rb2, gm, Rl, ku, GotowyProgram);

            Console.ReadKey();

        }

        private static void FastMath(double Ic, double Ucc, double Ro, double Ib, out double Ib1, out double Ib2, double Ube, out double Beta, out double Ie, double Rc, double Re, out double Rb1, out double Rb2, out double gm, out double Rl, out double ku)
        {
            Beta = Ic / Ib;
            Ie = Ic + Ib;
            Ib1 = 11 * Ib;
            Ib2 = 10 * Ib;
            Rb2 = Math.Round((Ube + Ic * Re) / Ib2, 1);
            Rb1 = Math.Round((Ucc - (Ube + Ic * Re)) / Ib1, 1);
            gm = Ie / 25;
            Rl = (Rc * Ro) / Rc + Ro;
            ku = -gm * Rl;
        }

        static void SaveAs(double Uce, double Ic, double Ucc, double Ro, double Fd, double Ib, double Ib1, double Ib2, double Ube, double Beta, double Ie, double Rc, double Re, double Rb1, double Rb2, double gm, double Rl, double ku, string GotowyProgram)
            {
                Console.WriteLine("\n\nGotowy program: \n\n");

                GotowyProgram = GotowyProgram.Replace(',', '.');
                File.WriteAllText(".\\Gotowiec\\Program.cir", GotowyProgram);
                Console.WriteLine(GotowyProgram);

                string zmienne = $"Uce = {Uce}, Ic = {Ic}, Ucc = {Ucc}, Ro = {Ro}, Fd = {Fd}, Ib = {Ib}, Ib1 = {Ib1}, Ib2 = {Ib2}, Ube = {Ube}, Beta = {Beta}, Ie = {Ie}, Rc = {Rc}, Re = {Re}, Rb1 = {Rb1}, Rb2 = {Rb2}, gm = {gm}, Rl = {Rl}, ku = {ku}";

                zmienne = zmienne.Replace(", ", "\n");

                File.WriteAllText("Zmienne.txt", zmienne);

                Console.WriteLine("Wszytstkie zmienne są w lokalizacji .\\Zmienne.txt");
            }

        static void GetValuesFromArgs(string?[] args, ref double Uce, ref double Ic, ref double Ucc, ref double Ro, ref double Fd, ref string Q1, string LIB)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-a" || args[i] == "--arg")
                {
                    if (args.Length >= 2)
                    {
                        i++;
                        Uce = Convert.ToDouble(args[i].Replace('.', ','));
                    }
                    if (args.Length >= 3)
                    {
                        i++;
                        Ic = Convert.ToDouble(args[i].Replace('.', ','))/1000;
                    }
                    if (args.Length >= 4)
                    {
                        i++;
                        Ucc = Convert.ToDouble(args[i].Replace('.', ','));
                    }
                    if (args.Length >= 5)
                    {
                        i++;
                        Ro = Convert.ToDouble(args[i].Replace('.', ','))*1000;
                    }
                    if (args.Length >= 6)
                    {
                        i++;
                        Fd = Convert.ToDouble(args[i].Replace('.', ','));
                    }
                }
                else if (args[i] == "-q" && args.Length >= i + 1)
                {
                    i++;
                    Q1 = args[i++];

                    if (File.ReadAllText(LIB).Split(Q1.ToUpper()).Length <= 1)
                    { Q1 = ""; }

                }
            }
            
        }

        static void SettingFirstValues(ref double Uce, ref double Ic, ref double Ucc, ref double Ro, ref double Fd, ref string Q1, ref string LIB)
        {
            while (Uce == 0)
            {
                Console.Write("Podaj dane:\nUce[V] = ");
                Uce = Convert.ToDouble(ReadLine());
            }

            while (Ic == 0)
            {
                Console.Write("Ic[mA] = ");
                Ic = Convert.ToDouble(ReadLine()) / 1000;
            }

            while (Ucc == 0)
            {
                Console.Write("Ucc[V] = ");
                Ucc = Convert.ToDouble(ReadLine());
            }

            while (Ro == 0)
            {
                Console.Write("Ro[kOhm] = ");
                Ro = Convert.ToDouble(ReadLine()) * 1000;
            }

            while (Fd == 0)
            {
                Console.Write("Fd[Hz] = ");
                Fd = Convert.ToDouble(ReadLine());
            }

            while (Q1 == "")
            {
                Console.Write("Czy chcesz wybrać inny niż domyślny model tranzystora?(T/N) ");
                if(Console.ReadKey().Key == ConsoleKey.N)
                {
                    if (File.ReadAllText(LIB).Split("BC107B").Length > 1)
                    {
                        Q1 = "BC107B";
                        break;
                    }
                    else
                    {
                        while (true)
                        {
                            Console.WriteLine($"Błąd!!!\nBrak pliku biblioteki(EUROPE.LIB)");
                            Console.Write("Czy chcesz wybrać inną bibliotekę(muci ona być w folderze programu)?(T/N) ");
                            if (Console.ReadLine().ToLower().Remove(2) == "t")
                            {
                                Console.Write("\nPodaj jej nazwę(wraz z rozszerzeniem): ");
                                LIB = ".\\Biblioteki\\" + Console.ReadLine();
                            }
                            else { Console.WriteLine("Załaduj plik i uruchom program ponownie"); Environment.Exit(0); }
                            if (File.Exists(LIB))
                                break;
                        }
                        continue;
                    }
                }    
                while (Q1 == "")
                {

                    Console.Write("\nNazwa Traznystora: ");
                    string PotencjalneQ1 = Console.ReadLine().ToUpper();
                    if (!File.Exists(LIB))
                    {
                        while (true)
                        {
                            Console.WriteLine($"Błąd!!!\nBrak pliku biblioteki(EUROPE.LIB)");
                            Console.Write("Czy chcesz wybrać inną bibliotekę(muci ona być w folderze programu)?(T/N) ");
                            if (Console.ReadLine().ToLower().Remove(2) == "t")
                            {
                                Console.Write("Podaj jej nazwę(wraz z rozszerzeniem): ");
                                LIB = ".\\Biblioteki\\" + Console.ReadLine();
                            }
                            if (File.Exists(LIB))
                                break;
                        }
                    }
                    if (File.ReadAllText(LIB).Split(PotencjalneQ1).Length > 1)
                        Q1 = PotencjalneQ1;
                    else Console.WriteLine("Brak podanego modelu w wybranej bibiotece.");
                }
            }
                
        }

        static void SetCapasitors(ref double C1, ref double C2, ref double Ce)
        {
            while (C1 == 0)
            {
                Console.Write("C1[uF] = ");
                C1 = Convert.ToDouble(ReadLine()) / 1000000;
            }
            while (C2 == 0)
            {
                Console.Write("C2[uF] = ");
                C2 = Convert.ToDouble(ReadLine()) / 1000000;
            }
            while (Ce == 0)
            {
                Console.Write("Ce[uF] = ");
                Ce = Convert.ToDouble(ReadLine()) / 1000000;
            }
        }

        static void Manual()
        {
            string man = "Instrukcja:\n" +
                "-h  --help\t\t\t\t\t\t\t\tOtwarcie manuala\n" +
                "-a  --arg\t<Uce[V]> <Ic[mA]> <Ucc[V]> <Ro[kOhm]> <Fd[Hz]>\t\tArgumenty startowe - trzeba podać w kolejności\n" +
                "-q\t\t<Q1_NAME>\t\t\t\t\t\tPrzypisanie innego tranzystora(podaj numer)\n";

            Console.WriteLine(man);
        }
    }

    public class FILTR
    {
        
        public void FPP()
        {
            Console.Write("Fd [Hz]: ");
            Double.TryParse(Console.ReadLine(), out double Freq);

            Console.Write("B [Hz]: ");
            Double.TryParse(Console.ReadLine(), out double B);

            Console.Write("R1 [Ohm]: ");
            Double.TryParse(Console.ReadLine(), out double R1);

            Console.Write("R2 [Ohm]: ");
            Double.TryParse(Console.ReadLine(), out double R2);

            double C1 = 0, C2 = 0;

            C1 = (1 / (2 * Math.PI * R1 * (Freq + B / 2)) * 1000000);
            C2 = (1 / (2 * Math.PI * R2 * (Freq - B / 2)) * 1000000);

            string GotowyProgram = 
                $"* FPP\n" +
                $"* Fd = {Freq.ToString().Replace(',', '.')}\n" +
                $"V_Vin 1 0 AC 1\n" +
                $"R1 1 2 {R1.ToString().Replace(',', '.')}\n" +
                $"R2 3 0 {R2.ToString().Replace(',', '.')}\n" +
                $"C1 2 0 {C1.ToString().Replace(',', '.')}u\n" +
                $"C2 2 3 {C2.ToString().Replace(',', '.')}u\n" +
                $".AC dec 500 100 10meg\n" +
                $".PROBE\n" +
                $".END";

            File.WriteAllText(".\\Gotowiec\\Filtr.cir", GotowyProgram);

        }

        public void FPZ()
        {
            Console.Write("Fd [Hz]: ");
            Double.TryParse(Console.ReadLine(), out double Freq);

            Console.Write("B [Hz]: ");
            Double.TryParse(Console.ReadLine(), out double B);

            Console.Write("R1 [Ohm]: ");
            Double.TryParse(Console.ReadLine(), out double R1);

            Console.Write("R2 [Ohm]: ");
            Double.TryParse(Console.ReadLine(), out double R2);

            double C1 = 0, C2 = 0;

            C1 = (1 / (2 * Math.PI * R1 * (Freq + B / 2)) * 1000000);
            C2 = (1 / (2 * Math.PI * R2 * (Freq - B / 2)) * 1000000);

            string GotowyProgram =
                $"* FPZ\n" +
                $"* Fd = {Freq.ToString().Replace(',', '.')}\n" +
                $"V_Vin 1 0 AC 1\n" +
                $"R1 1 2 {R1.ToString().Replace(',', '.')}\n" +
                $"R2 3 0 {R2.ToString().Replace(',', '.')}\n" +
                $"C1 1 2 {C1.ToString().Replace(',', '.')}u\n" +
                $"C2 2 3 {C2.ToString().Replace(',', '.')}u\n" +
                $".AC dec 500 100 10meg\n" +
                $".PROBE\n" +
                $".END";

            File.WriteAllText(".\\Gotowiec\\Filtr.cir", GotowyProgram);

        }
        public void FDP()
        {
            Console.Write("Fg [Hz]: ");
            Double.TryParse(Console.ReadLine(), out double Freq);
            
            Console.Write("R1 [Ohm]: ");
            Double.TryParse(Console.ReadLine(), out double R1);

            double C1 = 0;

            C1 = (1 / (2 * Math.PI * R1 * (Freq) * 1000000));
            

            string GotowyProgram =
                $"* FDP\n" +
                $"* Fd = {Freq.ToString().Replace(',', '.')}\n" +
                $"V_Vin 1 0 AC 1\n" +
                $"R1 1 2 {R1.ToString().Replace(',', '.')}\n" +
                $"C1 2 0 {C1.ToString().Replace(',','.')}u\n" +
                $".AC dec 500 100 10meg\n" +
                $".PROBE\n" +
                $".END";

            File.WriteAllText(".\\Gotowiec\\Filtr.cir", GotowyProgram);

        }
        public void FGP()
        {
            Console.Write("Fg [Hz]: ");
            Double.TryParse(Console.ReadLine(), out double Freq);

            Console.Write("R1 [Ohm]: ");
            Double.TryParse(Console.ReadLine(), out double R1);


            double C1 = 0;

            C1 = (1 / (2 * Math.PI * R1 * (Freq) * 1000000));


            string GotowyProgram =
                $"* FDP\n" +
                $"* Fd = {Freq.ToString().Replace(',', '.')}\n" +
                $"V_Vin 1 0 AC 1\n" +
                $"R1 2 0 {R1.ToString().Replace(',', '.')}\n" +
                $"C1 1 2 {C1.ToString().Replace(',', '.')}u\n" +
                $".AC dec 500 100 10meg\n" +
                $".PROBE\n" +
                $".END";

            File.WriteAllText(".\\Gotowiec\\Filtr.cir", GotowyProgram);
        }
    }
}