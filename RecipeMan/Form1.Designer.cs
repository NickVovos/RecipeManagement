namespace RecipeMan
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Button btnCreateRecipe;
        private System.Windows.Forms.Button btnEditRecipes;
        private System.Windows.Forms.Button btnViewRecipe;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnCreateRecipe = new System.Windows.Forms.Button();
            btnEditRecipes = new System.Windows.Forms.Button();
            btnViewRecipe = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // btnCreateRecipe
            // 
            btnCreateRecipe.Location = new System.Drawing.Point(50, 50);
            btnCreateRecipe.Name = "btnCreateRecipe";
            btnCreateRecipe.Size = new System.Drawing.Size(200, 50);
            btnCreateRecipe.TabIndex = 0;
            btnCreateRecipe.Text = "Create Recipe";
            btnCreateRecipe.UseVisualStyleBackColor = true;
            btnCreateRecipe.Click += btnCreateRecipe_Click;
            // 
            // btnEditRecipes
            // 
            btnEditRecipes.Location = new System.Drawing.Point(50, 120);
            btnEditRecipes.Name = "btnEditRecipes";
            btnEditRecipes.Size = new System.Drawing.Size(200, 50);
            btnEditRecipes.TabIndex = 1;
            btnEditRecipes.Text = "Edit Recipes";
            btnEditRecipes.UseVisualStyleBackColor = true;
            btnEditRecipes.Click += btnEditRecipes_Click;
            // 
            // btnViewRecipe
            // 
            btnViewRecipe.Location = new System.Drawing.Point(50, 190);
            btnViewRecipe.Name = "btnViewRecipe";
            btnViewRecipe.Size = new System.Drawing.Size(200, 50);
            btnViewRecipe.TabIndex = 2;
            btnViewRecipe.Text = "Execute Recipe";
            btnViewRecipe.UseVisualStyleBackColor = true;
            btnViewRecipe.Click += btnViewRecipe_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(btnViewRecipe);
            Controls.Add(btnEditRecipes);
            Controls.Add(btnCreateRecipe);
            Name = "Form1";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Recipe Management";
            ResumeLayout(false);
        }

        #endregion
    }
}

