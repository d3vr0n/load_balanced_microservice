# Microservices Project with load balance between a dotnet and go microservice

This project consists of two microservices, one written in .NET and the other in Go, managed using Docker Compose. An Nginx load balancer is used to route traffic between the services.

## Project Structure
```
└── 📁test-bluebeam
    └── 📁.vscode
        └── launch.json
    └── 📁dotnetmicroservice1
        └── 📁src
            └── 📁Properties
                └── launchSettings.json
            └── appsettings.Development.json
            └── appsettings.json
            └── Dockerfile
            └── dotnetmicroservice1.csproj
            └── Program.cs
    └── 📁golangmicroservice2
        └── Dockerfile
        └── go.mod
        └── main.go
    └── docker-compose.yml
    └── nginx.conf
    └── readme.md
```

## Code 
```csharp
// best way to get the intersection of two arrays
    var intersection1 = request.Array1.Intersect(request.Array2).ToArray();

    // deconstruct Intersect method to show how it works
    var set1 = new HashSet<int>(request.Array1); // creatinon of HashSet is O(n)
    var intersection2 = new HashSet<int>();
    foreach (var item in request.Array2)
    {
        if (set1.Contains(item)) // O(1) lookup using HashSet totally O(m) where m is the length of the second array
        {
            intersection2.Add(item); // total O(n+m)
        }
    }

    // crude way to get the intersection of two arrays
    var intersection3 = request.Array1.Where(x => request.Array2.Contains(x)).ToArray();

    // expanding where loop using for loops to get the intersection of two arrays
    var intersection4 = new List<int>();
    
    for (int i = 0; i < request.Array1.Length; i++)
    {
        for (int j = 0; j < request.Array2.Length; j++)
        {
            if (request.Array1[i] == request.Array2[j])
            {
                // Check if the element is not already in the intersection                
                bool found = false;
                for (int k = 0; k < intersection4.Count; k++)
                {
                    if (intersection4[k] == request.Array1[i])
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    intersection4.Add(request.Array1[i]);
                }
                break; // No need to continue inner loop once a match is found
            }
        }
    }
```

## Services

> Prerequisite - `dotnet 8`, `go 1.22` or/and `docker` installed on local system

### .NET Microservice

The .NET microservice is located in the [`dotnetmicroservice1`](.\dotnetmicroservice1) directory. It is a web application targeting .NET 8.0.

#### Build and Run

To build and run the .NET microservice, use the following commands:

```sh
cd dotnetmicroservice1/dotnetmicroservice1
dotnet build
dotnet run
```
Or click on VS code debugging
Open Postman - 
```
POST http://localhost:5012/intersection
```
payload - 
```json
{
  "array1": [1, 2, 3, 4, 1, 8, 9, 13],
  "array2": [3, 4, 5, 6, 6, 1, 12, 8]
}
```

Alternatively to run dotnet microservice in docker - 
```sh
docker build -t dotnetmicroservice1 .
docker run -d -p 5000:8080 --name dotnetmicroservice1 dotnetmicroservice1
curl -X POST -H "Content-Type: application/json" -d '{"array1": [1, 2, 3, 4], "array2": [3, 4, 5, 6]}' http://localhost:8080/intersection

```
### Go Microservice

The Go microservice is located in the [`golangmicroservice2`](.\golangmicroservice2) directory. It is a gin framework based API.

#### Build and Run

Prerequisite (to be run in go microservice folder)- 
```sh
go mod tidy
go mod download
```

```sh
cd golangmicroservice2
go run main.go
```
```
POST http://localhost:8081/intersection
```

Alternatively to run go microservice in docker - 
```sh
docker build -t intersection-app .
docker run -p 8080:8081 intersection-app
curl -X POST -H "Content-Type: application/json" -d '{"array1": [1, 2, 3, 4], "array2": [3, 4, 5, 6]}' http://localhost:8081/intersection

```

### Docker Compose
The docker-compose.yml file defines the services and their configurations.

Usage
To start all services using Docker Compose, run:
```sh
docker-compose up --build
```
Open Postman - 
```
POST http://localhost:5005/intersection
```
payload - 
```json
{
  "array1": [1, 2, 3, 4, 1, 8, 9, 13],
  "array2": [3, 4, 5, 6, 6, 1, 12, 8]
}
```
This will build and start the .NET and Go microservices, along with the Nginx load balancer.

### Configuration
- __dotnetmicroservice__: Defined in the dotnetmicroservice1 directory.
- __golangmicroservice__: Defined in the golangmicroservice2 directory.
- __loadbalancer__: Uses the nginx:latest image and is configured using the nginx.conf file. `Note: ` response structure has been intentionally kept separate to see load balance in action.

### Nginx Configuration
The Nginx configuration is defined in the nginx.conf file. It routes traffic to the appropriate microservice based on the request.

### Development
#### VS Code Tasks
VS Code tasks are defined in the .vscode/tasks.json file for building, publishing, and running the .NET microservice.

#### Debugging
Debug configurations can be added to the .vscode/launch.json file for debugging the microservices.

#### Todo
 - Add helm chart to deploy to local kube cluster. 
 - Use kubenetes default n/w load balancer (OSI L4) instead of nginx (OSI L7) 