namespace Pizza.Model
{
    public interface IMenuItem
    {
        string Description { get; }
        decimal RetailPrice { get; }
    }
}