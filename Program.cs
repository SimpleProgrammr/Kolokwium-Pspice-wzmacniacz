/*
*Wzmacniacz
*
*Uce = 5.5V		// Podane w Zadaniu
*Ic = 1.1 mA		//
*Ucc = 11V		//
*Ro = 10K		//
*fd = 110Hz		// 
*
*Ib = 3.96uA	// Z charakterystyk
*Ube = 665mV	//
*
*Beta = Ic/Ib = 275							// Policz (Wzory proste)
*Ie = Ic+Ib = 1.103mA							//
*Idzielnika = Ib2 = 10 * Ib => Ib1 = 11 * Ib 				//
*
*Rc = 10*Re								// Policz z oczka
*Rc = (Ucc-Uce)/1.1*Ic = 4545						//
*Re = 454.5								//
*									// 
*Rb1 = (Ucc - (Ube + Ic * Re)/(11 * Ib) = 225.8k		 	//
*Rb2 = (Ube + Ic * Re)/10 * Ib = 29.4k					//
*
*reb = 25
*gm = Ie/25mV = 44 mS
*Rl = (Rc * Ro)/Rc + Ro = 3124.8
*ku = -gm * Rl = 137

.lib europe.lib

V1 2 0 {Uce}
Rc 3 2 4530
Q1 3 1 4 bc107b
Re 4 0 453

Ro 3 0 10k

Rb1 2 1 225.8k
Rb2 1 0 29.4k

.param Uce=5.5
.step param Uce 0 12.5 0.5

I1 0 1 DC 1

.DC I1 0 20u 0.1u
.probe
.end
 */

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
        static void Main(string[] args)
        {
            NumberFormatInfo setPrec = new();
            setPrec.NumberDecimalDigits = 3;

            double Uce, Ic, Ucc, Ro, Fd, Ib, Ib1, Ib2, Ube, Beta, Ie, Rc, Re, Rb1, Rb2, gm, Rl, ku;
            
            Console.Write("Podaj dane:\nUce[V] = ");
            Uce = Convert.ToDouble (Console.ReadLine());

            Console.Write("Ic[mA] = ");
            Ic = Convert.ToDouble(Console.ReadLine())/1000;

            Console.Write("Ucc[V] = ");
            Ucc = Convert.ToDouble(Console.ReadLine());

            Console.Write("Ro[kOhm] = ");
            Ro = Convert.ToDouble(Console.ReadLine())*1000;

            Console.Write("Fd[Hz] = ");
            Fd = Convert.ToDouble(Console.ReadLine());

            Rc = Math.Round((Ucc - Uce) / (1.1 * Ic),2);
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

            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Gotowiec");
            File.WriteAllText(".\\Gotowiec\\Program.cir", GotowyProgram);
            File.Copy(@"E:\Szkoła\Informatyka\C#\Kolokwium-Pspice-wzmacniacz\EUROPE.LIB", ".\\Gotowiec\\EUROPE.LIB", true);

            Console.WriteLine("Zamknij Pspice i kliknij ENTER!!!");
            Console.ReadLine();

            Console.Write("Ib[uA] = ");
            Ib = Convert.ToDouble(Console.ReadLine()) / 1000000;

            Console.Write("Ube[mV] = ");
            Ube = Convert.ToDouble(Console.ReadLine()) / 1000;

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
            Console.WriteLine($"Wszytstkie zmienne są w lokalizacji {Directory.GetCurrentDirectory()}\\Zmienne.txt");

            Console.ReadKey();

        }
    }
}