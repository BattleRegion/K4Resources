using UnityEngine;
using System.Collections;

public class GameRouteConfig
{
    #region temp
    static readonly public string FreeDiamond = "area.playerHandler.freeDiamond";
    #endregion

    #region account
    static readonly public string BindAccount = "area.playerHandler.bindAccount";
    static readonly public string GetFriendList = "area.friendHandler.getFriends";
    static readonly public string GetFriendRequests = "area.friendHandler.getRequests";
    static readonly public string SendFriendRequest = "area.friendHandler.sendRequest";
    static readonly public string ReplyFriendRequests = "area.friendHandler.replyRequest";
    static readonly public string RemoveFriend = "area.friendHandler.removeFriend";
    static readonly public string RemoveRequest = "area.friendHandler.removeRequest";
    static readonly public string SearchFriend = "area.friendHandler.searchInfo";
    #endregion

    #region Lottery
    static readonly public string Lottery = "area.lotteryHandler.lottery";
    static readonly public string DiamondLottery = "area.lotteryHandler.rotate";
    static readonly public string ResetLotteryTurntable = "area.lotteryHandler.resetRotate";
    #endregion

    #region player
    static readonly public string ChangeTeam = "area.playerHandler.queue"; //保存当前队伍
    static readonly public string PlayerInit = "connector.entryHandler.playerInit";
	static readonly public string PvpPlayerInit = "together.entryHandler.playerInit";
    static readonly public string CreateRole = "connector.entryHandler.createRole";
    static readonly public string PlayerReconnect = "connector.entryHandler.playerReconnect";
    static readonly public string ExpandPetHouse = "area.petHandler.expandWarehouse";
    static readonly public string ExpandWareHouse = "area.bagHandler.expandWarehouse";
    static readonly public string MakeTeam = "area.petHandler.makeTeam";
    static readonly public string BuyEnergy = "area.playerHandler.buyEnergy";
    static readonly public string PetSkill = "area.petHandler.riseMonsterSkill";
    static readonly public string WeaponSkill = "area.hardwareHandler.riseWeaponSkill";
    static readonly public string RecoverEnergy = "area.dungeonHandler.receiveAwards"; //使用体力奖励池
    static readonly public string GetEnergyPool = "area.dungeonHandler.getAwards"; //获取体力奖励池
    #endregion

    #region pet
    static readonly public string SalePet = "area.petHandler.sellMonsters";
    static readonly public string UpgradePet = "area.petHandler.riseMonster";
    static readonly public string EvoPet = "area.petHandler.evolveMonster";
    #endregion

    #region config
    static readonly public string UpdateConfig = "connector.entryHandler.updateConfig";
    #endregion

    #region dungeion
    static readonly public string GetHelpFriends = "area.dungeonHandler.getHelpFriends";

    static readonly public string EnterScene = "area.dungeonHandler.enterScene";

    static readonly public string OverScene = "area.dungeonHandler.overScene";

    static readonly public string relive = "area.dungeonHandler.relive";

    static readonly public string leaveScene = "area.dungeonHandler.leaveScene";

    static readonly public string guideStep = "area.playerHandler.guideStep";

    #endregion

    #region hardware
    static readonly public string WearWeapon = "area.hardwareHandler.wearWeapon";
    static readonly public string MakeHardware = "area.hardwareHandler.composeWeapon";
    static readonly public string EvoHardware = "area.hardwareHandler.evolveWeapon";
    static readonly public string RiseHardware = "area.hardwareHandler.riseWeapon";
    static readonly public string SellHardware = "area.hardwareHandler.sellWeapons";
    #endregion

    #region item
    static readonly public string SellItem = "area.bagHandler.sellItems";
    #endregion

    #region PVP
    static readonly public string GetTicket= "area.pvpHandler.getTicket";
	static readonly public string GetLastRank = "area.pvpHandler.getLastRank";
	static readonly public string GetSeasonRank = "area.pvpHandler.getSeasonRank";

    //pvp 排队
	static readonly public string PvpEnterWaiting = "together.entryHandler.enter";
    static readonly public string PvpMovePaths = "pvp.fightHandler.move";
	static readonly public string PvpReBound = "pvp.fightHandler.info";
	static readonly public string PvpSkill = "pvp.fightHandler.skill";
	static readonly public string PvpSurrender = "pvp.fightHandler.giveUp";
	static readonly public string PvpFace = "pvp.fightHandler.face";

    static readonly public string PvpOver = "area.pvpHandler.overPvp";
    #endregion
}
