# Sonar Backend - Project Architecture & Technology Overview

## 📋 Project Overview

**Sonar** is a comprehensive music streaming and social platform backend built with .NET 9.0. The system provides a full-featured API for music distribution, user management, social interactions, chat functionality, and content management. The platform supports artists, distributors, playlists, albums, tracks, user libraries, and various user experience features including achievements, subscriptions, and cosmetic items.

## 🏗️ Architecture

The project follows **Clean Architecture** principles with clear separation of concerns across multiple layers:

### Layer Structure

```
Sonar-back-end/
├── Sonar/                    # Presentation Layer (API Controllers, Hubs, Middleware)
├── Application/              # Application Layer (Services, DTOs, Business Logic)
├── Infrastructure/           # Infrastructure Layer (Data Access, Repositories, External Services)
├── Entities/                 # Domain Layer (Domain Models, Enums)
└── Logging/                  # Cross-cutting Concern (Logging Infrastructure)
```

### Architecture Layers Explained

#### 1. **Sonar (Presentation Layer)**
- **Purpose**: Entry point of the application, handles HTTP requests and WebSocket connections
- **Components**:
  - Controllers (REST API endpoints)
  - SignalR Hubs (real-time chat functionality)
  - Middleware (exception handling, authentication)
  - Health checks
  - gRPC clients for external services
- **Responsibilities**:
  - Request/response handling
  - Authentication & authorization
  - Input validation
  - Response formatting

#### 2. **Application (Application Layer)**
- **Purpose**: Contains business logic and application services
- **Components**:
  - Services (business logic implementation)
  - DTOs (Data Transfer Objects)
  - Interfaces (service contracts)
  - Response models
  - Email templates
- **Key Features**:
  - Transaction management
  - Email sending (SMTP)
  - File processing (audio, image, video)
  - QR code generation
  - Share functionality

#### 3. **Infrastructure (Infrastructure Layer)**
- **Purpose**: Data access and external service integration
- **Components**:
  - Entity Framework Core DbContext
  - Repositories (data access abstraction)
  - Database migrations
  - Seed data factories
  - File storage service
- **Responsibilities**:
  - Database operations
  - Transaction management
  - Data persistence

#### 4. **Entities (Domain Layer)**
- **Purpose**: Core domain models and business entities
- **Components**:
  - Domain models (User, Track, Album, Chat, etc.)
  - Enums
  - Base model classes
- **Key Entities**:
  - User management (User, UserSession, UserState, UserFollow)
  - Music (Track, Album, Artist, Playlist, Genre, MoodTag)
  - Chat (Chat, Message, MessageRead, Post)
  - Library (Library, Folder, Collection)
  - Distribution (Distributor, DistributorAccount, License)
  - User Experience (Achievement, CosmeticItem, Gift, SubscriptionPack)
  - Access Control (AccessFeature, VisibilityState, Suspension)
  - Reports (Report, ReportReasonType, ReportableEntityType)

#### 5. **Logging (Cross-cutting)**
- **Purpose**: Centralized logging infrastructure
- **Features**:
  - Custom EF Core logger provider
  - Log categorization
  - Structured logging

## 🛠️ Technology Stack

### Core Framework & Runtime
- **.NET 9.0** - Latest .NET runtime
- **ASP.NET Core 9.0** - Web framework
- **C# 12** - Programming language with modern features

### Database & ORM
- **PostgreSQL** - Primary database
- **Entity Framework Core 9.0.11** - ORM framework
- **Npgsql.EntityFrameworkCore.PostgreSQL 9.0.4** - PostgreSQL provider
- **Identity Framework** - User authentication and authorization

### Authentication & Security
- **JWT Bearer Authentication** - Token-based authentication
- **ASP.NET Core Identity** - User management system
- **Password hashing** - Secure password storage
- **Refresh tokens** - Session management
- **2FA (Two-Factor Authentication)** - Email-based 2FA

### API & Communication
- **REST API** - Standard HTTP endpoints
- **SignalR** - Real-time communication (chat)
- **gRPC** - Inter-service communication with Analytics service
- **OpenAPI/Swagger** - API documentation

### File Processing & Media
- **NAudio 2.2.1** - Audio file processing
- **TagLibSharp 2.3.0** - Metadata extraction from media files
- **FileSignatures 6.1.1** - File type validation
- **QRCoder 1.7.0** - QR code generation

### Email Services
- **SMTP** - Email delivery
- **HTML email templates** - Customizable email templates

### Development & Testing
- **Swashbuckle.AspNetCore 9.0.6** - Swagger UI
- **Microsoft.EntityFrameworkCore.Tools** - Migration tools
- **xUnit** (implied by test project) - Unit testing framework

### Containerization & Deployment
- **Docker** - Containerization
- **Docker Compose** - Multi-container orchestration
- **Linux containers** - Production deployment

## 📦 Key Features & Modules

