# Class Diagram - Inheritance & Patterns

## Inheritance Hierarchy

```
┌────────────────────────────────────────────────────┐
│                  <<interface>>                     │
│                   ICloneable                       │
│  + Clone(): object                                 │
└────────────────┬───────────────────────────────────┘
                 │
                 │ implements
                 │
    ┌────────────┴────────────┬──────────────────────┐
    │                         │                      │
┌───▼──────────────────┐      │              ┌───────▼────────┐
│   <<abstract>>       │      │              │  Ingredient    │
│   BaseEntity         │      │              │ (Value Object) │
│ ─────────────────    │      │              │ ──────────────│
│ + Id: int            │      │              │ + Quantity     │
│ ─────────────────    │      │              │ + Name         │
│ + Clone(): object    │      │              │ + ToString()   │
│   <<abstract>>       │      │              │ + Clone()      │
└───┬──────────────────┘      │              └────────────────┘
    │                         │
    │ inherits                │
    │                         │
    ├─────────────────────────┼──────────────────────────┐
    │                         │                          │
┌───▼──────────────────┐  ┌──▼─────────────────┐   ┌────▼──────────────┐
│      Recipe          │  │       Step         │   │      Image        │
│ ───────────────────  │  │ ─────────────────  │   │ ──────────────── │
│ + Name               │  │ + Order            │   │ + Name            │
│ + CategoryName       │  │ + Title            │   │ + Data: byte[]    │
│ + Difficulty         │  │ + Description      │   │ ──────────────── │
│ + Description        │  │ + Duration         │   │ + GetSizeInBytes()│
│ + Steps: List<Step>  │  │ + Ingredients      │   │ + GetFormattedSize│
│ + Images: List<Image>│  │ + Images           │   │ + Clone()         │
│ ───────────────────  │  │ ─────────────────  │   └───────────────────┘
│ GRASP: INFO EXPERT   │  │ GRASP: INFO EXPERT │
│ ───────────────────  │  │ ─────────────────  │
│ + CalculateTotal     │  │ + GetFormatted     │
│   Duration()         │  │   Ingredients()    │
│ + Validate()         │  │ + Clone()          │
│ + ReorderSteps()     │  └────────────────────┘
│ + Clone()            │
└──────────────────────┘
```

---

## Service Layer (GRASP: Controller, Polymorphism)

```
┌─────────────────────────────────────────────────────────────┐
│                    <<interface>>                            │
│                   IRecipeService                            │
│  ─────────────────────────────────────────────────────────  │
│  + GetAllRecipesAsync(): Task<IReadOnlyList<Recipe>>        │
│  + GetRecipeByIdAsync(int): Task<Recipe>                    │
│  + CreateRecipeAsync(Recipe): Task<Recipe>                  │
│  + UpdateRecipeAsync(Recipe): Task                          │
│  + DeleteRecipeAsync(int): Task                             │
│  + GetRecipeIdByNameAsync(string): Task<int>                │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      │ implements
                      │
         ┌────────────▼──────────────┐
         │     RecipeService         │
         │ ────────────────────────  │
         │ GRASP: CONTROLLER         │
         │ GRASP: HIGH COHESION      │
         │ ────────────────────────  │
         │ - _repository             │
         │ - _nameToIdCache          │
         │ ────────────────────────  │
         │ Coordinates:              │
         │   • Validation            │
         │   • Business Rules        │
         │   • Caching               │
         │   • Delegation            │
         └───────────────────────────┘
```

---

## Repository Layer (GRASP: Polymorphism, Protected Variations)

```
┌─────────────────────────────────────────────────────────────┐
│                    <<interface>>                            │
│                  IRecipeRepository                          │
│  ─────────────────────────────────────────────────────────  │
│  + GetAllAsync(): Task<IReadOnlyList<Recipe>>               │
│  + GetByIdAsync(int): Task<Recipe>                          │
│  + CreateAsync(Recipe): Task<Recipe>                        │
│  + UpdateAsync(Recipe): Task                                │
│  + DeleteAsync(int): Task                                   │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      │ implements
                      │
         ┌────────────▼──────────────┐
         │  RecipeApiRepository      │
         │ ────────────────────────  │
         │ GRASP: HIGH COHESION      │
         │ GRASP: LOW COUPLING       │
         │ ────────────────────────  │
         │ Responsibilities:         │
         │   • HTTP calls            │
         │   • DTO mapping           │
         │   • Error wrapping        │
         │ ────────────────────────  │
         │ Uses:                     │
         │   RecipeApiClient         │
         │     (static methods)      │
         └───────────────────────────┘
```

