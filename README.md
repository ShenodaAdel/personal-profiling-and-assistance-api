# Personal Profiling and Assistance

## 📌 Project Overview
Personal Profiling and Assistance is a web-based platform designed for the **Arab world** to help individuals assess their **personality, mental health, and communication skills** through AI-powered tests. The system provides personalized insights and recommendations based on test results.

## 🚀 Features
### 🧠 Personality & Mental Health Assessments
- **Personality Type Determination Test**
- **Anxiety Test**
- **Depression Test**
- **Communication Skills Test** (AI-based voice & facial expression analysis)

### 🎯 AI-Powered Insights
- **Voice analysis**: Assess personality traits from recorded speech.
- **Facial expression recognition**: Detect emotions and personality tendencies.

### 📍 Assistance & Tracking
- **Track test results** over time.
- **Find the nearest clinic** using integrated maps if help is needed.

## 🛠️ Tech Stack
- **Frontend:**  Angular
- **Backend:** .NET 8 with C#
- **Database:** SQL Server (with Entity Framework Core)
- **Authentication:** JWT-based authentication
- **AI Integration:** Machine Learning models for voice & facial analysis
- **Cloud & Hosting:** Azure / AWS / On-Premises deployment 

## ⚙️ Setup & Installation
### Prerequisites
- .NET 8 SDK
- SQL Server
- Packages -> (ByCrypt + Tools + SqlServer + JwtBearer + Identity.EntityFrameworkCore + Ef + )
- Visual Studio / VS Code / Postman 

### 🔧 Backend Setup
```bash
# Clone the repository
git clone https://github.com/your-repo/PersonalProfiling.git
cd PersonalProfiling

# Configure appsettings.json (update database connection string)

# Apply migrations
dotnet ef database update

# Run the API
dotnet run
```

### 🎨 Frontend Setup (if applicable)


## 🏗️ Project Structure
```
📦 PersonalProfiling
 ┣ 📂 BusinessLogic (Service layer)
 ┣ 📂 Data (Entity Framework models, DB context)
 ┣ 📂 API (Controllers, Middleware, Authentication)
 ┣ 📂 AI (Machine Learning models for speech & face analysis)
 ┣ 📂 Frontend (React/Angular/Vue)
 ┗ README.md
```

## 📡 API Endpoints


## 🛡️ Security
- **JWT authentication** for user sessions.
- **Hashed passwords** using BCrypt.
- **Database security best practices.**

## 📌 Future Enhancements
- **Mobile app integration** 
- **More AI-based assessments**
- **Add companies to create my tests for each company separately, apply them, and choose the best of all applicants for this job**

## 🏆 Contributors
- **Shenoda Adel** (The Team Leader & Backend Developer)
- **Sandra Emad** (FrontEnd Developer)
- **Ahmed Shaker** (Machine Learning and AI)
- **Sara Maher** (FrontEnd Developer)
- **Abanuob Fawwaz** (BackEnd Developer)
- **Bassent Hassan** (Machine Learning and AI)
  
## 📜 License
This project is licensed under the **MIT License**.

---
Made with ❤️ for the Arab world 🌍

