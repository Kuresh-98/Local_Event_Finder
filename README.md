# Local Event Finder

A modern ASP.NET Core web application for discovering and managing local events in Gujarat, India. Built with ASP.NET Core 9.0, Entity Framework Core, and Bootstrap.

## 🚀 Features

### For Users
- **Browse Events**: View all available events with filtering by city, category, and date
- **Event Registration**: Reserve seats for events with real-time availability tracking
- **My Events**: Track your registered events and manage reservations
- **Location-Based Search**: Find events within a 10km radius of your location
- **Event Details**: View comprehensive event information including venue, organizer, and seat availability
- **Map Integration**: Click on event addresses to open them in your default map application

### For Administrators
- **Event Management**: Create, edit, and delete events
- **Seat Management**: Set total seats and monitor available seats
- **Event Cancellation**: Cancel events with automatic notification to registered users
- **User Management**: Full administrative control over the platform

### Technical Features
- **Role-Based Access Control**: Separate interfaces for users and administrators
- **Real-Time Seat Tracking**: Automatic seat availability updates
- **Responsive Design**: Modern, mobile-friendly UI with dark theme
- **Database Persistence**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity with custom login/register pages

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core 9.0 (MVC + Razor Pages)
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Bootstrap 5, jQuery, Custom CSS
- **Maps**: Integration with external map services
- **Architecture**: Repository pattern with service layer

## 📋 Prerequisites

- .NET 9.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code (recommended)

## 🚀 Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/yourusername/local-event-finder.git
cd local-event-finder
```

### 2. Database Setup
The application uses Entity Framework Core with Code First migrations. The database will be created automatically on first run.

### 3. Configuration
Update the connection string in `appsettings.json` if needed:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LocalEventFinder;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 4. Run the Application
```bash
dotnet restore
dotnet run
```

The application will be available at `http://localhost:5000`

### 5. Default Admin Account
On first run, the application creates a default admin account:
- **Email**: admin@example.com
- **Password**: Admin123!

## 📁 Project Structure

```
Local Event Finder/
├── Controllers/          # MVC Controllers
├── Models/              # Data Models
├── Services/            # Business Logic Services
├── Data/               # Database Context and Migrations
├── Views/              # Razor Views
├── Areas/              # Identity Area
├── ViewModels/         # View Models
├── wwwroot/            # Static Files (CSS, JS, Images)
└── Properties/         # Launch Settings
```

## 🎯 Key Features Implementation

### Event Management
- **CRUD Operations**: Full create, read, update, delete functionality
- **Seat Tracking**: Real-time seat availability with automatic updates
- **Event Cancellation**: Admin can cancel events with user notifications

### User Experience
- **Responsive Design**: Mobile-first approach with Bootstrap
- **Dark Theme**: Modern dark UI with custom styling
- **Interactive Elements**: Hover effects, animations, and smooth transitions
- **Form Validation**: Client and server-side validation

### Location Services
- **Distance Calculation**: Haversine formula for accurate distance measurement
- **Map Integration**: Direct links to external map services
- **Location-Based Filtering**: Find events within specified radius

## 🔧 Configuration

### Database Connection
Update the connection string in `appsettings.json` for your environment.

### Email Configuration (Optional)
Configure SMTP settings in `appsettings.json` for email notifications.

### Map Services
The application integrates with external map services. Update map URLs in views as needed.

## 🧪 Testing

The application includes sample data for testing:
- 25+ sample events across Gujarat, India
- Various event categories (Music, Education, Sports, etc.)
- Different cities and venues
- Sample seat configurations

## 📱 Screenshots

### Home Page
- Modern hero section with feature highlights
- Step-by-step guide for new users
- Responsive design with dark theme

### Events Page
- Grid layout with event cards
- Advanced filtering options
- Pagination for large event lists

### Event Details
- Comprehensive event information
- Seat availability tracking
- Registration functionality

### My Events
- User's registered events
- Cancellation status
- Event management

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Bootstrap for the responsive UI framework
- Entity Framework Core for data access
- ASP.NET Core team for the excellent framework
- All contributors and testers

## 📞 Support

For support, email support@localeventfinder.com or create an issue in the repository.

---

**Built with ❤️ using ASP.NET Core**
