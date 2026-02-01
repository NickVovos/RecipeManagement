# Code Examples - Using the New Architecture

This guide shows practical examples of how to work with the refactored codebase.

---

## 📚 Table of Contents
1. [Working with Domain Models](#working-with-domain-models)
2. [Using Services](#using-services)
3. [Extending the Architecture](#extending-the-architecture)
4. [Testing Examples](#testing-examples)
5. [Common Patterns](#common-patterns)

---

## 1. Working with Domain Models

### Creating a Recipe (Information Expert Pattern)

```csharp
using RecipeMan.Models;

// Create a new recipe
var recipe = new Recipe(
    name: "Spaghetti Carbonara",
    category: "Italian",
    difficulty: RecipeDifficulty.Medium,
    description: "Classic Italian pasta dish"
);

// Add steps (domain objects manage their own data)
var step1 = new Step(
    order: 1,
    title: "Boil pasta",
    description: "Bring water to boil and cook spaghetti",
    duration: 10
);

// Add ingredients to step (Information Expert)
step1.Ingredients.Add(new Ingredient("400g", "Spaghetti"));
step1.Ingredients.Add(new Ingredient("2L", "Water"));

recipe.Steps.Add(step1);

// Add second step
var step2 = new Step(2, "Prepare sauce", "Mix eggs and cheese", 5);
step2.Ingredients.Add(new Ingredient("3", "Eggs"));
step2.Ingredients.Add(new Ingredient("100g", "Pecorino cheese"));

recipe.Steps.Add(step2);

// Let recipe validate itself (Information Expert)
var validation = recipe.Validate();
if (!validation.IsValid)
{
    Console.WriteLine($"Validation failed: {validation.Message}");
    return;
}

// Let recipe reorder its steps (Information Expert)
recipe.ReorderSteps();

// Let recipe calculate total time (Information Expert)
int totalMinutes = recipe.CalculateTotalDuration();
Console.WriteLine($"Total cooking time: {totalMinutes} minutes");
```

### Cloning Objects (Inheritance Pattern)

```csharp
// Clone a recipe (uses ICloneable from BaseEntity)
var originalRecipe = new Recipe("Pizza", "Italian", RecipeDifficulty.Easy, "Yum");
var clonedRecipe = (Recipe)originalRecipe.Clone();

// Modify clone without affecting original
clonedRecipe.Name = "Deep Dish Pizza";
clonedRecipe.Difficulty = RecipeDifficulty.Hard;

// Clone a step
var originalStep = new Step(1, "Mix dough", "Combine ingredients", 15);
var clonedStep = (Step)originalStep.Clone();

// Polymorphic cloning
BaseEntity entity = new Recipe(...);
BaseEntity cloned = (BaseEntity)entity.Clone();
```

### Working with Ingredients and Images

```csharp
// Create ingredient
var ingredient = new Ingredient("250g", "Flour");
Console.WriteLine(ingredient.ToString()); // "250g Flour"

// Create image
byte[] imageData = File.ReadAllBytes("recipe.jpg");
var image = new Image("recipe.jpg", imageData);

Console.WriteLine(image.GetFormattedSize()); // "512 KB"
Console.WriteLine(image.GetSizeInBytes());   // 524288

// Add to recipe
recipe.Images.Add(image);
```

---

## 2. Using Services

### Getting the Service (Creator Pattern)

```csharp
using RecipeMan.Services;

// Use ServiceFactory to get service instance (Creator Pattern)
IRecipeService recipeService = ServiceFactory.GetRecipeService();
```

### CRUD Operations (Controller Pattern)

```csharp
// CREATE
var newRecipe = new Recipe("Lasagna", "Italian", RecipeDifficulty.Hard, "Layered pasta");
newRecipe.Steps.Add(new Step(1, "Make sauce", "Cook meat sauce", 30));

try
{
    var created = await recipeService.CreateRecipeAsync(newRecipe);
    Console.WriteLine($"Created recipe with ID: {created.Id}");
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to create: {ex.Message}");
}

// READ ALL
try
{
    var allRecipes = await recipeService.GetAllRecipesAsync();
    foreach (var recipe in allRecipes)
    {
        Console.WriteLine($"{recipe.Name} - {recipe.CalculateTotalDuration()} min");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to load: {ex.Message}");
}

// READ BY ID
try
{
    var recipe = await recipeService.GetRecipeByIdAsync(1);
    Console.WriteLine($"Loaded: {recipe.Name}");
}
catch (Exception ex)
{
    Console.WriteLine($"Not found: {ex.Message}");
}

// UPDATE
var existingRecipe = await recipeService.GetRecipeByIdAsync(1);
existingRecipe.Name = "Updated Name";
existingRecipe.Description = "New description";

try
{
    await recipeService.UpdateRecipeAsync(existingRecipe);
    Console.WriteLine("Updated successfully");
}
catch (Exception ex)
{
    Console.WriteLine($"Update failed: {ex.Message}");
}

// DELETE
try
{
    await recipeService.DeleteRecipeAsync(1);
    Console.WriteLine("Deleted successfully");
}
catch (Exception ex)
{
    Console.WriteLine($"Delete failed: {ex.Message}");
}

// GET ID BY NAME
int recipeId = await recipeService.GetRecipeIdByNameAsync("Lasagna");
if (recipeId > 0)
{
    Console.WriteLine($"Recipe ID: {recipeId}");
}
else
{
    Console.WriteLine("Recipe not found");
}
```

---

## 3. Extending the Architecture

### Adding a Mock Repository (Polymorphism)

```csharp
// Create a mock repository for testing
public class MockRecipeRepository : IRecipeRepository
{
    private readonly List<Recipe> _recipes = new List<Recipe>();
    private int _nextId = 1;

    public Task<IReadOnlyList<Recipe>> GetAllAsync()
    {
        return Task.FromResult<IReadOnlyList<Recipe>>(_recipes.ToList());
    }

    public Task<Recipe> GetByIdAsync(int id)
    {
        var recipe = _recipes.FirstOrDefault(r => r.Id == id);
        if (recipe == null)
            throw new Exception("Recipe not found");
        return Task.FromResult(recipe);
    }

    public Task<Recipe> CreateAsync(Recipe recipe)
    {
        recipe.Id = _nextId++;
        _recipes.Add((Recipe)recipe.Clone());
        return Task.FromResult(recipe);
    }

    public Task UpdateAsync(Recipe recipe)
    {
        var existing = _recipes.FirstOrDefault(r => r.Id == recipe.Id);
        if (existing == null)
            throw new Exception("Recipe not found");
            
        _recipes.Remove(existing);
        _recipes.Add((Recipe)recipe.Clone());
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var recipe = _recipes.FirstOrDefault(r => r.Id == id);
        if (recipe != null)
            _recipes.Remove(recipe);
        return Task.CompletedTask;
    }
}

// Usage - just change the factory:
public static class ServiceFactory
{
    public static IRecipeService GetRecipeService()
    {
        // var repository = new RecipeApiRepository(); // Production
        var repository = new MockRecipeRepository();   // Testing
        return new RecipeService(repository);
    }
}
```

### Adding a Database Repository

```csharp
public class RecipeDatabaseRepository : IRecipeRepository
{
    private readonly string _connectionString;

    public RecipeDatabaseRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IReadOnlyList<Recipe>> GetAllAsync()
    {
        // Implement database logic
        using (var connection = new SqlConnection(_connectionString))
        {
            // SQL query to get recipes
            // Map from database to domain models
        }
    }

    // Implement other methods...
}

// Update factory:
public static IRecipeService GetRecipeService()
{
    var connectionString = ConfigurationManager.ConnectionStrings["RecipeDB"].ConnectionString;
    var repository = new RecipeDatabaseRepository(connectionString);
    return new RecipeService(repository);
}
```

### Adding a New Domain Entity

```csharp
// Example: Adding a Review entity
public class Review : BaseEntity
{
    public int RecipeId { get; set; }
    public string AuthorName { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedDate { get; set; }

    // Information Expert: Review validates itself
    public ValidationResult Validate()
    {
        if (Rating < 1 || Rating > 5)
            return ValidationResult.Failure("Rating must be between 1 and 5");
            
        if (string.IsNullOrWhiteSpace(Comment))
            return ValidationResult.Failure("Comment is required");
            
        return ValidationResult.Success();
    }

    // Inheritance: Implements Clone from BaseEntity
    public override object Clone()
    {
        return new Review
        {
            Id = this.Id,
            RecipeId = this.RecipeId,
            AuthorName = this.AuthorName,
            Rating = this.Rating,
            Comment = this.Comment,
            CreatedDate = this.CreatedDate
        };
    }
}

// Add to Recipe
public class Recipe : BaseEntity
{
    // ... existing properties ...
    public List<Review> Reviews { get; set; } = new List<Review>();
    
    // Information Expert: Recipe calculates average rating
    public double GetAverageRating()
    {
        if (Reviews == null || Reviews.Count == 0)
            return 0;
        return Reviews.Average(r => r.Rating);
    }
}
```

---

## 4. Testing Examples

### Unit Testing Domain Models

```csharp
[TestClass]
public class RecipeTests
{
    [TestMethod]
    public void CalculateTotalDuration_WithSteps_ReturnsCorrectSum()
    {
        // Arrange
        var recipe = new Recipe("Test", "Test", RecipeDifficulty.Easy, "Test");
        recipe.Steps.Add(new Step(1, "Step 1", "Desc", 10));
        recipe.Steps.Add(new Step(2, "Step 2", "Desc", 15));
        recipe.Steps.Add(new Step(3, "Step 3", "Desc", 5));

        // Act
        int totalDuration = recipe.CalculateTotalDuration();

        // Assert
        Assert.AreEqual(30, totalDuration);
    }

    [TestMethod]
    public void Validate_WithoutName_ReturnsFailed()
    {
        // Arrange
        var recipe = new Recipe("", "Italian", RecipeDifficulty.Easy, "Desc");

        // Act
        var result = recipe.Validate();

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual("Recipe name is required", result.Message);
    }

    [TestMethod]
    public void Clone_CreatesDeepCopy()
    {
        // Arrange
        var original = new Recipe("Original", "Cat", RecipeDifficulty.Easy, "Desc");
        original.Steps.Add(new Step(1, "Step", "Desc", 10));

        // Act
        var cloned = (Recipe)original.Clone();
        cloned.Name = "Cloned";
        cloned.Steps[0].Title = "Modified Step";

        // Assert
        Assert.AreEqual("Original", original.Name);
        Assert.AreEqual("Step", original.Steps[0].Title);
        Assert.AreNotSame(original, cloned);
    }
}
```

### Unit Testing Service Layer

```csharp
[TestClass]
public class RecipeServiceTests
{
    private IRecipeRepository _mockRepository;
    private RecipeService _service;

    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new MockRecipeRepository();
        _service = new RecipeService(_mockRepository);
    }

    [TestMethod]
    public async Task CreateRecipeAsync_ValidRecipe_ReturnsRecipeWithId()
    {
        // Arrange
        var recipe = new Recipe("Test", "Cat", RecipeDifficulty.Easy, "Desc");
        recipe.Steps.Add(new Step(1, "Step", "Desc", 10));

        // Act
        var created = await _service.CreateRecipeAsync(recipe);

        // Assert
        Assert.IsNotNull(created);
        Assert.IsTrue(created.Id > 0);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task CreateRecipeAsync_InvalidRecipe_ThrowsException()
    {
        // Arrange
        var recipe = new Recipe("", "Cat", RecipeDifficulty.Easy, "Desc");

        // Act
        await _service.CreateRecipeAsync(recipe);

        // Assert - expects exception
    }

    [TestMethod]
    public async Task GetAllRecipesAsync_ReturnsAllRecipes()
    {
        // Arrange
        await _service.CreateRecipeAsync(new Recipe("R1", "C", RecipeDifficulty.Easy, "D"));
        await _service.CreateRecipeAsync(new Recipe("R2", "C", RecipeDifficulty.Easy, "D"));

        // Act
        var recipes = await _service.GetAllRecipesAsync();

        // Assert
        Assert.AreEqual(2, recipes.Count);
    }
}
```

---

## 5. Common Patterns

### Pattern: Validation Before Save

```csharp
private async Task<bool> ValidateAndSaveRecipe(Recipe recipe)
{
    // Information Expert: Recipe validates itself
    var validation = recipe.Validate();
    
    if (!validation.IsValid)
    {
        MessageBox.Show(validation.Message, "Validation Error");
        return false;
    }

    // Controller: Service coordinates the save
    try
    {
        await recipeService.CreateRecipeAsync(recipe);
        return true;
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Failed to save: {ex.Message}", "Error");
        return false;
    }
}
```

### Pattern: Safe Data Access

```csharp
private async Task<Recipe> SafeGetRecipe(int id)
{
    try
    {
        return await recipeService.GetRecipeByIdAsync(id);
    }
    catch (RepositoryException ex)
    {
        MessageBox.Show(
            $"Failed to load recipe: {ex.Message}\n\n" +
            "Make sure the RecipeApi is running on https://localhost:44352",
            "API Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error
        );
        return null;
    }
}
```

### Pattern: Working with Clones in UI

```csharp
private async void EditRecipe(Recipe original)
{
    // Clone for editing (doesn't affect original until saved)
    var editableRecipe = (Recipe)original.Clone();
    
    // Show edit dialog
    using (var dialog = new EditRecipeDialog(editableRecipe))
    {
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            // Validate
            var validation = editableRecipe.Validate();
            if (!validation.IsValid)
            {
                MessageBox.Show(validation.Message);
                return;
            }
            
            // Save changes
            try
            {
                await recipeService.UpdateRecipeAsync(editableRecipe);
                // Refresh UI
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Update failed: {ex.Message}");
            }
        }
    }
}
```

### Pattern: Building Complex Recipes

```csharp
public class RecipeBuilder
{
    private Recipe _recipe;

    public RecipeBuilder(string name, string category, RecipeDifficulty difficulty)
    {
        _recipe = new Recipe(name, category, difficulty, "");
    }

    public RecipeBuilder WithDescription(string description)
    {
        _recipe.Description = description;
        return this;
    }

    public RecipeBuilder AddStep(string title, string description, int duration)
    {
        var step = new Step(_recipe.Steps.Count + 1, title, description, duration);
        _recipe.Steps.Add(step);
        return this;
    }

    public RecipeBuilder AddIngredientToLastStep(string quantity, string name)
    {
        if (_recipe.Steps.Count > 0)
        {
            var lastStep = _recipe.Steps[_recipe.Steps.Count - 1];
            lastStep.Ingredients.Add(new Ingredient(quantity, name));
        }
        return this;
    }

    public Recipe Build()
    {
        _recipe.ReorderSteps();
        return _recipe;
    }
}

// Usage:
var recipe = new RecipeBuilder("Pasta", "Italian", RecipeDifficulty.Easy)
    .WithDescription("Quick pasta dish")
    .AddStep("Boil water", "Fill pot with water", 5)
    .AddIngredientToLastStep("2L", "Water")
    .AddStep("Cook pasta", "Add pasta to boiling water", 10)
    .AddIngredientToLastStep("400g", "Spaghetti")
    .Build();
```

---

## 🎓 Key Takeaways

1. **Use domain models for business logic** - Let objects manage their own data (Information Expert)
2. **Use services for coordination** - Let services orchestrate operations (Controller)
3. **Depend on interfaces** - Easy to test and swap implementations (Polymorphism)
4. **Keep classes focused** - Single responsibility per class (High Cohesion)
5. **Minimize dependencies** - Use interfaces to reduce coupling (Low Coupling)
6. **Clone for editing** - Prevent accidental modifications
7. **Validate before operations** - Fail fast with clear messages
8. **Handle errors at UI layer** - MessageBox in forms, not in services

---

*For more details, see REFACTORING_DOCUMENTATION.md*
