## About the project
This is a practice-oriented backend service for a software company that sells products **through upfront contracts** and **subscriptions**.  
The system manages clients (individuals/companies), products and versions, discounts, contracts and payments, subscriptions and renewals — and based on this data calculates both **actual** and **predicted** revenue.

The project is designed to demonstrate a well-thought-out domain model, strict business rules, and a clean data model with constraints (unique PESEL/KRS, prevention of “double ownership” of a product, etc.).

## 🧩 Domain Overview
**Clients**
- Individual: firstName, lastName, address, email, phone, **PESEL** (immutable), soft-delete.
- Company: companyName, address, email, phone, **KRS** (immutable), not deletable.

**Products & Discounts**
- Software: name, description, currentVersion, category.
- Discount: name, scope (upfront/subscription), value%, activeFrom–activeTo. Apply best available; add loyal 5% if eligible.

**Contracts & Payments (Upfront)**
- Contract: productId, clientId, version, startDate–endDate (validity **3–30 days**), basePrice, discountsApplied, supportYears (0–3), finalPrice.
- Payments: multiple allowed; **sum must equal** finalPrice; reject/return late payments. When fully paid within window → contract **signed** and **recognized as revenue**.

**Subscriptions (Optional)**
- SubscriptionOffer: productId, name, renewalPeriod (1 month – 2 years), price.
- Subscription: clientId, productId, startedAt; first period **paid at registration**; renewals must match period amount and timing; apply loyal discount; cancel if unpaid.

**Revenue**
- Calculate **current** revenue from completed payments/contracts.
- Calculate **predicted** revenue assuming renewals continue and unsigned contracts will be signed (configurable).

---

## 🔗 REST API

### Clients (`/api/clients`)
- `POST /api/clients`
- `GET /api/clients/{id}`
- `PUT /api/clients/{id}`
- `DELETE /api/clients/{id}`

### Contracts (`/api/contracts`)
- `POST /api/contracts`
- `GET /api/contracts/{id}`
- `GET /api/contracts/client/{clientId}`
- `POST /api/contracts/{id}/payments` *(body: decimal `amount`)*
- `GET /api/contracts/{id}/isRevenueRecognized`

### Revenue (`/api/revenue`)
- `GET /api/revenue/actual?from=YYYY-MM-DD&to=YYYY-MM-DD`
- `GET /api/revenue/actual/product/{softwareId}?from=YYYY-MM-DD&to=YYYY-MM-DD`
- `GET /api/revenue/predicted`
- `GET /api/revenue/predicted/product/{softwareId}`
- `GET /api/revenue/convert?amountPln=DECIMAL&currency=PLN|USD|...`

### Subscriptions (`/api/subscriptions`)
- `POST /api/subscriptions`
- `POST /api/subscriptions/{id}/renew?amount=DECIMAL`
- `POST /api/subscriptions/{id}/cancel`
- `GET /api/subscriptions/client/{clientId}`
- `GET /api/subscriptions/{id}/next-renewal`
- `GET /api/subscriptions/revenue/current`
- `GET /api/subscriptions/revenue/predicted`

---
Key constraints & indexes: unique PESEL/KRS; prevent double-active product ownership; composite indexes for revenue queries.
## 🔐 Security
- **Employees only.** Authentication (login/password) with **roles**:
  - **admin:** edit/remove clients;
  - **user:** other operations (contracts, payments, revenue, etc.).

---

## 🧪 Business Rules (highlights)
- Loyal client = has at least one past contract or subscription → **+5% discount** (stacks with best other discount).
- Choose **highest** eligible discount in time range.
- Contract cannot be modified; only removed (business cancel). Late payments must be **returned** and not counted as revenue.
- Subscriptions: reject duplicate payments for current period; **amount must match** period price; cancel if unpaid at period start.

---
## 🗺️ Roadmap
- Currency conversion via public FX API with caching.
- Subscription module (full).
- Admin UI for client/product/discount management.
- Advanced revenue dashboards (filters, product/company views).
---
## 🧰 Tech Stack
- **Backend:** .NET 9, ASP.NET Core Web API, EF Core
- **Database:** PostgreSQL (preferred),SqlLite, MsSQL
- **Docs:** Swagger / Swashbuckle
- **DevOps:** EF Core Migrations
- **IDE:** Rider
