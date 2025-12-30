# 🚑 Volunteer Management System 🧑‍🤝‍🧑⏱️🧠

A **Volunteer & Task Management System** developed as part of an academic  
**Software Engineering** project 🎓💻  

The system simulates a **real-time volunteering environment**, combining  
**clean layered architecture**, **time simulation**, and **multi-threaded execution**  
to accurately model real-world scenarios ⚙️🕰️

---

## 🎯 System Overview

This project focuses on managing volunteers and tasks while simulating  
real-life time progression and asynchronous system behavior.

Key goals:
- Realistic time-based task handling  
- Clear separation of responsibilities  
- Flexible data persistence strategies  
- Maintainable and testable architecture  

---

## ✅ System Capabilities

- 🧑‍🤝‍🧑 Manage volunteers and task assignments  
- 📋 Track tasks and their full lifecycle  
- ⏱️ Simulate time using an internal logical system clock  
- 🔄 Automatically update task & volunteer states  
- 🧵 Execute time progression and business logic **in parallel** (multi-threading)  
- 🧪 Support multiple data sources (List / XML) for testing & persistence  

---

## 🧠 Core Concepts & Design

### ⏱️ Time Simulation Engine
- The system does **not rely on real system time**
- A logical clock advances in controlled **ticks**
- Enables fast-forward simulations and edge-case testing 🚀

### 🧵 Multi-Threaded Execution
- Time simulation runs on a **dedicated thread**
- Business logic reacts to time changes **asynchronously**
- Ensures responsiveness and realistic system behavior ⚙️⚙️

### 🏗️ Layered Architecture
The project follows a **clean layered architecture**, enforcing separation of concerns:

- **Presentation Layer (PL)** – user interaction  
- **Business Logic Layer (BL)** – rules, validations, time-based logic  
- **Data Access Layer (DAL)** – data storage & persistence  

This approach improves maintainability, scalability, and testability ✨

---

## 📁 Project Structure
Volunteer-Management-System/
│
├── BL/ # Business Logic – rules, validations, time-based logic
├── PL/ # Presentation Layer – user-facing interface
│
├── DalFacade/ # DAL interfaces & contracts
├── DalList/ # In-memory data implementation (testing/debug)
├── DalXml/ # XML-based persistent data implementation
│
├── BlTest/ # Business Logic unit tests
├── DalTest/ # Data Access Layer unit tests
│
├── xml/ # XML data files
├── stage0/ # Initial prototype / early development stage
│
├── dotNet5785_-4642_7701.sln # Visual Studio solution file
└── README.md
---

## ▶️ How to Run the Project

### 🛠️ Prerequisites
- Visual Studio (recommended: 2022 or later)
- .NET SDK installed

### 🚀 Running the System

1. Open the solution file in Visual Studio:
   dotNet5785_-4642_7701.sln

2. Set the **PL (Presentation Layer)** project as the startup project

3. Build and run the solution ▶️

💡 You can switch between different DAL implementations  
(List / XML) depending on testing or persistence needs.

---

## 🛠️ Technologies Used

- 💻 C#  
- 🧩 .NET (Multi-Project Solution)  
- 🏗️ Layered Architecture (PL / BL / DAL)  
- 🧵 Multi-Threading  
- ⏱️ Time Simulation  
- 📄 XML Serialization  
- 🧪 Unit Testing  

---

## 👩‍💻 Project Contributors

- **Shirel Farzam**  
  GitHub: [shirel-farzam](https://github.com/shirel-farzam)

- **Ayelet Benisti**  
  GitHub: [Ayelet929](https://github.com/Ayelet929)

---

## ✨ Notes

This project emphasizes **architecture, correctness, and realism** over UI design  
and serves as a strong example of structured system design in .NET environments.

---

⭐ If you find this project interesting, feel free to give it a star! ⭐  
Built with clean architecture, careful design, and real-world thinking 🚀
