# SMS Rate Limiter Microservice

## Overview
The **SMS Rate Limiter Microservice** is a .NET Core application designed to manage and enforce rate limits for sending SMS messages. It ensures that messages are sent without exceeding the provider's global and per-business number limits. The service integrates with **MongoDB** for persistent storage and **Memcached** for in-memory rate limiting. It also provides a web interface built with **Angular** for monitoring and filtering historical data.

## Features

### Rate Limiting:
- Enforces global and per-business number rate limits.
- Uses **Memcached** for real-time rate limiting.

### Persistent Storage:
- Stores accounts, business numbers, and rate limit history in **MongoDB**.
- Supports filtering historical data by account, business number, and date range.

### Web Interface:
- Built with **Angular** for real-time monitoring.
- Displays global and per-business number rates.
- Allows filtering by account, business number, and date range.

### Scalability:
- Designed to handle high volumes of requests.
- Uses **MongoDB** for scalable and persistent storage.
- Uses **Memcached** for distributed in-memory rate limiting.

## Architecture

### Components

#### .NET Core Microservice:
- **RateLimitService**: Manages rate limiting logic using Memcached.
- **MongoService**: Handles interactions with MongoDB.
- **API Controllers**: Expose endpoints for rate limiting and monitoring.

#### Angular Web Interface:
- Displays real-time and historical data.
- Provides filtering and monitoring capabilities.

#### MongoDB:
- Stores accounts, business numbers, and rate limit history.

#### Memcached:
- Provides in-memory caching for real-time rate limiting.

## Database Design

### Collections

#### Accounts:
Stores account-level information, including global rate limits.

**Fields:**
- **Id (ObjectId)**: Unique identifier.
- **AccountName (string)**: Name of the account.
- **GlobalLimit (int)**: Global rate limit per second.

#### BusinessNumbers:
Stores business number-level information, including per-second rate limits.

**Fields:**
- **Id (ObjectId)**: Unique identifier.
- **AccountId (string)**: Reference to the account.
- **PhoneNumber (string)**: Business phone number.
- **PerSecondLimit (int)**: Rate limit per second for the business number.

#### RateLimitHistory:
Stores historical data for rate limit checks.

**Fields:**
- **Id (ObjectId)**: Unique identifier.
- **AccountId (string)**: Reference to the account.
- **BusinessPhone (string)**: Business phone number.
- **Timestamp (DateTime)**: Timestamp of the rate limit check.
- **Allowed (bool)**: Whether the message was allowed.

## API Endpoints

### Rate Limiting
#### **GET /api/rate/can-send**
Checks if a message can be sent without exceeding rate limits.

**Query Parameters:**
- `accountId (string)`: The account ID.
- `businessPhone (string)`: The business phone number.

**Response:**
```json
{
  "allowed": true
}
```

### Monitoring
#### **GET /api/monitoring/accounts**
Retrieves a list of all accounts.

**Response:**
```json
[
  {
    "Id": "account-123",
    "AccountName": "Primary Account",
    "GlobalLimit": 1000
  }
]
```

#### **GET /api/monitoring/business-numbers**
Retrieves a list of business numbers for a specific account.

**Query Parameters:**
- `accountId (string)`: The account ID.

**Response:**
```json
[
  {
    "Id": "business-123",
    "AccountId": "account-123",
    "PhoneNumber": "+1234567890",
    "PerSecondLimit": 100
  }
]
```

#### **GET /api/monitoring/history**
Retrieves historical rate limit data.

**Query Parameters:**
- `accountId (string, optional)`: The account ID.
- `businessPhone (string, optional)`: The business phone number.
- `start (DateTime)`: Start of the date range.
- `end (DateTime)`: End of the date range.

**Response:**
```json
[
  {
    "Id": "history-123",
    "AccountId": "account-123",
    "BusinessPhone": "+1234567890",
    "Timestamp": "2023-10-01T12:00:00Z",
    "Allowed": true
  }
]
```

