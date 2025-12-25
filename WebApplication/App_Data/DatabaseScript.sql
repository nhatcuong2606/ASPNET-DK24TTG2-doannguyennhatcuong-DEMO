-- ============================================
-- DATABASE SCRIPT: Quản Lý Chi Tiêu Cá Nhân
-- ============================================

-- Tạo database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ExpenseManagement')
BEGIN
    CREATE DATABASE ExpenseManagement;
END
GO

USE ExpenseManagement;
GO

-- ============================================
-- Bảng Users: Lưu thông tin người dùng
-- ============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE Users (
        UserID INT PRIMARY KEY IDENTITY(1,1),
        Username NVARCHAR(50) NOT NULL UNIQUE,
        Password NVARCHAR(100) NOT NULL,
        FullName NVARCHAR(100),
        Email NVARCHAR(100),
        CreatedDate DATETIME DEFAULT GETDATE()
    );
END
GO

-- ============================================
-- Bảng Categories: Danh mục chi tiêu
-- ============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Categories' AND xtype='U')
BEGIN
    CREATE TABLE Categories (
        CategoryID INT PRIMARY KEY IDENTITY(1,1),
        CategoryName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(255),
        UserID INT NOT NULL,
        FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
    );
END
GO

-- ============================================
-- Bảng Expenses: Các khoản chi tiêu
-- ============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Expenses' AND xtype='U')
BEGIN
    CREATE TABLE Expenses (
        ExpenseID INT PRIMARY KEY IDENTITY(1,1),
        Amount DECIMAL(18,2) NOT NULL,
        ExpenseDate DATE NOT NULL,
        CategoryID INT NOT NULL,
        Note NVARCHAR(500),
        UserID INT NOT NULL,
        CreatedDate DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
        FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
    );
END
GO

-- ============================================
-- Bảng Budgets: Ngân sách theo tháng
-- ============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Budgets' AND xtype='U')
BEGIN
    CREATE TABLE Budgets (
        BudgetID INT PRIMARY KEY IDENTITY(1,1),
        Month INT NOT NULL CHECK (Month >= 1 AND Month <= 12),
        Year INT NOT NULL,
        LimitAmount DECIMAL(18,2) NOT NULL,
        UserID INT NOT NULL,
        FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
        UNIQUE(Month, Year, UserID)
    );
END
GO

-- ============================================
-- Bảng Goals: Mục tiêu tiết kiệm
-- ============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Goals' AND xtype='U')
BEGIN
    CREATE TABLE Goals (
        GoalID INT PRIMARY KEY IDENTITY(1,1),
        GoalName NVARCHAR(200) NOT NULL,
        TargetAmount DECIMAL(18,2) NOT NULL,
        CurrentAmount DECIMAL(18,2) DEFAULT 0,
        Deadline DATE,
        UserID INT NOT NULL,
        CreatedDate DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
    );
END
GO

-- ============================================
-- DỮ LIỆU MẪU
-- ============================================

-- Thêm user mẫu (password: 123456)
IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, Password, FullName, Email)
    VALUES ('admin', '123456', N'Quản trị viên', 'admin@example.com');
END
GO

-- Lấy UserID của admin
DECLARE @AdminUserID INT;
SELECT @AdminUserID = UserID FROM Users WHERE Username = 'admin';

-- Thêm danh mục mẫu
IF NOT EXISTS (SELECT * FROM Categories WHERE UserID = @AdminUserID AND CategoryName = N'Ăn uống')
BEGIN
    INSERT INTO Categories (CategoryName, Description, UserID)
    VALUES 
        (N'Ăn uống', N'Chi phí ăn uống hàng ngày', @AdminUserID),
        (N'Đi lại', N'Chi phí di chuyển, xăng xe', @AdminUserID),
        (N'Học tập', N'Chi phí học phí, sách vở', @AdminUserID),
        (N'Giải trí', N'Chi phí vui chơi, giải trí', @AdminUserID),
        (N'Mua sắm', N'Chi phí mua sắm đồ dùng', @AdminUserID),
        (N'Hóa đơn', N'Điện, nước, internet', @AdminUserID);
