using System;
using eBay.Services;
using eBay.Services.Finding;
using Slf;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ConsoleFindItems
{

    class Program
    {
        static void Main(string[] args)
        {
            DB db = new DB(15);
            List<RSL> rsl = db.Query(">", 3);

            
            Console.WriteLine("Press any key to close the program.");
            Console.ReadKey();
        }
        
    }
}
