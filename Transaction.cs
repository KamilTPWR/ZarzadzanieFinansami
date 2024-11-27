namespace ZarzadzanieFinansami;

public class Transaction
{
    public string Nazwa { get; set; }
    public double Kwota { get; set; }
    public string Data { get; set; }
    public string Uwagi { get; set; }

    public Transaction(string nazwa, double kwota, string data, string uwagi)
    {
        Nazwa = nazwa;
        Kwota = kwota;
        Data = data;
        Uwagi = uwagi;
    }
}