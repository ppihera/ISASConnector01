using System.Text;
using System;
using ADODB;

namespace IsasConnectorBase
{
    public class ISASService
    {
        public OutputDTO GetOutput(InputDTO inputDTO)
        {
            if (string.IsNullOrWhiteSpace(inputDTO.DatabaseName)) throw new ArgumentException("Není zadán název databáze.");
            if (string.IsNullOrWhiteSpace(inputDTO.UserName)) throw new ArgumentException("Není zadáno uživatelské jméno.");
            if (string.IsNullOrWhiteSpace(inputDTO.Password)) throw new ArgumentException("Není zadáno uživatelské heslo.");

            string connectString = GetConnectionString(inputDTO.DatabaseName, inputDTO.UserName, inputDTO.Password);
            var DB = new ADODB.Connection();
            DB.Open(connectString);

            if(!String.IsNullOrWhiteSpace(inputDTO.Roles))
            {
                DB.Execute($"set role {inputDTO.Roles}", out object retRole, 0);
            }

            ADODB.Recordset recordset = DB.Execute(inputDTO.SqlQuery, out object ret, 0);
            OutputDTO output = new OutputDTO(inputDTO);
            int counter = 0;

            if(recordset.State == 1)
            {
                while (!recordset.EOF)
                {
                    RecordDTO recordDTO = new RecordDTO() { Index = counter };
                    var fields = ((dynamic)recordset).Fields;
                    for (int i = 0; i < fields.Count; i++)
                    {
                        dynamic field = fields[i];

                        string keyString = "";
                        string valueString = "";

                        try
                        {
                            keyString = field.Name.ToString();
                        }
                        catch { }

                        try
                        {
                            valueString = field.Value.ToString();
                        }
                        catch { }

                        FieldDTR fieldDTR = new FieldDTR() { Key = keyString, Value = valueString };
                        recordDTO.Fields.Add(fieldDTR);
                        counter++;
                    }
                    output.Records.Add(recordDTO);
                    recordset.MoveNext();
                }
            }

            DB.Close();
            return output;
        }


        private string GetConnectionString(string dbName, string login, string password)
        {
            StringBuilder connectionStrig = new StringBuilder();
            connectionStrig.Append(@"PROVIDER=MSDASQL;");
            connectionStrig.Append(@"DRIVER={Microsoft ODBC for Oracle};");
            connectionStrig.Append(@"SERVER=" + dbName + ";");
            connectionStrig.Append(@"UID=" + login + ";");
            connectionStrig.Append(@"PWD=" + password);
            return connectionStrig.ToString();
        }
    }
}