---

## Adapter Layer (GRASP: Protected Variations)

```
┌─────────────────────────────────────────────────────────────┐
│                      RecipeStore                            │
│                  (Static Facade/Adapter)                    │
│  ─────────────────────────────────────────────────────────  │
│  GRASP: PROTECTED VARIATIONS                                │
│  GRASP: LOW COUPLING                                        │
│  ─────────────────────────────────────────────────────────  │
│  - _recipeService: IRecipeService                           │
│  ─────────────────────────────────────────────────────────  │
│  + GetAll(): Task<IReadOnlyList<FormData>>                  │
│  + Add(FormData): Task                                      │
│  + Update(FormData, FormData): Task                         │
│  + Remove(FormData): Task                                   │
│  + Clone(FormData): FormData                                │
│  + GetRecipeId(string): Task<int>                           │
│  ─────────────────────────────────────────────────────────  │
│  Private Helpers:                                           │
│    - ConvertToFormData(Recipe): FormData                    │
│    - ConvertToDomainModel(FormData): Recipe                 │
│  ─────────────────────────────────────────────────────────  │
│  Protects UI from:                                          │
│    • Domain model changes                                   │
│    • Service implementation changes                         │
└─────────────────────────────────────────────────────────────┘
```

---

## Factory (GRASP: Creator)

```
┌─────────────────────────────────────────────────────────────┐
│                    ServiceFactory                           │
│                   (Static Factory)                          │
│  ─────────────────────────────────────────────────────────  │
│  GRASP: CREATOR                                             │
│  ─────────────────────────────────────────────────────────  │
│  - _recipeService: IRecipeService                           │
│  ─────────────────────────────────────────────────────────  │
│  + GetRecipeService(): IRecipeService                       │
│  ─────────────────────────────────────────────────────────  │
│  Creates and configures:                                    │
│    • RecipeApiRepository                                    │
│    • RecipeService                                          │
│  ─────────────────────────────────────────────────────────  │
│  Benefits:                                                  │
│    • Centralized creation                                   │
│    • Easy to swap implementations                           │
│    • Singleton pattern for service                          │
└─────────────────────────────────────────────────────────────┘
```

---

## Full Flow Diagram

```
┌──────────────────┐
│   WinForms UI    │
│  ──────────────  │
│  CreateRecipeForm│
│  EditRecipesForm │
│  ViewRecipeForm  │
└────────┬─────────┘
         │
         │ calls
         │
         ▼
┌────────────────────────┐
│    RecipeStore         │
│    (Adapter)           │
│  Converts:             │
│    FormData ←→ Recipe  │
└────────┬───────────────┘
         │
         │ uses
         │
         ▼
┌────────────────────────┐
│   ServiceFactory       │
│   (Creator)            │
│  Creates services      │
└────────┬───────────────┘
         │
         │ returns
         │
         ▼
┌────────────────────────┐
│   IRecipeService       │◄──────────┐
│   (Interface)          │           │
└────────┬───────────────┘           │
         │                           │
         │ implemented by            │
         │                           │
         ▼                           │
┌────────────────────────┐           │
│   RecipeService        │           │
│   (Controller)         │           │
│  ──────────────────    │           │
│  Coordinates:          │           │
│    • Validation        │───────────┤
│    • Business Rules    │           │ uses
│    • Caching           │           │ domain
│    • Repository calls  │           │ models
└────────┬───────────────┘           │
         │                           │
         │ uses                      │
         │                           │
         ▼                           │
┌────────────────────────┐           │
│  IRecipeRepository     │           │
│  (Interface)           │           │
└────────┬───────────────┘           │
         │                           │
         │ implemented by            │
         │                           │
         ▼                           │
┌────────────────────────┐           │
│ RecipeApiRepository    │           │
│  ──────────────────    │           │
│  • Maps DTOs           │           │
│  • HTTP calls          │           │
│  • Error handling      │           │
└────────┬───────────────┘           │
         │                           │
         │ uses                      │
         │                           │
         ▼                           │
┌────────────────────────┐           │
│  RecipeApiClient       │           │
│  (HTTP Client)         │           │
│  Static methods        │           │
└────────────────────────┘           │
                                     │
         ┌───────────────────────────┘
         │
         ▼
┌────────────────────────┐
│   Domain Models        │
│  ──────────────────    │
│   Recipe               │
│   Step                 │
│   Ingredient           │
│   Image                │
│  ──────────────────    │
│  Each has business     │
│  logic for its data    │
│  (Information Expert)  │
└────────────────────────┘
```

