# RecipeMan Windows Forms - API Integration

## Changes Made

The RecipeMan Windows Forms application has been updated to use the RecipeApi instead of in-memory storage.

## Required Manual Steps

### 1. Add System.Configuration Reference
In Visual Studio:
1. Right-click on the **RecipeMan** project
2. Select **Add** > **Reference**
3. In the Assemblies section, find and check **System.Configuration**
4. Click OK

### 2. Install Newtonsoft.Json NuGet Package
In Visual Studio Package Manager Console:
```powershell
Install-Package Newtonsoft.Json -Version 13.0.3 -ProjectName RecipeMan
```

Or via NuGet Package Manager UI:
1. Right-click on **RecipeMan** project
2. Select **Manage NuGet Packages**
3. Search for "Newtonsoft.Json"
4. Install version 13.0.3

### 3. Add RecipeApiClient.cs to Project
Make sure the file `RecipeMan/RecipeApiClient.cs` is included in the project build.

## How to Use

### 1. Start the API First
Before running RecipeMan, ensure the RecipeApi is running:
1. Set **RecipeApi** as the startup project
2. Press F5 to run it
3. The API should start on `https://localhost:44352`

### 2. Run RecipeMan
1. Set **RecipeMan** as the startup project (or run it from the solution)
2. The Windows Forms app will now connect to the API

### 3. Configure API URL (Optional)
You can change the API base URL in `App.config`:
```xml
<appSettings>
    <add key="RecipeApiBaseUrl" value="https://localhost:44352" />
</appSettings>
```

## Architecture Changes

### New Files
- **RecipeApiClient.cs**: HTTP client for calling the Recipe API endpoints
- **packages.config**: NuGet package configuration
- Updated **App.config**: Added API base URL configuration

### Modified Files
- **RecipeStore.cs**: Now uses RecipeApiClient instead of in-memory list
  - All CRUD operations call the API
  - Includes local caching for better performance
  - Shows error messages if API is unavailable

### API Integration
- **GET /api/recipes**: Load all recipes
- **GET /api/recipes/{id}**: Load single recipe
- **POST /api/recipes**: Create new recipe
- **PUT /api/recipes/{id}**: Update existing recipe
- **DELETE /api/recipes/{id}**: Delete recipe

### Error Handling
The application now shows user-friendly error messages if:
- The API is not running
- Network connection fails
- API returns an error

## Troubleshooting

### "Failed to load recipes from API" Error
**Solution**: Make sure RecipeApi is running on https://localhost:44352

### SSL Certificate Errors
**Solution**: Trust the development certificate:
```powershell
dotnet dev-certs https --trust
```

### Connection Refused
**Solution**: 
1. Check that RecipeApi is running
2. Verify the port in `App.config` matches the API's launch settings
3. Check firewall settings

## Features Preserved
All existing features work the same:
- Create, Edit, Delete recipes
- View recipes with image carousels
- Step-by-step navigation
- Progress tracking
- All forms and dialogs remain unchanged
