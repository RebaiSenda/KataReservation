# KataReservation API

This project is a reservation system API built with .NET, providing REST endpoints to manage persons, rooms, and bookings.

## API Endpoints

### Persons

- **GET /api/persons** => Fetch all persons
- **GET /api/persons/{id}** => Fetch a person by id
- **POST /api/persons** => Create a new person
- **PUT /api/persons/{id}** => Update a person
- **DELETE /api/persons/{id}** => Delete a person by id

### Rooms

- **GET /api/rooms** => Fetch all rooms (requires authentication)
- **GET /api/rooms/{id}** => Fetch a room by id
- **POST /api/rooms** => Create a new room
- **PUT /api/rooms/{id}** => Update a room
- **DELETE /api/rooms/{id}** => Delete a room by id

### Bookings

- **POST /api/bookings** => Create a new booking
- **DELETE /api/bookings/{id}** => Delete a booking by id

## Authentication

This API is secured by JWT authentication. The rooms listing endpoint requires authentication.

## Error Handling

The API includes robust error handling:

- **400 Bad Request** - Invalid input data
- **404 Not Found** - Resource not found
- **409 Conflict** - Booking conflicts with existing reservations
- **500 Internal Server Error** - Unexpected server errors

## Booking System

The booking system allows users to:
- Create reservations for rooms with specific time slots
- Check for conflicts with existing bookings
- Get information about available time slots when conflicts occur

## Data Models

### Person
- ID
- First Name
- Last Name

### Room
- ID
- Room Name

### Booking
- ID
- Room ID
- Person ID
- Booking Date
- Start Slot
- End Slot

## Technologies

- .NET
- ASP.NET Core
- In-memory storage for data persistence

## Getting Started

1. Clone the repository
2. Open the solution in Visual Studio or your preferred IDE
3. Run the project
4. Access the API at `https://localhost:5001/api`

## Authentication

To access the protected endpoints, you need to obtain a JWT token.