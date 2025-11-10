using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastReport;
using System.IO;
using Hotel.Models;
using System.Reflection;
using Hotel.Utils;
using Hotel.Localization;

namespace Hotel
{
    public partial class FormReport : Form
    {
        Report report = new Report();
        List<GuestReportDto> listGuests = new List<GuestReportDto>();

        public FormReport()
        {
            InitializeComponent();
            this.Opacity = 0;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void FormReport_Load(object sender, EventArgs e)
        {
            this.Text = Strings.ButtonReport; // (ЗМІНЕНО)

            string dataFileName = "guests_report.xml";
            string reportFileName = "GuestReport.frx";

            try
            {
                if (!File.Exists(reportFileName))
                {
                    // (ЗМІНЕНО)
                    MessageBox.Show($"Файл шаблону звіту не знайдено: {reportFileName}", Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }
                if (!File.Exists(dataFileName))
                {
                    // (ЗМІНЕНО)
                    MessageBox.Show($"Файл даних XML не знайдено: {dataFileName}.", Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                ClassSerializare.DeserializeFromXml<List<GuestReportDto>>(ref listGuests, dataFileName);

                DataTable guestTable = ToDataTable(listGuests);
                guestTable.TableName = "GuestReportDto";

                report.Load(reportFileName);
                report.RegisterData(guestTable, "GuestReportDto");
                report.GetDataSource("GuestReportDto").Enabled = true;
                report.Prepare();
                report.ShowPrepared();
                this.Close();
            }
            catch (Exception ex)
            {
                // (ЗМІНЕНО)
                MessageBox.Show($"Помилка завантаження або відображення звіту: {ex.Message}\n\n{ex.InnerException?.Message}", Strings.ButtonReport, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        public static DataTable ToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        private void FormReport_FormClosed(object sender, FormClosedEventArgs e)
        {
            report?.Dispose();
        }
    }
}