Adds functionality of serializing plane classes (not derived from UnityEngine.Object) by interface or abstract class using SerializeReference

# Be careful with renaming classes, namespaces and etc because Unity doesn't really handle it at all and you'll have to fix it in some text editor by hand

Use [PlaneData] attribute on [SerializeReference] field to automatically generate dropdown of compatable field types in editor

Useful for decreasing amount of scriptable objects in project

![PlaneDataShowcase](https://user-images.githubusercontent.com/30838103/154221762-4bb81601-c41a-4dc5-8ef3-a0f0177522bf.gif)

## Example
```csharp
using UnityEngine;
using Recstazy.PlaneData;

public interface IMyInterface { }

public struct MyDataA : IMyInterface 
{
    [SerializeField]
    private float _floatValue;
}

public class MyDataB : IMyInterface 
{
    [SerializeField]
    private bool _boolValue;
}

[CreateAssetMenu(menuName = "PlaneData/Showcase")]
public class PlaneDataShowcase : ScriptableObject
{
    [PlaneData]
    [SerializeReference]
    private IMyInterface _someData;

    [PlaneData]
    [SerializeReference]
    private IMyInterface[] _dataArray;
}
```
