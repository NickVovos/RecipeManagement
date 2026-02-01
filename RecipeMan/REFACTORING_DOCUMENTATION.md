# GRASP Patterns & Inheritance Refactoring

## Overview
This document explains the inheritance features and GRASP design patterns implemented in the RecipeManagement application.

---

## 🎯 Inheritance Hierarchy Implemented

### Base Classes

#### 1. **BaseEntity** (Abstract Base Class)
```
BaseEntity (abstract)
├── Recipe
├── Step
└── Image
```

**Purpose**: Provides common identity (Id) and cloning behavior to all entities.

**Benefits**:
- Code reuse through inheritance
- Consistent interface for all domain entities
- Implements `ICloneable` for safe object copying

**Location**: `RecipeMan/Models/BaseEntity.cs`

---

### 2. **Domain Model Classes**

#### Recipe (inherits BaseEntity)
- Represents the complete recipe with metadata, steps, and images
- Contains business logic for validation and duration calculation

#### Step (inherits BaseEntity)
- Represents a single step in a recipe
- Contains ingredients and step-specific images

#### Image (inherits BaseEntity)
- Represents image data with metadata
- Knows how to format its own size

#### Ingredient (Value Object)
- Does NOT inherit from BaseEntity (has no independent identity)
- Implements `ICloneable` directly

**Location**: `RecipeMan/Models/`

---

## 📐 GRASP Design Patterns Implemented

### 1. **Information Expert** ✅

**Principle**: Assign responsibility to the class that has the information necessary to fulfill it.

**Implementation**:

#### Recipe Class - Knows its own business logic:
```csharp
// Recipe calculates its own total duration
public int CalculateTotalDuration()
{
    return Steps?.Sum(s => s.Duration) ?? 0;
}

// Recipe validates itself
public ValidationResult Validate()
{
    if (string.IsNullOrWhiteSpace(Name))
        return ValidationResult.Failure("Recipe name is required");
    // ... more validation
}

// Recipe reorders its own steps
public void ReorderSteps()
{
    for (int i = 0; i < Steps.Count; i++)
        Steps[i].Order = i + 1;
}
```

#### Step Class - Formats its own ingredients:
```csharp
public string GetFormattedIngredients()
{
    return string.Join(", ", Ingredients.Select(i => i.ToString()));
}
```

#### Image Class - Knows its own size:
```csharp
public int GetSizeInBytes()
{
    return Data?.Length ?? 0;
}

public string GetFormattedSize()
{
    // Converts bytes to KB/MB
}
```

**Before**: Logic was scattered in `RecipeStore`, forms, and static methods.
**After**: Each class is responsible for its own data and operations.

---

### 2. **Controller** ✅

**Principle**: Assign responsibility for handling system events to a controller class that coordinates and delegates work.

**Implementation**:

#### RecipeService (Service Layer Controller)
```csharp
public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _repository;
    
    public async Task<Recipe> CreateRecipeAsync(Recipe recipe)
    {
        // Coordinates business operations:
        // 1. Validate
        var validation = recipe.Validate();
        if (!validation.IsValid)
            throw new InvalidOperationException(validation.Message);
            
        // 2. Prepare data
        recipe.ReorderSteps();
        
        // 3. Delegate to repository
        return await _repository.CreateAsync(recipe);
    }
}
```

**Benefits**:
- Centralized business logic coordination
- No duplication across UI forms
- Easy to test and maintain

**Location**: `RecipeMan/Services/RecipeService.cs`

---

### 3. **Low Coupling** ✅

**Principle**: Reduce dependencies between classes. Depend on abstractions, not concrete implementations.

**Implementation**:

#### Interface-based design:
```csharp
// Service depends on abstraction, not implementation
public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _repository; // ← Interface, not class
    
    public RecipeService(IRecipeRepository repository)
    {
        _repository = repository;
    }
}
```

#### RecipeStore acts as adapter:
- Forms depend on `RecipeStore` (stable facade)
- `RecipeStore` depends on `IRecipeService`
- Easy to swap implementations (API, Database, Mock)

**Before**: 
- Forms directly called `RecipeApiClient` static methods
- Tight coupling to HTTP implementation
- Hard to test and maintain

**After**:
- Forms → RecipeStore → IRecipeService → IRecipeRepository → API
- Each layer can be replaced independently

**Location**: All service and repository classes

---

### 4. **High Cohesion** ✅

**Principle**: Keep responsibilities of a class focused and related.

**Implementation**:

#### Separation of Concerns:

**RecipeService** - Business logic only:
- Validation coordination
- Business rules enforcement
- No HTTP, no UI, no data access details

**RecipeApiRepository** - Data access only:
- HTTP calls
- DTO mapping
- No business logic

**RecipeStore** - Adapter only:
- Convert between domain models and form DTOs
- No business logic, no HTTP calls

**Forms** - UI only:
- User interaction
- Display logic
- Error handling with MessageBox (UI concern)

**Before**:
- `RecipeStore` mixed repository + UI (MessageBox) + caching
- Low cohesion

**After**:
- Each class has a single, well-defined responsibility
- High cohesion

---

### 5. **Polymorphism** ✅

**Principle**: Use polymorphic operations to handle alternatives based on type.

**Implementation**:

