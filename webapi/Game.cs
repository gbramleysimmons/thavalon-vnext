using Newtonsoft.Json.Converters;
using System.Data;
using System.Text.Json.Serialization;

namespace webapi;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Alignment
{
    Evil,
    Good, 
    Neutral
}


[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RoleName
{
    Lancelot,
    Merlin,
    Percival,
    Tristan,
    Iseult,
    Guinevere,
    Mordred,
    Morgana,
    Maelegant,
    Oberon,
    Titania,
    Arthur,
    Agravaine,
    Colgrevance
}

public class Role
{
    public RoleName RoleName { get; set; }

    public Alignment Alignment { get; set;  }

    public List<string> Information { get; set; } = new List<string>();

    public string PlayerName { get; set; }

    public bool IsFalsified { get; set; }

    public List<string> Sees { get; set; } = new List<string>();

}


public class Game
{
    public Dictionary<string, Role> Roles { get; set; }

    public string GameId { get; set; }
}

public class Ratio
{
    public int NumGood { get; set; }

    public int NumEvil { get; set; }

    public int PossibleGood { get; set; }
}