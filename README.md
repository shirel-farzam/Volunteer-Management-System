# 🚑 Volunteer Management System 🧑‍🤝‍🧑⏱️🧠

A **Volunteer & Task Management System** developed as part of an academic **Software Engineering** project 🎓💻  
The system simulates a **real-time volunteering environment**, combining **clean layered architecture**,  
**time simulation**, and **multi-threaded execution** to accurately model real-world scenarios ⚙️🕰️

---

## 🎯 System Capabilities

- 🧑‍🤝‍🧑 Manage volunteers and task assignments  
- 📋 Track tasks and their full lifecycle  
- ⏱️ Simulate the passage of time using an internal logical clock  
- 🔄 Automatically update task & volunteer states  
- 🧵 Run time progression and business logic **in parallel** using multi-threading  
- 🧪 Support multiple data sources (List / XML) for testing and persistence  

---

## 🧠 Core Concepts & Design Highlights

### ⏱️ Time Simulation Engine
- The system does **not rely on real system time**
- A logical clock advances in controlled **"ticks"**
- Enables fast-forward simulations and edge-case testing 🚀

---

### 🧵 Multi-Threaded Design
- Time simulation runs on a **dedicated thread**
- Business logic reacts to time changes **asynchronously**
- Ensures responsiveness and realistic system behavior ⚙️⚙️

---

### 🏗️ Layered Architecture
The project follows a **clean layered architecture**, enforcing a clear separation of concerns:

- **Presentation Layer (PL)** – User interaction  
- **Business Logic Layer (BL)** – Core rules & system logic  
- **Data Access Layer (DAL)** – Data storage & persistence  

This design improves maintainability, scalability, and testability ✨

---

## 📁 Project Structure

Volunteer-Management-System/
│
├── BL/             # Business Logic – rules, validations, time-based logic
├── PL/             # Presentation Layer – user-facing interface
│
├── DalFacade/      # DAL interfaces & contracts
├── DalList/        # In-memory data implementation (testing/debug)
├── DalXml/         # XML-based persistent data implementation
│
├── BlTest/         # Business Logic unit tests
├── DalTest/        # Data Access Layer tests
│
├── xml/            # XML data files
├── stage0/         # Initial prototype / early development stage
│
├── dotNet5785_-4642_7701.sln   # Visual Studio solution file
└── README.md

---

## ▶️ How to Run the Project

### 🛠️ Prerequisites
- Visual Studio (recommended: 2022 or later)
- .NET SDK installed

---

### 🚀 Running the System

1. Open the solution file in Visual Studio:
   dotNet5785_-4642_7701.sln

2. Set the **Presentation Layer (PL)** project as the startup project

3. Build and run the solution ▶️

💡 The system allows switching between different DAL implementations  
(List / XML) via configuration or code setup.

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

---

## 🎨 README Design Credit

README style inspired by modern GitHub project presentations ✨  
Designed with clarity, structure, and developer experience in mind 🧠💙

---

⭐ If you find this project interesting, feel free to give it a star! ⭐  
Built with clean architecture, careful design, and real-world thinking 🚀