#### Interface Polymorphism:
```csharp
// Can swap implementations without changing client code
IRecipeService service = new RecipeService(new RecipeApiRepository());
// OR
IRecipeService service = new RecipeService(new MockRepository());
// OR
IRecipeService service = new RecipeService(new DatabaseRepository());

// Client code unchanged:
var recipes = await service.GetAllRecipesAsync();
```

#### ICloneable Polymorphism:
```csharp
public abstract class BaseEntity : ICloneable
{
    public abstract object Clone(); // Each subclass implements
}

// Usage:
BaseEntity entity = new Recipe(...);
BaseEntity cloned = (BaseEntity)entity.Clone(); // Polymorphic call
```

**Benefits**:
- Easy to add new repository implementations
- Testable with mock implementations
- Open/Closed Principle compliance

---

### 6. **Protected Variations** ✅

**Principle**: Protect elements from variations in other elements by wrapping the focus of instability with an interface.

**Implementation**:

#### API instability protected by interfaces:
```csharp
// If API changes, only RecipeApiRepository needs updating
// Service layer and above are protected
IRecipeRepository → RecipeApiRepository (wraps API volatility)
```

#### Domain model protects UI from API changes:
```csharp
// UI uses RecipeStore (adapter)
// RecipeStore converts between domain models and form DTOs
// If domain model changes, only RecipeStore adapters need updating
```

**Location**: `RecipeMan/Repository/IRecipeRepository.cs` and `RecipeMan/RecipeStore.cs`

---

### 7. **Creator** ✅

**Principle**: Assign creation responsibility to the class that has the information to create the object.

**Implementation**:

#### ServiceFactory (Simple Factory Pattern):
```csharp
public static class ServiceFactory
{
    public static IRecipeService GetRecipeService()
    {
        var repository = new RecipeApiRepository();
        return new RecipeService(repository);
    }
}
```

**Benefits**:
- Centralized object creation
- Easy to configure different implementations
- Could be replaced with DI container in future

**Location**: `RecipeMan/Services/ServiceFactory.cs`

---

## 📊 Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                          UI Layer                           │
│  (Forms: CreateRecipeForm, EditRecipesForm, ViewRecipeForm) │
│                    - User Interaction                       │
│                    - Display Logic                          │
│                    - Error Presentation                     │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                     Adapter Layer                           │
│                    RecipeStore                              │
│                - Converts Domain ↔ Form DTOs                │
│                - Facade for UI                              │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                    Service Layer                            │
│                 IRecipeService                              │
│                 RecipeService                               │
│          - Business Logic Coordination                      │
│          - Validation                                       │
│          - Caching                                          │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                   Domain Model Layer                        │
│         Recipe, Step, Ingredient, Image                     │
│         - Business Rules (Information Expert)               │
│         - Validation Logic                                  │
│         - Cloning                                           │
└─────────────────────────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                  Repository Layer                           │
│                IRecipeRepository                            │
│              RecipeApiRepository                            │
│          - Data Access via API                              │
│          - DTO Mapping                                      │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                 Infrastructure                              │
│                RecipeApiClient                              │
│              - HTTP Communication                           │
│              - JSON Serialization                           │
└─────────────────────────────────────────────────────────────┘
```

---

## 🔑 Key Improvements Summary

### Before Refactoring:
❌ No inheritance - code duplication  
❌ Business logic in UI (forms)  
❌ Static RecipeStore with MessageBox calls  
❌ Tight coupling to API  
❌ Hard to test  
❌ Low cohesion - mixed concerns  
❌ Clone logic scattered  

### After Refactoring:
✅ Clean inheritance hierarchy (BaseEntity → Recipe, Step, Image)  
✅ Information Expert - objects know their own data  
✅ Controller pattern - RecipeService coordinates operations  
✅ Low Coupling - interface-based design  
✅ High Cohesion - single responsibility per class  
✅ Polymorphism - swappable implementations  
✅ Protected Variations - stability through interfaces  
✅ Creator - ServiceFactory for object creation  
✅ Cloning via ICloneable interface  
✅ Easy to test (mock repositories)  
✅ Separation of Concerns - UI errors in UI layer  

---

## 🚀 How to Extend

### Adding a New Repository (e.g., Database):
1. Create `RecipeDatabaseRepository : IRecipeRepository`
2. Update `ServiceFactory` to use it
3. No other code changes needed!

### Adding a New Domain Entity:
1. Inherit from `BaseEntity`
2. Implement `Clone()` method
3. Add business logic as methods
4. Follow Information Expert pattern

### Adding New Business Rules:
1. Add to appropriate domain model class
2. Call from `RecipeService` coordination methods
3. Keep UI layer clean

---

## 📚 References

- **GRASP Patterns**: Craig Larman, "Applying UML and Patterns"
- **Inheritance**: Gang of Four, "Design Patterns"
- **Clean Architecture**: Robert C. Martin

---

## ✨ Benefits Achieved

1. **Maintainability**: Clear separation of concerns
2. **Testability**: Interface-based design allows mocking
3. **Flexibility**: Easy to swap implementations
4. **Reusability**: Inheritance reduces duplication
5. **Readability**: Each class has focused responsibility
6. **Scalability**: Easy to extend with new features

---

*Generated as part of GRASP patterns and inheritance refactoring*
