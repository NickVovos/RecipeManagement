using Common.DTOs;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace RecipeApi.Data
{
    public class SqlRepository
    {
        private readonly string _connString;
        public SqlRepository(IConfiguration config)
        {
            _connString = config.GetConnectionString("RecipeDb");
            EnsureDatabaseExists();
            EnsureSchemaExists();
        }

        private void EnsureDatabaseExists()
        {
            var builder = new SqlConnectionStringBuilder(_connString);
            var databaseName = builder.InitialCatalog;
            if (string.IsNullOrWhiteSpace(databaseName)) return;

            var masterConnString = new SqlConnectionStringBuilder(_connString)
            {
                InitialCatalog = "master"
            }.ConnectionString;

            using (var conn = new SqlConnection(masterConnString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"IF DB_ID(@db) IS NULL CREATE DATABASE [{databaseName}]";
                cmd.Parameters.AddWithValue("@db", databaseName);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void EnsureSchemaExists()
        {
            using (var conn = new SqlConnection(_connString))
            {
                conn.Open();
                
                ExecuteNonQuery(conn, @"
                    IF OBJECT_ID('dbo.Recipes') IS NULL
                    BEGIN
                        CREATE TABLE dbo.Recipes (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Name NVARCHAR(200) NOT NULL,
                            Category NVARCHAR(100) NOT NULL,
                            Difficulty INT NOT NULL,
                            Description NVARCHAR(MAX) NULL
                        );
                    END");

                ExecuteNonQuery(conn, @"
                    IF OBJECT_ID('dbo.RecipeImages') IS NULL
                    BEGIN
                        CREATE TABLE dbo.RecipeImages (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            RecipeId INT NOT NULL FOREIGN KEY REFERENCES dbo.Recipes(Id) ON DELETE CASCADE,
                            Name NVARCHAR(255) NOT NULL,
                            Data VARBINARY(MAX) NULL
                        );
                    END");

                ExecuteNonQuery(conn, @"
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
                    END");

                ExecuteNonQuery(conn, @"
                    IF OBJECT_ID('dbo.StepIngredients') IS NULL
                    BEGIN
                        CREATE TABLE dbo.StepIngredients (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            StepId INT NOT NULL FOREIGN KEY REFERENCES dbo.Steps(Id) ON DELETE CASCADE,
                            Quantity NVARCHAR(100) NOT NULL,
                            Name NVARCHAR(200) NOT NULL
                        );
                    END");

                ExecuteNonQuery(conn, @"
                    IF OBJECT_ID('dbo.StepImages') IS NULL
                    BEGIN
                        CREATE TABLE dbo.StepImages (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            StepId INT NOT NULL FOREIGN KEY REFERENCES dbo.Steps(Id) ON DELETE CASCADE,
                            Name NVARCHAR(255) NOT NULL,
                            Data VARBINARY(MAX) NULL
                        );
                    END");

                CreateStoredProcedures(conn);
            }
        }

        private void CreateStoredProcedures(SqlConnection conn)
        {
            ExecuteNonQuery(conn, @"
                IF OBJECT_ID('dbo.spCreateRecipe') IS NULL
                EXEC ('CREATE PROCEDURE dbo.spCreateRecipe
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
                END')");

            ExecuteNonQuery(conn, @"
                IF OBJECT_ID('dbo.spUpdateRecipe') IS NULL
                EXEC ('CREATE PROCEDURE dbo.spUpdateRecipe
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
                END')");

            ExecuteNonQuery(conn, @"
                IF OBJECT_ID('dbo.spGetRecipe') IS NULL
                EXEC ('CREATE PROCEDURE dbo.spGetRecipe
                    @RecipeId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT Id, Name, Category, Difficulty, Description
                    FROM dbo.Recipes
                    WHERE Id=@RecipeId;
                END')");

            ExecuteNonQuery(conn, @"
                IF OBJECT_ID('dbo.spGetAllRecipes') IS NULL
                EXEC ('CREATE PROCEDURE dbo.spGetAllRecipes
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT Id, Name, Category, Difficulty, Description
                    FROM dbo.Recipes;
                END')");

            ExecuteNonQuery(conn, @"
                IF OBJECT_ID('dbo.spDeleteRecipe') IS NULL
                EXEC ('CREATE PROCEDURE dbo.spDeleteRecipe
                    @RecipeId INT
                AS
                BEGIN
                    SET NOCOUNT OFF;
                    DELETE FROM dbo.Recipes WHERE Id=@RecipeId;
                END')");

            ExecuteNonQuery(conn, @"
                IF OBJECT_ID('dbo.spAddRecipeImage') IS NULL
                EXEC ('CREATE PROCEDURE dbo.spAddRecipeImage
                    @RecipeId INT,
                    @Name NVARCHAR(255),
                    @Data VARBINARY(MAX) NULL
                AS
                BEGIN
                    SET NOCOUNT ON;
                    INSERT INTO dbo.RecipeImages(RecipeId, Name, Data)
                    VALUES(@RecipeId, @Name, @Data);
                END')");

            ExecuteNonQuery(conn, @"
                IF OBJECT_ID('dbo.spDeleteRecipeImages') IS NULL
                EXEC ('CREATE PROCEDURE dbo.spDeleteRecipeImages
                    @RecipeId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    DELETE FROM dbo.RecipeImages WHERE RecipeId=@RecipeId;
                END')");

            ExecuteNonQuery(conn, @"
                IF OBJECT_ID('dbo.spGetRecipeImages') IS NULL
                EXEC ('CREATE PROCEDURE dbo.spGetRecipeImages
                    @RecipeId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT Id, Name, Data
                    FROM dbo.RecipeImages
                    WHERE RecipeId=@RecipeId;
                END')");

            ExecuteNonQuery(conn, @"
                IF OBJECT_ID('dbo.spAddStep') IS NULL
                EXEC ('CREATE PROCEDURE dbo.spAddStep
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
                END')");

            ExecuteNonQuery(conn, @"
                IF OBJECT_ID('dbo.spDeleteRecipeSteps') IS NULL
                EXEC ('CREATE PROCEDURE dbo.spDeleteRecipeSteps
                    @RecipeId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    DELETE FROM dbo.Steps WHERE RecipeId=@RecipeId;
                END')");

            ExecuteNonQuery(conn, @"
                IF OBJECT_ID('dbo.spGetRecipeSteps') IS NULL
                EXEC ('CREATE PROCEDURE dbo.spGetRecipeSteps
                    @RecipeId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT Id, Title, Description, [Order], Duration
                    FROM dbo.Steps
                    WHERE RecipeId=@RecipeId
                    ORDER BY [Order];
                END')");

            ExecuteNonQuery(conn, @"
                IF OBJECT_ID('dbo.spAddStepIngredient') IS NULL
                EXEC ('CREATE PROCEDURE dbo.spAddStepIngredient
                    @StepId INT,
                    @Name NVARCHAR(200),
                    @Quantity NVARCHAR(100)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    INSERT INTO dbo.StepIngredients(StepId, Name, Quantity)
                    VALUES(@StepId, @Name, @Quantity);
                END')");

            ExecuteNonQuery(conn, @"
                IF OBJECT_ID('dbo.spGetStepIngredients') IS NULL
                EXEC ('CREATE PROCEDURE dbo.spGetStepIngredients
                    @StepId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT Quantity, Name
                    FROM dbo.StepIngredients
                    WHERE StepId=@StepId;
                END')");

            ExecuteNonQuery(conn, @"
                IF OBJECT_ID('dbo.spAddStepImage') IS NULL
                EXEC ('CREATE PROCEDURE dbo.spAddStepImage
                    @StepId INT,
                    @Name NVARCHAR(255),
                    @Data VARBINARY(MAX) NULL
                AS
                BEGIN
                    SET NOCOUNT ON;
                    INSERT INTO dbo.StepImages(StepId, Name, Data)
                    VALUES(@StepId, @Name, @Data);
                END')");

            ExecuteNonQuery(conn, @"
                IF OBJECT_ID('dbo.spGetStepImages') IS NULL
                EXEC ('CREATE PROCEDURE dbo.spGetStepImages
                    @StepId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT Id, Name, Data
                    FROM dbo.StepImages
                    WHERE StepId=@StepId;
                END')");
        }

        private void ExecuteNonQuery(SqlConnection conn, string sql)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        public bool DeleteRecipe(int id)
        {
            using (var conn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand("dbo.spDeleteRecipe", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RecipeId", id);
                conn.Open();
                var rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }

        public int CreateRecipe(RecipeDto recipe)
        {
            using (var conn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand("dbo.spCreateRecipe", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", recipe.Name);
                cmd.Parameters.AddWithValue("@Category", recipe.CategoryName);
                cmd.Parameters.AddWithValue("@Difficulty", (int)recipe.Difficulty);
                cmd.Parameters.AddWithValue("@Description", (object)recipe.Description ?? DBNull.Value);
                var idParam = new SqlParameter("@RecipeId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(idParam);
                conn.Open();
                cmd.ExecuteNonQuery();
                recipe.Id = (int)idParam.Value;
            }
            SaveRecipeImages(recipe);
            SaveSteps(recipe);
            return recipe.Id;
        }

        public void UpdateRecipe(RecipeDto recipe)
        {
            using (var conn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand("dbo.spUpdateRecipe", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RecipeId", recipe.Id);
                cmd.Parameters.AddWithValue("@Name", recipe.Name);
                cmd.Parameters.AddWithValue("@Category", recipe.CategoryName);
                cmd.Parameters.AddWithValue("@Difficulty", (int)recipe.Difficulty);
                cmd.Parameters.AddWithValue("@Description", (object)recipe.Description ?? DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            DeleteRecipeImages(recipe.Id);
            SaveRecipeImages(recipe);
            DeleteRecipeSteps(recipe.Id);
            SaveSteps(recipe);
        }

        public RecipeDto GetRecipe(int id)
        {
            var recipe = new RecipeDto();
            using (var conn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand("dbo.spGetRecipe", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RecipeId", id);
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        recipe.Id = rdr.GetInt32(rdr.GetOrdinal("Id"));
                        recipe.Name = rdr.GetString(rdr.GetOrdinal("Name"));
                        recipe.CategoryName = rdr.GetString(rdr.GetOrdinal("Category"));
                        recipe.Difficulty = (Difficulty)rdr.GetInt32(rdr.GetOrdinal("Difficulty"));
                        recipe.Description = rdr.IsDBNull(rdr.GetOrdinal("Description")) ? null : rdr.GetString(rdr.GetOrdinal("Description"));
                    }
                }
            }
            recipe.Images = GetRecipeImages(id);
            recipe.Steps = GetRecipeSteps(id);
            return recipe;
        }

        public List<RecipeDto> GetAllRecipes()
        {
            var list = new List<RecipeDto>();
            using (var conn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand("dbo.spGetAllRecipes", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        list.Add(new RecipeDto
                        {
                            Id = rdr.GetInt32(rdr.GetOrdinal("Id")),
                            Name = rdr.GetString(rdr.GetOrdinal("Name")),
                            CategoryName = rdr.GetString(rdr.GetOrdinal("Category")),
                            Difficulty = (Difficulty)rdr.GetInt32(rdr.GetOrdinal("Difficulty")),
                            Description = rdr.IsDBNull(rdr.GetOrdinal("Description")) ? null : rdr.GetString(rdr.GetOrdinal("Description"))
                        });
                    }
                }
            }
            foreach (var recipe in list)
            {
                recipe.Images = GetRecipeImagesWithoutData(recipe.Id);
                recipe.Steps = GetRecipeStepsWithoutImages(recipe.Id);
            }
            return list;
        }

        private List<ImageDto> GetRecipeImagesWithoutData(int recipeId)
        {
            var list = new List<ImageDto>();
            using (var conn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand("SELECT Id, Name FROM dbo.RecipeImages WHERE RecipeId=@RecipeId", conn))
            {
                cmd.Parameters.AddWithValue("@RecipeId", recipeId);
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        list.Add(new ImageDto
                        {
                            Id = rdr.GetInt32(rdr.GetOrdinal("Id")),
                            Name = rdr.GetString(rdr.GetOrdinal("Name")),
                            Data = null
                        });
                    }
                }
            }
            return list;
        }

        private List<StepDto> GetRecipeStepsWithoutImages(int recipeId)
        {
            var steps = new List<StepDto>();
            using (var conn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand("dbo.spGetRecipeSteps", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RecipeId", recipeId);
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        steps.Add(new StepDto
                        {
                            Id = rdr.GetInt32(rdr.GetOrdinal("Id")),
                            Title = rdr.GetString(rdr.GetOrdinal("Title")),
                            Description = rdr.IsDBNull(rdr.GetOrdinal("Description")) ? null : rdr.GetString(rdr.GetOrdinal("Description")),
                            Order = rdr.GetInt32(rdr.GetOrdinal("Order")),
                            Duration = rdr.GetInt32(rdr.GetOrdinal("Duration"))
                        });
                    }
                }
            }
            foreach (var step in steps)
            {
                step.Ingredients = GetStepIngredients(step.Id);
                step.Images = GetStepImagesWithoutData(step.Id);
            }
            return steps;
        }

        private List<ImageDto> GetStepImagesWithoutData(int stepId)
        {
            var list = new List<ImageDto>();
            using (var conn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand("SELECT Id, Name FROM dbo.StepImages WHERE StepId=@StepId", conn))
            {
                cmd.Parameters.AddWithValue("@StepId", stepId);
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        list.Add(new ImageDto
                        {
                            Id = rdr.GetInt32(rdr.GetOrdinal("Id")),
                            Name = rdr.GetString(rdr.GetOrdinal("Name")),
                            Data = null
                        });
                    }
                }
            }
            return list;
        }

        private void SaveRecipeImages(RecipeDto recipe)
        {
            if (recipe.Images == null) return;
            foreach (var img in recipe.Images)
            {
                using (var conn = new SqlConnection(_connString))
                using (var cmd = new SqlCommand("dbo.spAddRecipeImage", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RecipeId", recipe.Id);
                    cmd.Parameters.AddWithValue("@Name", img.Name);
                    cmd.Parameters.Add("@Data", SqlDbType.VarBinary, img.Data?.Length ?? 0).Value = (object)img.Data ?? DBNull.Value;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void SaveSteps(RecipeDto recipe)
        {
            if (recipe.Steps == null) return;
            foreach (var s in recipe.Steps)
            {
                int stepId;
                using (var conn = new SqlConnection(_connString))
                using (var cmd = new SqlCommand("dbo.spAddStep", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RecipeId", recipe.Id);
                    cmd.Parameters.AddWithValue("@Title", s.Title);
                    cmd.Parameters.AddWithValue("@Description", (object)s.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Order", s.Order);
                    cmd.Parameters.AddWithValue("@Duration", s.Duration);
                    var idParam = new SqlParameter("@StepId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(idParam);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    stepId = (int)idParam.Value;
                }
                if (s.Ingredients != null)
                {
                    foreach (var ing in s.Ingredients)
                    {
                        using (var conn = new SqlConnection(_connString))
                        using (var cmd = new SqlCommand("dbo.spAddStepIngredient", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@StepId", stepId);
                            cmd.Parameters.AddWithValue("@Name", ing.Name);
                            cmd.Parameters.AddWithValue("@Quantity", ing.Quantity);
                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                if (s.Images != null)
                {
                    foreach (var img in s.Images)
                    {
                        using (var conn = new SqlConnection(_connString))
                        using (var cmd = new SqlCommand("dbo.spAddStepImage", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@StepId", stepId);
                            cmd.Parameters.AddWithValue("@Name", img.Name);
                            cmd.Parameters.Add("@Data", SqlDbType.VarBinary, img.Data?.Length ?? 0).Value = (object)img.Data ?? DBNull.Value;
                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private void DeleteRecipeImages(int recipeId)
        {
            using (var conn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand("dbo.spDeleteRecipeImages", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RecipeId", recipeId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void DeleteRecipeSteps(int recipeId)
        {
            using (var conn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand("dbo.spDeleteRecipeSteps", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RecipeId", recipeId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private List<ImageDto> GetRecipeImages(int recipeId)
        {
            var list = new List<ImageDto>();
            using (var conn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand("dbo.spGetRecipeImages", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RecipeId", recipeId);
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        list.Add(new ImageDto
                        {
                            Id = rdr.GetInt32(rdr.GetOrdinal("Id")),
                            Name = rdr.GetString(rdr.GetOrdinal("Name")),
                            Data = rdr.IsDBNull(rdr.GetOrdinal("Data")) ? null : (byte[])rdr["Data"]
                        });
                    }
                }
            }
            return list;
        }

        private List<StepDto> GetRecipeSteps(int recipeId)
        {
            var steps = new List<StepDto>();
            using (var conn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand("dbo.spGetRecipeSteps", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RecipeId", recipeId);
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        steps.Add(new StepDto
                        {
                            Id = rdr.GetInt32(rdr.GetOrdinal("Id")),
                            Title = rdr.GetString(rdr.GetOrdinal("Title")),
                            Description = rdr.IsDBNull(rdr.GetOrdinal("Description")) ? null : rdr.GetString(rdr.GetOrdinal("Description")),
                            Order = rdr.GetInt32(rdr.GetOrdinal("Order")),
                            Duration = rdr.GetInt32(rdr.GetOrdinal("Duration"))
                        });
                    }
                }
            }
            foreach (var step in steps)
            {
                step.Ingredients = GetStepIngredients(step.Id);
                step.Images = GetStepImages(step.Id);
            }
            return steps;
        }

        private List<StepIngredientDto> GetStepIngredients(int stepId)
        {
            var list = new List<StepIngredientDto>();
            using (var conn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand("dbo.spGetStepIngredients", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StepId", stepId);
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        list.Add(new StepIngredientDto
                        {
                            Quantity = rdr.GetString(rdr.GetOrdinal("Quantity")),
                            Name = rdr.GetString(rdr.GetOrdinal("Name"))
                        });
                    }
                }
            }
            return list;
        }

        private List<ImageDto> GetStepImages(int stepId)
        {
            var list = new List<ImageDto>();
            using (var conn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand("dbo.spGetStepImages", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StepId", stepId);
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        list.Add(new ImageDto
                        {
                            Id = rdr.GetInt32(rdr.GetOrdinal("Id")),
                            Name = rdr.GetString(rdr.GetOrdinal("Name")),
                            Data = rdr.IsDBNull(rdr.GetOrdinal("Data")) ? null : (byte[])rdr["Data"]
                        });
                    }
                }
            }
            return list;
        }
    }
}
