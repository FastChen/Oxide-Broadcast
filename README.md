# Feature
* **ALL Messages Multi-language support**
* **ALL Messages Variable Support**
* Multi-language and Variable support for randomized Broadcast.
* Player Join or Leave Message.
* Player High Ping Notification.

# Chat Commands

* `/bc ping` - Check your latency with the server

# Variables

The variables are provided by the plugin.
You can see in the broadcast file or in the messages language file the difference between using the.

### List of available variables

* `[@PLAYER] ` - Name of the player receiving this broadcast
  * When a player join/leave the broadcast, the `[@PLAYER] ` is the name of the joining/leaving player.
* `[@PING]` -  Ping of the player receiving this broadcast (ms)
* `[@PINGLIMIT]` - Plugin configure `Ping Options > High Ping Limit (Ms)` value

**Variables must be used in the following format:**

1. All uppercase letters
2. Add the `@` symbol in front of the letters
3. Starting with the `[ ` symbol
4. Ends with the `]` symbol

# Configuration

## Plugin configure

Plugin configure path: `\oxide\config\Broadcast.json`

The Default Configuration is Control Function Enablement with Base Settings

```json
{
  "Player Options": {
    "Show Player Join Message (true/false)": true,
    "Show Player Leave Message (true/false)": true
  },
  "Broadcast Options": {
    "Show Prefix (true/false)": false,
    "Enable Random Broadcast (true/false)": true,
    "Random Cycle (Seconds)": 300
  },
  "Ping Options": {
    "Enable High Ping Check (true/false)": true,
    "Notification High Ping Player (true/false)": true,
    "High Ping Limit (Ms)": 150,
    "Check Cycle (Seconds)": 300
  }
}
```

## Broadcast configure

Broadcast configure Path:`\oxide\data\Broadcast\*.json`

The `Broadcast configure` is used to store Broadcasts in different languages.

### How is work?

> **First will find player language files If not find in folder, then using server language file by default (en.json is most cases in server language)**
> If all the files are empty, no broadcast will be sent, but will be sent a warning to the console.

In order to support sending broadcasts to players in different languages,
you need to configure the contents of `\oxide\data\Broadcast\` folder `Language_code.json` here

like `\oxide\data\Broadcast\en.json` (en.json is server language in most cases)
```json
{
  "BroadcastList": [
    "Hello, [@PLAYER] !",
    "this is a Broadcast.",
    "This Broadcast came from \\oxide\\data\\Broadcast\\en.json file!"
  ]
}
```

like `\oxide\data\Broadcast\zh-CN.json` this wiil be Broadcast to Chinese players.
```json
{
  "BroadcastList": [
    "你好, [@PLAYER] !",
    "这是一条广播。",
    "这条广播来自 \\oxide\\data\\Broadcast\\zh-CN.json 文件中!"
  ]
}
```

or `\oxide\data\Broadcast\ru.json` for Russian players.

```json
{
  "BroadcastList": [
    "Здравствуйте. [@PLAYER] !",
    "Я не понимаю русский язык."
  ]
}
```

# Localization

You can create or edit the contents of `\oxide\lang\{language folder}\Broadcast.json` for localization!
