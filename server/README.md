# Server

> This directory contains all the necessary files to run KeiPai backend

## Project structure

```
├───Background Services
├───bin
├───Controllers
├───Data
├───Dtos
│   ├───Account
│   ├───Campaign
│   ├───Game
│   ├───Key
│   ├───Message
│   ├───OtherMedia
│   ├───Request
│   ├───Review
│   ├───Tag
│   ├───Twitch
│   └───Youtube
├───Extensions
├───Initialization
├───Interfaces
├───Migrations
├───Models
├───obj
├───Properties
├───Repository
└───Services
```

`Background Services` - services that run in the background all the time and periodically check for updates (new YouTube videos, Twitch livestreams snapshots etc.)  

`Controllers` - responsible for responding to requests incoming from the client  

`Data` - stores `ApplicationDBContext.cs` which is responsible for managing the database context for the application  

`Dtos` - every *Data Transfer Object* used in the project  

`Extensions` - extension methods for some of the classes  

`Initialization` - methods that run on the empty database to prepare some necessary data (e.g. tags, platforms, sample users etc.)  

`Interfaces` - interfaces for all the `Repository` and `Service` classes  

`Migrations` - Entity Core Framework created migrations for the database

`Models` - all the database tables

`Repository` - classes that are focused on data access

`Services` - classes that are focused on communicating with external APIs (YouTube, Twitch etc.)  
