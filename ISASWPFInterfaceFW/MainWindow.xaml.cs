﻿using IsasConnectorBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ISASWPFInterfaceFW
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ISASService isasService { get; set; }
        MainViewModel viewModel { get; set; } = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            isasService = new ISASService();
            this.DataContext = viewModel;
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            var response = isasService.GetOutput(
                    new InputDTO()
                    {
                        DatabaseName = "OSMOOST",
                        UserName = "pihepa",
                        Password = "Tajne.heslo.3333",
                        Roles = "",
                        SqlQuery = "select ROCNIK from VEC where BC_VEC = 999"
                    }
                );

            MessageBox.Show(response.Records.Count().ToString());
        }

        private void UploadFromFile()
        {
            // Open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".csv"; // Default file extension
            dlg.Filter = "CSV documents (.csv)|*.csv"; // Filter files by extension

            // Show open file dialog box
            bool? fileResult = dlg.ShowDialog();
            if (fileResult == true)
            {
                // Get the selected file name and display in a TextBox
                string filename = dlg.FileName;

                // read file to string
                string fileContent = System.IO.File.ReadAllText(filename, Encoding.GetEncoding(1250));

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
                            if (columnCounter <= keys.Count)
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

                int counter = 0;
                foreach (var entry in entries)
                {
                    string query = $"INSERT INTO CCAR_DOPLNENI_PAR_UDAJE ({String.Join(",", keys)}) VALUES ({String.Join(",", entry)})";

                    try
                    {
                        var result = isasService.GetOutput(new InputDTO()
                        {
                            DatabaseName = this.TextBoxDatabase.Text,
                            UserName = "pihepa",
                            Password = "Tajne.heslo.3333",
                            Roles = "CCASP_FUU069F",
                            SqlQuery = query
                        });
                        counter++;
                    }
                    catch (Exception ex)
                    {
                        viewModel.Errors.Add(
                            new ErrorDTO()
                            {
                                Fields = entry,
                                Message = ex.Message
                            });
                    }
                }

                MessageBox.Show($"Úspěšně importováno {counter} záznamů. {viewModel.Errors.Count} záznamů se importovat nepodařilo.");
                viewModel.OnPropertyChanged("Errors");
            }
        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            UploadFromFile();
        }
    }
}