### 1. **User Management**
- User registration with email confirmation
- Login with JWT tokens
- Password recovery
- User profiles and settings
- User sessions management
- Public identifiers
- User follow system

### 2. **Music Management**
- Track upload and management
- Album creation and management
- Artist profiles
- Playlist creation and management
- Genre and mood tag system
- Track metadata extraction
- Audio file processing

### 3. **Library System**
- Personal music library
- Folder organization
- Collections (playlists, albums, blends)
- Root folder structure
- Collection management

### 4. **Chat & Social**
- Real-time chat via SignalR
- Group chats with admins and members
- Direct messages
- Message read receipts
- Chat stickers and categories
- User posts

### 5. **Distribution**
- Artist registration
- Distributor accounts
- License management
- Content distribution workflow

### 6. **User Experience**
- Achievements system
- Achievement progress tracking
- Cosmetic items (stickers, avatars)
- Gift system
- Subscription packs and features
- Inventory management

### 7. **Access Control**
- Access features (role-based permissions)
- Visibility states (public/private)
- User suspensions
- Privacy settings

### 8. **Reporting System**
- Content reporting
- User reporting
- Report reason types
- Reportable entity types

### 9. **Search & Discovery**
- Content search
- User search
- Integration with Analytics service via gRPC

### 10. **File Management**
- Audio file upload/processing
- Image file management
- Video file support
- File format validation
- Default file handling

## 🔄 Data Flow & Patterns

### Request Flow
```
Client Request
    ↓
Controller (Sonar Layer)
    ↓
Service (Application Layer)
    ↓
Repository (Infrastructure Layer)
    ↓
Database (PostgreSQL)
```

### Transaction Management
- Database transactions for critical operations (e.g., user registration)
- Rollback on failures (e.g., email sending failure cancels registration)
- Transaction isolation for data consistency

### Dependency Injection
- All services registered via DI container
- Interface-based design for testability
- Scoped, Singleton, and Transient lifetimes appropriately used

### Repository Pattern
- Generic repository for common CRUD operations
- Specialized repositories for complex queries
- Repository interfaces in Application layer
- Implementations in Infrastructure layer

## 🔐 Security Features

1. **Authentication**
   - JWT token-based authentication
   - Refresh token mechanism
   - Session management
   - Device tracking

2. **Authorization**
   - Role-based access control (RBAC)
   - Access features system
   - Resource-level permissions

3. **Data Protection**
   - Password hashing (Identity framework)
   - SQL injection prevention (EF Core parameterized queries)
   - Input validation
   - Email confirmation for registration

4. **Privacy**
   - Visibility states (public/private)
   - User privacy settings
   - Blocked users functionality

## 📊 Database Design

### Key Relationships
- **User** has one-to-one relationships with Settings, UserState, Inventory, Library, VisibilityState
- **User** has many-to-many relationships with Friends, Chats (as member/admin)
- **Track/Album** belong to Artists (many-to-many)
- **Playlist** belongs to User (creator)
- **Library** contains Folders, which contain Collections
- **Chat** has Messages, which have MessageRead records

### Delete Behaviors
- **Cascade**: Dependent entities deleted with parent (e.g., user sessions, messages)
- **NoAction**: Independent entities preserved (e.g., default avatar images)
- **Restrict**: Prevents deletion if dependencies exist

## 🚀 Deployment

### Docker Support
- Multi-stage Dockerfile for optimized builds
- Docker Compose for local development
- Linux-based containers
- Volume mounts for file storage

### Configuration
- `appsettings.json` for configuration
- Environment-specific settings
- Connection strings
- SMTP settings
- JWT configuration
- Frontend URL configuration

## 📝 API Documentation

The project includes comprehensive API documentation:
- OpenAPI/Swagger integration
- Endpoint documentation in `Documentation/` folder
- DTO documentation
- Response format specifications

## 🔧 Development Practices

1. **Clean Code**
   - SOLID principles
   - DRY (Don't Repeat Yourself)
   - Separation of concerns

2. **Error Handling**
   - Custom exception middleware
   - Structured error responses
   - Detailed error messages

3. **Logging**
   - Custom logging infrastructure
   - EF Core query logging
   - Categorized logs

4. **Testing**
   - Test project structure
   - Unit test capabilities

## 🌐 External Integrations

1. **Analytics Service** (gRPC)
   - Analytics data collection
   - Recommendations engine

2. **SMTP Server**
   - Email delivery
   - Transactional emails

## 📈 Scalability Considerations

- Stateless API design
- Database connection pooling
- Efficient query patterns
- File storage separation
- Microservices-ready architecture (gRPC integration)

## 🔮 Future Enhancements

Based on the architecture, potential enhancements could include:
- Caching layer (Redis)
- Message queue (RabbitMQ/Azure Service Bus)
- CDN for file delivery
- Full-text search (Elasticsearch)
- Monitoring and observability (Application Insights, Prometheus)

---

**Last Updated**: December 2024  
**Version**: 1.0  
**Target Framework**: .NET 9.0

