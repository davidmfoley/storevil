namespace StorEvil
{
    public interface IConfigSource
    {
        ConfigSettings GetConfig(string directoryOrFile);
    }
}