using StorEvil.Core;

namespace StorEvil
{
    public interface IStoryParser
    {
        Story Parse(string storyText, string id);
    }
}