---

## Pattern Application Map

```
┌───────────────────────────────────────────────────────────────┐
│                    GRASP PATTERNS APPLIED                     │
├───────────────────────────────────────────────────────────────┤
│                                                               │
│  1. INFORMATION EXPERT                                        │
│     ↓                                                         │
│     Recipe, Step, Ingredient, Image                           │
│     (Each knows its own business logic)                       │
│                                                               │
│  2. CONTROLLER                                                │
│     ↓                                                         │
│     RecipeService                                             │
│     (Coordinates business operations)                         │
│                                                               │
│  3. LOW COUPLING                                              │
│     ↓                                                         │
│     Interfaces: IRecipeService, IRecipeRepository             │
│     (Depend on abstractions)                                  │
│                                                               │
│  4. HIGH COHESION                                             │
│     ↓                                                         │
│     Each class has single, focused responsibility:            │
│       • RecipeService → Business logic                        │
│       • RecipeApiRepository → Data access                     │
│       • RecipeStore → Adapter                                 │
│       • Forms → UI                                            │
│                                                               │
│  5. POLYMORPHISM                                              │
│     ↓                                                         │
│     IRecipeService, IRecipeRepository, ICloneable             │
│     (Swappable implementations)                               │
│                                                               │
│  6. PROTECTED VARIATIONS                                      │
│     ↓                                                         │
│     Interfaces protect against:                               │
│       • API changes → IRecipeRepository                       │
│       • Domain changes → RecipeStore adapter                  │
│                                                               │
│  7. CREATOR                                                   │
│     ↓                                                         │
│     ServiceFactory                                            │
│     (Centralized object creation)                             │
│                                                               │
└───────────────────────────────────────────────────────────────┘
```

---

## Inheritance Benefits Visualization

```
         BaseEntity (abstract)
         ├── Id: int (SHARED)
         └── Clone() (SHARED PATTERN)
                 │
                 ├─────────────────┬─────────────────┐
                 │                 │                 │
              Recipe             Step              Image
         (Specific logic)  (Specific logic)  (Specific logic)
         
Benefits:
  ✓ Code Reuse - Id property not duplicated
  ✓ Consistency - All entities have Clone()
  ✓ Type Safety - Can use BaseEntity type
  ✓ Polymorphism - Treat all entities uniformly
  ✓ Extensibility - Easy to add new entities
```

---

## Summary of Improvements

```
BEFORE                          AFTER
──────                          ─────

RecipeStore (static)     →     Layered Architecture
  ├── Business logic           ├── RecipeStore (Adapter)
  ├── Data access              ├── RecipeService (Controller)
  ├── UI (MessageBox)          ├── RecipeApiRepository (Data)
  └── Caching                  └── Domain Models (Expert)

Tight Coupling          →      Loose Coupling (Interfaces)
  
Mixed Responsibilities  →      Single Responsibility

No Inheritance          →      BaseEntity Hierarchy

Scattered Logic         →      Information Expert

Static Methods          →      Instance-based Services

No Abstraction          →      Protected Variations
```

---

*Visual representation of inheritance and GRASP patterns implementation*
