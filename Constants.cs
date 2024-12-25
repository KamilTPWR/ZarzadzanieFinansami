using System.Windows.Media;
using LiveCharts.Wpf;

namespace ZarzadzanieFinansami;

public abstract class Constants
{
    public static int NUMBEROFROWS = 50;
    public static readonly int STATICNUMBEROFCOLUMNS = 6;
    public static readonly string DEFAULTCLOCK = "00/00/0000 00:00:00";
    public static readonly string NULLPAGE = " 0 - 0 ";
    public static readonly string NULLROWNUMBER = " 000/000 ";
    public static readonly string WHITESPACEPIECHART =
        "\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800\u2800";
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
        "ID",
        "Nazwa",
        "Kwota",
        "Data",
        "Uwagi",
        "Kategorie.Nazwa"
    };
    public static readonly string[] EXPECTEDSCHAMES = [
        "CREATE TABLE \"Kategorie\" (\r\n\t\"ID\"\tINTEGER NOT NULL,\r\n\t\"Nazwa\"\tTEXT NOT NULL,\r\n\tPRIMARY KEY(\"ID\" AUTOINCREMENT)\r\n)",
        "CREATE TABLE \"ListaTranzakcji\" (\r\n\t\"ID\"\tINTEGER NOT NULL,\r\n\t\"Nazwa\"\tTEXT NOT NULL,\r\n\t\"Kwota\"\tREAL NOT NULL,\r\n\t\"Data\"\tTEXT NOT NULL,\r\n\t\"Uwagi\"\tTEXT,\r\n\t\"KategoriaID\"\tINTEGER NOT NULL,\r\n\tPRIMARY KEY(\"ID\" AUTOINCREMENT),\r\n\tCONSTRAINT \"KategorieForeignKey\" FOREIGN KEY(\"KategoriaID\") REFERENCES \"Kategorie\"(\"ID\")\r\n)"
    ];
    public static readonly ColorsCollection COLORS =
    [
        Colors.White, // 1. White
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