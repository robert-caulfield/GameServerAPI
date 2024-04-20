# GameServerAPI

This RESTful API is designed to facilitate a multiplayer gaming environment. It enables player authentication and access to game server information. It provides a platform for game servers to register, manage their information, and authenticate players.

## Key Features

### Authentication and Authorization:
- Incorporates Microsoft Identity for user authentication, providing a secure environment for users and their account information.
- Authenticated users are provided a JSON Web Token (JWT) for future API requests, ensuring secure and stateless authentication.
- Role-based authorization to ensure secure access control. The roles include: ‘Admin’, 'Server', and ‘Player’. 
- Utilizes token-based interaction between players and game servers, allowing game servers to authenticate players who join without exposing sensitive player account information.

### Dedicated Servers Management:
- Provides endpoints for game servers to update themselves through their life cycle. This includes server registration, status management, player validation, and deregistration.
- Players are able to access information on registered game servers.
- A background service that removes game servers if they fail to send a heartbeat to the API within the specified time period.

### Database and In-Memory Storage:
The API employs a hybrid data storage approach to efficiently handle both persistent and volatile game server and authentication information.

#### Database Storage
- **Authentication Information**: Utilizes Microsoft Identity to store user authentication data, including login credentials and roles.
- **Persistent Game Server Data**: Stores essential information such as IP, port, server name, and player capacity.
- **Entity Framework Core**: Manages all interactions with the database, including queries and updates, using Entity Framework Core.

#### In-Memory Storage
- **Game Server Data**: Maintains data subject to frequent updates, such as player counts, in a thread-safe dictionary. This approach minimizes the load on the database.

## API Controllers

The API is documented using Swagger. You can view a static page of the documentation [here]

- **Auth Controller**: Manages user authentication and registration, issuing JWTS for authenticated sessions.
- **Game Server Controller**: Manages game server lifecycle operations including registration, status updates, and validation of PlayerJoinTokens.
- **Server Browser Controller**: Allows players to view active game servers and their details.
- **Player Token Controller**: Issues PlayerJoinTokens that are used to authenticate players to game servers.

## Database Schema

<p align="center">
  <img src="https://github.com/robert-caulfield/GameServerAPI/assets/113054389/f8d02860-00f0-4bfd-8a6f-456cb1ab9db4"/>
</p>

*Note: For simplicity, this diagram omits certain less relevant tables and attributes.*

## Role Descriptions and Permissions

- **Admin**
  - Can create servers.
  - Can modify any server, regardless of creator.
- **Server**
  - Can create servers.
  - Can modify servers that are created by their own account.
- **Player**
  - Can request player join tokens.
  - Cannot create servers.

## Implementation and Usage

