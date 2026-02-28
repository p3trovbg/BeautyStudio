# Appointment Management System

A full-stack appointment management platform built with **ASP.NET Core 8**, **React 18 + TypeScript**, **PostgreSQL 16**, and deployed via **Docker Compose** with **Ansible** provisioning.

## Features

- **Owners** manage their availability and services
- **Customers** book, view, and cancel appointments
- **Overlap prevention** — application-level check + PostgreSQL GiST exclusion constraint
- **Email notifications** — booking confirmation, cancellation, 24h reminders (MailKit + Hangfire)
- **Calendar view** — React Big Calendar with status-colored events
- **JWT authentication** with Owner/Customer role-based authorization

## Architecture

```
src/
├── backend/          ASP.NET Core 8 — Clean Architecture
│   ├── Domain/       Entities, ValueObjects, Enums, Exceptions (feature folders)
│   ├── Application/  Interfaces, Services, DTOs, Validators
│   ├── Infrastructure/  EF Core, MailKit, Hangfire, Configurations
│   └── WebApi/       Controllers, Middleware, Program.cs
├── frontend/         React 18 + TypeScript — Feature Folder Structure
│   ├── features/     Appointments, Owners, Customers
│   └── shared/       Axios, Layout, Auth, Utilities
docker/               Dockerfiles + Nginx config
ansible/              Server provisioning playbooks
```

## Quick Start — Local Development

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- [PostgreSQL 16](https://www.postgresql.org/) (or use Docker)

### 1. Clone & Setup

```bash
git clone <your-repo-url> && cd appointment-system
cp .env.example .env
# Edit .env with your local database and SMTP credentials
```

### 2. Backend

```bash
cd src/backend
dotnet restore
dotnet build
dotnet run --project src/WebApi
# API runs at http://localhost:5000
# Swagger at http://localhost:5000/swagger
```

### 3. Frontend

```bash
cd src/frontend
npm install
npm run dev
# App runs at http://localhost:3000
```

### 4. Run Tests

```bash
cd src/backend
dotnet test   # Runs xUnit overlap detection tests
```

## Docker — Full Stack

```bash
cp .env.example .env
# Edit .env with your values

docker compose up -d --build
# Frontend:  http://localhost:3000
# API:       http://localhost:5000
# Postgres:  localhost:5432
# Hangfire:  http://localhost:3000/hangfire
```

### Useful Commands

```bash
docker compose logs -f api         # Tail API logs
docker compose down                # Stop all services
docker compose down -v             # Stop + remove volumes (⚠️ destroys data)
docker compose up -d --build api   # Rebuild only the API
```

## Ansible — Server Deployment

### Prerequisites

- Target: Ubuntu 22.04 server with SSH access
- Controller: Ansible 2.14+ installed locally

### Deploy

```bash
cd ansible

# Edit inventory and group_vars
vim inventory/hosts.yml
vim group_vars/all.yml

# Run the full playbook
ansible-playbook -i inventory/hosts.yml site.yml
```

### Roles

| Role | Description |
|------|-------------|
| `common` | apt update, install utilities, configure UFW |
| `docker` | Install Docker CE + Compose plugin, log rotation |
| `app_user` | Create deploy user, add to docker group |
| `deploy` | git pull, copy .env, docker compose up |
| `ssl` | Certbot + Let's Encrypt + auto-renewal |

## EF Core Migrations

```bash
cd src/backend

# Create a new migration
dotnet ef migrations add <MigrationName> \
  --project src/Infrastructure \
  --startup-project src/WebApi

# Apply migrations
dotnet ef database update \
  --project src/Infrastructure \
  --startup-project src/WebApi
```

> **Note:** The GiST exclusion constraint for overlap detection is applied via a custom migration in `Infrastructure/Persistence/Migrations/`.

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/appointments` | List appointments (paged) |
| GET | `/api/appointments/{id}` | Get appointment by ID |
| POST | `/api/appointments` | Create appointment |
| PUT | `/api/appointments/{id}` | Update appointment |
| DELETE | `/api/appointments/{id}` | Cancel appointment |
| GET | `/api/appointments/check-overlap` | Check time slot availability |
| GET/POST/PUT/DELETE | `/api/owners` | Owner CRUD |
| GET/POST/PUT/DELETE | `/api/customers` | Customer CRUD |
| GET | `/health` | Health check |

## Terraform Pipeline (GitHub Actions)

Infrastructure is managed by Terraform and deployed via GitHub Actions. The workflow lives in `.github/workflows/terraform.yml`.

### How it works

| Event | What happens |
|-------|-------------|
| Open / update a **Pull Request** | `terraform plan` runs automatically and posts the output as a PR comment |
| Click **"Run workflow"** in the Actions tab | `terraform apply` (or `destroy`) runs — you choose the action in a dropdown |

> **Nothing applies automatically on merge.** You always click the button intentionally.

### One-time Setup

#### 1. Terraform Cloud (free)

1. Sign up at [app.terraform.io](https://app.terraform.io)
2. Create an organization — set the name in `terraform/backend.tf` and in the `env:` block of `terraform.yml` (replace `"your-org"`)
3. Create a workspace called **`beauty-studio`** → set **Execution Mode** to **Local** (the CI runner executes Terraform, not Terraform Cloud)
4. Generate an API token: **User Settings → Tokens → Create API token**

#### 2. GitHub Secrets

Go to **Settings → Secrets and variables → Actions** in your GitHub repo and add:

| Secret name | Value |
|-------------|-------|
| `TF_API_TOKEN` | The Terraform Cloud API token from step 1 |
| `KUBECONFIG` | The full contents of your `~/.kube/config` file |

#### 3. GitHub Environment (optional but recommended)

Create a **`production`** environment in **Settings → Environments**. You can add required reviewers there so that even the manual apply needs someone to approve before it runs.

### Triggering Apply

1. Go to **Actions** tab → **Terraform** workflow
2. Click **"Run workflow"** (top-right)
3. Choose branch (`main`) and action (`apply`)
4. Click **"Run workflow"**

### Running Locally

```bash
cd terraform
terraform init
terraform plan
terraform apply
```

---

## Tech Stack

**Backend:** ASP.NET Core 8 · EF Core 8 · PostgreSQL 16 · Hangfire · MailKit · Serilog · FluentValidation · AutoMapper

**Frontend:** React 18 · TypeScript · Vite · TanStack Query v5 · Zustand · React Hook Form + Zod · React Big Calendar · Tailwind CSS · Sonner

**DevOps:** Docker Compose · Nginx · Ansible · Let's Encrypt · Terraform · GitHub Actions

## License

MIT
