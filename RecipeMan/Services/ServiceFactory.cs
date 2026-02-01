using RecipeMan.Repository;

namespace RecipeMan.Services
{
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
