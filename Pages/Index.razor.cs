using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using WhatsAppLink.Models;
using static MudBlazor.CategoryTypes;
using static WhatsAppLink.Pages.FetchData;


namespace WhatsAppLink.Pages;

public partial class Index
{
    string error = null;
    string phoneNumber = null;
    string name = null;
    string Message = null;
    bool isError = false;
    bool _loading = false;

    List<Country> countries = new List<Country>();
    private Country selectedCountry;

    List<Data> savedData = new List<Data>();
    protected override async Task OnInitializedAsync()
    {               
        try
        {
            if (await LocalStorage.ContainKeyAsync("data"))
            {
                string json = await LocalStorage.GetItemAsStringAsync("data");
                savedData = JsonConvert.DeserializeObject<List<Data>>(json);
            }
        }
        catch (Exception ex)
        {
            error = ex.Message;
            StateHasChanged();
        }

        try
        {
            string url = "https://wal.smalltaskforce.com/CountryCodes.json";
            countries = await Http.GetFromJsonAsync<List<Country>>(url);

        }
        catch (Exception ex)
        {
            error = ex.Message;
            isError = true;
            StateHasChanged();
        }
    }

    string wtsappLink = "#";
    void CreateWhatsAppLink()
    {
        if (selectedCountry != null && !string.IsNullOrWhiteSpace(phoneNumber))
        {
            if (!string.IsNullOrWhiteSpace(Message))
            {
                wtsappLink = "https://wa.me/" + selectedCountry.dial_code + phoneNumber + "?text=" + Message;
            }
            else
            {
                wtsappLink = "https://wa.me/" + selectedCountry.dial_code + phoneNumber;
            }

            SaveData();
        }
    }

    [Inject]
    public ILocalStorageService LocalStorage { get; set; }
    async void SaveData()
    {
        //ObjectId _id = ObjectId.GenerateNewId();
        try
        {
            if(selectedCountry.dial_code != null && !string.IsNullOrWhiteSpace(phoneNumber))
            {
                string _name;

                if(string.IsNullOrWhiteSpace(name))
                {
                    _name = phoneNumber;
                }
                else
                {
                    _name = name;
                }

                // Create a new Data object with the desired values
                Data newData = new Data
                {
                    Id = Guid.NewGuid(),
                    Name = _name,
                    Link = wtsappLink
                };

                if (await LocalStorage.ContainKeyAsync("data"))
                {
                    // Get the existing list of Data objects from local storage
                    string json = await LocalStorage.GetItemAsStringAsync("data");
                    List<Data> dataList = JsonConvert.DeserializeObject<List<Data>>(json);

                    // Add the new Data object to the list
                    dataList.Add(newData);

                    savedData.Add(newData);

                    // Serialize the updated list back into JSON
                    string updatedJson = JsonConvert.SerializeObject(dataList);

                    await LocalStorage.SetItemAsStringAsync("data", updatedJson);
                }
                else
                {
                    // Create a new list with the new Data object and save it to local storage
                    List<Data> dataList = new List<Data> { newData };
                    savedData.Add(newData);

                    string json = JsonConvert.SerializeObject(dataList);
                    await LocalStorage.SetItemAsStringAsync("data", json);

                }

                StateHasChanged();
            }            
        }
        catch ( Exception ex)
        {
            error = ex.Message;
            StateHasChanged();
            isError = true;
        }

    }

    async void CopyLink(string link)
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", link);
        }
        catch (Exception ex)
        {
            error = ex.Message;
            StateHasChanged();
            isError = true;
        }
        
    }

    async void DeleteLink(Data data)
    {
        savedData.Remove(data);

        // Serialize the updated list back into JSON
        string updatedJson = JsonConvert.SerializeObject(savedData);
        await LocalStorage.SetItemAsStringAsync("data", updatedJson);
        StateHasChanged();
    }

    private bool resetValueOnEmptyText;
    private bool coerceText;
    private bool coerceValue;
    private async Task<IEnumerable<Country>> Search(string value)
    {
        if(value == null)
        {
            return await Task.FromResult(countries);
        }

        return await Task.FromResult(countries.Where(x => x.name.Contains(value, StringComparison.OrdinalIgnoreCase)));
    }    
}
