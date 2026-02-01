using RecipeMan.Repository;

namespace RecipeMan.Services
{
    /// <summary>
    /// Simple factory for creating services
    /// In a real app, this would be replaced with DI container
    /// Creator GRASP pattern - centralized creation
    /// </summary>
    public static class ServiceFactory
    {
        private static IRecipeService _recipeService;

        public static IRecipeService GetRecipeService()
        {
            if (_recipeService == null)
            {
                var repository = new RecipeApiRepository();
                _recipeService = new RecipeService(repository);
            }
            return _recipeService;
        }
    }
}
