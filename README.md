🚑 Volunteer Management System 🧑‍🤝‍🧑⏱️🧠

A Volunteer & Task Management System developed as part of an academic software engineering project 🎓💻
The system simulates a real-time volunteering environment, combining clean layered architecture,
time simulation, and multi-threaded execution to model real-world scenarios accurately ⚙️🕰️

🎯 What Can the System Do? 🎯

🧑‍🤝‍🧑 Manage volunteers and assignments

📋 Track tasks and their lifecycle

⏱️ Simulate the passage of time (system clock)

🔄 Automatically update task & volunteer states

🧵 Run time progression and logic in parallel using Multi-Threading

🧪 Support multiple data sources (List / XML) for testing & persistence

🧠 Core Concepts & Highlights 🧠
⏱️ Time Simulation Engine

The system does not rely on real system time

A logical clock advances in controlled “ticks”

Enables fast-forward simulations and edge-case testing 🚀

🧵 Multi-Threaded Design

Time simulation runs on a dedicated thread

Business logic reacts to time changes asynchronously

Ensures responsiveness and realistic system behavior ⚙️⚙️

🏗️ Layered Architecture

Clear separation between:

Presentation

Business Logic

Data Access

Promotes maintainability, scalability, and clean code ✨

📁 Project Structure 📁
🧠 Business Logic (BL/)

Core system rules and validations

Task & volunteer state management

Time-based decision making

🧱 Data Access Layer
DalFacade/

Interfaces & contracts for data access

Enables easy switching between data sources

DalList/

In-memory data storage

Ideal for testing and debugging 🧪

DalXml/

Persistent XML-based storage

Uses serialized files located in the xml/ directory 📄

🖥️ Presentation Layer (PL/)

User-facing interface (Console / UI)

Communicates only with BL, never directly with DAL

🧪 Testing

BlTest/ – Business Logic tests

DalTest/ – Data Access tests

🛠️ Technologies Used 🛠️

💻 C#

🧩 .NET (Multi-Project Solution)

🏗️ Layered Architecture (PL / BL / DAL)

🧵 Multi-Threading

⏱️ Time Simulation

📄 XML Serialization

🧪 Unit Testing

▶️ How to Run ▶️
# Open the solution file in Visual Studio
dotNet5785_-4642_7701.sln

# Build & Run the Presentation Layer project


💡 You can switch between DAL implementations (List / XML) easily via configuration.

🧑‍💻 Project Contributors 🧑‍💻

Elyasaf Cohen

Team Members (as listed in project submission)

🎨 README Style Credit 🎨

README design inspired by:
Shirel Farzam 💖
GitHub: https://github.com/shirel-farzam

⭐ If this project impressed you – a GitHub star is always appreciated! ⭐
Built with passion, architecture, and a lot of brainpower 🧠🔥
