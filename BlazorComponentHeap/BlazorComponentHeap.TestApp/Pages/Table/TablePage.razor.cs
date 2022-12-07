using BlazorComponentHeap.Shared.Enums.Table;
using BlazorComponentHeap.Shared.Models.Table;
using BlazorComponentHeap.TestApp.TestModels;

namespace BlazorComponentHeap.TestApp.Pages.Table;

public partial class TablePage
{
    private List<ColumnInfo> _columns = new();
    private List<UserModel> _users = new();
    private List<string> _selectData = new();

    private int _currentPage = 2;

    protected override void OnInitialized()
    {
        _columns.AddRange(
            new List<ColumnInfo>
            {
            new ColumnInfo
            {
                ColumnName = "First Name",
                PropertyName = "FirstName",
                Width = 25,
                IsPx = false,
                FilterType = ColumnFilterType.MultiSelect,
                MinWidth = 200,
                IsSorted = true
            },
            new ColumnInfo
            {
                ColumnName = "Last Name",
                PropertyName = "LastName",
                Width = 25,
                IsPx = false,
                FilterType = ColumnFilterType.NumberSearch,
                MinWidth = 200,
                IsSorted = true
            },
            new ColumnInfo
            {
                ColumnName = "Email",
                PropertyName = "Email",
                Width = 25,
                IsPx = false,
                FilterType = ColumnFilterType.Select,
                MinWidth = 200,
                IsSorted = true
            },
            new ColumnInfo
            {
                ColumnName = "BirthDay",
                PropertyName = "BirthDay",
                Width = 25,
                IsPx = false,
                FilterType = ColumnFilterType.Date,
                MinWidth = 200,
                IsSorted = true
            },
            new ColumnInfo
            {
                ColumnName = "Period",
                PropertyName = "StartDate",
                Width = 25,
                IsPx = false,
                FilterType = ColumnFilterType.DateRange,
                MinWidth = 200,
                IsSorted = true
            },
            new ColumnInfo
            {
                ColumnName = "Actions",
                PropertyName = "Actions",
                Width = 25,
                IsPx = false,
                FilterType = ColumnFilterType.TextSearch,
                MinWidth = 200,
                IsSorted = true,
                Buttons = new List<ButtonConfig>
                {
                    new ButtonConfig
                    {
                        Name = "Update",
                        ImgUrl = ""
                    },
                    new ButtonConfig
                    {
                        Name = "Delete",
                        ImgUrl = ""
                    }
                }
            },
            }
       );

        _users.AddRange(
            new List<UserModel>
            {
            new UserModel
            {
                FirstName = "FirstName 1",
                LastName = "LastName 1",
                Email = "email1@gmail.com",
                PhoneNumber = "111111111"
            },
            new UserModel
            {
                FirstName = "FirstName 2",
                LastName = "LastName 2",
                Email = "email2@gmail.com",
                PhoneNumber = "111111111"
            },
            new UserModel
            {
                FirstName = "FirstName 3",
                LastName = "LastName 3",
                Email = "email3@gmail.com",
                PhoneNumber = "111111111"
            },
            new UserModel
            {
                FirstName = "FirstName 1",
                LastName = "LastName 1",
                Email = "email1@gmail.com",
                PhoneNumber = "111111111"
            },
            new UserModel
            {
                FirstName = "FirstName 2",
                LastName = "LastName 2",
                Email = "email2@gmail.com",
                PhoneNumber = "111111111"
            },
            new UserModel
            {
                FirstName = "FirstName 3",
                LastName = "LastName 3",
                Email = "email3@gmail.com",
                PhoneNumber = "111111111"
            },
            new UserModel
            {
                FirstName = "FirstName 1",
                LastName = "LastName 1",
                Email = "email1@gmail.com",
                PhoneNumber = "111111111"
            },
            new UserModel
            {
                FirstName = "FirstName 2",
                LastName = "LastName 2",
                Email = "email2@gmail.com",
                PhoneNumber = "111111111"
            },
            new UserModel
            {
                FirstName = "FirstName 3",
                LastName = "LastName 3",
                Email = "email3@gmail.com",
                PhoneNumber = "111111111"
            },
            new UserModel
            {
                FirstName = "FirstName 1",
                LastName = "LastName 1",
                Email = "email1@gmail.com",
                PhoneNumber = "111111111"
            },
            new UserModel
            {
                FirstName = "FirstName 2",
                LastName = "LastName 2",
                Email = "email2@gmail.com",
                PhoneNumber = "111111111"
            },
            new UserModel
            {
                FirstName = "FirstName 3",
                LastName = "LastName 3",
                Email = "email3@gmail.com",
                PhoneNumber = "111111111"
            },
            }
        );

        _selectData.AddRange(
            new List<string> { "test1", "test2", "test3" }
        );
    }

    private void ClickedByButton(ButtonData<UserModel> buttonData)
    {

    }

    private void FilterData(SelectData value)
    {
        Console.WriteLine(value.PropertyName);

        value.Data.ForEach(item =>
        {
            Console.WriteLine(item);
        });

    }

    private void SortedData(SortedData data)
    {
        Console.WriteLine(data.Value);
        Console.WriteLine(data.PropertyName);
    }

    private void OnPagination(int numberPage)
    {
        Console.WriteLine(numberPage);
    }

    private void OnScrollPagination()
    {
        Console.WriteLine("bottom");
    }
}
