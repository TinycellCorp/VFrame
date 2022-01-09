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

# DOTween

* add scripting define symbol
```text
VFRAME_DOTWEEN
```