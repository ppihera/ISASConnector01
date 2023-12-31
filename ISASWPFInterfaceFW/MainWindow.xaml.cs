﻿using IsasConnectorBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

                string fileContent = null;
                // read file to string
                try
                {
                    fileContent = System.IO.File.ReadAllText(filename, Encoding.GetEncoding(1250));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

                // split fileContent to array of strings by new line
                string[] lines = fileContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                List<Entry> entries = new List<Entry>();
                List<string> keys = new List<string>();
                viewModel.Errors = new System.Collections.ObjectModel.ObservableCollection<ErrorDTO>();

                int lineCounter = 0;
                int attributeIndex = 0;
                int tableIndex = 0;

                foreach (string line in lines)
                {
                    lineCounter++;
                    if (keys.Count == 0)
                    {
                        keys = line.Split(';').Select(x => x.Replace("NDB_ATRIBUT", "ATRIBUT")).ToList();
                        int i = 0;
                        foreach(string key in keys)
                        {
                            if (key.Fix().Contains("tabulka")) tableIndex = i;
                            if (key.Fix().Contains("atribut")) attributeIndex = i;
                            i++;
                        }
                    }
                    else
                    {
                        Entry entry = new Entry() { Index = lineCounter};

                        // split line to array of strings by semicomma
                        string[] columns = line.Split(';');
                        
                        if(columns.Count() >= keys.Count)
                        {
                            string attribute = columns[attributeIndex];
                            string table = columns[tableIndex];

                            AttributeDTO attributeDTO = new AttributeDTO() { ColInput = attribute, TableInput = table };
                            attributeDTO = attributeDTO.FixAttribute();

                            if (attributeDTO == null) continue;

                            int columnCounter = 0;
                            foreach (string column in columns)
                            {
                            
                                if (columnCounter < keys.Count)
                                {
                                    if(columnCounter == attributeIndex)
                                    {
                                        entry.Columns.Add($"'{attributeDTO.AtributOutput}'");
                                    }
                                    else if (columnCounter == tableIndex)
                                    {
                                        entry.Columns.Add($"'{attributeDTO.TableInput}'");
                                    }
                                    else if (column is null)
                                    {
                                        entry.Columns.Add("NULL");
                                    }
                                    //test if column is numeric
                                    else if (!column.StartsWith("0") && int.TryParse(column, out int n))
                                    {
                                        // if column is numeric, add it to list
                                        entry.Columns.Add(column);
                                    }
                                    else
                                    {
                                        // if column is not numeric, add it to list with quotes
                                        entry.Columns.Add($"'{column.GetAtribut()}'");
                                    }
                                    if (entry.Columns.Count == keys.Count) entries.Add(entry);
                                }
                                columnCounter++;
                            }
                        }
                       
                    }
                }

                int counter = 0;
                foreach (var entry in entries)
                {
                    string query = $"INSERT INTO CCAR_DOPLNENI_PAR_UDAJE ({String.Join(",", keys)}) VALUES ({String.Join(",", entry.Columns)})";

                    try
                    {
                        var result = isasService.GetOutput(new InputDTO()
                        {
                            DatabaseName = this.TextBoxDatabase.Text,
                            UserName = this.TextBoxUser.Text,
                            Password = this.PasswordBox.Password,
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
                                Fields = entry.Columns,
                                Index = entry.Index,
                                Message = ex.Message.RemovePrefix().GetFirstLine()
                            });

                        if(viewModel.Errors.Count == 1)
                        {
                            if (MessageBox.Show($"Databáze ohlásila chybu: {ex.Message}.\nChcete pokračovat?", "Chyba", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No) break;
                        }
                    }
                }
                viewModel.Keys = keys;
          
                MessageBox.Show($"Úspěšně importováno {counter} záznamů. {viewModel.Errors.Count} záznamů se importovat nepodařilo.");
                viewModel.OnPropertyChanged("Errors");
            }
        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            UploadFromFile();
        }

        private void ButtonClr_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = isasService.GetOutput(new InputDTO()
                {
                    DatabaseName = this.TextBoxDatabase.Text,
                    UserName = this.TextBoxUser.Text,
                    Password = this.PasswordBox.Password,
                    Roles = "CCASP_FUU069F",
                    SqlQuery = "delete from CCAR_DOPLNENI_PAR_UDAJE"
                });

                MessageBox.Show("Hotovo. Tabulka byla vymazána.", "Smazání celé tabulky", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba při komunikaci s databází:", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            Export();
        }

        void Export()
        {
            // Export viewmodel.Errors to csv file
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Join(";", viewModel.Keys));

            foreach (var error in viewModel.Errors)
            {
                sb.AppendLine($"{String.Join(";", error.Fields)};{error.Message}");
            }
            // Save sb.ToString() to file with SaveFileDialog
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Chyby"; // Default file name
            dlg.DefaultExt = ".csv"; // Default file extension
            dlg.Filter = "CSV documents (.csv)|*.csv"; // Filter files by extension

            // Show save file dialog box
            bool? fileResult = dlg.ShowDialog();
            if (fileResult == true)
            {
                // Get the selected file name and display in a TextBox
                string filename = dlg.FileName;

                // write sb.ToString() to file
                try
                {
                    System.IO.File.WriteAllText(filename, sb.ToString().Replace("'", ""), Encoding.GetEncoding(1250));
                    MessageBox.Show("Hotovo. Soubor byl uložen.", "Export záznamů o chybě", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Chyba při ukládání souboru:", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
              
        }
    }
}
