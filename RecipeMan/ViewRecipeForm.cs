using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecipeMan
{
    public class ViewRecipeForm : Form
    {
        private ListBox lbRecipes;
        private Label lblName;
        private Label lblCategory;
        private Label lblDifficulty;
        private TextBox txtDescription;

        // Progress based on durations
        private ProgressBar progressRecipe;
        private Label lblProgress;

        // Recipe images carousel
        private PictureBox pbRecipeImage;
        private Button btnPrevRecipeImg;
        private Button btnNextRecipeImg;
        private Label lblRecipeImgIndex;

        // Step viewer
        private Label lblStepTitle;
        private TextBox txtStepDescription;
        private Label lblStepDuration;
        private Label lblStepIngredients;
        private PictureBox pbStepImage;
        private Button btnPrevStep;
        private Button btnNextStep;
        private Button btnPrevStepImg;
        private Button btnNextStepImg;
        private Label lblStepIndex;
        private Label lblStepImgIndex;

        private CreateRecipeForm.RecipeData current;
        private int currentRecipeImgIndex = 0;
        private int currentStepIndex = 0;
        private int currentStepImgIndex = 0;

        public ViewRecipeForm()
        {
            Text = "View Recipe";
            Width = 1000;
            Height = 750;
            StartPosition = FormStartPosition.CenterParent;
            InitializeLayout();
            this.Load += async (s, e) => await LoadRecipes();
        }

        private void InitializeLayout()
        {
            // Left: recipe list
            lbRecipes = new ListBox { Location = new Point(20, 20), Width = 300, Height = 660 };
            lbRecipes.SelectedIndexChanged += LbRecipes_SelectedIndexChanged;

            // Right: details
            int x = 340;
            lblName = new Label { Location = new Point(x, 20), Width = 600, Font = new Font(FontFamily.GenericSansSerif, 11f, FontStyle.Bold) };
            lblCategory = new Label { Location = new Point(x, 50), Width = 300 };
            lblDifficulty = new Label { Location = new Point(x, 70), Width = 300 };

            txtDescription = new TextBox { Location = new Point(x, 100), Width = 620, Height = 80, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical };

            // Progress bar
            progressRecipe = new ProgressBar { Location = new Point(x, 190), Width = 620, Height = 20, Minimum = 0, Maximum = 100 };
            lblProgress = new Label { Location = new Point(x + 530, 170), Width = 90, TextAlign = ContentAlignment.MiddleRight };

            // Recipe images area
            pbRecipeImage = new PictureBox { Location = new Point(x, 220), Width = 300, Height = 200, BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.Zoom };
            btnPrevRecipeImg = new Button { Text = "<", Location = new Point(x, 430), Width = 40 };
            btnNextRecipeImg = new Button { Text = ">", Location = new Point(x + 260, 430), Width = 40 };
            lblRecipeImgIndex = new Label { Location = new Point(x + 120, 430), Width = 100, TextAlign = ContentAlignment.MiddleCenter };
            btnPrevRecipeImg.Click += (s, e) => { ChangeRecipeImage(-1); };
            btnNextRecipeImg.Click += (s, e) => { ChangeRecipeImage(1); };

            // Step navigation
            lblStepTitle = new Label { Location = new Point(x + 340, 220), Width = 280, Font = new Font(FontFamily.GenericSansSerif, 10f, FontStyle.Bold) };
            lblStepDuration = new Label { Location = new Point(x + 340, 245), Width = 280 };
            lblStepIndex = new Label { Location = new Point(x + 340, 270), Width = 280 };
            txtStepDescription = new TextBox { Location = new Point(x + 340, 295), Width = 280, Height = 125, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical };
            lblStepIngredients = new Label { Location = new Point(x + 340, 425), Width = 280, Height = 40 };
            btnPrevStep = new Button { Text = "Prev Step", Location = new Point(x + 340, 470), Width = 100 };
            btnNextStep = new Button { Text = "Next Step", Location = new Point(x + 520, 470), Width = 100 };
            btnPrevStep.Click += (s, e) => { ChangeStep(-1); };
            btnNextStep.Click += (s, e) => { ChangeStep(1); };

            // Step images carousel
            pbStepImage = new PictureBox { Location = new Point(x + 340, 510), Width = 280, Height = 150, BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.Zoom };
            btnPrevStepImg = new Button { Text = "<", Location = new Point(x + 340, 670), Width = 40 };
            btnNextStepImg = new Button { Text = ">", Location = new Point(x + 580, 670), Width = 40 };
            lblStepImgIndex = new Label { Location = new Point(x + 440, 670), Width = 120, TextAlign = ContentAlignment.MiddleCenter };
            btnPrevStepImg.Click += (s, e) => { ChangeStepImage(-1); };
            btnNextStepImg.Click += (s, e) => { ChangeStepImage(1); };

            Controls.AddRange(new Control[]
            {
                lbRecipes,
                lblName, lblCategory, lblDifficulty, txtDescription,
                lblProgress, progressRecipe,
                pbRecipeImage, btnPrevRecipeImg, btnNextRecipeImg, lblRecipeImgIndex,
                lblStepTitle, lblStepDuration, lblStepIndex, txtStepDescription, lblStepIngredients,
                btnPrevStep, btnNextStep,
                pbStepImage, btnPrevStepImg, btnNextStepImg, lblStepImgIndex
            });
        }

        private async Task LoadRecipes()
        {
            lbRecipes.Items.Clear();
            foreach (var r in await RecipeStore.GetAll())
            {
                lbRecipes.Items.Add(new RecipeListItem(r));
            }
        }

        private async void LbRecipes_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = lbRecipes.SelectedItem as RecipeListItem;
            if (item == null) return;
            
            try
            {
                int recipeId = await RecipeStore.GetRecipeId(item.Data.Name);
                if (recipeId > 0)
                {
                    var fullRecipe = await RecipeApiClient.GetRecipeAsync(recipeId);
                    current = fullRecipe;
                }
                else
                {
                    current = item.Data;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load recipe details: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                current = item.Data;
            }
            
            currentRecipeImgIndex = 0;
            currentStepIndex = 0;
            currentStepImgIndex = 0;
            RenderRecipe();
        }

        private void RenderRecipe()
        {
            if (current == null) return;
            lblName.Text = current.Name;
            lblCategory.Text = "Category: " + current.CategoryName;
            lblDifficulty.Text = "Difficulty: " + current.Difficulty;
            txtDescription.Text = current.Description;
            RenderRecipeImage();
            RenderStep();
            UpdateProgress();
        }

        private void RenderRecipeImage()
        {
            var imgs = current.Images ?? new List<CreateRecipeForm.ImageData>();
            if (imgs.Count == 0)
            {
                pbRecipeImage.Image = null;
                lblRecipeImgIndex.Text = "0/0";
                btnPrevRecipeImg.Enabled = false;
                btnNextRecipeImg.Enabled = false;
                return;
            }
            currentRecipeImgIndex = NormalizeIndex(currentRecipeImgIndex, imgs.Count);
            pbRecipeImage.Image = ByteArrayToImage(imgs[currentRecipeImgIndex].Data);
            lblRecipeImgIndex.Text = $"{currentRecipeImgIndex + 1}/{imgs.Count}";
            btnPrevRecipeImg.Enabled = imgs.Count > 1;
            btnNextRecipeImg.Enabled = imgs.Count > 1;
        }

        private void RenderStep()
        {
            var steps = current.Steps ?? new List<CreateRecipeForm.StepData>();
            if (steps.Count == 0)
            {
                lblStepTitle.Text = "No steps";
                lblStepDuration.Text = string.Empty;
                lblStepIndex.Text = "0/0";
                txtStepDescription.Text = string.Empty;
                lblStepIngredients.Text = string.Empty;
                pbStepImage.Image = null;
                btnPrevStep.Enabled = false;
                btnNextStep.Enabled = false;
                btnPrevStepImg.Enabled = false;
                btnNextStepImg.Enabled = false;
                lblStepImgIndex.Text = "0/0";
                return;
            }

            currentStepIndex = NormalizeIndex(currentStepIndex, steps.Count);
            var step = steps[currentStepIndex];
            lblStepTitle.Text = step.Title;
            lblStepDuration.Text = $"Duration: {step.Duration} min";
            lblStepIndex.Text = $"Step {currentStepIndex + 1}/{steps.Count}";
            txtStepDescription.Text = step.Description;
            lblStepIngredients.Text = "Ingredients: " + string.Join(", ", (step.Ingredients ?? new List<CreateRecipeForm.IngredientData>()).Select(i => i.Quantity + " " + i.Name));

            var imgs = step.Images ?? new List<CreateRecipeForm.ImageData>();
            if (imgs.Count == 0)
            {
                pbStepImage.Image = null;
                lblStepImgIndex.Text = "0/0";
                btnPrevStepImg.Enabled = false;
                btnNextStepImg.Enabled = false;
            }
            else
            {
                currentStepImgIndex = NormalizeIndex(currentStepImgIndex, imgs.Count);
                pbStepImage.Image = ByteArrayToImage(imgs[currentStepImgIndex].Data);
                lblStepImgIndex.Text = $"{currentStepImgIndex + 1}/{imgs.Count}";
                btnPrevStepImg.Enabled = imgs.Count > 1;
                btnNextStepImg.Enabled = imgs.Count > 1;
            }

            btnPrevStep.Enabled = steps.Count > 1;
            btnNextStep.Enabled = steps.Count > 1;
        }

        private void ChangeRecipeImage(int delta)
        {
            if (current == null || current.Images == null || current.Images.Count == 0) return;
            currentRecipeImgIndex = NormalizeIndex(currentRecipeImgIndex + delta, current.Images.Count);
            RenderRecipeImage();
        }

        private void ChangeStep(int delta)
        {
            if (current == null || current.Steps == null || current.Steps.Count == 0) return;
            currentStepIndex = NormalizeIndex(currentStepIndex + delta, current.Steps.Count);
            currentStepImgIndex = 0;
            RenderStep();
            UpdateProgress();
        }

        private void ChangeStepImage(int delta)
        {
            var steps = current?.Steps;
            if (steps == null || steps.Count == 0) return;
            var imgs = steps[currentStepIndex].Images;
            if (imgs == null || imgs.Count == 0) return;
            currentStepImgIndex = NormalizeIndex(currentStepImgIndex + delta, imgs.Count);
            RenderStep();
        }

        private void UpdateProgress()
        {
            var steps = current?.Steps ?? new List<CreateRecipeForm.StepData>();
            var total = steps.Sum(s => Math.Max(0, s.Duration));
            if (total <= 0)
            {
                progressRecipe.Value = 0;
                lblProgress.Text = "0%";
                return;
            }
            // Include the current step's duration so first step shows its percentage and last step reaches 100%
            var done = steps.Take(currentStepIndex + 1).Sum(s => Math.Max(0, s.Duration));
            var percent = (int)Math.Round(done * 100.0 / total);
            percent = Math.Max(0, Math.Min(100, percent));
            progressRecipe.Value = percent;
            lblProgress.Text = percent + "%";
        }

        private static int NormalizeIndex(int index, int count)
        {
            if (count <= 0) return 0;
            if (index < 0) index = count - 1;
            if (index >= count) index = 0;
            return index;
        }

        private static Image ByteArrayToImage(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;
            using (var ms = new System.IO.MemoryStream(bytes))
            {
                return Image.FromStream(ms);
            }
        }

        private class RecipeListItem
        {
            public CreateRecipeForm.RecipeData Data { get; }
            public RecipeListItem(CreateRecipeForm.RecipeData data) { Data = data; }
            public override string ToString()
            {
                var total = Data?.Steps?.Sum(s => s.Duration) ?? 0;
                return $"{Data.Name} - {Data.CategoryName} - {Data.Difficulty} - {total} min";
            }
        }
    }
}
