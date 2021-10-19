using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace DevTestCore
{
    class Program
    {
        HttpClient client = new HttpClient();

        static async Task Main (string[] args)
        {
            Program program = new Program();
            await program.GetValuesFromAPI();
        }

        private async Task GetValuesFromAPI()
        {
            string response_Passengers = await client.GetStringAsync(
                "https://6165a7fccb73ea0017642166.mockapi.io/api/v1/passengers");            
            List<Passenger> passengerList = new List<Passenger>();
            passengerList = JsonConvert.DeserializeObject<List<Passenger>>(response_Passengers);

            string response_Airlines = await client.GetStringAsync(
               "https://6165a7fccb73ea0017642166.mockapi.io/api/v1/airlines");
            List<Airline> airlinesList = new List<Airline>();
            airlinesList = JsonConvert.DeserializeObject<List<Airline>>(response_Airlines);
            

            // Print airline with the most passengers            
            int maxAirlineId = passengerList.Max(t=> t.airlineId);
            var airlineName = airlinesList.Where(emp => (emp.id == maxAirlineId));
            foreach (var value in airlineName)
                Console.WriteLine(value.name);          


            // Print top 3 airlines (by unflown passengers) and passengers sorted by flight date
            List<Passenger> sortedList = passengerList.OrderByDescending(passenger => passenger.flightDate).ToList();
            List<Passenger> firstThreeAirlines = sortedList.Take(3).ToList();
            foreach (var passenger in firstThreeAirlines)
            {
                var airlineName2 = airlinesList.Where(emp => (emp.id == passenger.airlineId));
                foreach (var value in airlineName2)
                Console.WriteLine(value.name + ": " + passenger.name);
                
            }

            // Create new passenger for the airline with most passengers
            Passenger newPassenger = new Passenger { id = 78, name = "Test Test", airlineId = 10, flightDate= DateTime.Now};
            
            var response = client.PostAsJsonAsync("https://6165a7fccb73ea0017642166.mockapi.io/api/v1/passengers", newPassenger).Result;
            if (response.IsSuccessStatusCode)
            {
                Console.Write("New Passenger is added successfully");
            }
            else
                Console.Write("Something went wrong. Check the Passenger ID and try again.");          

        }

    }
    class Airline
    {
        public int id { get; set; }
        public string name { get; set; }
        
    }
    class Passenger
    {
        public int id { get; set; }
        public string name { get; set; }        
        public int airlineId { get; set; }
        public DateTime flightDate { get; set; }
    }
}

