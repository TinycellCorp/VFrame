# dependencies

* unitask
* vcontainer
* dotween
* unity.localization

# installation

* manifest.json
* add scope

```json
{
  "dependencies": {
  },
  "scopedRegistries": [
    {
      "name": "package.openupm.com",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.cysharp.unitask",
        "com.demigiant.dotween",
        "com.openupm",
        "jp.hadashikick.vcontainer"
      ]
    }
  ]
}
* ```

# pacages

* openupm

```text
openupm add com.cysharp.unitask
openupm add jp.hadashikick.vcontainer
//openupm add com.demigiant.dotween
```

* package manager or manifest.json

```text
"com.unity.localization": "1.0.5"
```

