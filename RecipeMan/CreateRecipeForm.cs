using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RecipeMan
{
    public class CreateRecipeForm : Form
    {
        private TextBox txtName;
        private TextBox txtCategory;
        private ComboBox cmbDifficulty;
        private TextBox txtDescription;
        private ListView lvSteps;
        private Button btnAddStep;
        private Button btnEditStep;
        private Button btnRemoveStep;
        private ListView lvRecipeImages;
        private Button btnAddRecipeImage;
        private Button btnRemoveRecipeImage;
        private Button btnSave;
        private Button btnCancel;

        private readonly List<StepData> steps = new List<StepData>();
        private readonly List<ImageData> recipeImages = new List<ImageData>();
        private RecipeData existing;

        public CreateRecipeForm()
        {
            Text = "Create Recipe";
            Width = 900;
            Height = 700;
            StartPosition = FormStartPosition.CenterParent;
            InitializeLayout();
        }

        public CreateRecipeForm(RecipeData recipe) : this()
        {
            Text = "Edit Recipe";
            existing = recipe;
            Populate(recipe);
        }

        private void InitializeLayout()
        {
            var lblName = new Label { Text = "Name", Location = new Point(20, 20), AutoSize = true };
            txtName = new TextBox { Location = new Point(120, 16), Width = 250 };

            var lblCategory = new Label { Text = "Category", Location = new Point(20, 55), AutoSize = true };
            txtCategory = new TextBox { Location = new Point(120, 51), Width = 250 };

            var lblDifficulty = new Label { Text = "Difficulty", Location = new Point(20, 90), AutoSize = true };
            cmbDifficulty = new ComboBox { Location = new Point(120, 86), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbDifficulty.Items.AddRange(new object[] { Difficulty.Easy, Difficulty.Medium, Difficulty.Hard });
            cmbDifficulty.SelectedIndex = 0;

            var lblDescription = new Label { Text = "Description", Location = new Point(20, 125), AutoSize = true };
            txtDescription = new TextBox { Location = new Point(120, 121), Width = 740, Height = 80, Multiline = true, ScrollBars = ScrollBars.Vertical };

            var lblRecipeImages = new Label { Text = "Recipe Images", Location = new Point(20, 220), AutoSize = true };
            lvRecipeImages = new ListView { Location = new Point(120, 216), Width = 740, Height = 120, View = View.Details, FullRowSelect = true, GridLines = true };
            lvRecipeImages.Columns.Add("Name", 500);
            lvRecipeImages.Columns.Add("Size", 200);
            btnAddRecipeImage = new Button { Text = "Add Image", Location = new Point(120, 340), Width = 100 };
            btnRemoveRecipeImage = new Button { Text = "Remove", Location = new Point(230, 340), Width = 100 };

            var lblSteps = new Label { Text = "Steps", Location = new Point(20, 380), AutoSize = true };
            lvSteps = new ListView
            {
                Location = new Point(120, 376),
                Width = 740,
                Height = 200,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            lvSteps.Columns.Add("Order", 60);
            lvSteps.Columns.Add("Title", 200);
            lvSteps.Columns.Add("Duration (min)", 120);
            lvSteps.Columns.Add("Ingredients", 240);
            lvSteps.Columns.Add("Images", 100);

            btnAddStep = new Button { Text = "Add Step", Location = new Point(120, 580), Width = 100 };
            btnEditStep = new Button { Text = "Edit Step", Location = new Point(230, 580), Width = 100 };
            btnRemoveStep = new Button { Text = "Remove Step", Location = new Point(340, 580), Width = 100 };
            btnSave = new Button { Text = "Save", Location = new Point(760, 620), Width = 100 };
            btnCancel = new Button { Text = "Cancel", Location = new Point(650, 620), Width = 100 };

            btnAddStep.Click += BtnAddStep_Click;
            btnEditStep.Click += BtnEditStep_Click;
            btnRemoveStep.Click += BtnRemoveStep_Click;
            btnAddRecipeImage.Click += BtnAddRecipeImage_Click;
            btnRemoveRecipeImage.Click += BtnRemoveRecipeImage_Click;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => this.Close();

            Controls.AddRange(new Control[]
            {
                lblName, txtName,
                lblCategory, txtCategory,
                lblDifficulty, cmbDifficulty,
                lblDescription, txtDescription,
                lblRecipeImages, lvRecipeImages, btnAddRecipeImage, btnRemoveRecipeImage,
                lblSteps, lvSteps,
                btnAddStep, btnEditStep, btnRemoveStep,
                btnCancel, btnSave
            });
        }

        private void Populate(RecipeData recipe)
        {
            if (recipe == null) return;
            txtName.Text = recipe.Name;
            txtCategory.Text = recipe.CategoryName;
            cmbDifficulty.SelectedItem = recipe.Difficulty;
            txtDescription.Text = recipe.Description;
            steps.Clear();
            recipeImages.Clear();
            if (recipe.Images != null)
            {
                recipeImages.AddRange(recipe.Images.Select(i => new ImageData { Name = i.Name, Data = i.Data }));
            }
            RefreshRecipeImages();
            if (recipe.Steps != null)
            {
                steps.AddRange(recipe.Steps.Select(s => new StepData
                {
                    Order = s.Order,
                    Title = s.Title,
                    Description = s.Description,
                    Duration = s.Duration,
                    Ingredients = s.Ingredients?.Select(i => new IngredientData { Quantity = i.Quantity, Name = i.Name }).ToList() ?? new List<IngredientData>(),
                    Images = s.Images?.Select(i => new ImageData { Name = i.Name, Data = i.Data }).ToList() ?? new List<ImageData>()
                }));
            }
            for (int i = 0; i < steps.Count; i++) steps[i].Order = i + 1;
            RefreshStepsList();
        }

        private void BtnAddRecipeImage_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog { Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp" };
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                var bytes = File.ReadAllBytes(ofd.FileName);
                recipeImages.Add(new ImageData { Name = Path.GetFileName(ofd.FileName), Data = bytes });
                RefreshRecipeImages();
            }
        }

        private void BtnRemoveRecipeImage_Click(object sender, EventArgs e)
        {
            if (lvRecipeImages.SelectedIndices.Count == 0) return;
            int idx = lvRecipeImages.SelectedIndices[0];
            if (idx >= 0 && idx < recipeImages.Count)
            {
                recipeImages.RemoveAt(idx);
                RefreshRecipeImages();
            }
        }

        private void RefreshRecipeImages()
        {
            lvRecipeImages.Items.Clear();
            foreach (var img in recipeImages)
            {
                var item = new ListViewItem(new[] { img.Name, (img.Data?.Length ?? 0).ToString() + " bytes" });
                lvRecipeImages.Items.Add(item);
            }
        }

        private void BtnAddStep_Click(object sender, EventArgs e)
        {
            using (var dlg = new StepDialog())
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    var step = dlg.GetStep();
                    step.Order = steps.Count + 1;
                    steps.Add(step);
                    RefreshStepsList();
                }
            }
        }

        private void BtnEditStep_Click(object sender, EventArgs e)
        {
            if (lvSteps.SelectedIndices.Count == 0) return;
            int idx = lvSteps.SelectedIndices[0];
            if (idx < 0 || idx >= steps.Count) return;
            var original = steps[idx];
            using (var dlg = new StepDialog(CloneStep(original)))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    var edited = dlg.GetStep();
                    edited.Order = original.Order;
                    steps[idx] = edited;
                    RefreshStepsList();
                }
            }
        }

        private CreateRecipeForm.StepData CloneStep(CreateRecipeForm.StepData s)
        {
            return new StepData
            {
                Order = s.Order,
                Title = s.Title,
                Description = s.Description,
                Duration = s.Duration,
                Ingredients = s.Ingredients?.Select(i => new IngredientData { Quantity = i.Quantity, Name = i.Name }).ToList() ?? new List<IngredientData>(),
                Images = s.Images?.Select(i => new ImageData { Name = i.Name, Data = i.Data }).ToList() ?? new List<ImageData>()
            };
        }

        private void BtnRemoveStep_Click(object sender, EventArgs e)
        {
            if (lvSteps.SelectedIndices.Count == 0) return;
            int idx = lvSteps.SelectedIndices[0];
            if (idx >= 0 && idx < steps.Count)
            {
                steps.RemoveAt(idx);
                for (int i = 0; i < steps.Count; i++) steps[i].Order = i + 1;
                RefreshStepsList();
            }
        }

        private void RefreshStepsList()
        {
            lvSteps.Items.Clear();
            foreach (var s in steps)
            {
                var item = new ListViewItem(new[]
                {
                    s.Order.ToString(),
                    s.Title,
                    s.Duration.ToString(),
                    string.Join(", ", s.Ingredients.Select(i => i.Quantity + " " + i.Name)),
                    (s.Images?.Count ?? 0).ToString()
                });
                lvSteps.Items.Add(item);
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter a recipe name.");
                return;
            }

            var recipe = new RecipeData
            {
                Name = txtName.Text.Trim(),
                CategoryName = txtCategory.Text.Trim(),
                Difficulty = (Difficulty)cmbDifficulty.SelectedItem,
                Description = txtDescription.Text.Trim(),
                Images = recipeImages.ToList(),
                Steps = steps.Select((s, i) => { s.Order = i + 1; return s; }).ToList()
            };

            if (existing == null)
            {
                await RecipeStore.Add(recipe);
            }
            else
            {
                await RecipeStore.Update(existing, recipe);
            }

            var totalDuration = recipe.Steps.Sum(s => s.Duration);
            MessageBox.Show($"Recipe saved: {recipe.Name}\nCategory: {recipe.CategoryName}\nDifficulty: {recipe.Difficulty}\nTotal Duration: {totalDuration} min\nSteps: {recipe.Steps.Count}\nImages: {recipe.Images.Count}");
            DialogResult = DialogResult.OK;
            Close();
        }

        public enum Difficulty { Easy, Medium, Hard }

        public class RecipeData
        {
            public string Name { get; set; }
            public string CategoryName { get; set; }
            public Difficulty Difficulty { get; set; }
            public string Description { get; set; }
            public List<StepData> Steps { get; set; }
            public List<ImageData> Images { get; set; } = new List<ImageData>();
        }

        public class StepData
        {
            public int Order { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public int Duration { get; set; }
            public List<IngredientData> Ingredients { get; set; } = new List<IngredientData>();
            public List<ImageData> Images { get; set; } = new List<ImageData>();
        }

        public class IngredientData
        {
            public string Quantity { get; set; }
            public string Name { get; set; }
        }

        public class ImageData
        {
            public string Name { get; set; }
            public byte[] Data { get; set; }
        }
    }
}
