namespace Pizza.Specifications.Model
{
    public interface IMenuItem
    {
        string Description { get; }
        decimal RetailPrice { get; }
    }
}