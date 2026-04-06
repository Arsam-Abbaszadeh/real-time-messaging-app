# real-time-messaging-app

> **Work in progress:** this repository is actively being built.

## What this project is

This repository contains a real-time messaging application. It is split into a Vue frontend, an ASP.NET Core backend, a Kafka-based message pipeline, and shared infrastructure for persistence and file storage.

The goal of the project is to support authenticated chat between users, including chat creation, member management, live message delivery, message history, friendship requests, and media uploads.

## How the system works

1. A user signs up and logs in through the web app.
2. The backend issues a JWT access token and a refresh token.
3. The frontend loads the user's chats and message history through the API.
4. The frontend connects to the SignalR chat hub for live updates.
5. New messages are broadcast to connected clients in real time.
6. Message events are published to Kafka for background processing.
7. Media uploads use pre-signed object storage URLs instead of sending large files directly through the API.

## What is implemented today

### Frontend

The frontend lives in `src/frontend` and currently uses:

- **Vue 3**
- **Vite**
- **TypeScript**
- **Pinia** for application state
- **Vue Router** for navigation
- **SignalR client code** for real-time communication
- **SCSS** for styling

Current frontend structure includes:

- a login route and login page
- a create-account route
- a chat route
- API request helpers for auth and chat requests
- store modules for auth and chat state

### Backend API

The main backend lives in `src/backend/realTimeMessagingWebApp` and currently provides:

- **ASP.NET Core Web API**
- **Swagger** in development
- **JWT bearer authentication**
- **refresh-token support**
- **SignalR** for live chat events
- **Kafka producer integration**

Current backend endpoints support:

- account creation
- user login
- access-token refresh
- chat creation
- adding chat members
- removing chat members
- changing chat admins
- deleting chats
- fetching chat summaries
- fetching chat messages
- creating friendship requests
- accepting friendship requests
- declining friendship requests

The SignalR hub currently handles:

- joining a chat room
- leaving a chat room
- requesting recent chat history
- sending chat messages
- requesting pre-signed upload URLs for chat media

### Persistence and storage

The shared infrastructure project lives in `src/backend/realTimeMessagingWebAppInfra`.

It currently includes:

- **EF Core**
- **PostgreSQL** support through **Npgsql**
- database migrations
- an object storage service using an **S3-compatible API**
- configuration for **Cloudflare R2**

The current data model includes:

- users
- chats
- chat memberships
- messages
- message attachments
- friendships
- refresh tokens

### Messaging infrastructure

The messaging pipeline currently includes:

- a Kafka producer in the main API
- a worker project in `src/backend/KafkaConsumer`
- Docker Compose setup for **Kafka** and **Kafka UI** in `infra/docker-compose.yml`

### API tooling

The repository also includes a **Bruno** collection in `realTimeMessagingWebAppBruno/` for manual API testing.

## What still needs to be built

The next major pieces of work are:

- finish the create-account flow in the frontend
- build out the chat layout and chat components
- complete end-to-end paginated message history
- complete the Kafka consumer so message events can be processed and persisted through the background worker
- finish the frontend flow for media uploads
- tighten session refresh and auth failure handling in the frontend
- improve local development wiring for frontend-to-backend and SignalR communication
- add automated tests

## Repository structure

```text
infra/                                     local Kafka and Kafka UI setup
realTimeMessagingWebAppBruno/              Bruno API request collection
src/frontend/                              Vue frontend
src/backend/realTimeMessagingWebApp/       ASP.NET Core API and SignalR hub
src/backend/KafkaConsumer/                 background worker for Kafka consumption
src/backend/realTimeMessagingWebAppInfra/  database and storage infrastructure
```

## Running locally

### Services

Start the local Kafka services from `infra/docker-compose.yml`.

### Backend configuration

The backend expects configuration for:

- `ConnectionStrings:DefaultConnection`
- `Jwt:JwtOptions`
- `Jwt:JwtCreationOptions`
- `KafkaConfigurations`
- `R2`

### Backend apps

Run:

- the API from `src/backend/realTimeMessagingWebApp`
- the worker from `src/backend/KafkaConsumer`

The API development profile uses `http://localhost:5231`.

### Frontend

In `src/frontend`, set `VITE_API_URL` to the backend API base URL and run the Vite app on port `5173`.

## Summary

This repository is the foundation for a full real-time messaging system. The backend architecture, data model, authentication flow, message pipeline, and storage integrations are already in place. The remaining work is focused on completing the user-facing chat experience and finishing the background processing path.
