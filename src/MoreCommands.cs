using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
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

            //Bases
            api.RegisterCommand("me", "Send message as player action", "/me msg", MeCommand, Privilege.chat);
            api.RegisterCommand("tell", "Send private message to other player", "/tell player msg", TellCommand, Privilege.chat);
            api.RegisterCommand("reply", "Reply on last private message", "/reply msg", ReplyCommand, Privilege.chat);

            //Aliases
            api.RegisterCommand("t", "Send private message to other player", "/t player msg", TellCommand, Privilege.chat);
            api.RegisterCommand("r", "Reply on last private message", "/r msg", ReplyCommand, Privilege.chat);
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
                if (receiver.Entity.Properties.Server == null) receiver.Entity.Properties.Server = new EntityServerProperties(null);
                if (receiver.Entity.Properties.Server.Attributes == null) receiver.Entity.Properties.Server.Attributes = new TreeAttribute();
                receiver.Entity.Properties.Server?.Attributes?.SetString("lastTellFromPlayer", receiver.PlayerName);

                api.SendMessage(receiver, GlobalConstants.GeneralChatGroup, Lang.Get("You whispers to") + " " + receiver.PlayerName + " : " + msg, EnumChatType.OwnMessage);
                api.SendMessage(receiver, GlobalConstants.GeneralChatGroup, sender.PlayerName + " " + Lang.Get("whispers") + ": " + msg, EnumChatType.OwnMessage);
            }
            else
            {
                api.SendMessage(sender, GlobalConstants.CurrentChatGroup, Lang.Get("Error: Player not found"), EnumChatType.OwnMessage);
            }
        }
        public void ReplyCommand(IServerPlayer sender, int groupId, CmdArgs args)
        {
            if (args == null || args.Length < 1)
            {
                api.SendMessage(sender, GlobalConstants.CurrentChatGroup, Lang.Get("Syntax: /reply msg"), EnumChatType.OwnMessage);
                return;
            }

            string receiverName = sender.Entity.Properties.Server?.Attributes?.GetString("lastTellFromPlayer");
            if (receiverName != null)
            {
                TellCommand(sender, GlobalConstants.CurrentChatGroup, new CmdArgs(new string[] { receiverName, args[0] }));
            }
            else
            {
                api.SendMessage(sender, GlobalConstants.CurrentChatGroup, Lang.Get("You haven't received any private messages"), EnumChatType.OwnMessage);
            }
        }
    }
}