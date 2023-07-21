using Microsoft.Extensions.Caching.Memory;
using System.Runtime.Caching;

namespace webapi.services;

public interface IGameService
{
    public Game CreateGame(List<string> players);

    public Game GetGame(string gameId);
}

public class GameService : IGameService
{
    private Dictionary<string, Game> _games = new Dictionary<string, Game>();
    private Dictionary<int, Ratio> _gameRatios = new Dictionary<int, Ratio>()
    {
        { 5, new Ratio { NumGood = 3, NumEvil = 2, PossibleGood = 6 } },
        { 7, new Ratio { NumGood = 4, NumEvil = 3, PossibleGood = 8 } },
        { 8, new Ratio { NumGood = 4, NumEvil = 8, PossibleGood = 8 } },
        { 10, new Ratio { NumGood = 6, NumEvil = 4, PossibleGood = 8} }
    };
    private IMemoryCache _cache;

    public GameService(
        IMemoryCache memoryCache)
    {
        _cache = memoryCache;
    }

    public Game CreateGame(List<string> players)
    {
        var gameId = Guid.NewGuid().ToString().Substring(25);
        var game = new Game() { GameId = gameId, Roles = new Dictionary<string, Role>()};
        Console.WriteLine(gameId);
        var ratio = _gameRatios.GetValueOrDefault(players.Count, null);
        if (ratio == null)
        {
            throw new InvalidOperationException("Invalid number of players");
        }
        var goodRoles = PullGoodRoles(ratio);
        var evilRoles = PullEvilRoles(ratio, goodRoles);

        var rand = new Random();
        players = players.Randomize();

        var roles = goodRoles.Concat(evilRoles).ToList();
        
        if (roles.Count != players.Count)
        {
            throw new SystemException();
        }

        var roleMap = new Dictionary<RoleName, Role>();
        for (int i = 0; i < roles.Count; i++)
        {
            roles[i].PlayerName = players[i];
            game.Roles[players[i]] = roles[i];
            roleMap[roles[i].RoleName] = roles[i];
        }

        if (roleMap.ContainsKey(RoleName.Oberon))
        {
            var target = goodRoles.Where(x => x.RoleName != RoleName.Lancelot).ToList().GetRandomValue();
            roleMap[RoleName.Oberon].Information.Add("You have added false information to one good player.");
        }
        if (roleMap.ContainsKey(RoleName.Mordred))
        {
            roleMap[RoleName.Mordred].Information.Add("You are not seen by Merlin.");
        }

        if (roleMap.ContainsKey(RoleName.Maelegant))
        {
            roleMap[RoleName.Maelegant].Information.Add("You may play reverse cards.");
        }

        if (roleMap.ContainsKey(RoleName.Morgana))
        {
            roleMap[RoleName.Morgana].Information.Add("You are seen by Percival;");
        }

        if (roleMap.ContainsKey(RoleName.Titania)) {
            evilRoles.GetRandomValue().IsFalsified = true;
            roleMap[RoleName.Titania].Information.Add("You have added false information to one evil player");
            if (roleMap.ContainsKey(RoleName.Oberon))
            {
                roleMap[RoleName.Titania].Information.Add("There is an Oberon in the game.");
            }
        }

        foreach (Role role in evilRoles)
        {
            var otherEvil = evilRoles.Where(i => !i.Equals(role) && i.RoleName != RoleName.Colgrevance).Select(x => x.PlayerName + " ").ToList();
            role.Sees = role.Sees.Concat(otherEvil).ToList();
            if (role.IsFalsified)
            {
                otherEvil[rand.Next(otherEvil.Count)] = "? ";
            }
            role.Information.Add($"You see the following players as your fellow evil: {String.Join(',', otherEvil)}");
        }
        if (roleMap.ContainsKey(RoleName.Merlin))
        {
            var sees = new List<string>();
            var merlinRole = roleMap[RoleName.Merlin];
            foreach (Role role in roles)
            {
                if (role.RoleName == RoleName.Lancelot || (role.Alignment == Alignment.Evil && role.RoleName != RoleName.Mordred))
                {
                    merlinRole.Sees.Add(role.PlayerName + " ");
                    sees.Add(role.PlayerName);
                }
            }

            if (merlinRole.IsFalsified)
            {
                sees.Add(roles.Where(x => !sees.Contains(x.PlayerName)).ToList().GetRandomValue().PlayerName);
                merlinRole.Information.Add("You have been Oberoned!");
            }
            merlinRole.Information.Add($"You see the following players as evil (or Lancelot): {String.Join(',', sees)}");
        }

        if (roleMap.ContainsKey(RoleName.Percival))
        {
            var sees = new List<string>();
            var percivaleRole = roleMap[RoleName.Percival];
            foreach (Role role in roles)
            {
                if (role.RoleName == RoleName.Merlin || role.RoleName == RoleName.Morgana)
                {
                    percivaleRole.Sees.Add(role.PlayerName + " ");
                    sees.Add(role.PlayerName);
                }

                if (percivaleRole.IsFalsified)
                {
                    sees.Add(roles.Where(x => !sees.Contains(x.PlayerName)).ToList().GetRandomValue().PlayerName);
                    percivaleRole.Information.Add("You have been Oberoned!");
                }

            }
            percivaleRole.Information.Add($"You see the following players as Merlin or Morgana: {String.Join(',', sees)}");
        }

        if (roleMap.ContainsKey(RoleName.Tristan))
        {
            var tristan = roleMap[RoleName.Tristan];
            var iseult = roleMap[RoleName.Iseult];
            iseult.Sees.Add(tristan.PlayerName);
            iseult.Information.Add($"You see {tristan.PlayerName} as your luxurious lover Tristan.");

            tristan.Sees.Add(iseult.PlayerName);
            tristan.Information.Add($"You see {iseult.PlayerName} as your luscious lover Iseult");
        }

        if (roleMap.ContainsKey(RoleName.Lancelot))
        {
            roleMap[RoleName.Lancelot].Information.Add("You may play reverse cards.");
        }

        if (roleMap.ContainsKey(RoleName.Arthur))
        {
            if (roleMap[RoleName.Arthur].IsFalsified)
            {
                roleMap[RoleName.Arthur].Information.Add("You have been Oberoned!");
                var goodRoleNames = goodRoles.Select(role => role.RoleName.ToString() + " ").ToList();
                goodRoleNames[rand.Next(goodRoleNames.Count)] = "? ";
                roleMap[RoleName.Arthur].Information.Add($"The following good roles are in the game: {String.Join(',', goodRoleNames)}");
            }
            else
            {
                roleMap[RoleName.Arthur].Information.Add($"The following good roles are in the game: {String.Join(',', goodRoles.Select(role => role.RoleName.ToString() + " "))}");

            }
        }

        if (roleMap.ContainsKey(RoleName.Guinevere))
        {
            var potentialTruths = roles.Where(role => role.Sees.Count > 0 && !(role.RoleName == RoleName.Guinevere)).ToList();
            var truth = potentialTruths.GetRandomValue();
            var truthString = $"{truth.PlayerName} sees {truth.Sees.GetRandomValue()}";
            var lie = roles.GetRandomValue();
            var lieTargets = roles.Where(role => !lie.Sees.Contains(role.PlayerName)).ToList();
            var lieTarget = lieTargets.GetRandomValue();
            var lieString = $"{lie.PlayerName} sees {lieTarget.PlayerName}";         
            
            if (roleMap[RoleName.Guinevere].IsFalsified)
            {
                var choose = rand.Next(4);
                switch (choose)
                {
                    case 0:  
                        truthString = $"? sees {truth.Sees.GetRandomValue()}";
                        break;
                    case 1:
                        truthString = $"{truth.PlayerName} sees ?";
                        break;
                    case 2:
                        lieString = $"? sees {lieTarget.PlayerName}";
                        break;
                    default:
                        lieString = $"{lie.PlayerName} sees ?"; 
                        break;
                }

                roleMap[RoleName.Guinevere].Information.Add("You have been Obereoned!");
            }

            if (rand.CoinFlip())
            {
                roleMap[RoleName.Guinevere].Information.Add(truthString);
                roleMap[RoleName.Guinevere].Information.Add(lieString);
            }
            else
            {
                roleMap[RoleName.Guinevere].Information.Add(lieString);
                roleMap[RoleName.Guinevere].Information.Add(truthString);
            }
        }
        _cache.Set(gameId, game);

        return game;

    }

