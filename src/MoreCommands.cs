using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace MoreCommands
{
    public class MoreCommands : ModSystem
    {
        ICoreServerAPI api;
        public override void StartServerSide(ICoreServerAPI api)
        {
            base.StartServerSide(api);
            this.api = api;

            api.RegisterCommand("me", "Send message as player action", "/me msg", MeCommand);
            api.RegisterCommand("tell", "Send private message to other player", "/tell player msg", TellCommand);
        }
        public void MeCommand(IServerPlayer player, int groupId, CmdArgs args)
        {
            if (args == null || args.Length < 1)
            {
                api.SendMessage(player, GlobalConstants.CurrentChatGroup, Lang.Get("Syntax: /me msg"), EnumChatType.OwnMessage);
                return;
            }

            string msg = "";
            for (int i = 0; i < args.Length; i++) msg += args[i];
            api.SendMessage(player, GlobalConstants.GeneralChatGroup, player.PlayerName + " " + msg, EnumChatType.OthersMessage);
        }
        public void TellCommand(IServerPlayer sender, int groupId, CmdArgs args)
        {
            if (args == null || args.Length < 1)
            {
                api.SendMessage(sender, GlobalConstants.CurrentChatGroup, Lang.Get("Syntax: /tell player msg"), EnumChatType.OwnMessage);
                return;
            }

            string receiverName = args[0].ToLower();
            IPlayer receiver = null;
            foreach (var player in api.World.AllOnlinePlayers)
            {
                if (player.PlayerName.ToLower() == receiverName)
                {
                    receiver = player;
                    break;
                }
            }

            string msg = "";
            for (int i = 1; i < args.Length; i++) msg += args[i];

            if (receiver != null)
            {
                api.SendMessage(receiver, GlobalConstants.GeneralChatGroup, Lang.Get("You whispers to") + " " + receiver.PlayerName + " : " + msg, EnumChatType.OwnMessage);
                api.SendMessage(receiver, GlobalConstants.GeneralChatGroup, sender.PlayerName + " " + Lang.Get("whispers") + ": " + msg, EnumChatType.OwnMessage);
            }
            else
            {
                api.SendMessage(sender, GlobalConstants.CurrentChatGroup, Lang.Get("Error: Player not found"), EnumChatType.OwnMessage);
            }
        }
    }
}