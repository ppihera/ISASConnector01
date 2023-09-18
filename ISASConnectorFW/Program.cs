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
                    keys = line.Split(';').ToList();
                }
                else
                {
                    List<string> entry = new List<string>();

                    // split line to array of strings by semicomma
                    string[] columns = line.Split(';');
                    foreach (string column in columns)
                    {
                        if (column is null)
                        {
                            entry.Add("");
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
                            entry.Add($"'{column}'");
                        }
                        entries.Add(entry);
                    }
                }
            }

            string keysCompiled = "";
            foreach (string key in keys)
            {
                keysCompiled += $"{key},";
            }
            keysCompiled = keysCompiled.Trim(',');

            string valuesCompiled = "";
            foreach (List<string> entry in entries)
            {
                if(entry.Count == keys.Count)
                {
                    string values = "";
                    foreach (string column in entry)
                    {
                        values += $"{column},";
                    }
                    valuesCompiled += $"({values.Trim(',')}),";
                }
            }
            valuesCompiled = valuesCompiled.Trim(',');

            string query = $"INSERT INTO CCAR_DOPLNENI_PAR_UDAJE ({keysCompiled}) VALUES ({valuesCompiled})";

            //string query = Queries.test02;

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
        }
    }
}
