using Microsoft.Data.Sqlite;
using System.Text.Json;
using System.Text.RegularExpressions;
using Vergil.Models;

namespace Vergil
{
    internal class Program
    {
        static readonly string NuGetFullFeedUrl = "https://api.nuget.org/v3/catalog0/index.json";
        static readonly HttpClient DataClient = new();
        static readonly JsonSerializerOptions SerializerOptions = new() { PropertyNameCaseInsensitive = true };

        static async Task Main(string[] args)
        {
            await GetNuGetPackages();
            Console.WriteLine("Finished.");
        }

        static async Task<bool> GetNuGetPackages()
        {
            HttpResponseMessage result = await DataClient.GetAsync(NuGetFullFeedUrl);

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var packagePointers = JsonSerializer.Deserialize<Catalog>(await result.Content.ReadAsStringAsync(), SerializerOptions);
                var orderedPages = packagePointers.Items.OrderBy(x => Convert.ToInt32(Regex.Match(x.QualifiedId, "(page([0-9]+){1})", RegexOptions.IgnoreCase).Groups[2].Value));
                var maxPage = Regex.Match(orderedPages.Last().QualifiedId, "(page([0-9]+){1})", RegexOptions.IgnoreCase).Groups[2].Value;

                foreach (var pointer in orderedPages)
                {
                    var currentPage = Regex.Match(pointer.QualifiedId, "(page([0-9]+){1})", RegexOptions.IgnoreCase).Groups[2].Value;

                    HttpResponseMessage pageData = await DataClient.GetAsync(pointer.QualifiedId);
                    if (pageData.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var packageReferences = JsonSerializer.Deserialize<Catalog>(await pageData.Content.ReadAsStringAsync(), SerializerOptions);
                        if (packageReferences.Items != null && packageReferences.Items.Any())
                        {
                            Parallel.ForEach(packageReferences.Items, async packageReference =>
                            {
                                HttpResponseMessage packageData = await DataClient.GetAsync(packageReference.QualifiedId);
                                var packageString = await packageData.Content.ReadAsStringAsync();

                                if (!string.IsNullOrWhiteSpace(packageString))
                                {
                                    using (var connection = new SqliteConnection(@"DataSource=C:\Users\ddelimarsky\Downloads\nugetdb.db"))
                                    {
                                        connection.Open();

                                        var command = connection.CreateCommand();
                                        command.CommandText =
                                        @"
                                            INSERT INTO PackageData (ResponseBody) VALUES ($data)
                                        ";
                                        command.Parameters.AddWithValue("data", packageString);

                                        int queryResult = command.ExecuteNonQuery();
                                        if (queryResult > 0)
                                        {
                                            Console.WriteLine($"[{currentPage}/{maxPage}] Inserted data for {packageReference.NuGetId} @ {packageReference.NuGetVersion}.");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"[{currentPage}/{maxPage}] Did not insert data for {packageReference.NuGetId} @ {packageReference.NuGetVersion}.");
                                        }
                                    }
                                }
                            });
                        }
                        else
                        {
                            Console.WriteLine($"[{currentPage}/{maxPage}] No packages in {pointer.Id}");
                        }
                    }
                }
                return true;
            }

            return false;
        }
    }
}