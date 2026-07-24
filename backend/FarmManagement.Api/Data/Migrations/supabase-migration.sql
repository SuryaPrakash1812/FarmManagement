CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE TABLE "ActivityLogs" (
        "Id" uuid NOT NULL,
        "Actor" text NOT NULL,
        "Action" text NOT NULL,
        "EntityName" text NOT NULL,
        "EntityId" uuid,
        "Details" text,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_ActivityLogs" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE TABLE "Animals" (
        "Id" uuid NOT NULL,
        "AnimalCode" text NOT NULL,
        "TagNumber" text NOT NULL,
        "Name" text NOT NULL,
        "Species" text NOT NULL,
        "Breed" text NOT NULL,
        "Gender" integer NOT NULL,
        "DateOfBirth" date,
        "PurchaseDate" date,
        "PurchasePrice" numeric NOT NULL,
        "CurrentValue" numeric NOT NULL,
        "Weight" numeric,
        "HealthStatus" text NOT NULL,
        "VaccinationDetails" text,
        "MedicalHistory" text,
        "IsPregnant" boolean NOT NULL,
        "FatherId" uuid,
        "MotherId" uuid,
        "PhotoUrl" text,
        "Notes" text,
        "Status" integer NOT NULL,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Animals" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Animals_Animals_FatherId" FOREIGN KEY ("FatherId") REFERENCES "Animals" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Animals_Animals_MotherId" FOREIGN KEY ("MotherId") REFERENCES "Animals" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE TABLE "AuditLogs" (
        "Id" uuid NOT NULL,
        "EntityName" text NOT NULL,
        "EntityId" uuid NOT NULL,
        "Action" text NOT NULL,
        "ChangedBy" text,
        "Changes" text,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_AuditLogs" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE TABLE "Customers" (
        "Id" uuid NOT NULL,
        "Name" text NOT NULL,
        "Phone" text,
        "Email" text,
        "Address" text,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Customers" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE TABLE "Employees" (
        "Id" uuid NOT NULL,
        "FullName" text NOT NULL,
        "Role" text NOT NULL,
        "Salary" numeric NOT NULL,
        "Phone" text,
        "Address" text,
        "Tasks" text,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Employees" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE TABLE "Expenses" (
        "Id" uuid NOT NULL,
        "Category" text NOT NULL,
        "Amount" numeric NOT NULL,
        "PaymentMethod" text NOT NULL,
        "Date" date NOT NULL,
        "Notes" text,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Expenses" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE TABLE "FarmSettings" (
        "Id" uuid NOT NULL,
        "FarmName" text NOT NULL,
        "Currency" text NOT NULL,
        "LogoUrl" text,
        "EmailFrom" text,
        "EnableNotifications" boolean NOT NULL,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_FarmSettings" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE TABLE "Incomes" (
        "Id" uuid NOT NULL,
        "Source" text NOT NULL,
        "Amount" numeric NOT NULL,
        "Date" date NOT NULL,
        "Notes" text,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Incomes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE TABLE "Investments" (
        "Id" uuid NOT NULL,
        "InvestmentType" text NOT NULL,
        "Amount" numeric NOT NULL,
        "Date" date NOT NULL,
        "Description" text,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Investments" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE TABLE "Payments" (
        "Id" uuid NOT NULL,
        "Direction" integer NOT NULL,
        "Amount" numeric NOT NULL,
        "Status" integer NOT NULL,
        "Method" text NOT NULL,
        "TransactionReference" text,
        "DueDate" date NOT NULL,
        "PaidDate" date,
        "PartyName" text,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Payments" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE TABLE "StockItems" (
        "Id" uuid NOT NULL,
        "ItemName" text NOT NULL,
        "Category" text NOT NULL,
        "Quantity" numeric NOT NULL,
        "Unit" text NOT NULL,
        "Cost" numeric NOT NULL,
        "Supplier" text,
        "PurchaseDate" date NOT NULL,
        "ExpiryDate" date,
        "ReorderLevel" numeric NOT NULL,
        "Barcode" text,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_StockItems" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE TABLE "Users" (
        "Id" uuid NOT NULL,
        "FullName" text NOT NULL,
        "Email" text NOT NULL,
        "PasswordHash" text NOT NULL,
        "Role" integer NOT NULL,
        "Phone" text,
        "AvatarUrl" text,
        "IsActive" boolean NOT NULL,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE TABLE "Vendors" (
        "Id" uuid NOT NULL,
        "Name" text NOT NULL,
        "Phone" text,
        "Email" text,
        "Address" text,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Vendors" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE TABLE "BreedingRecords" (
        "Id" uuid NOT NULL,
        "MaleAnimalId" uuid NOT NULL,
        "FemaleAnimalId" uuid NOT NULL,
        "MatingDate" date NOT NULL,
        "ExpectedDeliveryDate" date,
        "DeliveryDate" date,
        "NewbornDetails" text,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_BreedingRecords" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_BreedingRecords_Animals_FemaleAnimalId" FOREIGN KEY ("FemaleAnimalId") REFERENCES "Animals" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_BreedingRecords_Animals_MaleAnimalId" FOREIGN KEY ("MaleAnimalId") REFERENCES "Animals" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE TABLE "HealthRecords" (
        "Id" uuid NOT NULL,
        "AnimalId" uuid NOT NULL,
        "RecordType" text NOT NULL,
        "Date" date NOT NULL,
        "NextDueDate" date,
        "DoctorName" text,
        "Medicines" text,
        "Diagnosis" text,
        "Notes" text,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_HealthRecords" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HealthRecords_Animals_AnimalId" FOREIGN KEY ("AnimalId") REFERENCES "Animals" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE TABLE "Sales" (
        "Id" uuid NOT NULL,
        "CustomerId" uuid,
        "ProductType" text NOT NULL,
        "ProductName" text NOT NULL,
        "Quantity" numeric NOT NULL,
        "Amount" numeric NOT NULL,
        "Gst" numeric NOT NULL,
        "Discount" numeric NOT NULL,
        "PaymentStatus" integer NOT NULL,
        "InvoiceNumber" text NOT NULL,
        "Date" date NOT NULL,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Sales" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Sales_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE TABLE "AttendanceRecords" (
        "Id" uuid NOT NULL,
        "EmployeeId" uuid NOT NULL,
        "Date" date NOT NULL,
        "IsPresent" boolean NOT NULL,
        "Notes" text,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_AttendanceRecords" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_AttendanceRecords_Employees_EmployeeId" FOREIGN KEY ("EmployeeId") REFERENCES "Employees" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE TABLE "StockMovements" (
        "Id" uuid NOT NULL,
        "StockItemId" uuid NOT NULL,
        "MovementType" text NOT NULL,
        "Quantity" numeric NOT NULL,
        "Reference" text,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_StockMovements" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_StockMovements_StockItems_StockItemId" FOREIGN KEY ("StockItemId") REFERENCES "StockItems" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE TABLE "Purchases" (
        "Id" uuid NOT NULL,
        "VendorId" uuid,
        "ItemName" text NOT NULL,
        "Quantity" numeric NOT NULL,
        "Cost" numeric NOT NULL,
        "PaymentMethod" text NOT NULL,
        "InvoiceUrl" text,
        "PurchaseDate" date NOT NULL,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Purchases" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Purchases_Vendors_VendorId" FOREIGN KEY ("VendorId") REFERENCES "Vendors" ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE INDEX "IX_Animals_FatherId" ON "Animals" ("FatherId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE INDEX "IX_Animals_MotherId" ON "Animals" ("MotherId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_Animals_TagNumber" ON "Animals" ("TagNumber");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE INDEX "IX_AttendanceRecords_EmployeeId" ON "AttendanceRecords" ("EmployeeId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE INDEX "IX_BreedingRecords_FemaleAnimalId" ON "BreedingRecords" ("FemaleAnimalId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE INDEX "IX_BreedingRecords_MaleAnimalId" ON "BreedingRecords" ("MaleAnimalId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE INDEX "IX_HealthRecords_AnimalId" ON "HealthRecords" ("AnimalId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE INDEX "IX_Purchases_VendorId" ON "Purchases" ("VendorId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE INDEX "IX_Sales_CustomerId" ON "Sales" ("CustomerId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE INDEX "IX_StockMovements_StockItemId" ON "StockMovements" ("StockItemId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724060258_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260724060258_InitialCreate', '8.0.10');
    END IF;
END $EF$;
COMMIT;

