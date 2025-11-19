# Contract Monthly Claim System (CMCS) - PROG6212 Part 3

This project is a comprehensive, web-based Contract Monthly Claim System developed using **ASP.NET Core MVC** and **Entity Framework Core**. It streamlines the claim submission and approval process for university lecturers, Programme Coordinators, and Academic Managers.

For Part 3, the system has been significantly enhanced with enterprise-grade features including a migration to **MS SQL Server**, a new **HR Super User** role, and extensive **process automation**.

---

## 🚀 Key Features & Part 3 Enhancements

### 1. Automated Lecturer Claim Submission
* **Smart Auto-Fill:** Lecturer personal details and hourly rates are automatically pulled from their HR-managed profile, preventing manual entry errors.
* **Real-Time Calculation:** The claim total is automatically calculated via JavaScript as the lecturer types.
* **Policy Validation:** Built-in validation logic automatically blocks claims that exceed policy limits (e.g., >180 hours/month).
* **Secure File Uploads:** Supporting documents are validated (PDF/DOCX, max 5MB) and stored securely using **AES-256 encryption**.

### 2. HR Management & Reporting (New Role)
* **Centralized User Management:** A new **HR Super User** role has full control to create users, update profiles, and set verified hourly rates.
* **Automated Reporting:** HR can instantly generate a professional **Payment Report/Invoice** that aggregates all approved claims and calculates the grand total for finance processing.

### 3. Automated Verification (Manager View)
* **Smart Flagging:** The "Approver Queue" automatically analyzes claims against predefined criteria.
* **Visual Alerts:** Claims exceeding **R10,000** or **100 hours** are automatically flagged with an **Amber highlight** and specific warning badges ("High Value", "High Hours") to alert managers.

### 4. Enterprise Architecture
* **Database:** Migrated from SQLite to **MS SQL Server** for robustness and scalability.
* **Security:** Implemented role-based authorization (Lecturer, Coordinator, Manager, HR) and secure document handling.
* **Quality Assurance:** Core business logic and new automation features are verified by **8 Unit Tests**.

---

## 🛠️ Technologies Used
* **Framework:** ASP.NET Core 9.0 MVC
* **Database:** MS SQL Server (Entity Framework Core)
* **Authentication:** ASP.NET Core Identity
* **Frontend:** Bootstrap 5.3, jQuery
* **Testing:** xUnit, Moq
* **Security:** AES-256 Encryption

---

## 🎥 Project Demonstration
Watch the full demonstration of the Part 3 features here:
**[https://youtu.be/RUjLIzFlytQ](https://youtu.be/RUjLIzFlytQ)**

---

## 🔑 Test Accounts (Login Credentials)

Please use the following credentials to test the specific roles and automation features:

| Role | Username / Email | Password |
| :--- | :--- | :--- |
| **HR (Super User)** | `hr@test.com` | `Password123` |
| **Academic Manager** | `manager@test.com` | `Password123` |
| **Programme Coordinator** | `coordinator@test.com` | `Password123` |
| **Lecturer** | `lecturer@test.com` | `Password123` |

---

## 🏃 How to Run the Project

1.  **Clone the Repository:**
    ```bash
    git clone [https://github.com/MtamboGoshen/prog6212-part3.git](https://github.com/MtamboGoshen/prog6212-part3.git)
    ```
2.  **Database Setup:**
    * Ensure you have **SQL Server LocalDB** or **SQL Express** installed.
    * Update the connection string in `appsettings.json` if your server name differs from `localhost`.
    * Run the following command in the Package Manager Console or Terminal to create the database and seed the test users:
    ```bash
    dotnet ef database update
    ```
3.  **Run the Application:**
    * Open the solution in Visual Studio 2022.
    * Press **F5** to run.

---

## 📜 Version Control
This project follows professional version control practices with a history of **10+ descriptive commits**, documenting the evolution from Part 2 to the final Part 3 submission.
