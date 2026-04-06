# real-time-messaging-app

> **Work in progress:** this project is still being built, and a few important pieces are clearly mid-flight.

## What this project does

This repo is aiming to become a real-time messaging platform with a Vue frontend and an ASP.NET Core backend. The overall direction looks like private and group chat with JWT-based auth, live messaging over SignalR, Kafka in the message pipeline, PostgreSQL for persistence, and object storage for media uploads.

From the code that is here today, the intended flow seems to be:

1. A user creates an account and logs in.
2. The backend issues an access token and refresh token.
3. The frontend loads chat summaries and message history.
4. Users join chat rooms through SignalR and exchange messages in real time.
5. Messages are pushed into Kafka for downstream processing.
6. Attachments are uploaded through pre-signed object storage URLs.

## Technical pieces that are already implemented

### Backend

- **ASP.NET Core Web API** in `src/backend/realTimeMessagingWebApp`
- **Swagger** enabled in development
- **JWT authentication** with refresh-token support
- **SignalR chat hub** for joining chats, requesting recent history, sending messages, and requesting pre-signed upload URLs
- **Kafka producer** wired into the message send path
- **EF Core + PostgreSQL** persistence with migrations already checked in
- **Cloudflare R2 / S3-compatible storage service** for generating client upload and download URLs

The current API surface already includes routes for:

- creating accounts
- logging in
- refreshing access tokens
- creating chats
- adding or removing chat members
- changing chat admins
- deleting chats
- fetching chat summaries
- fetching recent messages
- creating, accepting, and declining friendship requests

### Frontend

- **Vue 3 + Vite + TypeScript**
- **Pinia** for state management
- **Vue Router** with login, create-account, and chat routes
- **HTTP request layer** for auth and chat endpoints
- **SignalR client scaffolding**
- a **basic login screen** that is connected to the auth store

### Data and infrastructure

- A PostgreSQL-backed domain model for:
  - users
  - chats
  - chat memberships
  - messages
  - message attachments
  - friendships
  - refresh tokens
- A local **Docker Compose** setup in `infra/docker-compose.yml` for:
  - Kafka broker
  - Kafka UI
- A **Bruno collection** in `realTimeMessagingWebAppBruno/` for manual API testing

## What still looks unfinished

Based on the current code, these are the main gaps that still seem to need work:

- The **Kafka consumer** exists as a project, but it is still basically a stub and does not appear to consume and persist messages yet.
- The main **chat UI** is not built out yet. `ChatLayout.vue` is still effectively a placeholder, and the chat components are empty right now.
- The **create-account page** is still placeholder UI and is not wired into the backend flow yet.
- **Paginated chat history** looks partially started on the backend and frontend, but not finished end to end.
- **Frontend auth/session flow** still needs polish, especially around access-token handling, re-auth, and redirect behavior when auth fails.
- The backend has support for **pre-signed upload URLs**, but the frontend media upload experience does not look finished yet.
- Some **frontend/backend route wiring** still looks out of sync, which suggests a few flows are mid-integration.
- **CORS and local dev wiring** still look pretty development-only and likely need cleanup before this feels smooth to run.
- There are no obvious **automated tests** in the repo yet.

## Repo layout

```text
infra/                                   Kafka + Kafka UI local setup
realTimeMessagingWebAppBruno/            Bruno API request collection
src/frontend/                            Vue frontend
src/backend/realTimeMessagingWebApp/     ASP.NET Core API + SignalR hub
src/backend/KafkaConsumer/               background worker for Kafka consumption
src/backend/realTimeMessagingWebAppInfra/ persistence + storage infrastructure
```

## Running it locally

This repo does not look fully turnkey yet, but the current setup suggests this general workflow:

1. Start Kafka and Kafka UI from `infra/docker-compose.yml`.
2. Provide backend configuration for:
   - `ConnectionStrings:DefaultConnection`
   - `Jwt:JwtOptions`
   - `Jwt:JwtCreationOptions`
   - `KafkaConfigurations`
   - `R2`
3. Run the API in `src/backend/realTimeMessagingWebApp` on `http://localhost:5231`.
4. Run the worker in `src/backend/KafkaConsumer`.
5. In `src/frontend`, set `VITE_API_URL` to the backend API base (likely `http://localhost:5231/api`) and start the Vite app on port `5173`.

The frontend SignalR connection also still looks like it may need some extra local-dev proxy or origin configuration before that part feels smooth.

## Current takeaway

The project already has a strong technical backbone: auth, chat domain modeling, SignalR, Kafka, PostgreSQL, and object storage are all present. The biggest next step is turning those backend foundations into a complete end-to-end messaging experience by finishing the consumer, wiring the frontend properly, and closing the gaps around chat UI, account creation, and message persistence.
