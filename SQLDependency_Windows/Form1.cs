using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace SQLDependency_Windows
{
    public partial class Form1 : Form
    {
        private readonly string sampleConnectionString;
        private SqlDependency sampleSqlDependency;
        private SqlCommand sampleSqlCommand;
        private SqlConnection sampleSqlConnection;
        private readonly string notificationStoredProcedure;
        public Form1()
        {
            this.sampleConnectionString = ConfigurationManager.ConnectionStrings["SampleDbConnection"].ConnectionString;
            this.notificationStoredProcedure = "uspGetSampleInformation";
            InitializeComponent();
        }


        private void RegisterDependencyUsingDefaultQueue()
        {
            SqlDependency.Stop(this.sampleConnectionString);
            SqlDependency.Stop(this.sampleConnectionString, "QueueSampleInformationDataChange");
            SqlDependency.Start(this.sampleConnectionString);
            {
                this.ConfigureDependencyUsingStoreProcedureAndDefaultQueue();
            }

        }

        private async void ConfigureDependencyUsingStoreProcedureAndDefaultQueue()
        {
            if (null != this.sampleSqlDependency)
            {
                this.sampleSqlDependency.OnChange -= null;
            }

            if (null != this.sampleSqlCommand)
            {
                this.sampleSqlCommand.Dispose();
            }

            if (null != this.sampleSqlConnection)
            {
                this.sampleSqlConnection.Dispose();
            }

            this.sampleSqlDependency = null;
            this.sampleSqlCommand = null;
            this.sampleSqlConnection = null;

            //// Create connection.
            this.sampleSqlConnection = new SqlConnection(this.sampleConnectionString);

            //// Create command.
            this.sampleSqlCommand = new SqlCommand { Connection = this.sampleSqlConnection };
            this.sampleSqlCommand.CommandType = CommandType.StoredProcedure;
            this.sampleSqlCommand.CommandText = this.notificationStoredProcedure;
            this.sampleSqlCommand.Notification = null;

            //// Create Sql Dependency.
            this.sampleSqlDependency = new SqlDependency(this.sampleSqlCommand);
            this.sampleSqlDependency.OnChange += this.SqlDependencyOnChange;
            await this.sampleSqlCommand.Connection.OpenAsync();
            await this.sampleSqlCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection);

            if (null != this.sampleSqlCommand)
            {
                this.sampleSqlCommand.Dispose();
            }

            if (null != this.sampleSqlConnection)
            {
                this.sampleSqlConnection.Dispose();
            }
        }

        private void SqlDependencyOnChange(object sender, SqlNotificationEventArgs eventArgs)
        {
            if (eventArgs.Info == SqlNotificationInfo.Invalid)
            {
                //Console.WriteLine("The above notification query is not valid.");
            }
            else
            {
                loadData();
            }

            this.ConfigureDependencyUsingStoreProcedureAndDefaultQueue();

        }

        void loadData()
        {
            try
            {
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("select * from Table1", this.sampleConnectionString);
                DataSet ds = new DataSet();
                sqlDataAdapter.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            loadData();
            RegisterDependencyUsingDefaultQueue();
        }

        
    }

}
