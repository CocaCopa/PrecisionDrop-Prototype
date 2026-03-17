# Prefab Registry

Prefab Registry is a small utility that allows prefabs to be registered in a catalog and instantiated at runtime through a simple API.  
Consumers interact **only through the PrefabRegistry API**, avoiding hard prefab references across the codebase.

The registry acts as a centralized lookup for prefabs using a **Group + Key** model.

---

# Creating the Registry

The registry requires a `PrefabRegistrySettings` ScriptableObject that references a `PrefabCatalog`.

You can create both assets through the Unity menu:

```
Tools → CocaCopa → Prefab Registry → Create Settings + Catalog
```

This will create:

```
PrefabRegistrySettings.asset
PrefabCatalog.asset
```

The **PrefabRegistrySettings asset must be placed inside a `Resources` folder** so it can be loaded automatically at runtime.

Example structure:

```
Assets
 ├─ Resources
 │   └─ PrefabRegistrySettings.asset
 │
 └─ PrefabCatalog.asset (Folder path does not matter)
```

The `PrefabRegistrySettings` asset contains a reference to the `PrefabCatalog`.  
If not done automatically, assign the created catalog to the **Catalog** field in the inspector.

Once this reference is assigned, the registry will automatically initialize at runtime.

---

# Catalog Setup

Open `PrefabCatalog.asset` and register prefabs.

Each prefab entry consists of:

```
GroupId
Key
Prefab
```

Example:

```
Enemies
    EnemyKey_1 -> EnemyPrefab_1
    EnemyKey_2 -> EnemyPrefab_2

VFX
    MyVfxKey -> MyVfxPrefab
```

Rules:

- `GroupId` must be unique
- `Key` must be unique within the group
- `Prefab` references must not be null

---

# API Usage

Consumers should **only interact with the registry through the PrefabRegistry API**.

No direct catalog access is required.

---

## Instantiate Using String Literals

If the prefab cannot be fetched, the method will return `false`.

```
if (PrefabRegistry.TryInstantiate("MyGroupIdName", "EnemyKeyName", parent, out GameObject instance))
{
    // Success
}
```

---

## Instantiate Using Enums

Enums help avoid magic strings but it will **throw** if the prefab cannot be fetched.

```
public enum EnemyType
{
    Type_1,
    Type_2
}
```

Usage:

```
GameObject enemyObj = PrefabRegistry.InstantiateEnum("Enemies", EnemyType.Type_1);
```

The enum value is automatically converted to its string name.

---

# Summary

1. Register prefabs in `PrefabCatalog`.
2. Access them through `PrefabRegistry` by consuming `PrefabRegistryAPI.cs`.
3. Instantiate using **Group + Key** or **Enums**.

This keeps prefab access centralized and easy to maintain.