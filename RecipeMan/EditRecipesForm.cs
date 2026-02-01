using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.DTOs;

namespace RecipeMan
{
    public class EditRecipesForm : Form
    {
        private ListBox lbRecipes;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnClose;

        public EditRecipesForm()
        {
            Text = "Edit Recipes";
            Width = 700;
            Height = 450;
            StartPosition = FormStartPosition.CenterParent;
            InitializeLayout();
  
            this.Load += async (s, e) => await LoadRecipes();  
        }

        private void InitializeLayout()
        {
            lbRecipes = new ListBox { Location = new System.Drawing.Point(20, 20), Width = 640, Height = 320 };
            btnEdit = new Button { Text = "Edit Selected", Location = new System.Drawing.Point(480, 360), Width = 180 };
            btnDelete = new Button { Text = "Delete Selected", Location = new System.Drawing.Point(320, 360), Width = 150 };
            btnClose = new Button { Text = "Close", Location = new System.Drawing.Point(20, 360), Width = 100 };

            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClose.Click += (s, e) => Close();

            Controls.Add(lbRecipes);
            Controls.Add(btnEdit);
            Controls.Add(btnDelete);
            Controls.Add(btnClose);
        }

        private async Task LoadRecipes()
        {
            lbRecipes.Items.Clear();
            foreach (var r in await RecipeStore.GetAll())
            {
                lbRecipes.Items.Add(new RecipeListItem(r));
            }
        }

        private async void BtnEdit_Click(object sender, EventArgs e)
        {
            var item = lbRecipes.SelectedItem as RecipeListItem;
            if (item == null)
            {
                MessageBox.Show("Select a recipe to edit.");
                return;
            }

            var recipeCopy = RecipeStore.Clone(item.Data);
            using (var form = new CreateRecipeForm(recipeCopy))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    await LoadRecipes();
                }
            }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            var item = lbRecipes.SelectedItem as RecipeListItem;
            if (item == null)
            {
                MessageBox.Show("Select a recipe to delete.");
                return;
            }

            var confirm = MessageBox.Show($"Delete recipe '{item.Data.Name}'?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                await RecipeStore.Remove(item.Data);
                await LoadRecipes();
            }
        }

        private class RecipeListItem
        {
            public RecipeDto Data { get; }
            public RecipeListItem(RecipeDto data) { Data = data; }
            public override string ToString()
            {
                var total = Data?.Steps?.Sum(s => s.Duration) ?? 0;
                return $"{Data.Name} - {Data.CategoryName} - {Data.Difficulty} - {total} min";
            }
        }
    }
}
