<h1 align="center">Config.json File</h1>

- [README](https://github.com/Yti890/DiscordIntegration/blob/master/README.md)
- [Old README](./README.old.md)
- [EXILED README](./README.EXILED.md)
- [LabAPI README](./README.LabAPI.md)
- [Create Discord Bot README](./README.CDB.md)  

<h2 align="center">How configure the execution of game commands through Discord</h2>

- Open your bot `config.json` file.
- Add role IDs and list every command they can execute. You can use `.*` to permit to that role ID to use all game commands without restrictions.
  
```json
  "ValidCommands": {
    "1": {
      "953784342595915779": [
        "di"
      ]
    }
  },
 ```

- **Never duplicate commands.** Higher roles on your Discord server will be able to use lower roles commands as well, based on the position of the roles.

<h2>Available commands</h2>

| Command | Description | Arguments | Permission | Example |
| --- | --- | --- | --- | --- |
| di playerlist | Gets the list of players in the server. | | di.playerlist | **di playerlist** |
| di stafflist | Gets the list of staffers in the server. | | di.stafflist | **di stafflist** |
