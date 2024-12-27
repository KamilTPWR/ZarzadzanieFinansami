namespace ZarzadzanieFinansami;

public class Transaction : IComparable<Transaction>
{
    public int ID { get; set; }
    public string Nazwa { get; set; }
    public double Kwota { get; set; }
    public string Data { get; set; }
    public string Uwagi { get; set; }
    public string Kategoria { get; set; }

    public Transaction(int id, string nazwa, double kwota, string data, string uwagi, string kategoria)
    {
        ID = id;
        Nazwa = nazwa;
        Kwota = kwota;
        Data = data;
        Uwagi = uwagi;
        Kategoria = kategoria;
    }

    public int CompareTo(Transaction? other)
    {
        throw new ArgumentException("Invalid parameter for comparison.");
    }

    public int CompareTo(Transaction? other, ComparisonField parameter)
    {
        if (other == null) return 1;

        switch (parameter)
        {
            case ComparisonField.ID:
                return ID.CompareTo(other.ID);
            case ComparisonField.Nazwa:
                return string.Compare(Nazwa, other.Nazwa, StringComparison.OrdinalIgnoreCase);
            case ComparisonField.Kwota:
                return Kwota.CompareTo(other.Kwota);
            case ComparisonField.Data:
                DateTime thisDate = DateTime.ParseExact(Data, "dd.MM.yyyy", null);
                DateTime otherDate = DateTime.ParseExact(other.Data, "dd.MM.yyyy", null);
                return thisDate.CompareTo(otherDate);
            case ComparisonField.Uwagi:
                return string.Compare(Uwagi, other.Uwagi, StringComparison.OrdinalIgnoreCase);
            case ComparisonField.Kategoria:
                return string.Compare(Kategoria, other.Kategoria, StringComparison.OrdinalIgnoreCase);
            default:
                throw new ArgumentException("Invalid parameter for comparison.");
        }
    }
}