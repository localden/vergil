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
        static readonly SqliteConnection Connection = new SqliteConnection(@"DataSource=C:\Users\ddelimarsky\Downloads\nugetdb.db");

        static async Task Main(string[] args)
        {
            Connection.Open();

            var command = Connection.CreateCommand();
            command.CommandText = "PRAGMA journal_mode=WAL;";
            int queryResult = command.ExecuteNonQuery();
            Console.WriteLine($"WAL setup result: {queryResult}");

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
                Connection.Open();

                foreach (var pointer in orderedPages)
                {
                    var currentPage = Regex.Match(pointer.QualifiedId, "(page([0-9]+){1})", RegexOptions.IgnoreCase).Groups[2].Value;

                    try
                    {
                        HttpResponseMessage pageData = await DataClient.GetAsync(pointer.QualifiedId);
                        if (pageData.IsSuccessStatusCode)
                        {
                            var packageReferences = JsonSerializer.Deserialize<Catalog>(await pageData.Content.ReadAsStringAsync(), SerializerOptions);
                            if (packageReferences.Items != null && packageReferences.Items.Any())
                            {
                                Parallel.ForEach(packageReferences.Items, new ParallelOptions { MaxDegreeOfParallelism = 5 }, async packageReference =>
                                {
                                    try
                                    {
                                        HttpResponseMessage packageData = await DataClient.GetAsync(packageReference.QualifiedId);
                                        if (packageData.IsSuccessStatusCode)
                                        {
                                            var packageString = await packageData.Content.ReadAsStringAsync();

                                            if (!string.IsNullOrWhiteSpace(packageString))
                                            {
                                                var command = Connection.CreateCommand();
                                                command.CommandText =
                                                @"
                                            INSERT INTO PackageData (ResponseBody) VALUES ($data)
                                            ";
                                                command.Parameters.AddWithValue("data", packageString);

                                                try
                                                {
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
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine($"[{currentPage}/{maxPage}] Error inserting {packageReference.NuGetId} @ {packageReference.NuGetVersion}. {ex.Message}");
                                                }

                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine($"[{currentPage}/{maxPage}] Package data acquisition not successful for {packageReference.QualifiedId}.");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[{currentPage}/{maxPage}] Failure for {packageReference} acquisition. {ex.Message}");
                                    }

                                });
                            }
                            else
                            {
                                Console.WriteLine($"[{currentPage}/{maxPage}] No packages in {pointer.Id}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[{currentPage}/{maxPage}] Page data acquisition not successful for {pointer.QualifiedId}.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[{currentPage}/{maxPage}] Failure for {pointer.QualifiedId} acquisition. {ex.Message}");
                    }
                }
                return true;
            }

            return false;
        }
    }
}