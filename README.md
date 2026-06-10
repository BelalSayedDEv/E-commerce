# 🛒 E-Commerce API

A professional **ASP.NET Core 8 Web API** for an e-commerce platform with full authentication, shopping cart, order management, role-based authorization, structured logging, and global exception handling.

Built as a learning project to master backend engineering concepts — from clean architecture to production-ready practices.

---

## 🚀 Features

- **🔐 Authentication & Authorization**
  - JWT-based login/registration
  - Role-based access (Admin, User)
  - Secure token generation with configurable expiration

- **📦 Product & Category Management**
  - Full CRUD operations
  - Stock tracking
  - Admin-only product/category modification

- **🛒 Shopping Cart**
  - Add/remove items with quantity
  - Auto-calculated totals
  - Cart auto-creation on user registration

- **📋 Order Management**
  - Place orders with stock validation
  - Order history per user
  - Admin status updates (Pending → Shipped → Delivered)
  - Automatic stock reduction & cart clearing on checkout

- **📝 Structured Logging**
  - `ILogger<T>` across all services
  - Proper log levels: Trace → Debug → Information → Warning → Error → Critical
  - Context-rich error messages
  - Startup health check on database connectivity

- **🛡️ Global Exception Middleware**
  - Centralized error handling
  - Consistent JSON error responses
  - No stack trace leakage to clients
  - Covers all unhandled exceptions

- **🌐 CORS Enabled**
  - Cross-origin support for frontend integration

- **📊 Swagger Documentation**
  - Interactive API testing UI
  - Auto-generated endpoint documentation

---

## 🏗️ Architecture & Design

```
┌─────────────────────────────────────────────┐
│              Controllers (API)              │
├─────────────────────────────────────────────┤
│               Services (Logic)              │
├─────────────────────────────────────────────┤
│  Repositories (Data Access / EF Core)       │
├─────────────────────────────────────────────┤
│           Database (SQL Server)             │
└─────────────────────────────────────────────┘
```

- **Controller → Service → Repository** layered architecture
- **DTOs** for all API communication (never expose entities directly)
- **Dependency Injection** throughout
- **Async/Await** for all database operations
- **Repository Pattern** for clean data access abstraction

---

## 🧰 Tech Stack

| Technology | Purpose |
|---|---|
| ASP.NET Core 8 | Web API framework |
| Entity Framework Core | ORM / Data access |
| SQL Server (LocalDB) | Database |
| ASP.NET Core Identity | Authentication & user management |
| JWT Bearer Tokens | Stateless authentication |
| Swagger / Swashbuckle | API documentation |
| `ILogger<T>` | Structured logging |
| LINQ | Querying |

---

## 📋 API Endpoints

### Account
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/Account/Register` | ❌ | Register new user |
| POST | `/api/Account/Login` | ❌ | Login & receive JWT |
| POST | `/api/Account/AdminRegister` | ❌ | Register admin user |

### Products
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/Product` | ❌ | Get all products |
| GET | `/api/Product/{id}` | ❌ | Get product by ID |
| POST | `/api/Product` | Admin | Add product |
| PUT | `/api/Product/{id}` | Admin | Update product |
| DELETE | `/api/Product/{id}` | Admin | Delete product |

### Categories
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/Category` | ❌ | Get all categories |
| GET | `/api/Category/{id}` | ❌ | Get category by ID |
| POST | `/api/Category` | Admin | Add category |
| PUT | `/api/Category/{id}` | Admin | Update category |
| DELETE | `/api/Category/{id}` | Admin | Delete category |

### Cart
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/Cart` | ✅ | Get user's cart |
| POST | `/api/Cart` | ✅ | Add item to cart |
| DELETE | `/api/Cart/{itemId}` | ✅ | Remove item from cart |

### Orders
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/Order/checkout` | ✅ | Place order |
| GET | `/api/Order/History` | ✅ | Get order history |
| PATCH | `/api/Order/{id}/Status` | Admin | Update order status |

### Roles
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/Role/AddRole` | ❌ | Create new role |

---

## 🧠 What I Learned

This project was built progressively, with each cycle introducing a new concept. Here's what was covered:

### ✅ Completed Learning Cycles

| Cycle | Concept | What I Built / Learned |
|---|---|---|
| 1.1 | Service Layer | Extracted business logic from controllers into services |
| 1.2 | Category Service | Full CRUD with layered architecture |
| 1.3 | Async/Await | Converted sync methods to async for scalability |
| 2.1 | Authentication | ASP.NET Identity + JWT token generation |
| 2.2 | JWT Configuration | Claim handling, token expiration, signing keys |
| 2.3 | Role-Based Auth | Admin/User roles with `[Authorize(Roles = "...")]` |
| 3.1 | Cart Models | Cart + CartItem with FK relationships |
| 3.2 | Cart CRUD | Add/remove items, auto-create on registration |
| 3.3 | Cart Totals | Dynamic price calculation |
| 4.1 | Order Models | Order + OrderItem entities |
| 4.2 | Place Order | Stock validation, reduction, cart clearing |
| 4.3 | Order History | User-specific order listing |
| 4.4 | Status Flow | Admin status updates with PATCH |
| 6.1 | Global Exception Middleware | Centralized error handling |
| 6.2 | Structured Logging | ILogger, log levels, error context |
| 6.3 | API Response Format | Generic `ApiResponse<T>` wrapper |
| — | Git & GitHub | Repository setup, commits, push |

### 🐛 Mistakes & Debugging Wins

| Mistake | How I Fixed It | What I Learned |
|---|---|---|
| `cart.TotalPrice` always `0` in DB | Calculate totals dynamically from cart items | Never trust a stored calculated field |
| FK constraint error on `OrderItem.OrderId` = `0` | Use navigation property `Order = order` instead of setting FK directly | EF Core relationship fixup |
| Missing `Id = p.Id` in DTO mapping | Added mapping in `GetProductByIdAsync` | Always check every field in every DTO mapping |
| Spaces in JWT config keys (`"Jwt :Key"`) | Removed spaces | JSON keys are sensitive to whitespace |
| Short JWT secret (< 32 chars) | Extended to 32+ characters | Symmetric security key minimum length |
| Unique index on `ProductId` in `OrderItems` | Dropped index via SQL | Check migration constraints before running |
| `return null` outside `if` block (missing `{}`) | Added curly braces | C# without braces is dangerous |
| `Console.WriteLine` for logging | Replaced with `ILogger<T>` | Logging needs persistence and structure |
| Double `Save()` in `AddToCart` | Left as-is (design decision noted) | Atomic operations need single `SaveChanges()` |
| Not checking `null` on navigation properties | Added `?.` null-conditional operator | Defensive coding matters |

---

## 🔮 Future Roadmap

### Phase 5 — Advanced Querying
- Pagination for product listing
- Search & filter by name/category/price
- Sort by price, name, date

### Phase 7 — Cleanup
- Remove dead sync methods
- Fix nullable reference warnings

### Phase 8 — Testing & Deployment
- Unit tests with xUnit
- Integration tests
- Deployment configuration

### Planned Features (Coming Soon)
- **Admin Dashboard** — Manage products, orders, users from a dedicated UI
- **User Profiles** — View/edit personal information, order history
- **Product Comments** — User reviews and ratings on product pages
- **Transactions** — Atomic order processing with rollback support
- **Consistent API Response** — Full `ApiResponse<T>` integration across all endpoints

---

## 🛠️ Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 (recommended)

### Setup
```bash
# Clone the repository
git clone https://github.com/BelalSayedDEv/E-commerce.git
cd E-commerce

# Update the connection string in appsettings.json
# Restore dependencies
dotnet restore

# Apply migrations
dotnet ef database update

# Run the application
dotnet run
```

The API will be available at `http://localhost:5249` with Swagger UI at `/swagger`.

---

## 👨‍💻 About Me

**Belal Sayed** — Backend Developer (.NET)

I built this project to grow from a junior who knows fundamentals into a developer who thinks about architecture, edge cases, logging, security, and production readiness.

Every mistake here is a lesson learned. Every refactor is a skill gained. This is not a finished product — it's a living project that grows as I do.

---

## 📝 License

This project is for learning and portfolio purposes.
