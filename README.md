Installation
===

Dependencies
---

* [UniTask](https://github.com/Cysharp/UniTask)
* [VContainer](https://github.com/hadashiA/VContainer)
* [DoTween](https://openupm.com/packages/com.demigiant.dotween/)

manifest.json
---

```json
{
    "dependencies": {
        "com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
        "jp.hadashikick.vcontainer": "https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer#1.11.0",
        "com.demigiant.dotween": "1.2.632"
    },
    "scopedRegistries": [
        {
            "name": "package.openupm.com",
            "url": "https://package.openupm.com",
            "scopes": [
                "com.openupm",
                "com.demigiant.dotween"
            ]
        }
    ]
}
```

Add Package from git url
---

```
https://github.com/qkrsogusl3/VFrame.git?path=Assets/VFrame
```



