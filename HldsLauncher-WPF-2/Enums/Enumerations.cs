namespace HldsLauncher.Enums
{
    public enum ConsoleType
    {
        Integrated,
        Native
    }

    public enum GoldSourceGame
    {
        [EnumDescription("Counter-Strike 1.6")]
        cstrike,
        [EnumDescription("Counter-Strike: Condition Zero")]
        czero,
        [EnumDescription("Deathmatch Classic")]
        dmc,
        [EnumDescription("Day of Defeat")]
        dod,
        [EnumDescription("Half-Life: Opposing Force")]
        gearbox,
        [EnumDescription("Ricochet")]
        ricochet,
        [EnumDescription("Team Fortress Classic")]
        tfc,
        [EnumDescription("Half-Life")]
        valve
    }

    public enum SourceGame
    {
        [EnumDescription("Counter-Strike: Source")]
        cstrike,
        [EnumDescription("Day of Defeat: Source")]
        dod,
        [EnumDescription("Half-Life Deathmatch: Source")]
        hl1mp,
        [EnumDescription("Half-Life 2: Deathmatch")]
        hl2mp,
        [EnumDescription("Team Fortress 2")]
        tf,
        [EnumDescription("Left 4 Dead")]
        left4dead,
        [EnumDescription("Left 4 Dead 2")]
        left4dead2,
        [EnumDescription("Counter-Strike: Global Offensive")]
        csgo
        /*,
        [EnumDescription("Today")]
        garrysmod*/
    }

    public enum ServerType
    {
        GoldSource,
        Source,
        Hltv
    }

    public enum HldsUpdateStatus
    {
        NewVersionAvailable,
        NoNewVersionAvailable,
        Fail
    }
}