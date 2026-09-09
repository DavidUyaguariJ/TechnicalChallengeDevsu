/* =========================================================
   MICROSERVICE ARCHITECTURE
   =========================================================

/* =========================================================
   DATABASE 1: CustomersDB
   Microservice: Customer / Person
   ========================================================= */

IF DB_ID(N'CustomersDB') IS NULL
BEGIN
    CREATE DATABASE CustomersDB;
END;
GO

USE CustomersDB;
GO

/* Drop table if it already exists */
IF OBJECT_ID(N'dbo.Customers', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Customers;
END;
GO


/* =========================================================
   TABLE: Customers
   ========================================================= */

CREATE TABLE dbo.Customers
(
    client_id       BIGINT IDENTITY(1,1) NOT NULL,
    name            NVARCHAR(100) NOT NULL,
    gender          NVARCHAR(20) NOT NULL,
    age             INT NOT NULL,
    identification  VARCHAR(20) NOT NULL,
    address         NVARCHAR(200) NOT NULL,
    phone           VARCHAR(20) NOT NULL,
    password        VARCHAR(100) NOT NULL,
    status          BIT NOT NULL,

    CONSTRAINT PK_Customers
        PRIMARY KEY (client_id),

    CONSTRAINT UQ_Customers_Identification
        UNIQUE (identification)
);
GO


/* =========================================================
   SAMPLE DATA: Customers
   ========================================================= */

INSERT INTO dbo.Customers
(
    name,
    address,
    age,
    gender,
    identification,
    phone,
    password,
    status
)
VALUES
(
    'Jose Lema',
    'Otavalo sn y principal',
    35,
    'Male',
    '0102030405',
    '098254785',
    '1234',
    1
),
(
    'Marianela Montalvo',
    'Amazonas y NNUU',
    32,
    'Female',
    '0203040506',
    '097548965',
    '5678',
    1
),
(
    'Juan Osorio',
    '13 de junio y Equinoccial',
    40,
    'Male',
    '0304050607',
    '098874587',
    '1245',
    1
);
GO


/* =========================================================
   DATABASE 2: AccountsDB
   Microservice: Account / Movement
   ========================================================= */

IF DB_ID(N'AccountsDB') IS NULL
BEGIN
    CREATE DATABASE AccountsDB;
END;
GO

USE AccountsDB;
GO


/* =========================================================
   Drop existing tables
   ========================================================= */

IF OBJECT_ID(N'dbo.Movements', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Movements;
END;
GO

IF OBJECT_ID(N'dbo.Accounts', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Accounts;
END;
GO


/* =========================================================
   TABLE: Accounts
   ========================================================= */

CREATE TABLE dbo.Accounts
(
    account_id      BIGINT IDENTITY(1,1) NOT NULL,
    account_number  VARCHAR(20) NOT NULL,
    account_type    VARCHAR(20) NOT NULL,
    initial_balance DECIMAL(15,2) NOT NULL,
    status          BIT NOT NULL,
    customer_id     BIGINT NOT NULL,

    CONSTRAINT PK_Accounts
        PRIMARY KEY (account_id),

    CONSTRAINT UQ_Accounts_AccountNumber
        UNIQUE (account_number)
);
GO


/* =========================================================
   Index for customer_id
   ========================================================= */

CREATE INDEX IX_Accounts_CustomerId
    ON dbo.Accounts(customer_id);
GO


/* =========================================================
   TABLE: Movements
   ========================================================= */

CREATE TABLE dbo.Movements
(
    movement_id    BIGINT IDENTITY(1,1) NOT NULL,
    movement_date  DATETIME2 NOT NULL,
    movement_type  VARCHAR(20) NOT NULL,
    value          DECIMAL(15,2) NOT NULL,
    balance        DECIMAL(15,2) NOT NULL,
    account_id     BIGINT NOT NULL,

    CONSTRAINT PK_Movements
        PRIMARY KEY (movement_id),

    CONSTRAINT FK_Movements_Accounts
        FOREIGN KEY (account_id)
        REFERENCES dbo.Accounts(account_id)
);
GO


/* =========================================================
   SAMPLE DATA: Accounts
   ========================================================= */

INSERT INTO dbo.Accounts
(
    account_number,
    account_type,
    customer_id,
    initial_balance,
    status
)
VALUES
(
    '478758',
    'Savings',
    1,
    2000.00,
    1
),
(
    '225487',
    'Checking',
    2,
    100.00,
    1
),
(
    '495878',
    'Savings',
    3,
    0.00,
    1
),
(
    '496825',
    'Savings',
    2,
    540.00,
    1
),
(
    '585545',
    'Checking',
    1,
    1000.00,
    1
);
GO


/* =========================================================
   SAMPLE DATA: Movements
   ========================================================= */

INSERT INTO dbo.Movements
(
    value,
    movement_date,
    movement_type,
    balance,
    account_id
)
VALUES
(
    -575.00,
    '2026-01-22T23:14:34.5610922',
    'WITHDRAWAL',
    1425.00,
    1
),
(
    600.00,
    '2026-01-23T00:15:37.4702866',
    'DEPOSIT',
    700.00,
    2
),
(
    150.00,
    '2026-01-23T00:15:42.2188839',
    'DEPOSIT',
    150.00,
    3
),
(
    -540.00,
    '2026-01-23T00:15:46.1836925',
    'WITHDRAWAL',
    0.00,
    4
);
GO
