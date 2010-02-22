namespace StorEvil
{
    public interface IConfigParser
    {
        ConfigSettings Read(string contents);
    }
}