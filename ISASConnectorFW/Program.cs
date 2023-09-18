using IsasConnectorBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace ISASConnectorFW
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            ISASService service = new ISASService();

            string filePath = @"C:\Tmp\Definice.csv";

            // read file to string
            string fileContent = System.IO.File.ReadAllText(filePath, Encoding.GetEncoding(1250));

            // split fileContent to array of strings by new line
            string[] lines = fileContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            List<List<string>> entries = new List<List<string>>();
            List<string> keys = new List<string>();

            foreach (string line in lines)
            {
                if (keys.Count == 0)
                {
                    keys = line.Split(';').Select(x => x.Replace("NDB_ATRIBUT", "ATRIBUT")).ToList();
                }
                else
                {
                    List<string> entry = new List<string>();

                    // split line to array of strings by semicomma
                    string[] columns = line.Split(';');

                    int columnCounter = 0;
                    foreach (string column in columns)
                    {
                        columnCounter++;
                        if(columnCounter <= keys.Count)
                        {
                            if (column is null)
                            {
                                entry.Add("NULL");
                            }
                            //test if column is numeric
                            else if (int.TryParse(column, out int n))
                            {
                                // if column is numeric, add it to list
                                entry.Add(column);
                            }
                            else
                            {
                                // if column is not numeric, add it to list with quotes
                                entry.Add($"'{column.GetAtribut()}'");
                            }
                            if (entry.Count == keys.Count) entries.Add(entry);
                        }
                    }
                }
            }

            List<string> errorMessages = new List<string>();
            int counter = 0;

            foreach(var entry in entries)
            {
                string query = $"INSERT INTO CCAR_DOPLNENI_PAR_UDAJE ({String.Join(",", keys)}) VALUES ({String.Join(",",entry)})";

               try
                {
                    var result = service.GetOutput(new InputDTO()
                    {
                        DatabaseName = "CVICNA",
                        UserName = "pihepa",
                        Password = "Tajne.heslo.3333",
                        Roles = "CCASP_FUU069F",
                        SqlQuery = query
                    });
                    counter++;
                }
                catch (Exception ex)
                {
                    errorMessages.Add(ex.Message);
                }
            }


            Console.WriteLine($"Úspěšně importováno {counter} záznamů. {errorMessages.Count} záznamů se importovat nepodařilo.");

            //string queryCLR = Queries.test02;

            //var result = service.GetOutput(new InputDTO()
            //{
            //    DatabaseName = "CVICNA",
            //    UserName = "pihepa",
            //    Password = "Tajne.heslo.3333",
            //    Roles = "CCASP_FUU069F",
            //    SqlQuery = queryCLR
            //});

            //Console.WriteLine(result.Records.Count);
            Console.ReadLine();
        }
    }
}
