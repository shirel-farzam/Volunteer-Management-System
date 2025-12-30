# 🚑 Volunteer Management System 🧑‍🤝‍🧑⏱️🧠

A **Volunteer & Task Management System** developed as part of an academic  
**Software Engineering** project 🎓💻  

The system simulates a **real-time volunteering environment**, combining  
**clean layered architecture**, **time simulation**, and **multi-threaded execution**  
to accurately model real-world scenarios ⚙️🕰️

---

## 🎯 System Overview

This project manages volunteers and tasks while simulating realistic  
time progression and asynchronous system behavior.

The main focus is on:
- Correct architectural separation  
- Time-based logic and simulation  
- Parallel execution  
- Testability and maintainability  

---

## ✅ System Capabilities

- 🧑‍🤝‍🧑 Manage volunteers and task assignments  
- 📋 Track tasks throughout their lifecycle  
- ⏱️ Simulate time using an internal logical clock  
- 🔄 Automatically update task & volunteer states  
- 🧵 Execute logic concurrently using multi-threading  
- 🧪 Support multiple persistence strategies (List / XML)  

---

## 🧠 Core Concepts

### ⏱️ Time Simulation Engine
- Does **not** rely on real system time  
- Uses a logical clock that advances in controlled ticks  
- Allows fast-forward simulations and edge-case testing  

### 🧵 Multi-Threaded Execution
- Time simulation runs on a dedicated thread  
- Business logic reacts asynchronously to time changes  
- Ensures realistic and responsive system behavior  

### 🏗️ Layered Architecture
The system follows a strict layered architecture:
- Presentation Layer (PL)  
- Business Logic Layer (BL)  
- Data Access Layer (DAL)  

This separation improves clarity, scalability, and testability.

---

## 📁 Project Structure

### 🧠 Business Logic (BL/)
- Core system rules  
- Validations and constraints  
- Time-based decision logic  
- Volunteer & task state management  

---

### 🖥️ Presentation Layer (PL/)
- User-facing interface  
- System interaction and input handling  
- Communicates **only** with the Business Logic layer  

---

### 🗄️ Data Access Layer (DAL)

#### DalFacade/
- Interfaces and contracts for data access  
- Decouples business logic from storage implementation  

#### DalList/
- In-memory data implementation  
- Used mainly for testing and debugging  

#### DalXml/
- XML-based persistent data storage  
- Uses serialized files for long-term persistence  

---

### 🧪 Testing
- BlTest/ – Business Logic unit tests  
- DalTest/ – Data Access Layer unit tests  

---

### 📂 Additional Folders
- xml/ – XML data files  
- stage0/ – Initial prototype / early development stage  

---

### 📄 Solution Files
- dotNet5785_-4642_7701.sln – Visual Studio solution file  
- README.md – Project documentation  

---

## ▶️ How to Run the Project

### 🛠️ Prerequisites
- Visual Studio (recommended: 2022 or later)  
- .NET SDK installed  

### 🚀 Running the System
1. Open the solution file:
   dotNet5785_-4642_7701.sln  
2. Set the **PL (Presentation Layer)** project as the startup project  
3. Build and run the solution ▶️  

You can switch between DAL implementations (List / XML) depending on  
testing or persistence needs.

---

## 🛠️ Technologies Used

- 💻 C#  
- 🧩 .NET (Multi-Project Solution)  
- 🏗️ Layered Architecture  
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

⭐ If you find this project interesting, feel free to give it a star! ⭐  
Built with clean architecture, careful design, and real-world thinking 🚀
