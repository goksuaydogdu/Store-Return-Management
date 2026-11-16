using System.ComponentModel;
using System.Globalization;
using MagazaIadeOtomasyonu.Data;
using MagazaIadeOtomasyonu.Models;
using MagazaIadeOtomasyonu.Repositories;

namespace MagazaIadeOtomasyonu.Forms;

public class OperationsForm : Form
{
    private readonly CustomerRepository _customerRepository = new();
    private readonly ReturnRepository _returnRepository = new();
    private readonly DataGridView _customersGrid = new();
    private readonly TextBox _fullNameTextBox = new();
    private readonly TextBox _phoneTextBox = new();
    private readonly TextBox _emailTextBox = new();
    private readonly Button _addButton = new();
    private readonly Button _updateButton = new();
    private readonly Button _deleteButton = new();
    private readonly Button _reportsButton = new();
    private readonly ComboBox _customerComboBox = new();
    private readonly ComboBox _requestTypeComboBox = new();
    private readonly ComboBox _reasonComboBox = new();
    private readonly TextBox _noteTextBox = new();
    private readonly TextBox _itemNameTextBox = new();
    private readonly TextBox _itemBarcodeTextBox = new();
    private readonly TextBox _itemQuantityTextBox = new();
    private readonly TextBox _itemUnitPriceTextBox = new();
    private readonly ComboBox _itemActionComboBox = new();
    private readonly Button _addItemButton = new();
    private readonly Button _removeItemButton = new();
    private readonly Button _createReturnButton = new();
    private readonly Button _approveButton = new();
    private readonly Button _rejectButton = new();
    private readonly Button _completeButton = new();
    private readonly Button _deleteReturnButton = new();
    private readonly DataGridView _returnItemsGrid = new();
    private readonly DataGridView _returnsGrid = new();
    private readonly BindingList<ReturnItem> _pendingItems = new();

    public OperationsForm()
    {
        Text = "İşlemler";
        Width = 800;
        Height = 600;
        InitializeLayout();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!SessionContext.IsAuthenticated)
        {
            MessageBox.Show("Bu formu açmadan önce giriş yapın.", "Erişim Engellendi");
            Close();
            return;
        }

