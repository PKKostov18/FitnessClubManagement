# ⚔️ NEXUS COMBAT | Enterprise Training Systems

<p align="center">
  <img src="https://img.shields.io/badge/.NET_8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8.0" />
  <img src="https://img.shields.io/badge/MS_SQL_Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="SQL Server" />
  <img src="https://img.shields.io/badge/EF_Core_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="EF Core" />
  <img src="https://img.shields.io/badge/Bootstrap_5.3-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white" alt="Bootstrap 5" />
  <img src="https://img.shields.io/badge/Theme-Modern_Brutalist-DA1212?style=for-the-badge" alt="Brutalist Theme" />
</p>

---

### ⚡ THE ARCHITECTURE OF INTENSITY
**Nexus Combat** is an advanced, production-grade multi-role training coordination platform architected specifically for high-intensity martial arts gyms and elite athletic academies. Diverging from standard monolithic gym templates, Nexus Combat deploys a high-contrast **Modern Brutalist Design Philosophy**—utilizing flat surfaces, absolute pitch-black backgrounds (`#070707`), sharp geometric edges, and aggressive crimson accents (`#DA1212`) to structurally mirror the raw discipline, intensity, and precision of combat sports.

---

## 🛠️ CORE SYSTEM ARCHITECTURE

```text
                      ┌──────────────────────────────────┐
                      │      NEXUS COMBAT PLATFORM       │
                      └────────────────┬─────────────────┘
                                       │
                ┌──────────────────────┼──────────────────────┐
                ▼                      ▼                      ▼
   ┌────────────────────────┐ ┌────────────────────────┐ ┌────────────────────────┐
   │     FIGHTER ECOSYSTEM  │ │    TRAINER COMMAND     │ │     SYSTEM ADMIN       │
   ├────────────────────────┤ ├────────────────────────┤ ├────────────────────────┤
   │ 🔒 Volatile Sessions   │ │ 📊 Real-time Rosters   │ │ 👥 Roster Overviews    │
   │ 📅 Roster Capacity UI  │ │ ⏳ Live Chrono Filters │ │ 🏢 Global Infrastructure│
   │ 🛑 Direct Drop-Spot    │ │ 🆕 Session Deployment  │ │ 🔑 Claim Escalations   │
   └────────────────────────┘ └────────────────────────┘ └────────────────────────┘
```

---

## 💎 PLATFORM CAPABILITIES & FEATURES

### 1. Multi-Tier Role Infrastructure & Claims Management
The system enforces strict RBAC (Role-Based Access Control) using a highly secure ASP.NET Core Claims Identity configuration. Users are securely segmented into three strategic roles:
* **System Administrators:** Complete operational authority over the infrastructure, user directories, and system-wide roster monitoring.
* **Trainers (HQ):** Full management over their independent training calendars, live deployment of conditioning blocks, and absolute visibility into upcoming fighter manifests.
* **Fighters (Users):** Independent scheduling portals, real-time booking history, and active session management.

### 2. High-Velocity Booking & Live Capacity Constraints
* Direct integration with the persistence layer ensures zero concurrency friction or overbooking vulnerabilities.
* Live capacity nodes continuously evaluate current vs. max roster spots (e.g., `1 / 6 Fight Capacity`).
* Dynamic front-end blocks automatically transition active components into disabled states (`ALREADY BOOKED` or `MAX CAPACITY`) based on real-time user-to-session relationships.

### 3. Volatile Session Security Layer
* Traditional hard-drive persistent tracking cookies were completely purged from the identity pipeline.
* Configured ephemeral **Session Cookies (`IsPersistent = false`)** operating inside the browser’s volatile memory space.
* Enforced a strict server-side **30-minute inactivity sliding expiration window** paired with absolute cryptographic destruction upon total browser or tab group closure.

### 4. Global Structural Refactoring & Flexbox Anchor
* Unified the entire site's presentation layer by implementing a global C# layout matrix.
* Built layout fallbacks directly into `site.css` to seamlessly translate outdated layout frameworks into sharp, aggressive Brutalist containers without structural regression.
* Configured a viewport-aware Flexbox alignment engine that reliably anchors the platform's minimalist footer to the absolute base of any screen resolution.

