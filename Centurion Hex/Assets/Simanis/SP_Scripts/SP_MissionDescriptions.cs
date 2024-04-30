using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mission
{
    TheGloriousTreasure,
    Ambush,
    JustaPawn,
    ProtectthePrincess,
    BattleofHex,
    AdventureMode
}

public class SP_MissionDescriptions : MonoBehaviour
{

    public static string GetNextMissionSceneName(Mission mission)
    {
        string missionName = "SP_2";
        switch (mission)
        {
            case Mission.TheGloriousTreasure:
                missionName = "SP_2";
                break;
            case Mission.Ambush:
                missionName = "SP_3";
                break;
            case Mission.JustaPawn:
                missionName = "SP_4";
                break;
            case Mission.ProtectthePrincess:
                missionName = "SP_5";
                break;
            case Mission.BattleofHex:
                missionName = "SP_5";
                break;
            case Mission.AdventureMode:
                missionName = "SP_5";
                break;
        }
        return missionName;
    }

    public static string GetMissionName(Mission mission)
    {
        string missionName = "Mission";
        switch (mission)
        {
            case Mission.TheGloriousTreasure:
                missionName = "The Glorious Treasure";
                break;
            case Mission.Ambush:
                missionName = "Ambush";
                break;
            case Mission.JustaPawn:
                missionName = "Just a Pawn";
                break;
            case Mission.ProtectthePrincess:
                missionName = "Protect the Princess";
                break;
            case Mission.BattleofHex:
                missionName = "Battle of Hex";
                break;
            case Mission.AdventureMode:
                missionName = "Adventure Mode";
                break;
        }
        return missionName;
    }

    public static string GetMissionDescription(Mission mission)
    {
        string description = "Kill all enemy units";
        switch (mission)
        {
            case Mission.TheGloriousTreasure:
                description = "The king has grown obsessed with a particular strain of gemstone. " +
                    "By his royal decree, anyone who can obtain even a tiny jewel shall be greatly rewarded. " +
                    "Money, land, livestock - anything a heart may desire" +
                    "\n\nMISSION OBJECTIVES" +
                    "\n-Don’t die" +
                    "\n-Find the gemstone";
                break;
            case Mission.Ambush:
                description = "Having delivered the king what he desired, you have become a wealthy man. " +
                    "But it has come at a cost. Jealous nobles now see you as a rival in the royal court. " +
                    "Eliminate the cutthroats that have been hired to ambush you" +
                    "\n\nMISSION OBJECTIVES" +
                    "\n-Kill all hostiles" +
                    "\n-At least one ally must survive";
                break;
            case Mission.JustaPawn:
                description = "Your continued success has become a burden. The king now sees you as a brilliant strategist. " +
                    "He kindly requests you recover a prized jewel stolen by an evil witch. " +
                    "\n\nMISSION OBJECTIVES" +
                    "\n-Recover the jewel";
                break;
            case Mission.ProtectthePrincess:
                description = "The princess has grown weary of her fathers obsession with shiny trinkets. " +
                    "She has fled the castle and the city. Protect her at all costs." +
                    "\n\nMISSION OBJECTIVES" +
                    "\n-The princess must survive" +
                    "\n-Kill all hostile units";
                break;
            case Mission.BattleofHex:
                description = "The princess you saved turned out to be the evil witch. " +
                    "The enemies you slew to protect her were members of the royal court. You are now a wanted man" +
                    "\n\nMISSION OBJECTIVES" +
                    "\n-Slay the witch" +
                    "\n-Slay the king’s men" +
                    "\n-Survive";
                break;
            case Mission.AdventureMode:
                description = "You travel into the wilderness to escape your past. What adventures await you there?" +
                    "\n\nMISSION OBJECTIVES" +
                    "\n-Kill all hostiles" +
                    "\n-Survive";
                break;
        }
        return description;
    }
}
