namespace StorEvil.Configuration
{
    public interface IConfigSource
    {
        ConfigSettings GetConfig(string directoryOrFile);
    }
}