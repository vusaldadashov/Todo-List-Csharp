using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Xml.Linq;
using Todo_List.Models;

namespace Todo_List.Controllers
{
    public class HomeController : Controller
    {   
        private static List<TodoTask> tasks = new List<TodoTask>();
        public static string errorMessage = "";

        public IActionResult Index()
        { 
            try
            {
                string connection_string = "Data Source=Lenovo\\SQLEXPRESS;Initial Catalog=todoappdb;Integrated Security=True";
                using(SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    string sql = "SELECT * FROM tasks";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using(SqlDataReader reader = command.ExecuteReader())
                        {
                            tasks.Clear();
                            while (reader.Read())
                            {
                                TodoTask newTask = new TodoTask();
                                newTask.Id = reader.GetInt32(0);
                                newTask.Title = reader.GetString(1);
                                newTask.Description = reader.GetString(2);
                                newTask.Status = reader.GetString(3);
                                newTask.CreatedDate = reader.GetDateTime(4);
                                newTask.Deadline = reader.GetDateTime(5);
                                tasks.Add(newTask);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);

            }
            ViewBag.errorMessage = errorMessage;
            return View(tasks);
        }

        public IActionResult CreateNewTask(string Title, string Description, string Status, DateTime Deadline)
        {
            if(string.IsNullOrEmpty(Title) || string.IsNullOrEmpty(Description))
            {
                errorMessage = "All the fields are required";
                return RedirectToAction("Index");
            }
            TodoTask newTodoTask = new TodoTask()
            { 
                Id = tasks.Count,
                Title = Title,
                Description = Description,
                Status = Status,
                CreatedDate = DateTime.Now,
                Deadline = Deadline,
            };
            errorMessage = "";
            try
            {
                string connection_string = "Data Source=Lenovo\\SQLEXPRESS;Initial Catalog=todoappdb;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    string sql = "INSERT INTO tasks(title,description,status, deadline) " +
                        "VALUES(@title, @description, @status, @deadline);";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@title", newTodoTask.Title);
                        command.Parameters.AddWithValue("@description", newTodoTask.Description);
                        command.Parameters.AddWithValue("@status", newTodoTask.Status);
                        command.Parameters.AddWithValue("@deadline", newTodoTask.Deadline);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
            {
                errorMessage = ex.Message;
            }
            return RedirectToAction("Index");
        }
        public IActionResult DeleteTask (int id)
        { 
            try
            {
                string connection_string = "Data Source=Lenovo\\SQLEXPRESS;Initial Catalog=todoappdb;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    string sql = "DELETE FROM tasks WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return RedirectToAction("Index");
        }

        public IActionResult EditTask(int id)
        {
            return View(tasks.Find(task => task.Id == id));
        }

        public IActionResult SaveEditedTask(TodoTask todoTask)
        {
            try
            {
                string connection_string = "Data Source=Lenovo\\SQLEXPRESS;Initial Catalog=todoappdb;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    string sql = "UPDATE tasks " + 
                        "Set title=@title, description=@description," +
                        "status=@status, deadline=@deadline " + 
                        "WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    { 
                        command.Parameters.AddWithValue("@title", todoTask.Title);
                        command.Parameters.AddWithValue("@description", todoTask.Description);
                        command.Parameters.AddWithValue("@status", todoTask.Status);
                        command.Parameters.AddWithValue("@deadline", todoTask.Deadline);
                        command.Parameters.AddWithValue("@id", todoTask.Id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return RedirectToAction("Index");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}