using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Configuration;
using Oxide.Core.Plugins;

namespace Oxide.Plugins;

[Info("Broadcast", "FastChen", "1.1.0")]
[Description("A multiple languages supported variables Broadcast and player status.")]
public class Broadcast : CovalencePlugin
{
    private class PluginConfig
    {
        [JsonProperty(PropertyName = "Player Options")]
        public PlayerOptions Player { get; set; } = new();

        [JsonProperty(PropertyName = "Broadcast Options")]
        public BroadcastOptions Broadcast { get; set; } = new();

        [JsonProperty(PropertyName = "Ping Options")]
        public PingOptions Ping { get; set; } = new();

        public class PlayerOptions
        {
            [JsonProperty(PropertyName = "Show Player Join Message (true/false)")]
            public bool ShowJoinMessage { get; set; } = true;
            
            [JsonProperty(PropertyName = "Show Player Leave Message (true/false)")]
            public bool ShowLeaveMessage { get; set; } = true;

            [JsonProperty(PropertyName = "Show Player Kicked Message (true/false)")]
            public bool ShowKickedMessage { get; set; } = true;

            [JsonProperty(PropertyName = "Show Player Banned Message (true/false)")]
            public bool ShowBannedMessage { get; set; } = true;

            [JsonProperty(PropertyName = "Show Player Unbanned Message (true/false)")]
            public bool ShowUnbannedMessage { get; set; } = true;
        }

        public class BroadcastOptions
        {
            [JsonProperty(PropertyName = "Show Prefix (true/false)")]
            public bool ShowPrefix { get; set; } = true;

            [JsonProperty(PropertyName = "Enable Random Broadcast (true/false)")]
            public bool EnableRandom { get; set; } = true;

            [JsonProperty(PropertyName = "Random Cycle (Seconds)")]
            public int RandomCycle { get; set; } = 300;
        }

        public class PingOptions
        {
            [JsonProperty(PropertyName = "Enable High Ping Check (true/false)")]
            public bool EnableHighPingCheck { get; set; } = true;

            [JsonProperty(PropertyName = "Notification High Ping Player (true/false)")]
            public bool NotificationHighPingPlayer { get; set; } = true;

            [JsonProperty(PropertyName = "High Ping Limit (Ms)")]
            public int HighPingLimit { get; set; } = 150;

            [JsonProperty(PropertyName = "Check Cycle (Seconds)")]
            public int CheckCycle { get; set; } = 300;
        }
    }

    private PluginConfig _config;

    void Init()
    {
        LoadDefaultConfig();
        LoadDefaultMessages();
    }

    protected override void LoadDefaultMessages()
    {
        lang.RegisterMessages(new Dictionary<string, string>
        {
            {"Prefix", "<color=#68CDFC>[ Broadcast ]</color> "},
            {"PlayerConnected", "[@PLAYER] 加入!"},
            {"PlayerDisconnected", "[@PLAYER] 离开!"},
            {"PlayerHighPing", "[@PLAYER] 当前你的 Ping 值 [@PING](ms) 高于限制: [@PINGLIMIT](ms)!"},
            {"PlayerKicked", "[@PLAYER] 已被踢出服务器，原因：[@REASON]!"},
            {"PlayerBanned", "[@PLAYER] 已被封禁，原因：[@REASON]!"},
            {"PlayerUnbanned", "[@PLAYER] 已解除封禁!"},
            
        }, this, "zh-CN");

        lang.RegisterMessages(new Dictionary<string, string>
        {
            {"Prefix", "<color=#68CDFC>[ Broadcast ]</color> "},
            {"PlayerConnected", "[@PLAYER] Joined!"},
            {"PlayerDisconnected", "[@PLAYER] Disconnected!"},
            {"PlayerHighPing", "[@PLAYER] Your Ping [@PING](ms) is above the limit: [@PINGLIMIT](ms)!"},
            {"PlayerKicked", "[@PLAYER] kicked. reason: [@REASON]!"},
            {"PlayerBanned", "[@PLAYER] banned. reason: [@REASON]!"},
            {"PlayerUnbanned", "[@PLAYER] was unbanned!"},

        },this, lang.GetServerLanguage());
    }

    protected override void LoadDefaultConfig()
    {
        _config = Config.ReadObject<PluginConfig>();

        if(_config == null)
        {
            _config = new PluginConfig();
        }

        Config.WriteObject(_config, true);
    }

    private DynamicConfigFile dataFile;
    private List<string> exampleBroadcastList = new(){ "This is Example Broadcast", "Edit oxide\\data\\Broadcast\\{LangCode}.json" ,"You Can Create More!" };

    void OnServerInitialized()
    {
        // Example: lang.GetServerLanguage()(en.json)
        if(!AutoBroadcastDataFileExists(lang.GetServerLanguage()))
        {
            dataFile = Interface.Oxide.DataFileSystem.GetDatafile(Path.Combine("Broadcast", lang.GetServerLanguage()));

            dataFile["BroadcastList"] = exampleBroadcastList;
            dataFile.Save();
        }
        // Example: zh-CN.json
        // if(!AutoBroadcastDataFileExists("zh-CN"))
        // {
        //     dataFile = Interface.Oxide.DataFileSystem.GetDatafile(Path.Combine("Broadcast", "zh-CN"));

        //     dataFile["BroadcastList"] = new List<string>(){ "这是示例 Broadcast", "编辑 oxide\\data\\Broadcast\\zh-CN.json" ,"来添加更多 Broadcast!" };;
        //     dataFile.Save();
        // }

        Puts("WORK! WORK!");
        Puts($"DEBUG: ServerLanguage:{lang.GetServerLanguage()}");
        PingCheck();
        AutoBroadcast();
    }

