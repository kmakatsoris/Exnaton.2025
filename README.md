Exnaton 2025 - README

![logo](https://github.com/user-attachments/assets/698b375b-db74-45fb-819e-5631cad89ed4)

# Overview
Welcome to the Exnaton 2025! This project provides a monitoring system tracking data from mu and utilizating a webapi offer options for storing data, data analysis and other.

Key Features:
- Web API: Retrieve data, store them and analyse them.
- Database Integration: Uses MySQL for data storage.
- K8 and Docker Integration: It allows seamless containerization for both development and production environments, making the system scalable, maintainable, and extremely convenient for various use cases, such as switching between different databases, launching and stopping the system without reconfiguration, and managing everything with a single command to launch the desired environment and reconfigure it from a single source
- Swagger UI: Connection with the web api via a friendly developer user interface.
- Serilog & Seq UI: Provides the capability to monitor information from the system throughout a friendly and easy to use for various of actors user interface. 

The single/ultimate/main purpose of that project is to showcase essential skills required from the **Exnaton company**.

# Setup
To get started with this project, ensure that your environment meets the prerequisites listed below.

## Prerequisites
Before running the project, ensure you have the following tools installed:

1. Docker: For containerizing the application.
   1.1. Docker Installation,
   - If you select to install it to **Debian/Ubutnu OS** refer to the following source: https://docs.docker.com/engine/install/ubuntu/
   - If you select to install it to **Windows OS** refer to the following source: https://docs.docker.com/desktop/setup/install/windows-install/
   1.2. Docker Compose Installation,
   - **Widnows/MacOS/Linux**: https://medium.com/@piyushkashyap045/comprehensive-guide-installing-docker-and-docker-compose-on-windows-linux-and-macos-a022cf82ac0b

2. [Optional]: .NET SDK 8: To build and run the .NET-based application.
   Is Required only if you select the option of running the application in Local/Debug/Development stage.
   - From **Linux/MacOS/Windows** refer to the following source: https://dotnet.microsoft.com/en-us/download/dotnet/8.0

## Installation & Run Steps
- 1. Clone the Repository: Start by downloading the repository to your local machine. (Or The project would be attached to a mail, take the latest version of it based on the date candidate(kostas makatsoris) has sent to)
- 2. Navigate to project's directory and lets refer to that as directoryA
- 3. There you could find a folder with name Docker; Open that on another tab for your convenience; Inside that directory lets refer to that as directoryB, execute the following commands.
   - 3.1.a Update the webapi appsetting.json -> environment as "Development" and then "docker-compose -f docker-compose.dev.yml up --build -d", to launch the docker infrastracture as development mode; That means it will **start only the mysql database and the seq monitoring tool**
   - 3.1.b Navigate to the directoryA and execute: "dotnet restore && dotnet build && dotnet run" (**Or if you are using an IDE just "Rebuild and Run the WebAPI"**).
   - 3.2. Update the webapi appsetting.json -> environment as "Docker-Development" and then "docker-compose -f docker-compose-dev.yml -f docker-compose-prod.yml up --build -d", to launch the docker infrastracture as production mode; That means it will **start all the services: mysql db, seq, webapi**

![Screenshot from 2025-03-23 19-51-10](https://github.com/user-attachments/assets/0e36b529-3173-43eb-9011-86e1a4b7a275)
![Screenshot from 2025-03-23 19-51-20](https://github.com/user-attachments/assets/5f8738c4-68f7-4336-a686-489b9ec3dbb8)



### Suggestion:
Launch the system with the method 3.2. because it is one command execution: "docker-compose -f docker-compose-dev.yml -f docker-compose-prod.yml up --build -d". But to see the test suits launch with 3.1 latter. Incl: unit (it is more like unit and integration testing) & integration (it is more like system testing) testing.

## Accessing the Application
Once the containers are running, you should be able to interact with the Web API at:

- Localhost (Development) via Swagger: http://localhost:5142/swagger/index.html
- Monitoring Tool (Seq): http://localhost:8081/ 

# Points of Interest
## Web API Structure
The Web API is organized into several key components to maintain clarity and modularity. Below is an overview of the structure:

- Controllers: The entry points of the API, where HTTP requests are received and processed.

- Exceptions: Custom exception handling that improves error reporting and system stability; Scalable and maintanable for security or other handling processes flexible.

- Implementations: Contains the core business logic that handles the requests. **Goal:**(Independent with the other system, serving the necessities of the Controller and UI as a consequence)

- Interfaces: Defines contracts for services used within the API, helping in maintaining abstraction. **Goal:**(Independent with the other system, serving the necessities of the Controller and UI as a consequence)

- Models: Defines the data structures used in the API, such as request and response objects.

- Utils: Utility classes that provide helper functions to aid in common tasks across the application.

- Repositories: Services to access and expose specified functionalities to the rest of the system which enable integration with the db **Goal:** (Independent from the rest implementation, serving essential and available db functionalities)

Here is a visual representation of how the Web API is structured:

<add-screenshot-from-figma>.png

# Points/Regions of interest (POI or ROI)
- The database and the code is implemented and configured with that way to allow high speed on reading/updating and writing data to the database because it is essential from the nature of the frequent data we have to habdle!
  
# Other Information
Author: Kostas Makatsoris
License: Kostas Makatsoris
Date: March 2025

# Project File Structure
Here is an overview of the project’s file structure:

![Screenshot from 2025-04-02 14-31-00](https://github.com/user-attachments/assets/d34b95b9-cacf-47fb-9128-595b92cb0741)




