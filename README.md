## Setup

### 1. Database
- Mở SQL Server Management Studio
- Chạy file `PRN212-SEAL/Database/001_InitialDatabase.sql`

### 2. Connection String
- Copy file `appsettings.example.json` → đổi tên thành `appsettings.json`
- Điền password SA của máy bạn vào `YOUR_PASSWORD_HERE`
- File này KHÔNG được commit lên GitHub

### 3. Build & Run
- Mở `PRN212-SEAL.slnx` bằng Visual Studio
- Build solution (Ctrl+Shift+B)
- Chạy (F5)