        LoadCustomers();
        LoadReturns();
    }

    private void InitializeLayout()
    {
        var headerPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            Padding = new Padding(10)
        };

        _reportsButton.Text = "Raporlar";
        _reportsButton.Click += (_, _) => OpenReports();
        headerPanel.Controls.Add(_reportsButton);

        var tabs = new TabControl { Dock = DockStyle.Fill };
        var customersTab = new TabPage("Müşteriler");
        var returnsTab = new TabPage("İade / Değişim");

        customersTab.Controls.Add(BuildCustomersSection());
        returnsTab.Controls.Add(BuildReturnsSection());

        tabs.TabPages.Add(customersTab);
        tabs.TabPages.Add(returnsTab);

        Controls.Add(tabs);
        Controls.Add(headerPanel);
    }

    private Control BuildCustomersSection()
    {
        var inputLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 2,
            RowCount = 4,
            Padding = new Padding(10),
            AutoSize = true
        };

        inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        inputLayout.Controls.Add(new Label { Text = "Ad Soyad", AutoSize = true }, 0, 0);
        inputLayout.Controls.Add(_fullNameTextBox, 1, 0);
        inputLayout.Controls.Add(new Label { Text = "Telefon", AutoSize = true }, 0, 1);
        inputLayout.Controls.Add(_phoneTextBox, 1, 1);
        inputLayout.Controls.Add(new Label { Text = "E-posta", AutoSize = true }, 0, 2);
        inputLayout.Controls.Add(_emailTextBox, 1, 2);

        var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
        _addButton.Text = "Ekle";
        _updateButton.Text = "Güncelle";
        _deleteButton.Text = "Sil";

        _addButton.Click += (_, _) => AddCustomer();
        _updateButton.Click += (_, _) => UpdateCustomer();
        _deleteButton.Click += (_, _) => DeleteCustomer();

        buttonPanel.Controls.Add(_addButton);
        buttonPanel.Controls.Add(_updateButton);
        buttonPanel.Controls.Add(_deleteButton);
        inputLayout.Controls.Add(buttonPanel, 1, 3);

        _customersGrid.Dock = DockStyle.Fill;
        _customersGrid.ReadOnly = true;
        _customersGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _customersGrid.MultiSelect = false;
        _customersGrid.AutoGenerateColumns = true;
        _customersGrid.SelectionChanged += (_, _) => FillInputsFromSelection();

        var container = new Panel { Dock = DockStyle.Fill };
        container.Controls.Add(_customersGrid);
        container.Controls.Add(inputLayout);
        return container;
    }

    private void LoadCustomers()
    {
        try
        {
            var customers = _customerRepository.GetAll();
            _customersGrid.DataSource = customers;
            ApplyCustomerGridHeaders();

            _customerComboBox.DataSource = customers;
            _customerComboBox.DisplayMember = "FullName";
            _customerComboBox.ValueMember = "CustomerId";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Yükleme Başarısız");
        }
    }

    private void FillInputsFromSelection()
    {
        if (_customersGrid.CurrentRow?.DataBoundItem is not Customer customer)
        {
            return;
        }

        _fullNameTextBox.Text = customer.FullName;
        _phoneTextBox.Text = customer.Phone;
        _emailTextBox.Text = customer.Email;
    }

    private void AddCustomer()
    {
        var customer = new Customer
        {
            FullName = _fullNameTextBox.Text.Trim(),
            Phone = _phoneTextBox.Text.Trim(),
            Email = _emailTextBox.Text.Trim()
        };

        if (string.IsNullOrWhiteSpace(customer.FullName))
        {
            MessageBox.Show("Ad Soyad zorunludur.", "Doğrulama");
            return;
        }

        try
        {
            _customerRepository.Add(customer);
            LoadCustomers();
            ClearInputs();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ekleme Başarısız");
        }
    }

    private void UpdateCustomer()
    {
        if (_customersGrid.CurrentRow?.DataBoundItem is not Customer selected)
        {
            MessageBox.Show("Önce bir müşteri seçin.", "Doğrulama");
            return;
        }

        selected.FullName = _fullNameTextBox.Text.Trim();
        selected.Phone = _phoneTextBox.Text.Trim();
        selected.Email = _emailTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(selected.FullName))
        {
            MessageBox.Show("Ad Soyad zorunludur.", "Doğrulama");
            return;
        }

        try
        {
            _customerRepository.Update(selected);
            LoadCustomers();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Güncelleme Başarısız");
        }
    }

    private void DeleteCustomer()
    {
        if (_customersGrid.CurrentRow?.DataBoundItem is not Customer selected)
        {
            MessageBox.Show("Önce bir müşteri seçin.", "Doğrulama");
            return;
        }

        try
        {
            _customerRepository.Delete(selected.CustomerId);
            LoadCustomers();
            ClearInputs();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Silme Başarısız");
        }
    }

    private void ClearInputs()
    {
        _fullNameTextBox.Text = string.Empty;
        _phoneTextBox.Text = string.Empty;
        _emailTextBox.Text = string.Empty;
    }

    private Control BuildReturnsSection()
    {
        var returnsContainer = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Horizontal,
            SplitterDistance = 300
        };

        var headerLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 2,
            RowCount = 4,
            Padding = new Padding(10),
            AutoSize = true
        };

        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        _customerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _requestTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _requestTypeComboBox.FormattingEnabled = true;
        _requestTypeComboBox.Items.AddRange(new object[] { "Return", "Exchange" });
        _requestTypeComboBox.Format += (_, e) => e.Value = MapRequestType(e.Value?.ToString());
        _reasonComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _reasonComboBox.FormattingEnabled = true;
        _reasonComboBox.Items.AddRange(new object[] { "Defective", "Wrong Size", "Unwanted", "Other" });
        _reasonComboBox.Format += (_, e) => e.Value = MapReason(e.Value?.ToString());
        _noteTextBox.Multiline = true;
        _noteTextBox.Height = 60;

        headerLayout.Controls.Add(new Label { Text = "Müşteri", AutoSize = true }, 0, 0);
        headerLayout.Controls.Add(_customerComboBox, 1, 0);
        headerLayout.Controls.Add(new Label { Text = "Talep Türü", AutoSize = true }, 0, 1);
        headerLayout.Controls.Add(_requestTypeComboBox, 1, 1);
        headerLayout.Controls.Add(new Label { Text = "Sebep", AutoSize = true }, 0, 2);
        headerLayout.Controls.Add(_reasonComboBox, 1, 2);
        headerLayout.Controls.Add(new Label { Text = "Not", AutoSize = true }, 0, 3);
        headerLayout.Controls.Add(_noteTextBox, 1, 3);

        var itemPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            Padding = new Padding(10),
            WrapContents = true
        };

        _itemNameTextBox.Width = 160;
        _itemBarcodeTextBox.Width = 120;
        _itemQuantityTextBox.Width = 60;
        _itemUnitPriceTextBox.Width = 80;
        _itemActionComboBox.Width = 100;
        _itemActionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _itemActionComboBox.FormattingEnabled = true;
        _itemActionComboBox.Items.AddRange(new object[] { "Refund", "Exchange" });
        _itemActionComboBox.Format += (_, e) => e.Value = MapAction(e.Value?.ToString());

        _addItemButton.Text = "Ürün Ekle";
        _removeItemButton.Text = "Ürün Çıkar";
        _addItemButton.Click += (_, _) => AddReturnItem();
        _removeItemButton.Click += (_, _) => RemoveReturnItem();

        itemPanel.Controls.Add(new Label { Text = "Ürün", AutoSize = true });
        itemPanel.Controls.Add(_itemNameTextBox);
        itemPanel.Controls.Add(new Label { Text = "Barkod", AutoSize = true });
        itemPanel.Controls.Add(_itemBarcodeTextBox);
        itemPanel.Controls.Add(new Label { Text = "Adet", AutoSize = true });
        itemPanel.Controls.Add(_itemQuantityTextBox);
        itemPanel.Controls.Add(new Label { Text = "Birim Fiyat", AutoSize = true });
        itemPanel.Controls.Add(_itemUnitPriceTextBox);
        itemPanel.Controls.Add(new Label { Text = "İşlem", AutoSize = true });
        itemPanel.Controls.Add(_itemActionComboBox);
        itemPanel.Controls.Add(_addItemButton);
        itemPanel.Controls.Add(_removeItemButton);

        _returnItemsGrid.Dock = DockStyle.Fill;
        _returnItemsGrid.ReadOnly = true;
        _returnItemsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _returnItemsGrid.MultiSelect = false;
        _returnItemsGrid.AutoGenerateColumns = true;
        _returnItemsGrid.DataSource = _pendingItems;
        _returnItemsGrid.CellFormatting += ReturnItemsGridCellFormatting;
        _returnItemsGrid.DataBindingComplete += (_, _) => ApplyReturnItemsGridHeaders();

        var createButtonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            Padding = new Padding(10)
        };

        _createReturnButton.Text = "Talep Oluştur";
        _createReturnButton.Click += (_, _) => CreateReturn();
        createButtonPanel.Controls.Add(_createReturnButton);

        var createPanel = new Panel { Dock = DockStyle.Fill };
        createPanel.Controls.Add(_returnItemsGrid);
        createPanel.Controls.Add(createButtonPanel);
        createPanel.Controls.Add(itemPanel);
        createPanel.Controls.Add(headerLayout);

        returnsContainer.Panel1.Controls.Add(createPanel);

        var statusPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true
        };

        _approveButton.Text = "Onayla";
        _rejectButton.Text = "Reddet";
        _completeButton.Text = "Tamamla";
        _deleteReturnButton.Text = "Sil";

        _approveButton.Click += (_, _) => UpdateReturnStatus("Approved");
        _rejectButton.Click += (_, _) => UpdateReturnStatus("Rejected");
        _completeButton.Click += (_, _) => CompleteReturn();
        _deleteReturnButton.Click += (_, _) => DeleteReturn();

        statusPanel.Controls.Add(_approveButton);
        statusPanel.Controls.Add(_rejectButton);
        statusPanel.Controls.Add(_completeButton);
        statusPanel.Controls.Add(_deleteReturnButton);

        _returnsGrid.Dock = DockStyle.Fill;
        _returnsGrid.ReadOnly = true;
        _returnsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _returnsGrid.MultiSelect = false;
        _returnsGrid.AutoGenerateColumns = true;
        _returnsGrid.CellFormatting += ReturnsGridCellFormatting;

        var listPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        listPanel.Controls.Add(_returnsGrid);
        listPanel.Controls.Add(statusPanel);

        returnsContainer.Panel2.Controls.Add(listPanel);

        return returnsContainer;
    }

    private void AddReturnItem()
    {
        var productName = _itemNameTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(productName))
        {
            MessageBox.Show("Ürün adı zorunludur.", "Doğrulama");
            return;
        }

        if (!int.TryParse(_itemQuantityTextBox.Text.Trim(), out var quantity) || quantity <= 0)
        {
            MessageBox.Show("Adet pozitif bir sayı olmalıdır.", "Doğrulama");
            return;
        }

        if (!decimal.TryParse(_itemUnitPriceTextBox.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out var unitPrice))
        {
            unitPrice = 0m;
        }

        var action = _itemActionComboBox.SelectedItem as string ?? string.Empty;
        if (string.IsNullOrWhiteSpace(action))
        {
            MessageBox.Show("Bir işlem seçin.", "Doğrulama");
            return;
        }

        _pendingItems.Add(new ReturnItem
        {
            ProductName = productName,
            Barcode = _itemBarcodeTextBox.Text.Trim(),
            Quantity = quantity,
            UnitPrice = unitPrice,
            Action = action
        });

        ClearItemInputs();
    }

    private void RemoveReturnItem()
    {
        if (_returnItemsGrid.CurrentRow?.DataBoundItem is not ReturnItem item)
        {
            return;
        }

        _pendingItems.Remove(item);
    }

    private void CreateReturn()
    {
        if (_customerComboBox.SelectedItem is not Customer customer)
        {
            MessageBox.Show("Bir müşteri seçin.", "Doğrulama");
            return;
        }

        var requestType = _requestTypeComboBox.SelectedItem as string ?? string.Empty;
        var reason = _reasonComboBox.SelectedItem as string ?? string.Empty;

        if (string.IsNullOrWhiteSpace(requestType) || string.IsNullOrWhiteSpace(reason))
        {
            MessageBox.Show("Talep türü ve sebep seçin.", "Doğrulama");
            return;
        }

        if (_pendingItems.Count == 0)
        {
            MessageBox.Show("En az bir ürün ekleyin.", "Doğrulama");
            return;
        }

        if (SessionContext.UserId == 0)
        {
            MessageBox.Show("Kullanıcı oturumu bulunamadı.", "Doğrulama");
            return;
        }

        var header = new Return
        {
            CustomerId = customer.CustomerId,
            CreatedByUserId = SessionContext.UserId,
            RequestType = requestType,
            Status = "Pending",
            Reason = reason,
            RequestDate = DateTime.Now,
            Note = _noteTextBox.Text.Trim()
        };

        try
        {
            _returnRepository.CreateReturn(header, _pendingItems.ToList());
            _pendingItems.Clear();
            ClearReturnInputs();
            LoadReturns();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Talep Oluşturma Başarısız");
        }
    }

    private void LoadReturns()
    {
        try
        {
            _returnsGrid.DataSource = _returnRepository.GetReturns();
            ApplyReturnsGridHeaders();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Yükleme Başarısız");
        }
    }

    private void UpdateReturnStatus(string status)
    {
        if (_returnsGrid.CurrentRow?.DataBoundItem is not Return selected)
        {
            MessageBox.Show("Önce bir talep seçin.", "Doğrulama");
            return;
        }

        if (!string.Equals(selected.Status, "Pending", StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show("Yalnızca beklemedeki talepler onaylanabilir veya reddedilebilir.", "Doğrulama");
            return;
        }

        try
        {
            _returnRepository.UpdateStatus(selected.ReturnId, status);
            LoadReturns();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Güncelleme Başarısız");
        }
    }

    private void CompleteReturn()
    {
        if (_returnsGrid.CurrentRow?.DataBoundItem is not Return selected)
        {
            MessageBox.Show("Önce bir talep seçin.", "Doğrulama");
            return;
        }

        if (string.Equals(selected.Status, "Rejected", StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show("Reddedilen talepler tamamlanamaz.", "Doğrulama");
            return;
        }

        if (!string.Equals(selected.Status, "Approved", StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show("Yalnızca onaylanan talepler tamamlanabilir.", "Doğrulama");
            return;
        }

        try
        {
            _returnRepository.UpdateStatus(selected.ReturnId, "Completed");
            LoadReturns();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Güncelleme Başarısız");
        }
    }

    private void DeleteReturn()
    {
        if (_returnsGrid.CurrentRow?.DataBoundItem is not Return selected)
        {
            MessageBox.Show("Önce bir talep seçin.", "Doğrulama");
            return;
        }

        try
        {
            _returnRepository.DeleteReturn(selected.ReturnId);
            LoadReturns();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Silme Başarısız");
        }
    }

    private void ClearItemInputs()
    {
        _itemNameTextBox.Text = string.Empty;
        _itemBarcodeTextBox.Text = string.Empty;
        _itemQuantityTextBox.Text = string.Empty;
        _itemUnitPriceTextBox.Text = string.Empty;
        _itemActionComboBox.SelectedIndex = -1;
    }

    private void ClearReturnInputs()
    {
        _requestTypeComboBox.SelectedIndex = -1;
        _reasonComboBox.SelectedIndex = -1;
        _noteTextBox.Text = string.Empty;
        ClearItemInputs();
    }

    private void OpenReports()
    {
        using var reportsForm = new ReportsForm();
        reportsForm.ShowDialog(this);
    }

    private void ApplyCustomerGridHeaders()
    {
        if (_customersGrid.Columns[nameof(Customer.CustomerId)] is DataGridViewColumn idColumn)
        {
            idColumn.HeaderText = "Müşteri Id";
        }

        if (_customersGrid.Columns[nameof(Customer.FullName)] is DataGridViewColumn fullNameColumn)
        {
            fullNameColumn.HeaderText = "Ad Soyad";
        }

        if (_customersGrid.Columns[nameof(Customer.Phone)] is DataGridViewColumn phoneColumn)
        {
            phoneColumn.HeaderText = "Telefon";
        }

        if (_customersGrid.Columns[nameof(Customer.Email)] is DataGridViewColumn emailColumn)
        {
            emailColumn.HeaderText = "E-posta";
        }
    }

    private void ApplyReturnsGridHeaders()
    {
        if (_returnsGrid.Columns[nameof(Return.ReturnId)] is DataGridViewColumn idColumn)
        {
            idColumn.HeaderText = "Talep Id";
        }

        if (_returnsGrid.Columns[nameof(Return.CustomerId)] is DataGridViewColumn customerColumn)
        {
            customerColumn.HeaderText = "Müşteri Id";
        }

        if (_returnsGrid.Columns[nameof(Return.CreatedByUserId)] is DataGridViewColumn userColumn)
        {
            userColumn.HeaderText = "Oluşturan";
        }

        if (_returnsGrid.Columns[nameof(Return.RequestType)] is DataGridViewColumn typeColumn)
        {
            typeColumn.HeaderText = "Talep Türü";
        }

        if (_returnsGrid.Columns[nameof(Return.Status)] is DataGridViewColumn statusColumn)
        {
            statusColumn.HeaderText = "Durum";
        }

        if (_returnsGrid.Columns[nameof(Return.Reason)] is DataGridViewColumn reasonColumn)
        {
            reasonColumn.HeaderText = "Sebep";
        }

        if (_returnsGrid.Columns[nameof(Return.RequestDate)] is DataGridViewColumn dateColumn)
        {
            dateColumn.HeaderText = "Tarih";
        }

        if (_returnsGrid.Columns[nameof(Return.Note)] is DataGridViewColumn noteColumn)
        {
            noteColumn.HeaderText = "Not";
        }
    }

    private void ApplyReturnItemsGridHeaders()
    {
        if (_returnItemsGrid.Columns[nameof(ReturnItem.ProductName)] is DataGridViewColumn productColumn)
        {
            productColumn.HeaderText = "Ürün";
        }

        if (_returnItemsGrid.Columns[nameof(ReturnItem.Barcode)] is DataGridViewColumn barcodeColumn)
        {
            barcodeColumn.HeaderText = "Barkod";
        }

        if (_returnItemsGrid.Columns[nameof(ReturnItem.Quantity)] is DataGridViewColumn quantityColumn)
        {
            quantityColumn.HeaderText = "Adet";
        }

        if (_returnItemsGrid.Columns[nameof(ReturnItem.UnitPrice)] is DataGridViewColumn priceColumn)
        {
            priceColumn.HeaderText = "Birim Fiyat";
        }

        if (_returnItemsGrid.Columns[nameof(ReturnItem.Action)] is DataGridViewColumn actionColumn)
        {
            actionColumn.HeaderText = "İşlem";
        }
    }

    private void ReturnsGridCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (_returnsGrid.Columns[e.ColumnIndex].DataPropertyName == nameof(Return.Status))
        {
            e.Value = MapStatus(e.Value?.ToString());
            e.FormattingApplied = true;
        }

        if (_returnsGrid.Columns[e.ColumnIndex].DataPropertyName == nameof(Return.RequestType))
        {
            e.Value = MapRequestType(e.Value?.ToString());
            e.FormattingApplied = true;
        }
    }

    private void ReturnItemsGridCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (_returnItemsGrid.Columns[e.ColumnIndex].DataPropertyName == nameof(ReturnItem.Action))
        {
            e.Value = MapAction(e.Value?.ToString());
            e.FormattingApplied = true;
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

    private static string MapAction(string? value)
    {
        return value switch
        {
            "Refund" => "İade",
            "Exchange" => "Değişim",
            _ => value ?? string.Empty
        };
    }

    private static string MapReason(string? value)
    {
        return value switch
        {
            "Defective" => "Arızalı",
            "Wrong Size" => "Beden Uyumsuz",
            "Unwanted" => "İstenmiyor",
            "Other" => "Diğer",
            _ => value ?? string.Empty
        };
    }
}
