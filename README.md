# Kolmeo Coding Challenge
This is my work for Kolmeo's coding test.

## API Overview
This service exposes the following APIs
- GET /products - list all products
- GET /products/{id} - get a specific product
- POST /products - create a new product
- PUT /products/{id} - update a specific product
- DELETE / products/{id} - delete a specific product

### Example product JSON

```
{
    "id": 1,
    "name": "Test product",
    "description": "Description for test product",
    "price": 100.0
}
```


## Assumptions

- Integer is used for product ID.
- Product name must not be empty.
- Product price must not be negative.

## Requirements
- .NET Core 6

## How to run


### Build app
F5 in Visual Studio, or:
```
dotnet build
```

### Run app
F5 in Visual Studio, or
```
dotnet run --project src/KolmeoCodingTest.WebApi
```

### Run tests
Use Test Explorer in Visual Studio, or
```
dotnet test
```