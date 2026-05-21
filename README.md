# ⚔️ HeroEngine

**HeroEngine** is a web application built with ASP.NET Core Razor Pages (.NET 10) that simulates a fantasy RPG hero management system. It allows you to create, manage and battle heroes from the kingdom of **Bytecroft** against the Bug Primordial's forces.

---

## Project Structure

The solution is divided into two projects:

```
HeroEngine/
├── HeroEngine.Core/          # Class library — domain logic, models, data access
│   ├── Abilities/            # Ability system (IAbility, BaseAbility, concrete abilities)
│   ├── Combat/               # Combat engine (enemies, ICombatant, CombatHelper)
│   ├── Data/                 # EF Core context, repositories, sync service, config
│   ├── DTOs/                 # Data Transfer Objects (HeroDto, AbilityDto, etc.)
│   ├── Migrations/           # EF Core database migrations
│   └── Models/               # Domain models (Hero, Warrior, Mage, Rogue, HeroEntity…)
│
└── HeroEngine.Web/           # ASP.NET Core Razor Pages web application
    ├── Pages/
    │   ├── Combat/           # Combat simulator page
    │   ├── Files/            # File management (JSON/CSV/XML)
    │   ├── Heroes/           # Hero CRUD (list, create, edit, detail, delete)
    │   ├── HeroClasses/      # Hero class browser
    │   ├── Stats/            # Statistics and analytics
    │   └── Index             # Dashboard (counts, recent heroes, sync buttons)
    ├── Data/
    │   ├── heroes.json       # Persistent hero store (file-based)
    │   ├── combat_stats.csv  # Combat history log
    │   └── game_config.xml   # Runtime game configuration
    └── wwwroot/              # Static assets (Bootstrap, jQuery, custom CSS/JS)
```

---

## Features

### Hero Management
- Full **CRUD** for heroes stored in SQL Server via Entity Framework Core.
- Three playable **hero classes**: Warrior, Mage and Rogue — each with unique stats and scaling.
- **Level-based stat scaling**: HP, attack and armor grow with each level.
- Assign **abilities** to heroes with four rarity tiers: Common, Rare, Epic and Legendary.

### Ability System
| Ability | Type | Rarity | Power |
|---|---|---|---|
| Thunder Smash | Attack | Legendary | 95 |
| Iron Fortress | Defense | Epic | 65 |
| Shadow Strike | Attack | Epic | 65 |
| Second Wind | Healing | Rare | 40 |
| Arcane Barrier | Defense | Rare | 40 |
| War Taunt | Support | Common | 20 |

### Combat Simulator
- Turn-based battles with initiative ordering (heroes and enemies act by speed).
- Three enemy types: **Minion** (40 HP, fast), **Elite** (100 HP, balanced), **Boss** (250 HP, devastating).
- Post-battle **statistics** report (total damage, MVP hero, fastest kill).
- Combat results logged to `combat_stats.csv`.

### Data Layer — Dual Persistence
The application maintains two parallel stores that can be **synchronised** from the dashboard:

| Direction | Action |
|---|---|
| DB → Files | Export all heroes from SQL Server to `heroes.json` |
| Files → DB | Import heroes from `heroes.json` into SQL Server (upsert by name) |

### Statistics & Analytics
- Filter heroes by class, minimum level or name search.
- Top-N heroes by level.
- Heroes grouped by class and abilities grouped by rarity (LINQ aggregations).

### Game Configuration
Runtime parameters are loaded from `game_config.xml` at startup and can be modified without recompiling:

| Parameter | Default |
|---|---|
| `LevelMultiplier` | 1.15 |
| `CriticalHitChance` | 20% |
| `MaxCombatRounds` | 20 |
| `MaxHeroesPerBattle` | 4 |

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 10 — Razor Pages |
| ORM | Entity Framework Core 10 |
| Database | SQL Server (LocalDB / SQL Express) |
| File persistence | JSON (`System.Text.Json`), CSV, XML (`System.Xml.Linq`) |
| Frontend | Bootstrap 5, jQuery, jQuery Validation |
| Target runtime | .NET 10 |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server Express (or LocalDB)
- Visual Studio 2022+ or VS Code with the C# extension

### 1. Clone the repository

```bash
git clone https://github.com/your-username/HeroEngine.git
cd HeroEngine
```

### 2. Configure the connection string

Edit `HeroEngine.Web/appsettings.json` and set your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=HeroEngine;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Apply database migrations

```bash
cd HeroEngine.Web
dotnet ef database update --project ../HeroEngine.Core
```

### 4. Run the application

```bash
dotnet run
```

The app will be available at `https://localhost:5001` (or the port shown in the terminal). On first launch it automatically seeds the database with the initial hero classes (Warrior, Mage, Rogue) and two sample heroes.

---

## Data Files

The application reads and writes three files stored in `HeroEngine.Web/Data/` at runtime:

| File | Format | Purpose |
|---|---|---|
| `heroes.json` | JSON | Flat hero store for file-based persistence |
| `combat_stats.csv` | CSV | Append-only log of combat results |
| `game_config.xml` | XML | Configurable game parameters |

These files are created automatically if they do not exist.

---

## Database Schema

```
HeroClass (Id, Name, Description)
    └── HeroEntity (Id, Name, Level, MaxHp, Armor, Biography, HeroClassId)
            └── Ability (Id, Name, Type, Rarity, Cost, HeroId)
```

- Deleting a `HeroClass` is **blocked** if heroes of that class exist (restrict).
- Deleting a `HeroEntity` **cascades** to its abilities.

---

## Domain Model

```
Hero (abstract)
├── Warrior  — high HP & armor, physical attacker, has BattleCry
├── Mage     — low HP, high magic damage, mana system
└── Rogue    — fast initiative, backstab, evasion mechanics

Enemy (abstract) : ICombatant
├── Minion   — 40 HP, initiative 8, 5–11 dmg
├── Elite    — 100 HP, initiative 5, 18–29 dmg
└── Boss     — 250 HP, initiative 3, 35–54 dmg (Corrupt Memory)
```

---

Miquel Manzano - DAMv2
