namespace StorEvil.Configuration
{
    public interface IConfigParser
    {
        ConfigSettings Read(string contents);
    }
}