    public Game GetGame(string gameId)
    {
        return _cache.Get<Game>(gameId);
    }
    private List<Role> PullGoodRoles(Ratio ratio)
    {
        var goodRoles = new List<Role>();

        var rand = new Random();

        if (rand.CoinFlip(2.0 / ratio.NumGood))
        {
            goodRoles.Add(new Role()
            {
                Alignment = Alignment.Good,
                RoleName = RoleName.Tristan,
            });

            goodRoles.Add(new Role()
            {
                Alignment = Alignment.Good,
                RoleName = RoleName.Iseult,
            });
        }

        var otherGood = new List<RoleName>() { RoleName.Lancelot, RoleName.Merlin, RoleName.Guinevere, RoleName.Percival };

        if (ratio.NumGood > 3)
        {
            otherGood.Add(RoleName.Titania);
            otherGood.Add(RoleName.Arthur);
        }
        otherGood = otherGood.Randomize();
        while (goodRoles.Count < ratio.NumGood)
        {
            var i = rand.Next(otherGood.Count);
            goodRoles.Add(new Role()
            {
                Alignment = Alignment.Good,
                RoleName = otherGood[i],
            });
            otherGood.RemoveAt(i);
        }
        return goodRoles;
    }
    
    private List<Role> PullEvilRoles(Ratio ratio, List<Role> goodRoles)
    {
        var rand = new Random();
        var evilRoles = new List<Role>();
        var possibleEvil = new List<RoleName>() { RoleName.Mordred, RoleName.Morgana, RoleName.Oberon };
        if (ratio.NumEvil + ratio.NumGood > 7)
        {
            possibleEvil.Add(RoleName.Agravaine);
        }

        if (ratio.NumEvil + ratio.NumGood > 8)
        {
            possibleEvil.Add(RoleName.Colgrevance);
        }
        possibleEvil = possibleEvil.Randomize();
        if (goodRoles.GetRole(RoleName.Percival) != null)
        {
            if (!goodRoles.Where(i => i.RoleName == RoleName.Merlin).Any())
            {
                evilRoles.Add(new Role()
                {
                    RoleName = RoleName.Morgana,
                    Alignment = Alignment.Evil,
                    Information = new List<string>()
                });
                possibleEvil.Remove(RoleName.Morgana);
            }
        }

        while (evilRoles.Count < ratio.NumEvil)
        {
            var i = rand.Next(possibleEvil.Count);
            evilRoles.Add(
                new Role()
                {
                    Alignment = Alignment.Evil,
                    RoleName = possibleEvil[i],
                });
            possibleEvil.RemoveAt(i);
        }
        return evilRoles;
    }
}

public static class RandomString
{
    public static T GetRandomValue<T>(this List<T> list)
    {
        return list[(new Random()).Next(0, list.Count)];
    }

    public static List<T> Randomize<T>(this List<T> list)
    {
        var rand = new Random();
        return list.OrderBy(i => rand.Next()).ToList();
    }

    public static Role GetRole(this List<Role> list, RoleName roleName)
    {
        return list.Where(role => role.RoleName == roleName).FirstOrDefault();
    }

    public static bool CoinFlip(this Random rand, double prob = 0.5)
    {
        return prob > rand.NextDouble();
    }

}