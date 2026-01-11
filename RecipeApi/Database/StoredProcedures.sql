-- Database and tables
IF DB_ID('RecipeDb') IS NULL
BEGIN
    CREATE DATABASE RecipeDb;
END
GO

USE RecipeDb;
GO

IF OBJECT_ID('dbo.Recipes') IS NULL
BEGIN
    CREATE TABLE dbo.Recipes (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        Category NVARCHAR(100) NOT NULL,
        Difficulty INT NOT NULL,
        Description NVARCHAR(MAX) NULL
    );
END
GO

IF OBJECT_ID('dbo.RecipeImages') IS NULL
BEGIN
    CREATE TABLE dbo.RecipeImages (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        RecipeId INT NOT NULL FOREIGN KEY REFERENCES dbo.Recipes(Id) ON DELETE CASCADE,
        Name NVARCHAR(255) NOT NULL,
        Data VARBINARY(MAX) NULL
    );
END
GO

IF OBJECT_ID('dbo.Steps') IS NULL
BEGIN
    CREATE TABLE dbo.Steps (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        RecipeId INT NOT NULL FOREIGN KEY REFERENCES dbo.Recipes(Id) ON DELETE CASCADE,
        Title NVARCHAR(200) NOT NULL,
        Description NVARCHAR(MAX) NULL,
        [Order] INT NOT NULL,
        Duration INT NOT NULL
    );
END
GO

IF OBJECT_ID('dbo.StepIngredients') IS NULL
BEGIN
    CREATE TABLE dbo.StepIngredients (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        StepId INT NOT NULL FOREIGN KEY REFERENCES dbo.Steps(Id) ON DELETE CASCADE,
        Quantity NVARCHAR(100) NOT NULL,
        Name NVARCHAR(200) NOT NULL
    );
END
GO

IF OBJECT_ID('dbo.StepImages') IS NULL
BEGIN
    CREATE TABLE dbo.StepImages (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        StepId INT NOT NULL FOREIGN KEY REFERENCES dbo.Steps(Id) ON DELETE CASCADE,
        Name NVARCHAR(255) NOT NULL,
        Data VARBINARY(MAX) NULL
    );
END
GO

-- Stored procedures
IF OBJECT_ID('dbo.spCreateRecipe') IS NULL
EXEC ('
CREATE PROCEDURE dbo.spCreateRecipe
    @Name NVARCHAR(200),
    @Category NVARCHAR(100),
    @Difficulty INT,
    @Description NVARCHAR(MAX) NULL,
    @RecipeId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Recipes(Name, Category, Difficulty, Description)
    VALUES(@Name, @Category, @Difficulty, @Description);
    SET @RecipeId = SCOPE_IDENTITY();
END');
GO

IF OBJECT_ID('dbo.spUpdateRecipe') IS NULL
EXEC ('
CREATE PROCEDURE dbo.spUpdateRecipe
    @RecipeId INT,
    @Name NVARCHAR(200),
    @Category NVARCHAR(100),
    @Difficulty INT,
    @Description NVARCHAR(MAX) NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Recipes
    SET Name=@Name, Category=@Category, Difficulty=@Difficulty, Description=@Description
    WHERE Id=@RecipeId;
END');
GO

IF OBJECT_ID('dbo.spGetRecipe') IS NULL
EXEC ('
CREATE PROCEDURE dbo.spGetRecipe
    @RecipeId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Category, Difficulty, Description
    FROM dbo.Recipes
    WHERE Id=@RecipeId;
END');
GO

IF OBJECT_ID('dbo.spGetAllRecipes') IS NULL
EXEC ('
CREATE PROCEDURE dbo.spGetAllRecipes
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Category, Difficulty, Description
    FROM dbo.Recipes;
END');
GO

IF OBJECT_ID('dbo.spAddRecipeImage') IS NULL
EXEC ('
CREATE PROCEDURE dbo.spAddRecipeImage
    @RecipeId INT,
    @Name NVARCHAR(255),
    @Data VARBINARY(MAX) NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.RecipeImages(RecipeId, Name, Data)
    VALUES(@RecipeId, @Name, @Data);
END');
GO

IF OBJECT_ID('dbo.spDeleteRecipeImages') IS NULL
EXEC ('
CREATE PROCEDURE dbo.spDeleteRecipeImages
    @RecipeId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.RecipeImages WHERE RecipeId=@RecipeId;
END');
GO

IF OBJECT_ID('dbo.spAddStep') IS NULL
EXEC ('
CREATE PROCEDURE dbo.spAddStep
    @RecipeId INT,
    @Title NVARCHAR(200),
    @Description NVARCHAR(MAX) NULL,
    @Order INT,
    @Duration INT,
    @StepId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Steps(RecipeId, Title, Description, [Order], Duration)
    VALUES(@RecipeId, @Title, @Description, @Order, @Duration);
    SET @StepId = SCOPE_IDENTITY();
END');
GO

IF OBJECT_ID('dbo.spDeleteRecipeSteps') IS NULL
EXEC ('
CREATE PROCEDURE dbo.spDeleteRecipeSteps
    @RecipeId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Steps WHERE RecipeId=@RecipeId;
END');
GO

IF OBJECT_ID('dbo.spAddStepIngredient') IS NULL
EXEC ('
CREATE PROCEDURE dbo.spAddStepIngredient
    @StepId INT,
    @Name NVARCHAR(200),
    @Quantity NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.StepIngredients(StepId, Name, Quantity)
    VALUES(@StepId, @Name, @Quantity);
END');
GO

IF OBJECT_ID('dbo.spGetRecipeSteps') IS NULL
EXEC ('
CREATE PROCEDURE dbo.spGetRecipeSteps
    @RecipeId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Title, Description, [Order], Duration
    FROM dbo.Steps
    WHERE RecipeId=@RecipeId
    ORDER BY [Order];
END');
GO

IF OBJECT_ID('dbo.spGetStepIngredients') IS NULL
EXEC ('
CREATE PROCEDURE dbo.spGetStepIngredients
    @StepId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Quantity, Name
    FROM dbo.StepIngredients
    WHERE StepId=@StepId;
END');
GO

IF OBJECT_ID('dbo.spAddStepImage') IS NULL
EXEC ('
CREATE PROCEDURE dbo.spAddStepImage
    @StepId INT,
    @Name NVARCHAR(255),
    @Data VARBINARY(MAX) NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.StepImages(StepId, Name, Data)
    VALUES(@StepId, @Name, @Data);
END');
GO

IF OBJECT_ID('dbo.spGetRecipeImages') IS NULL
EXEC ('
CREATE PROCEDURE dbo.spGetRecipeImages
    @RecipeId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Data
    FROM dbo.RecipeImages
    WHERE RecipeId=@RecipeId;
END');
GO

IF OBJECT_ID('dbo.spGetStepImages') IS NULL
EXEC ('
CREATE PROCEDURE dbo.spGetStepImages
    @StepId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Data
    FROM dbo.StepImages
    WHERE StepId=@StepId;
END');
GO

-- Add delete recipe stored procedure
IF OBJECT_ID('dbo.spDeleteRecipe') IS NULL
EXEC ('
CREATE PROCEDURE dbo.spDeleteRecipe
    @RecipeId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Recipes WHERE Id=@RecipeId;
END');
GO
