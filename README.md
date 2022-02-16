Use [PlaneData] attribute on [SerializeReference] field to automatically generate dropdown of compatable field types in editor

```csharp
using GameOn.PlaneData;

[PlaneData]
[SerializeReference]
private UnitEffect[] _defaultEffects;
```
