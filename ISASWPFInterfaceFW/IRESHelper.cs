using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISASWPFInterfaceFW
{
    internal static class IRESHelper
    {
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
                case "Rok Z/P": return "ROK_PAR";
                case "Dokladová řada Z/P": return "DOKL_RADA_PAR";
                case "Číslo dokladu Z/P": return "CISLO_DOKLADU_PAR";
                default: return ndbAtribut;
            }
        }
    }
}