---

## 🎨 DESIGN SPECIFICATION Matrix

| Attribute | Technical Hex / Rule | UX Purpose |
| :--- | :--- | :--- |
| **Primary Canvas** | `#070707` (Pitch Black) | Absorbs visual glare, enforces deep dark-mode aesthetic. |
| **Surface Containers** | `#121212` (Dark Carbon) | Breaks flat space; elevates interactive panels. |
| **Accent Shock** | `#DA1212` (Aggressive Crimson) | Drives attention to active calls to action, states, and errors. |
| **Typography Geometry**| `Inter` / Sans-Serif | High readability, sharp numeric spacing, zero decorative clutter. |
| **Edge Treatment** | `border-radius: 0px !important` | Brutalist industrialism; communicates raw discipline. |
| **Background Texture** | `55px x 55px` Custom Linear Grid | Emulates tactical geometry and cage chain-links. |

---

## 💻 TECH STACK

* **Backend Engine:** .NET 8.0 (C#) ASP.NET Core MVC
* **Persistence Layer:** Entity Framework Core 8
* **Database Engine:** Microsoft SQL Server (Relational Engine)
* **Cryptographic Layer:** SHA-256 Custom Password Hashing Engine
* **Front-End Matrix:** Razor Architecture (`.cshtml`), Bootstrap 5 CSS Framework, Compiled Brutalist Custom CSS Engine

---

## ⚡ QUICKSTART DEPLOYMENT GUIDE

### Prerequisites
Ensure your workstation has the following enterprise packages installed:
* [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* [Microsoft SQL Server LocalDB / Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
* Visual Studio 2022 (Enterprise/Professional/Community) or JetBrains Rider

### Installation Blueprint

1. **Clone the platform repository:**
   ```bash
   git clone https://github.com/your-username/nexus-combat.git
   cd nexus-combat
   ```

2. **Configure Local SQL Persistence Pipeline:**
   Navigate to the root directory, open `appsettings.json`, and supply your local server signature within the database connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_LOCAL_SQLEXPRESS;Database=NexusCombatDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   }
   ```

3. **Execute Structural Migrations:**
   Compile the data access layer and map the schema into your local SQL Server instance using the Package Manager Console:
   ```powershell
   Update-Database
   ```
   *Or via the command-line interface:*
   ```bash
   dotnet ef database update
   ```

4. **Initialize Sockets & Bootstrap Engine:**
   ```bash
   dotnet build
   dotnet run
   ```
   The application will mount and stream logs locally on `https://localhost:7084` (or your assigned active development port).

---

## 🔑 PRE-SEEDED ENCRYPTED LAB CREDENTIALS

Use these cryptographically pre-seeded identities to transition across the role architecture instantly during evaluation:

| Identity Tier | Username / Access Code | Secure Passphrase | Active Operational Dashboard |
| :--- | :--- | :--- | :--- |
| **System Administrator** | `admin@nexus.fit` | `Admin123!` | `/Admin/Index` |
| **Elite Trainer** | `ivan@gmail.com` | `Trainer123!` | `/Trainer/Index` |
| **Registered Fighter** | `test@gmail.com` | `User123!` | `/User/Dashboard` |

---

## 📈 ROADMAP & CURRENT COMPONENT TELEMETRY
- [x] **RBAC Claims Identity Authentication Pipeline**
- [x] **Volatile Non-Persistent Session Security Engine**
- [x] **Roster Capacity Logic & Auto-Dismission Scripts**
- [x] **Trainer HQ Workouts Portal & Real-time Live Manifests**
- [x] **Global Layout Architecture Refactoring**
- [ ] **P2P Advanced Sparring Matchmaking Matrix** *(Scheduled for development)*

---

```text
⚡ DISCIPLINE IS NOT A LIMIT. IT IS AN ENGINE. ⚡
```
