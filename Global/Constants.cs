using System.Windows.Media;
using LiveCharts.Wpf;

namespace ZarzadzanieFinansami;

public abstract class Constants
{
    public static int NUMBEROFROWS = 50;
    public static int SIZEOFLEGEND = 15;
    public static readonly int STATICNUMBEROFCOLUMNS = 6;
    public static readonly string DEFAULTCLOCK = "00/00/0000 00:00:00";
    public static readonly string NULLPAGE = " 0 - 0 ";
    public static readonly string NULLROWNUMBER = " 000/000 ";
    //public static readonly string WHITESPACEPIECHART =
        //"\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800";
        public static readonly string WHITESPACEPIECHART =
            "\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800";
    public static readonly Dictionary<int, int> RAWVALUES  = new()
    {
        { 0, 10 },
        { 1, 20 },
        { 2, 50 },
        { 3, 100 },
        { 4, 200 },
        { 5, 500 },
        { 6, 1000 }
    };
    public static readonly List<string> DEFAULTCOLUMNS = new()
    {
        "ListaTranzakcji.ID",
        "ListaTranzakcji.Nazwa",
        "Kwota",
        "Data",
        "Uwagi",
        "Kategorie.Nazwa"
    };
    public static readonly string[] EXPECTEDSCHAMES = [
        "CREATE TABLE \"Kategorie\" (\r\n\t\"ID\"\tINTEGER NOT NULL,\r\n\t\"Nazwa\"\tTEXT NOT NULL,\r\n\tPRIMARY KEY(\"ID\" AUTOINCREMENT)\r\n)",
        "CREATE TABLE \"ListaTranzakcji\" (\n    \"ID\" INTEGER NOT NULL,\n    \"Nazwa\" TEXT NOT NULL,\n    \"Kwota\" REAL NOT NULL,\n    \"Data\" TEXT NOT NULL,\n    \"Uwagi\" TEXT,\n    \"KategoriaID\" INTEGER NOT NULL,\n    PRIMARY KEY(\"ID\" AUTOINCREMENT),\n    CONSTRAINT \"KategorieForeignKey\" FOREIGN KEY(\"KategoriaID\") REFERENCES \"Kategorie\"(\"ID\") ON DELETE CASCADE\n)"
    ];
    
    public static readonly ColorsCollection COLORS =
    [
        Colors.GreenYellow, 
        Colors.Green, 
        Colors.DarkGreen, 
        Colors.LawnGreen,
        Colors.SeaGreen, 
        Colors.PaleGreen, 
        Colors.LightGreen, 
        Colors.SpringGreen, 
        Colors.ForestGreen,
        Colors.Chartreuse,
    ];
    
}