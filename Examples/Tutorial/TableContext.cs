using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StorEvil.Utility;

namespace Tutorial
{
    [StorEvil.Context]
    public class TableContext
    {
        private string[][] _groups;
        private List<TeamInfo> _teams = new List<TeamInfo>();
        private Hashtable _roster;

        public void Given_the_following_competition_groups(string[][] groupInfo)
        {
            _groups = groupInfo;
        }
        public void then_there_should_be_number_groups(int number)
        {
            _groups.Length.ShouldEqual(number);
        }

        public void then_team1_and_team2_should_be_in_the_same_group(string team1, string team2)
        {
            var inSameGroup = _groups.Any(
                group => group.Contains(team1)
                         && group.Contains(team2));
            inSameGroup.ShouldEqual(true);
        }

        public void Given_the_following_teams(TeamInfo[] teamInfo)
        {
            _teams.AddRange(teamInfo);
        }

        public void then_there_should_be_count_teams(int count)
        {
            _teams.Count.ShouldEqual(count);
        }

        public void then_nation_should_be_ranked_expectedRank(string nation, int expectedRank)
        {
            var team = GetTeamByNationName(nation);
            team.ShouldNotBeNull();
            team.Rank.ShouldEqual(expectedRank);
        }

        private TeamInfo GetTeamByNationName(string nation)
        {
            return _teams.FirstOrDefault(t => t.Nation == nation);
        }

        public void then_nation_should_be_in_region(string nation, Regions region)
        {
            GetTeamByNationName(nation).Region.ShouldEqual(region);
        }

        public void Given_the_following_team(TeamInfo teamInfo)
        {
            _teams.Add(teamInfo);
        }

        public void Given_the_following_roster(Hashtable rosterTable)
        {
            _roster = rosterTable;
        }

        public void then_there_should_be_count_players(int count)
        {
            _roster.Count.ShouldEqual(count);
        }

        public void then_playerName_should_be_number_expectedNumber(string playerName, 
            int expectedNumber)
        {
            _roster[expectedNumber.ToString()].ShouldEqual(playerName);
        }
    }

    public class TeamInfo
    {
        public int Rank { get; set; }
        public string Nation  { get; set; }
        public Regions Region { get; set; }
    }

    public enum Regions
    {
        Europe,
        SouthAmerica,
        NorthAmerica
    }
}
