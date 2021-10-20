# IVI configuration
Create the following file (create folders as needed) to provide IVI configuration through User Secrets:

Windows: `%APPDATA%\Microsoft\UserSecrets\293dde30-ae44-4f1e-80ca-f6490ce9124d\secrets.json`

Linux/macOS: `~/.microsoft/usersecrets/293dde30-ae44-4f1e-80ca-f6490ce9124d/secrets.json`

with the following data:
```json
{
  "IviConfiguration:EnvironmentId": "REPLACE_WITH_YOUR_VALUE",
  "IviConfiguration:ApiKey": "REPLACE_WITH_YOUR_VALUE",
  "IviConfiguration:Host": "REPLACE_WITH_YOUR_VALUE"
}
```

Where did `293dde30-ae44-4f1e-80ca-f6490ce9124d` come from? It comes from the `<UserSecretsId>293dde30-ae44-4f1e-80ca-f6490ce9124d</UserSecretsId>` setting in the .csproj file.