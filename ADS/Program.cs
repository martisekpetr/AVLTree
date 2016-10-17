using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVLstromy;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            AVLstrom strom = new AVLstrom();
            Help();
            string line = Console.ReadLine();
            while (line != "q")
            {
                string[] param = line.Split(' ');
                if (param.Count() == 2)
                {
                    int klic = 0;
                    if (!Int32.TryParse(param[1], out klic))    //zadani v nespravnem formatu je ignorovano
                    {
                        line = Console.ReadLine();
                        continue;
                    }
                    switch (param[0])
                    {
                        case "i": 
                            {
                                strom.Insert(klic);
                                break;
                            }
                        case "d":
                            {
                                strom.Delete(klic);
                                break;
                            }
                        case "f":
                            {
                                if (strom.Find(klic))
                                    Console.WriteLine("Klic je ve strome.");
                                else
                                    Console.WriteLine("Nenalezeno.");
                                break;
                            }
                        default:
                            break;
                    }
                }
                else if (param.Count() == 1)
                {
                    if (param[0] == "v")
                    {
                        Console.Write(strom.Vypis());
                    }
                    if (param[0] == "h")
                    {
                        Help();
                    }
                    
                }
                line = Console.ReadLine();
            }
        }

        static public void Help()
        {
            Console.WriteLine("Prikazy: ");
            Console.WriteLine("* i [x] - pridani klice x do stromu, napr 'i 5'");
            Console.WriteLine("* d [x] - smazani klice x ze stromu, napr 'd 5'");
            Console.WriteLine("* f [x] - dotaz na pritomnost klice x ve strome");
            Console.WriteLine("* v - vypis struktury stromu");
            Console.WriteLine("* q - ukonceni programu");
            Console.WriteLine("* h - napoveda");
            Console.WriteLine("-----------------------------------");
        }
    }
}
