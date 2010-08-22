namespace StorEvil.TeamCity
{
    public class TeamCityMessageWriter : ITeamCityMessageWriter
    {
        public void Write(string teamCityMessage)
        {
            System.Console.WriteLine(teamCityMessage);
        }
    }
}