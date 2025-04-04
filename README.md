Exnaton 2025 - README

![ai-logo-readmefile](https://github.com/user-attachments/assets/1406776c-da93-4593-a829-d9b5a79b66b5)
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

[Recommended]
I have created two files for installing all the dependencies and running the system for both Windows and Unix systems.
You can find it inside the directory: 
- Unix: "Exnaton.2025/Kubernetes/.setup.sh"
- Windows: "Exnaton.2025/Kubernetes/.setup.bat"
Otherwise...

1. Docker: For containerizing the application.
   1.1. Docker Installation,
   - If you select to install it to **Debian/Ubutnu OS** refer to the following source: https://docs.docker.com/engine/install/ubuntu/
   - If you select to install it to **Windows OS** refer to the following source: https://docs.docker.com/desktop/setup/install/windows-install/
   1.2. Docker Compose Installation,
   - **Widnows/MacOS/Linux**: https://medium.com/@piyushkashyap045/comprehensive-guide-installing-docker-and-docker-compose-on-windows-linux-and-macos-a022cf82ac0b
   1.3. Kubernetes installation:
   - Unix: https://kubernetes.io/docs/tasks/tools/install-kubectl-linux/
   - Windows: https://kubernetes.io/docs/tasks/tools/install-kubectl-windows/
   1.4. Minikube installation:
   - Unix: https://minikube.sigs.k8s.io/docs/start/?arch=%2Flinux%2Fx86-64%2Fstable%2Fbinary+download
   - Windows: https://minikube.sigs.k8s.io/docs/start/?arch=%2Flinux%2Fx86-64%2Fstable%2Fbinary+download

2. [Optional]: .NET SDK 8: To build and run the .NET-based application.
   Is Required only if you select the option of running the application in Local/Debug/Development stage.
   - From **Linux/MacOS/Windows** refer to the following source: https://dotnet.microsoft.com/en-us/download/dotnet/8.0
  
![Screenshot from 2025-04-04 02-47-12](https://github.com/user-attachments/assets/fe0fd042-fefe-4b1c-8d5b-b35595421bd4)


## Installation & Run Steps
[Recommended]
I have created two files for installing all the dependencies and running the system for both Windows and Unix systems.
You can find it inside the directory: 
- Unix: "Exnaton.2025/Kubernetes/.setup.sh"
- Windows: "Exnaton.2025/Kubernetes/.setup.bat"
Otherwise...

- 1. Clone the Repository: Start by downloading the repository to your local machine. (Or The project would be attached to a mail, take the latest version of it based on the date candidate(kostas makatsoris) has sent to)
- 2. Navigate to project's directory and lets refer to that as directoryA
- 3. There you could find a folder with name Kubernetes; Open that on another tab for your convenience;
  3.1. Then having the minikube cluster running and verify that the kubectl has the right context
  3.2. Apply all the yamls inside that Kubernetes directory. Exclude the ones inside "Future" and "Not Used".

![Screenshot from 2025-03-23 19-51-10](https://github.com/user-attachments/assets/0e36b529-3173-43eb-9011-86e1a4b7a275)
![Screenshot from 2025-03-23 19-51-20](https://github.com/user-attachments/assets/5f8738c4-68f7-4336-a686-489b9ec3dbb8)


## Accessing the Application
Once the containers are running, you should be able to interact with the Web API at:

- Localhost (Development) via Swagger: http://192.168.49.2:32566/index.html
- Monitoring Tool (Seq): http://192.168.49.2:31409

## Http calls further information
- POST http://localhost:5184/Analysis/measurements
{
"muid": "1db7649e-9342-4e04-97c7-f0ebb88ed1f8",
"measurement": "energy",
"limit": 100,
"start": "2022-12-01T23:45:00.000Z",
"stop": "2023-03-01T23:45:00.000Z"
}

Creates a PDF with the analysis of the data coming from database based on the POST body

- POST http://localhost:5184/meterdata/read
{
"muid": "95ce3367-cbce-4a4d-bbe3-da082831d7bd",
"measurement": "energy",
"limit": 100,
"start": "2023-02-10T23:45:00.000Z",
"stop": "2023-02-28T23:45:00.000Z"
}

Fetch the data asked from UI

- GET http://localhost:5184/meterdata/measurement?muid=95ce3367-cbce-4a4d-bbe3-da082831d7bd&measurement=energy&limit=100&start=2023-02-10T23%3A45%3A00.000Z&stop=2023-02-28T23%3A45%3A00.000Z'

Retrieving the data from the mu and store them in the database. As extra there is the option to display some of them as response.

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
- Bottleneck or points that are crucial and we should care about our implementation and system design is the database. We have not consider any back up mechanism (out of the scope of that assignment) but we have created with pagination the database to be
  easily migrated, scaling and indexing to find our data. The data are also time series so we need to have a series of them so the pagination should be based on that criteria and we have page the database based on month (week or year possible). To summarise the database migration and scaling and also the scaling of the service receiving the data from the MUs are the most vulnerable/fragile units.
- The main problems that may face in the future are that we need to group some sources of mu data based on the muid (like loadbalancin but with tenants) and with Ingress service to split the flow to seperate web api services. Also with the same way to seperate the databases and with paging based on Month or other date intervals.
- Other optimization is to perform database read and write strategies
- Ofcourse indexes but better to have one (to both of the three main criteria of the request) to not having write delays
- Autoscalling pods and database is also solution
- And with all of them is also easy to migrate or backup the databases

!More details and answers at the generated PDFs.
![Screenshot from 2025-04-04 03-17-17](https://github.com/user-attachments/assets/5ff1feff-760d-467f-87a2-9a37f0911586)

  
# Other Information
Author: Kostas Makatsoris
License: Kostas Makatsoris
Date: March 2025

# Project File Structure
Here is an overview of the project’s file structure:

![Screenshot from 2025-04-02 14-31-00](https://github.com/user-attachments/assets/d34b95b9-cacf-47fb-9128-595b92cb0741)
![Screenshot from 2025-04-04 02-20-42](https://github.com/user-attachments/assets/222105de-5ba8-4d85-9614-421f8ba87345)
![Screenshot from 2025-04-04 02-20-57](https://github.com/user-attachments/assets/7efc06f4-8108-4e7c-98af-04f2cf06d657)
![Screenshot from 2025-04-04 02-58-16](https://github.com/user-attachments/assets/98fb7f5a-29a7-42b8-b4dd-37e07abf38f8)



