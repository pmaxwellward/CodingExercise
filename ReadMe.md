# How to Run (Visual Studio & CLI)

## Visual Studio (both projects as startup projects)

1. Open the solution in **Visual Studio 2022**.
2. Right-click the solution ➜ **Set Startup Projects…**  
   - Choose **Multiple startup projects**.  
   - Set **InvestmentPerformanceWebAPI.Server** ➜ **Start**.  
   - Set **InvestmentPerformanceWebAPI.Client** ➜ **Start**.
3. Press **F5** (or ▶️ Run).  
   - The API will run on `https://localhost:7065` (and `http://localhost:5140`).  
   - The Vue app (Vite) will run on its dev port and proxy `/api/*` to the API.

> Tip: If PowerShell blocks `npm`, run `Set-ExecutionPolicy -Scope CurrentUser RemoteSigned`, restart the terminal, then run `npm install`.

---

## Command Line (dotnet CLI + npm)

### 1) Start the API (pin ports)
```bash
cd InvestmentPerformanceWebAPI.Server
dotnet restore
dotnet dev-certs https --trust
dotnet run --urls "https://localhost:7065;http://localhost:5140"
```
### 2) In another terminal, start the dev server
```bash 
cd InvestmentPerformanceWebAPI.client
npm install
npm run dev
```

Visit the Vue dev server URL it prints (e.g., https://localhost:56256).
Calls to /api/* are proxied to https://localhost:7065.

### API Endpoints - (base localhost:56256)

| Method | Endpoint | Description |
|---------|-----------|-------------|
| **GET** | `/api/users` | Returns all users in the database. <br>*(Acceptable User IDs: 1 & 2)* |
| **GET** | `/api/users/{user.id}/investments` | Returns all investments for a user. <br>Each item includes **Investment ID** and **Name**. |
| **GET** | `/api/users/{user.id}/investments/{investment.id}` | Returns detailed investment information (shares, cost basis, current value, term, gain/loss). |


I used a Vue front end because it was mentioned as part of your common stack. I have also included global exception handling, logging, and unit tests.

P. Maxwell Ward - 2025