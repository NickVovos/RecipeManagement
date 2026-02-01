using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Common.DTOs;

namespace RecipeMan
{
    public class StepDialog : Form
    {
        private TextBox txtTitle;
        private TextBox txtDescription;
        private NumericUpDown numDuration;
        private ListView lvIngredients;
        private Button btnAddIngredient;
        private Button btnRemoveIngredient;
        private ListView lvImages;
        private Button btnAddImage;
        private Button btnRemoveImage;
        private Button btnOk;
        private Button btnCancel;

        private readonly List<StepIngredientDto> ingredients = new List<StepIngredientDto>();
        private readonly List<ImageDto> images = new List<ImageDto>();

        public StepDialog()
        {
            Text = "Add Step";
            Width = 700;
            Height = 600;
            StartPosition = FormStartPosition.CenterParent;
            InitializeLayout();
        }

        public StepDialog(StepDto existing) : this()
        {
            Text = "Edit Step";
            Populate(existing);
        }

        private void InitializeLayout()
        {
            var lblTitle = new Label { Text = "Title", Location = new Point(20, 20), AutoSize = true };
            txtTitle = new TextBox { Location = new Point(120, 16), Width = 530 };

            var lblDescription = new Label { Text = "Description", Location = new Point(20, 55), AutoSize = true };
            txtDescription = new TextBox { Location = new Point(120, 51), Width = 530, Height = 100, Multiline = true, ScrollBars = ScrollBars.Vertical };

            var lblDuration = new Label { Text = "Duration (min)", Location = new Point(20, 165), AutoSize = true };
            numDuration = new NumericUpDown { Location = new Point(120, 161), Width = 100, Minimum = 0, Maximum = 1000 };

            var lblIngredients = new Label { Text = "Ingredients", Location = new Point(20, 200), AutoSize = true };
            lvIngredients = new ListView { Location = new Point(120, 196), Width = 530, Height = 150, View = View.Details, FullRowSelect = true, GridLines = true };
            lvIngredients.Columns.Add("Quantity", 150);
            lvIngredients.Columns.Add("Name", 350);

            btnAddIngredient = new Button { Text = "Add Ingredient", Location = new Point(120, 350), Width = 120 };
            btnRemoveIngredient = new Button { Text = "Remove", Location = new Point(250, 350), Width = 90 };

            var lblImages = new Label { Text = "Step Images", Location = new Point(20, 390), AutoSize = true };
            lvImages = new ListView { Location = new Point(120, 386), Width = 530, Height = 120, View = View.Details, FullRowSelect = true, GridLines = true };
            lvImages.Columns.Add("Name", 360);
            lvImages.Columns.Add("Size", 150);
            btnAddImage = new Button { Text = "Add Image", Location = new Point(120, 510), Width = 100 };
            btnRemoveImage = new Button { Text = "Remove", Location = new Point(230, 510), Width = 100 };

            btnOk = new Button { Text = "OK", Location = new Point(570, 540), Width = 80 };
            btnCancel = new Button { Text = "Cancel", Location = new Point(480, 540), Width = 80 };

            btnAddIngredient.Click += BtnAddIngredient_Click;
            btnRemoveIngredient.Click += BtnRemoveIngredient_Click;
            btnAddImage.Click += BtnAddImage_Click;
            btnRemoveImage.Click += BtnRemoveImage_Click;
            btnOk.Click += (s, e) => { DialogResult = DialogResult.OK; Close(); };
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.AddRange(new Control[]
            {
                lblTitle, txtTitle,
                lblDescription, txtDescription,
                lblDuration, numDuration,
                lblIngredients, lvIngredients,
                btnAddIngredient, btnRemoveIngredient,
                lblImages, lvImages,
                btnAddImage, btnRemoveImage,
                btnCancel, btnOk
            });
        }

        private void Populate(StepDto step)
        {
            if (step == null) return;
            txtTitle.Text = step.Title;
            txtDescription.Text = step.Description;
            numDuration.Value = Math.Min(Math.Max(step.Duration, 0), (int)numDuration.Maximum);
            ingredients.Clear();
            images.Clear();
            if (step.Ingredients != null)
            {
                foreach (var ing in step.Ingredients)
                {
                    ingredients.Add(new StepIngredientDto() { Quantity = ing.Quantity, Name = ing.Name });
                    lvIngredients.Items.Add(new ListViewItem(new[] { ing.Quantity, ing.Name }));
                }
            }
            if (step.Images != null)
            {
                foreach (var img in step.Images)
                {
                    images.Add(new ImageDto { Name = img.Name, Data = img.Data });
                    lvImages.Items.Add(new ListViewItem(new[] { img.Name, (img.Data?.Length ?? 0).ToString() + " bytes" }));
                }
            }
        }

        private void BtnAddIngredient_Click(object sender, EventArgs e)
        {
            using (var dlg = new IngredientDialog())
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    var ing = dlg.GetIngredient();
                    ingredients.Add(ing);
                    var item = new ListViewItem(new[] { ing.Quantity, ing.Name });
                    lvIngredients.Items.Add(item);
                }
            }
        }

        private void BtnRemoveIngredient_Click(object sender, EventArgs e)
        {
            if (lvIngredients.SelectedIndices.Count == 0) return;
            int idx = lvIngredients.SelectedIndices[0];
            if (idx >= 0 && idx < ingredients.Count)
            {
                ingredients.RemoveAt(idx);
                lvIngredients.Items.RemoveAt(idx);
            }
        }

        private void BtnAddImage_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog { Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp" };
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                var bytes = File.ReadAllBytes(ofd.FileName);
                var img = new ImageDto { Name = System.IO.Path.GetFileName(ofd.FileName), Data = bytes };
                images.Add(img);
                var item = new ListViewItem(new[] { img.Name, (img.Data?.Length ?? 0).ToString() + " bytes" });
                lvImages.Items.Add(item);
            }
        }

        private void BtnRemoveImage_Click(object sender, EventArgs e)
        {
            if (lvImages.SelectedIndices.Count == 0) return;
            int idx = lvImages.SelectedIndices[0];
            if (idx >= 0 && idx < images.Count)
            {
                images.RemoveAt(idx);
                lvImages.Items.RemoveAt(idx);
            }
        }

        public StepDto GetStep()
        {
            return new StepDto
            {
                Title = txtTitle.Text.Trim(),
                Description = txtDescription.Text.Trim(),
                Duration = (int)numDuration.Value,
                Ingredients = ingredients.ToList(),
                Images = images.ToList()
            };
        }
    }
}
