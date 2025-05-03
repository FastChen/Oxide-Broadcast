# 功能
* **全部文本内容均支持多语言**
* **全部语言文件均支持变量**
* 多语言和变量支持的随机广播通知
* 玩家加入/离开消息广播通知
* 玩家高延迟检测/广播通知

# 聊天命令

* `/bc ping` - 检测你与服务器的延迟

# 变量

变量由是插件提供的。
你可以在广播中和语言文件中使用变量。

### 目前可用的变量

* `[@PLAYER] ` - 接收到消息的玩家名称
  * 当玩家加入/离开时 `[@PLAYER] ` 是触发此事件的玩家名称
* `[@PING]` -  收到消息的玩家延迟 (ms)
* `[@PINGLIMIT]` - 插件设置中 `Ping Options > High Ping Limit (Ms)` 设置的值

**变量必须按照以下格式使用:**
1. 全部字母大写
2. 在变量字母第一位以 `@` 符号开始
3. 使用 `[` 符号开头
4. 使用 `]` 符号结尾，将整体包裹起来

# 配置文件

## 插件配置

插件配置路径: `\oxide\config\Broadcast.json`

插件配置用于开启或禁用功能，或设置一些功能的参数。

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

## 广播配置

广播配置文件路径:`\oxide\data\Broadcast\*.json`

`广播配置` 是用于存储不同语言的广播内容以便发送给对应语言的玩家。

### 它是如何工作的?

> **先在玩家语言对应的广播文件中获取广播内容，如果没有对应的文件或者内容，就会去服务器默认语言文件中寻找广播内容。(服务器默认语言通常为 en 也就是 en.json)。**
> 如果当全部都找不到内容时，则不会发送内容给玩家，而是在服务器日志中提示警告信息。

为了支持多语言广播的发送，你可用前往 `\oxide\data\Broadcast\` 文件夹配置多国语言 `Language_code.json` 的广播文件。

例如 `\oxide\data\Broadcast\en.json` (en.json 文件大部分情况下是服务器默认语言)
```json
{
  "BroadcastList": [
    "Hello, [@PLAYER] !",
    "this is a Broadcast.",
    "This Broadcast came from \\oxide\\data\\Broadcast\\en.json file!"
  ]
}
```

例如 `\oxide\data\Broadcast\zh-CN.json` 这个文件的广播内容将会发送给使用中文的玩家。
```json
{
  "BroadcastList": [
    "你好, [@PLAYER] !",
    "这是一条广播。",
    "这条广播来自 \\oxide\\data\\Broadcast\\zh-CN.json 文件中!"
  ]
}
```

或者创建 `\oxide\data\Broadcast\ru.json` 则会发送给俄罗斯的玩家。

```json
{
  "BroadcastList": [
    "Здравствуйте. [@PLAYER] !",
    "Я не понимаю русский язык."
  ]
}
```

# 本地化

你可以创建或编辑 `\oxide\lang\{语言文件夹}\Broadcast.json` 中的内容来实现本地化！
