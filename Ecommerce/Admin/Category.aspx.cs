using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Ecommerce.Admin
{
    public partial class Category : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMsg.Visible = false;
            getCategories();
        }

        void getCategories()
        {
            con = new SqlConnection(Utils.getConnection());
            cmd = new SqlCommand("CATEGORY_CRUD", con);

            cmd.Parameters.AddWithValue("@ACAO", "GETALL");
            cmd.CommandType = CommandType.StoredProcedure;
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);
            rCategory.DataSource = dt;
            rCategory.DataBind();


           
        }

        protected void btnAddOrUpdate_Click(object sender, EventArgs e)
        {
            string actionName = String.Empty;
            string imagePath = String.Empty;
            string fileExtension = String.Empty;
            bool isValidToExecute = false;
            int categoryId = Convert.ToInt32(hfCategoryId.Value);

            con = new SqlConnection(Utils.getConnection());
            cmd = new SqlCommand("CATEGORY_CRUD", con);

            cmd.Parameters.AddWithValue("@ACAO", categoryId == 0 ? "INSERT" : "UPDATE" );
            cmd.Parameters.AddWithValue("@ID_CATEGORIA", categoryId);
            cmd.Parameters.AddWithValue("@NM_CATEGORIA", txtCategoryName.Text.Trim());          
            cmd.Parameters.AddWithValue("@FL_ATIVA", cbIsActive.Checked);
            if(fuCategoryImage.HasFile)
            {
                if (Utils.isValidExtension(fuCategoryImage.FileName))
                {
                    
                    string newImageName = Utils.getUniqueId();
                    fileExtension = Path.GetExtension(fuCategoryImage.FileName);
                    imagePath = "Images/Category/" + newImageName.ToString() + fileExtension;
                    fuCategoryImage.PostedFile.SaveAs(Server.MapPath("~/Images/Category/") + newImageName.ToString() + fileExtension);
                    cmd.Parameters.AddWithValue("@IMG_URL_CATEGORIA", imagePath);
                    isValidToExecute = true;
                }
                else
                {
                    lblMsg.Visible = false;
                    lblMsg.Text = "Por favor selecione imagem .png, .jpeg ou .jpg" ;
                    lblMsg.CssClass = "alert alert-danger";
                    isValidToExecute = false;
                }
            }
            else
            {
                isValidToExecute = true;
            }

            if (isValidToExecute)
            {
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    actionName = categoryId == 0 ? "Iserida" : "Atualizado";
                    lblMsg.Visible = true;
                    lblMsg.Text = "Categoria " + actionName + " com sucesso";
                    lblMsg.CssClass = "alert alert-danger";
                    getCategories();
                    clear();

                }
                catch (Exception ex)
                {
                    lblMsg.Visible = true; 
                    lblMsg.Text = "Error - " + ex.Message;    
                    lblMsg.CssClass = "alert alert-danger";
                }
                finally
                {
                    con.Close();
                } 
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            clear();
        }

        void clear()
        {
            txtCategoryName.Text = string.Empty;
            cbIsActive.Checked = false;
            hfCategoryId.Value = "0";
            btnAddOrUpdate.Text = "Adicionar";
            imagePreview.ImageUrl = string.Empty;

        }

        protected void rCategory_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lblMsg.Visible = false;
            if(e.CommandName == "edit")
            {
                con = new SqlConnection(Utils.getConnection());
                cmd = new SqlCommand("CATEGORY_CRUD", con);

                cmd.Parameters.AddWithValue("@ACAO", "GETBYID");
                cmd.Parameters.AddWithValue("@ID_CATEGORIA", e.CommandArgument);
                cmd.CommandType = CommandType.StoredProcedure;
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);
                txtCategoryName.Text = dt.Rows[0]["NM_CATEGORIA"].ToString();
                cbIsActive.Checked = Convert.ToBoolean(dt.Rows[0]["FL_ATIVA"]);
                imagePreview.ImageUrl = string.IsNullOrEmpty(dt.Rows[0]["IMG_URL_CATEGORIA"].ToString()) ? "../Images/No_image.png" : "../" + dt.Rows[0]["IMG_URL_CATEGORIA"].ToString();
                imagePreview.Height = 200;
                imagePreview.Width = 200;
                hfCategoryId.Value = dt.Rows[0]["ID_CATEGORIA"].ToString();
                btnAddOrUpdate.Text = "Atualizar";


            }

            if (e.CommandName == "delete")
            {
                con = new SqlConnection(Utils.getConnection());
                cmd = new SqlCommand("CATEGORY_CRUD", con);

                cmd.Parameters.AddWithValue("@ACAO", "DELETE");
                cmd.Parameters.AddWithValue("@ID_CATEGORIA", e.CommandArgument);
                cmd.CommandType = CommandType.StoredProcedure;
                
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery(); 
                    lblMsg.Visible = true;
                    lblMsg.Text = "Categoria deletada com sucesso!";
                    lblMsg.CssClass = "alert alert-danger";
                    getCategories();
                 
                }
                catch(Exception ex)
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Error - " + ex.Message;
                    lblMsg.CssClass = "alert alert-danger";
                }
                finally { con.Close(); }

            }
        }
    }
}