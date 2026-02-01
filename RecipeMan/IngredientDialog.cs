using System;
using System.Drawing;
using System.Windows.Forms;
using Common.DTOs;

namespace RecipeMan
{
    public class IngredientDialog : Form
    {
        private TextBox txtQuantity;
        private TextBox txtName;
        private Button btnOk;
        private Button btnCancel;

        public IngredientDialog()
        {
            Text = "Add Ingredient";
            Width = 400;
            Height = 200;
            StartPosition = FormStartPosition.CenterParent;
            InitializeLayout();
        }

        private void InitializeLayout()
        {
            var lblQuantity = new Label { Text = "Quantity", Location = new System.Drawing.Point(20, 20), AutoSize = true };
            txtQuantity = new TextBox { Location = new System.Drawing.Point(120, 16), Width = 240 };

            var lblName = new Label { Text = "Name", Location = new System.Drawing.Point(20, 55), AutoSize = true };
            txtName = new TextBox { Location = new System.Drawing.Point(120, 51), Width = 240 };

            btnOk = new Button { Text = "OK", Location = new System.Drawing.Point(280, 120), Width = 80 };
            btnCancel = new Button { Text = "Cancel", Location = new System.Drawing.Point(190, 120), Width = 80 };

            btnOk.Click += (s, e) => { DialogResult = DialogResult.OK; Close(); };
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.AddRange(new Control[] { lblQuantity, txtQuantity, lblName, txtName, btnCancel, btnOk });
        }

        public StepIngredientDto GetIngredient()
        {
            return new StepIngredientDto { Quantity = txtQuantity.Text.Trim(), Name = txtName.Text.Trim() };
        }
    }
}