    [Command("bc")]
    private void BroadcastCommand(IPlayer player, string command, string[] args)
    {
        string prefix = Lang("Prefix", player.Id);

        foreach (string arg in args)
        {
            if (arg == "ping")
            {
                player.Reply($"{prefix}Pong:{player.Ping}(ms)");
            }
        }
    }

    void OnUserConnected(IPlayer player)
    {
        if(!_config.Player.ShowJoinMessage){ return; }

        SendMessageToOnlinePlayers("PlayerConnected", player.Name);
    }

    void OnUserDisconnected(IPlayer player)
    {
        if(!_config.Player.ShowLeaveMessage){ return; }

        SendMessageToOnlinePlayers("PlayerDisconnected", player.Name);
    }

    void OnUserKicked(IPlayer player, string reason)
    {
        if(!_config.Player.ShowKickedMessage){ return; }

        SendMessageToOnlinePlayers("PlayerKicked", player.Name, reason);
    }

    void OnUserBanned(string name, string id, string ipAddress, string reason)
    {
        if(!_config.Player.ShowBannedMessage){ return; }

        SendMessageToOnlinePlayers("PlayerBanned", name, reason);
    }

    void OnUserUnbanned(string name, string id, string ipAddress)
    {
        if(!_config.Player.ShowUnbannedMessage){ return; }

        SendMessageToOnlinePlayers("PlayerUnbanned", name);
    }

    void SendMessageToOnlinePlayers(string messageKey, string playerName = "", string reason = "")
    {
        if (players.Connected.Count() <= 0) return;

        foreach (var player in players.Connected)
        {
            string prefix = Lang("Prefix", player.Id);
            string message = Lang(messageKey, player.Id)
            .Replace("[@PLAYER]", playerName)
            .Replace("[@REASON]", string.IsNullOrEmpty(reason) ? "no reason" : reason);

            player.Message($"{prefix}{message}");
        }
    }

    void AutoBroadcast()
    {
        if(!_config.Broadcast.EnableRandom)
        {
            Puts("AutoBroadcast OFF!");
            return;
        }

        Puts("AutoBroadcast ON!");

        timer.Every(_config.Broadcast.RandomCycle, () =>
        {
            if (players.Connected.Count() <= 0) return;

            DynamicConfigFile broadcastData;

            foreach (var player in players.Connected)
            {
                string prefix = "";
                if(_config.Broadcast.ShowPrefix)
                {
                    prefix = Lang("Prefix", player.Id);
                }

                List<object> broadcastList = null;
                
                // 默认读取玩家Lang的通知
                // Defalut read player Lang datafile.
                if(AutoBroadcastDataFileExists(lang.GetLanguage(player.Id)))
                {
                    broadcastData = Interface.Oxide.DataFileSystem.GetDatafile(Path.Combine("Broadcast", lang.GetLanguage(player.Id)));
                    broadcastList = broadcastData["BroadcastList"] as List<object>;
                }
               
                // 如果玩家的通知为null或者为0则读取服务器语言的通知
                // if player lang datafile is null or Count = 0 will get serverlang datafile.
                if(broadcastList == null || broadcastList.Count == 0)
                {
                    broadcastData = Interface.Oxide.DataFileSystem.GetDatafile(Path.Combine("Broadcast", lang.GetServerLanguage()));
                    broadcastList = broadcastData["BroadcastList"] as List<object>;
                }

                // 如果还是为空为0就弹出警告到控制台
                // if serverlang still null or count = 0 will Warning log to console.
                if(broadcastList == null || broadcastList.Count == 0)
                {
                    LogWarning($"No broadcastList found for {player.Name} in {lang.GetLanguage(player.Id)} or {lang.GetServerLanguage()}");
                    continue;
                }

                // 发送给玩家
                // send to player.
                string message = broadcastList[new System.Random().Next(0, broadcastList.Count)].ToString()
                .Replace("[@PLAYER]", player.Name)
                .Replace("[@PING]", player.Ping.ToString())
                .Replace("[@PINGLIMIT]", _config.Ping.HighPingLimit.ToString())
                ;
                
                player.Message($"{prefix}{message}");
            }
        });
    }

    void PingCheck()
    {
        if(!_config.Ping.EnableHighPingCheck)
        {
            Puts("PingCheck OFF!");
            return;
        }

        Puts("PingCheck ON!");
        
        timer.Every(_config.Ping.CheckCycle, () =>
        {
            if (players.Connected.Count() <= 0) return;

            foreach (var player in players.Connected)
            {
                string prefix = Lang("Prefix", player.Id);
                string message = Lang("PlayerHighPing", player.Id)
                .Replace("[@PLAYER]", player.Name)
                .Replace("[@PING]", player.Ping.ToString())
                .Replace("[@PINGLIMIT]", _config.Ping.HighPingLimit.ToString())
                ;
                if(player.Ping > _config.Ping.HighPingLimit)
                {
                    if(_config.Ping.NotificationHighPingPlayer)
                    {
                        player.Reply($"{prefix}{message}");
                    }

                }
            }
        });
    }

    #region Helpers

    T GetConfig<T>(string name, T value) => Config[name] == null ? value : (T)Convert.ChangeType(Config[name], typeof(T));

    string Lang(string key, string id = null, params object[] args) => string.Format(lang.GetMessage(key, this, id), args);

    bool AutoBroadcastDataFileExists(string lang)
    {
        return Interface.Oxide.DataFileSystem.ExistsDatafile(Path.Combine("Broadcast", lang));
    }

    #endregion
}