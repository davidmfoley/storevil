using StorEvil.Core;

namespace StorEvil.Parsing
{
    public interface IStoryParser
    {
        Story Parse(string storyText, string id);
    }
}