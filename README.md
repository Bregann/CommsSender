# 📨 CommsSender

<div align="center">

A comprehensive communication platform for sending push notifications and Telegram messages through a unified API, with a native mobile app for push token management.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=.net)](https://dotnet.microsoft.com/)
[![React Native](https://img.shields.io/badge/React_Native-0.81-61DAFB?logo=react)](https://reactnative.dev/)
[![Expo](https://img.shields.io/badge/Expo-~54.0-000020?logo=expo)](https://expo.dev/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-336791?logo=postgresql)](https://www.postgresql.org/)
[![Hangfire](https://img.shields.io/badge/Hangfire-Job_Scheduler-00A000)](https://www.hangfire.io/)

</div>

---

## 🌟 Overview

**CommsSender** is a robust, production-ready communication service that provides:

- 📱 **Push Notifications** - Send notifications to iOS and Android devices via Expo Push Service
- 💬 **Telegram Integration** - Deliver messages through Telegram Bot API
- 🔄 **Background Processing** - Reliable message queue with Hangfire
- 📊 **Message Tracking** - Full audit trail and delivery status monitoring
- 🔐 **Secure API** - API key authentication middleware
- 📲 **Mobile App** - React Native/Expo app for registering push tokens

## 🏗️ Architecture

### Backend Stack
- **.NET 10.0** - Modern C# backend API
- **PostgreSQL** - Primary database with Entity Framework Core
- **Hangfire** - Background job processing and scheduling
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - Interactive API documentation
- **Docker** - Containerization support

### Mobile App Stack
- **React Native** - Cross-platform mobile development
- **Expo SDK 54** - Managed workflow and native APIs
- **TypeScript** - Type-safe development
- **Expo Router** - File-based navigation

## 📋 Features

### Core Functionality

#### 🔔 Push Notifications
- Queue and send push notifications to registered devices
- Automatic retry logic for failed deliveries
- Validation of successful delivery
- Support for title, body, and custom data payloads
- Expo Push Notification service integration

#### 💬 Telegram Messages
- Send messages to Telegram chats via Bot API
- Support for Markdown and HTML formatting
- Message queue management
- Delivery status tracking

#### 🔄 Background Jobs
- **Message Processor** - Processes pending messages every 20 seconds
- **Validation Job** - Validates push notification delivery every minute
- Configurable polling intervals
- Automatic error handling and retry logic

#### 🛡️ Security
- API key authentication via custom middleware
- CORS policy configuration
- Secure environmental settings management

### 📱 Mobile App Features
- Request and manage notification permissions
- Register Expo push tokens with the backend
- Display current push token
- Test notification delivery
- Clean, modern UI with React Native

## 🚀 Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 15+](https://www.postgresql.org/download/) (or use Docker)
- [Node.js 18+](https://nodejs.org/) and npm
- [Expo CLI](https://docs.expo.dev/get-started/installation/)
- (Optional) [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Backend Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd CommsSender-main
   ```

2. **Configure Database**
   
   For development, the app uses Testcontainers and will automatically spin up a PostgreSQL instance.
   
   For production, set environment variables:
   ```bash
   xxxConnStringLive="Host=your-host;Database=commssender;Username=user;Password=pass"
   xxxConnString="Host=your-host;Database=commssender;Username=user;Password=pass"
   ```

3. **Configure API Key**
   
   Update `appsettings.json`:
   ```json
   {
     "CommsSenderApiKey": "your-secure-api-key-here"
   }
   ```

4. **Configure Telegram Bot**
   
   Set up environmental settings in the database:
   - `TelegramBotToken` - Your Telegram Bot API token
   - `TelegramChatId` - Default chat ID for messages

5. **Run the API**
   ```bash
   cd CommsSender.Core
   dotnet restore
   dotnet run
   ```

   The API will be available at `https://localhost:5001` (or configured port)

6. **Access Hangfire Dashboard**
   
   Navigate to `https://localhost:5001/hangfire` to monitor background jobs.

### Mobile App Setup

1. **Navigate to the app directory**
   ```bash
   cd comms-sender
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Configure API settings**
   
   Create a `.env` file:
   ```bash
   EXPO_PUBLIC_API_KEY=your-api-key-here
   EXPO_PUBLIC_API_URL=https://your-api-domain.com
   ```

4. **Start the development server**
   ```bash
   npm start
   ```

5. **Run on device**
   
   ⚠️ **Important:** Push notifications only work on physical devices, not simulators!
   
   - **iOS:** Connect your iPhone and run `npm run ios`
   - **Android:** Connect your Android device and run `npm run android`
   - **Expo Go:** Scan the QR code with Expo Go app

## 📡 API Endpoints

### Message Controller

#### Send Push Notification
```http
POST /api/Message/SendPushNotification
X-CommsSender-ApiKey: your-api-key

{
  "title": "Hello!",
  "body": "This is a push notification"
}
```

#### Send Telegram Message
```http
POST /api/Message/SendTelegramMessage
X-CommsSender-ApiKey: your-api-key

{
  "chatId": 123456789,
  "messageText": "Hello from CommsSender!"
}
```

### Push Token Controller

#### Register Push Token
```http
POST /api/PushToken/RegisterPushToken
X-CommsSender-ApiKey: your-api-key
Content-Type: application/json

"ExponentPushToken[xxxxxxxxxxxxxxxxxxxxxx]"
```

## 🗄️ Database Schema

### Core Tables

- **Messages** - Stores all messages (push notifications and Telegram)
- **PushTokens** - Registered device push tokens
- **MessageErrorLogs** - Error tracking and debugging
- **EnvironmentalSettings** - System configuration key-value store

### Message Statuses

- `Pending` - Queued for processing
- `Sent` - Successfully delivered
- `Failed` - Delivery failed
- `Processing` - Currently being processed

## 🔧 Configuration

### Environmental Settings

The system uses a database-driven configuration system. Key settings include:

- `TelegramBotToken` - Telegram Bot API token
- `TelegramChatId` - Default Telegram chat ID
- `HangfireUsername` - Hangfire dashboard credentials
- `HangfirePassword` - Hangfire dashboard credentials
- `CommsSenderApiKey` - API authentication key

These can be managed through the `EnvironmentalSettings` table or via the `IEnvironmentalSettingHelper` service.

## 🐳 Docker Support

A `Dockerfile` is included in the `CommsSender.Core` project:

```bash
docker build -t commssender:latest -f CommsSender.Core/Dockerfile .
docker run -p 5000:8080 -e xxxConnStringLive="..." commssender:latest
```

## 🔄 Background Jobs

The system uses Hangfire for reliable background processing:

| Job | Schedule | Purpose |
|-----|----------|---------|
| `process-pending-messages` | Every 20 seconds | Processes queued messages |
| `validate-push-notifications` | Every minute | Validates push notification delivery |

Access the Hangfire dashboard at `/hangfire` to monitor jobs, view statistics, and manually trigger processing.

## 🧪 Development

### Running in Debug Mode

In debug mode, the application:
- Uses Testcontainers to spin up a PostgreSQL instance automatically
- Seeds the database with initial data
- Uses in-memory storage for Hangfire (can be configured)
- Enables Swagger UI

### Project Structure

```
├── CommsSender.Core/          # ASP.NET Core Web API
│   ├── Controllers/           # API Controllers
│   ├── Middleware/            # Custom middleware (API Key)
│   └── Program.cs             # Application startup
│
├── CommsSender.Domain/        # Domain layer
│   ├── Database/              # EF Core DbContext and Models
│   ├── DTOs/                  # Data Transfer Objects
│   ├── Enums/                 # Enumerations
│   ├── Helpers/               # Helper services
│   ├── Interfaces/            # Service interfaces
│   └── Services/              # Business logic
│
└── comms-sender/              # React Native/Expo mobile app
    ├── app/                   # Expo Router pages
    ├── assets/                # Images and resources
    └── config/                # App configuration
```

## 📝 Usage Example

### C# Client Implementation

```csharp
public class CommsSenderClient : ICommsSenderClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseAddress;

    public async Task<bool> SendPushNotification(string title, string body)
    {
        var request = new
        {
            Title = title,
            Body = body
        };

        _httpClient.DefaultRequestHeaders.Add("X-CommsSender-ApiKey", _apiKey);
        var response = await _httpClient.PostAsJsonAsync(
            $"{_baseAddress}/api/Message/SendPushNotification", 
            request
        );
        
        return response.IsSuccessStatusCode;
    }
}
```

### Mobile App Integration

The mobile app automatically:
1. Requests notification permissions on launch
2. Retrieves the Expo push token
3. Displays the token to the user
4. Provides a button to register the token with your API

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is licensed under the terms included in the `LICENSE` file.

## 🙏 Acknowledgments

- [Expo](https://expo.dev/) - For the amazing mobile development platform
- [Hangfire](https://www.hangfire.io/) - For reliable background job processing
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) - For data access
- [Testcontainers](https://testcontainers.com/) - For containerized testing

## 📞 Support

For issues, questions, or contributions, please open an issue on GitHub.

---

<div align="center">

**Built with ❤️ using .NET and React Native**

</div>
