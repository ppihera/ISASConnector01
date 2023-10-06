using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ISASWPFInterfaceFW
{
    internal static class IRESHelper
    {
        static List<TemplateDTO> templates;

        static void LoadTemplates()
        {
            templates = new List<TemplateDTO>();
            string content = Resources.Fields;
            string[] lines = content.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] items = line.Split(new char[] { ';' });
                if (items.Length > 3)
                {
                    templates.Add(new TemplateDTO { UserTableName = items[0], DbTableName = items[1], UserColName = items[2], DbColName = items[3] });
                }
            }
        }


        internal static string GetAtribut(this string ndbAtribut)
        {
            switch (ndbAtribut)
            {
                case "Rok": return "ROK";
                case "Dokladová řada": return "DOKL_RADA";
                case "Číslo dokladu": return "CISLO_DOKLADU";
                case "Variabilní symbol": return "VAR_SYMBOL";
                case "Konstantní symbol": return "KONS_SYMBOL";
                case "Specifický symbol": return "SPEC_SYMBOL";
                case "Rok smlouvy": return "ROK_SML";
                case "Číslo smlouvy": return "CISLO_SML";
                case "Číslo položky smlouvy": return "CISLO_POLOZKY_SML";
                case "Osoba/firma": return "OSOBA";
                case "Rok Z/P": return "ROK";
                case "Dokladová řada Z/P": return "DOKL_RADA_ZAV_POHL";
                case "Číslo dokladu Z/P": return "CISLO_DOKLADU";
                default: return ndbAtribut;
            }
        }

        internal static AttributeDTO FixAttribute(this AttributeDTO attribute)
        {
            if (templates == null) LoadTemplates();
            foreach (TemplateDTO template in templates)
            {
                if (attribute.TableInput.Fix() == template.UserTableName.Fix()
                    ||
                    attribute.TableInput.Fix() == template.DbTableName.Fix())
                {
                    if (attribute.ColInput.Fix() == template.UserColName.Fix()
                        ||
                       attribute.ColInput.Fix() == template.DbColName.Fix())
                    {
                        attribute.TableOutput = template.DbTableName;
                        attribute.AtributOutput = template.DbColName;
                        return attribute;
                    }
                }
            }
            MessageBox.Show($"Tabulka: {attribute.TableInput}, atribut: {attribute.ColInput}.", "Atribut nenalezen.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            return null;
        }

        /// ze vstupního stringu odstaní všechny znaky předcházející dvojtečce
        internal static string RemovePrefix(this string input)
        {
            int index = input.IndexOf(':');
            if (index > 0)
                return input.Substring(index + 1).Trim(new char[] { ':', ' ' });
            else
                return input;
        }

        /// ze vstupního stringu vrátí první řádek
        internal static string GetFirstLine(this string input)
        {
            var items = input.Split(new Char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return items[0];
        }

        internal static string Fix(this string input)
        {
            return input.ToLower().Replace(" ", "");
        }
    }


    class TemplateDTO
    {
        public string UserTableName { get; set; }
        public string DbTableName { get; set; }
        public string UserColName { get; set; }
        public string DbColName { get; set; }
    }
}