The API is designed to be versatile, suitable for use in any game development environment. A detailed implementation within the Godot engine is available here: [GameServer](https://github.com/robert-caulfield/GameServerAPI-ServerGodot) [GameClient](https://github.com/robert-caulfield/GameServerAPI-ClientGodot).

## Installation Guide

### Prerequisites

Before you begin the installation, ensure you have the following software installed:
- **.NET SDK 8**: Required to build and run the .NET application.
- **Visual Studio 2019 or later**: Recommended IDE for developing .NET applications. You can download it from [Visual Studio Official Site](https://visualstudio.microsoft.com/downloads/).
- **SQL Server**: Necessary for database operations. You can use any version compatible with .NET 8.
- **SQL Server Management Studio (SSMS)**: Optional but recommended for managing your SQL database visually. Download from [SSMS Official Site](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms).

## Step-by-Step Installation

<details>
<summary><strong>Installation Guide without Visual Studio</strong></summary>
<p>

#### 1. Clone the Repository
Clone the repository containing the ASP.NET project using the following command in your terminal:
```bash
git clone https://github.com/robert-caulfield/GameServerAPI
cd [Project Folder]
```
#### 2. Restore the Project Dependencies
Navigate to the project directory where the `*.csproj` file is located and run:
```bash
dotnet restore
```
This command restores all the project dependencies.
#### 3. Set Up the Database Connection String:
- Open the [`appsettings.json`]("GameServerAPI/appsettings.json") file in the project.
- Modify the `ConnectionStrings` section with your SQL Server details if neccessary, this is the default:
```
"ConnectionStrings": {
  "DefaultConnection": "Server=(LocalDb)\\MSSQLLocalDB;Database=GameServerAPI;TrustServerCertificate=True;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```
#### 4. Install Entity Framework CLI
If you haven't already installed the Entity Framework CLI, install it globally using the following command:
```bash
dotnet tool install --global dotnet-ef
```
#### 5. Run Entity Framework Migrations
Apply the migrations to your database to create the necessary schema:
```bash
dotnet ef database update
```
#### 6. Build and Run the Application
Build the project using:
```bash
dotnet build
```
Run the application with:
```bash
dotnet run
```
#### 7. Access Swagger UI:
- The swagger UI can be accessed in your browser at `https://localhost:port/swagger` where `port` is the port number specified in [`Properties/launchSettings.json`](GameServerAPI/Properties/launchSettings.json) (7242 by default).
- You can interact with the API directly from the Swagger interface to test different endpoints.
</p>
</details>

<details>
<summary><strong>Installation Guide with Visual Studio</strong></summary>
<p>

#### 1. Open the Project:
- Open Visual Studio.
- Select 'Open a project or solution' from the start window, or go to `File -> Open -> Project/Solution` from the menu.
- Navigate to the folder where you cloned your repository and open the solution file (.sln).
#### 2. Restore NuGet Packages:
Right-click on the solution in the Solution Explorer and select 'Restore NuGet Packages'. This will ensure all dependencies are correctly installed based on the project configuration.
#### 3. Set Up the Database Connection String:
- Open the [`appsettings.json`]("GameServerAPI/appsettings.json") file in the project.
- Modify the `ConnectionStrings` section with your SQL Server details if neccessary, this is the default:
```
"ConnectionStrings": {
  "DefaultConnection": "Server=(LocalDb)\\MSSQLLocalDB;Database=GameServerAPI;TrustServerCertificate=True;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```
#### 4. Apply Migrations:
- Open the Package Manager Console by going to `Tools -> NuGet Package Manager -> Package Manager Console`.
- Ensure the Default project is set to the project containing your Entity Framework context.
- Run the command:
```
Update-Database
```
This command applies any pending migrations to the SQL database, creating it if it does not already exist.
#### 5. Build and Run the Project:
- Build the project by selecting `Build -> Build Solution`.
- Run the project by pressing `F5` or clicking the 'Start Debugging' button. This will launch the API in your default browser with the Swagger UI loaded.
#### 6. Access Swagger UI:
- The Swagger UI should open automatically in your browser at `https://localhost:port/swagger` where `port` is the port number specified in [`Properties/launchSettings.json`](GameServerAPI/Properties/launchSettings.json) (7242 by default).
- You can interact with the API directly from the Swagger interface to test different endpoints.
</p>
</details>

<details>
<summary><strong>Post-Installation</strong></summary>
<p>

### 1. Update Secret Keys
- Navigate to [`appsettings.json`]("GameServerAPI/appsettings.json")
- Replace secret key values with a new strong secret keys
Example:
```js
"ApiSettings": {
     "Secret": "NewStrongSecretKeyHere1234567890", // Secret key used for user authentication JWTs
     "PlayerJoinTokenSecret": "AnotherStrongSecretKeyHere987654321" // Secret key used for PlayerJoinToken JWTs
}
```
*Note: In a production environment these secret keys would be stored in a secure storage solution like Azure Key Vault.*

### 2. Configure Game Server Manager Settings
- Navigate to [`appsettings.json`]("GameServerAPI/appsettings.json")
- Configure `GameServerManagerSettings` to the needs of your project:
```js
  // Used to populate GameServerManagerSettings
  "GameServerManagerSettings": {
    "HeartbeatEnabled": true, // If true, enables background cleanup service
    "HeartbeatTimeout": 90, // Time in seconds before a game server is considered inactive and removed.
    "HeartbeatCheckInterval": 30 // Time interval in seconds that the background service cleans up inactive servers
  }
```
</p>
</details>


## Technologies Used
- ASP.NET 8
- Microsoft Identity
- Microsoft Entity Framework
- SQL Server
