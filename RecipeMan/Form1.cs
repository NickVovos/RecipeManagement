using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecipeMan
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnCreateRecipe_Click(object sender, EventArgs e)
        {
            using (var form = new CreateRecipeForm())
            {
                form.ShowDialog(this);
            }
        }

        private void btnEditRecipes_Click(object sender, EventArgs e)
        {
            using (var form = new EditRecipesForm())
            {
                form.ShowDialog(this);
            }
        }

        private void btnViewRecipe_Click(object sender, EventArgs e)
        {
            using (var form = new ExecuteRecipeForm())
            {
                form.ShowDialog(this);
            }
        }
    }
}
