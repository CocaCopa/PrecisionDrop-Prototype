# Object Pool

A data-driven object pooling system for Unity that allows prefabs to be pooled and reused at runtime through a simple API.  
Consumers interact **only through the `PoolApi`**, avoiding manual instantiation and destruction.

The system uses a **Group + Id** model to identify pooled objects.

---

# Creating the Pool

The system requires a `PoolSettings` ScriptableObject that references a `PoolCatalog`.

You can create both assets through the Unity menu:
```
Tools → CocaCopa → Object Pool → Create Settings + Catalog
```

This will create:
```
PoolSettings.asset
PoolCatalog.asset
```

The **PoolSettings asset must be placed inside a `Resources` folder** so it can be loaded automatically at runtime.

Example structure:
```
Assets
├─ Resources
│ └─ PoolSettings.asset
│
└─ PoolCatalog.asset (Folder path does not matter)
```

The `PoolSettings` asset contains a reference to the `PoolCatalog`.  
If not done automatically, assign the created catalog to the **Catalog** field in the inspector.

Once this reference is assigned, the system will automatically initialize at runtime.

---

# Catalog Setup

Open `PoolCatalog.asset` and register pooled prefabs.

Each entry consists of:
```
GroupId
├── Id
├── Prefab
├── MaxPoolCount
├── PrewarmMode
└── PrewarmCount
```


Rules:

- `GroupId` must be unique
- `Id` must be unique within the group
- `Prefab` must not be null

---

# API Usage

Consumers should **only interact through the `PoolApi`**.

No direct catalog or pool access is required.

---

### Rent an Object
Renting an object requires you to pass at least the id of its group and the id of the prefab.
```csharp
// Quck rent:
GameObject obj = PoolApi.Rent("Your_Group_Id", "Prefab_Id");

// With parent:
GameObject obj = PoolApi.Rent("Your_Group_Id", "Prefab_Id", parentTransform);

// Keep world pos:
GameObject obj = PoolApi.Rent("Your_Group_Id", "Prefab_Id", parentTransform, true/false);
```

---

### Return an Object
To return an object to its pool, simply pass the instance:
```csharp
PoolApi.Return(obj);
```


---

# Optional: Poolable Behaviour

If a prefab needs logic when reused, implement `IPoolable`:

```csharp
public class MyObject : MonoBehaviour, IPoolable {
    public void ResetForReuse() {
        // Logic to execute when the object is rented
    }
    public void PrepareForRelease() { 
        // Logic to execute when the object is returned to the pool
    }
}
```


---

# Prewarm

Prewarm controls how many instances of a prefab are created upfront and stored in the pool.

### Group Prewarm

You can enable prewarming for an entire group using:

- `prewarmGroup`
- `prewarmPercentage`

When enabled:
- Individual PrewarmMode on entries is ignored
- All entries in the group are prewarmed during initialization
- The amount is calculated as: `prewarmCount = MaxPoolCount * (prewarmPercentage / 100)`

Example:
- `MaxPoolCount = 100`
- `prewarmPercentage = 20`

→ 20 objects will be created upfront for that entry

### Automatic

When `PrewarmMode` is set to `Automatic`, the pool will prewarm **during initialization**.

- Runs automatically when the system starts
- Uses the configured `PrewarmCount`
- No additional code required

---

### Manual

When `PrewarmMode` is set to `Manual`, the pool will **not prewarm automatically**.

You must trigger it manually by calling:

```csharp
PoolApi.Prewarm("Your_Group_Id", "Prefab_Id");
```

---

# Safeguards

The system includes built-in protections to catch incorrect usage:

- Renting the same object twice will throw
- Returning an object that does not belong to a pool will throw
- Returning the same object twice will throw
- Missing `GroupId` or `Id` will throw
- Invalid catalog configuration is validated in the editor

These checks help catch bugs early during development.

---

# Summary

1. Register prefabs in `PoolCatalog`.
2. Access them through `PoolApi`.
3. Use **Group + Id** to rent and return objects.

This keeps object reuse centralized, safe, and efficient.
