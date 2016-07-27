using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new VsoBotStorage.Token();
            var text = bot.GetToken("sanag@microsoft.com");
            Console.WriteLine(text);
            Console.ReadLine();
        }
    }
}