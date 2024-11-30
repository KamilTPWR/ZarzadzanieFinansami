namespace ZarzadzanieFinansami;

public class Transaction
{
    public int ID { get; set; }
    public string Nazwa { get; set; }
    public double Kwota { get; set; }
    public string Data { get; set; }
    public string Uwagi { get; set; }

    public Transaction(int id,string nazwa, double kwota, string data, string uwagi)
    {
        ID = id; 
        Nazwa = nazwa;
        Kwota = kwota;
        Data = data;
        Uwagi = uwagi;
    }
}