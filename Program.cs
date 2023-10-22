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

using System.Globalization;
using System.Windows;
using TextCopy;

namespace Kolokwium_Pspice_wzmacniacz
{
    internal class Program
    {
        static void Main(string[] args)
        {
            NumberFormatInfo setPrec = new();
            setPrec.NumberDecimalDigits = 3;

            double Uce, Ic, Ucc, Ro, Fd, Ib, Ube, Beta, Ie, I_dzielnika, Rc, Re, Rb1, Rb2, Reb, gm, Rl, ku;
            
            Console.Write("Enter Data:" +
                "\n Uce[V] = ");
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
            Re = Rc / 10;

            Console.WriteLine("\n\nOdpal ten program(już jest w schowku) i odczytaj z wykresu Ib i Ube");
            string GotowyProgram = "*Wzmacniacz\n" +
                ".lib europe.lib\n" +
                $"V1 2 0 {Uce}\n" +
                $"Rc 3 2 {Rc}\n" +
                $"Q1 3 1 4 bc107b\n" +
                $"Re 4 0 {Re}\n" +
                $"\n" +
                $"I1 0 1 DC 1\n" +
                $".DC I1 0 20u 0.1u\n" +
                $".probe\n" +
                $".end";
            Console.WriteLine(GotowyProgram);
            ClipboardService.SetText(GotowyProgram);
            Console.ReadLine();
        }
    }
}