# Ride Matching System

A simple backend system for matching riders with nearby available drivers using ASP.NET Core Web API.

## Tech Stack

* ASP.NET Core Web API (.NET 10)
* C#
* Entity Framework Core
* SQL Server / LocalDB
* SignalR
* Background Worker (`BackgroundService`)
* IMemoryCache
* Swagger / OpenAPI
* Docker

## Features

### Rider Management

* Create a rider
* Request a ride with pickup and drop locations
* Check ride status
* Complete a ride

### Driver Management

* Create a driver
* Set driver online/offline
* Update driver location
* Track driver availability

### Ride Matching

The system automatically matches a requested ride with the nearest available online driver.

A background worker checks pending rides periodically and assigns an available driver based on location.

### Real-Time Updates

SignalR is used to notify the assigned driver when a ride is matched.

SignalR Hub:

```text
/rideHub
```

Drivers can join their own SignalR group using:

```text
JoinDriver(driverId)
```

The server sends:

```text
RideAssigned
```

when a ride is assigned.

### Caching

`IMemoryCache` is used to cache the list of online drivers for a short period.

The cache is invalidated when a driver is assigned to a ride.

For a distributed production environment, this can be replaced with Redis or another distributed cache.

### Background Processing

A `BackgroundService` runs every few seconds and processes rides with:

```text
Status = Requested
```

This keeps ride matching asynchronous instead of requiring the client to perform the matching operation.

### Basic Concurrency Handling

Ride matching uses a database transaction for the manual matching flow and updates the ride and driver state together.

The driver is marked unavailable after being assigned to a ride.

For a multi-instance production system, optimistic concurrency, distributed locking, or a queue-based architecture could be added.

## API Endpoints

### Drivers

```text
POST /api/Drivers
POST /api/Drivers/{id}/online
POST /api/Drivers/{id}/location
POST /api/Drivers/{id}/offline
```

### Riders

```text
POST /api/Riders
```

### Rides

```text
POST /api/Rides
POST /api/Rides/{id}/match
GET  /api/Rides/{id}
POST /api/Rides/{id}/complete
```

## Example Ride Request

```json
{
  "riderId": 1,
  "pickupLatitude": 12.9716,
  "pickupLongitude": 80.2209,
  "dropLatitude": 12.9800,
  "dropLongitude": 80.2300
}
```

## How Ride Matching Works

```text
Rider requests ride
        |
        v
Ride stored with "Requested" status
        |
        v
Background Worker checks pending rides
        |
        v
Find online drivers
        |
        v
Calculate nearest driver
        |
        v
Assign driver
        |
        v
Driver marked unavailable
        |
        v
SignalR notification sent
        |
        v
Ride status = DriverAssigned
```

## Project Structure

```text
RideMatchingSystem
│
├── Controllers
│   ├── DriversController.cs
│   ├── RidersController.cs
│   └── RidesController.cs
│
├── Data
│   └── AppDbContext.cs
│
├── Hubs
│   └── RideHub.cs
│
├── Models
│   ├── Driver.cs
│   ├── Rider.cs
│   └── Ride.cs
│
├── Services
│   ├── RideMatchingService.cs
│   └── RideMatchingWorker.cs
│
├── Migrations
├── Program.cs
├── Dockerfile
└── appsettings.json
```

## Database

The application uses SQL Server LocalDB by default.

Database:

```text
RideMatchingDb
```

Entity Framework Core migrations are included in the repository.

## Running the Application

### 1. Clone the repository

```bash
git clone https://github.com/sowmya-kumar-dev/RideMatchingSystem.git
```

### 2. Open the solution

Open:

```text
RideMatchingSystem.slnx
```

### 3. Apply the database migration

Using Package Manager Console:

```powershell
Update-Database
```

### 4. Run the application

Using Visual Studio or:

```bash
dotnet run
```

### 5. Open Swagger

```text
https://localhost:7207/swagger
```

The exact port may differ depending on the local launch settings.

## Scalability Considerations

For a production-scale ride matching system, the following improvements could be introduced:

* Redis for distributed caching
* Message queue such as Azure Service Bus or RabbitMQ
* Multiple background worker instances
* Distributed locking or optimistic concurrency
* Load balancer for multiple API instances
* Geospatial indexing/service for efficient nearby-driver searches
* Separate services for driver location tracking and ride matching
* Monitoring, logging and distributed tracing

## Assumptions

* A driver can handle one active ride at a time.
* Only online drivers are considered for matching.
* A driver becomes unavailable after being assigned a ride.
* LocalDB is used for demonstration purposes.
* Authentication and authorization are outside the scope of this assignment.
* The matching calculation uses a simple coordinate-distance calculation for demonstration.

## Author

Sowmya K
