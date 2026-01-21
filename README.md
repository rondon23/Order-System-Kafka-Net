# 📦 Kafka + .NET – Event-Driven Microservices Portfolio

This repository contains a **distributed, event-driven system** built with **.NET 8** and **Apache Kafka** as the messaging platform.

The goal of this project is **hands-on learning** and **professional portfolio building**, showcasing real-world concepts used in modern high-scale systems such as **event-driven architecture**, **microservices**, **asynchronous messaging**, **resilience**, and **observability**.

---

## 🎯 Project Goals

* Demonstrate practical usage of **Kafka with .NET**
* Implement **asynchronous communication between services**
* Apply **distributed systems architecture concepts**
* Simulate real-world problems (failures, retries, duplicates)
* Serve as both a **learning lab** and a **professional showcase**

---

## 🧠 Concepts Covered

* Event-Driven Architecture (EDA)
* Producer / Consumer pattern
* Consumer Groups
* *At-least-once delivery* guarantee
* Idempotency
* Retry with backoff
* Dead Letter Queue (DLQ)
* Event versioning
* Separation of concerns
* Dockerized local environment

---

## 🧱 High-Level Architecture

```
┌──────────────┐
│  Order API   │  (.NET 8 – Producer)
└──────┬───────┘
       │ Event: OrderCreated
       ▼
┌──────────────┐
│   Kafka      │
└──────┬───────┘
       ▼
┌──────────────┐
│ Payment      │  (.NET Worker – Consumer)
└──────┬───────┘
       │ Event: PaymentProcessed
       ▼
┌──────────────┐
│ Notification │  (.NET Worker – Consumer)
└──────────────┘
```

Each service is **independent** and communicates exclusively through events published to Kafka.

---

## 🧩 System Services

### 1️⃣ Order.API (Producer)

**Responsibilities:**

* Receive orders via HTTP
* Persist order data
* Publish the `OrderCreated` event

**Technologies:**

* ASP.NET Core Web API
* .NET 8
* Confluent.Kafka

---

### 2️⃣ Payment.Worker (Consumer)

**Responsibilities:**

* Consume `OrderCreated` events
* Process payments (simulated)
* Publish `PaymentProcessed` events
* Implement retry and idempotency

**Technologies:**

* .NET Worker Service
* Kafka Consumer Groups

---

### 3️⃣ Notification.Worker (Consumer)

**Responsibilities:**

* Consume `PaymentProcessed` events
* Simulate notification delivery (email / log)

---

## 📂 Repository Structure

```
kafka-dotnet-portfolio/
├─ README.md
├─ docker/
│  └─ docker-compose.yml
├─ src/
│  ├─ BuildingBlocks/
│  │  └─ EventBus
│  ├─ Order.API
│  ├─ Payment.Worker
│  └─ Notification.Worker
└─ docs/
   ├─ architecture.md
   ├─ kafka-topics.md
   └─ decisions.md
```

---

## 🐳 Docker Infrastructure

The local environment uses **Docker Compose** to start:

* Zookeeper
* Kafka Broker

### Start the environment:

```bash
docker-compose up -d
```

---

## 📬 Kafka Topics

| Topic                 | Description                     |
| --------------------- | ------------------------------- |
| `order-created`       | Order creation event            |
| `payment-processed`   | Payment successfully processed  |
| `order-created-retry` | Retry topic for failed messages |
| `order-created-dlq`   | Dead Letter Queue               |

---

## 🔁 Retry and DLQ Strategy

* Temporary failures → message forwarded to retry topic
* Permanent failures → message forwarded to DLQ
* Idempotent processing to avoid duplicated side effects

---

## 🔐 Delivery Guarantees

* **At-least-once delivery**
* Manual offset commits
* Idempotency based on `EventId`

---

## ▶️ Running Locally

1. Start Kafka:

```bash
docker-compose up -d
```

2. Run the services:

```bash
dotnet run --project src/Order.API
dotnet run --project src/Payment.Worker
dotnet run --project src/Notification.Worker
```

3. Send an order request via API:

```http
POST /orders
```

---

## 🧪 Testing & Planned Improvements

* [ ] Kafka Testcontainers
* [ ] Outbox Pattern
* [ ] Avro + Schema Registry
* [ ] Observability (structured logging)
* [ ] Distributed tracing

---

## 📌 Why Kafka?

Kafka is widely adopted in systems that require:

* High throughput
* Scalability
* Asynchronous processing
* Loosely coupled communication

This project simulates common scenarios found in **fintech, banking, and e-commerce platforms**.

---

## 👨‍💻 Author

**Bruno Rondon da Silva**
Software Developer (.NET)

📍 Brazil / Australia (exchange program)
🎯 Focused on backend, distributed systems, and software architecture

---

## ⭐ Final Notes

This project is not just a technical example, but a **continuous learning lab**, evolving as new concepts are studied and applied.

Feedback and suggestions are very welcome 🚀

