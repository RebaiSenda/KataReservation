# KataReservation API

This project is a sample API REST in .NET 9.

There are 4 endpoints :
- **GET /api/values** => Fetch all values
- **POST /api/values** => Create a value
- **GET /api/values/{id}** => Fetch a value by id
- **PUT /api/values/{id}** => Update a value
- **DELETE /api/values/{id}** => Delete a value by id

The data are persisted in a very simple in-memory storage.

This API is secured by JWT authentication.

To have an access token, you have to use authority : [Sample.IdentityProvider](https://github.com/kevsofr/Sample.IdentityProvider).