END
GO

-- Thêm chi tiêu mẫu
DECLARE @AdminUserID2 INT;
SELECT @AdminUserID2 = UserID FROM Users WHERE Username = 'admin';

DECLARE @CatAnUong INT, @CatDiLai INT, @CatHocTap INT, @CatGiaiTri INT;
SELECT @CatAnUong = CategoryID FROM Categories WHERE CategoryName = N'Ăn uống' AND UserID = @AdminUserID2;
SELECT @CatDiLai = CategoryID FROM Categories WHERE CategoryName = N'Đi lại' AND UserID = @AdminUserID2;
SELECT @CatHocTap = CategoryID FROM Categories WHERE CategoryName = N'Học tập' AND UserID = @AdminUserID2;
SELECT @CatGiaiTri = CategoryID FROM Categories WHERE CategoryName = N'Giải trí' AND UserID = @AdminUserID2;

IF NOT EXISTS (SELECT * FROM Expenses WHERE UserID = @AdminUserID2)
BEGIN
    INSERT INTO Expenses (Amount, ExpenseDate, CategoryID, Note, UserID)
    VALUES 
        (50000, DATEADD(DAY, -1, GETDATE()), @CatAnUong, N'Ăn sáng', @AdminUserID2),
        (100000, DATEADD(DAY, -2, GETDATE()), @CatAnUong, N'Ăn trưa và tối', @AdminUserID2),
        (200000, DATEADD(DAY, -3, GETDATE()), @CatDiLai, N'Đổ xăng', @AdminUserID2),
        (500000, DATEADD(DAY, -5, GETDATE()), @CatHocTap, N'Mua sách', @AdminUserID2),
        (150000, DATEADD(DAY, -7, GETDATE()), @CatGiaiTri, N'Xem phim', @AdminUserID2),
        (75000, DATEADD(DAY, -10, GETDATE()), @CatAnUong, N'Cà phê', @AdminUserID2),
        (300000, DATEADD(DAY, -15, GETDATE()), @CatDiLai, N'Grab', @AdminUserID2);
END
GO

-- Thêm ngân sách mẫu
DECLARE @AdminUserID3 INT;
SELECT @AdminUserID3 = UserID FROM Users WHERE Username = 'admin';

IF NOT EXISTS (SELECT * FROM Budgets WHERE UserID = @AdminUserID3 AND Month = MONTH(GETDATE()) AND Year = YEAR(GETDATE()))
BEGIN
    INSERT INTO Budgets (Month, Year, LimitAmount, UserID)
    VALUES (MONTH(GETDATE()), YEAR(GETDATE()), 5000000, @AdminUserID3);
END
GO

-- Thêm mục tiêu mẫu
DECLARE @AdminUserID4 INT;
SELECT @AdminUserID4 = UserID FROM Users WHERE Username = 'admin';

IF NOT EXISTS (SELECT * FROM Goals WHERE UserID = @AdminUserID4)
BEGIN
    INSERT INTO Goals (GoalName, TargetAmount, CurrentAmount, Deadline, UserID)
    VALUES 
        (N'Mua laptop mới', 15000000, 5000000, DATEADD(MONTH, 6, GETDATE()), @AdminUserID4),
        (N'Du lịch hè', 10000000, 3000000, DATEADD(MONTH, 4, GETDATE()), @AdminUserID4);
END
GO

-- ============================================
-- KIỂM TRA DỮ LIỆU
-- ============================================
SELECT 'Users' AS TableName, COUNT(*) AS RecordCount FROM Users
UNION ALL
SELECT 'Categories', COUNT(*) FROM Categories
UNION ALL
SELECT 'Expenses', COUNT(*) FROM Expenses
UNION ALL
SELECT 'Budgets', COUNT(*) FROM Budgets
UNION ALL
SELECT 'Goals', COUNT(*) FROM Goals;
GO
