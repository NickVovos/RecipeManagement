using Microsoft.AspNetCore.Mvc;
using RecipeApi.Data;
using RecipeApi.Models;
using System.Collections.Generic;

namespace RecipeApi.Controllers
{
    [ApiController]
    [Route("api/recipes")]
    public class RecipesController : ControllerBase
    {
        private readonly SqlRepository _repo;
        public RecipesController(SqlRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IEnumerable<RecipeDto> GetAll()
        {
            return _repo.GetAllRecipes();
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var recipe = _repo.GetRecipe(id);
            if (recipe == null || recipe.Id == 0) return NotFound();
            return Ok(recipe);
        }

        [HttpPost]
        public IActionResult Create([FromBody]RecipeDto recipe)
        {
            if (recipe == null) return BadRequest("Recipe is required");
            var id = _repo.CreateRecipe(recipe);
            recipe.Id = id;
            return Created($"/api/recipes/{id}", recipe);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody]RecipeDto recipe)
        {
            if (recipe == null || recipe.Id != id) return BadRequest("Invalid recipe");
            _repo.UpdateRecipe(recipe);
            return Ok(recipe);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var success = _repo.DeleteRecipe(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
