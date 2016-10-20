using System;


namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
			AVLTree.AVLTree tree = new AVLTree.AVLTree();

			DisplayHelp();

			string line = Console.ReadLine();
            while (line != "q")
            {
                string[] param = line.Split(' ');
                if (param.Length == 2)
                {
                    int klic = 0;
					if (!Int32.TryParse(param[1], out klic))    
                    {
						// input in wrong format is ignored
                        line = Console.ReadLine();
                        continue;
                    }
                    switch (param[0])
                    {
                        case "i": 
                            {
                                tree.Insert(klic);
                                break;
                            }
                        case "d":
                            {
                                tree.Delete(klic);
                                break;
                            }
                        case "f":
                            {
                                if (tree.Find(klic))
                                    Console.WriteLine("Klic je ve strome.");
                                else
                                    Console.WriteLine("Nenalezeno.");
                                break;
                            }
                        default:
                            break;
                    }
                }
                else if (param.Length == 1)
                {
                    if (param[0] == "p")
                    {
                        Console.Write(tree.Print());
                    }
                    if (param[0] == "h")
                    {
                        DisplayHelp();
                    }
                    
                }
                line = Console.ReadLine();
            }
        }

		/// <summary>
		/// Prints the available commands.
		/// </summary>
        static public void DisplayHelp()
        {
            Console.WriteLine("Commands: ");
            Console.WriteLine("* i [x] - insert key x to the tree, e.g. 'i 5'");
            Console.WriteLine("* d [x] - delete key x from the tree, e.g. 'd 5'");
            Console.WriteLine("* f [x] - find the key in the tree (Y/N answer)");
            Console.WriteLine("* p - print out the tree");
            Console.WriteLine("* q - quit");
            Console.WriteLine("* h - help");
            Console.WriteLine("-----------------------------------");
        }
    }
}
