# Dungeons.Dragons.Game.API

An API for the game **Dungeons & Dragons**. 
This project allows you to manage characters, initiate a game run, explore environments, encounter enemies, choose to flee or abort, and much more.

---

## Features

- Create, update, and delete characters  
- Initiate a “run” (game session)  
- Explore the game world  
- Encounter enemies, battle or flee from them  
- Abort a run and handle end-game scenarios  
- Modular, layered architecture — DTOs, repository, services, game logic  
- Unit tests covering core logic  

---

## Architecture 

- **Language & Platform:** C# / .NET (the repository is 100% C#) :contentReference[oaicite:1]{index=1}  
- **Layers / Projects:**  
  - DTO layer (for data transfer objects)  
  - Repository layer (data access)  
  - Service layer (business logic)  
  - GameData (sample data for testing)  
  - Unit test project


### Prerequisites

- .NET SDK 10 (version … specify exact version)   
- Postman or similar to test the API endpoints  

### Installation

1. Clone the repository  
   ```bash
   git clone https://github.com/khadija282/Dungeons.Dragons.Game.API.git
