using IsasConnectorBase;
using System;

namespace ISASConnectorFW
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            ISASService service = new ISASService();

            string query = Queries.test02;

            var result = service.GetOutput(new InputDTO()
            {
                DatabaseName = "CVICNA",
                UserName = "pihepa",
                Password = "Tajne.heslo.3333",
                Roles = "CCASP_FUU069F",
                SqlQuery = query
            });

            Console.WriteLine(result.Records.Count);
            Console.ReadLine();
            /xxxx
        }
    }
}
