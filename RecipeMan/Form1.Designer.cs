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
            this.components = new System.ComponentModel.Container();
            this.btnCreateRecipe = new System.Windows.Forms.Button();
            this.btnEditRecipes = new System.Windows.Forms.Button();
            this.btnViewRecipe = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCreateRecipe
            // 
            this.btnCreateRecipe.Location = new System.Drawing.Point(50, 50);
            this.btnCreateRecipe.Name = "btnCreateRecipe";
            this.btnCreateRecipe.Size = new System.Drawing.Size(200, 50);
            this.btnCreateRecipe.TabIndex = 0;
            this.btnCreateRecipe.Text = "Create Recipe";
            this.btnCreateRecipe.UseVisualStyleBackColor = true;
            this.btnCreateRecipe.Click += new System.EventHandler(this.btnCreateRecipe_Click);
            // 
            // btnEditRecipes
            // 
            this.btnEditRecipes.Location = new System.Drawing.Point(50, 120);
            this.btnEditRecipes.Name = "btnEditRecipes";
            this.btnEditRecipes.Size = new System.Drawing.Size(200, 50);
            this.btnEditRecipes.TabIndex = 1;
            this.btnEditRecipes.Text = "Edit Recipes";
            this.btnEditRecipes.UseVisualStyleBackColor = true;
            this.btnEditRecipes.Click += new System.EventHandler(this.btnEditRecipes_Click);
            // 
            // btnViewRecipe
            // 
            this.btnViewRecipe.Location = new System.Drawing.Point(50, 190);
            this.btnViewRecipe.Name = "btnViewRecipe";
            this.btnViewRecipe.Size = new System.Drawing.Size(200, 50);
            this.btnViewRecipe.TabIndex = 2;
            this.btnViewRecipe.Text = "View Recipe";
            this.btnViewRecipe.UseVisualStyleBackColor = true;
            this.btnViewRecipe.Click += new System.EventHandler(this.btnViewRecipe_Click);
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnViewRecipe);
            this.Controls.Add(this.btnEditRecipes);
            this.Controls.Add(this.btnCreateRecipe);
            this.Name = "Form1";
            this.Text = "Recipe Management";
            this.ResumeLayout(false);
        }

        #endregion
    }
}

