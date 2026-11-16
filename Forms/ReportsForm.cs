using System.Data;
using Microsoft.Data.SqlClient;
using MagazaIadeOtomasyonu.Data;

namespace MagazaIadeOtomasyonu.Forms;

public class ReportsForm : Form
{
    private readonly DateTimePicker _fromPicker = new();
    private readonly DateTimePicker _toPicker = new();
    private readonly Button _byDateButton = new();
    private readonly Button _topProductsButton = new();
    private readonly Button _topReasonsButton = new();
    private readonly Button _statusCountsButton = new();
    private readonly DataGridView _resultsGrid = new();

    public ReportsForm()
    {
        Text = "Raporlar";
        Width = 800;
        Height = 600;
        BuildUi();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!SessionContext.IsAuthenticated)
        {
            MessageBox.Show("Bu formu açmadan önce giriş yapın.", "Erişim Engellendi");
            Close();
        }
    }

    private void BuildUi()
    {
        var filtersPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 4,
            RowCount = 2,
            Padding = new Padding(10),
            AutoSize = true
        };

        filtersPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        filtersPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
        filtersPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        filtersPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));

        filtersPanel.Controls.Add(new Label { Text = "Başlangıç Tarihi", AutoSize = true }, 0, 0);
        _fromPicker.Format = DateTimePickerFormat.Short;
        filtersPanel.Controls.Add(_fromPicker, 1, 0);

        filtersPanel.Controls.Add(new Label { Text = "Bitiş Tarihi", AutoSize = true }, 2, 0);
        _toPicker.Format = DateTimePickerFormat.Short;
        filtersPanel.Controls.Add(_toPicker, 3, 0);

        var buttonsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            Padding = new Padding(10)
        };

        _byDateButton.Text = "Tarihe Göre Talepler";
        _topProductsButton.Text = "En Çok İade Edilen Ürünler";
        _topReasonsButton.Text = "En Sık İade Sebepleri";
        _statusCountsButton.Text = "Durum Sayıları";

        _byDateButton.Click += (_, _) => LoadReturnsByDate();
        _topProductsButton.Click += (_, _) => LoadTopReturnedProducts();
        _topReasonsButton.Click += (_, _) => LoadTopReturnReasons();
        _statusCountsButton.Click += (_, _) => LoadStatusCounts();

        buttonsPanel.Controls.Add(_byDateButton);
        buttonsPanel.Controls.Add(_topProductsButton);
        buttonsPanel.Controls.Add(_topReasonsButton);
        buttonsPanel.Controls.Add(_statusCountsButton);

        _resultsGrid.Dock = DockStyle.Fill;
        _resultsGrid.ReadOnly = true;
        _resultsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _resultsGrid.MultiSelect = false;
        _resultsGrid.AutoGenerateColumns = true;
        _resultsGrid.CellFormatting += ResultsGridCellFormatting;
        _resultsGrid.DataBindingComplete += (_, _) => ApplyGridHeaders();

        Controls.Add(_resultsGrid);
        Controls.Add(buttonsPanel);
        Controls.Add(filtersPanel);
    }

    private void LoadReturnsByDate()
    {
        var fromDate = _fromPicker.Value.Date;
        var toDate = _toPicker.Value.Date.AddDays(1).AddTicks(-1);

        const string query = @"
SELECT r.ReturnId,
       c.FullName AS CustomerName,
       r.RequestType,
       r.Status,
       r.Reason,
       r.RequestDate
FROM Returns r
INNER JOIN Customers c ON r.CustomerId = c.CustomerId
WHERE r.RequestDate >= @from AND r.RequestDate <= @to
ORDER BY r.ReturnId DESC;";

        var parameters = new[]
        {
            new SqlParameter("@from", SqlDbType.DateTime) { Value = fromDate },
            new SqlParameter("@to", SqlDbType.DateTime) { Value = toDate }
        };

        ExecuteReport(query, parameters);
    }

    private void LoadTopReturnedProducts()
    {
        const string query = @"
SELECT ProductName,
       SUM(Quantity) AS TotalQty
FROM ReturnItems
GROUP BY ProductName
ORDER BY TotalQty DESC;";

        ExecuteReport(query, Array.Empty<SqlParameter>());
    }

    private void LoadTopReturnReasons()
    {
        const string query = @"
SELECT Reason,
       COUNT(*) AS Cnt
FROM Returns
GROUP BY Reason
ORDER BY Cnt DESC;";

        ExecuteReport(query, Array.Empty<SqlParameter>());
    }

    private void LoadStatusCounts()
    {
        const string query = @"
SELECT Status,
       COUNT(*) AS Cnt
FROM Returns
GROUP BY Status
ORDER BY Cnt DESC;";

        ExecuteReport(query, Array.Empty<SqlParameter>());
    }

    private void ExecuteReport(string query, SqlParameter[] parameters)
    {
        try
        {
            using var connection = DbHelper.CreateConnection();
            using var command = new SqlCommand(query, connection);
            if (parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            using var adapter = new SqlDataAdapter(command);
            var table = new DataTable();
            adapter.Fill(table);
            _resultsGrid.DataSource = table;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Yükleme Başarısız");
        }
    }

    private void ResultsGridCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        var columnName = _resultsGrid.Columns[e.ColumnIndex].Name;

        if (string.Equals(columnName, "Status", StringComparison.OrdinalIgnoreCase))
        {
            e.Value = MapStatus(e.Value?.ToString());
            e.FormattingApplied = true;
        }

        if (string.Equals(columnName, "RequestType", StringComparison.OrdinalIgnoreCase))
        {
            e.Value = MapRequestType(e.Value?.ToString());
            e.FormattingApplied = true;
        }
    }

    private void ApplyGridHeaders()
    {
        if (_resultsGrid.Columns["ReturnId"] is DataGridViewColumn returnIdColumn)
        {
            returnIdColumn.HeaderText = "Talep Id";
        }

        if (_resultsGrid.Columns["CustomerName"] is DataGridViewColumn customerColumn)
        {
            customerColumn.HeaderText = "Müşteri";
        }

        if (_resultsGrid.Columns["RequestType"] is DataGridViewColumn typeColumn)
        {
            typeColumn.HeaderText = "Talep Türü";
        }

        if (_resultsGrid.Columns["Status"] is DataGridViewColumn statusColumn)
        {
            statusColumn.HeaderText = "Durum";
        }

        if (_resultsGrid.Columns["Reason"] is DataGridViewColumn reasonColumn)
        {
            reasonColumn.HeaderText = "Sebep";
        }

        if (_resultsGrid.Columns["RequestDate"] is DataGridViewColumn dateColumn)
        {
            dateColumn.HeaderText = "Tarih";
        }

        if (_resultsGrid.Columns["ProductName"] is DataGridViewColumn productColumn)
        {
            productColumn.HeaderText = "Ürün";
        }

        if (_resultsGrid.Columns["TotalQty"] is DataGridViewColumn totalColumn)
        {
            totalColumn.HeaderText = "Toplam Adet";
        }

        if (_resultsGrid.Columns["Cnt"] is DataGridViewColumn countColumn)
        {
            countColumn.HeaderText = "Adet";
        }
    }

    private static string MapStatus(string? value)
    {
        return value switch
        {
            "Pending" => "Beklemede",
            "Approved" => "Onaylandı",
            "Rejected" => "Reddedildi",
            "Completed" => "Tamamlandı",
            _ => value ?? string.Empty
        };
    }

    private static string MapRequestType(string? value)
    {
        return value switch
        {
            "Return" => "İade",
            "Exchange" => "Değişim",
            _ => value ?? string.Empty
        };
    }
}
